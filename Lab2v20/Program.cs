using System;
using System.Collections.Generic;

public class Dictionary
{
    // Приватне поле для зберігання даних словника
    private Dictionary<string, string> _data = new Dictionary<string, string>();

    // Індексатор для доступу до значень за ключем
    public string this[string key]
    {
        get => _data.ContainsKey(key) ? _data[key] : string.Empty; // Повертає значення або порожній рядок
        set => _data[key] = value; // Додає або оновлює значення за ключем
    }

    // Перевантаження оператора + для додавання пари ключ-значення
    public static Dictionary operator +(Dictionary dict, (string key, string value) pair)
    {
        dict._data[pair.key] = pair.value; // Додає пару ключ-значення до словника
        return dict;
    }

    // Перевантаження оператора - для видалення пари за ключем
    public static Dictionary operator -(Dictionary dict, string key)
    {
        dict._data.Remove(key); // Видаляє пару ключ-значення зі словника
        return dict;
    }

    // Метод для відображення всіх пар ключ-значення
    public void Display()
    {
        if (_data.Count == 0)
        {
            Console.WriteLine("Словник порожній."); // Повідомлення, якщо словник порожній
            return;
        }

        foreach (var kvp in _data)
        {
            Console.WriteLine($"{kvp.Key}: {kvp.Value}"); // Виводить ключ і значення
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Створення об'єкта словника
        Dictionary dict = new Dictionary();

        // Додавання пар ключ-значення за допомогою оператора +
        dict += ("Name", "John");
        dict += ("Age", "25");

        // Відображення всіх пар ключ-значення
        Console.WriteLine("Словник після додавання пар:");
        dict.Display();

        // Доступ до значення за ключем через індексатор
        Console.WriteLine($"Значення для ключа 'Name': {dict["Name"]}");

        // Видалення пари ключ-значення за допомогою оператора -
        dict -= "Age";

        // Відображення словника після видалення
        Console.WriteLine("Словник після видалення 'Age':");
        dict.Display();
    }
}