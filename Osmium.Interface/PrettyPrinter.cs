using Osmium.Core;

namespace Osmium.Interface;

public class PrettyPrinter
{
    static readonly Dictionary<Piece, char> unicodePieces = new()
    {
        { new(Piece.Type.Pawn, false), '♙'},
        { new(Piece.Type.Bishop, false), '♗'},
        { new(Piece.Type.Knight, false), '♘' },
        { new(Piece.Type.Rook, false), '♖' },
        { new(Piece.Type.Queen, false), '♕' },
        { new(Piece.Type.King, false), '♔' },
        { new(Piece.Type.Pawn, true), '♟'},
        { new(Piece.Type.Bishop, true), '♝'},
        { new(Piece.Type.Knight, true), '♞' },
        { new(Piece.Type.Rook, true), '♜' },
        { new(Piece.Type.Queen, true), '♛' },
        { new(Piece.Type.King, true), '♚' }
    };

    public enum PieceOptions
    {
        Ascii,
        Unicode,
        UnicodeInverted
    }

    public enum BackgroundOptions
    {
        Simple,
        Shaded,
        ShadedInverted
    }

    public static void Print(Position position, PieceOptions pieceOptions, BackgroundOptions backgroundOptions, Vector2? from, Vector2[] tos)
    {
        string output = "";
        for (int rank = 7; rank >= 0; rank--)
        {
            output += (rank + 1).ToString() + " ";
            for (int file = 0; file < 8; file++)
            {
                if (position.GetPiece(rank, file) is null)
                    output += tos.Contains(new(file, rank)) ? "()" : backgroundOptions switch
                    {
                        BackgroundOptions.Simple => ". ",
                        BackgroundOptions.Shaded => GetSquareShadeString(rank, file, false),
                        BackgroundOptions.ShadedInverted => GetSquareShadeString(rank, file, true),
                        _ => throw new Exception()
                    };
                else
                    output += pieceOptions switch
                    {
                        PieceOptions.Ascii => position.GetPiece(rank, file)?.ToString(),
                        PieceOptions.Unicode => unicodePieces[(Piece)position.GetPiece(rank, file)],
                        PieceOptions.UnicodeInverted => unicodePieces[(Piece)position.GetPiece(rank, file)?.GetInverted()],
                        _ => throw new Exception()
                    } + ((from is not null && from == new Vector2(file, rank)) ? ")" : " ");
            }
            output += "\n";
        }
        output += "  a b c d e f g h ";
        Console.WriteLine(output);
    }

    public static void Print(Position position)
        => Print(position, PieceOptions.Ascii, BackgroundOptions.ShadedInverted, null, []);

    public static void Print(Position position, PieceOptions pieceOptions, BackgroundOptions backgroundOptions)
        => Print(position, pieceOptions, backgroundOptions, null, []);

    public static void Print(Position position, Vector2? from, Vector2[] tos)
        => Print(position, PieceOptions.Ascii, BackgroundOptions.ShadedInverted, from, tos);

    static bool IsSquareWhite(int rank, int file)
        => (rank + file) % 2 != 0;

    static string GetSquareShadeString(int rank, int file, bool invert)
        => (IsSquareWhite(rank, file) ^ invert) ? "░░" : "▒▒";
}
