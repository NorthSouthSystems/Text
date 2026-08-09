namespace NorthSouthSystems.Text;

[Flags]
public enum CharCategories
{
    None = 0,
    Control = 1 << 0,
    Digit = 1 << 1,
    HighSurrogate = 1 << 2,
    Letter = 1 << 3,
    Lower = 1 << 4,
    LowSurrogate = 1 << 5,
    Number = 1 << 6,
    Punctuation = 1 << 7,
    Separator = 1 << 8,
    Surrogate = 1 << 9,
    Symbol = 1 << 10,
    Upper = 1 << 11,
    WhiteSpace = 1 << 12,
    All = (1 << 13) - 1
}

public static partial class CharExtensions
{
    public static bool IsInAnyCategory(this char value, CharCategories categories) =>
        (categories.HasFlag(CharCategories.Control) && char.IsControl(value))
        || (categories.HasFlag(CharCategories.Digit) && char.IsDigit(value))
        || (categories.HasFlag(CharCategories.HighSurrogate) && char.IsHighSurrogate(value))
        || (categories.HasFlag(CharCategories.Letter) && char.IsLetter(value))
        || (categories.HasFlag(CharCategories.Lower) && char.IsLower(value))
        || (categories.HasFlag(CharCategories.LowSurrogate) && char.IsLowSurrogate(value))
        || (categories.HasFlag(CharCategories.Number) && char.IsNumber(value))
        || (categories.HasFlag(CharCategories.Punctuation) && char.IsPunctuation(value))
        || (categories.HasFlag(CharCategories.Separator) && char.IsSeparator(value))
        || (categories.HasFlag(CharCategories.Surrogate) && char.IsSurrogate(value))
        || (categories.HasFlag(CharCategories.Symbol) && char.IsSymbol(value))
        || (categories.HasFlag(CharCategories.Upper) && char.IsUpper(value))
        || (categories.HasFlag(CharCategories.WhiteSpace) && char.IsWhiteSpace(value));
}
