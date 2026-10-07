namespace NorthSouthSystems.Text;

/// <summary>
/// Extensions for Splitting and Joining delimited sequences of characters (rows) that may possess quoted (surrounded by Quote)
/// fields in order for them to contain instances of Delimiter.
/// </summary>
public static partial class StringQuotedExtensions
{
    public static string JoinQuotedRows(
        this IEnumerable<IEnumerable<string>> rowsOfFields,
        StringQuotedSignals signals,
        bool forceQuotes = false)
    {
        ArgumentNullException.ThrowIfNull(rowsOfFields);
        ArgumentNullException.ThrowIfNull(signals);

        if (!signals.NewRowIsSpecified)
            throw new ArgumentException("signals.NewRow must not be null or empty.");

        if (forceQuotes && !signals.QuoteIsSpecified)
            throw new ArgumentException("Quote'ing forced; therefore, signals.Quote must not be null or empty.");

        return string.Join(
            signals.NewRow,
            rowsOfFields.Select(fields => JoinQuotedRowImpl(fields, signals, forceQuotes)));
    }

    /// <summary>
    /// Joins a sequence of fields, separates them with Delimiter, and allows for instances of Delimiter (or the NewRow signal)
    /// to occur within individual fields.  Such fields will be quoted (surrounded by Quote) to allow for this behavior. Instances
    /// of the Quote signal within fields will be escaped by doubling (Quote + Quote).
    /// </summary>
    /// <param name="fields">The fields (strings) to be joined.</param>
    /// <param name="forceQuotes">
    /// Dictates whether to force every field to be quoted regardless of whether or not the field contains an instance
    /// of Delimiter or NewRow. (default = false)
    /// </param>
    /// <example>
    /// <code>
    /// string[] fields = new string[] { "a", "b", "c" };
    /// string result = fields.JoinQuotedRow(StringQuotedSignals.CsvNewRowTolerantWindowsPrimaryRFC4180, false);
    /// Console.WriteLine(result);
    /// </code>
    /// Console Output:<br/>
    /// a,b,c<br/>
    /// <code>
    /// string[] fields = new string[] { "a,a", "b", "c" };
    /// string result = fields.JoinQuotedRow(StringQuotedSignals.CsvNewRowTolerantWindowsPrimaryRFC4180, false);
    /// Console.WriteLine(result);
    /// </code>
    /// Console Output:<br/>
    /// "a,a",b,c<br/>
    /// <code>
    /// string[] fields = new string[] { "a", "b" + Environment.NewLine + "b", "c" };
    /// string result = fields.JoinQuotedRow(StringQuotedSignals.CsvNewRowTolerantWindowsPrimaryRFC4180, true);
    /// Console.WriteLine(result);
    /// </code>
    /// Console Output:<br/>
    /// "a","b<br/>
    /// b","c"<br/>
    /// <code>
    /// string[] fields = new string[] { "a\"a", "b", "c" };
    /// string result = fields.JoinQuotedRow(StringQuotedSignals.CsvNewRowTolerantWindowsPrimaryRFC4180, false);
    /// Console.WriteLine(result);
    /// </code>
    /// Console Output:<br/>
    /// a""a,b,c<br/>
    /// </example>
    public static string JoinQuotedRow(
        this IEnumerable<string> fields,
        StringQuotedSignals signals,
        bool forceQuotes = false)
    {
        ArgumentNullException.ThrowIfNull(signals);

        if (forceQuotes && !signals.QuoteIsSpecified)
            throw new ArgumentException("Quote'ing forced; therefore, signals.Quote must not be null or empty.");

        return JoinQuotedRowImpl(fields, signals, forceQuotes);
    }

    private static string JoinQuotedRowImpl(IEnumerable<string> fields, StringQuotedSignals signals, bool forceQuotes)
    {
        ArgumentNullException.ThrowIfNull(fields);

        return string.Join(
            signals.Delimiter,
            fields.Select(field => QuoteAndEscapeField(field ?? string.Empty, signals, forceQuotes)));
    }

    private static string QuoteAndEscapeField(string field, StringQuotedSignals signals, bool forceQuotes)
    {
        var found = new StringQuotedSignalsFound(signals, field);
        var escaping = signals.Escaping.Value;

        if (found.EscapeFound)
            field = field.Replace(signals.Escape, escaping.EscapedEscape, StringComparison.CurrentCulture);

        if (signals.QuoteIsSpecified)
        {
            if (forceQuotes || found.RequiresQuotingOrEscaping)
                field = signals.Quote
                    + field.Replace(signals.Quote, escaping.EscapedQuote, StringComparison.CurrentCulture)
                    + signals.Quote;
        }
        else if (signals.EscapeIsSpecified)
        {
            if (found.DelimiterFound)
            {
                foreach (var escapedDelimiter in escaping.EscapedDelimiters)
                    field = field.Replace(
                        escapedDelimiter.ReplaceOld,
                        escapedDelimiter.ReplaceNew,
                        StringComparison.CurrentCulture);
            }

            if (found.NewRowFound)
            {
                foreach (var escapedNewRow in escaping.EscapedNewRows)
                    field = field.Replace(
                        escapedNewRow.ReplaceOld,
                        escapedNewRow.ReplaceNew,
                        StringComparison.CurrentCulture);
            }
        }
        else if (found.RequiresQuotingOrEscaping)
            throw new ArgumentException(
                "Quoting or Escaping is required; therefore, either signals.Quote or signals.Escape must not be null or empty.");

        return field;
    }
}
