using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixTask_1
{
    class FirstExercise
    {
        public static void Main(string[] args)
        {
            string path = "C:\\Users\\User333\\Desktop" +
                "\\Универ (универсальное)\\«Конструирование алгоритмов»" +
                "\\EXERCISES\\По списку из 17 (репо)\\importantFiles\\task1.txt";
            try
            {
                List<double> tokens = ReadAll(path);
                if (tokens.Count == 0)
                {
                    Console.Error.WriteLine("Empty or invalid!");
                    Environment.Exit(1);
                }

                IEnumerator<double> it = tokens.GetEnumerator();
                it.MoveNext();
                int N = (int)Math.Round(it.Current);
                if (N <= 0)
                {
                    Console.Error.WriteLine("Invalid dimension.");
                    Environment.Exit(2);
                }

                int needNums = N * N + N;
                if (tokens.Count - 1 < needNums)
                {
                    Console.Error.WriteLine($"Not enough nums in input: expected {needNums} matrix+vector nums, got {tokens.Count - 1}.");
                    Environment.Exit(3);
                }

                double[,] G = new double[N, N];
                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < N; j++)
                    {
                        if (!it.MoveNext()) ErrorInput();
                        G[i, j] = it.Current;
                    }
                }

                double[] x = new double[N];
                for (int i = 0; i < N; i++)
                {
                    if (!it.MoveNext()) ErrorInput();
                    x[i] = it.Current;
                }

                double eps = 1e-9;
                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < N; j++)
                    {
                        if (Math.Abs(G[i, j] - G[j, i]) > eps)
                        {
                            Console.Error.WriteLine($"Matrix G is not symmetric: G[{i}][{j}] = {G[i, j]}, G[{j}][{i}] = {G[j, i]}");
                            Environment.Exit(4);
                        }
                    }
                }

                double[] y = new double[N];
                for (int i = 0; i < N; i++)
                {
                    double sum = 0.0;
                    for (int j = 0; j < N; j++)
                    {
                        sum += G[i, j] * x[j];
                    }
                    y[i] = sum;
                }

                double s = 0.0;
                for (int i = 0; i < N; i++) s += x[i] * y[i];

                if (s < -1e-12)
                {
                    Console.Error.WriteLine("Warning: quadtratic form is negative!");
                }

                double length = Math.Sqrt(Math.Max(0.0, s));
                Console.WriteLine($"Length = {length:G12}");
            }
            catch (IOException e)
            {
                Console.Error.WriteLine("I/O error: " + e.Message);
                Environment.Exit(13);
            }
        }

        private static void ErrorInput()
        {
            Console.Error.WriteLine("Bad formatting or unexpected token!");
            Environment.Exit(12);
        }

        private static List<double> ReadAll(string path)
        {
            List<double> lout = new List<double>();
            foreach (var line in File.ReadAllLines(path))
            {
                var trimmed = line.Trim();
                if (trimmed.Length == 0 || trimmed.StartsWith("#")) continue;

                var parts = trimmed.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                foreach (var p in parts)
                {
                    try
                    {
                        double val = double.Parse(p, System.Globalization.CultureInfo.InvariantCulture);
                        lout.Add(val);
                    }
                    catch (FormatException)
                    {
                        Console.Error.WriteLine($"Non-numeric token ignored: '{p}'");
                    }
                }
            }
            return lout;
        }

    }
}
