namespace NorthSouthSystems.Text;

public static partial class CharExtensions
{
    /// <summary>
    /// Creates and returns a new string containing all of the characters in the provided enumeration.
    /// </summary>
    public static string ToNewString(this IEnumerable<char> chars) => new(chars.ToArray());
}
