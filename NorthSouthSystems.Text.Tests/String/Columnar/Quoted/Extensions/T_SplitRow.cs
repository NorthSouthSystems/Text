using MoreLinq;

public class T_StringQuotedExtensions_SplitRow
{
    [Fact]
    public void EmptyFields() => T_StringQuotedFixture.Signals.ForEach(signals =>
    {
        SplitAndAssert(string.Empty, 0);

        foreach (string delimiter in signals.Delimiters)
        {
            SplitAndAssert(delimiter, 2);
            SplitAndAssert(delimiter + delimiter, 3);

            if (signals.QuoteIsSpecified)
            {
                string quotedEmpty = signals.Quote + signals.Quote;

                SplitAndAssert(quotedEmpty + delimiter, 2);
                SplitAndAssert(delimiter + quotedEmpty, 2);
                SplitAndAssert(quotedEmpty + delimiter + quotedEmpty, 2);

                SplitAndAssert(quotedEmpty + delimiter + delimiter, 3);
                SplitAndAssert(delimiter + quotedEmpty + delimiter, 3);
                SplitAndAssert(delimiter + delimiter + quotedEmpty, 3);
                SplitAndAssert(quotedEmpty + delimiter + quotedEmpty + delimiter + quotedEmpty, 3);
            }
        }

        void SplitAndAssert(string row, int expectedFieldCount)
        {
            var expectedFields = Enumerable.Repeat(string.Empty, expectedFieldCount);

            row.SplitQuotedRow(signals).Should().Equal(expectedFields);

            if (expectedFieldCount > 0)
                row.SplitQuotedRows(signals).Single().Should().Equal(expectedFields);
        }
    });

    [Fact]
    public void ComplicatedDelimiter()
    {
        var signals = new StringQuotedSignals(["ab"], null, null, null);
        "1aab2aab3a".SplitQuotedRow(signals)
            .Should().Equal("1a", "2a", "3a");

        signals = new(["ababb"], null, null, null);
        "1abababb2abababb3ab".SplitQuotedRow(signals)
            .Should().Equal("1ab", "2ab", "3ab");
    }

    [Fact]
    public void Exceptions()
    {
        Action act;

        act = () => ((string)null).SplitQuotedRow(StringQuotedSignals.CsvNewRowTolerantWindowsPrimaryRFC4180);
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => string.Empty.SplitQuotedRow(null);
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => T_StringQuotedFixture.Replace(
                "a,b,c{n}d",
                StringQuotedSignals.CsvNewRowTolerantWindowsPrimaryRFC4180)
            .First()
            .SplitQuotedRow(StringQuotedSignals.CsvNewRowTolerantWindowsPrimaryRFC4180);
        act.Should().ThrowExactly<ArgumentException>("NewLineInArgument");

        act = () => T_StringQuotedFixture.Replace(
                "a,b,c{n}d,e,f",
                StringQuotedSignals.CsvNewRowTolerantWindowsPrimaryRFC4180)
            .First()
            .SplitQuotedRow(StringQuotedSignals.CsvNewRowTolerantWindowsPrimaryRFC4180);
        act.Should().ThrowExactly<ArgumentException>("NewLineInArgument");

        act = () => T_StringQuotedFixture.Replace(
                "a,b,c{n}d,e,f{n}g,h,i",
                StringQuotedSignals.CsvNewRowTolerantWindowsPrimaryRFC4180)
            .First()
            .SplitQuotedRow(StringQuotedSignals.CsvNewRowTolerantWindowsPrimaryRFC4180);
        act.Should().ThrowExactly<ArgumentException>("NewLineInArgument");
    }
}
