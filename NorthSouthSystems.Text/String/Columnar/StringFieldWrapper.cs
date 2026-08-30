using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml;

namespace NorthSouthSystems.Text;

/// <summary>
/// Wraps an individual field of a string row and allows for XElement style explicit type conversion
/// of the contained field.
/// </summary>
public readonly struct StringFieldWrapper : IEquatable<StringFieldWrapper>
{
    internal StringFieldWrapper(string columnName, string? value)
    {
        ColumnName = columnName;
        Value = value;
    }

    public string ColumnName { get; }
    public string? Value { get; }

    public override string ToString() => Value ?? string.Empty;

    #region Value Equality

    public override bool Equals(object? obj) =>
        obj is StringFieldWrapper wrapper && Equals(wrapper);

    public bool Equals(StringFieldWrapper other) =>
        ColumnName == other.ColumnName && Value == other.Value;

    public static bool operator ==(StringFieldWrapper left, StringFieldWrapper right) =>
        left.Equals(right);

    public static bool operator !=(StringFieldWrapper left, StringFieldWrapper right) =>
        !left.Equals(right);

    public override int GetHashCode() => HashCode.Combine(ColumnName, Value);

    // We protect from the "missing" compiler warning when comparing StringFieldWrapper to null. This
    // issue only occurs because operator ==(StringFieldWrapper, StringFieldWrapper) is overloaded which causes
    // the compiler to automatically generate the lifted operator ==(StringFieldWrapper?, StringFieldWrapper?).
    //
    // https://stackoverflow.com/questions/1972262/c-sharp-okay-with-comparing-value-types-to-null
    //
    // Overloading operator ==(StringFieldWrapper, object) would result in library consumer compiler errors
    // due to ambiguity. Those compiler errors would prevent the library consumer from using the valid
    // lifted operator ==(StringFieldWrapper?, StringFieldWrapper?). We have instead arbitrarily chosen to use
    // operator ==(StringFieldWrapper, string) to detect and prevent the comparison to null.

    [Obsolete("Comparing a struct to null always returns false.", true)]
    [ExcludeFromCodeCoverage]
    public static bool operator ==(StringFieldWrapper left, string right) =>
        left.Equals(right);

    [Obsolete("Comparing a struct to null always returns false.", true)]
    [ExcludeFromCodeCoverage]
    public static bool operator !=(StringFieldWrapper left, string right) =>
        !left.Equals(right);

    #endregion

    #region Operators

    // Operators mimic the behavior of .NET Framework's System.Linq.Xml.XElement's operators.
    // The rationale was to think of this as LINQ to Text.
    // https://github.com/microsoft/referencesource/blob/master/System.Xml.Linq/System/Xml/Linq/XLinq.cs

    private static T Required<T>(StringFieldWrapper field, Func<string, T> convert) =>
        field.Value == null
            ? throw new ArgumentNullException(nameof(field))
            : convert(field.Value);

    private static T? Optional<T>(StringFieldWrapper field, Func<string, T> convert)
        where T : struct =>
        string.IsNullOrWhiteSpace(field.Value)
            ? null
            : convert(field.Value!);

    public static explicit operator string?(StringFieldWrapper field) =>
        field.Value;

    // TODO : CA2225 fix?
#pragma warning disable CA2225 // Provide a method named 'To...' or 'From...' as an alternate for operator op_Explicit (https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2225)
#pragma warning disable CA1308 // XmlConvert.ToBoolean requires lowercase.
    public static explicit operator bool(StringFieldWrapper field) =>
        Required(field, static value => XmlConvert.ToBoolean(value.ToLower(CultureInfo.InvariantCulture)));

    public static explicit operator bool?(StringFieldWrapper field) =>
        Optional(field, static value => XmlConvert.ToBoolean(value.ToLower(CultureInfo.InvariantCulture)));
#pragma warning restore CA1308

    public static explicit operator int(StringFieldWrapper field) =>
        Required(field, XmlConvert.ToInt32);

    public static explicit operator int?(StringFieldWrapper field) =>
        Optional(field, XmlConvert.ToInt32);

    public static explicit operator uint(StringFieldWrapper field) =>
        Required(field, XmlConvert.ToUInt32);

    public static explicit operator uint?(StringFieldWrapper field) =>
        Optional(field, XmlConvert.ToUInt32);

    public static explicit operator long(StringFieldWrapper field) =>
        Required(field, XmlConvert.ToInt64);

    public static explicit operator long?(StringFieldWrapper field) =>
        Optional(field, XmlConvert.ToInt64);

    public static explicit operator ulong(StringFieldWrapper field) =>
        Required(field, XmlConvert.ToUInt64);

    public static explicit operator ulong?(StringFieldWrapper field) =>
        Optional(field, XmlConvert.ToUInt64);

    public static explicit operator float(StringFieldWrapper field) =>
        Required(field, XmlConvert.ToSingle);

    public static explicit operator float?(StringFieldWrapper field) =>
        Optional(field, XmlConvert.ToSingle);

    public static explicit operator double(StringFieldWrapper field) =>
        Required(field, XmlConvert.ToDouble);

    public static explicit operator double?(StringFieldWrapper field) =>
        Optional(field, XmlConvert.ToDouble);

    public static explicit operator decimal(StringFieldWrapper field) =>
        Required(field, XmlConvert.ToDecimal);

    public static explicit operator decimal?(StringFieldWrapper field) =>
        Optional(field, XmlConvert.ToDecimal);

    public static explicit operator DateTime(StringFieldWrapper field) =>
        Required(field, static value => DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind));

    public static explicit operator DateTime?(StringFieldWrapper field) =>
        Optional(field, static value => DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind));

    public static explicit operator DateTimeOffset(StringFieldWrapper field) =>
        Required(field, XmlConvert.ToDateTimeOffset);

    public static explicit operator DateTimeOffset?(StringFieldWrapper field) =>
        Optional(field, XmlConvert.ToDateTimeOffset);

    public static explicit operator TimeSpan(StringFieldWrapper field) =>
        Required(field, XmlConvert.ToTimeSpan);

    public static explicit operator TimeSpan?(StringFieldWrapper field) =>
        Optional(field, XmlConvert.ToTimeSpan);

    public static explicit operator Guid(StringFieldWrapper field) =>
        Required(field, XmlConvert.ToGuid);

    public static explicit operator Guid?(StringFieldWrapper field) =>
        Optional(field, XmlConvert.ToGuid);
#pragma warning restore

    #endregion
}
