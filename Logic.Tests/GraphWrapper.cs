namespace Logic.Tests;
public record GraphWrapper(uint size, uint maxWeight)
{
    public Graph Graph { get; } = FullGraph(size, maxWeight);

    public override string ToString() => Utils.PrintGraph(Graph);
    public static Graph FullGraph(uint size, uint maxWeight)
    {
        var random = new System.Random();
        var neighbours = new List<List<int?>>();

        for (var i = 0; i < size; ++i)
        {
            neighbours.Add(new());
            for (var j = 0; j < size; ++j)
            {
                var weight = random.Next(1, (int)maxWeight + 1);

                neighbours[i].Add(weight);
            }
        }

        var graph = Graph.FromNeighbourLists(neighbours);

        return graph;
    }

}
