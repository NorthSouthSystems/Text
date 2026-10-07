namespace NorthSouthSystems.Text;

/// <summary>
/// StringSchema is a class to manage a Schema's StringSchemaEntries while validating that they are logically correct.
/// A valid StringSchema has no Entry with a Header that StartWiths any other Entry's Header.
/// <see cref="NorthSouthSystems.Text.StringSchemaExtensions">StringSchemaExtensions</see> explains a "schema'd" sequence of characters.
/// </summary>
public sealed class StringSchema
{
    /// <summary>
    /// Creates a new StringSchema without any Entries.
    /// </summary>
    public StringSchema() { }

    private readonly Dictionary<string, StringSchemaEntry> _entries = new();

    /// <summary>
    /// Validates and adds a new StringSchemaEntry to the Schema.
    /// </summary>
    public void AddEntry(StringSchemaEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        if (_entries.Values.Any(existingEntry => existingEntry.HeaderOverlaps(entry)))
            throw new ArgumentOutOfRangeException(
                nameof(entry),
                entry.Header,
                "No entry.Header may StartWith any other existing entry.Header.");

        _entries.Add(entry.Header, entry);
    }

    public StringSchemaEntry this[string header] => _entries[header];

    internal StringSchemaEntry GetEntryForRow(string row) =>
        _entries.Values.FirstOrDefault(entry => row.StartsWith(entry.Header, StringComparison.Ordinal))
        ?? throw new ArgumentOutOfRangeException(nameof(row), row, "No matching schema definition.");
}
