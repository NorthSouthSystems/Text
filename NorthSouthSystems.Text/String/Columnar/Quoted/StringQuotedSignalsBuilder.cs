using System.Runtime.CompilerServices;

namespace NorthSouthSystems.Text;

public sealed class StringQuotedSignalsBuilder
{
    /// <summary>
    /// String used to separate fields of a row. It is the only required Signal.
    /// </summary>
    /// <param name="primary">Used for Splitting and Joining.</param>
    /// <param name="alternates">Used only for Splitting.</param>
    public StringQuotedSignalsBuilder Delimiter(string primary, params string[] alternates) =>
        MultiHelper(() => _delimiters, x => _delimiters = x, primary, alternates);

    private string[]? _delimiters;

    // We allow NewRowTolerant multi-signals to "starts-with-contains-overlap" (see MultiHelper), unlike Delimiters.
    // This is because we can safely hardcode this very common case in our SplitQuotedProcessor. Admittedly, the most
    // common case is the overlap of Linux and Windows NewRows, which could be handled without any special coding;
    // however, we want to be thorough and additionally handle the case of standalone Carriage Return and Windows NewRows
    // overlapping. To allows such overlap, we bypass MultiHelper and call Helper directly.

    public StringQuotedSignalsBuilder NewRowTolerantEnvironmentPrimary() => Environment.NewLine switch
    {
        "\r" => NewRowTolerantCarriageReturnPrimary(),
        "\n" => NewRowTolerantLinuxPrimary(),
        "\r\n" => NewRowTolerantWindowsPrimary(),

        _ => throw new NotSupportedException(Environment.NewLine)
    };

    public StringQuotedSignalsBuilder NewRowTolerantCarriageReturnPrimary() =>
        Helper(() => _newRows, x => _newRows = x, ["\r", "\n", "\r\n"]);

    public StringQuotedSignalsBuilder NewRowTolerantLinuxPrimary() =>
        Helper(() => _newRows, x => _newRows = x, ["\n", "\r", "\r\n"]);

    public StringQuotedSignalsBuilder NewRowTolerantWindowsPrimary() =>
        Helper(() => _newRows, x => _newRows = x, ["\r\n", "\n", "\r"]);

    /// <summary>
    /// String used to separate rows.
    /// </summary>
    /// <param name="primary">Used for Splitting and Joining.</param>
    /// <param name="alternates">Used only for Splitting.</param>
    public StringQuotedSignalsBuilder NewRow(string primary, params string[] alternates) =>
        MultiHelper(() => _newRows, x => _newRows = x, primary, alternates);

    private string[]? _newRows;

    /// <summary>
    /// String used to surround (i.e. quote) a field and allow it to contain an instance of Delimiter or NewRow.
    /// </summary>
    public StringQuotedSignalsBuilder Quote(string signal) =>
        Helper(() => _quote, x => _quote = x, signal);

    private string? _quote;

    /// <summary>
    /// String used to escape the meaning of the immediately following character.
    /// </summary>
    public StringQuotedSignalsBuilder Escape(string signal) =>
        Helper(() => _escape, x => _escape = x, signal);

    private string? _escape;

    private StringQuotedSignalsBuilder MultiHelper(
        Func<string[]?> getter,
        Action<string[]?> setter,
        string primary,
        string[] alternates,
        [CallerMemberName] string? callerMemberName = null)
    {
        string[] multi = alternates.Prepend(primary).Where(string.IsNotNullAndNotEmpty).Distinct().ToArray();

        // We know that all strings are NotNull, NotEmpty (Length >= 1), and Distinct by the LINQ above. We also know that
        // AnyPermutationPair sorts from longest to shortest string. We leverage both facts here when we ensure that no signals
        // are impossible to trigger by being "hidden" by other "nested" signals.
        if (StringExtensions.AnyPermutationPair(
                multi,
                (x, y) => x.Length > y.Length && x[..^1].Contains(y, StringComparison.CurrentCulture)))
        {
            throw new ArgumentException(
                $"{callerMemberName} may only EndsWith another {callerMemberName}. All other StartsWith or Contains are invalid.");
        }

        return Helper(getter, setter, multi);
    }

    private StringQuotedSignalsBuilder Helper<T>(Func<T> getter, Action<T> setter, T multiOrSignal)
    {
        var rollback = getter();

        setter(multiOrSignal);

        if (AnySignalContainsAnyOther())
        {
            setter(rollback);
            throw new ArgumentException("No Signal may Contains any other Signal.");
        }

        return this;
    }

    private bool AnySignalContainsAnyOther()
    {
        foreach (string? delimiter in (_delimiters ?? []).DefaultIfEmpty())
        foreach (string? newRow in (_newRows ?? []).DefaultIfEmpty())
        {
            if (AnyPermutationPairContains(delimiter, newRow, _quote, _escape))
                return true;
        }

        return false;

        static bool AnyPermutationPairContains(params string?[] signals) =>
            StringExtensions.AnyPermutationPairContains(signals.Where(string.IsNotNullAndNotEmpty).Select(s => s!));
    }

    public StringQuotedSignals ToSignals()
    {
        if ((_delimiters?.Length ?? 0) == 0)
            throw new ArgumentException("Delimiter must be non-null and non-empty.");

        return new(_delimiters, _newRows, _quote, _escape);
    }
}
