using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;

namespace NNS7_2
{
    internal static class Program
    {
        // Создаем класс контроллера
        class CameraController
        {
            // Определяем свойства класса
            public double Aperture { get; set; } // Значение диафрагмы
            public string ApertureComm { get; set; } // Комментарий к значению диафрагмы
            public double Light { get; set; } // Уровень освещения
            public string LightComm { get; set; } // Комментарий к уровню освещения
            public double Distance { get; set; } // Расстояние до объекта съемки
            public string DistanceComm { get; set; } // Комментарий к расстоянию до объекта съемки

            // Определяем массивы для хранения значений входных и выходных переменных
            private int[] x_light; // Уровень освещения от 0 до 1000 лк
            private int[] x_distance; // Расстояние до объекта съемки от 0 до 10 м
            private double[] x_aperture; // Значение диафрагмы от f/1.4 до f/16

            // Определяем массивы для хранения значений функций принадлежности для каждой переменной
            private double[] dark; // Темно: a = 0, b = 0, c = 50, d = 100
            private double[] bright; // Светло: a = 200, b = 300, c = 1000, d = 1000
            private double[] medium_light; // Средне: a = 50, b = 100, c = 200, d = 300
            private double[] close; // Близко: a = 0, b = 0, c =1 ,d=2 
            private double[] far; // Далеко: a = 5, b = 6, c = 10, d =10 
            private double[] medium_distance; // Средне: a =1 ,b=2 ,c=5 ,d=6 
            private double[] wide; // Широкая: a=f/1.4,b=f/1.4,c=f/2.8,d=f/4 
            private double[] narrow; // Узкая: a=f/11,b=f/13,c=f/16,d=f/16 
            private double[] medium_aperture; // Средняя: a=f/4,b=f/5.6,c=f/11,d=f/13 

            // Определяем массив для хранения классификации переменных
            private string[] classification = new string[] { "UNKNOWN", "Темно", "Светло", "Средне", "Близко", "Далеко", "Широкая", "Узкая" };

