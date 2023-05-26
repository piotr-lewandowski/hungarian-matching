using System.Text;
using Logic;
using System.Diagnostics;

var size = 0;
var maxWeight = 0;
var failed = false;

if (args.Length > 0)
{
    var sizeStr = args[0];
    var parsed = int.TryParse(sizeStr, out size);
    failed = failed || !parsed;
}
if(args.Length == 1 && args[0] == "time")
{
    GenerateTimeData();
}
if (args.Length == 1 && !failed)
{
    maxWeight = 2 * size;
}
if (args.Length == 2 && !failed)
{
    var maxWeightStr = args[1];
    var parsed = int.TryParse(maxWeightStr, out maxWeight);
    failed = failed || !parsed;
}
if (args.Length == 0 || args.Length > 2 || failed)
{
    Console.WriteLine("Usage: generator <size> [<max-weight>]");
    return;
}

var random = new Random();

var neighbours = new List<List<string>>();

for (var i = 0; i < size; ++i)
{
    neighbours.Add(new());
    for (var j = 0; j < size; ++j)
    {
        var isMissing = random.Next() % 37 == 0;
        var weight = random.Next(1, maxWeight + 1);

        neighbours[i].Add(isMissing ? "x" : weight.ToString());
    }
}

var output = $"{size}\n" + string.Join('\n', neighbours.Select(v => string.Join(' ', v)));

Console.WriteLine(output);

var outputFile = new FileInfo($"generated_input_{size}_{maxWeight}.txt");
using var textWriter = outputFile.CreateText();
textWriter.Write(output);

void GenerateTimeData()
{
    var matching = new HungarianMatching();
    var header = "vertices,time";
    var sb = new StringBuilder();
    var random = new Random();
    sb.AppendLine(header);

    for (uint i = 5; i <= 500; i += 5)
    {
        Console.WriteLine($"############ i = {i} ############");
        for (int j = 0; j < 200; ++j)
        {
            var weightCoeff = random.Next(1, 5);
            var graph = FullGraph(i, (uint)weightCoeff * i);

            var stopwatch = Stopwatch.StartNew();
            long nanosecPerTick = (1000L * 1000L * 1000L) / Stopwatch.Frequency;

            var failed = false;
            try
            {
                matching.Match(graph);
            }
            catch (Exception)
            {
                failed = true;
            }
            finally { stopwatch.Stop(); }
            if (!failed)
            {
                var ticks = stopwatch.ElapsedTicks;
                var time = ticks * nanosecPerTick;

                Console.WriteLine($"{j} - {time} nanoseconds");
                sb.AppendLine($"{i},{time}");
            }
            else
            {
                Console.WriteLine("failed");
            }
        }
    }

    Graph FullGraph(uint size, uint maxWeight)
    {
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

    var output = sb.ToString();
    var outputFile = new FileInfo($"times.csv");
    using var textWriter = outputFile.CreateText();
    textWriter.Write(output);
}
