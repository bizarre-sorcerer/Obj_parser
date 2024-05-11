using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using System.Globalization;

class ObjectGroup
{
    // Координаты всех вершин треугольников. Каждый элемент это одна вершина и список, где хранятся значения координат по x, y, z
    public List<List<float>> vertexes;

    // Координаты текстур
    public List<List<float>> textures;

    // Нормали
    public List<List<float>> normals;

    //////////////
    // Лица треугольников
    // faces = [
    //      // Первое лицо
    //      [
    //            [553, 970],  // Первая точка. Элементы этого списка вершина и нормаль(v, vn)
    //            [],  // Вторая точка
    //            []   // Третья точка
    //      ]
    //////////////
    public List<List<List<int>>> faces = new List<List<List<int>>>();

    public ObjectGroup(List<List<float>> vertexes, List<List<float>> textures, List<List<float>> normals, List<List<List<int>>> faces)
    {
        this.vertexes = vertexes;
        this.textures = textures;
        this.normals = normals;
        this.faces = faces;
    }
}

// Класс парсер
// В метод Parse(filePath) дается путь к .obj файлу. Он его парсит и извлекает линии в отдельные списки в зависимости от токена в начале линии(v/vt/vn/f)
// Списки не возвращаются, а храняться статически как свойства класса
class ObjectParser
{
    // Список всех групп
    public static List<ObjectGroup> groups = new List<ObjectGroup>();
    

    
    /////////////////
    ////////// Данные текущей группы (g). Когда парситься вся группа, с этими данными создается объект класса ObjectGroup и списки очищаються для след группы ////////
    ////////////////
    // Координаты всех вершин треугольников. Каждый элемент это одна вершина и список, где хранятся значения координат по x, y, z
    public static List<List<float>> vertexes = new List<List<float>>();

    // Координаты текстур
    public static List<List<float>> textures = new List<List<float>>();

    // Нормали
    public static List<List<float>> normals = new List<List<float>>();

    // Лица треугольников
    // faces = [
    //      // Первое лицо
    //      [
    //            [553, 970],  // Первая точка. Элементы этого списка вершина и нормаль(v, vn)
    //            [],  // Вторая точка
    //            []   // Третья точка
    //      ]
    public static List<List<List<int>>> faces = new List<List<List<int>>>();



    /////////////
    /////////// Глобальные данные на весь файл. Тут все вертексы, нормали, текстуры и полигоны //////////
    /////////////
    public static List<List<float>> allVertexes = new List<List<float>>();

    // Координаты текстур
    public static List<List<float>> allTextures = new List<List<float>>();

    // Нормали
    public static List<List<float>> allNormals = new List<List<float>>();

    // Лица
    public static List<List<List<int>>> allFaces = new List<List<List<int>>>();



    // Делает из линии список, где значения это координаты вершины по x, y, z
    private static List<float> ModifyLine(string line)
    {
        // Массив всех 'слов' линии разделенный пробелом 
        string[] words = line.Split();

        // Удаляем токен(f, vn etc), то есть первый элемент массива
        string tokenlessValues = string.Join(" ", words.Skip(1));

        // Массив всех значений линии, разделенные пробелом 
        string[] values = tokenlessValues.Split();

        // Список координат вершин где будут 3 элемента: x, y, z
        List<float> result = new List<float>();

        foreach (string value in values)
        {
            // Добавляет в список каждую из координат в типе данных float
            result.Add(float.Parse(value.Trim(), CultureInfo.InvariantCulture));
        }

        return result;
    }

