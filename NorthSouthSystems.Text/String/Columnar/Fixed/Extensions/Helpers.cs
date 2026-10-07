using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace NorthSouthSystems.Text;

public static partial class StringFixedExtensions
{
    internal static void VerifyColumnWidths([NotNull] int[] columnWidths)
    {
        ArgumentNullException.ThrowIfNull(columnWidths);

        if (columnWidths.Length == 0)
            throw new ArgumentException("Length must be > 0", nameof(columnWidths));

        if (Array.Exists(columnWidths, width => width <= 0))
            throw new ArgumentOutOfRangeException(nameof(columnWidths), "Each column width must be > 0");
    }

    internal static void VerifyCoalesceAndFitFields([NotNull] string[] fields, int[] columnWidths, bool leftToFit)
    {
        ArgumentNullException.ThrowIfNull(fields);

        if (fields.Length != columnWidths.Length)
            throw new ArgumentException("Number of fields must equal number of column widths.");

        var errors = new List<string>();

        for (int i = 0; i < fields.Length; i++)
        {
            fields[i] = fields[i] ?? string.Empty;

            if (leftToFit)
                fields[i] = fields[i].Left(columnWidths[i]);
            else if (fields[i].Length > columnWidths[i])
            {
                errors.Add(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Column index (0-based): {0}. Column width: {1}. Field length: {2}.",
                        i,
                        columnWidths[i],
                        fields[i].Length));
            }
        }

        if (errors.Count > 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(fields),
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Each field's length must be <= to its corresponding column's width (use leftToFit = true if truncation is allowed).{0}{1}",
                    Environment.NewLine,
                    string.Join(Environment.NewLine, errors)));
        }
    }
}
