namespace Logic.Tests;

using System.Text;

internal class Utils
{
    public static string PrintGraph(Graph graph)
    {
        var sb = new StringBuilder();

        var n = graph.Edges.GetLength(0);
        sb.AppendLine(n.ToString());
        for (var i = 0; i < n; ++i)
        {
            for (var j = 0; j < n; ++j)
            {
                sb.Append($"{graph.Edges[i, j]} ");
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

}
