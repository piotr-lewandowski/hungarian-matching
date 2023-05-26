namespace Logic.Tests;

public partial class PropertyTest
{

    [Property]
    public Property ExactResult_isNotWorseThan_RandomResult(GraphWrapper wrapper)
    {
        var graph = wrapper.Graph;
        var randomMatching = new RandomMatching(graph);
        var hungarianMatching = new HungarianMatching().Match(graph);

        return (randomMatching.Weight >= hungarianMatching.Weight).ToProperty();
    }

    [Property]
    public Property When_ThereIsOnlyOneOptimalSolution_ItShouldBeUsed(GraphWithKnownSolution graphWithSolution)
    {
        var matching = graphWithSolution.Matching;
        var hungarianMatch = new HungarianMatching().Match(matching.Graph);

        return (hungarianMatch.IsEqualTo(matching)).ToProperty();
    }

    [Property]
    public Property When_ThereIsNoPerfectMatch_MissingEdgeShouldBeUsed(GraphWithNoSolution graph)
    {
        var hungarianMatch = new HungarianMatching().Match(graph.NoSolutionGraph);

        return (!hungarianMatch.IsPerfect()).ToProperty();
    }
}