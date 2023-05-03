namespace Logic;

public record Matching(List<Edge> Edges, Graph Graph)
{
    public int Weight => Edges.Sum(e => e.Weight);

    public bool IsPerfect()
    {
        var n = Graph.VertexCount;
        var seenLeft = new bool[n];
        var seenRight = new bool[n];

        foreach(var e in  Edges)
        {
            seenLeft[e.Start] = true;
            seenRight[e.End] = true;
        }

        var all = true;

        for(var i=0; i<n; ++i)
        {
            all &= seenLeft[i] && seenRight[i];
        }

        return all;
    }
}