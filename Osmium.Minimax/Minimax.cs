using Osmium.Core;
using Osmium.Heuristics;

namespace Osmium.Minimax;

public class Minimax
{
    public enum DebugPrintMode
    {
        ProgressBar,
        Elaborate
    }

    static readonly int checkmateEval = 50_000;
    static readonly int stalemateEval = 0;

    static TranspositionTable transpositionTable = new(1 << 20);

    public static Move FindBestMove(Position position, int depth, int evalSortDepth, int highestEvalWhiteCanForce, int lowestEvalBlackCanForce, DebugPrintMode debugPrintMode, out int bestEval) // very similar to Evaluate() but also returns the move
    {
        var moves = position.GetAllPseudoLegalMoves().ToArray();
        Console.WriteLine($"Found {moves.Length} move(s)..");
        // preliminary pass at lower depth to sort the moves by eval
        if (evalSortDepth < depth && evalSortDepth > 0)
        {
            Console.WriteLine($"Sorting moves by eval at depth {evalSortDepth}..");
            List<int> evals = [];
            foreach (var move in moves)
            {
                position.MakeMove(move, out var undoInfo);
                if (position.IsKingInCheck(!position.whiteToMove))
                {
                    position.UnmakeMove(move, undoInfo);
                    continue;
                }
                evals.Add(Evaluate(position, evalSortDepth - 1, highestEvalWhiteCanForce, lowestEvalBlackCanForce));
                position.UnmakeMove(move, undoInfo);
            }
            Array.Sort(evals.ToArray(), moves); // lowest to highest
            if (position.whiteToMove)
                Array.Reverse(moves); // highest to lowest
        }
        //
        Console.WriteLine($"Evaluating moves at depth {depth}..");
        bestEval = position.whiteToMove ? int.MinValue : int.MaxValue;
        Move bestMove = moves[0];
        if (debugPrintMode == DebugPrintMode.ProgressBar)
            Console.WriteLine(new string(' ', moves.Length) + moves.Length.ToString()); // print progress bar
        foreach (var move in moves)
        {
            position.MakeMove(move, out var undoInfo);
            if (position.IsKingInCheck(!position.whiteToMove))
            {
                position.UnmakeMove(move, undoInfo);
                continue;
            }
            int eval = Evaluate(position, depth - 1, highestEvalWhiteCanForce, lowestEvalBlackCanForce);
            position.UnmakeMove(move, undoInfo);
            bool isBetterThanPrevious = position.whiteToMove ? (eval > bestEval) : (eval < bestEval);
            //
            if (debugPrintMode == DebugPrintMode.ProgressBar)
                Console.Write("▒"); // print progress bar
            else// if (debugPrintMode == DebugPrintMode.Elaborate)
                Console.WriteLine($"{move}: {eval}");
            //
            if (!isBetterThanPrevious)
                continue;
            bestEval = eval;
            bestMove = move;
            if (position.whiteToMove)
            {
                if (bestEval >= lowestEvalBlackCanForce) // ..then black will not make the move that would lead to this node.
                    break;
                if (bestEval > highestEvalWhiteCanForce)
                    highestEvalWhiteCanForce = bestEval;
            }
            else // black to move
            {
                if (bestEval <= highestEvalWhiteCanForce) // ..then white will not make the move that would lead to this node.
                    break;
                if (bestEval < lowestEvalBlackCanForce)
                    lowestEvalBlackCanForce = bestEval;
            }
        }
        if (debugPrintMode == DebugPrintMode.ProgressBar)
            Console.WriteLine();
        return bestMove;
    }

    public static Move FindBestMove(Position position, int depth, int evalSortDepth, DebugPrintMode debugPrintMode, out int bestEval)
        => FindBestMove(position, depth, evalSortDepth, int.MinValue, int.MaxValue, debugPrintMode, out bestEval);

