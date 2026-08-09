namespace NorthSouthSystems.Text;

public static partial class StringExtensions
{
    /// <summary>
    /// Returns a string containing a specified number of characters from the left side of a string. string.Substring
    /// is not suitable because it will throw an exception when <paramref name="length"/> exceeds the length
    /// of the original string. Mimics the behavior of <see cref="Microsoft.VisualBasic.Strings.Left(String, Int32)"/>;
    /// however, Exception Types may be different.
    /// </summary>
    public static string Left(this string value, int length)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentOutOfRangeException.ThrowIfNegative(length);

        if (length == 0)
            return string.Empty;
        else if (value.Length <= length)
            return value;
        else
            return value.Substring(0, length);
    }

    /// <summary>
    /// Returns a string containing a specified number of characters from the right side of a string.
    /// Mimics the behavior of <see cref="Microsoft.VisualBasic.Strings.Right(String, Int32)"/>;
    /// however, Exception Types may be different.
    /// </summary>
    public static string Right(this string value, int length)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentOutOfRangeException.ThrowIfNegative(length);

        if (length == 0)
            return string.Empty;
        else if (value.Length <= length)
            return value;
        else
            return value.Substring(value.Length - length, length);
    }
}
