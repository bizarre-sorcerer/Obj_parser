using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;


// Класс парсер
// В метод Parse(filePath) дается путь к .obj файлу. Он его парсит и извлекает линии в отдельные списки в зависимости от токена в начале линии(v/vt/vn/f)
// Списки не возвращаются, а храняться статически как свойства класса
class ObjParser
{
    // Координаты всех вершин треугольников
    public static List<string> vertexes = new List<string>();

    // Координаты текстур
    public static List<string> textures = new List<string>();

    // Нормали
    public static List<string> normals = new List<string>();

    // Лица треугольников
    public static List<string> faces = new List<string>();

    // Возвращает список где каждая линия файла это элемент списка
    private static string[] getAllText(string filePath)
    {
        // Каждая строка файла отдельный элемент в массиве
        string[] linesArray = File.ReadAllLines(filePath);

        return linesArray;
    }

    // Сортирует линии по отдельным спискам. Вершины в одном списке, нормали в одной списке и т.д
    private static void sortLines(string[] lines)
    {
        // Проверяeт каждую линию и добавляет ее в соответстующий список в зависимости от токена(v/vt и т.д)
        foreach (string line in lines)
        {
            // Вершина 
            if (line[0] == 'v' && line[1] == ' ')
            {
                vertexes.Add(line);
            }
            // Текстура
            else if (line[0] == 'v' && line[1] == 't')
            {
                textures.Add(line);
            }
            // Нормаль
            else if (line[0] == 'v' && line[1] == 'n')
            {
                normals.Add(line);
            }
            // Лицо
            else if (line[0] == 'f')
            {
                faces.Add(line);
            }
        }
    }

    // Это будет финальной функций которая будет делать все сразу.
    // Парсит файл, вытаскивает все нужные данные и организовывает их формате, который понимают движки для 3д рендеринга
    public static void Parse(string filePath)
    {
        // Получаем список со всеми линиями текста
        string[] lines = getAllText(filePath);

        // Запихиваем из по спискам
        sortLines(lines);

        // TODO
        // Огранизует в тип данных который понятен для движков 3д рендеринга
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
        foreach (string item in ObjParser.normals)
        {
            Console.WriteLine(item);
        }
        Console.ReadLine();
    }
}

