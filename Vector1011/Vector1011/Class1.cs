using System;
using System.IO;
using System.Text;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== IP Address Extractor ===\n");

        string inputFile = "input.txt";
        string outputFile = "output.txt";

        if (!File.Exists(inputFile))
        {
            Console.WriteLine($"Error: File '{inputFile}' not found!");
            Console.WriteLine("Creating sample input.txt...\n");
            CreateSampleInput(inputFile);
        }

        Console.WriteLine($"Reading from {inputFile}...");
        MyVector<string> lines = ReadLinesFromFile(inputFile);
        Console.WriteLine($"Read {lines.Size()} lines\n");

        if (lines.Size() == 0)
        {
            Console.WriteLine("Input file is empty. Nothing to process.");
            return;
        }

        MyVector<string> ipAddresses = new MyVector<string>();

        Console.WriteLine("Processing lines:");
        Console.WriteLine(new string('-', 60));

        for (int i = 0; i < lines.Size(); i++)
        {
            string line = lines.Get(i);
            Console.WriteLine($"Line {i + 1}: {line}");

            MyVector<string> ips = ExtractIPAddresses(line);

            if (ips.Size() > 0)
            {
                Console.WriteLine($"  Found {ips.Size()} valid IP(s):");
                for (int j = 0; j < ips.Size(); j++)
                {
                    string ip = ips.Get(j);
                    ipAddresses.Add(ip);
                    Console.WriteLine($"    ✓ {ip}");
                }
            }
            else
            {
                Console.WriteLine("    No valid IPs found");
            }
            Console.WriteLine();
        }

        Console.WriteLine(new string('-', 60));
        Console.WriteLine($"\nTotal extracted IPs: {ipAddresses.Size()}");

        if (ipAddresses.Size() > 0)
        {
            WriteLinesToFile(outputFile, ipAddresses);
            Console.WriteLine($"Results written to {outputFile}");

            Console.WriteLine("\nExtracted IP addresses:");
            for (int i = 0; i < ipAddresses.Size(); i++)
            {
                Console.WriteLine($"  {i + 1}. {ipAddresses.Get(i)}");
            }
        }
        else
        {
            Console.WriteLine("No IP addresses to write.");
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    static void CreateSampleInput(string filename)
    {
        try
        {
            using (StreamWriter sw = new StreamWriter(filename, false, Encoding.UTF8))
            {
                sw.WriteLine("My IP: 192.168.1.1, текст?");
                sw.WriteLine("Invalid: 256.1.1.1, 192.168.1.256, 192.168.001.1");
                sw.WriteLine("Edge cases: 121.121.121.121.2 and 111.111.111.1111");
                sw.WriteLine("Valid: 0.0.0.0 and 255.255.255.255");
                sw.WriteLine("Inside text192.168.0.1more text");
                sw.WriteLine("Multiple: 10.0.0.1 and 8.8.8.8 and 1.1.1.1");
                sw.WriteLine("Leading zeros: 192.168.01.1 (invalid)");
                sw.WriteLine("Localhost: 127.0.0.1");
            }
            Console.WriteLine("Sample input.txt created successfully!\n");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error creating sample file: {e.Message}");
        }
    }

    static MyVector<string> ReadLinesFromFile(string filename)
    {
        MyVector<string> result = new MyVector<string>();

        try
        {
            using (StreamReader sr = new StreamReader(filename, Encoding.UTF8))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    result.Add(line);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error reading file: {e.Message}");
        }

        return result;
    }

    static void WriteLinesToFile(string filename, MyVector<string> lines)
    {
        try
        {
            using (StreamWriter sw = new StreamWriter(filename, false, Encoding.UTF8))
            {
                for (int i = 0; i < lines.Size(); i++)
                {
                    sw.WriteLine(lines.Get(i));
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error writing file: {e.Message}");
        }
    }

    static MyVector<string> ExtractIPAddresses(string text)
    {
        MyVector<string> result = new MyVector<string>();

        int i = 0;
        while (i < text.Length)
        {
            if (char.IsDigit(text[i]))
            {
                // Проверяем границу ДО начала IP
                if (i > 0 && char.IsDigit(text[i - 1]))
                {
                    i++;
                    continue;
                }

                int startPos = i;
                string ip = TryExtractIP(text, ref i);

                if (ip != null)
                {
                    // Проверяем границу ПОСЛЕ IP
                    if (i < text.Length && char.IsDigit(text[i]))
                    {
                        // Пересечение - продолжаем с текущей позиции
                        continue;
                    }

                    result.Add(ip);
                }
                else
                {
                    // КРИТИЧНОСТЬ
                    i = startPos + 1;
                }
            }
            else
            {
                i++;
            }
        }

        return result;
    }

    static string TryExtractIP(string s, ref int i)
    {
        int start = i;
        int[] octets = new int[4];

        // Парсим 4 октета
        for (int octetIndex = 0; octetIndex < 4; octetIndex++)
        {
            if (!TryParseOctet(s, ref i, out octets[octetIndex]))
            {
                // НЕ откатываем i - оставляем там, где остановились
                return null;
            }

            // После октета должна быть точка (кроме последнего)
            if (octetIndex < 3)
            {
                if (i >= s.Length || s[i] != '.')
                {
                    return null;
                }
                i++;
            }
        }

        return $"{octets[0]}.{octets[1]}.{octets[2]}.{octets[3]}";
    }

    static bool TryParseOctet(string s, ref int i, out int value)
    {
        value = 0;
        int start = i;

        // Читаем цифры
        while (i < s.Length && char.IsDigit(s[i]))
        {
            value = value * 10 + (s[i] - '0');
            i++;

            // Защита от слишком длинных чисел
            if (i - start > 3)
            {
                return false;
            }
        }

        int len = i - start;

        // Пустой октет
        if (len == 0)
            return false;

        // Проверка диапазона
        if (value > 255)
            return false;

        // Проверка ведущих нулей
        if (len > 1 && s[start] == '0')
            return false;

        return true;
    }
}