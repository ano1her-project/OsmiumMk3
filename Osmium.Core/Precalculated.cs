using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmium.Core;

public class Precalculated
{
    public readonly static Vector2[,][] knightTargets = PrecalculateLeaperTargets(Vector2.hippogonalDirections);
    public readonly static Vector2[,][] kingTargets = PrecalculateLeaperTargets(Vector2.allDirections);

    static Vector2[,][] PrecalculateLeaperTargets(Vector2[] directions)
    {
        var result = new Vector2[8, 8][];
        for (int rank = 0; rank < 8; rank++)
        {
            for (int file = 0; file < 8; file++)
            {
                List<Vector2> targets = [];
                foreach (var direction in directions)
                {
                    Vector2 leaper = new(file, rank);
                    if ((leaper + direction).IsInBounds())
                        targets.Add(leaper + direction);
                }
                result[rank, file] = [..targets];
            }
        }
        return result;
    }
}
