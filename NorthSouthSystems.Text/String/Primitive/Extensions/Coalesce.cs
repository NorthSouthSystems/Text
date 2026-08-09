namespace NorthSouthSystems.Text;

public static partial class StringExtensions
{
    /// <summary>
    /// VIOLATES NULL REFERENCE SEMANTICS! Will return null if string.IsNullOrEmpty. This simplifies code that treats
    /// null string values the same as it treats Empty string values, and wants only to deal with null.
    /// </summary>
    /// <returns>Returns null when value == string.Empty; else, returns the original string.</returns>
    public static string? EmptyToNull(this string? value) => string.IsNullOrEmpty(value) ? null : value;

    /// <summary>
    /// VIOLATES NULL REFERENCE SEMANTICS! Will return string.Empty if string.IsNullOrEmpty. This simplifies code that treats
    /// null string values the same as it treats Empty string values, and wants only to deal with string.Empty.
    /// </summary>
    /// <returns>Returns string.Empty when value == null; else, returns the original string.</returns>
    public static string NullToEmpty(this string? value) => value ?? string.Empty;

    /// <summary>
    /// VIOLATES NULL REFERENCE SEMANTICS! Will return null if string.IsNullOrWhiteSpace. This simplifies code that treats
    /// null string values the same as it treats WhiteSpace string values, and wants only to deal with null.
    /// </summary>
    /// <returns>Returns null when string.IsNullOrWhiteSpace; else, returns the original string.</returns>
    public static string? WhiteSpaceToNull(this string? value) => string.IsNullOrWhiteSpace(value) ? null : value;

    /// <summary>
    /// VIOLATES NULL REFERENCE SEMANTICS! Will return string.Empty if object == null. This method is provided so that
    /// callers can use a fluent syntax for manipulation of the result of object.ToString(), which is not possible with the
    /// C# 6 ?. operator which terminates the fluent method chain upon encountering null.
    /// </summary>
    /// <example>
    /// object instance = GetInstance();
    /// 
    /// string s = instance.ToStringNullToEmpty()
    ///     .Trim()
    ///     .ToUpperInvariant();
    /// </example>
    public static string ToStringNullToEmpty(this object? instance) => instance?.ToString() ?? string.Empty;
}
