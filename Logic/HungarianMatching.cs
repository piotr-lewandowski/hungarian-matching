using System.Diagnostics;

namespace Logic;

public record CrossOutResult(
    int[] IndependentInRow,
    int[] IndependentInColumn,
    Graph Graph,
    bool[] CoverColumns,
    bool[] CoverRows,
    int[] MarkedInRow
)
{
    public int CoveredCount => CoverColumns.Sum(b => b ? 1 : 0) + CoverRows.Sum(b => b ? 1 : 0);

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
        var IndependentInColumn = new int[n];

        var markedInRow = new int[n];

        for (var i = 0; i < n; ++i)
        {
            coverColumns[i] = false;
            coverRows[i] = false;
            IndependentInColumn[i] = -1;
            independentInRow[i] = -1;
            markedInRow[i] = -1;
        }

        var coveredCount = 0;
        for (var i = 0; i < n; ++i)
        {
            for (var j = 0; j < n; ++j)
            {
                if (!coverColumns[j] && !coverRows[i] && graph.Edges[i, j] == 0)
                {
                    coverColumns[j] = true;

                    independentInRow[i] = j;
                    IndependentInColumn[j] = i;

                    ++coveredCount;

                    break;
                }
            }
        }

        var result = new CrossOutResult(independentInRow, IndependentInColumn, graph, coverColumns, coverRows, markedInRow);
        return Iterate(result);
    }

    public CrossOutResult Iterate(CrossOutResult previous)
    {
        var n = previous.Graph.VertexCount;
        var coveredCount = previous.CoveredCount;

        if (coveredCount == n)
        {
            return previous;
        }

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
            previous.MarkedInRow[i] = -1;
            previous.CoverRows[i] = false;
            previous.CoverColumns[i] = previous.IndependentInColumn[i] != -1;
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

        return new(result, new(originalWeights));
    }

    public CrossOutResult IterateCover(CrossOutResult res)
    {
        var n = res.Graph.VertexCount;

        if (res.CoveredCount == n)
        {
            return res;
        }

        var min = int.MaxValue;
        for (var i = 0; i < n; ++i)
        {
            for (var j = 0; j < n; ++j)
            {
                if (res.Graph.Edges[i, j] < min)
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