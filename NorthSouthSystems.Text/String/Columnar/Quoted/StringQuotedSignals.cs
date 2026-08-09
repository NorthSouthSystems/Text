namespace NorthSouthSystems.Text;

/// <summary>
/// Represents the Signals that determine how fields of a row (sequence of chars) are delimited
/// (separated by Delimiter), quoted (surrounded by Quote) or escaped (contain signals preceeded by Escape)
/// to allow themselves to contain an instance of Delimiter, and put onto new rows (rows separated by NewRow) in the
/// case of serialization.
/// </summary>
/// <remarks>
/// Although the functionality is referred to as StringQuoted, a Quote signal is not required; a Delimiter is the
/// only required signal. Escaping is not a feature that is mutally exclusive to Quoting; however, it can entirely
/// obsolete Quoting if the Escape and Delimiter are both single character signals. Combining Escaping and Quoting
/// can result in unexpected behavior; however, there are several rules of thumb.
/// <list type="bullet">
/// <item>
/// <description>When Splitting, Escape behaves identically both outside of and inside of Quotes.</description>
/// </item>
/// <item>
/// <description>
/// When Joining, a Quote signal will be Escaped while inside of Quotes. If the Escape signal
/// is specified, it will be used. If not, a preceeding Quote will be used as Escape.
/// </description>
/// </item>
/// <item>
/// <description>
/// When Joining, an Escape within a field is always Escaped by a second Escape.
/// </description>
/// </item>
/// </list>
/// One other important thing to note is that Escaping multiple character signals can have unexpected results.
/// </remarks>
public sealed class StringQuotedSignals
{
    public static StringQuotedSignals CsvNewRowLinux { get; } =
        new StringQuotedSignalsBuilder()
            .Delimiter(",")
            .NewRow("\n")
            .Quote("\"")
            .ToSignals();

    public static StringQuotedSignals CsvNewRowWindows { get; } =
        new StringQuotedSignalsBuilder()
            .Delimiter(",")
            .NewRow("\r\n")
            .Quote("\"")
            .ToSignals();

    public static StringQuotedSignals CsvNewRowTolerantWindowsPrimaryRFC4180 { get; } =
        new StringQuotedSignalsBuilder()
            .Delimiter(",")
            .NewRowTolerantWindowsPrimary()
            .Quote("\"")
            .ToSignals();

    private static readonly IEnumerable<string> _newRowsTolerant = ["\n", "\r", "\r\n"];

    internal StringQuotedSignals(string[]? delimiters, string[]? newRows, string? quote, string? escape)
    {
        Delimiters = (delimiters ?? []).Where(string.IsNotNullAndNotEmpty).ToArray();
        Delimiter = Delimiters.Count > 0 ? Delimiters[0].NullToEmpty() : string.Empty;

        if (!DelimiterIsSpecified)
            throw new ArgumentException("Delimiter must be non-null and non-empty.");

        NewRows = (newRows ?? []).Where(string.IsNotNullAndNotEmpty).ToArray();
        NewRow = NewRows.Count > 0 ? NewRows[0].NullToEmpty() : string.Empty;

        Quote = quote.NullToEmpty();
        Escape = escape.NullToEmpty();

        Escaping = new(() => new(this));
    }

    public bool DelimiterIsSpecified => !string.IsNullOrEmpty(Delimiter);
    public string Delimiter { get; }
    public IReadOnlyList<string> Delimiters { get; }

    public bool NewRowIsSpecified => !string.IsNullOrEmpty(NewRow);
    public string NewRow { get; }
    public IReadOnlyList<string> NewRows { get; }

    // StringQuotedSignalsBuilder calls Distinct.
    internal bool IsNewRowTolerant =>
        NewRows.Count == 3 && NewRows.All(_newRowsTolerant.Contains);

    public bool QuoteIsSpecified => !string.IsNullOrEmpty(Quote);
    public string Quote { get; }

    public bool EscapeIsSpecified => !string.IsNullOrEmpty(Escape);
    public string Escape { get; }

    internal bool IsSimple =>
        Delimiters.Count == 1
        && Delimiter.Length == 1
        && (!NewRowIsSpecified || (NewRows.Count == 1 && NewRow.Length == 1))
        && (!QuoteIsSpecified || Quote.Length == 1)
        && (!EscapeIsSpecified || Escape.Length == 1);

    internal bool IsNewRowTolerantSimple =>
        Delimiters.Count == 1
        && Delimiter.Length == 1
        && IsNewRowTolerant
        && (!QuoteIsSpecified || Quote.Length == 1)
        && (!EscapeIsSpecified || Escape.Length == 1);

    internal Lazy<StringQuotedSignalsEscaping> Escaping { get; }
}

internal sealed class StringQuotedSignalsEscaping
{
    internal StringQuotedSignalsEscaping(StringQuotedSignals signals)
    {
        EscapedDelimiters = Construct(signals.Escape, signals.Delimiters);
        EscapedNewRows = Construct(signals.Escape, signals.NewRows);
        EscapedQuote = (signals.EscapeIsSpecified ? signals.Escape : signals.Quote) + signals.Quote;
        EscapedEscape = signals.Escape + signals.Escape;
    }

    private static (string ReplaceOld, string ReplaceNew)[]
        Construct(string escape, IReadOnlyList<string> multiSignal)
    {
        if (multiSignal.Count == 0)
            return [];

        if (multiSignal.Count == 1)
            return [(multiSignal[0], escape + multiSignal[0])];

        var replacements = new List<(string ReplaceOld, string ReplaceNew)>(multiSignal.Count);

        foreach (string signal in multiSignal.OrderBy(s => s.Length))
        {
            string replaceOld = signal;

            foreach (var replacement in replacements)
                replaceOld = replaceOld.Replace(replacement.ReplaceOld, replacement.ReplaceNew, StringComparison.CurrentCulture);

            replacements.Add((replaceOld, escape + signal));
        }

        return replacements.ToArray();
    }

    internal IReadOnlyList<(string ReplaceOld, string ReplaceNew)> EscapedDelimiters { get; }
    internal IReadOnlyList<(string ReplaceOld, string ReplaceNew)> EscapedNewRows { get; }
    internal string EscapedQuote { get; }
    internal string EscapedEscape { get; }
}