            // Создаем конструктор класса
            public CameraController(double _l, double _d)
            {
                // Инициализируем свойства класса значениями с "датчиков"
                Light = _l;
                Distance = _d;

                // Инициализируем массивы значений входных и выходных переменных
                x_light = new int[1001]; // Уровень освещения от 0 до 1000 лк
                for (int i = 0; i < x_light.Length; i++)
                {
                    x_light[i] = i; // Заполняем массив последовательными числами
                }
                x_distance = new int[11]; // Расстояние до объекта съемки от 0 до 10 м
                for (int i = 0; i < x_distance.Length; i++)
                {
                    x_distance[i] = i; // Заполняем массив последовательными числами
                }
                x_aperture = new double[148]; // Значение диафрагмы от f/1.4 до f/16
                for (int i = 0; i < x_aperture.Length; i++)
                {
                    x_aperture[i] = (i + 14) / 10.0; // Заполняем массив дробными числами с шагом 0.1
                }
                // Инициализируем массивы значений функций принадлежности для каждой переменной
                dark = new double[x_light.Length]; // Темно: a = 0, b = 0, c = 50, d = 100
                for (int i = 0; i < dark.Length; i++)
                {
                    dark[i] = Trapezoid(x_light[i], 0, 0, 50, 100); // Вычисляем степень принадлежности для каждого значения переменной
                }
                bright = new double[x_light.Length]; // Светло: a = 200, b = 300, c = 1000, d = 1000
                for (int i = 0; i < bright.Length; i++)
                {
                    bright[i] = Trapezoid(x_light[i], 200, 300, 1000, 1000); // Вычисляем степень принадлежности для каждого значения переменной
                }
                medium_light = new double[x_light.Length]; // Средне: a = 50, b = 100, c = 200, d = 300
                for (int i = 0; i < medium_light.Length; i++)
                {
                    medium_light[i] = Trapezoid(x_light[i], 50, 100, 200, 300); // Вычисляем степень принадлежности для каждого значения переменной
                }
                close = new double[x_distance.Length]; // Близко: a = 0, b = 0, c =1 ,d=2 
                for (int i = 0; i < close.Length; i++)
                {
                    close[i] = Trapezoid(x_distance[i], 0, 0, 1, 2); // Вычисляем степень принадлежности для каждого значения переменной
                }
                far = new double[x_distance.Length]; // Далеко: a = 5, b = 6, c = 10, d =10 
                for (int i = 0; i < far.Length; i++)
                {
                    far[i] = Trapezoid(x_distance[i], 5, 6, 10, 10); // Вычисляем степень принадлежности для каждого значения переменной
                }
                medium_distance = new double[x_distance.Length]; // Средне: a =1 ,b=2 ,c=5 ,d=6 
                for (int i = 0; i < medium_distance.Length; i++)
                {
                    medium_distance[i] = Trapezoid(x_distance[i], 1, 2, 5, 6); // Вычисляем степень принадлежности для каждого значения переменной
                }
                wide = new double[x_aperture.Length]; // Широкая: a=f/1.4,b=f/1.4,c=f/2.8,d=f/4 
                for (int i = 0; i < wide.Length; i++)
                {
                    wide[i] = Trapezoid(x_aperture[i], 1.4, 1.4,  2.8,  4); // Вычисляем степень принадлежности для каждого значения переменной
                }
                narrow = new double[x_aperture.Length]; // Узкая: a=f/11,b=f/13,c=f/16,d=f/16 
                for (int i = 0; i < narrow.Length; i++)
                {
                    narrow[i] = Trapezoid(x_aperture[i], 11, 13, 16, 16); // Вычисляем степень принадлежности для каждого значения переменной
                }
                medium_aperture = new double[x_aperture.Length]; // Средняя: a=f/4,b=f/5.6,c=f/11,d=f/13 
                for (int i = 0; i < medium_aperture.Length; i++)
                {
                    medium_aperture[i] = Trapezoid(x_aperture[i],  4, 5.6, 11, 13); // Вычисляем степень принадлежности для каждого значения переменной
                }
            }
            // Определяем функцию для вычисления трапециевидной функции принадлежности
            private double Trapezoid(double x, double a, double b, double c, double d)
            {
                // Если x меньше a или больше d, то степень принадлежности равна 0
                if (x < a || x > d)
                {
                    return 0;
                }
                // Если x между a и b, то степень принадлежности линейно возрастает от 0 до 1
                else if (a <= x && x < b)
                {
                    return (x - a) / (b - a);
                }
                // Если x между b и c, то степень принадлежности равна 1
                else if (b <= x && x <= c)
                {
                    return 1;
                }
                // Если x между c и d, то степень принадлежности линейно убывает от 1 до 0
                else if (c < x && x <= d)
                {
                    return (d - x) / (d - c);
                }
                // Возвращаем значение по умолчанию
                else
                {
                    return 0;
                }
            }

