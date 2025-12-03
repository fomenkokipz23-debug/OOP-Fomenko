using System;
using System.Net.Http;

public class NetworkClient
{
    private int failCount = 0;

    public void SendPushNotification(string deviceId, string payload)
    {
        failCount++;

        if (failCount <= 2)
            throw new HttpRequestException("Помилка мережі (імітація).");

        Console.WriteLine($"Push до {deviceId} надіслано успішно!");
    }
}
