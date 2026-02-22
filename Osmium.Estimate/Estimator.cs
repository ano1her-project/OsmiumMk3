using Osmium.Core;

namespace Osmium.Estimate
{
    public class Estimator
    {
        public static int GetEstimate(Position position)
        {
            return GetMaterialBalance(position);
        }

        static int[] materialValue = [1, 3, 3, 5, 9, 0];

        public static int GetMaterialBalance(Position position)
        {
            int result = 0;
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    var piece = position.GetPiece(rank, file);
                    if (piece is not null)
                        result += materialValue[(int)piece?.type] * ((bool)piece?.isWhite ? 1 : -1);
                }
            }
            return result;
        }
    }
}
