namespace Interface;

using System.Text;
using Logic;

class WeightWriter : IGraphWriter
{
    public string Write(Graph graph)
    {
        var sb = new StringBuilder();

        sb.AppendLine(graph.Edges.GetLength(0).ToString());
        foreach(var row in graph.Edges)
        {
            sb.AppendJoin(' ', row);
            sb.AppendLine();
        }

        return sb.ToString();
    }
}