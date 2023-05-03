using Interface;
using Logic;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Parsing;
using System.CommandLine;

var fileArgument = new Argument<FileInfo>(
    name: "filepath",
    description: "File containing the input graph."
);

fileArgument.AddValidator(result =>
{
    var fileInfo = result.GetValueForArgument<FileInfo>(fileArgument);
    if (!fileInfo.Exists)
    {
        result.ErrorMessage = $"File {fileInfo.FullName} does not exist.";
    }
});

var rootCommand = new RootCommand(
    "Graph colouring of arbitrary graphs using First Fit, Largest First, Smallest Last or Maximum Cardinality Search."
) { fileArgument };

rootCommand.SetHandler((file) =>
{
    var reader = new GraphReader();
    var writer = new MatchingWriter();
    var error = Console.Error;

    try
    {
        var graph = reader.ReadSingle(file.FullName);

        var matching = new RandomMatching(graph);

        var text = writer.Write(matching);
        Console.Write(text);
    }
    catch (System.Exception)
    {
        error.WriteLine("Input graph was not in a correct format.");
    }

}, fileArgument);

await rootCommand.InvokeAsync(args);
