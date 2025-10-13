package tasks;

import java.io.*;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.*;

public class FirstExercise {
    public static void main(String... args) throws IOException {
        Path path = Paths.get("importantFiles/task1.txt");
        try {
            List<Double> tokens = readAll(path);
            if (tokens.isEmpty()) {
                System.err.println("Empty or invalid!");
                System.exit(1);
            }

            Iterator<Double> it = tokens.iterator();
            int N = (int) Math.round(it.next());
            if (N <= 0) {
                System.err.println("Invalid dimension.");
                System.exit(2);
            }

            int needNums = N * N + N;
            if (tokens.size() - 1 < needNums) {
                System.err.printf("Not enough nums in input: " +
                        "expected %d matrix+vector nums, got %d.%n",
                        needNums, tokens.size() - 1);
                System.exit(3);
            }

            double[][] G = new double[N][N];
            for (int i = 0; i < N; i++) {
                for (int j = 0; j < N; ++j) {
                    if (!it.hasNext()) {
                        errorInput();
                    }
                    G[i][j] = it.next();
                }
            }

            // Vector x
            double[] x = new double[N];
            for (int i = 0; i < N; ++i) {
                if (!it.hasNext()) {
                    errorInput();
                }
                x[i] = it.next();
            }

            // Symmetric
            double eps = 1e-9;
            for (int i = 0; i < N; ++i) {
                for (int j = 0; j < N; ++j) {
                    if (Math.abs(G[i][j] - G[j][i]) > eps) {
                        System.err.printf("Matrix G is not symmetric:" +
                                " G[%d][%d] = %f, G[%d][%d] = %f%n",
                                i, j, G[i][j], j, i, G[j][i]);
                        System.exit(4);
                    }
                }
            }

            // y = G * x
            double[] y = new double[N];
            for (int i = 0; i < N; ++i) {
                double sum = 0.0;
                for (int j = 0; j < N; ++j) {
                    sum += G[i][j] * x[j];
                }
                y[i] = sum;
            }

            // s = x^T * y
            double s = 0.0;
            for (int i = 0; i < N; ++i) { s += x[i] * y[i]; }

            if (s < -1e-12) {
                System.err.println("Warning: quadtratic form is negative!");
            }

            double length = Math.sqrt(Math.max(0.0, s));
            System.out.printf(Locale.US, "Length = %.12g%n", length);
        } catch (IOException e) {
            System.err.println("I/O error: " + e.getMessage());
            System.exit(13);
        }
    }

    private static void errorInput() {
        System.err.println("Bad formatting or unexpected token!");
        System.exit(12);
    }

    /**
     * Считывание всех чисел из файла в порядке появления.
     */
    private static List<Double> readAll(Path path)
        throws IOException {
        List<Double> lout = new ArrayList<>();
        try (BufferedReader br = Files.newBufferedReader(path)) {
            String line;
            while ((line = br.readLine()) != null) {
                line = line.trim();
                if (line.isEmpty() || line.startsWith("#")) { continue; }

                // \s - любой пробельный символ
                //  + - несколько раз
                String[] parts = line.split("\\s+");

                for (String p : parts) {
                    if (p.isEmpty()) { continue; }
                    try {
                        lout.add(Double.parseDouble(p));
                    } catch (NumberFormatException ex) {
                        System.err.println("Non-numeric token ignored: '" +
                                p + "'");
                    }
                }
            }
        }
        return lout;
    }
}