    public static int Evaluate(Position position, int depth, int highestEvalWhiteCanForce, int lowestEvalBlackCanForce)
    {
        // first, check the transposition table
        var index = transpositionTable.GetIndex(position.hash);
        var entry = transpositionTable.entries[index];
        if (entry.hash == position.hash && entry.depth >= depth)
        {
            if ((entry.nodeType == NodeType.Exact) ||
                (entry.nodeType == NodeType.LowerBound && entry.eval >= lowestEvalBlackCanForce) ||
                (entry.nodeType == NodeType.UpperBound && entry.eval < highestEvalWhiteCanForce))
                return entry.eval;
        }
        // check for checkmate or stalemate
        var moves = position.GetAllLegalMoves();
        if (moves.Count == 0)
        {
            int eval = position.IsKingInCheck(position.whiteToMove) ? (position.whiteToMove ? -checkmateEval : checkmateEval) : stalemateEval;
            transpositionTable.entries[index] = new(position.hash, depth, NodeType.Exact, eval);
            return eval;
        }
        // if reached the end of depth
        if (depth == 0)
        {
            int eval = Heuristics.Heuristics.Evaluate(position);
            transpositionTable.entries[index] = new(position.hash, depth, NodeType.Exact, eval);
            return eval;
        }
        // otherwise, just recurse deeper
        int bestEval = position.whiteToMove ? int.MinValue : int.MaxValue;
        foreach (var move in moves)
        {
            position.MakeMove(move, out var undoInfo);
            int eval = Evaluate(position, depth - 1, highestEvalWhiteCanForce, lowestEvalBlackCanForce);
            position.UnmakeMove(move, undoInfo);
            bool isBetterThanPrevious = position.whiteToMove ? (eval > bestEval) : (eval < bestEval);
            if (!isBetterThanPrevious)
                continue;
            bestEval = eval;
            if (position.whiteToMove)
            {
                if (bestEval >= lowestEvalBlackCanForce) // ..then black will not make the move that would lead to this node.
                {
                    // add to the transposition table
                    if (entry.hash != position.hash || entry.depth < depth)
                        transpositionTable.entries[index] = new(position.hash, depth, NodeType.LowerBound, bestEval);
                    // return
                    return bestEval;
                }
                if (bestEval > highestEvalWhiteCanForce)
                    highestEvalWhiteCanForce = bestEval;
            }
            else // black to move
            {
                if (bestEval <= highestEvalWhiteCanForce) // ..then white will not make the move that would lead to this node.
                {
                    // add to the transposition table
                    if (entry.hash != position.hash || entry.depth < depth)
                        transpositionTable.entries[index] = new(position.hash, depth, NodeType.UpperBound, bestEval);
                    // return
                    return bestEval;
                }
                if (bestEval < lowestEvalBlackCanForce)
                    lowestEvalBlackCanForce = bestEval;
            }
        }
        // add to the transposition table
        if (entry.hash != position.hash || entry.depth < depth)
            transpositionTable.entries[index] = new(position.hash, depth, NodeType.Exact, bestEval);
        // return
        return bestEval;
    }
}

public enum NodeType
{
    Exact,
    LowerBound,
    UpperBound
}

public readonly struct TranspositionTableEntry
{
    public readonly long hash;
    //readonly Position position;
    public readonly int depth;
    public readonly NodeType nodeType;
    public readonly int eval;

    public TranspositionTableEntry(long p_hash, int p_depth, /*Position p_position,*/ NodeType p_nodeType, int p_eval)
    {
        hash = p_hash;
        //position = p_position.DeepCopy();
        depth = p_depth;
        nodeType = p_nodeType;
        eval = p_eval;
    }
}

public class TranspositionTable
{
    public TranspositionTableEntry[] entries;
    readonly int size;
    readonly int sizeMask;

    public TranspositionTable(int p_size)
    {
        size = p_size;
        sizeMask = size - 1; // all one bits
        entries = new TranspositionTableEntry[size];
    }

    public long GetIndex(long hash)
        => hash & (long)sizeMask;
}

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
