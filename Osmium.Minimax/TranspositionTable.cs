namespace Osmium.Minimax;

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