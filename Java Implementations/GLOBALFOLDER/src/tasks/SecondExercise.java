package tasks;

import java.util.Scanner;

public class SecondExercise {
    public static void main(String... args) {
        Scanner sc = new Scanner(System.in);
        Complex z = new Complex(0, 0); // начальное число

        System.out.println("Комплексный калькулятор. Текущее число: " + z);

        while (true) {
            System.out.print("""
                    
                    Выберите операцию:
                    a - сложить
                    s - вычесть
                    m - умножить
                    d - разделить
                    r - модуль
                    g - аргумент
                    q - выход
                    >  """);

            char command = sc.next().charAt(0);

            switch (command) {
                case 'a' -> {
                    System.out.print("Введите второе число (Re Im): ");
                    double re = sc.nextDouble(), im = sc.nextDouble();
                    z.add(new Complex(re, im));
                    System.out.println("Результат: " + z);
                }
                case 's' -> {
                    System.out.print("Введите второе число (Re Im): ");
                    double re = sc.nextDouble(), im = sc.nextDouble();
                    z.subtract(new Complex(re, im));
                    System.out.println("Результат: " + z);
                }
                case 'm' -> {
                    System.out.print("Введите второе число (Re Im): ");
                    double re = sc.nextDouble(), im = sc.nextDouble();
                    z.multiply(new Complex(re, im));
                    System.out.println("Результат: " + z);
                }
                case 'd' -> {
                    System.out.print("Введите второе число (Re Im): ");
                    double re = sc.nextDouble(), im = sc.nextDouble();
                    z.divide(new Complex(re, im));
                    System.out.println("Результат: " + z);
                }
                case 'r' -> System.out.println("Модуль: " + z.abs());
                case 'g' -> System.out.println("Аргумент (в радианах): " + z.arg());
                case 'q', 'Q' -> {
                    System.out.println("Выход из программы.");
                    return;
                }
                default -> System.out.println("Неизвестная команда!");
            }
        }
    }
}

class Complex implements Comparable<Complex> {
    private double realPiece;
    private double imaginaryPiece;

    /**
     * Конструктор, принимающий два параметра.
     * @param re действительная часть
     * @param im мнимая часть
     */
    public Complex(double re, double im) {
        this.realPiece = re;
        this.imaginaryPiece = im;
    }

    /**
     * Метод сложения двух комплексных чисел
     * @param other другое комплексное
     */
    public void add(Complex other) {
        this.realPiece += other.realPiece;
        this.imaginaryPiece += other.imaginaryPiece;
    }

    /**
     * Метод вычитания комплексных чисел
     * @param other другое комплексное
     */
    public void subtract(Complex other) {
        this.realPiece -= other.realPiece;
        this.imaginaryPiece -= other.imaginaryPiece;
    }

    /**
     * Метод умножения комплексных чисел
     * @param other другое
     */
    public void multiply(Complex other) {
        double newReal =
                this.realPiece * other.realPiece - this.imaginaryPiece * other.imaginaryPiece;
        double newImaginary =
                this.realPiece * other.imaginaryPiece + this.imaginaryPiece * other.realPiece;
        this.realPiece = newReal;
        this.imaginaryPiece = newImaginary;
    }

    /**
     * Метод деления двух комплексных чисел
     * @param other
     */
    public void divide(Complex other) {
        double denominator = other.realPiece * other.realPiece
                + other.imaginaryPiece * other.imaginaryPiece;
        if (denominator == 0) {
            System.err.println("Divide by a zero");
            return;
        }

        double newRe = (this.realPiece * other.realPiece
            + this.imaginaryPiece * other.imaginaryPiece) / denominator;
        double newIm = (this.imaginaryPiece * other.realPiece
            - this.realPiece * other.imaginaryPiece) / denominator;

        this.imaginaryPiece = newIm;
        this.realPiece = newRe;
    }

    /**
     * Вычисление аргумента комплексного числа.
     */
    public double arg() {
        return Math.atan2(imaginaryPiece, realPiece);
    }

    /**
     * Вычисление модуля комплексного числа
     */
    public double abs() {
        return Math.sqrt(realPiece * realPiece +
                imaginaryPiece * imaginaryPiece);
    }

    public double real() { return this.realPiece; }
    public double imaginary() { return this.imaginaryPiece; }

    @Override
    public String toString() {
        return String.format("%.3f %s %.3fi",
                realPiece, (imaginaryPiece >= 0 ? "+" : "-"), Math.abs(imaginaryPiece));
    }

    @Override
    public int compareTo(Complex other) {
        double abs1 = this.abs();
        double abs2 = other.abs();
        return Double.compare(abs1, abs2);
    }
}