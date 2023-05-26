namespace Logic.Tests;
public record GraphWithKnownSolution(uint size)
{
    public Matching Matching { get; } = GenerateTestData(size + 1, 2 * size);

    public static Matching GenerateTestData(uint size, uint weight)
    {
        var random = new System.Random();
        var indices = new int[size];
        for (int i = 0; i < size; ++i)
        {
            indices[i] = i;
        }
        for (int i = 0; i < size; ++i)
        {
            var j = random.NextInt64(i, size);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }

        var edges = new int[size, size];

        var sum = 0;
        for (var i = 0; i < size; i++)
        {
            var newWeight = random.Next(0, (int)weight);
            edges[i, indices[i]] = newWeight;
            sum += newWeight;
        }

        for (var i = 0; i < size; ++i)
        {
            for (var j = 0; j < size; ++j)
            {
                if (j == indices[i]) continue;
                var newWeight = random.Next(sum + 1, sum + (int)weight);
                edges[i, j] = newWeight;
            }
        }

        var match = new List<Edge>();

        for (var i = 0; i < size; ++i)
        {
            match.Add(new Edge(i, indices[i], edges[i, indices[i]]));
        }

        return new Matching(match, new Graph(edges));
    }
    public override string ToString() => Utils.PrintGraph(Matching.Graph);
}