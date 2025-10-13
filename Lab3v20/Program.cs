using System;
using System.Collections.Generic;

// Базовий клас
public abstract class TimeSpanBase
{
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }

    public TimeSpanBase(TimeSpan start, TimeSpan end)
    {
        Start = start;
        End = end;
    }

    // Віртуальний метод — буде перевизначений у нащадках
    public virtual TimeSpan GetDuration()
    {
        return End - Start;
    }

    public abstract void ShowInfo();
}

// Похідний клас 1: Заняття
public class LessonTime : TimeSpanBase
{
    public string Subject { get; set; }

    public LessonTime(string subject, TimeSpan start, TimeSpan end)
        : base(start, end) { Subject = subject; }

    public override void ShowInfo()
    {
        Console.WriteLine($"Заняття: {Subject}, тривалість {GetDuration().TotalMinutes} хв");
    }
}

// Похідний клас 2: Перерва
public class BreakTime : TimeSpanBase
{
    public BreakTime(TimeSpan start, TimeSpan end)
        : base(start, end) { }

    public override void ShowInfo()
    {
        Console.WriteLine($"Перерва: {GetDuration().TotalMinutes} хв");
    }
}

// Демонстрація роботи
class Program
{
    static void Main()
    {
        List<TimeSpanBase> schedule = new List<TimeSpanBase>
        {
            new LessonTime("Математика", new TimeSpan(8, 30, 0), new TimeSpan(9, 15, 0)),
            new BreakTime(new TimeSpan(9, 15, 0), new TimeSpan(9, 25, 0)),
            new LessonTime("Інформатика", new TimeSpan(9, 25, 0), new TimeSpan(10, 10, 0)),
            new BreakTime(new TimeSpan(10, 10, 0), new TimeSpan(10, 20, 0)),
            new LessonTime("Фізика", new TimeSpan(10, 20, 0), new TimeSpan(11, 5, 0))
        };

        TimeSpan totalLessons = TimeSpan.Zero;
        TimeSpan totalBreaks = TimeSpan.Zero;

        Console.WriteLine("=== Розклад дня ===");
        foreach (var item in schedule)
        {
            item.ShowInfo();

            // Демонстрація поліморфізму
            if (item is LessonTime) totalLessons += item.GetDuration();
            else if (item is BreakTime) totalBreaks += item.GetDuration();
        }

        Console.WriteLine("\n=== Підсумок ===");
        Console.WriteLine($"Загальна тривалість занять: {totalLessons.TotalMinutes} хв");
        Console.WriteLine($"Загальна тривалість перерв: {totalBreaks.TotalMinutes} хв");
    }
}
