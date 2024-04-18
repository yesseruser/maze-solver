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

string[][]? FindShortestPath(string[][] maze, Point start, Point end)
{
    if (start == end)
        return maze;
    
    var numberedCells = new int[maze.Length, maze[0].Length];
    numberedCells[end.X, end.Y] = 0;
    
    var didFindStart = false;
    
    List<Point> lastPoints = [new Point(end.X, end.Y)];
    List<Point> usedPoints = [new Point(end.X, end.Y)];
    List<Point> newLastPoints = [];

    while (!didFindStart)
    {
        if (lastPoints.Count == 0) // The maze has no solution.
            return null;
        
        foreach (var point in lastPoints)
        {
            if (point == start)
                didFindStart = true;
            
            if (maze.Length > point.X + 1
                && maze[point.X + 1][point.Y] != "#" 
                && !usedPoints.Contains(new Point(point.X + 1, point.Y)))
            {
                numberedCells[point.X + 1, point.Y] = numberedCells[point.X, point.Y] + 1;
                newLastPoints.Add(new Point(point.X + 1, point.Y));
            }

            if (point.X - 1 >= 0
                && maze[point.X - 1][point.Y] != "#"
                && !usedPoints.Contains(new Point(point.X - 1, point.Y)))
            {
                numberedCells[point.X - 1, point.Y] = numberedCells[point.X, point.Y] + 1;
                newLastPoints.Add(new Point(point.X - 1, point.Y));
            }

            if (maze[0].Length > point.Y + 1
                && maze[point.X][point.Y + 1] != "#"
                && !usedPoints.Contains(new Point(point.X, point.Y + 1)))
            {
                numberedCells[point.X, point.Y + 1] = numberedCells[point.X, point.Y] + 1;
                newLastPoints.Add(new Point(point.X, point.Y + 1));
            }

            if (point.Y - 1 >= 0
                && maze[point.X][point.Y - 1] != "#"
                && !usedPoints.Contains(new Point(point.X, point.Y - 1)))
            {
                numberedCells[point.X, point.Y - 1] = numberedCells[point.X, point.Y] + 1;
                newLastPoints.Add(new Point(point.X, point.Y - 1));
            }
        }

        lastPoints = newLastPoints;
        usedPoints.AddRange(newLastPoints);
        newLastPoints = [];
    }

    string[][] solvedMaze = maze;
    var lastPoint = start;

    for (int i = 0; i < maze.Length; i++)
    {
        for (int j = 0; j < maze[0].Length; j++)
        {
            if (new Point(i, j) != end && numberedCells[i, j] == 0)
                numberedCells[i, j] = int.MaxValue;
        }
    }
    
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
        
        solvedMaze[lastPoint.X][lastPoint.Y] = "X";
    }

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

var solvedMaze = FindShortestPath(input.Maze, start.Value, end.Value);

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