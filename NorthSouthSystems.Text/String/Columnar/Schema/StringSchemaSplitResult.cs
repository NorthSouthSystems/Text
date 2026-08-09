namespace NorthSouthSystems.Text;

/// <summary>
/// StringSchemaSplitResult holds both Schema data and Split result data.
/// </summary>
public sealed class StringSchemaSplitResult
{
    internal StringSchemaSplitResult(StringSchemaEntry entry, string[] fields)
    {
        Entry = entry;
        Result = entry.RowWrapperFactory.Wrap(fields);
    }

    public StringSchemaEntry Entry { get; }
    public StringRowWrapper Result { get; }
}
