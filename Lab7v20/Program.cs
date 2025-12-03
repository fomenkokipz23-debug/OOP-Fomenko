using System;
using System.IO;
using System.Net.Http;
using Polly;
using Polly.Retry;

class Program
{
    static void Main()
    {
        var fileProcessor = new FileProcessor();
        var networkClient = new NetworkClient();

        // Політика Retry для IO + Http помилок
        var retryPolicy = Policy
            .Handle<IOException>()
            .Or<HttpRequestException>()
            .WaitAndRetry(
                retryCount: 5,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt - 1)),
                onRetry: (exception, delay, attempt, ctx) =>
                {
                    Console.WriteLine($"Спроба #{attempt}, затримка: {delay.TotalSeconds} c");
                    Console.WriteLine($"Причина: {exception.GetType().Name} — {exception.Message}\n");
                });

        Console.WriteLine("=== Отримання payload з файлу ===");

        string payload = retryPolicy.Execute(() =>
        {
            return fileProcessor.GetNotificationPayload("data.json");
        });

        Console.WriteLine($"Payload отримано: {payload}\n");

        Console.WriteLine("=== Надсилання push-сповіщення ===");

        retryPolicy.Execute(() =>
        {
            networkClient.SendPushNotification("DEVICE_123", payload);
        });

        Console.WriteLine("\nГотово!");
    }
}
