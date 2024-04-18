namespace Maze;

public struct Point(int x, int y)
{
    public int X = x;
    public int Y = y;

    public static bool operator ==(Point a, Point b)
        => a.X == b.X && a.Y == b.Y;

    public static bool operator !=(Point a, Point b)
        => !(a == b);

    public override string ToString()
    {
        return $"{X}, {Y}";
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Point pointObject) return false;
        return this == pointObject;
    }
    
    public bool Equals(Point other)
    {
        return X == other.X && Y == other.Y;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}