namespace Logic.Tests;

using System.Text;

public class PropertyTest
{
    public record GraphWrapper(uint size, uint maxWeight)
    {
        public Graph Graph { get; } = FullGraph(size, maxWeight);

        public override string ToString()
        {
            var sb = new StringBuilder();

            var n = Graph.Edges.GetLength(0);
            sb.AppendLine(n.ToString());
            for(var i=0; i<n; ++i)
            {
                for(var j=0; j<n; ++j)
                {
                    sb.Append($"{Graph.Edges[i,j]} ");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }

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

    [Property]
    public Property ExactResult_isNotWorseThan_RandomResult(GraphWrapper wrapper)
    {
        var graph = wrapper.Graph;
        var randomMatching = new RandomMatching(graph);
        var hungarianMatching = new HungarianMatching().Match(graph);

        return (randomMatching.Weight >= hungarianMatching.Weight).ToProperty();
    }
}