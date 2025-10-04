using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.UI.DataVisualization.Charting;
using System.Windows.Forms;
namespace NNS4
{
    public partial class Form1 : Form
    {
        int[] chImg = new int[100]; //Контейнер оцифрованного образа для проверки обученной сети 
        int[] DESIRED_VALUES = null; //Контейнер ожидаемых значений для обучения сети 
        int countSamples = 0; //Количество загруженных образов для обучения
        double[] A = null; //Контейнер для весовых коэффициентов
        const double THRESHOLD = 1; // Порог
        const double ALPHA = 0.05; // Коэффициент Скорости обучения
        List<int[]> SAMPLE_LIST = new List<int[]>(); //Лист оцифрованных образов для обучения сети 

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Метод загрузки и отрисовки проверяемого образа
        /// </summary>
        /// <param name="width">Клиентская Ширина изображения (меньше фактической)</param>
        /// <param name="height">Клиентская Высота изображения (меньше фактической)</param>
        /// <param name="img">Массив для загрузки оцифрованного образа (100 элементов)</param>
        /// <returns>Изображение оцифрованного образа (10 на 10 "пикселов")</returns>
        public Bitmap Draw(int width, int height, int[] img)
        {
            try
            {
                //Открываем файл
                OpenFileDialog openFile = new OpenFileDialog();
                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    //считываем все символы
                    string str = File.ReadAllText(openFile.FileName);
                    //Подготавливаем строку (убираем все пробелы и спецсимволы)
                    str = str.Replace(" ", "");
                    str = str.Replace("\r\n", "");
                    str = str.Replace(",", "");
                    str = str.Replace(".", "");
                    
                    //Переносим символы в массив
                    int i = 0;
                    foreach (var el in str)
                    {
                        img[i] = (int)char.GetNumericValue(el);
                        i++;
                    }
                    richTextBox1.Text += "\nФайл: " + openFile.SafeFileName + " удачно считан!";
                }
                else
                {
                    richTextBox1.Text += "\nФайл для проверки не выбран!";
                    img = new int[100];
                    return null;
                }
            }
            catch
            {
                richTextBox1.Text += "\nПроизошла ошибка при считывании файла.";
                img = new int[100];
                return null;
            }
            //Задаю размер клеток
            int heightCell = height / 10;
            int widthCell = width / 10;
            //создаю пустое изображение 
            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            var graphics = Graphics.FromImage(bitmap);
            //задаю тип сглаживаия
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            //задаю кисти: pen - для отрисовки граней, brush - для заливки 
            Pen pen = new Pen(new SolidBrush(Color.Green));
            SolidBrush brush1 = new SolidBrush(Color.White);
            SolidBrush brush2 = new SolidBrush(Color.Red);
            //Начинаю отрисовку
            for (int i = 0; i < 10; i++)
                for(int j = 0; j < 10; j++)
                {
                    switch (img[i*10+j])
                    {
                        case 0: //если 0, то пустая клетка
                            graphics.DrawRectangle(pen, j*widthCell, i*heightCell, widthCell, heightCell); //грань
                            graphics.FillRectangle(brush1, j*widthCell, i*heightCell, widthCell, heightCell); //заливка
                            break;
                        case 1: //если 1, то заполненная
                            graphics.DrawRectangle(pen, j * widthCell, i * heightCell, widthCell, heightCell); //грань
                            graphics.FillRectangle(brush2, j * widthCell, i * heightCell, widthCell, heightCell); //заливка
                            break;
                    }
                }
            richTextBox1.Text += "\nСчитанный файл: удачно отрисован!";
            //Возвращаю заполненное изображение
            return bitmap;
        }
        /// <summary>
        /// Событие нажатия кнопки загрузки образа для проверки обученной сети
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Draw(pictureBox1.Width, pictureBox1.Height, chImg) ;
        }
        /// <summary>
        /// Метод обновления весов по дельта-правилу
        /// </summary>
        /// <param name="num">Порядковый номер образа для обучения</param>
        /// <param name="a">Массив весовых коэффициентов</param>
        /// <param name="delta">Значение отклонения (Ожидаемое - Фактическое)</param>
        /// <returns>Массив обновлённых весовых коэффициентов</returns>
        double[] DeltaRule(int num, double[] a, double delta)
        {
            // Для каждого элемента в массиве a
            for (int j = 0; j < a.Length; j++)
            {
                // Прибавляем к нему произведение константы ALPHA, дельты и соответствующего элемента изображения
                a[j] += ALPHA * delta * SAMPLE_LIST[num][j];
            }
            // Возвращаем обновленный массив a
            return a;
        }
        /// <summary>
        /// Метод случайной генерации весовых коэффициентов
        /// </summary>
        /// <returns>Заполненный случайными числами [-1..1] массив весовых коэффициентов</returns>
         List<double> setRandMass()
        {
            List<double> a = new List<double>(); // Вектор весов
            Random random = new Random();
            for (int i = 0; i < SAMPLE_LIST[0].Length; i++)
            {
                a.Add(random.NextDouble() * 2 - 1);
            }
            return a;
        }
        /// <summary>
        /// Метод процесса обучения персептрона 
        /// </summary>
        /// <param name="number">Массив с порядковыми номерами образов от 0 до N-1</param>
        public void NeuralTraining(int[] number)
        {
            // Подготовка к обучению
            int epoch = 0; // Счетчик эпох
            A = setRandMass().ToArray(); // Получаем начальные веса

            // Создаем форму и элемент управления Chart
            Chart chart = new Chart();

            // Добавляем область для рисования графика
            chart.ChartAreas.Add(new ChartArea());

            // Добавляем серию для отображения данных
            chart.Series.Add(new Series());
            chart.Series[0].ChartType = SeriesChartType.Line; // Тип графика - линейный
            chart.Series[0].Color = System.Drawing.Color.Red; // Цвет линии - красный

            // Задаем подписи и масштаб осей
            chart.ChartAreas[0].AxisX.Title = "Эпоха"; // Подпись оси X
            chart.ChartAreas[0].AxisX.Minimum = 0; // Минимальное значение оси X
            //chart.ChartAreas[0].AxisX.Maximum = ; // Максимальное значение оси X
            chart.ChartAreas[0].AxisY.Title = "Количество неправильных выходов"; // Подпись оси Y
            chart.ChartAreas[0].AxisY.Minimum = 0; // Минимальное значение оси Y
            chart.ChartAreas[0].AxisY.Maximum = countSamples; // Максимальное значение оси Y

            // Запускаем алгоритм
            while (true)
            {
                epoch += 1; // Увеличиваем счетчик эпох
                if (epoch > 100) // Условие выхода. 
                {//Если превысили лимит эпох
                    richTextBox1.Text +=  $"\n\tНе удалось вычислить значение за {epoch-1} эпох."; // Выводим сообщение об неудаче обучения
                    break; // Прерываем цикл
                }

                int trainedSuccess = 0; // Счетчик успешно обработанных образов
                foreach (int num in number) // для каждого образа (number массив порядковых чисел от 0 до N-1)
                {
                    string result = $"Обработан {num}-й образ."; // Формируем строку результата
                    double g = 0; // Обнуляем переменную g
                    for (int i = 0; i < A.Length; i++) // Для каждого элемента в массиве весов A
                    {
                        // Высчитываем сумму произведений значений сигналов и соответствующих весов
                        g +=  SAMPLE_LIST[num][i] * A[i];
                    }
                    // Вычисляем выход нейрона с помощью функции активации
                    int Y = g >= THRESHOLD ? 1 : 0; // Возвращаем 1, если g больше или равно порогового значения, иначе 0
                    if (Y == DESIRED_VALUES[num]) // Если выход совпадает с желаемым значением для данного образа
                    {
                        result += $"({(Y == 1 ? "РОМБ" : "НЕ РОМБ")})"; //Выводим результат
                        trainedSuccess += 1; // Увеличиваем счетчик успешно обработанных образов на 1
                    }
                    else // Если выход не совпадает с желаемым значением
                    {
                        A = DeltaRule(num, A, DESIRED_VALUES[num] - Y); // Обновляем веса в соответствии с дельта-правилом
                    }
                    richTextBox1.Text += "\n" + result; // Выводим строку результата
                }
                // Выводим количество правильно вычисленных образов на данной эпохе
                richTextBox1.Text += $"\n\t***На эпохе {epoch} правильно вычислено {trainedSuccess} образов***"; 

                // Добавляем точки данных в серию
                chart.Series[0].Points.AddXY(epoch, countSamples - trainedSuccess); // Координаты X и Y для каждой точки

                if (trainedSuccess == number.Length) // Если все образы были правильно вычислены
                    break; // Прерываем цикл
            }
            //Вывожу график изменения ошибки в ЛОГ
            // Создаем поток в памяти
            using (MemoryStream stream = new MemoryStream())
            {
                // Сохраняем график в поток в формате BMP
                chart.SaveImage(stream, ChartImageFormat.Bmp);
                // Перемещаем указатель потока в начало
                stream.Seek(0, SeekOrigin.Begin);

                Image img = Image.FromStream(stream);
                //Отправляем изображение в буфер обмена
                Clipboard.SetImage(img);
                //Переводим фокус на RichTextBox
                richTextBox1.ScrollToCaret();
                   richTextBox1.Focus();
                richTextBox1.Select(richTextBox1.Text.Length, 0);
                //Симулируем нажатие CTRL+V
                SendKeys.Send(@"\^V");
            }
        }
        /// <summary>
        /// Метод проверки образа обученной нейронной сетью
        /// </summary>
        /// <param name="img">Оцифрованный образ для проверки (образ фомировался в методе загрузки и отрисовки образа)</param>
        public void NeuralProcess(int[] img)
        {
            double g = 0; // Обнуляем переменную g
            for (int i = 0; i < A.Length; i++) // Для каждого элемента в массиве A
            {
                // Высчитываем сумму произведений значений сигналов и соответствующих весов
                g += img[i] * A[i];
            }
            int Y = g >= THRESHOLD ? 1 : 0; // Вычисляем выход нейрона с помощью функции активации
            richTextBox1.Text += $"\n\t***Проверяемый образ {(Y == 1 ? "РОМБ" : "НЕ РОМБ")}***";
        }
        /// <summary>
        /// Метод загрузки образцов для обучения (доступен мультивыбор)
        /// </summary>
        public void LoadSample()
        {
            try
            {
                //Открываем файлы
                OpenFileDialog openFile = new OpenFileDialog();
                openFile.Multiselect = true;
                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    foreach (var file in openFile.FileNames)
                    {
                        //считываем все символы
                        string str = File.ReadAllText(file);
                        //Подготавливаем строку (убираем все пробелы и спецсимволы)
                        str = str.Replace(" ", "");
                        str = str.Replace("\r\n", "");
                        str = str.Replace(",", "");
                        str = str.Replace(".", "");

                        int[] img = new int[chImg.Length];
                        //Переносим символы в массив
                        int i = 0;
                        foreach (var el in str)
                        {
                            img[i] = (int)char.GetNumericValue(el);
                            i++;
                        }
                        richTextBox1.Text += "\nФайл: " + file.Split('\\').Last() + " удачно считан!";
                        //Добавляем массив в лист
                        SAMPLE_LIST.Add(img);
                    }
                    countSamples = openFile.FileNames.Length;
                    richTextBox1.Text += $"\nВсе файлы удачно считаны! Обучающая выборка сформирована и состоит из {countSamples} образов.";

                }
                else
                    richTextBox1.Text += "\nФайл для проверки не выбран!";
            }
            catch
            {
                richTextBox1.Text += "\nПроизошла ошибка при считывании файла.";
            }
        }
        /// <summary>
        /// Событие нажатия кнопки загрузки файлов длля обучения
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            LoadSample(); //Вызываем метод загрузки образцов (можно выбрать несколько)
            richTextBox1.Text += "\n-------------------------------------";
        }
        /// <summary>
        /// Событие нажатия кнопки обучения персептрона
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            //Проверка на допустимость подготовленных к этому этапу данных
            if (!String.IsNullOrWhiteSpace(textBox1.Text))
            {
                string str = textBox1.Text.Replace(" ", "");
                if (countSamples > 0 && str.Length == countSamples)
                {
                    int i = 0;
                    DESIRED_VALUES = new int[countSamples];
                    foreach (var el in str) // Заполняем ожидаемые значения из строки на форме
                    {
                        DESIRED_VALUES[i] = (int)Char.GetNumericValue(el);
                        i++;
                    }
                    //Вызываем метод обучения
                    NeuralTraining(Enumerable.Range(0, countSamples).ToArray());
                }
                else
                {
                    DESIRED_VALUES = null;
                    MessageBox.Show("Количество цифр не совпадает или неправильные разделители!", "Не все обязательные действия выполнены!",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                DESIRED_VALUES = null;
                MessageBox.Show("Введите ожидаемое значение для обучения. Это 1 или 0 разделённые пробелами." +
                    " Количество цифр совпадает с загруженных количеством образов.", "Не все обязательные действия выполнены!", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            richTextBox1.Text += "\n-------------------------------------";
        }
        /// <summary>
        /// Событие нажатия кнопки проверки образа в обученной сети
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            if(chImg != null) // Если файл для проверки загружен
                NeuralProcess(chImg); //Вызываем метод проверки образа в обученной сети
            else
                MessageBox.Show("\nОбраз для проверки не был загружен или был сброшен! повторите загрузку и отрисовку образа.", "Загрузитте образ!",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            richTextBox1.Text += "\n-------------------------------------";
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}