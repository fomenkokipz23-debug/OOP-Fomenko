using System;
using System.Threading;

public static class RetryHelper
{
    public static T ExecuteWithRetry<T>(
        Func<T> operation,
        int retryCount = 3,
        TimeSpan initialDelay = default,
        Func<Exception, bool> shouldRetry = null)
    {
        if (initialDelay == default)
            initialDelay = TimeSpan.FromSeconds(1);

        int attempt = 0;

        while (true)
        {
            try
            {
                attempt++;
                Console.WriteLine($"Спроба #{attempt}");
                return operation();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.GetType().Name} — {ex.Message}");

                if (attempt > retryCount || (shouldRetry != null && !shouldRetry(ex)))
                {
                    Console.WriteLine("Повторні спроби припинені.");
                    throw;
                }

                // експоненційна затримка
                var delay = TimeSpan.FromMilliseconds(initialDelay.TotalMilliseconds * Math.Pow(2, attempt - 1));
                Console.WriteLine($"Очікування {delay.TotalSeconds} секунд...\n");

                Thread.Sleep(delay);
            }
        }
    }
}
