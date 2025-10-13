using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace NNS5
{
    public partial class Form1 : Form
    {
        class ForecastNS
        {
            public int layersSize;/* {get{ return layersSize; } set{ layersSize = value; } }*/ // Количество слоев в сети
            public int inputWindowSize; /*{get{ return inputWindowSize; } set{ inputWindowSize = value; } }*/ // Размер окна входных данных
            public const double EPS = 0.1; // Порог ошибки для остановки обучения
            public const double ALPHA = 0.01 ; // Скорость обучения
            public const double IMP_ALPHA = 0.9; // Коэффициент импульса (из лекции)
            private List<Layer> layers; // Список слоев сети

            public ForecastNS(int laySize, int inWinSize)
            {
                layersSize = laySize;
                inputWindowSize = inWinSize;
                layers = new List<Layer>();
                for (int i = 0; i < laySize; i++)
                {
                    int inputSize = (i == 0) ? inWinSize : (laySize - i) * 2 + 1; // Размер входа для каждого слоя
                    int size = (laySize - i - 1) * 2 + 1; // Размер выхода для каждого слоя
                    layers.Add(new Layer(size, inputSize)); // Создание и добавление слоя в сеть
                }
            }

            private class Neuron
            {
                private double value = 0.0; // Значение выхода нейрона
                private double fi = 0.0; // Значение производной функции активации нейрона
                private List<double> deltas = new List<double>(); // Список дельт-весов для корректировки весов
                private List<double> weights = new List<double>(); // Список весов для каждого входа нейрона
                private List<double> innerInputs = new List<double>(); // Список входных значений нейрона

                public Neuron(int inputSize)
                {
                    for (int i = 0; i < inputSize; i++)
                    {
                        deltas.Add(0.0); // Инициализация дельт-весов нулями
                        weights.Add(new Random().NextDouble()); // Инициализация весов случайными числами
                        innerInputs.Add(new Random().NextDouble()); // Инициализация входных значений случайными числами
                    }
                }
                /// <summary>
                /// Функция активации
                /// </summary>
                private double sigmoid(double x)
                {
                    return 1.0 / (1.0 - Math.Pow(Math.E, -x)); // Сигмоидальная функция активации
                }
                /// <summary>
                /// Производна функции активации
                /// </summary>
                private double transfer_sigmoid(double x)
                {
                    return x * (1.0 - x); // Производная сигмоидальной функции активации
                }
                /// <summary>
                /// Метод прямого распространения на уровне нейрона
                /// </summary>
                /// <param name="inputs">Входные значения</param>
                /// <returns>Значение выхода нейрона</returns>
                public double feedForward(List<double> inputs)
                {
                    innerInputs.Clear();
                    innerInputs.AddRange(inputs); // Копирование входных значений из параметра
                    if (innerInputs.Count != weights.Count)
                        throw new Exception("Incorrect input size [Input size " + innerInputs.Count + 
                            " ; Weights size " + weights.Count + "]"); // Проверка соответствия размеров входов и весов
                    double total = 0.0;
                    for (int i = 0; i < innerInputs.Count; i++)
                        total += innerInputs[i] * weights[i]; // Вычисление линейной комбинации входов и весов
                    this.value = sigmoid(total); // Вычисление значения выхода с помощью сигмоидальной функции активации
                    return this.value;
                }
                /// <summary>
                /// Метод коррекции весовых коэффициентов по импульсному дельта-правилу
                /// </summary>
                /// <param name="part">Сумма весов нейрона</param>
                public void correctWeights(double part)
                {
                    fi = transfer_sigmoid(value) * part; // Вычисление производной функции активации с учетом части ошибки из предыдущего слоя
                    double change_weights(double input) => fi * ALPHA * input; // Функция для вычисления изменения веса с учетом скорости обучения и входного значения
                    for (int i = 0; i < weights.Count; i++)
                    {
                        deltas[i] = change_weights(innerInputs[i]) + IMP_ALPHA * deltas[i]; // Вычисление дельта-веса с учетом импульса
                        weights[i] += deltas[i]; // Корректировка веса с учетом дельта-веса
                    }
                }
                /// <summary>
                /// Метод умножения входов на производную 
                /// </summary>
                /// <returns>Лист произведений входных значений и производной функции активации для каждого нейрона</returns>
                public List<double> getListWeightFi()
                {
                    return innerInputs.Select(input => input * fi).ToList(); 
                }
            }
            private class Layer
            {
                private List<Neuron> neurons; // Список нейронов в слое

                public Layer(int size, int inputSize)
                {
                    neurons = new List<Neuron>();
                    for (int i = 0; i < size; i++)
                        neurons.Add(new Neuron(inputSize)); // Создание и добавление нейронов в слой
                }
                /// <summary>
                /// Метод прямого распространения на уровне слоя
                /// </summary>
                /// <param name="inputs">Входные значения</param>
                /// <returns>Значение выхода слоя</returns>
                public List<double> feedForward(List<double> inputs)
                {
                    var outputLayer = new List<double>();
                    foreach (var neuron in neurons)
                        outputLayer.Add(neuron.feedForward(inputs)); // Вычисление выходных значений для каждого нейрона в слое
                    return outputLayer;
                }
                /// <summary>
                /// Метод обратного распространения ошибки на уровне слоя
                /// </summary>
                /// <param name="backListWeightFi">Список ошибок для выходного слоя</param>
                /// <returns>Обновлённый список ошибок выходного слоя</returns>
                public List<List<double>> backwardPropagate(List<List<double>> backListWeightFi)
                {
                    var backwardListWeightFi = new List<List<double>>();
                    for (int i = 0; i < neurons.Count; i++)
                    {
                        double sumWeightFi = 0.0;
                        foreach (var weightFi in backListWeightFi)
                            sumWeightFi += weightFi[i]; // Вычисление суммы произведений входных значений и производной функции активации из предыдущего слоя для каждого нейрона
                        neurons[i].correctWeights(sumWeightFi); // Корректировка весов для каждого нейрона с учетом части ошибки
                        backwardListWeightFi.Add(neurons[i].getListWeightFi()); // Добавление списка произведений входных значений и производной функции активации для каждого нейрона в текущем слое
                    }
                    return backwardListWeightFi;
                }
            }
            /// <summary>
            /// Метод обратного распространения ошибки
            /// </summary>
            /// <param name="error">Значение ошибки</param>
            private void backwardPropagate(double error)
            {
                var backwardData = new List<List<double>>() { new List<double>() { error } }; // Инициализация списка ошибок для выходного слоя
                for (int i = layers.Count - 1; i >= 0; i--)
                    backwardData = layers[i].backwardPropagate(backwardData); // Обратное распространение ошибки по всем слоям сети
            }
            /// <summary>
            /// Метод прямого распространения
            /// </summary>
            /// <param name="inputs">Входные значения</param>
            /// <returns>Значение выхода</returns>
            private double feedForward(List<double> inputs)
            {
                var localInputs = inputs;
                foreach (var layer in layers)
                    localInputs = layer.feedForward(localInputs); // Прямое распространение сигнала по всем слоям сети
                return localInputs.First(); // Возвращение значения выходного нейрона
            }
            /// <summary>
            /// Метод округлния числа после запятой
            /// </summary>
            /// <param name="num">Число под округление</param>
            /// <param name="size">Количество чисел после запятой, которе останется</param>
            /// <returns>Значеие с округлённой вещественной частью</returns>
            private double round(double num, int size = 2)
            {
                double s = Math.Pow(10.0, size);
                return Math.Round(num * s) / s;
            }
            /// <summary>
            /// Метод обучения сети
            /// </summary>
            /// <param name="rb">Объект вывода текста</param>
            /// <param name="data">Лист значений для обучения</param>
            /// <returns>Лист с результатом обучения</returns>
            public List<double> training(RichTextBox rb,List<double> data)
            {
                if (data == null || data.Count == 0)
                    throw new Exception("data not specified"); // Проверка наличия данных для обучения. Если данных нет, то выбрасывается исключение.
                rb.Text += "data size " + data.Count; // Вывод размера данных на консоль.
                List<double> result = null; // Объявление переменной для хранения результата обучения. Инициализируется значением null.
                var inputWindow = new List<double>(); // Создание списка для хранения окна входных данных.
                for (int i = 0; i < inputWindowSize; i++)
                    inputWindow.Add(data[i]); // Инициализация окна входных данных первыми значениями из данных для обучения.
                for (int i = inputWindowSize; i < data.Count; i++) // Цикл по всем значениям функции в данных для обучения, начиная с индекса, равного размеру окна входных данных.
                {
                    double trueResult = data[i]; // Истинное значение функции на текущем шаге. Берется из данных для обучения по текущему индексу i.
                    double localResult; // Объявление переменной для хранения приближенного значения функции на текущем шаге.
                    int k = 0; // Объявление и инициализация переменной для хранения количества итераций обучения на текущем шаге.
                    int c_err = 0; // Объявление и инициализация переменной для хранения количества итераций с неизменной ошибкой на текущем шаге.
                    double error = 0.0; // Объявление и инициализация переменной для хранения ошибки на текущем шаге.
                    while (true) // Бесконечный цикл, который будет прерван, если ошибка станет меньше порога EPS или не будет меняться более 100 итераций.
                    {
                        k++; // Увеличение количества итераций обучения на единицу.
                        localResult = feedForward(inputWindow); // Вычисление приближенного значения функции на текущем шаге с помощью метода feedForward(), который принимает окно входных данных и прямо распространяет его по всем слоям сети. Значение выходного нейрона присваивается переменной localResult.
                        if (round(error,12) == round(trueResult - localResult,12)) c_err++; // Если ошибка не изменилась по сравнению с предыдущей итерацией, то увеличиваем количество итераций с неизменной ошибкой на единицу.
                        else c_err = 0; // Иначе обнуляем количество итераций с неизменной ошибкой.
                        if (c_err > 100) // Если количество итераций с неизменной ошибкой превысило 100, то
                        {
                            rb.Text +=("\nstop " + i + ": " + k + " steps\nerror " + error); // Выводим на консоль номер шага, количество итераций обучения и значение ошибки на этом шаге.
                            return result; // Останавливаем обучение и возвращаем результат. 
                        }
                        error = trueResult - localResult; // Вычисляем ошибку как разность между истинным и приближенным значением функции на текущем шаге. Присваиваем значение переменной error.
                        if (EPS < Math.Abs(error)) backwardPropagate(error); // Если ошибка больше порога EPS по модулю, то корректируем веса сети с помощью метода backwardPropagate(), который принимает ошибку и обратно распространяет ее по всем слоям сети.
                        else break; // Иначе прерываем цикл, так как достигнута требуемая точность.
                    }
                    rb.Text += ("\ntraining step " + i + ": " + k + " steps"); // Выводим на консоль номер шага и количество итераций обучения на этом шаге.
                    for (int j = 0; j < inputWindow.Count - 1; j++)
                        inputWindow[j] = inputWindow[j + 1]; // Сдвигаем окно входных данных на один шаг вправо, удаляя первое значение из окна.
                    inputWindow[inputWindowSize - 1] = data[i]/*localResult*/; // Добавляем приближенное значение функции на текущем шаге в конец окна входных данных.
                }
                return result; // Возвращаем результат обучения.
            }
            /// <summary>
            /// Сетод прогнозирования новых значений
            /// </summary>
            /// <param name="rb">Объект вывода текста</param>
            /// <param name="data">Лист фактических значений для прогнозирования</param>
            /// <returns>Лист с результатом прогнозов</returns>
            public List<double> forecast(RichTextBox rb, List<double> data)
            {
                if (data == null || data.Count == 0)
                    throw new Exception("data not specified"); // Проверка наличия данных для прогнозирования. Если данных нет, то выбрасывается исключение.
                rb.Text += ("\ndata size " + data.Count); // Вывод размера данных на консоль.
                var result = new List<double>(); // Создание списка для хранения результата прогнозирования.
                var inputWindow = new List<double>(); // Создание списка для хранения окна входных данных.
                for (int i = 0; i < inputWindowSize; i++)
                    inputWindow.Add(data[i]); // Инициализация окна входных данных последними значениями из данных для обучения.
                for (int i = inputWindowSize; i < data.Count; i++) // Цикл по всем значениям функции в данных для прогнозирования, начиная с индекса, равного размеру окна входных данных.
                {
                    double localResult;
                    localResult = feedForward(inputWindow); // Вычисление приближенного значения функции на текущем шаге с помощью метода feedForward(), который принимает окно входных данных и прямо распространяет его по всем слоям сети. Значение выходного нейрона присваивается переменной localResult.
                    rb.Text += ("\nforecast step " + i + ": " + localResult); // Выводим на консоль номер шага и приближенное значение функции на этом шаге.
                    result.Add(localResult/*feedForward(inputWindow)*/); // Добавляем приближенное значение функции на следующем шаге в список результатов.
                    for (int j = 0; j < inputWindow.Count - 1; j++)
                        inputWindow[j] = inputWindow[j + 1]; // Сдвигаем окно входных данных на один шаг вправо, удаляя первое значение из окна.
                    inputWindow[inputWindowSize - 1] = data[i]; // Добавляем истинное значение функции на текущем шаге в конец окна входных данных.
                }
                return result; // Возвращаем результат прогнозирования.
            }
            //public List<double> forecast(RichTextBox rb, List<double> data)
            //{
            //    if (data == null || data.Count == 0)
            //        throw new Exception("data not specified"); // Проверка наличия данных для прогнозирования. Если данных нет, то выбрасывается исключение.
            //    rb.Text += ("\ndata size " + data.Count); // Вывод размера данных на консоль.
            //    var result = new List<double>(); // Создание списка для хранения результата прогнозирования.
            //    var inputWindow = new List<double>(); // Создание списка для хранения окна входных данных.
            //    for (int i = 0; i < inputWindowSize; i++)
            //        inputWindow.Add(data[i]); // Инициализация окна входных данных последними значениями из данных для обучения.
            //    for (int i = inputWindowSize; i < data.Count; i++) // Цикл по всем значениям функции в данных для прогнозирования, начиная с индекса, равного размеру окна входных данных.
            //    {
            //        double localResult;
            //        localResult = feedForward(inputWindow); // Вычисление приближенного значения функции на текущем шаге с помощью метода feedForward(), который принимает окно входных данных и прямо распространяет его по всем слоям сети. Значение выходного нейрона присваивается переменной localResult.
            //        rb.Text += ("\nforecast step " + i + ": " + localResult); // Выводим на консоль номер шага и приближенное значение функции на этом шаге.
            //        result.Add(localResult); // Добавляем приближенное значение функции в список результатов.
            //        for (int j = 0; j < inputWindow.Count - 1; j++)
            //            inputWindow[j] = inputWindow[j + 1]; // Сдвигаем окно входных данных на один шаг вправо, удаляя первое значение из окна.
            //        inputWindow[inputWindowSize - 1] = localResult; // Добавляем приближенное значение функции на текущем шаге в конец окна входных данных.
            //    }
            //    return result; // Возвращаем результат прогнозирования.
            //}
        }

        public Form1()
        {
            InitializeComponent();
        }

        // Создание объекта класса ForecastNS
        ForecastNS fn;
        // Контенер для списка значений функции Sin(2*PI * dT) на отрезке [minT, maxT] с шагом dT
        List<double> data = new List<double>();
        int stopTrainPoint = 0;

        private void Form1_Load(object sender, EventArgs e)
        {
            //fn.layersSize = 2 - количество слоев в сети (по условию)
            //fn.inputWindowSize = 5 - Размер окна входных данных (по условию)
            fn = new ForecastNS(2, 5);
            stopTrainPoint = (int)numericUpDown_StopTrainPoint.Value;
        }
        /// <summary>
        /// Событие нажатия кнопки генерации значений на промежутке
        /// </summary>
        private void button_CalcBasePoints_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            if (data.Count > 0)
                data.Clear();

            // Создание списка значений функции Sin(2*PI * dT) на отрезке [minT, maxT] с шагом dT
            double dT = (double)numericUpDown_dT.Value;
            double minT = (double)numericUpDown_minT.Value;
            double maxT = (double)numericUpDown_maxT.Value;
            for (double x = minT; x <= maxT; x += dT)
                data.Add(Math.Sin(2*Math.PI * x));
            foreach (var el in data) 
                dataGridView1.Rows.Add(el);

            //double maxData = data.Max();
            //double minData = data.Min();
            //richTextBox1.Text += "\nМасштабирую значения";
            //for (int i = 0; i < data.Count; i++)
            //    data[i] = (data[i] - minData) / (maxData - minData);
            //foreach (var el in data)
            //    dataGridView2.Rows.Add(el);
        }
        /// <summary>
        /// Событие нажатия кнопки запуска обучения
        /// </summary>
        private void button_Train_Click(object sender, EventArgs e)
        {
            List<double> trainData = new List<double>(data.Take(stopTrainPoint)); //Отсекаем лишнюю часть данных (конец, который будем рогнозировать)
            fn.training(richTextBox1, trainData);// Обучение сети
        }
        /// <summary>
        /// Событие нажатия кнопки запуска прогнозирования
        /// </summary>
        private void button_Aprox_Click(object sender, EventArgs e)
        {
            List<double> newData = new List<double>(data);
            newData.RemoveRange(0, stopTrainPoint);//отсекаем лишнюю часть данных (начало на котором обучалось)
            // Прогнозирование значений функции на втором участке диапазона
            List<double> forecast = fn.forecast(richTextBox1, newData);

            //Настройка свойств Графика 
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisX.Title = "Время"; 
            chart1.ChartAreas[0].AxisY.Title = "Значение функции"; 
            chart1.ChartAreas[0].AxisX.Minimum = 0; 
            chart1.ChartAreas[0].AxisX.Maximum = data.Count;
            chart1.ChartAreas[0].AxisY.Minimum = -1.2;
            chart1.ChartAreas[0].AxisY.Maximum = 1.2;
            // Создание серии для изначальной функции
            Series original = new Series("Реальное"); 
            original.ChartType = SeriesChartType.Line; 
            original.Color = Color.Blue; 
            // Создание серии для аппроксимированного участка
            Series approximated = new Series("Прогноз"); 
            approximated.ChartType = SeriesChartType.Line; 
            approximated.Color = Color.Red; 
            // Добавление точек данных в серию для изначальной функции
            for (int i = 0; i < data.Count; i++)  
                original.Points.AddXY(i, data[i]); 
            // Добавление точек данных в серию для аппроксимированного участка
            for (int i = 0; i < forecast.Count; i++) 
                approximated.Points.AddXY(stopTrainPoint + i, forecast[i]); 
            // Добавление серий в компонент
            chart1.Series.Add(original); 
            chart1.Series.Add(approximated); 
            chart1.Series[0].ChartType = SeriesChartType.Line;
            chart1.Series[1].ChartType = SeriesChartType.Line;
        }
        /// <summary>
        /// Событие добавления новых рядов в таблицы. Заполняет заголовки рядов порядковым значением ряда
        /// </summary>
        private void dgGrid_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;
            var rowIdx = (e.RowIndex + 1).ToString();
            var centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }
        /// <summary>
        /// Событие изменения значения элемента формы точки обрезки выборки значений
        /// </summary>
        private void numericUpDown_StopTrainPoint_ValueChanged(object sender, EventArgs e)
        {
            stopTrainPoint = (int)numericUpDown_StopTrainPoint.Value;
        }
    }
}
