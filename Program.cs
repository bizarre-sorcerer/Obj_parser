using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;


// Класс парсер
class ObjParser
{
    // Статический метод (Не обязательно создавать экземпляр класса для его вызова) читает файл
    public static void getAllText()
    {
        //Путь к файлу
        string filePath = "../../../sword/sword.obj";

        // Каждая строка файла отдельный элемент в списке
        string[] lines = File.ReadAllLines(filePath);

        foreach (string line in lines)
        {
            Console.WriteLine(line);
        }

        Console.ReadLine();
    }
}

class Program
{
    static void Main()
    {
        ObjParser.getAllText();
    }
}

