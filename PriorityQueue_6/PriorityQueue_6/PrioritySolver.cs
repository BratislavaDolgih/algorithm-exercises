using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriorityQueue_6
{
    public class Request : IComparable<Request>
    {
        public int Priority { get; }
        public int RequestNumber { get; } // Номер заявки (сквозная нумерация)
        public int StepIn { get; }        // Номер шага поступления в систему

        public Request(int priority, int requestNumber, int stepIn)
        {
            Priority = priority;
            RequestNumber = requestNumber;
            StepIn = stepIn;
        }

        public int CompareTo(Request other)
        {
            if (other == null) return 1;

            // Если this.Priority > other.Priority, то Priority.CompareTo вернет > 0.
            // Мы возвращаем < 0, чтобы Min-Heap поставила "больший" приоритет в начало.
            int priorityComparison = other.Priority.CompareTo(Priority);

            if (priorityComparison != 0)
            {
                return priorityComparison;
            }

            // Если приоритеты равны, для стабильности Min-Heap
            // можно сравнивать по номеру заявки (RequestNumber)
            return RequestNumber.CompareTo(other.RequestNumber);
        }

        public override string ToString()
        {
            return $"Номер: {RequestNumber}, Приоритет: {Priority}, Поступила на шаге: {StepIn}";
        }
    }

    public class PriorityQueueSolver
    {
        private MyPriorityQueue<Request> _queue;
        private Random _random = new Random();
        private int _requestCounter = 1; // Сквозная нумерация заявок
        private const string LogFileName = "log.txt";

        public PriorityQueueSolver()
        {
            // Инициализируем очередь, используя естественный порядок сортировки Request
            _queue = new MyPriorityQueue<Request>();
        }

        public void RunSimulation(int nSteps)
        {
            // Очищаем файл лога перед началом
            System.IO.File.WriteAllText(LogFileName, string.Empty);

            // 1. Шаги с генерацией
            for (int step = 1; step <= nSteps; step++)
            {
                GenerateAndAddRequests(step);

                // Удаление заявки с наибольшим приоритетом (Max-Heap)
                if (!_queue.IsEmpty())
                {
                    RemoveRequest(step);
                }
            }

            // 2. Шаги без генерации (только удаление)
            int currentStep = nSteps + 1;
            while (!_queue.IsEmpty())
            {
                RemoveRequest(currentStep);
                currentStep++;
            }

            FindMaxWaitTime(currentStep - 1);
        }

        private void GenerateAndAddRequests(int step)
        {
            int count = _random.Next(1, 11); // От 1 до 10 заявок

            for (int i = 0; i < count; i++)
            {
                int priority = _random.Next(1, 6); // От 1 до 5
                Request newRequest = new Request(priority, _requestCounter++, step);

                _queue.Add(newRequest);
                LogAction("ADD", newRequest, step);
            }
        }

        private void RemoveRequest(int step)
        {
            Request removedRequest = _queue.Poll();
            if (removedRequest != null)
            {
                LogAction("REMOVE", removedRequest, step);
            }
        }

        private void LogAction(string action, Request request, int step)
        {
            // Структура: ADD/REMOVE НомерЗаявки Приоритет НомерШага
            string logEntry = $"{action} {request.RequestNumber} {request.Priority} {step}";
            System.IO.File.AppendAllText(LogFileName, logEntry + Environment.NewLine);
        }

        private void FindMaxWaitTime(int finalStep)
        {
            // Чтобы отследить удаленные заявки и найти среди них заявку с максимальным временем,
            // нам нужен журнал, где мы сопоставим ADD и REMOVE.
            // Я буду использовать лог-файл, как единственный источник данных о завершенных заявках.

            Dictionary<int, int> addSteps = new Dictionary<int, int>(); // НомерЗаявки -> ШагПоступления
            Dictionary<int, int> removeSteps = new Dictionary<int, int>(); // НомерЗаявки -> ШагУдаления

            string[] logLines = System.IO.File.ReadAllLines(LogFileName);

            foreach (string line in logLines)
            {
                string[] parts = line.Split(' ');
                if (parts.Length < 4) continue;

                string action = parts[0];
                int reqNum = int.Parse(parts[1]);
                // int priority = int.Parse(parts[2]);  не нужен
                int step = int.Parse(parts[3]);

                if (action == "ADD")
                {
                    addSteps[reqNum] = step;
                }
                else if (action == "REMOVE")
                {
                    removeSteps[reqNum] = step;
                }
            }

            long maxWaitTime = -1;
            int maxWaitRequestNumber = -1;

            // Ищем заявки, которые были удалены (REMOVE)
            foreach (var entry in removeSteps)
            {
                int reqNum = entry.Key;
                int stepRemoved = entry.Value;

                if (addSteps.TryGetValue(reqNum, out int stepAdded))
                {
                    // Время ожидания = ШагУдаления - ШагПоступления
                    long waitTime = stepRemoved - stepAdded;

                    if (waitTime > maxWaitTime)
                    {
                        maxWaitTime = waitTime;
                        maxWaitRequestNumber = reqNum;
                    }
                }
            }

            // Поиск информации о заявке с максимальным временем (требуется перепарсить лог, чтобы найти ее данные)
            Request maxWaitRequestInfo = null;
            if (maxWaitRequestNumber != -1)
            {
                // Ищем информацию о заявке в логе
                foreach (string line in logLines)
                {
                    if (line.StartsWith("ADD"))
                    {
                        string[] parts = line.Split(' ');
                        if (int.Parse(parts[1]) == maxWaitRequestNumber)
                        {
                            maxWaitRequestInfo = new Request(
                                int.Parse(parts[2]), // Приоритет
                                int.Parse(parts[1]), // Номер
                                int.Parse(parts[3])  // Шаг поступления
                            );
                            break;
                        }
                    }
                }
            }

            Console.WriteLine("\n" + new string('=', 40));
            Console.WriteLine($"Симуляция завершена. Всего шагов: {finalStep}.");
            Console.WriteLine($"Максимальное время ожидания: {maxWaitTime} шагов.");

            if (maxWaitRequestInfo != null)
            {
                Console.WriteLine("Информация о заявке, ожидавшей максимальное время:");
                Console.WriteLine($"\t{maxWaitRequestInfo}");
            }
            else
            {
                Console.WriteLine("Заявки, прошедшие через систему, не найдены.");
            }
            Console.WriteLine(new string('=', 40));
        }
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("Введите количество шагов N для генерации заявок:");

            if (int.TryParse(Console.ReadLine(), out int nSteps) && nSteps > 0)
            {
                // Здесь мы используем твою собственную реализацию MyPriorityQueue
                var solver = new PriorityQueueSolver();
                solver.RunSimulation(nSteps);
            }
            else
            {
                Console.WriteLine("Некорректный ввод. Пожалуйста, введите положительное целое число.");
            }
        }
    }
}