            // Определяем функцию для классификации значений входных переменных
            private void Qualifier()
            {
                Console.WriteLine("Классификации значений входных переменных");

                // Классифицируем уровень освещения
                if (Light >= 0 && Light <= 100)
                {
                    LightComm = classification[1]; // Темно
                }
                else if (Light > 100 && Light <= 300)
                {
                    LightComm = classification[2]; // Светло
                }
                else if (Light > 300 && Light <= 1000)
                {
                    LightComm = classification[3]; // Средне
                }
                else
                {
                    LightComm = classification[0]; // UNKNOWN
                }

                // Классифицируем расстояние до объекта съемки
                if (Distance >= 0 && Distance <= 2)
                {
                    DistanceComm = classification[4]; // Близко
                }
                else if (Distance > 2 && Distance <= 6)
                {
                    DistanceComm = classification[5]; // Далеко
                }
                else if (Distance > 6 && Distance <= 10)
                {
                    DistanceComm = classification[3]; // Средне
                }
                else
                {
                    DistanceComm = classification[0]; // UNKNOWN
                }
            }
            // Определяем функцию для вычисления значения выходной переменной
            private void Inference()
            {
                Console.WriteLine("Начинаем вычисление значения выходной переменной");

                // Создаем массив для хранения степеней принадлежности выходной переменной к разным нечетким множествам
                double[] aperture_membership = new double[3]; // Широкая, Узкая, Средняя

                // Применяем правила нечеткого вывода в виде логических выражений
                Console.WriteLine("Применяем метод Мамдани (логическое И)");

                // Если Темно И Близко, то Широкая
                aperture_membership[0] = Math.Min(dark[(int)Light], close[(int)Distance]);
                // Если Светло И Далеко, то Узкая
                aperture_membership[1] = Math.Min(bright[(int)Light], far[(int)Distance]);
                // Если Средне И Средне, то Средняя
                aperture_membership[2] = Math.Min(medium_light[(int)Light], medium_distance[(int)Distance]);

                Console.WriteLine("Следом логическое ИЛИ. Находим максимальную степень принадлежности среди всех правил");

                // Находим максимальную степень принадлежности среди всех правил
                double max_membership = Math.Max(aperture_membership[0], Math.Max(aperture_membership[1], aperture_membership[2]));

                // Находим индекс правила с максимальной степенью принадлежности
                int max_index = Array.IndexOf(aperture_membership, max_membership);

                // Применяем метод центра тяжести для дефаззификации выходной переменной
                Console.WriteLine("Применяем метод \"Центра тяжести\" для дефаззификации выходной переменной");

                double sum = 0; // Сумма произведений значений переменной и степеней принадлежности
                double count = 0; // Сумма степеней принадлежности
                for (int i = 0; i < x_aperture.Length; i++)
                {
                    // Учитываем только те значения переменной, которые принадлежат нечеткому множеству с максимальной степенью принадлежности
                    if (max_index == 0) // Широкая
                    {
                        sum += x_aperture[i] * wide[i];
                        count += wide[i];
                    }
                    else if (max_index == 1) // Узкая
                    {
                        sum += x_aperture[i] * narrow[i];
                        count += narrow[i];
                    }
                    else if (max_index == 2) // Средняя
                    {
                        sum += x_aperture[i] * medium_aperture[i];
                        count += medium_aperture[i];
                    }
                }

                // Вычисляем значение выходной переменной как среднее взвешенное значений переменной и степеней принадлежности
                Console.WriteLine("Вычисляем значение выходной переменной как среднее взвешенное значений переменной и степеней принадлежности");

                Aperture = sum / count;
      
                // Классифицируем значение выходной переменной по нечетким множествам
                if (Aperture >= 1.4 && Aperture <= 4)
                {
                    ApertureComm =  classification[6]; // Широкая
                }
                else if (Aperture > 11 && Aperture <= 16)
                {
                    ApertureComm = classification[7]; // Узкая
                }
                else if (Aperture > 4 && Aperture <=  11)
                {
                    ApertureComm = classification[3]; // Средняя
                }
                else
                {
                    ApertureComm = classification[0]; // UNKNOWN
                }

                Console.WriteLine("Отрисовываю Суперпозицию в новой форме и наношу на него центр тяести");

                //Отрисовываю Суперпозицию в новой форме
                // Создаем новую форму для отображения графика суперпозиции
                Form form2 = new Form();
                form2.Text = "Суперпозиция нечетких множеств";
                form2.Size = new Size(800, 600);

                // Создаем компонент Chart для отображения графика
                Chart chartSuper = new Chart();
                chartSuper.Dock = DockStyle.Fill;
                chartSuper.Height = 600;

                // Добавляем компонент Chart на форму
                form2.Controls.Add(chartSuper);

                // Задаем свойства компонента Chart
                chartSuper.Titles.Add("Суперпозиция нечетких множеств"); // Задаем заголовок графика
                chartSuper.ChartAreas.Add(new ChartArea()); // Добавляем область для построения графика
                chartSuper.ChartAreas[0].AxisX.Title = "f-число"; // Задаем подпись для оси X
                chartSuper.ChartAreas[0].AxisY.Title = "Степень принадлежности"; // Задаем подпись для оси Y
                chartSuper.ChartAreas[0].AxisX.Minimum = 1.4; // Задаем минимальное значение для оси X
                chartSuper.ChartAreas[0].AxisX.Maximum = 16; // Задаем максимальное значение для оси X
                chartSuper.ChartAreas[0].AxisY.Minimum = 0; // Задаем минимальное значение для оси Y
                chartSuper.ChartAreas[0].AxisY.Maximum = 1; // Задаем максимальное значение для оси Y

                // Строим график суперпозиции нечетких множеств
                chartSuper.Series.Add("Суперпозиция"); // Добавляем серию данных для суперпозиции
                chartSuper.Series["Суперпозиция"].ChartType = SeriesChartType.Line; // Задаем тип графика линейным
                chartSuper.Series["Суперпозиция"].Color = Color.Blue; // Задаем цвет линии синим
                chartSuper.Series["Суперпозиция"].BorderWidth *= 2;

                // Создаем массив для хранения степеней принадлежности суперпозиции для каждого значения переменной
                double[] super_membership = new double[x_aperture.Length];

                // Вычисляем степени принадлежности суперпозиции с помощью операции логического ИЛИ (Math.Max)
                for (int i = 0; i < super_membership.Length; i++)
                {
                    // Учитываем только те значения переменной, которые принадлежат нечеткому множеству с максимальной степенью принадлежности
                    if (max_index == 0) // Широкая
                    {
                        super_membership[i] = wide[i];
                    }
                    else if (max_index == 1) // Узкая
                    {
                        super_membership[i] = narrow[i];
                    }
                    else if (max_index == 2) // Средняя
                    {
                        super_membership[i] = medium_aperture[i];
                    }
                }

                // Связываем данные по оси X и Y с массивами значений
                chartSuper.Series["Суперпозиция"].Points.DataBindXY(x_aperture, super_membership);

                // Добавляем вертикальную прямую для центра тяжести
                chartSuper.Series.Add("Центр тяжести"); // Добавляем серию данных для центра тяжести
                chartSuper.Series["Центр тяжести"].ChartType = SeriesChartType.Line; // Задаем тип графика линейным
                chartSuper.Series["Центр тяжести"].Color = Color.Purple ; // Задаем цвет линии
                chartSuper.Series["Центр тяжести"].BorderWidth *=2;
                chartSuper.Series["Центр тяжести"].BorderDashStyle = ChartDashStyle.Dash;

                chartSuper.Series["Центр тяжести"].Points.AddXY(Aperture, 0); // Добавляем точку в начале прямой
                chartSuper.Series["Центр тяжести"].Points.AddXY(Aperture, 1); // Добавляем точку в конце прямой

                // Отображаем форму на экране
                form2.Show();
            }

