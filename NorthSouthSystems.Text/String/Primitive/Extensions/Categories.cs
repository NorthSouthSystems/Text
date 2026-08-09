namespace NorthSouthSystems.Text;

public static partial class StringExtensions
{
    /// <inheritdoc cref="WhereIsInAnyCategory(IEnumerable{char}, CharCategories)"/>
    public static string WhereIsInAnyCategory(this string value, CharCategories categories) =>
        WhereIsInAnyCategory((IEnumerable<char>)value, categories).ToNewString();

    /// <summary>
    /// Removes unwanted characters from a sequence of characters.
    /// </summary>
    /// <param name="categories">Bitwise union of 1 or more CharCategories designating which characters to allow through the filter.</param>
    /// <returns>The original sequence of chars with unwanted chars omitted.</returns>
    /// <example>
    /// <code>
    /// Console.WriteLine("a1b2c3d".WhereIsInAnyCategory(CharCategories.All));
    /// Console.WriteLine("a1b2c3d".WhereIsInAnyCategory(CharCategories.Digit));
    /// Console.WriteLine("a1b2c3d".WhereIsInAnyCategory(CharCategories.Punctuation | CharCategories.WhiteSpace));
    /// Console.WriteLine("a1b2-c3d".WhereIsInAnyCategory(CharCategories.Digit | CharCategories.Letter));
    /// Console.WriteLine("a1b2-c3d".WhereIsInAnyCategory(CharCategories.Punctuation));
    /// </code>
    /// Console Output:<br/>
    /// a1b2c3d<br/>
    /// 123<br/>
    /// <br/>
    /// a1b2c3d<br/>
    /// -<br/>
    /// </example>
    public static IEnumerable<char> WhereIsInAnyCategory(this IEnumerable<char> chars, CharCategories categories)
    {
        ArgumentNullException.ThrowIfNull(chars);

        return categories == CharCategories.All ? chars : chars.Where(c => c.IsInAnyCategory(categories));
    }
}
