namespace NorthSouthSystems.Text;

public static partial class StringExtensions
{
    /// <summary>
    /// Overwrites characters in a string starting at a provided index.
    /// </summary>
    /// <param name="value">The string within which to overwrite characters.</param>
    /// <param name="startIndex">The index within <paramref name="value"/> at which to begin overwriting characters.
    /// Overwriting is also allowed when <paramref name="startIndex"/> equals the length of <paramref name="value"/>.</param>
    /// <param name="newValue">The string to be used for overwriting characters. Overwriting will continue
    /// past the length of <paramref name="value"/> when applicable.</param>
    /// <returns>The new string resulting from the overwrite.</returns>
    /// <example>
    /// <code>
    /// Console.WriteLine("abc".Overwrite(0, "z"));
    /// Console.WriteLine("abc".Overwrite(1, "z"));
    /// Console.WriteLine("abc".Overwrite(1, "zy"));
    /// Console.WriteLine("abc".Overwrite(2, "zy"));
    /// Console.WriteLine("abc".Overwrite(3, "zy"));
    /// </code>
    /// Console Output:<br/>
    /// zbc<br/>
    /// azc<br/>
    /// azy<br/>
    /// abzy<br/>
    /// abczy<br/>
    /// </example>
    public static string Overwrite(this string value, int startIndex, string newValue)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (startIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, "startIndex must be >= 0.");

        if (startIndex > value.Length)
            throw new ArgumentOutOfRangeException(nameof(startIndex), "startIndex must be <= value.Length.");

        ArgumentNullException.ThrowIfNull(newValue);

        return startIndex == value.Length
            ? value + newValue
            : value
                .Remove(startIndex, Math.Min(value.Length - startIndex, newValue.Length))
                .Insert(startIndex, newValue);
    }
}
