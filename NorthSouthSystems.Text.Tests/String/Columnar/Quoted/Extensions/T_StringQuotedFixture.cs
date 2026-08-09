internal static class T_StringQuotedFixture
{
    internal static IEnumerable<string> Replace(string format, StringQuotedSignals signals)
    {
        foreach (string delimiter in signals.Delimiters)
            foreach (string newRow in signals.NewRows.DefaultIfEmpty(string.Empty))
                yield return Replace(format, signals.Delimiter, signals.NewRow, signals.Quote, signals.Escape);
    }

    internal static string Replace(string format,
        string delimiter, string newRow, string quote, string escape) =>
        format.Replace("{d}", delimiter)
            .Replace("{n}", newRow)
            .Replace("{q}", quote)
            .Replace("{e}", escape);

    internal static string Random(IReadOnlyList<string> signals) =>
        signals.Count > 0 ? signals[_random.Next(signals.Count)] : string.Empty;

    private static readonly Random _random = new(22);

    internal static IReadOnlyList<StringQuotedSignals> Signals { get; } =
    [
        new([","], ["\r\n", "\n", "\r"], null, null),               // IsNewRowTolerantSimple (no quote, no escape)
        new([","], ["\r\n", "\n", "\r"], null, "\\"),               // IsNewRowTolerantSimple (no quote, escape)
        StringQuotedSignals.CsvNewRowTolerantWindowsPrimaryRFC4180, // IsNewRowTolerantSimple (quote, no escape)
        new([","], ["\r\n", "\n", "\r"], "\"", "\\"),               // IsNewRowTolerantSimple (quote, escape)

        new(["\t"], ["\n"], null, null), // IsSimple (no quote, no escape)
        new(["\t"], ["\n"], null, "\\"), // IsSimple (no quote, escape)
        new(["\t"], ["\n"], "'", null),  // IsSimple (quote, no escape)
        new(["\t"], ["\n"], "'", "\\"),  // IsSimple (quote, escape)

        new(["DELIMITER"], ["NEWLINE"], null, null),        // Full (no quote, no escape)
        new(["DELIMITER"], ["NEWLINE"], null, "ESCAPE"),    // Full (no quote, escape)
        new(["DELIMITER"], ["NEWLINE"], "QUOTE", null),     // Full (quote, no escape)
        new(["DELIMITER"], ["NEWLINE"], "QUOTE", "ESCAPE"), // Full (quote, escape)

        new([",", "\t", "DELIMITER"], ["\r\n", "\n", "\r"], null, null), // Full (no quote, no escape)
        new([",", "\t", "DELIMITER"], ["\r\n", "\n", "\r"], null, "\\"), // Full (no quote, escape)
        new([",", "\t", "DELIMITER"], ["\r\n", "\n", "\r"], "\"", null), // Full (quote, no escape)
        new([",", "\t", "DELIMITER"], ["\r\n", "\n", "\r"], "\"", "\\"), // Full (quote, escape)

        StringQuotedSignals.CsvNewRowLinux,  // IsSimple (quote, no escape)
        StringQuotedSignals.CsvNewRowWindows // Full (quote, no escape)
    ];
}

internal class T_SplitQuotedRawParsedFieldPair
{
    internal static IEnumerable<(string Raw, string Parsed)> Fuzzing(StringQuotedSignals signals)
    {
        // A standalone escape character in a field causes the SplitQuotedProcessor to ignore the following
        // delimiter and breaks all such Fuzzing tests as expected. Therefore, we ignore it for Fuzzing tests.
        //
        // An escaped Windows newRow cannot be used when signals.NewRows contains \n because the \r will become
        // escaped but the \n will still trigger the new row.
        foreach (var pair in _pairs
                     .Where(p => p.IsRelevant(signals))
                     .Where(p => p._rawFormat != "{e}"))
        {
            string delimiter = T_StringQuotedFixture.Random(signals.Delimiters);
            string newRow = T_StringQuotedFixture.Random(signals.NewRows);

            if (newRow == "\r\n" && pair._rawFormat.Contains("{e}{n}") && signals.NewRows.Contains("\n"))
                continue;

            yield return (
                T_StringQuotedFixture.Replace(pair._rawFormat, delimiter, newRow, signals.Quote, signals.Escape),
                T_StringQuotedFixture.Replace(pair._parsedFormat, delimiter, newRow, signals.Quote, signals.Escape));
        }
    }

