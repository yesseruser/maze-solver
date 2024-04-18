using Newtonsoft.Json;
using Maze;

Input? GetInput()
{
    Console.WriteLine("Enter JSON file path:");
    string? path = Console.ReadLine();

    if (!Path.Exists(path))
    {
        Console.WriteLine("The path specified does not exist.");
        return null;
    }

    Input? input;
    try
    {
        var serializer = new JsonSerializer();
        using var reader = new StreamReader(path);
        using var jsonReader = new JsonTextReader(reader);
        input = serializer.Deserialize<Input>(jsonReader);

        if (input is null)
        {
            Console.WriteLine("The input is null after deserialization.");
            return null;
        }
    }
    catch (JsonSerializationException e)
    {
        Console.WriteLine("An exception occured when deserializing:");
        Console.WriteLine(e);
        throw;
    }

    return input;
}

string[][] FindShortestPath(string[][] maze)
{
    
}

var input = GetInput();

if (input is null)
    return;

Point? start = null;
Point? end = null;

Console.WriteLine(input.Maze);

for (var i = 0; i < input.Maze.Length; i++)
{
    for (var j = 0; j < input.Maze[i].Length; j++)
    {
        Console.Write(input.Maze[i][j]);
        
        if (input.Maze[i][j] == "S")
            start = new(i, j);
        if (input.Maze[i][j] == "C")
            end = new(i, j);
    }

    Console.WriteLine();
}

if (start is null || end is null)
{
    Console.WriteLine("The specified maze is invalid (no start or end)");
    return;
}

Console.WriteLine($"Start: {start}");
Console.WriteLine($"Start: {end}");

input.Maze = FindShortestPath(input.Maze);