using Osmium.Core;

namespace Osmium.Engine
{
    public class Minimax
    {
        static readonly int checkmateEval = 50_000;
        static readonly int stalemateEval = 0;

        public static Move FindBestMove(Position position, int depth, int bestEvalWhiteCanGuarantee, int bestEvalBlackCanGuarantee, out int bestEval) // very similar to Evaluate() but also returns the move
        {
            var moves = position.GetAllLegalMoves();
            Console.WriteLine($"Found {moves.Count} move(s)..");
            bestEval = position.whiteToMove ? int.MinValue : int.MaxValue;
            Move bestMove = moves[0];
            Console.WriteLine(new string(' ', moves.Count) + moves.Count.ToString()); // print progress bar
            foreach (var move in moves)
            {
                position.MakeMove(move, out var undoInfo);
                int eval = Evaluate(position, depth - 1, bestEvalWhiteCanGuarantee, bestEvalBlackCanGuarantee);
                position.UnmakeMove(move, undoInfo);
                bool isBetterThanPrevious = position.whiteToMove ? (eval > bestEval) : (eval < bestEval);
                Console.Write("▒"); // print progress bar
                if (!isBetterThanPrevious)
                    continue;
                bestEval = eval;
                bestMove = move;
                if (position.whiteToMove)
                {
                    if (bestEval >= bestEvalBlackCanGuarantee) // ..then black will not let white get here.
                        break;
                    if (bestEval > bestEvalWhiteCanGuarantee)
                        bestEvalWhiteCanGuarantee = bestEval;
                }
                else // black to move
                {
                    if (bestEval <= bestEvalWhiteCanGuarantee) // ..then white will not let black get here.
                        break;
                    if (bestEval < bestEvalBlackCanGuarantee)
                        bestEvalBlackCanGuarantee = bestEval;
                }
            }
            Console.WriteLine();
            return bestMove;
        }

        public static Move FindBestMove(Position position, int depth, out int bestEval)
            => FindBestMove(position, depth, int.MinValue, int.MaxValue, out bestEval);

        public static int Evaluate(Position position, int depth, int bestEvalWhiteCanGuarantee, int bestEvalBlackCanGuarantee)
        {
            // first check for checkmate or stalemate
            var moves = position.GetAllLegalMoves();
            if (moves.Count == 0)
                return position.IsKingInCheck(position.whiteToMove) ? (position.whiteToMove ? -checkmateEval : checkmateEval) : stalemateEval;
            // if reached the end of depth
            if (depth == 0)
                return Estimator.GetEstimate(position);
            // otehrwise, just recurse deeper
            int bestEval = position.whiteToMove ? int.MinValue : int.MaxValue;
            foreach (var move in moves)
            {
                position.MakeMove(move, out var undoInfo);
                int eval = Evaluate(position, depth - 1, bestEvalWhiteCanGuarantee, bestEvalBlackCanGuarantee);
                position.UnmakeMove(move, undoInfo);
                bool isBetterThanPrevious = position.whiteToMove ? (eval > bestEval) : (eval < bestEval);
                if (!isBetterThanPrevious)
                    continue;
                bestEval = eval;
                if (position.whiteToMove)
                {
                    if (bestEval >= bestEvalBlackCanGuarantee) // ..then black will not let white get here.
                        break;
                    if (bestEval > bestEvalWhiteCanGuarantee)
                        bestEvalWhiteCanGuarantee = bestEval;
                }
                else
                {
                    if (bestEval <= bestEvalWhiteCanGuarantee) // ..then white will not let black get here.
                        break;
                    if (bestEval < bestEvalBlackCanGuarantee)
                        bestEvalBlackCanGuarantee = bestEval;
                }
            }
            return bestEval;
        }

        //

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

        public static int CountLeafNodesAtDepthByMove(Position position, int depth)
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
}