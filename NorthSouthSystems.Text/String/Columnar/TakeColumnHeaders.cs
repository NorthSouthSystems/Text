namespace NorthSouthSystems.Text;

public static class TakeColumnHeadersExtensions
{
    /// <summary>
    /// Provides an extension method for converting rows of fields (IEnumerable&lt;string[]&gt;) to
    /// StringRowWrappers (IEnumerable&lt;StringRowWrapper&gt;) by taking the first row to construct
    /// a StringRowWrapperFactory used to wrap subsequent rows. This is intended to be used fluently
    /// after a call to any of the Split*Rows extensions methods.
    /// </summary>
    /// <param name="rowsOfFields">The rows of fields resulting from a Split*Rows invocation.</param>
    /// <param name="expectedColumnNames">An optional parameter indicating that the caller would like
    /// an ArgumentException thrown if the first row is not this exact set of fields.
    /// (default = null)</param>
    /// <param name="enforceExpectedColumnNamesOrder">An optional parameter indicating that the caller
    /// would like an ArgumentException thrown if the first row is not the exact sequence of fields
    /// specified by expectedColumnNames. (default = false)</param>
    /// <returns>The rows of fields converted to StringRowWrappers excluding the first row representing
    /// column headers.</returns>
    public static IEnumerable<StringRowWrapper> TakeColumnHeaders(
        this IEnumerable<string[]> rowsOfFields,
        IEnumerable<string>? expectedColumnNames = null,
        bool enforceExpectedColumnNamesOrder = false)
    {
        ArgumentNullException.ThrowIfNull(rowsOfFields);

        return TakeColumnHeadersIterator(rowsOfFields, expectedColumnNames, enforceExpectedColumnNamesOrder);
    }

    private static IEnumerable<StringRowWrapper> TakeColumnHeadersIterator(
        IEnumerable<string[]> rowsOfFields,
        IEnumerable<string>? expectedColumnNames,
        bool enforceExpectedColumnNamesOrder)
    {
        StringRowWrapperFactory? rowWrapperFactory = null;

        foreach (string[] rowFields in rowsOfFields)
        {
            if (rowWrapperFactory == null)
            {
                if (expectedColumnNames?.Any() ?? false)
                    ThrowIfIncorrectColumnNames(rowFields, expectedColumnNames, enforceExpectedColumnNamesOrder);

                rowWrapperFactory = new(rowFields);
            }
            else
                yield return rowWrapperFactory.Wrap(rowFields);
        }
    }

    private static void ThrowIfIncorrectColumnNames(
        string[] rowFields,
        IEnumerable<string> expectedColumnNames,
        bool enforceExpectedColumnNamesOrder)
    {
        var errors = new List<string>();

        errors.AddRange(expectedColumnNames.Except(rowFields).Select(c => "Expected: " + c));
        errors.AddRange(rowFields.Except(expectedColumnNames).Select(c => "Unexpected: " + c));

        if (errors.Count == 0 && enforceExpectedColumnNamesOrder && !rowFields.SequenceEqual(expectedColumnNames))
            errors.Add("Column names are not in the expected order.");

        if (errors.Count > 0)
            throw new ArgumentException(string.Join(Environment.NewLine, errors));
    }
}
