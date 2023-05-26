namespace Logic;

public record Matching(List<Edge> Edges, Graph Graph)
{
    public int Weight => Edges.Sum(e => e.Weight);

    public bool IsPerfect()
    {
        return Edges.All(e => e.Weight != Graph.MissingWeight);       
    }

    public bool IsEqualTo(Matching m)
    {
        if (m.Edges.Count != this.Edges.Count) return false;

        var ourEdges = Edges.OrderBy(e => e.Start).ToArray();
        var theirEdges = m.Edges.OrderBy(e => e.Start).ToArray();

        var weightEqual = m.Weight == this.Weight;
        var edgesEqual = true;
        for (var i = 0; i < Edges.Count; ++i)
        {
            edgesEqual = edgesEqual && (ourEdges[i] == theirEdges[i]);
        }

        return edgesEqual && weightEqual;
    }
}