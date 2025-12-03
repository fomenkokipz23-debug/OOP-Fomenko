using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

public class Program
{
    private const int Million = 1_000_000;
    private const int MinValue = 1;
    private const int MaxValue = 1000;

    // Обчислювально інтенсивна функція: Перевірка на просте число
    static bool IsPrime(int number)
    {
        if (number <= 1) return false;
        if (number == 2) return true;
        if (number % 2 == 0) return false;

        var boundary = (int)Math.Floor(Math.Sqrt(number));

        for (int i = 3; i <= boundary; i = i + 2)
            if (number % i == 0)
                return false;

        return true;
    }

    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine(" Самостійна робота №12: Дослідження PLINQ");
        Console.WriteLine("---------------------------------------------");

        // Розміри колекцій для експериментів
        int[] collectionSizes = { 1 * Million, 5 * Million, 10 * Million };

        // ----------------------------------------------------
        // ЗАВДАННЯ 4: Порівняння продуктивності LINQ та PLINQ
        // ----------------------------------------------------
        Console.WriteLine("\n## Порівняння продуктивності LINQ vs PLINQ (IsPrime)");

        foreach (int size in collectionSizes)
        {
            List<int> data = GenerateRandomData(size);
            Console.WriteLine($"\n--- Колекція: {size:N0} елементів ---");

            // 1. LINQ (Послідовне виконання)
            var stopwatchLinq = Stopwatch.StartNew();
            var linqResults = data
                .Where(x => IsPrime(x))
                .ToList();
            stopwatchLinq.Stop();

            Console.WriteLine($"LINQ (Послідовно): {stopwatchLinq.ElapsedMilliseconds} мс");

            // 2. PLINQ (Паралельне виконання)
            var stopwatchPlinq = Stopwatch.StartNew();
            var plinqResults = data
                .AsParallel() // Перехід до паралельної обробки
                .Where(x => IsPrime(x))
                .ToList();
            stopwatchPlinq.Stop();

            Console.WriteLine($"PLINQ (Паралельно): {stopwatchPlinq.ElapsedMilliseconds} мс");
        }

        // ----------------------------------------------------
        // ЗАВДАННЯ 5: Дослідження потенційних проблем безпеки
        // ----------------------------------------------------
        Console.WriteLine("\n##  Дослідження проблем безпеки (Побічні ефекти)");

        int problemSize = 10 * Million;
        List<int> problemData = GenerateRandomData(problemSize);
        long expectedCount = problemSize;

        // --- Сценарій A: Небезпечна модифікація спільної змінної ---
        long unsafeCounter = 0;
        Console.WriteLine($"\n--- Сценарій A: Небезпечна модифікація (без lock) ---");
        Console.WriteLine($"Очікуваний результат: {expectedCount:N0}");

        problemData.AsParallel().ForAll(x =>
        {
            // Увага! Тут виникає стан гонитви (race condition)
            unsafeCounter++; 
        });

        Console.WriteLine($"Фактичний результат (unsafe): {unsafeCounter:N0}");
        Console.WriteLine($"Втрачено інкрементацій: {expectedCount - unsafeCounter:N0}");

        // --- Сценарій B: Виправлення за допомогою Interlocked ---
        long safeCounter = 0;
        Console.WriteLine($"\n--- Сценарій B: Виправлення (з Interlocked) ---");
        
        problemData.AsParallel().ForAll(x =>
        {
            // Використання Interlocked гарантує атомарність операції
            Interlocked.Increment(ref safeCounter);
        });

        Console.WriteLine($"Фактичний результат (safe): {safeCounter:N0}");
        Console.WriteLine($"Втрачено інкрементацій: {expectedCount - safeCounter:N0}"); 
        
        Console.WriteLine("\n---------------------------------------------");
        Console.WriteLine(" Виконання самостійної роботи завершено. Заповніть звіт у коментарях!");
    }

    /// <summary>
    /// Заповнює список випадковими числами.
    /// </summary>
    static List<int> GenerateRandomData(int count)
    {
        // Не використовуємо PLINQ для генерації, щоб не впливати на подальші виміри
        var random = new Random();
        var data = new List<int>(count);
        for (int i = 0; i < count; i++)
        {
            data.Add(random.Next(MinValue, MaxValue));
        }
        return data;
    }
}