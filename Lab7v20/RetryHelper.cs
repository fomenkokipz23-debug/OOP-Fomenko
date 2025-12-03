using Polly;
using Polly.Retry;
using System;

public static class RetryHelper
{
    // Політика автоматичного повтору
    private static readonly AsyncRetryPolicy retryPolicy = Policy
        .Handle<Exception>()                     // Перехоплюємо будь-який Exception
        .WaitAndRetryAsync(
            retryCount: 3,                      // Кількість повторів
            sleepDurationProvider: attempt => 
                TimeSpan.FromSeconds(Math.Pow(2, attempt)), // Експоненційна пауза 2s, 4s, 8s
            onRetry: (exception, time, attempt, context) =>
            {
                Console.WriteLine(
                    $"Спроба #{attempt}. Затримка {time.TotalSeconds}с. Помилка: {exception.Message}");
            });

    /// <summary>
    /// Виконує дію з автоматичним повтором.
    /// </summary>
    public static async Task ExecuteAsync(Func<Task> action)
    {
        await retryPolicy.ExecuteAsync(async () =>
        {
            await action();
        });
    }

    /// <summary>
    /// Виконує функцію з поверненням результату з автоматичним повтором.
    /// </summary>
    public static async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
    {
        return await retryPolicy.ExecuteAsync(async () =>
        {
            return await action();
        });
    }
}