    private static readonly IReadOnlyList<T_SplitQuotedRawParsedFieldPair> _pairs =
    [
        // Simple

        new("", ""),
        new("a", "a"),
        new("b", "b"),
        new("cd", "cd"),
        new(" a", " a"),
        new("a ", "a "),
        new(" a ", " a "),

        // Quoting Unneccessary

        new("{q}{q}", ""),
        new("{q}a{q}", "a"),
        new("{q}b{q}", "b"),
        new("{q}cd{q}", "cd"),
        new("{q} a{q}", " a"),
        new("{q}a {q}", "a "),
        new("{q} a {q}", " a "),

        // Quoting Neccessary

        new("{q}{d}{q}", "{d}"),
        new("{q}{n}{q}", "{n}"),
        new("{q}a{d}{q}", "a{d}"),
        new("{q}a{d}b{q}", "a{d}b"),
        new("{q}a{n}{q}", "a{n}"),
        new("{q}a{n}b{q}", "a{n}b"),

        // Quoting Quote Quote

        new("{q}{q}{q}{q}", "{q}"),
        new("{q}{q}a", "{q}a"),
        new("{q}{q}{q}a{q}", "{q}a"),
        new("{q}{q}{q}{q}a", "{q}{q}a"),
        new("{q}{q}{q}{q}{q}a{q}", "{q}{q}a"),
        new("{q}{q}{q}a{q}{q}{q}", "{q}a{q}"),
        new("{q}a{q}{q}a{q}{q}a{q}", "a{q}a{q}a"),

        // Quoting Complex

        new("{q}{q}{q}a{d}{q}", "{q}a{d}"),
        new("{q}{q}{q}{d}a{q}", "{q}{d}a"),
        new("{q}{q}{q}a{n}{q}", "{q}a{n}"),
        new("{q}{q}{q}{n}a{q}", "{q}{n}a"),
        new("{q}{q}{q}a{d}{n}{q}", "{q}a{d}{n}"),
        new("{q}{q}{q}{d}{n}a{q}", "{q}{d}{n}a"),
        new("{q}{q}{q}a{n}{d}{q}", "{q}a{n}{d}"),
        new("{q}{q}{q}{n}{d}a{q}", "{q}{n}{d}a"),

        // Escaping

        new("{e}", ""),
        new("{e} ", " "),
        new("{e}a", "a"),
        new("{e}{q}", "{q}"),
        new("{e}{d}", "{d}"),
        new("{e}{n}", "{n}"),
        new("{e}{e}", "{e}"),
        new("{e}{q}a", "{q}a"),
        new("{e}{d}a", "{d}a"),
        new("{e}{n}a", "{n}a"),
        new("{e}{e}a", "{e}a"),
        new("a{e}{d}", "a{d}"),
        new("a{e}{n}", "a{n}"),
        new("a{e}{e}", "a{e}"),
        new("a{e}{d}b", "a{d}b"),
        new("a{e}{n}b", "a{n}b"),
        new("a{e}{e}b", "a{e}b"),
        new("{e}{d}{e}{d}", "{d}{d}"),
        new("{e}{d}{e}{d}a", "{d}{d}a"),
        new("{e}{d}a{e}{d}", "{d}a{d}"),

        // Quoting + Escaping

        new("{e}{q}a", "{q}a"),
        new("a{e}{q}", "a{q}"),
        new("{q}{e}{q}{q}", "{q}"),
        new("{q}{e}{q}a{q}", "{q}a"),
        new("{e}{q}{e}{q}", "{q}{q}"),
        new("{e}{q}{e}{q}a", "{q}{q}a"),
        new("{q}{e}{q}{e}{q}{q}", "{q}{q}"),
        new("{q}{e}{q}{e}{q}a{q}", "{q}{q}a"),
        new("{q}{e}{q}a{e}{q}{q}", "{q}a{q}"),
        new("{q}{e}{q}a{d}{e}{q}{n}{q}", "{q}a{d}{q}{n}")
    ];

    private T_SplitQuotedRawParsedFieldPair(string rawFormat, string parsedFormat)
    {
        _rawFormat = rawFormat;
        _parsedFormat = parsedFormat;
    }

    private readonly string _rawFormat;
    private readonly string _parsedFormat;

    internal bool IsRelevant(StringQuotedSignals signals)
    {
        return (signals.DelimiterIsSpecified || !Contains("{d}"))
            && (signals.QuoteIsSpecified || !Contains("{q}"))
            && (signals.NewRowIsSpecified || !Contains("{n}"))
            && (signals.EscapeIsSpecified || !Contains("{e}"));

        bool Contains(string format) =>
            _rawFormat.Contains(format) || _parsedFormat.Contains(format);
    }
}
