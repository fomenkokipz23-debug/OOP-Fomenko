using System;
using System.IO;
using System.Net.Http;

class Program
{
    static void Main()
    {
        var fileProcessor = new FileProcessor();
        var networkClient = new NetworkClient();

        Console.WriteLine("=== Отримання payload з файлу ===");

        string payload = RetryHelper.ExecuteWithRetry(
            () => fileProcessor.GetNotificationPayload("data.json"),
            retryCount: 5,
            initialDelay: TimeSpan.FromSeconds(1),
            shouldRetry: ex => ex is IOException
        );

        Console.WriteLine($"Отримано payload: {payload}\n");

        Console.WriteLine("=== Надсилання push-сповіщення ===");

        RetryHelper.ExecuteWithRetry<object>(
            () =>
            {
                networkClient.SendPushNotification("DEVICE_123", payload);
                return null;
            },
            retryCount: 4,
            initialDelay: TimeSpan.FromSeconds(1),
            shouldRetry: ex => ex is HttpRequestException
        );

        Console.WriteLine("\nГотово!");
    }
}
