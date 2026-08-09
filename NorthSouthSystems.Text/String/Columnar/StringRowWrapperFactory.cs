using System.Globalization;
using System.Threading;

namespace NorthSouthSystems.Text;

/// <summary>
/// StringRowWrapperFactory is the only path for creating StringRowWrappers. Given columnNames, it can then be used
/// to wrap the fields of a row in a StringRowWrapper. StringRowWrappers then allow for index or column name based
/// access to the fields of a row.
/// </summary>
public sealed class StringRowWrapperFactory
{
    public StringRowWrapperFactory(string[] columnNames)
    {
        ArgumentNullException.ThrowIfNull(columnNames);

        string[] duplicateColumnNames = columnNames.GroupBy(columnName => columnName)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToArray();

        if (duplicateColumnNames.Length > 0)
            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Duplicate columnNames are not allowed: {0}.", string.Join(", ", duplicateColumnNames)));

        ColumnNames = new string[columnNames.Length];
        Array.Copy(columnNames, ColumnNames, columnNames.Length);

        int index = 0;
        _columnNameIndices = ColumnNames.ToDictionary(columnName => columnName, columnName => index++);
    }

    internal string[] ColumnNames { get; }

    private readonly Dictionary<string, int> _columnNameIndices;

    private int _wrapCounter;

    public StringRowWrapper Wrap(string[] fields)
    {
        int wrapCounter = Interlocked.Increment(ref _wrapCounter);

        if (fields == null)
            throw new ArgumentNullException(nameof(fields), FormattableString.Invariant($"wrapCounter = {wrapCounter}"));

        if (fields.Length > ColumnNames.Length)
            throw new ArgumentException(FormattableString.Invariant($"The number of fields must be <= the number of columns. wrapCounter = {wrapCounter}"));

        return new(this, fields);
    }

    internal bool TryGetIndex(string columnName, out int index) => _columnNameIndices.TryGetValue(columnName, out index);
}
