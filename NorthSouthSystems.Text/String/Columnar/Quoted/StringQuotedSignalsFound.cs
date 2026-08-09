namespace NorthSouthSystems.Text;

internal readonly struct StringQuotedSignalsFound
{
    internal StringQuotedSignalsFound(StringQuotedSignals signals, string field)
    {
        DelimiterFound = signals.Delimiters.Any(field.Contains);
        NewRowFound = signals.NewRows.Any(field.Contains);
        QuoteFound = signals.QuoteIsSpecified && field.Contains(signals.Quote, StringComparison.CurrentCulture);
        EscapeFound = signals.EscapeIsSpecified && field.Contains(signals.Escape, StringComparison.CurrentCulture);

        RequiresQuotingOrEscaping = DelimiterFound || QuoteFound || NewRowFound;
    }

    internal readonly bool DelimiterFound { get; }
    internal readonly bool NewRowFound { get; }
    internal readonly bool QuoteFound { get; }
    internal readonly bool EscapeFound { get; }

    internal readonly bool RequiresQuotingOrEscaping { get; }
}
