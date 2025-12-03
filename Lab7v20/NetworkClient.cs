using System;
using System.Net.Http;

public class NetworkClient
{
    private int failCount = 0;

    public void SendPushNotification(string deviceId, string payload)
    {
        failCount++;

        // Імітація помилки 2 рази
        if (failCount <= 2)
            throw new HttpRequestException("Помилка мережі (імітація).");

        Console.WriteLine($"Push надіслано успішно до {deviceId}!");
    }
}
