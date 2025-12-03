using System;
using System.IO;

public class FileProcessor
{
    private int failCount = 0;

    public string GetNotificationPayload(string path)
    {
        failCount++;

        if (failCount <= 3)
            throw new IOException("Не вдалося зчитати файл (імітація помилки).");

        return "{ \"title\": \"Hello\", \"message\": \"Push OK\" }";
    }
}
