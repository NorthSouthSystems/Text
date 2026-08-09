namespace NorthSouthSystems.Text;

/// <summary>
/// Extension methods for Splitting and Joining rows (sequences of chars) arranged in fixed-width columns.
/// The number and size of columns is determined by a StringSchema and its StringSchemaEntrys. Rows must StartsWith
/// the Header of exactly one Entry from a StringSchema. That Entry then contains the Widths
/// to use when processing that row along with the designated FillCharacter.
/// </summary>
public static partial class StringSchemaExtensions
{
    /// <summary>
    /// Joins the set of string fields using the StringSchemaEntry for instructions.
    /// </summary>
    /// <param name="fields">The fields (strings) to be joined.</param>
    /// <param name="leftToFit">Determines whether to Left a field to fit its column's width or throw an exception when a field exceeds the allowable column width. (default = false)</param>
    /// <example>
    /// <code>
    /// var a = new StringSchemaEntry("A", new[] { 1, 1, 1 });
    /// var b = new StringSchemaEntry("B", new[] { 2, 2, 2 });
    /// var c = new StringSchemaEntry("C", new[] { 2, 2, 2 }, '-');
    ///
    /// var fields = new[] { "1", "2", "3" };
    /// string row = fields.JoinSchemaRow(a, false);
    /// Console.WriteLine(row);
    ///
    /// fields = new[] { "12", "34", "56" };
    /// row = fields.JoinSchemaRow(b, false);
    /// Console.WriteLine(row);
    ///
    /// fields = new[] { "1", "2", "3" };
    /// row = fields.JoinSchemaRow(c, false);
    /// Console.WriteLine(row);
    /// </code>
    /// Console Output:<br/>
    /// A123<br/>
    /// B123456<br/>
    /// C1-2-3-<br/>
    /// </example>
    public static string JoinSchemaRow(this string[] fields, StringSchemaEntry entry, bool leftToFit = false)
    {
        ArgumentNullException.ThrowIfNull(fields);
        ArgumentNullException.ThrowIfNull(entry);

        return entry.Header
            + StringFixedExtensions.JoinFixedRowNoVerifyColumnWidths(fields, entry.Widths, entry.FillCharacter, leftToFit);
    }
}
