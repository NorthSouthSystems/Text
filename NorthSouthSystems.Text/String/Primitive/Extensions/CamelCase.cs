using System.Globalization;

namespace NorthSouthSystems.Text;

public static partial class StringExtensions
{
    /// <inheritdoc cref="ToLowerCamelCase(IEnumerable{char})"/>
    public static string ToLowerCamelCase(this string value) => ToCamelCase(value, true).ToNewString();

    /// <summary>
    /// Process a sequence of characters and returns its lower camel case representation.
    /// </summary>
    /// <example>
    /// <code>
    /// Console.WriteLine("FooBar".ToLowerCamelCase());
    /// Console.WriteLine(" FooBar".ToLowerCamelCase());
    /// </code>
    /// Console Output:<br/>
    /// fooBar<br/>
    ///  fooBar<br/>
    /// </example>
    public static IEnumerable<char> ToLowerCamelCase(this IEnumerable<char> chars) => ToCamelCase(chars, true);

    /// <inheritdoc cref="ToUpperCamelCase(IEnumerable{char})"/>
    public static string ToUpperCamelCase(this string value) => ToCamelCase(value, false).ToNewString();

    /// <summary>
    /// Process a sequence of characters and returns its upper camel case representation.
    /// </summary>
    /// <example>
    /// <code>
    /// Console.WriteLine("fooBar".ToUpperCamelCase());
    /// Console.WriteLine(" fooBar".ToUpperCamelCase());
    /// </code>
    /// Console Output:<br/>
    /// FooBar<br/>
    ///  FooBar<br/>
    /// </example>
    public static IEnumerable<char> ToUpperCamelCase(this IEnumerable<char> chars) => ToCamelCase(chars, false);

    private static IEnumerable<char> ToCamelCase(IEnumerable<char> chars, bool isLower)
    {
        ArgumentNullException.ThrowIfNull(chars);

        return ToCamelCaseIterator(chars, isLower);
    }

    private static IEnumerable<char> ToCamelCaseIterator(IEnumerable<char> chars, bool isLower)
    {
        bool previousWhiteSpace = true;
        int outputIndex = 0;

        foreach (char c in chars)
        {
            if (!char.IsWhiteSpace(c))
            {
                if (isLower && outputIndex == 0)
                    yield return char.ToLower(c, CultureInfo.CurrentCulture);
                else if (previousWhiteSpace)
                    yield return char.ToUpper(c, CultureInfo.CurrentCulture);
                else
                    yield return c;

                previousWhiteSpace = false;
                outputIndex++;
            }
            else
            {
                yield return c;

                previousWhiteSpace = true;
            }
        }
    }

    /// <inheritdoc cref="SpaceCamelCase(IEnumerable{char})"/>
    public static string SpaceCamelCase(this string value) => SpaceCamelCase((IEnumerable<char>)value).ToNewString();

    /// <summary>
    /// Process a sequence of characters and returns the same sequence of characters but with spaces inserted
    /// whenever camel casing indicates the start of a new word.
    /// </summary>
    /// <example>
    /// <code>
    /// Console.WriteLine("FooBarFoo FooBarFoo".SpaceCamelCase());
    /// Console.WriteLine("123A".SpaceCamelCase());
    /// Console.WriteLine("123a".SpaceCamelCase());
    /// Console.WriteLine("A123".SpaceCamelCase());
    /// Console.WriteLine("A123A".SpaceCamelCase());
    /// </code>
    /// Console Output:<br/>
    /// Foo Bar Foo Foo Bar Foo<br/>
    /// 123 A<br/>
    /// 123 a<br/>
    /// A 123<br/>
    /// A 123 A<br/>
    /// </example>
    public static IEnumerable<char> SpaceCamelCase(this IEnumerable<char> chars)
    {
        ArgumentNullException.ThrowIfNull(chars);

        return DelimitCamelCaseIterator(chars, " ");
    }

    /// <inheritdoc cref="DelimitCamelCase(IEnumerable{char}, string)"/>
    public static string DelimitCamelCase(this string value, string delimiter) => DelimitCamelCase((IEnumerable<char>)value, delimiter).ToNewString();

    /// <summary>
    /// Process a sequence of characters and returns the same sequence of characters but with delimiter inserted
    /// whenever camel casing indicates the start of a new word.
    /// </summary>
    /// <example>
    /// <code>
    /// Console.WriteLine("FooBarFoo FooBarFoo".DelimitCamelCase("-"));
    /// Console.WriteLine("123A".DelimitCamelCase("-"));
    /// Console.WriteLine("123a".DelimitCamelCase("-"));
    /// Console.WriteLine("A123".DelimitCamelCase("-"));
    /// Console.WriteLine("A123A".DelimitCamelCase("-"));
    /// </code>
    /// Console Output:<br/>
    /// Foo-Bar-Foo Foo-Bar-Foo<br/>
    /// 123-A<br/>
    /// 123-a<br/>
    /// A-123<br/>
    /// A-123-A<br/>
    /// </example>
    public static IEnumerable<char> DelimitCamelCase(this IEnumerable<char> chars, string delimiter)
    {
        ArgumentNullException.ThrowIfNull(chars);
        ArgumentNullException.ThrowIfNull(delimiter);

        return DelimitCamelCaseIterator(chars, delimiter);
    }

    private static IEnumerable<char> DelimitCamelCaseIterator(IEnumerable<char> chars, string delimiter)
    {
        bool first = true;

        bool previousIsDigit = false;
        bool previousIsLetter = false;
        bool previousIsLower = false;

        foreach (char c in chars)
        {
            bool isDigit = char.IsDigit(c);
            bool isLetter = char.IsLetter(c);
            bool isUpper = char.IsUpper(c);
            bool isLower = char.IsLower(c);

            if (first)
            {
                yield return c;

                first = false;
            }
            else
            {
                if ((isLetter && previousIsDigit)
                    || (isDigit && previousIsLetter)
                    || (isUpper && previousIsLower))
                {
                    foreach (char d in delimiter)
                        yield return d;
                }

                yield return c;
            }

            previousIsDigit = isDigit;
            previousIsLetter = isLetter;
            previousIsLower = isLower;
        }
    }
}
