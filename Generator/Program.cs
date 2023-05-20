using Logic;

var size = 0;
var maxWeight = 0;
var failed = false;

if (args.Length > 0)
{
    var sizeStr = args[0];
    var parsed = int.TryParse(sizeStr, out size);
    failed = failed || !parsed;
}
if (args.Length == 1)
{
    maxWeight = 2 * size;
}
if (args.Length == 2)
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