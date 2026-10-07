namespace NorthSouthSystems.Text;

public static partial class StringExtensions
{
    /// <inheritdoc cref="NormalizeWhiteSpace(IEnumerable{char}, string)"/>
    public static string NormalizeWhiteSpace(this string value) =>
        NormalizeWhiteSpace((IEnumerable<char>)value, Environment.NewLine).ToNewString();

    /// <inheritdoc cref="NormalizeWhiteSpace(IEnumerable{char}, string)"/>
    public static string NormalizeWhiteSpace(this string value, string respectNewLine) =>
        NormalizeWhiteSpace((IEnumerable<char>)value, respectNewLine).ToNewString();

    /// <inheritdoc cref="NormalizeWhiteSpace(IEnumerable{char}, string)"/>
    public static IEnumerable<char> NormalizeWhiteSpace(this IEnumerable<char> chars) =>
        NormalizeWhiteSpace(chars, Environment.NewLine);

    /// <summary>
    /// Trims the front and back sides of a sequence of chars while replacing any repeating instances of whitespace with a
    /// single instance of whitespace.  Newlines consisting of more than one whitespace character (Windows Newlines = \r\n) are
    /// treated as a single whitespace instance.
    /// </summary>
    /// <param name="respectNewLine">The Newline string to treat as a single whitespace instance. (default = Environment.NewLine)</param>
    /// <example>
    /// <code>
    /// Console.WriteLine(" A  B C   D   ".NormalizeWhiteSpace());
    /// Console.WriteLine(("Lots\tOf" + Environment.NewLine + "Changes").NormalizeWhiteSpace());
    /// </code>
    /// Console Output:<br/>
    /// A B C D<br/>
    /// Lots Of<br/>
    /// Changes<br/>
    /// </example>
    public static IEnumerable<char> NormalizeWhiteSpace(this IEnumerable<char> chars, string respectNewLine)
    {
        ArgumentNullException.ThrowIfNull(chars);

        if (!string.IsNullOrWhiteSpace(respectNewLine))
            throw new ArgumentException("Must only contain WhiteSpace chars.", nameof(respectNewLine));

        return NormalizeWhiteSpaceIterator(chars, respectNewLine);
    }

    private static IEnumerable<char> NormalizeWhiteSpaceIterator(IEnumerable<char> chars, string respectNewLine)
    {
        bool bufferedSpace = false;
        bool bufferedNewLine = false;

        respectNewLine = respectNewLine.NullToEmpty();
        var newLineTracker = StringSignalTracker.Create(respectNewLine);

        foreach (char c in chars.SkipWhile(char.IsWhiteSpace))
        {
            if (char.IsWhiteSpace(c))
            {
                bufferedSpace = true;

                if (newLineTracker.ProcessCharReturnsTriggeredLength(c) > 0)
                {
                    newLineTracker.Reset();

                    bufferedNewLine = true;
                }

                continue;
            }

            if (bufferedNewLine)
            {
                foreach (char newLineChar in respectNewLine)
                    yield return newLineChar;
            }
            else if (bufferedSpace)
                yield return ' ';

            bufferedSpace = false;
            bufferedNewLine = false;

            yield return c;
        }
    }
}
