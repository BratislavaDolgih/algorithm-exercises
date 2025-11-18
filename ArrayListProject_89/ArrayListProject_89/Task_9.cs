using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArrayListProject_89
{
    using System;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    // Класс для представления тега
    public class HtmlTag
    {
        public string NormalizedName { get; private set; }
        public string OriginalTag { get; private set; }

        public HtmlTag(string tag)
        {
            OriginalTag = tag;
            NormalizedName = NormalizeTag(tag);
        }

        // Нормализация: убираем <, >, /, приводим к нижнему регистру
        private string NormalizeTag(string tag)
        {
            string cleaned = tag.Trim('<', '>', '/');
            return cleaned.ToLower();
        }

        public override bool Equals(object obj)
        {
            if (obj is HtmlTag other)
                return NormalizedName == other.NormalizedName;
            return false;
        }

        public override int GetHashCode()
        {
            return NormalizedName.GetHashCode();
        }

        public override string ToString()
        {
            return OriginalTag;
        }
    }

    public class TagParser
    {
        // Регулярное выражение для валидации тега
        // < - начало
        // /? - необязательный слеш
        // [a-zA-Z] - первый символ обязательно буква
        // [a-zA-Z0-9]* - остальные символы буквы или цифры
        // > - конец
        private static readonly Regex TagRegex = new Regex(@"</?[a-zA-Z][a-zA-Z0-9]*>", RegexOptions.Compiled);

        // Извлечение тегов из строки
        public static MyArrayList<HtmlTag> ExtractTags(string line)
        {
            MyArrayList<HtmlTag> tags = new MyArrayList<HtmlTag>();

            if (string.IsNullOrEmpty(line))
                return tags;

            MatchCollection matches = TagRegex.Matches(line);
            foreach (Match match in matches)
            {
                tags.Add(new HtmlTag(match.Value));
            }

            return tags;
        }

        // Удаление дубликатов с учётом нормализации
        public static MyArrayList<HtmlTag> RemoveDuplicates(MyArrayList<HtmlTag> tags)
        {
            MyArrayList<HtmlTag> uniqueTags = new MyArrayList<HtmlTag>();

            for (int i = 0; i < tags.Size(); i++)
            {
                HtmlTag currentTag = tags.Get(i);
                bool isDuplicate = false;

                // Проверяем, есть ли уже такой тег в списке уникальных
                for (int j = 0; j < uniqueTags.Size(); j++)
                {
                    if (currentTag.Equals(uniqueTags.Get(j)))
                    {
                        isDuplicate = true;
                        break;
                    }
                }

                if (!isDuplicate)
                {
                    uniqueTags.Add(currentTag);
                }
            }

            return uniqueTags;
        }

        // Основной метод обработки файла
        public static void ProcessFile(string inputPath, string outputPath)
        {
            if (!File.Exists(inputPath))
            {
                throw new FileNotFoundException($"Файл {inputPath} не найден");
            }

            MyArrayList<HtmlTag> allTags = new MyArrayList<HtmlTag>();

            try
            {
                // Читаем файл построчно
                using (StreamReader reader = new StreamReader(inputPath, Encoding.UTF8))
                {
                    string line;
                    int lineNumber = 0;

                    while ((line = reader.ReadLine()) != null)
                    {
                        lineNumber++;
                        Console.WriteLine($"Обработка строки {lineNumber}: {line}");

                        MyArrayList<HtmlTag> lineTags = ExtractTags(line);

                        // Добавляем все найденные теги в общий список
                        for (int i = 0; i < lineTags.Size(); i++)
                        {
                            allTags.Add(lineTags.Get(i));
                        }

                        Console.WriteLine($"  Найдено тегов: {lineTags.Size()}");
                    }
                }

                Console.WriteLine($"\nВсего тегов до удаления дубликатов: {allTags.Size()}");

                // Удаляем дубликаты
                MyArrayList<HtmlTag> uniqueTags = RemoveDuplicates(allTags);

                Console.WriteLine($"Уникальных тегов: {uniqueTags.Size()}");

                // Записываем результат в файл
                using (StreamWriter writer = new StreamWriter(outputPath, false, Encoding.UTF8))
                {
                    for (int i = 0; i < uniqueTags.Size(); i++)
                    {
                        writer.WriteLine(uniqueTags.Get(i).OriginalTag);
                    }
                }

                Console.WriteLine($"\nРезультат записан в {outputPath}");

                // Выводим результат в консоль
                Console.WriteLine("\nУникальные теги:");
                for (int i = 0; i < uniqueTags.Size(); i++)
                {
                    Console.WriteLine($"  {i + 1}. {uniqueTags.Get(i).OriginalTag} (нормализовано: {uniqueTags.Get(i).NormalizedName})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обработке файла: {ex.Message}");
                throw;
            }
        }
    }

    // Программа для тестирования
    public class Task
    {
        public static void Main()
        {
            try
            {
                // Создаём тестовый файл input.txt
                CreateTestFile();

                // Обрабатываем файл
                TagParser.ProcessFile("input.txt", "output.txt");

                Console.WriteLine("\n--- Детальный анализ ---");
                DemonstrateNormalization();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критическая ошибка: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        // Создание тестового файла
        private static void CreateTestFile()
        {
            string[] testLines = new string[]
            {
            "<html><body><H1>Заголовок</H1></body></html>",
            "<div><p>Текст</p></div>",
            "<HTML></Body><h1>ДругойЗаголовок</h1>",
            "<table><tr><td>Ячейка</td></tr></table>",
            "</HtMl><BODY><P></p>",
            "<span123><div456></span123>",
            "НетТегов<br><hr/>Текст",
            "< invalid><valid123></valid123>"
            };

            using (StreamWriter writer = new StreamWriter("input.txt", false, Encoding.UTF8))
            {
                foreach (string line in testLines)
                {
                    writer.WriteLine(line);
                }
            }

            Console.WriteLine("Тестовый файл input.txt создан\n");
        }

        // Демонстрация нормализации
        private static void DemonstrateNormalization()
        {
            string[] examples = { "<html>", "</HTML>", "<HtMl>", "</htML>" };

            Console.WriteLine("Примеры нормализации:");
            foreach (string example in examples)
            {
                HtmlTag tag = new HtmlTag(example);
                Console.WriteLine($"  {example} -> {tag.NormalizedName}");
            }

            Console.WriteLine("\nСравнение:");
            HtmlTag tag1 = new HtmlTag("<html>");
            HtmlTag tag2 = new HtmlTag("</HTML>");
            Console.WriteLine($"  <html> == </HTML>? {tag1.Equals(tag2)}");

            HtmlTag tag3 = new HtmlTag("<body>");
            Console.WriteLine($"  <html> == <body>? {tag1.Equals(tag3)}");
        }
    }
}
