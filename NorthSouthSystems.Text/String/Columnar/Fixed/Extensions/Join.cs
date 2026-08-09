using System.Text;

namespace NorthSouthSystems.Text;

/// <summary>
/// Extension methods for Splitting and Joining rows (sequences of chars), arranged in fixed-width columns.
/// </summary>
public static partial class StringFixedExtensions
{
    /// <summary>
    /// Joins an array of fields (strings) together so that each field is "filled" to occupy a column with an exact width.
    /// </summary>
    /// <param name="fields">The fields (strings) to be joined.</param>
    /// <param name="columnWidths">The width of each column. E.g. The first field in fields will be place into a column the size of the first width in columnWidths.</param>
    /// <param name="fillCharacter">When a field is less than its column's width, fillCharacter is added until the columnWidth is reached. (default = ' ')</param>
    /// <param name="leftToFit">Determines whether to Left a field to fit its column's width or throw an exception when a field exceeds the allowable column width. (default = false)</param>
    /// <example>
    /// <code>
    /// string fields = new [] { "A", "B", "C" };
    /// string row = fields.JoinFixedRow(new [] { 1, 1, 1 }, ' ', false);
    /// Console.WriteLine(row);
    ///
    /// row = fields.JoinFixedRow(new [] { 1, 1, 1 }, '-', false);
    /// Console.WriteLine(row);
    /// </code>
    /// Console Output:<br/>
    /// ABC<br/>
    /// ABC<br/>
    /// <code>
    /// string fields = new [] { "A", "B", "C" };
    /// string row = fields.JoinFixedRow(new [] { 2, 1, 1 }, ' ', false);
    /// Console.WriteLine(row);
    ///
    /// row = fields.JoinFixedRow(new [] { 2, 1, 1 }, '-', false);
    /// Console.WriteLine(row);
    /// </code>
    /// Console Output:<br/>
    /// A BC<br/>
    /// A-BC<br/>
    /// <code>
    /// string fields = new [] { "A", "B", "C" };
    /// string row = fields.JoinFixedRow(new [] { 2, 2, 1 }, ' ', false);
    /// Console.WriteLine(row);
    ///
    /// row = fields.JoinFixedRow(new [] { 2, 2, 1 }, '-', false);
    /// Console.WriteLine(row);
    /// </code>
    /// Console Output:<br/>
    /// A B C<br/>
    /// A-B-C<br/>
    /// <code>
    /// string fields = new [] { "ABC", "123" };
    /// string row = fields.JoinFixedRow(new [] { 2, 2 }, ' ', true);
    /// Console.WriteLine(row);
    /// </code>
    /// Console Output:<br/>
    /// AB12<br/>
    /// </example>
    public static string JoinFixedRow(this string[] fields, int[] columnWidths, char fillCharacter = ' ', bool leftToFit = false)
    {
        VerifyColumnWidths(columnWidths);

        return JoinFixedRowNoVerifyColumnWidths(fields, columnWidths, fillCharacter, leftToFit);
    }

    /// <summary>
    /// This method exists solely as a performance optimization for StringSchemaExtensions.JoinSchemaRow. SchemaEntry ctor calls VerifyColumnWidths;
    /// therefore, JoinSchemaRow can call this method and bypass redundant calls to VerifyColumnWidths.
    /// </summary>
    internal static string JoinFixedRowNoVerifyColumnWidths(string[] fields, int[] columnWidths, char fillCharacter, bool leftToFit)
    {
        VerifyCoalesceAndFitFields(fields, columnWidths, leftToFit);

        var row = new StringBuilder();

        for (int i = 0; i < fields.Length; i++)
        {
            string field = fields[i];
            row.Append(field);

            int fillCount = columnWidths[i] - field.Length;

            if (fillCount > 0)
                row.Append(fillCharacter, fillCount);
        }

        return row.ToString();
    }
}
