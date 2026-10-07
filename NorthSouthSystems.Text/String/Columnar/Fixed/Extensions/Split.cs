namespace NorthSouthSystems.Text;

public static partial class StringFixedExtensions
{
    /// <summary>
    /// Splits a row (sequence of chars) into fields arranged in columns of fixed widths. The fillCharacter used to pad
    /// fields to fill their column's width are trimmed from the split results.
    /// </summary>
    /// <param name="row">The row (sequence of chars) to be split.</param>
    /// <param name="columnWidths">The width of each column. E.g. The first field in the result will be found in a column the size of the first width in columnWidths.</param>
    /// <param name="fillCharacter">The character used to pad a field so that its width reaches its column's width. Trimmed from the split results. (default = ' ')</param>
    /// <example>
    /// <code>
    /// string row = "ABC";
    /// string[] fields = row.SplitFixedRow(new [] { 1, 1, 1 }, ' ');
    ///
    /// foreach(string field in fields)
    ///     Console.WriteLine(field);
    ///
    /// fields = row.SplitFixedRow(new [] { 1, 1, 1 }, '-');
    ///
    /// foreach(string field in fields)
    ///     Console.WriteLine(field);
    /// </code>
    /// Console Output:<br/>
    /// A<br/>
    /// B<br/>
    /// C<br/>
    /// A<br/>
    /// B<br/>
    /// C<br/>
    /// <code>
    /// string row = "A-BC";
    /// string[] fields = row.SplitFixedRow(new [] { 2, 1, 1 }, ' ');
    ///
    /// foreach(string field in fields)
    ///     Console.WriteLine(field);
    ///
    /// fields = row.SplitFixedRow(new [] { 2, 1, 1 }, '-');
    ///
    /// foreach(string field in fields)
    ///     Console.WriteLine(field);
    /// </code>
    /// Console Output:<br/>
    /// A-<br/>
    /// B<br/>
    /// C<br/>
    /// A<br/>
    /// B<br/>
    /// C<br/>
    /// <code>
    /// string row = "A-B-C";
    /// string[] fields = row.SplitFixedRow(new [] { 2, 2, 1 }, ' ');
    ///
    /// foreach(string field in fields)
    ///     Console.WriteLine(field);
    ///
    /// fields = row.SplitFixedRow(new [] { 2, 2, 1 }, '-');
    ///
    /// foreach(string field in fields)
    ///     Console.WriteLine(field);
    /// </code>
    /// Console Output:<br/>
    /// A-<br/>
    /// B-<br/>
    /// C<br/>
    /// A<br/>
    /// B<br/>
    /// C<br/>
    /// </example>
    public static string[] SplitFixedRow(this IEnumerable<char> row, int[] columnWidths, char fillCharacter = ' ')
    {
        ArgumentNullException.ThrowIfNull(row);

        VerifyColumnWidths(columnWidths);

        using var charEnumerator = row.GetEnumerator();

        string[] fields = SplitFixedRowImplementation(charEnumerator, columnWidths, fillCharacter);

        if (fields.Length == 0)
            throw new ArgumentException("Empty row.", nameof(row));

        if (charEnumerator.MoveNext())
            throw new ArgumentOutOfRangeException(nameof(row), "row length must equal the sum of all column widths.");

        return fields;
    }

    /// <summary>
    /// See <see cref="SplitFixedRow(IEnumerable{char}, int[], char)"/>.  This method is identical, except that it allow rows to repeat one after another.
    /// As soon as the numbers of characters taken reaches the sum of all column widths, a new row is started.
    /// </summary>
    /// <example>
    /// <code>
    /// string rows = "ABCDEF";
    /// string[][] rowsFields = row.SplitFixedRepeating(new [] { 1, 1, 1 }, ' ');
    ///
    /// foreach(string[] rowFields in rowsFields)
    /// {
    ///     Console.WriteLine("Row");
    ///
    ///     foreach(string field in rowFields)
    ///         Console.WriteLine(field);
    /// }
    /// </code>
    /// Console Output:<br/>
    /// Row<br/>
    /// A<br/>
    /// B<br/>
    /// C<br/>
    /// Row<br/>
    /// D<br/>
    /// E<br/>
    /// F<br/>
    /// <code>
    /// string rows = "A-BCD-EF";
    /// string[][] rowsFields = row.SplitFixedRepeating(new [] { 2, 1, 1 }, '-');
    ///
    /// foreach(string[] rowFields in rowsFields)
    /// {
    ///     Console.WriteLine("Row");
    ///
    ///     foreach(string field in rowFields)
    ///         Console.WriteLine(field);
    /// }
    /// </code>
    /// Console Output:<br/>
    /// Row<br/>
    /// A<br/>
    /// B<br/>
    /// C<br/>
    /// Row<br/>
    /// D<br/>
    /// E<br/>
    /// F<br/>
    /// </example>
    public static IEnumerable<string[]> SplitFixedRepeating(
        this IEnumerable<char> rows,
        int[] columnWidths,
        char fillCharacter = ' ')
    {
        ArgumentNullException.ThrowIfNull(rows);

        VerifyColumnWidths(columnWidths);

        return SplitFixedRepeatingIterator(rows, columnWidths, fillCharacter);
    }

    private static IEnumerable<string[]> SplitFixedRepeatingIterator(
        IEnumerable<char> rows,
        int[] columnWidths,
        char fillCharacter)
    {
        using var charEnumerator = rows.GetEnumerator();

        string[]? fields = null;

        do
        {
            fields = SplitFixedRowImplementation(charEnumerator, columnWidths, fillCharacter);

            if (fields.Length > 0)
                yield return fields;
        } while (fields.Length > 0);
    }

    internal static string[] SplitFixedRowImplementation(
        IEnumerator<char> charEnumerator,
        int[] columnWidths,
        char fillCharacter)
    {
        string[] fields = new string[columnWidths.Length];

        for (int i = 0; i < columnWidths.Length; i++)
        {
            int charsToTake = columnWidths[i];
            char[] charsTaken = new char[charsToTake];

            while (charsToTake > 0)
            {
                if (charEnumerator.MoveNext())
                {
                    charsTaken[charsTaken.Length - charsToTake] = charEnumerator.Current;
                    charsToTake--;
                }
                else if (i == 0 && charsToTake == columnWidths[0]) // Empty enumerator
                    return Array.Empty<string>();
                else
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(charEnumerator),
                        "row length must equal the sum of all column widths.");
                }
            }

            int charsToKeep = columnWidths[i];

            while (charsToKeep > 0 && charsTaken[charsToKeep - 1] == fillCharacter)
                charsToKeep--;

            fields[i] = charsToKeep > 0 ? charsTaken.Take(charsToKeep).ToNewString() : string.Empty;
        }

        return fields;
    }
}
