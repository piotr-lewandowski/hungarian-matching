using System.Diagnostics;

namespace Logic;


public record Edge(int Start, int End, int Weight)
{
    public IEnumerable<int> Vertices { get; } = new[] { Start, End };

    public string SimpleToString()
    {
        return $"{Start} {End} {Weight}";
    }
}


public record Graph(int[,] Edges)
{
    public int VertexCount { get; } = Edges.GetLength(0);

    public static Graph FromNeighbourLists(List<List<int?>> edges)
    {
        var sum = edges.SelectMany(x => x).Sum() ?? 0;
        var missingWeight = sum + 1;

        var n = edges.Count;
        if (n == 0)
        {
            return new Graph(new int[0, 0]);
        }
        var m = edges[0].Count;
        if (edges.Any(list => list.Count != m) || n != m)
        {
            throw new ArgumentException("Input matrix was not a square matrix.");
        }

        var fullEdges = new int[n, m];
        for (var i = 0; i < n; ++i)
        {
            for (int j = 0; j < m; ++j)
            {
                fullEdges[i, j] = edges[i][j] ?? missingWeight;
            }
        }

        return new Graph(fullEdges);
    }
}