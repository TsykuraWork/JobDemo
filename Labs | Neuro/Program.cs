enum TypeAlgorithm { oneStep, linarStep }
class NeuronMax
{
    TypeAlgorithm activateFuncType = TypeAlgorithm.oneStep;

    List<List<double>> A = new List<List<double>>()
    {
        new List<double>() { 4, -1, 2, -1, 0 },
        new List<double>() { 2, 3, 4, 0, -1 }
    };

    List<double> B = new List<double>() { 12, 6, 16, 0, 0 };
    List<double> X1 = new List<double>() { 3.0, 4.0 };
    List<double> XN = new List<double>() { 4.8, 3.6 };

    List<int> C = new List<int>() { -1, -2 };

    double H = 0.001;
    int D = 1;

    public List<double> Calculate()
    {
        List<List<double>> Y = new List<List<double>>()
        {
            new List<double>(),
            new List<double>()
        };
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < B.Count; j++)
            {
                Y[i].Add(0);
            }
        }

        int k = 0;
        int iters = 2000;
        while (k < iters)
        {
            Console.WriteLine($"Шаг {k + 1}\t: X = [{string.Join(" ; ", X1)}]");
            for (int i = 0; i < A.Count; i++)
                for (int j = 0; j < A[0].Count; j++)
                    switch (activateFuncType)
                    {
                        case TypeAlgorithm.oneStep: 
                            Y[i][j] = ActivationFunc(A[i][j] * X1[i] - B[j]);
                            break;
                        case TypeAlgorithm.linarStep:
                            Y[i][j] = ActivationFunc2(A[i][j] * X1[i] - B[j]);
                            break;
                    }

            var AT = Transpose(A);

            double tmp;
            double eps = 0.001;
            var stopNum = X1;   
            for (int i = 0; i < 2; i++)
            {
                tmp = 0.0;
                for (int j = 0; j < B.Count; j++)
                    tmp += -H * (AT[j][i] * Y[i][j] + D * C[i]);
                tmp *= X1[i] - XN[i] < 0 ? 1.0 : -1.0;
                X1[i] = /*Round*/(X1[i] + tmp);
            }
            //условие выхода
            if (Math.Abs((X1[0] + X1[1]) - (XN[0] + XN[1])) <= eps && Math.Abs((X1[0] + X1[1]) - (stopNum[0] + stopNum[1])) <= eps)
                break;

            k++;
        }
        return X1;
    }

    private double Round(double num, int size = 2)
    {
        var s = Math.Pow(10.0, size);
        return Math.Round(num * s) / s;
    }

    private List<List<double>> Transpose(List<List<double>> matrix)
    {
        var res = new List<List<double>>();
        for (int i = 0; i < matrix.First().Count; i++)
        {
            var resultRow = new List<double>();
            foreach (var row in matrix)
                resultRow.Add(row[i]);
            res.Add(resultRow);
        }
        return res;
    }

    private int ActivationFunc(double n) =>
        n <= 0 ? 0 : 1; //единичный скачёк

    private double ActivationFunc2(double n) =>
        n <= 0 ? 0 : n; //линейный порог

    static void Main(string[] args)
    {
        bool mainLoop = true;
        while (mainLoop)
        {

            NeuronMax n1 = new NeuronMax();
            /*Настройка*/
            //Разные точки для моделирования
            Console.WriteLine("Выбери координату!\n\t1. [4.5, 3.5]\n\t2. [5.1, 4.5], \n\t3. [-3, -1]]");
            bool loop = true;
            while (loop)
                switch (Console.ReadLine())
                {
                    case "1":
                        n1.X1 = new List<double>() { 4.5, 3.5 };
                        loop = false; break;
                    case "2":
                        n1.X1 = new List<double>() { 5.1, 4.5 };
                        loop = false; break;
                    case "3":
                        n1.X1 = new List<double>() { -3, -1 };
                        loop = false; break;
                    default:
                        Console.WriteLine("Ошибка! Введите номер варианта!");
                        break;
                }

            Console.WriteLine("Выбери функцию активации!\n\t1. Единичный скачёк\n\t2. Линейный порог");
            loop = true;
            while (loop)
                switch (Console.ReadLine())
                {
                    case "1":
                        n1.activateFuncType = TypeAlgorithm.oneStep;
                        loop = false; break;
                    case "2":
                        n1.activateFuncType = TypeAlgorithm.linarStep;
                        loop = false; break;
                    default:
                        Console.WriteLine("Ошибка! Введите номер варианта!");
                        break;
                }

            n1.Calculate();
            
            Console.WriteLine("Выйти? [y ; n]");
            loop = true;
            while (loop)
                switch (Console.ReadLine())
                {
                    case "y":
                        mainLoop = false; loop = false; break;

                    case "n":
                        loop = false; break;
                    default:
                        Console.WriteLine("Ошибка! Введите \" y \" чтобы выйти или \" n \" чтобы продолжить!");
                        break;
                }
        }
    }
}