using Interface;
using Logic;

if (args.Length != 1)
{
    Console.Error.WriteLine("Wrong number of arguments, please provide the path of the input file.");
    return;
}

var fileArg = args[0];
var file = new FileInfo(fileArg);

if(!file.Exists) 
{
    Console.Error.WriteLine("Input file doesn't exist.");
    return;
}

var reader = new GraphReader();
var writer = new MatchingWriter();

try
{
    var graph = reader.ReadSingle(file.FullName);

    var matching = new RandomMatching(graph);

    var text = writer.Write(matching);
    Console.Write(text);

    var outputFile = new FileInfo(file.Name + "-output");
    using var textWriter = outputFile.CreateText();
    textWriter.Write(text);
}
catch (UnauthorizedAccessException)
{
    Console.Error.WriteLine("Could not access input file.");
}
catch (FormatException)
{
    Console.Error.WriteLine("Input file was not in a correct format.");
}
catch (OverflowException)
{
    Console.Error.WriteLine("Input file was not in a correct format.");
}
catch (ArgumentException)
{
    Console.Error.WriteLine("Input file was not in a correct format.");
}