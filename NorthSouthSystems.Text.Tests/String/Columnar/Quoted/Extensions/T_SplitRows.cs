using MoreLinq;

public class T_StringQuotedExtensions_SplitRows
{
    [Fact]
    public void EmptyRows() => T_StringQuotedFixture.Signals.ForEach(signals =>
    {
        SplitAndAssert(string.Empty, 0);

        foreach (string newRow in signals.NewRows)
        {
            SplitAndAssert(newRow, 1);
            SplitAndAssert(newRow + newRow, 2);
            SplitAndAssert(newRow + newRow + newRow, 3);

            if (signals.QuoteIsSpecified)
            {
                string quotedEmpty = signals.Quote + signals.Quote;

                SplitAndAssert(quotedEmpty, 1);
                SplitAndAssert(quotedEmpty + newRow, 1);

                SplitAndAssert(newRow + quotedEmpty, 2);
                SplitAndAssert(newRow + quotedEmpty + newRow, 2);

                SplitAndAssert(newRow + quotedEmpty + newRow + newRow, 3);
                SplitAndAssert(newRow + quotedEmpty + newRow + quotedEmpty, 3);
            }
        }

        void SplitAndAssert(string rows, int expectedRowCount)
        {
            var expectedRowsOfSingleField = Enumerable.Repeat(string.Empty, expectedRowCount);

            rows.SplitQuotedRows(signals).Select(fields => fields.Single())
                .Should().Equal(expectedRowsOfSingleField);
        }
    });

    [Fact]
    public void Exceptions()
    {
        Action act;

        act = () => ((string)null).SplitQuotedRows(StringQuotedSignals.CsvNewRowTolerantWindowsPrimaryRFC4180).ToArray();
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => string.Empty.SplitQuotedRows(null).ToArray();
        act.Should().ThrowExactly<ArgumentNullException>();
    }
}
