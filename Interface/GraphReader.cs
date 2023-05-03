namespace Interface;

using Logic;

class GraphReader : IGraphReader
{
    public Graph ReadSingle(string path)
    {
        var lines = File.ReadAllLines(path);
        var count = int.Parse(lines[0]);

        var edges = new List<List<int?>>();

        for(int i=0; i<count; ++i)
        {
            var row = lines[i+1]
                .Split(' ')
                .Select<string, int?>(s => s switch {
                    "x" => null,
                    _ => int.Parse(s)
                }
            );
            edges.Add(new(row));
        }

        return Graph.FromNeighbourLists(edges);
    }
}