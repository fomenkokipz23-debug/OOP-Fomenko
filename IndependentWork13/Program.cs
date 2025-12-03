using System;
using System.Threading;
using Polly;
using System.Net.Http;
// Ця директива потрібна для Circuit Breaker та Timeout
using Polly.CircuitBreaker;
using Polly.Timeout;
using System.Threading.Tasks; 


public class Program
{
    // Лічильник для імітації помилок у Сценарії 1
    private static int _apiCallAttempts = 0;
    // Об'єкт-заглушка для імітації даних у Сценарії 3
    private static readonly Random _random = new Random();
    
    // СЦЕНАРІЙ 1: Retry (Тимчасова недоступність API)

    /// <summary> Імітує виклик зовнішнього API, який тимчасово видає помилки. </summary>
    public static string CallExternalApi(string url)
    {
        _apiCallAttempts++;
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}]  Спроба {_apiCallAttempts}: Виклик API {url}...");

        if (_apiCallAttempts <= 2) // Імітуємо 2 невдачі
        {
            throw new HttpRequestException($"API call failed for {url}");
        }

        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}]  API call to {url} successful!");
        return "Data from API";
    }

    public static void Scenario1_Retry()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n--- СЦЕНАРІЙ 1: Retry (WaitAndRetry) ---");
        Console.ResetColor();
        Console.WriteLine("Проблема: Зовнішній API може тимчасово повертати помилки.");

        var retryPolicy = Policy
            .Handle<HttpRequestException>() // Обробляємо лише цей тип помилки
            .WaitAndRetry(
                3, // Максимум 3 повторні спроби
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Експоненційна затримка (2с, 4с, 8с)
                (exception, timeSpan, retryCount, context) =>
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Retry {retryCount} після {timeSpan.TotalSeconds:F1}с через: {exception.Message}");
                });

        try
        {
            string result = retryPolicy.Execute(() => CallExternalApi("https://api.example.com/data"));
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}]  Фінальний результат: {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}]  Операція провалилася після всіх спроб: {ex.Message}");
        }
    }

    // СЦЕНАРІЙ 2: Circuit Breaker (Захист від постійних збоїв)

    /// <summary> Імітує виклик бази даних, який завжди завершується помилкою. </summary>
    public static string CallDatabase()
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Спроба доступу до бази даних...");
        // Імітуємо помилку підключення/запиту
        throw new InvalidOperationException("Database connection is permanently broken.");
    }

    public static void Scenario2_CircuitBreaker()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n--- СЦЕНАРІЙ 2: Circuit Breaker ---");
        Console.ResetColor();
        Console.WriteLine("Проблема: Захист системи від постійних, неминучих збоїв зовнішнього сервісу.");

        // Політика: розірвати ланцюг після 2 послідовних помилок на 5 секунд.
        var circuitBreakerPolicy = Policy
            .Handle<InvalidOperationException>()
            .CircuitBreaker(
                exceptionsAllowedBeforeBreaking: 2,
                durationOfBreak: TimeSpan.FromSeconds(5),
                onBreak: (exception, duration) =>
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}]  CIRCUIT OPENED! Помилок: {exception.Message}. Перерва на {duration.TotalSeconds}с.");
                    Console.ResetColor();
                },
                onReset: () =>
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}]  CIRCUIT CLOSED! Сервіс відновлено.");
                    Console.ResetColor();
                });

        // 5 спроб для демонстрації.
        for (int i = 1; i <= 5; i++)
        {
            try
            {
                // Запуск політики
                circuitBreakerPolicy.Execute(() => CallDatabase());
            }
            catch (BrokenCircuitException ex)
            {
                // Помилка, коли ланцюг розірвано.
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}]  Спроба {i}: Перехоплено Circuit Breaker. Виклик не відбувся. ({ex.GetType().Name})");
            }
            catch (Exception ex)
            {
                // Помилка під час перших 2 спроб.
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}]  Спроба {i}: Фактична помилка. ({ex.GetType().Name})");
            }
            Thread.Sleep(500); // Невелика затримка між спробами
        }
    }

    // СЦЕНАРІЙ 3: Timeout (Операція, яка може "зависнути")
    
    /// <summary> Імітує повільний сервіс, що займає 1000 мс. </summary>
    public static int CallSlowService()
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}]  Початок повільної операції...");
        Thread.Sleep(1000); // Операція займає 1000 мс
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}]  Повільна операція завершена.");
        return _random.Next(1, 100);
    }

    public static void Scenario3_Timeout()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n--- СЦЕНАРІЙ 3: Timeout (Optimistic) ---");
        Console.ResetColor();
        Console.WriteLine("Проблема: Запобігання блокуванню ресурсів через занадто тривалі операції.");

        var timeoutPolicy = Policy
            .Timeout(
                TimeSpan.FromMilliseconds(500), // Обмеження в 500 мс
                TimeoutStrategy.Optimistic, // Дозволяє операції завершитися у фоновому режимі
                onTimeout: (context, timespan, task) =>
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ⏱ TIMEOUT! Операція перевищила ліміт у {timespan.TotalMilliseconds} мс.");
                    Console.ResetColor();
                    // Повернення Task.CompletedTask є обов'язковим для void-делегата
                    return Task.CompletedTask; 
                });

        try
        {
            int result = timeoutPolicy.Execute(() => CallSlowService());
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}]  Фінальний результат: {result}");
        }
        catch (TimeoutRejectedException ex)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}]  Перехоплено виняток: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}]  Неочікувана помилка: {ex.Message}");
        }
    }

    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine(" Самостійна робота №13: Polly/Retry Кейси");
        Console.WriteLine("---------------------------------------------");

        Scenario1_Retry();
        Scenario2_CircuitBreaker();
        Scenario3_Timeout();

        Console.WriteLine("\n---------------------------------------------");
        Console.WriteLine(" Виконання самостійної роботи завершено. Перегляньте детальний звіт у коментарях на початку файлу Program.cs.");
    }
}