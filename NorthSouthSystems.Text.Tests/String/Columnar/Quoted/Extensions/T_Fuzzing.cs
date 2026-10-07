using MoreLinq;
using System.Text;

public class T_StringQuotedExtensions_FuzzingSingleRow
{
    [Fact]
    public void SingleFieldSingleRow() =>
        T_StringQuotedFixture.Signals.ForEach(signals =>
        {
            // A row with a single empty field results in an empty collection as desired. That special case is addressed
            // in the EmptyFields Fact.
            foreach (var pair in T_SplitQuotedRawParsedFieldPair.Fuzzing(signals)
                         .Where(p => !string.IsNullOrEmpty(p.Raw)))
            {
                ActAndAssert(signals, pair.Raw, pair.Parsed);

                foreach (string delimiter in signals.Delimiters)
                {
                    foreach (string newRow in signals.NewRows.DefaultIfEmpty(string.Empty))
                    {
                        ActAndAssert(signals, pair.Raw + delimiter, pair.Parsed, string.Empty);
                        ActAndAssert(signals, delimiter + pair.Raw, string.Empty, pair.Parsed);
                        ActAndAssert(signals, pair.Raw + newRow, pair.Parsed);
                    }
                }
            }
        });

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    public void MultiFieldSingleRow(int fieldCount) =>
        T_StringQuotedFixture.Signals.AsParallel().ForAll(signals =>
        {
            foreach (var permutation in T_SplitQuotedRawParsedFieldPair.Fuzzing(signals)
                         .Subsets(fieldCount)
                         .SelectMany(subset => subset.Permutations()))
            {
                var rowBuilder = new StringBuilder();

                permutation.ForEach((pair, index) =>
                {
                    if (index > 0)
                        rowBuilder.Append(T_StringQuotedFixture.Random(signals.Delimiters));

                    rowBuilder.Append(pair.Raw);
                });

                string row = rowBuilder.ToString();
                string[] expectedFields = permutation.Select(pair => pair.Parsed).ToArray();

                ActAndAssert(signals, row, expectedFields);

                foreach (string newRow in signals.NewRows)
                    ActAndAssert(signals, row + newRow, expectedFields);
            }
        });

    private static void ActAndAssert(StringQuotedSignals signals, string row, params string[] expectedFields)
    {
        row.SplitQuotedRow(signals)
            .Should().Equal(expectedFields);

        row.SplitQuotedRows(signals).Single()
            .Should().Equal(expectedFields);

        bool expectEmpty = expectedFields.Length == 1 && string.IsNullOrEmpty(expectedFields[0]);

        row.SplitQuotedRow(signals).JoinQuotedRow(signals).SplitQuotedRow(signals)
            .Should().Equal(expectEmpty ? [] : expectedFields);
    }
}

public class T_StringQuotedExtensions_FuzzingSingleFieldMultiRows
{
    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    public void SingleFieldMultiRows(int rowCount) =>
        T_StringQuotedFixture.Signals.Where(signals => signals.NewRowIsSpecified).AsParallel().ForAll(signals =>
        {
            // We ignore string.Empty because of the Fuzzing difficulties caused by \r, \n, and \r\n
            // each representing a single NewRow in the case of IsNewRowTolerant. A string.Empty row
            // followed by a random NewRow can inadvertently create a single NewRow when two were expected.
            foreach (var permutation in T_SplitQuotedRawParsedFieldPair.Fuzzing(signals)
                         .Where(pair => pair.Raw != string.Empty)
                         .Subsets(rowCount)
                         .SelectMany(subset => subset.Permutations()))
            {
                var rowsBuilder = new StringBuilder();

                permutation.ForEach((pair, index) =>
                {
                    if (index > 0)
                        rowsBuilder.Append(T_StringQuotedFixture.Random(signals.NewRows));

                    rowsBuilder.Append(pair.Raw);
                });

                string rows = rowsBuilder.ToString();
                string[][] expectedRowsOfSingleField = permutation.Select(pair => new[] { pair.Parsed }).ToArray();

                ActAndAssert(signals, rows, expectedRowsOfSingleField);

                foreach (string newRow in signals.NewRows)
                    ActAndAssert(signals, rows + newRow, expectedRowsOfSingleField);
            }
        });

    private static void ActAndAssert(
        StringQuotedSignals signals,
        string rows,
        IEnumerable<string[]> expectedRowsOfFields) =>
        T_StringQuotedExtensions_FuzzingMultiFieldMultiRows.ActAndAssert(signals, rows, expectedRowsOfFields);
}

public class T_StringQuotedExtensions_FuzzingMultiFieldMultiRows
{
    [Theory]
    [InlineData(3, 1.00)]
    [InlineData(4, 0.05)]
    public void MultiFieldMultiRows(int totalFieldCount, double samplingPercentage) =>
        T_StringQuotedFixture.Signals.Where(signals => signals.NewRowIsSpecified).AsParallel().ForAll(signals =>
        {
            var random = new Random(839);

            // See FuzzingSingleFieldRows comment about ignoring string.Empty.
            foreach (var permutation in T_SplitQuotedRawParsedFieldPair.Fuzzing(signals)
                         .Where(pair => pair.Raw != string.Empty)
                         .Subsets(totalFieldCount)
                         .SelectMany(subset => subset.Permutations())
                         .Where(_ => random.NextDouble() < samplingPercentage))
            {
                var rowsBuilder = new StringBuilder();
                var expectedRowsOfFields = new List<string[]>();
                var expectedFields = new List<string>();

                permutation.ForEach(pair =>
                {
                    if (expectedFields.Count > 0)
                    {
                        // Coin-flip
                        if (random.Next(2) == 0)
                        {
                            rowsBuilder.Append(T_StringQuotedFixture.Random(signals.NewRows));
                            expectedRowsOfFields.Add(expectedFields.ToArray());
                            expectedFields.Clear();
                        }
                        else
                            rowsBuilder.Append(T_StringQuotedFixture.Random(signals.Delimiters));
                    }

                    rowsBuilder.Append(pair.Raw);
                    expectedFields.Add(pair.Parsed);
                });

                expectedRowsOfFields.Add(expectedFields.ToArray());

                string rows = rowsBuilder.ToString();

                ActAndAssert(signals, rows, expectedRowsOfFields);

                foreach (string newRow in signals.NewRows)
                    ActAndAssert(signals, rows + newRow, expectedRowsOfFields);
            }
        });

    internal static void ActAndAssert(
        StringQuotedSignals signals,
        string rows,
        IEnumerable<string[]> expectedRowsOfFields)
    {
        rows.SplitQuotedRows(signals)
            .EquiZip(expectedRowsOfFields, (fields, expectedFields) => fields.Should().Equal(expectedFields))
            .Consume();

        bool skipLastRow =
            expectedRowsOfFields.Last().Length == 1 && string.IsNullOrEmpty(expectedRowsOfFields.Last()[0]);

        rows.SplitQuotedRows(signals)
            .JoinQuotedRows(signals)
            .SplitQuotedRows(signals)
            .EquiZip(
                expectedRowsOfFields.SkipLast(skipLastRow ? 1 : 0),
                (fields, expectedFields) => fields.Should().Equal(expectedFields))
            .Consume();
    }
}
