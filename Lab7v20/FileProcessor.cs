using System;
using System.IO;

public class FileProcessor
{
    private int failCount = 0;

    public string GetNotificationPayload(string path)
    {
        failCount++;

        // Імітація помилки 3 рази
        if (failCount <= 3)
            throw new IOException("Помилка читання файлу (імітація).");

        return "{ \"title\": \"Hello\", \"message\": \"Push OK\" }";
    }
}
