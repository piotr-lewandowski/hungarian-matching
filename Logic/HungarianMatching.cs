using System.Diagnostics;

namespace Logic;

/// <summary>
/// Intermediate results of our computation.
/// </summary>
/// <param name="IndependentInRow">Position of an independent zero in a given row, -1 if no such element exists.</param>
/// <param name="IndependentInColumn">Position of an independent zero in a given column, -1 if no such element exists.</param>
/// <param name="Graph">Graph that we're computing the matching for.</param>
/// <param name="CoverColumns">Whether given column is a part of current zeros cover.</param>
/// <param name="CoverRows">Whether given row is a part of current zeros cover.</param>
/// <param name="MarkedInRow">Where is the marked element in a given row, -1 if no such element exists.</param>
public record CrossOutResult(
    int[] IndependentInRow,
    int[] IndependentInColumn,
    Graph Graph,
    bool[] CoverColumns,
    bool[] CoverRows,
    int[] MarkedInRow
)
{
    public int CountCovered()
    {
        var sum = 0;
        for(int i=0; i<Graph.VertexCount; ++i)
        {
            if(IndependentInColumn[i] != -1)
            {
                sum++;
            }
        }
        return sum;
    }

}

public class HungarianMatching
{
    public CrossOutResult CrossOut(Graph graph)
    {
        var n = graph.VertexCount;

        // whether a given line is in our cover
        var coverColumns = new bool[n];
        var coverRows = new bool[n];

        // -1 - means none chosen in that row
        var independentInRow = new int[n];
        var independentInColumn = new int[n];

        var markedInRow = new int[n];

        for (var i = 0; i < n; ++i)
        {
            coverColumns[i] = false;
            coverRows[i] = false;
            independentInColumn[i] = -1;
            independentInRow[i] = -1;
            markedInRow[i] = -1;
        }

        for (var i = 0; i < n; ++i)
        {
            for (var j = 0; j < n; ++j)
            {
                if (independentInRow[i] == -1 && independentInColumn[j] == -1 && graph.Edges[i, j] == 0)
                {
                    independentInRow[i] = j;
                    independentInColumn[j] = i;

                    break;
                }
            }
        }
        
        var coveredCount = 0;
        for (var i = 0; i < n; ++i)
        {
            if(independentInColumn[i] != -1)
            {
                coverColumns[i] = true;
                ++coveredCount;
            }
        }

        var result = new CrossOutResult(independentInRow, independentInColumn, graph, coverColumns, coverRows, markedInRow);
        return Iterate(result);
    }

    public CrossOutResult Iterate(CrossOutResult previous)
    {
        var n = previous.Graph.VertexCount;

        var marked = previous.MarkedInRow;
        (int Row, int Column)? found = null;
        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                if (!previous.CoverColumns[j] && !previous.CoverRows[i] && previous.Graph.Edges[i, j] == 0)
                {
                    marked[i] = j;
                    found = (i, j);
                    break;
                }
            }
            if (found != null) break;
        }
        
        if (found == null)
        {
            return previous;
        }

        var column = previous.IndependentInRow[found!.Value.Row];
        if (column != -1)
        {
            previous.CoverColumns[column] = false;
            previous.CoverRows[found.Value.Row] = true;
            return Iterate(previous);
        }

        var toAdd = new List<(int Row, int Column)>();
        var toRemove = new List<(int Row, int Column)>();
        toAdd.Add(found.Value);
        var row = previous.IndependentInColumn[found.Value.Column];
        while (row != -1)
        {
            column = found.Value.Column;
            toRemove.Add((row, column));

            var markedColumn = previous.MarkedInRow[row];
            var markedRow = row;
            Debug.Assert(markedColumn != -1);
            toAdd.Add((markedRow, markedColumn));

            found = (markedRow, markedColumn);
            row = previous.IndependentInColumn[found.Value.Column];
        }

        foreach (var elem in toRemove)
        {
            ProcessElement(elem.Row, elem.Column);
        }
        foreach (var elem in toAdd)
        {
            ProcessElement(elem.Row, elem.Column);
        }
        for (int i = 0; i < n; ++i)
        {
            // remove all marks
            previous.MarkedInRow[i] = -1;
            // remove all lines and add columns containing independent zeros
            previous.CoverRows[i] = false;
            previous.CoverColumns[i] = (previous.IndependentInColumn[i] != -1);
        }

        return Iterate(previous);

        void ProcessElement(int row, int column)
        {
            if (previous.IndependentInRow[row] == column && previous.IndependentInColumn[column] == row)
            {
                previous.IndependentInColumn[column] = -1;
                previous.IndependentInRow[row] = -1;
            }
            else
            {
                Debug.Assert(previous.IndependentInColumn[column] == -1);
                Debug.Assert(previous.IndependentInRow[row] == -1);
                previous.IndependentInColumn[column] = row;
                previous.IndependentInRow[row] = column;
            }
        }
    }

    public Matching Match(Graph graph)
    {
        var n = graph.VertexCount;
        var originalWeights = new int[n, n];
        for (var i = 0; i < n; ++i)
        {
            for (var j = 0; j < n; ++j)
            {
                originalWeights[i, j] = graph.Edges[i, j];
            }
        }

        for (int i = 0; i < n; ++i)
        {
            var smallest = int.MaxValue;
            for (int j = 0; j < n; ++j)
            {
                if (graph.Edges[i, j] < smallest)
                {
                    smallest = graph.Edges[i, j];
                }
            }

            for (int j = 0; j < n; ++j)
            {
                graph.Edges[i, j] -= smallest;
            }
        }

        for (int i = 0; i < n; ++i)
        {
            var smallest = int.MaxValue;
            for (int j = 0; j < n; ++j)
            {
                if (graph.Edges[j, i] < smallest)
                {
                    smallest = graph.Edges[j, i];
                }
            }

            for (int j = 0; j < n; ++j)
            {
                graph.Edges[j, i] -= smallest;
            }
        }

        var res = CrossOut(graph);

        res = IterateCover(res);

        var result = new List<Edge>();
        for (var i = 0; i < n; ++i)
        {
            var row = i;
            var column = res.IndependentInRow[row];
            var weight = originalWeights[row, column];
            result.Add(new(row, column, weight));
        }

        return new(result, new(originalWeights) { MissingWeight = graph.MissingWeight } );
    }

    public CrossOutResult IterateCover(CrossOutResult res)
    {
        var n = res.Graph.VertexCount;

        if (res.CountCovered() == n)
        {
            return res;
        }

        var min = int.MaxValue;
        for (var i = 0; i < n; ++i)
        {
            for (var j = 0; j < n; ++j)
            {
                if (res.Graph.Edges[i, j] < min && !res.CoverRows[i] && !res.CoverColumns[j])
                {
                    min = res.Graph.Edges[i, j];
                }
            }
        }
        for (var i = 0; i < n; ++i)
        {
            for (var j = 0; j < n; ++j)
            {
                if (!res.CoverRows[i] && !res.CoverColumns[j])
                {
                    res.Graph.Edges[i, j] -= min;
                }
                if (res.CoverRows[i] && res.CoverColumns[j])
                {
                    res.Graph.Edges[i, j] += min;
                }

            }
        }

        var newRes = Iterate(res);
        return IterateCover(newRes);
    }
}