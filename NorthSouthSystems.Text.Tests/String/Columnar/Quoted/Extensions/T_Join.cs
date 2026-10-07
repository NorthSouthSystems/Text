using MoreLinq;

public class T_StringQuotedExtensions_Join
{
    [Fact]
    public void Quoting() =>
        T_StringQuotedFixture.Signals.Where(signals => signals.QuoteIsSpecified).ForEach(signals =>
        {
            JoinAndAssert(["a", "b", "c"], signals, false, "a{d}b{d}c");
            JoinAndAssert([null, "b", "c"], signals, false, "{d}b{d}c");
            JoinAndAssert(["a", "b", string.Empty], signals, false, "a{d}b{d}");
            JoinAndAssert(["aa", "bb", "cc"], signals, false, "aa{d}bb{d}cc");

            foreach (string delimiter in signals.Delimiters)
                JoinAndAssert(["a" + delimiter, "b", "c"], signals, false, "{q}a{do}{q}{d}b{d}c", delimiter);

            if (signals.NewRowIsSpecified)
            {
                foreach (string newRow in signals.NewRows)
                {
                    JoinAndAssert(
                        ["a" + newRow + "a", "b", "c"],
                        signals,
                        false,
                        "{q}a{no}a{q}{d}b{d}c",
                        expectedNewRowOverride: newRow);
                }
            }

            JoinAndAssert(
                ["a" + signals.Quote, "b", "c"],
                signals,
                false,
                signals.EscapeIsSpecified
                    ? "{q}a{e}{q}{q}{d}b{d}c"
                    : "{q}a{q}{q}{q}{d}b{d}c");

            JoinAndAssert(["aa", "bb", "cc"], signals, true, "{q}aa{q}{d}{q}bb{q}{d}{q}cc{q}");
        });

    [Fact]
    public void Escaping() =>
        T_StringQuotedFixture.Signals.Where(signals => signals.EscapeIsSpecified && !signals.QuoteIsSpecified)
            .ForEach(signals =>
            {
                foreach (string delimiter in signals.Delimiters)
                    JoinAndAssert(["a" + delimiter, "b", "c"], signals, false, "a{e}{do}{d}b{d}c", delimiter);

                if (signals.NewRowIsSpecified)
                {
                    foreach (string newRow in signals.NewRows)
                    {
                        JoinAndAssert(
                            ["a" + newRow, "b", "c"],
                            signals,
                            false,
                            "a{e}{no}{d}b{d}c",
                            expectedNewRowOverride: newRow);
                    }
                }

                JoinAndAssert(["a" + signals.Escape, "b", "c"], signals, false, "a{e}{e}{d}b{d}c");
            });

    private static void JoinAndAssert(
        string[] actualFields,
        StringQuotedSignals signals,
        bool forceQuotes,
        string expectedFormat,
        string expectedDelimiterOverride = null,
        string expectedNewRowOverride = null)
    {
        actualFields.JoinQuotedRow(signals, forceQuotes)
            .Should().Be(Expected(expectedFormat));

        if (signals.NewRowIsSpecified)
        {
            new[] { actualFields }.JoinQuotedRows(signals, forceQuotes)
                .Should().Be(Expected(expectedFormat));

            new[] { actualFields, actualFields }.JoinQuotedRows(signals, forceQuotes)
                .Should().Be(Expected(expectedFormat + signals.NewRow + expectedFormat));

            new[] { actualFields, actualFields, actualFields }.JoinQuotedRows(signals, forceQuotes)
                .Should().Be(
                    Expected(expectedFormat + signals.NewRow + expectedFormat + signals.NewRow + expectedFormat));
        }

        string Expected(string format) =>
            T_StringQuotedFixture.Replace(format, signals)
                .First()
                .Replace("{do}", expectedDelimiterOverride ?? signals.Delimiter)
                .Replace("{no}", expectedNewRowOverride ?? signals.NewRow);
    }

    [Fact]
    public void ExceptionsRows()
    {
        Action act = null;

        act = () => ((string[][])null).JoinQuotedRows(StringQuotedSignals.CsvNewRowTolerantWindowsPrimaryRFC4180);
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => new[] { new[] { "A" } }.JoinQuotedRows(null);
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => new[] { new[] { "A" } }.JoinQuotedRows(new([","], null, "\"", null));
        act.Should().ThrowExactly<ArgumentException>("NewRowNotSpecified");

        act = () => new[] { new[] { "A" } }.JoinQuotedRows(new([","], ["\n"], null, null), true);
        act.Should().ThrowExactly<ArgumentException>("QuoteNotSpecified");
    }

    [Fact]
    public void ExceptionsRow()
    {
        Action act = null;

        act = () => ((string[])null).JoinQuotedRow(StringQuotedSignals.CsvNewRowTolerantWindowsPrimaryRFC4180);
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => new[] { "A" }.JoinQuotedRow(null);
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => new[] { "A" }.JoinQuotedRow(new([","], null, null, null), true);
        act.Should().ThrowExactly<ArgumentException>("QuoteNotSpecified");

        act = () => new[] { "A," }.JoinQuotedRow(new([","], null, null, null));
        act.Should().ThrowExactly<ArgumentException>("QuoteNotSpecified");

        act = () => new[] { "A" + Environment.NewLine }.JoinQuotedRow(new([","], [Environment.NewLine], null, null));
        act.Should().ThrowExactly<ArgumentException>("QuoteNotSpecified");
    }
}
