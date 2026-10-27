namespace Osmium.Core;

public class Zobrist
{
    static public long[,,] table = new long[8, 8, 12];
    static public long whiteToMove;
    static public long[] castlingRights = new long[4];
    static public long[,] enPassantSquares = new long[2, 8];

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
        for (int i = 0; i < 4; i++)
            castlingRights[i] = r.NextInt64();
        for (int side = 0; side < 2; side++)
        {
            for (int file = 0; file < 8; file++)
                enPassantSquares[side, file] = r.NextInt64();
        }
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
        for (int i = 0; i < 4; i++)
        {
            var flag = (Position.CastlingAvailability)(2 ^ i);
            if (position.castlingAvailability.HasFlag(flag))
                hash ^= castlingRights[i];
        }
        if (position.enPassantSquare is not null)
        {
            var enPassantSquare = position.enPassantSquare.Value;
            hash ^= enPassantSquares[enPassantSquare.rank == 2 ? 0 : 1, enPassantSquare.file];
        }
        return hash;
    }

    public static int GetPieceIndex(Piece piece)
        => (int)piece.type + (piece.isWhite ? 6 : 0);
}
