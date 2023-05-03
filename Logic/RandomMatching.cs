namespace Logic;

public record RandomMatching(Graph Graph) : Matching(MakeRandomMatching(Graph), Graph)
{
    private static Random Random { get; } = new Random();

    private static List<Edge> MakeRandomMatching(Graph graph)
    {
        // Generate a random permutation using Fisher-Yates algorithm
        var indices = new int[graph.VertexCount];
        for (int i = 0; i < graph.VertexCount; ++i)
        {
            indices[i] = i;
        }
        for (int i = 0; i < graph.VertexCount; ++i)
        {
            var j = Random.NextInt64(i, graph.VertexCount);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }

        return indices.Select((elem, index) =>
        {
            var weight = graph.Edges[elem, index];
            return new Edge(elem, index, weight);
        }).ToList();
    }
}