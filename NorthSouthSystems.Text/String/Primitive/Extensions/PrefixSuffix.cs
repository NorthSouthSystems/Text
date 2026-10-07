namespace NorthSouthSystems.Text;

public static partial class StringExtensions
{
    /// <summary>
    /// Removes a prefix, if it exists, from a string.
    /// </summary>
    /// <param name="value">The string from which to remove the prefix.</param>
    /// <param name="prefix">The prefix to remove.</param>
    /// <param name="comparison">The comparison to use when checking for the existence of prefix.</param>
    /// <returns>The new string without the prefix or pass-through value if prefix does not exist.</returns>
    /// <example>
    /// <code>
    /// Console.WriteLine("foobar".RemovePrefix("foo"));
    /// Console.WriteLine("foobar".RemovePrefix("bar"));
    /// Console.WriteLine("foobar".RemovePrefix("FOO"));
    /// Console.WriteLine("foobar".RemovePrefix("FOO", StringComparison.OrdinalIgnoreCase));
    /// </code>
    /// Console Output:<br/>
    /// bar<br/>
    /// foobar<br/>
    /// foobar<br/>
    /// bar<br/>
    /// </example>
    public static string RemovePrefix(this string value, string prefix, StringComparison comparison = default)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentException.ThrowIfNullOrEmpty(prefix);

        return value.StartsWith(prefix, comparison)
            ? value.Length > prefix.Length
                ? value[prefix.Length..]
                : string.Empty
            : value;
    }

    /// <summary>
    /// Removes a suffix, if it exists, from a string.
    /// </summary>
    /// <param name="value">The string from which to remove the suffix.</param>
    /// <param name="suffix">The suffix to remove.</param>
    /// <param name="comparison">The comparison to use when checking for the existence of suffix.</param>
    /// <returns>The new string without the suffix or pass-through value if suffix does not exist.</returns>
    /// <example>
    /// <code>
    /// Console.WriteLine("foobar".RemoveSuffix("foo"));
    /// Console.WriteLine("foobar".RemoveSuffix("bar"));
    /// Console.WriteLine("foobar".RemoveSuffix("BAR"));
    /// Console.WriteLine("foobar".RemoveSuffix("BAR", StringComparison.OrdinalIgnoreCase));
    /// </code>
    /// Console Output:<br/>
    /// foobar<br/>
    /// foo<br/>
    /// foobar<br/>
    /// foo<br/>
    /// </example>
    public static string RemoveSuffix(this string value, string suffix, StringComparison comparison = default)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentException.ThrowIfNullOrEmpty(suffix);

        return value.EndsWith(suffix, comparison)
            ? value.Length > suffix.Length
                ? value[..^suffix.Length]
                : string.Empty
            : value;
    }

    /// <summary>
    /// Replaces a prefix, if it exists, in a string.
    /// </summary>
    /// <param name="value">The string from which to replace the prefix.</param>
    /// <param name="prefix">The prefix to replace.</param>
    /// <param name="replacement">The replacement for the prefix.</param>
    /// <param name="comparison">The comparison to use when checking for the existence of prefix.</param>
    /// <returns>The new string with the prefix replaced or pass-through value if prefix does not exist.</returns>
    /// <example>
    /// <code>
    /// Console.WriteLine("foobar".ReplacePrefix("foo", "gee"));
    /// Console.WriteLine("foobar".ReplacePrefix("bar", "gee"));
    /// Console.WriteLine("foobar".ReplacePrefix("FOO", "gee"));
    /// Console.WriteLine("foobar".ReplacePrefix("FOO", "gee", StringComparison.OrdinalIgnoreCase));
    /// </code>
    /// Console Output:<br/>
    /// geebar<br/>
    /// foobar<br/>
    /// foobar<br/>
    /// geebar<br/>
    /// </example>
    public static string ReplacePrefix(
        this string value,
        string prefix,
        string replacement,
        StringComparison comparison = default)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentException.ThrowIfNullOrEmpty(prefix);

        return value.StartsWith(prefix, comparison)
            ? value.Length > prefix.Length
                ? replacement + value[prefix.Length..]
                : replacement.NullToEmpty()
            : value;
    }

    /// <summary>
    /// Replaces a suffix, if it exists, in a string.
    /// </summary>
    /// <param name="value">The string from which to replace the suffix.</param>
    /// <param name="suffix">The suffix to replace.</param>
    /// <param name="replacement">The replacement for the suffix.</param>
    /// <param name="comparison">The comparison to use when checking for the existence of suffix.</param>
    /// <returns>The new string with the suffix replaced or pass-through value if suffix does not exist.</returns>
    /// <example>
    /// <code>
    /// Console.WriteLine("foobar".ReplaceSuffix("foo", "gee"));
    /// Console.WriteLine("foobar".ReplaceSuffix("bar", "gee"));
    /// Console.WriteLine("foobar".ReplaceSuffix("BAR", "gee"));
    /// Console.WriteLine("foobar".ReplaceSuffix("BAR", "gee", StringComparison.OrdinalIgnoreCase));
    /// </code>
    /// Console Output:<br/>
    /// foobar<br/>
    /// foogee<br/>
    /// foobar<br/>
    /// foogee<br/>
    /// </example>
    public static string ReplaceSuffix(
        this string value,
        string suffix,
        string replacement,
        StringComparison comparison = default)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentException.ThrowIfNullOrEmpty(suffix);

        return value.EndsWith(suffix, comparison)
            ? value.Length > suffix.Length
                ? value[..^suffix.Length] + replacement
                : replacement.NullToEmpty()
            : value;
    }
}