            // Определяем функцию для отображения результатов работы контроллера
            public void ShowResult()
            {
                // Создаем форму для отображения графиков и результатов
                Console.WriteLine("Создаем форму для отображения графиков и результатов");
                Form form = new Form();
                form.Text = "Нечеткий контроллер для камеры";
                form.Size = new Size(800, 600);

                // Создаем компоненты Chart для каждой переменной
                Chart chartLight = new Chart();
                chartLight.Dock = DockStyle.Top;
                chartLight.Height = 200;
                Chart chartDistance = new Chart();
                chartDistance.Dock = DockStyle.Top;
                chartDistance.Height = 200;
                Chart chartAperture = new Chart();
                chartAperture.Dock = DockStyle.Top;
                chartAperture.Height = 200;

                // Добавляем компоненты Chart на форму
                form.Controls.Add(chartAperture);
                form.Controls.Add(chartDistance);
                form.Controls.Add(chartLight);

                // Задаем свойства компонентов Chart
                // Уровень освещения
                chartLight.Titles.Add("Уровень освещения"); // Задаем заголовок графика
                chartLight.ChartAreas.Add(new ChartArea()); // Добавляем область для построения графика
                chartLight.ChartAreas[0].AxisX.Title = "Освещенность, лк"; // Задаем подпись для оси X
                chartLight.ChartAreas[0].AxisY.Title = "Степень принадлежности"; // Задаем подпись для оси Y
                chartLight.ChartAreas[0].AxisX.Minimum = 0; // Задаем минимальное значение для оси X
                chartLight.ChartAreas[0].AxisX.Maximum = 1000; // Задаем максимальное значение для оси X
                chartLight.ChartAreas[0].AxisY.Minimum = 0; // Задаем минимальное значение для оси Y
                chartLight.ChartAreas[0].AxisY.Maximum = 1; // Задаем максимальное значение для оси Y

                // Расстояние до объекта съемки
                chartDistance.Titles.Add("Расстояние до объекта съемки"); // Задаем заголовок графика
                chartDistance.ChartAreas.Add(new ChartArea()); // Добавляем область для построения графика
                chartDistance.ChartAreas[0].AxisX.Title = "Расстояние, м"; // Задаем подпись для оси X
                chartDistance.ChartAreas[0].AxisY.Title = "Степень принадлежности"; // Задаем подпись для оси Y
                chartDistance.ChartAreas[0].AxisX.Minimum = 0; // Задаем минимальное значение для оси X
                chartDistance.ChartAreas[0].AxisX.Maximum = 10; // Задаем максимальное значение для оси X
                chartDistance.ChartAreas[0].AxisY.Minimum = 0; // Задаем минимальное значение для оси Y
                chartDistance.ChartAreas[0].AxisY.Maximum = 1; // Задаем максимальное значение для оси Y

                // Значение диафрагмы
                chartAperture.Titles.Add("Значение диафрагмы"); // Задаем заголовок графика
                chartAperture.ChartAreas.Add(new ChartArea()); // Добавляем область для построения графика
                chartAperture.ChartAreas[0].AxisX.Title = "f-число"; // Задаем подпись для оси X
                chartAperture.ChartAreas[0].AxisY.Title = "Степень принадлежности"; // Задаем подпись для оси Y
                chartAperture.ChartAreas[0].AxisX.Minimum = 1.4; // Задаем минимальное значение для оси X
                chartAperture.ChartAreas[0].AxisX.Maximum = 16; // Задаем максимальное значение для оси X
                chartAperture.ChartAreas[0].AxisY.Minimum = 0; // Задаем минимальное значение для оси Y
                chartAperture.ChartAreas[0].AxisY.Maximum = 1; // Задаем максимальное значение для оси Y

                // Строим графики для каждой переменной с помощью компонентов Chart
                // Уровень освещения
                chartLight.Series.Add("Темно"); // Добавляем серию данных для нечеткого множества Темно
                chartLight.Series["Темно"].ChartType = SeriesChartType.Line; // Задаем тип графика линейным
                chartLight.Series["Темно"].Color = Color.Red; // Задаем цвет линии красным
                chartLight.Series["Темно"].BorderWidth*= 2; // Задаем толщину линии
                chartLight.Series["Темно"].Points.DataBindXY(x_light, dark); // Связываем данные по оси X и Y с массивами значений

                chartLight.Series.Add("Светло"); // Добавляем серию данных для нечеткого множества Светло
                chartLight.Series["Светло"].ChartType = SeriesChartType.Line; // Задаем тип графика линейным
                chartLight.Series["Светло"].Color = Color.Orange; // Задаем цвет линии оранжевым
                chartLight.Series["Светло"].BorderWidth *= 2; // Задаем толщину линии
                chartLight.Series["Светло"].Points.DataBindXY(x_light, bright); // Связываем данные по оси X и Y с массивами значений

                chartLight.Series.Add("Средне"); // Добавляем серию данных для нечеткого множества Средне
                chartLight.Series["Средне"].ChartType = SeriesChartType.Line; // Задаем тип графика линейным
                chartLight.Series["Средне"].Color = Color.Green; // Задаем цвет линии зеленым
                chartLight.Series["Средне"].BorderWidth *= 2; // Задаем толщину линии
                chartLight.Series["Средне"].Points.DataBindXY(x_light, medium_light); // Связываем данные по оси X и Y с массивами значений

                // Расстояние до объекта съемки
                chartDistance.Series.Add("Близко"); // Добавляем серию данных для нечеткого множества Близко
                chartDistance.Series["Близко"].ChartType = SeriesChartType.Line; // Задаем тип графика линейным
                chartDistance.Series["Близко"].Color = Color.Red; // Задаем цвет линии красным
                chartDistance.Series["Близко"].BorderWidth *= 2; // Задаем толщину линии
                chartDistance.Series["Близко"].Points.DataBindXY(x_distance, close); // Связываем данные по оси X и Y с массивами значений

                chartDistance.Series.Add("Далеко"); // Добавляем серию данных для нечеткого множества Далеко
                chartDistance.Series["Далеко"].ChartType = SeriesChartType.Line; // Задаем тип графика линейным
                chartDistance.Series["Далеко"].Color = Color.Orange; // Задаем цвет линии оранжевым
                chartDistance.Series["Далеко"].BorderWidth *= 2; // Задаем толщину линии
                chartDistance.Series["Далеко"].Points.DataBindXY(x_distance, far); // Связываем данные по оси X и Y с массивами значений

                chartDistance.Series.Add("Средне"); // Добавляем серию данных для нечеткого множества Средне
                chartDistance.Series["Средне"].ChartType = SeriesChartType.Line; // Задаем тип графика линейным
                chartDistance.Series["Средне"].Color = Color.Green; // Задаем цвет линии зеленым
                chartDistance.Series["Средне"].BorderWidth *= 2; // Задаем толщину линии
                chartDistance.Series["Средне"].Points.DataBindXY(x_distance, medium_distance); // Связываем данные по оси X и Y с массивами значений

                // Значение диафрагмы
                chartAperture.Series.Add("Широкая"); // Добавляем серию данных для нечеткого множества Широкая
                chartAperture.Series["Широкая"].ChartType = SeriesChartType.Line; // Задаем тип графика линейным
                chartAperture.Series["Широкая"].Color = Color.Red; // Задаем цвет линии красным
                chartAperture.Series["Широкая"].BorderWidth *= 2; // Задаем толщину линии
                chartAperture.Series["Широкая"].Points.DataBindXY(x_aperture, wide); // Связываем данные по оси X и Y с массивами значений

                chartAperture.Series.Add("Узкая"); // Добавляем серию данных для нечеткого множества Узкая
                chartAperture.Series["Узкая"].ChartType = SeriesChartType.Line; // Задаем тип графика линейным
                chartAperture.Series["Узкая"].Color = Color.Orange; // Задаем цвет линии оранжевым
                chartAperture.Series["Узкая"].BorderWidth *= 2; // Задаем толщину линии
                chartAperture.Series["Узкая"].Points.DataBindXY(x_aperture, narrow); // Связываем данные по оси X и Y с массивами значений

                chartAperture.Series.Add("Средняя"); // Добавляем серию данных для нечеткого множества Средняя
                chartAperture.Series["Средняя"].ChartType = SeriesChartType.Line; // Задаем тип графика линейным
                chartAperture.Series["Средняя"].Color = Color.Green; // Задаем цвет линии зеленым
                chartAperture.Series["Средняя"].BorderWidth *= 2; // Задаем толщину линии
                chartAperture.Series["Средняя"].Points.DataBindXY(x_aperture, medium_aperture); // Связываем данные по оси X и Y с массивами значений

                // Вызываем функцию для классификации входных переменных
                Qualifier();

                // Вызываем функцию для вычисления выходной переменной
                Inference();

                // Выводим результаты работы контроллера на экран
                Console.WriteLine($"Полученные параметры съемки:");
                Console.WriteLine($"\tУровень освещения: {Light} лк, что попадает под категорию {LightComm}");
                Console.WriteLine($"\tРасстояние до объекта съемки: {Distance} м, что попадает под категорию {DistanceComm}");
                Console.WriteLine($"\tЗначение диафрагмы: f/{Aperture}, что попадает под категорию {ApertureComm}");

                // Отображаем форму на экране
                form.Show();
                Application.Run(); // не даю консоли закрыться

            }

            static void Main()
            {
                int lux = 0;
                int distance = 0;
                Console.WriteLine($"Введите значение освещённости окружения (от 0 до 1000 люксов)!\n\tОсвещённость: ");
                lux = int.Parse(Console.ReadLine());

                Console.WriteLine($"\nВведите значение расстояния до объекта съёмки (от 0 до 10 метров)!\n\tРасстояние: ");
                    distance = int.Parse(Console.ReadLine());
                    Console.WriteLine("\nДанные приняты. Процесс запущен!\n");
                CameraController controller = new CameraController(lux, distance);
                controller.ShowResult();
            }
        }

    }
}
