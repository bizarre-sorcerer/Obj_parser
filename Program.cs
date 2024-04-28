﻿using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using System.Globalization;


// Класс парсер
// В метод Parse(filePath) дается путь к .obj файлу. Он его парсит и извлекает линии в отдельные списки в зависимости от токена в начале линии(v/vt/vn/f)
// Списки не возвращаются, а храняться статически как свойства класса
class ObjParser
{
    // Координаты всех вершин треугольников
    public static List<List<float>> vertexes = new List<List<float>>();

    // Координаты текстур
    public static List<List<float>> textures = new List<List<float>>();

    // Нормали
    public static List<List<float>> normals = new List<List<float>>();

    // Лица треугольников
    public static List<string> faces = new List<string>();

    // Возвращает список где каждая линия файла это элемент списка
    private static string[] GetAllText(string filePath)
    {
        // Каждая строка файла отдельный элемент в массиве
        string[] linesArray = File.ReadAllLines(filePath);

        return linesArray;
    }

    // Возвращает новую строку без токена в начале линии. То есть остается только значение
    private static string RemoveToken(string line)
    {
        // Массив всех 'слов' линии разделенный пробелом 
        string[] words = line.Split();

        // Удаляем токен(f, vn etc), то есть первый элемент массива
        string tokenlessValues = string.Join(" ", words.Skip(1));

        return tokenlessValues;
    }

    // Делает из линии список, где значения это координаты вершины по x, y, z
    private static List<float> modifyLine(string line)
    {
        // Массив всех слов линии, разделенные пробелом 
        string[] words = line.Split();

        // Удаляем токен(f, vn etc), то есть первый элемент массива
        string[] tokenlessValues = words.Skip(1).ToArray();

        // Список координат вершин где будут 3 элемента: x, y, z
        List<float> result = new List<float>();

        foreach (string value in tokenlessValues)
        {
            // Добавляет в список каждую из координат в типе данных float
            result.Add(float.Parse(value.Trim(), CultureInfo.InvariantCulture));
        }

        return result;
    }

    // Сортирует линии по отдельным спискам. Вершины в одном списке, нормали в одной списке и т.д
    private static void SortLines(string[] lines)
    {
        // Проверяeт каждую линию и добавляет ее в соответстующий список в зависимости от токена(v/vt и т.д)
        foreach (string line in lines)
        {
            // Вершина 
            if (line[0] == 'v' && line[1] == ' ')
            {
                vertexes.Add(modifyLine(RemoveToken(line)));
            }
            // Текстура
            else if (line[0] == 'v' && line[1] == 't')
            {
                vertexes.Add(modifyLine(RemoveToken(line)));
            }
            // Нормаль
            else if (line[0] == 'v' && line[1] == 'n')
            {
                vertexes.Add(modifyLine(RemoveToken(line)));
            }
            // Лицо
            else if (line[0] == 'f')
            {
                faces.Add(RemoveToken(line));
            }
        }
    }

    // Это будет финальной функций которая будет делать все сразу.
    // Парсит файл, вытаскивает все нужные данные и организовывает их формате, который понимают движки для 3д рендеринга
    public static void Parse(string filePath)
    {
        // Получаем список со всеми линиями текста
        string[] lines = GetAllText(filePath);

        // Запихиваем из по спискам
        SortLines(lines);
    }
}

class Program
{
    static void Main()
    {
        // Путь к файлу
        string filePath = "../../../sword/sword.obj";

        // Парсит
        ObjParser.Parse(filePath);

        // Просто выводим в консоль чтобы убедить что работает как задуманно
        foreach (List<float> line in ObjParser.vertexes)
        {
            Console.WriteLine(line);
            
        }
        Console.ReadLine();
    }
}

