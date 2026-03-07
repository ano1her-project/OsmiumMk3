namespace Osmium.Core;

public class Zobrist
{
    static public long[,,] table = new long[8, 8, 12];
    static public long whiteToMove;

    public static void Initialize(int seed)
    {
        Random r = new(seed);
        for (int rank = 0; rank < 8; rank++)
        {
            for (int file = 0; file < 8; file++)
            {
                for (int piece = 0; piece < 12; piece++)
                    table[rank, file, piece] = r.NextInt64();
            }
        }
        whiteToMove = r.NextInt64();
    }

    public static long GetHashFor(Position position)
    {
        long hash = 0;
        for (int rank = 0; rank < 8; rank++)
        {
            for (int file = 0; file < 8; file++)
            {
                var piece = position.GetPiece(rank, file);
                if (piece is null)
                    continue;
                hash ^= table[rank, file, GetPieceIndex((Piece)piece)];
            }
        }
        if (position.whiteToMove)
            hash ^= whiteToMove;
        return hash;
    }

    public static int GetPieceIndex(Piece piece)
        => (int)piece.type + (piece.isWhite ? 6 : 0);
}
