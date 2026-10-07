namespace NorthSouthSystems.Text;

public static partial class StringSchemaExtensions
{
    /// <summary>
    /// Splits a row (string) using the provided StringSchema. Exactly one Entry must be found in the Schema
    /// for which the row StartsWith the Entry's Header. This Entry then provides Width and FillCharacter
    /// parameters needed to perform the Split.
    /// </summary>
    /// <param name="row">The row (string) to Split after finding its StringSchemaEntry.</param>
    /// <param name="schema">The StringSchema containing exactly one Entry for whom this row StartsWith the Entry's Header.</param>
    /// <example>
    /// <code>
    /// var schema = new StringSchema();
    /// schema.AddEntry(new StringSchemaEntry("A", new[] { 1, 1, 1 }));
    /// schema.AddEntry(new StringSchemaEntry("B", new[] { 2, 2, 2 }));
    /// schema.AddEntry(new StringSchemaEntry("CD", new[] { 3, 3, 3 }));
    ///
    /// var split = "A123".SplitSchemaRow(schema);
    /// Console.WriteLine(split.Entry.Header);
    ///
    /// foreach(StringFieldWrapper field in split.Result.Fields)
    ///     Console.WriteLine(field);
    ///
    /// split = "B123456".SplitSchemaRow(schema);
    /// Console.WriteLine(split.Entry.Header);
    ///
    /// foreach(StringFieldWrapper field in split.Result.Fields)
    ///     Console.WriteLine(field);
    ///
    /// split = "CD123456789".SplitSchemaRow(schema);
    /// Console.WriteLine(split.Entry.Header);
    ///
    /// foreach(StringFieldWrapper field in split.Result.Fields)
    ///     Console.WriteLine(field);
    /// </code>
    /// Console Output:<br/>
    /// A<br/>
    /// 1<br/>
    /// 2<br/>
    /// 3<br/>
    /// B<br/>
    /// 12<br/>
    /// 34<br/>
    /// 56<br/>
    /// CD<br/>
    /// 123<br/>
    /// 456<br/>
    /// 789<br/>
    /// </example>
    public static StringSchemaSplitResult SplitSchemaRow(this string row, StringSchema schema)
    {
        ArgumentNullException.ThrowIfNull(row);
        ArgumentNullException.ThrowIfNull(schema);

        var entry = schema.GetEntryForRow(row);

        using var charEnumerator = row.Skip(entry.Header.Length).GetEnumerator();

        return new(
            entry,
            StringFixedExtensions.SplitFixedRowImplementation(charEnumerator, entry.Widths, entry.FillCharacter));
    }
}
