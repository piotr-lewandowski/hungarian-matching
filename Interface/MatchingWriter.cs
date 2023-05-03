namespace Interface;

using System.Text;
using Logic;

class MatchingWriter : IMatchingWriter
{
    public string Write(Matching matching)
    {
        var sb = new StringBuilder();

        sb.AppendLine(matching.Weight.ToString());

        foreach(var e in matching.Edges)
        {
            sb.AppendLine(e.SimpleToString());
        }

        return sb.ToString();
    }

}