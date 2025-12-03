using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab6
{
    // Власний делегат для порівняння рейтингів
    delegate bool RatingComparison(double rating1, double rating2);

    class Book
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }
        public double Rating { get; set; }

        public Book(string title, string author, int year, double rating)
        {
            Title = title;
            Author = author;
            Year = year;
            Rating = rating;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Лабораторна робота №6 ===");
            Console.WriteLine("Тема: Лямбда-вирази, анонімні функції та делегати у C#");
            Console.WriteLine("-----------------------------------------------\n");

            // Створюємо колекцію книг

            List<Book> books = new List<Book>
            {
                new Book("Мистецтво війни", "Сунь-Цзи", -500, 4.8),
                new Book("1984", "Джордж Орвелл", 1949, 4.9),
                new Book("Майстер і Маргарита", "Михайло Булгаков", 1967, 4.6),
                new Book("Кобзар", "Тарас Шевченко", 1840, 4.7),
                new Book("Тореадори з Васюківки", "Всеволод Нестайко", 1965, 4.4),
                new Book("Анна Кареніна", "Лев Толстой", 1877, 4.5),
                new Book("Фауст", "Йоганн Ґете", 1808, 4.3)
            };

            // 1️ Власний делегат (RatingComparison)

            RatingComparison isHigherRating = (r1, r2) => r1 > r2;
            Console.WriteLine("Порівняння рейтингів (власний делегат):");
            Console.WriteLine($"Чи рейтинг 4.9 вищий за 4.5? {isHigherRating(4.9, 4.5)}\n");

            // 2️ Анонімний метод (підрахунок кількості старих книжок)

            Func<List<Book>, int> countOldBooks = delegate (List<Book> list)
            {
                return list.Count(b => b.Year < 1900);
            };
            Console.WriteLine($"Кількість книжок, виданих до 1900 року: {countOldBooks(books)}\n");

            // 3️ Predicate<Book> — перевірка рейтингу > 4.5

            Predicate<Book> highRated = b => b.Rating > 4.5;
            var bestBooks = books.FindAll(highRated);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Книги з рейтингом > 4.5:");
            Console.ResetColor();
            bestBooks.ForEach(b => Console.WriteLine($"{b.Title} ({b.Author}) - {b.Rating}★"));
            Console.WriteLine();

            // 4 Func<Book, string> — створення текстового опису

            Func<Book, string> describeBook = b => $"{b.Title} ({b.Year}) — {b.Author}, рейтинг: {b.Rating}";
            Console.WriteLine("Опис кожної книги (через Func):");
            books.ForEach(b => Console.WriteLine(describeBook(b)));
            Console.WriteLine();

            // 5️ Action<Book> — вивід короткого формату у консоль

            Action<Book> printShort = b => Console.WriteLine($"{b.Author}: {b.Title}");
            Console.WriteLine("Короткий список (через Action):");
            books.ForEach(printShort);
            Console.WriteLine();

            // 6️ Використання LINQ + лямбд

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("LINQ-операції:\n");
            Console.ResetColor();

            // Фільтрація книг із рейтингом > 4.5
            var filtered = books.Where(b => b.Rating > 4.5);

            // Групування за автором
            var grouped = filtered.GroupBy(b => b.Author);

            // Сортування за роком видання
            var sorted = books.OrderBy(b => b.Year);

            Console.WriteLine(" Книги з рейтингом > 4.5, згруповані за автором:");
            foreach (var group in grouped)
            {
                Console.WriteLine($"\nАвтор: {group.Key}");
                foreach (var book in group)
                {
                    Console.WriteLine($"  - {book.Title} ({book.Rating}★)");
                }
            }

            Console.WriteLine("\n Усі книги, відсортовані за роком видання:");
            foreach (var b in sorted)
            {
                Console.WriteLine($"{b.Year}: {b.Title}");
            }

            // 7️ Aggregate — обчислення середнього рейтингу

            double avgRating = books.Select(b => b.Rating).Average();
            Console.WriteLine($"\nСередній рейтинг усіх книг: {avgRating:F2}");

            Console.WriteLine("\n=== Кінець лабораторної роботи №6 ===");
        }
    }
}
