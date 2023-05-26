namespace Logic.Tests;
public record GraphWithNoSolution(uint size) : GraphWrapper(size + 1, 2 * size)
{
    public Graph NoSolutionGraph => Modify(Graph);

    private static Graph Modify(Graph graph)
    {
        graph.MissingWeight = int.MaxValue / 2;
        for(var i=0; i<graph.VertexCount; i++)
        {
            graph.Edges[0, i] = graph.MissingWeight;
        }

        return graph;
    }
}