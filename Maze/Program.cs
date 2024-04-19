using Newtonsoft.Json;
using Maze;

int[,] Fill2DArray(int[,] array)
{
    for (var i = 0; i < array.GetLength(0); i++)
    {
        for (var j = 0; j < array.GetLength(1); j++)
        {
            array[i, j] = int.MaxValue;
        }
    }

    return array;
}

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

void AddFilledCell(string[][] maze, int x, int y, Point currentPoint, int[,] numberedPoints, ICollection<Point> usedPoints,
    ICollection<Point> newLastPoints)
{
    if (x >= maze.Length || x <= 0 
        || y >= maze[0].Length || y <= 0
        || maze[x][y] == "#"
        || usedPoints.Contains(new Point(x, y))) return;
    
    numberedPoints[x, y] = numberedPoints[currentPoint.X, currentPoint.Y] + 1;
    newLastPoints.Add(new Point(x, y));
}

int[,] FloodFill(string[][] maze, Point end1, Point start1)
{
    var filledFields = new int[maze.Length, maze[0].Length];
    filledFields = Fill2DArray(filledFields);
    filledFields[end1.X, end1.Y] = 0;
    
    var didFindStart = false;
    
    List<Point> lastPoints = [new Point(end1.X, end1.Y)];
    List<Point> usedPoints = [new Point(end1.X, end1.Y)];
    List<Point> newLastPoints = [];

    while (!didFindStart)
    {
        if (lastPoints.Count == 0) // The maze has no solution.
            return filledFields;
        
        foreach (var point in lastPoints)
        {
            if (point == start1)
                didFindStart = true;
            
            AddFilledCell(maze, point.X + 1, point.Y, point, filledFields, usedPoints, newLastPoints);
            AddFilledCell(maze, point.X - 1, point.Y, point, filledFields, usedPoints, newLastPoints);
            AddFilledCell(maze, point.X, point.Y + 1, point, filledFields, usedPoints, newLastPoints);
            AddFilledCell(maze, point.X, point.Y - 1, point, filledFields, usedPoints, newLastPoints);
        }

        lastPoints = newLastPoints;
        usedPoints.AddRange(newLastPoints);
        newLastPoints = [];
    }

    return filledFields;
}

string[][] GenerateSolvedPath(string[][] maze, int[,] numberedCells, Point start, Point end)
{
    string[][] solvedMaze1 = maze;
    var lastPoint = start;
    
    while (true)
    {
        var lowestNum = numberedCells[start.X, start.Y];
        var lowestPoint = lastPoint;

        if (maze.Length > lastPoint.X + 1
            && numberedCells[lastPoint.X + 1, lastPoint.Y] != int.MaxValue
            && lowestNum > numberedCells[lastPoint.X + 1, lastPoint.Y])
        {
            lowestNum = numberedCells[lastPoint.X + 1, lastPoint.Y];
            lowestPoint = new Point(lastPoint.X + 1, lastPoint.Y);
        }
        if (lastPoint.X - 1 >= 0
            && numberedCells[lastPoint.X - 1, lastPoint.Y] != int.MaxValue
            && lowestNum > numberedCells[lastPoint.X - 1, lastPoint.Y])
        {
            lowestNum = numberedCells[lastPoint.X - 1, lastPoint.Y];
            lowestPoint = new Point(lastPoint.X - 1, lastPoint.Y);
        }
        if (maze[0].Length > lastPoint.Y + 1
            && numberedCells[lastPoint.X, lastPoint.Y + 1] != int.MaxValue
            && lowestNum > numberedCells[lastPoint.X, lastPoint.Y + 1])
        {
            lowestNum = numberedCells[lastPoint.X, lastPoint.Y + 1];
            lowestPoint = new Point(lastPoint.X, lastPoint.Y + 1);
        }
        if (lastPoint.Y >= 0
            && numberedCells[lastPoint.X, lastPoint.Y - 1] != int.MaxValue
            && lowestNum > numberedCells[lastPoint.X, lastPoint.Y - 1])
        {
            lowestNum = numberedCells[lastPoint.X, lastPoint.Y - 1];
            lowestPoint = new Point(lastPoint.X, lastPoint.Y - 1);
        }

        lastPoint = lowestPoint;
        
        if (lowestPoint == end)
            break;
        
        solvedMaze1[lastPoint.X][lastPoint.Y] = "X";
    }

    return solvedMaze1;
}

string[][]? SolveMaze(string[][] maze, Point start, Point end)
{
    if (start == end)
        return maze;
    
    var numberedCells = FloodFill(maze, end, start);
    var solvedMaze = GenerateSolvedPath(maze, numberedCells, start, end);

    return solvedMaze;
}

var input = GetInput();

if (input is null)
    return;

Point? start = null;
Point? end = null;

for (var i = 0; i < input.Maze.Length; i++)
{
    for (var j = 0; j < input.Maze[i].Length; j++)
    {
        Console.Write(input.Maze[i][j]);
        
        if (input.Maze[i][j] == "S")
            start = new Point(i, j);
        if (input.Maze[i][j] == "C")
            end = new Point(i, j);
    }

    Console.WriteLine();
}

if (start is null || end is null)
{
    Console.WriteLine("The specified maze is invalid (no start or end)");
    return;
}

var solvedMaze = SolveMaze(input.Maze, start.Value, end.Value);

if (solvedMaze is null)
{
    Console.WriteLine("Maze does not have a solution.");
    return;
}

Console.WriteLine("\nSolved Maze:");
for (int i = 0; i < solvedMaze.Length; i++)
{
    for (int j = 0; j < solvedMaze[i].Length; j++)
    {
        Console.Write(solvedMaze[i][j]);
    }

    Console.WriteLine();
}