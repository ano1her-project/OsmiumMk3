namespace Osmium.Core;

public readonly struct Piece
{
    public readonly Type type;
    public readonly bool isWhite;

    public enum Type
    {
        Pawn,
        Bishop,
        Knight,
        Rook,
        Queen,
        King
    }

    public Piece(Type p_type, bool p_isWhite)
    {
        type = p_type;
        isWhite = p_isWhite;
    }

    public static Piece FromChar(char ch)
    {
        bool isWhite = char.IsUpper(ch);
        return char.ToLower(ch) switch
        {
            'p' => new(Type.Pawn, isWhite),
            'b' => new(Type.Bishop, isWhite),
            'n' => new(Type.Knight, isWhite),
            'r' => new(Type.Rook, isWhite),
            'q' => new(Type.Queen, isWhite),
            'k' => new(Type.King, isWhite),
            _ => throw new Exception()
        };
    }

    public override bool Equals(object? obj)
        => obj is Piece piece && this == piece;

    public static bool operator ==(Piece a, Piece b)
        => a.type == b.type && a.isWhite == b.isWhite;

    public static bool operator !=(Piece a, Piece b)
        => !(a == b);

    public override int GetHashCode()
        => HashCode.Combine(type, isWhite);

    public char ToChar()
    {
        char ch = type switch
        {
            Type.Pawn => 'p',
            Type.Bishop => 'b',
            Type.Knight => 'n',
            Type.Rook => 'r',
            Type.Queen => 'q',
            Type.King => 'k',
            _ => throw new Exception()
        };
        return isWhite ? char.ToUpper(ch) : ch;
    }

    public override string ToString()
        => ToChar().ToString();

    public Piece GetInverted() // only used in the PrettyPrinter when inverting colors
        => new(type, !isWhite);
}
