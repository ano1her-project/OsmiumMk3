namespace Osmium.Core;

public readonly struct Move
{
    public readonly Vector2 from, to;
    public readonly Flag flag;

    public Move(Vector2 p_from, Vector2 p_to, Flag p_flag)
    {
        from = p_from;
        to = p_to;
        flag = p_flag;
    }

    public Move(Vector2 p_from, Vector2 p_to) : this(p_from, p_to, Flag.None) { }

    public enum Flag
    {
        None,
        CastlingKingside,
        CastlingQueenside,
        TwoSquarePawnPush,
        EnPassant,
        PromotionToQueen,
        PromotionToRook,
        PromotionToKnight,
        PromotionToBishop
    }

    public override string ToString()
        => $"{from}{to}" + (flag != Flag.None ? "*" : "");
}