    // Парсит именно лицы, проебразует строку в вид более удобный для использования
    public static List<List<int>> ParseFace(string line)
    {
        // Массив всех 'слов' линии разделенный пробелом 
        string[] words = line.Split();

        // Удаляем токен(f, vn etc), то есть первый элемент массива. Строка без токена
        string tokenlessString = string.Join(" ", words.Skip(1));    // Пример: "1//1 246//1 332//1 117//1"

        // Список, где каждый элемент это одна точка. Каждая из точек тоже список, где элементы это v, vt и vn
        List<List<int>> currentFace = new List<List<int>>();

        // Виды строк: 
        // string face = "f 343435 343436 343434";
        // string face = "f 1006//725 560//725 567//725 1018//725";
        // string face = "f 23/2 32/34 35/34";
        // string face "f 1006/454/725 560/345/725 567/345/725 1018/345/725"
        
        foreach (string point in tokenlessString.Split(' '))
        {
            // Пропускает пустую сабстроку
            if (string.IsNullOrWhiteSpace(point))
            {
                continue;
            }

            int delimetersCount = point.Count(c => c == '/');

            // Если у поллигона только вертексы есть
            if (delimetersCount == 0)
            {
                currentFace.Add(new List<int> { int.Parse(point) });
            }
            // Если у поллигона есть все или все кроме нормали
            else if (delimetersCount == 1)
            {
                List<int> pointValues = new List<int>();

                foreach (string value in point.Split('/'))
                {
                    pointValues.Add(int.Parse(value));
                }
                currentFace.Add(pointValues);
            }
            // У полигона нет текстуры
            else if (delimetersCount == 2)
            {
                List<int> pointValues = new List<int>();

                foreach (string value in point.Split(new string[] { "//" }, StringSplitOptions.None))
                {
                    pointValues.Add(int.Parse(value));
                }
                currentFace.Add(pointValues);
            }

        }

        return currentFace;
    }

    // Сортирует линии по отдельным спискам. Вершины в одном списке, нормали в одной списке и т.д
    private static void SortLines(string[] lines)
    {
        int count = 0;
        int currentLine = 0;

        // Проверяeт каждую линию и добавляет ее в соответстующий список в зависимости от токена(v/vt и т.д)
        foreach (string line in lines)
        {
            currentLine += 1;
            
            // Вершина 
            if (line.StartsWith("v "))
            {
                vertexes.Add(ModifyLine(line));
                allVertexes.Add(ModifyLine(line));
            }
            // Текстура
            else if (line.StartsWith("vt"))
            {
                textures.Add(ModifyLine(line));
                allTextures.Add(ModifyLine(line));
            }
            // Нормаль
            else if (line.StartsWith("vn"))
            {
                normals.Add(ModifyLine(line));
                allNormals.Add(ModifyLine(line));
            }
            // Лицо
            else if (line.StartsWith("f "))
            {
                faces.Add(ParseFace(line));
                allFaces.Add(ParseFace(line));
            }
            else if (line.StartsWith("g "))
            {
                count += 1;

                ObjectGroup objGroup =  new ObjectGroup(vertexes, textures, normals, faces);
                groups.Add(objGroup);
                
                vertexes.Clear();
                textures.Clear();
                normals.Clear();
                faces.Clear();
            } 

            if (currentLine == lines.Count())
            {
                ObjectGroup objGroup = new ObjectGroup(vertexes, textures, normals, faces);
                groups.Add(objGroup);
            }
        }
    }

    // Это будет финальной функций которая будет делать все сразу.
    // Парсит файл, вытаскивает все нужные данные и организовывает их формате, который понимают движки для 3д рендеринга
    public static void Parse(string filePath)
    {
        // Получаем список со всеми линиями текста
        string[] lines = File.ReadAllLines(filePath);

        // Запихиваем из по спискам
        SortLines(lines);
    }
}

class Program
{
    static void Main()
    {
        // Путь к файлу
        string filePath = "../../../3D_models/robot/Rmk3.obj";
        //string filePath = "../../../3D_models/sword/sword.obj";

        // Парсит
        ObjectParser.Parse(filePath);

        Console.WriteLine(ObjectParser.groups[0].faces.Count());
        
        Console.ReadLine();
    }
}

