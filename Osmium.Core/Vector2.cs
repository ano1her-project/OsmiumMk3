namespace Osmium.Core;

public readonly struct Vector2
{
    public readonly int file, rank; // which file = x, which rank = y

    public Vector2(int p_file, int p_rank)
    {
        file = p_file;
        rank = p_rank;
    }

    public static readonly Vector2 up = new(0, 1);
    public static readonly Vector2 right = new(1, 0);
    public static readonly Vector2 down = new(0, -1);
    public static readonly Vector2 left = new(-1, 0);
    public static readonly Vector2 one = new(1, 1);

    public static readonly Vector2[] orthogonalDirections = [up, right, down, left];
    public static readonly Vector2[] diagonalDirections = [one, right + down, -one, left + up];
    public static readonly Vector2[] allDirections = [up, one, right, right + down, down, -one, left, left + up];
    public static readonly Vector2[] hippogonalDirections = [new(1, 2), new(2, 1), new(2, -1), new(1, -2), new(-1, -2), new(-2, -1), new(-2, 1), new(-1, 2)];

    public override bool Equals(object? obj)
        => obj is Vector2 v && this == v;

    public static bool operator ==(Vector2 a, Vector2 b)
        => a.file == b.file && a.rank == b.rank;

    public static bool operator !=(Vector2 a, Vector2 b)
        => !(a == b);

    public override int GetHashCode()
        => HashCode.Combine(rank, file);

    public static Vector2 operator -(Vector2 v)
        => new(-v.file, -v.rank);

    public static Vector2 operator +(Vector2 a, Vector2 b)
        => new(a.file + b.file, a.rank + b.rank);

    public static Vector2 operator -(Vector2 a, Vector2 b)
        => new(a.file - b.file, a.rank - b.rank);

    public static Vector2 FromString(string str) // assuming a string in the format of, for instance, e4
        => new(str[0] - 'a', str[1] - '0' - 1);

    public override string ToString()
        => (char)('a' + file) + (rank + 1).ToString();

    public bool IsInBounds()
        => rank >= 0 && rank < 8 && file >= 0 && file < 8;
}
