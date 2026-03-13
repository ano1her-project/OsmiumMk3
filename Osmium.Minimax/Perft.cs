using Osmium.Core;

namespace Osmium.Minimax;

public class Perft
{
    public static int CountLeafNodesAtDepth(Position position, int depth) // perft
    {
        if (depth == 0)
            return 1;
        var moves = position.GetAllLegalMoves();
        if (depth == 1)
            return moves.Count;
        int leafCount = 0;
        foreach (var move in moves)
        {
            position.MakeMove(move, out var undoInfo);
            leafCount += CountLeafNodesAtDepth(position, depth - 1);
            position.UnmakeMove(move, undoInfo);
        }
        return leafCount;
    }

    public static int CountLeafNodesAtDepthByMove(Position position, int depth) // perft divide
    {
        var moves = position.GetAllLegalMoves();
        int totalLeafCount = 0;
        for (int i = 0; i < moves.Count; i++)
        {
            position.MakeMove(moves[i], out var undoInfo);
            int leafCount = CountLeafNodesAtDepth(position, depth - 1);
            totalLeafCount += leafCount;
            position.UnmakeMove(moves[i], undoInfo);
            Console.WriteLine($"({i + 1}/{moves.Count}) move {moves[i]}: {leafCount}");
        }
        return totalLeafCount;
    }
}