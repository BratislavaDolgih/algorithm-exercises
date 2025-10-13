using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexTask_2
{
    class SecondExercise
    {
        static void Main(string[] args)
        {
            var z = new Complex(0, 0);
            Console.WriteLine("Комплексный калькулятор. Текущее число: " + z);

            while (true)
            {
                Console.WriteLine("\nВыберите операцию:\n" +
                                  "a - сложить\n" +
                                  "s - вычесть\n" +
                                  "m - умножить\n" +
                                  "d - разделить\n" +
                                  "r - модуль\n" +
                                  "g - аргумент\n" +
                                  "q - выход\n> ");
                char command = Console.ReadKey().KeyChar;
                Console.WriteLine();

                switch (command)
                {
                    case 'a':
                        Console.Write("Введите второе число (Re Im): ");
                        var aParts = Console.ReadLine().Split();
                        z.Add(new Complex(double.Parse(aParts[0]), double.Parse(aParts[1])));
                        Console.WriteLine("Результат: " + z);
                        break;
                    case 's':
                        Console.Write("Введите второе число (Re Im): ");
                        var sParts = Console.ReadLine().Split();
                        z.Subtract(new Complex(double.Parse(sParts[0]), double.Parse(sParts[1])));
                        Console.WriteLine("Результат: " + z);
                        break;
                    case 'm':
                        Console.Write("Введите второе число (Re Im): ");
                        var mParts = Console.ReadLine().Split();
                        z.Multiply(new Complex(double.Parse(mParts[0]), double.Parse(mParts[1])));
                        Console.WriteLine("Результат: " + z);
                        break;
                    case 'd':
                        Console.Write("Введите второе число (Re Im): ");
                        var dParts = Console.ReadLine().Split();
                        z.Divide(new Complex(double.Parse(dParts[0]), double.Parse(dParts[1])));
                        Console.WriteLine("Результат: " + z);
                        break;
                    case 'r':
                        Console.WriteLine("Модуль: " + z.Abs());
                        break;
                    case 'g':
                        Console.WriteLine("Аргумент (в радианах): " + z.Arg());
                        break;
                    case 'q':
                    case 'Q':
                        Console.WriteLine("Выход из программы.");
                        return;
                    default:
                        Console.WriteLine("Неизвестная команда!");
                        break;
                }
            }
        }
    }

    class Complex : IComparable<Complex>
    {
        private double realPiece;
        private double imaginaryPiece;

        public Complex(double re, double im)
        {
            realPiece = re;
            imaginaryPiece = im;
        }

        public void Add(Complex other)
        {
            realPiece += other.realPiece;
            imaginaryPiece += other.imaginaryPiece;
        }

        public void Subtract(Complex other)
        {
            realPiece -= other.realPiece;
            imaginaryPiece -= other.imaginaryPiece;
        }

        public void Multiply(Complex other)
        {
            double newReal = realPiece * other.realPiece - imaginaryPiece * other.imaginaryPiece;
            double newImaginary = realPiece * other.imaginaryPiece + imaginaryPiece * other.realPiece;
            realPiece = newReal;
            imaginaryPiece = newImaginary;
        }

        public void Divide(Complex other)
        {
            double denominator = other.realPiece * other.realPiece + other.imaginaryPiece * other.imaginaryPiece;
            if (denominator == 0)
            {
                Console.Error.WriteLine("Divide by zero");
                return;
            }
            double newRe = (realPiece * other.realPiece + imaginaryPiece * other.imaginaryPiece) / denominator;
            double newIm = (imaginaryPiece * other.realPiece - realPiece * other.imaginaryPiece) / denominator;
            realPiece = newRe;
            imaginaryPiece = newIm;
        }

        public double Arg()
        {
            return Math.Atan2(imaginaryPiece, realPiece);
        }

        public double Abs()
        {
            return Math.Sqrt(realPiece * realPiece + imaginaryPiece * imaginaryPiece);
        }

        public double Real() => realPiece;
        public double Imaginary() => imaginaryPiece;

        public override string ToString()
        {
            return string.Format("{0:F3} {1} {2:F3}i", realPiece, imaginaryPiece >= 0 ? "+" : "-", Math.Abs(imaginaryPiece));
        }

        public int CompareTo(Complex other)
        {
            double abs1 = Abs();
            double abs2 = other.Abs();
            return abs1.CompareTo(abs2);
        }
    }
}
