using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace NorthSouthSystems.Text;

/// <summary>
/// Wraps the fields in a row and then allows for index or column name based access to them.
/// </summary>
public readonly struct StringRowWrapper : IEquatable<StringRowWrapper>
{
    internal StringRowWrapper(StringRowWrapperFactory factory, string[] fields)
    {
        _factory = factory;
        _fields = fields;
    }

    private readonly StringRowWrapperFactory _factory;
    private readonly string[] _fields;

    #region Value Equality

    public override bool Equals(object? obj) =>
        obj is StringRowWrapper wrapper && Equals(wrapper);

    public bool Equals(StringRowWrapper other) =>
        ReferenceEquals(_factory, other._factory) && ReferenceEquals(_fields, other._fields);

    public static bool operator ==(StringRowWrapper left, StringRowWrapper right) =>
        left.Equals(right);

    public static bool operator !=(StringRowWrapper left, StringRowWrapper right) =>
        !left.Equals(right);

    public override int GetHashCode() =>
        (_factory?.GetHashCode() ?? 0) ^ (_fields?.GetHashCode() ?? 0);

    // We protect from the "missing" compiler warning when comparing StringRowWrapper to null. This
    // issue only occurs because operator ==(StringRowWrapper, StringRowWrapper) is overloaded which causes
    // the compiler to automatically generate the lifted operator ==(StringRowWrapper?, StringRowWrapper?).
    //
    // https://stackoverflow.com/questions/1972262/c-sharp-okay-with-comparing-value-types-to-null
    //
    // Overloading operator ==(StringRowWrapper, object) would result in library consumer compiler errors
    // due to ambiguity. Those compiler errors would prevent the library consumer from using the valid
    // lifted operator ==(StringRowWrapper?, StringRowWrapper?). We have instead arbitrarily chosen to use
    // operator ==(StringRowWrapper, string) to detect and prevent the comparison to null.

    [Obsolete("Comparing a struct to null always returns false.", true)]
    [ExcludeFromCodeCoverage]
    public static bool operator ==(StringRowWrapper left, string right) =>
        left.Equals(right);

    [Obsolete("Comparing a struct to null always returns false.", true)]
    [ExcludeFromCodeCoverage]
    public static bool operator !=(StringRowWrapper left, string right) =>
        !left.Equals(right);

    #endregion

    #region Fields

    public StringFieldWrapper this[int index]
    {
        get
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "index must be >= 0.");

            if (index >= _factory.ColumnNames.Length)
                throw new ArgumentOutOfRangeException(
                    nameof(index),
                    "index must be < the number of columns in the StringRowWrapperFactory.");

            return CreateField(index);
        }
    }

    public StringFieldWrapper this[string columnName]
    {
        get
        {
            if (!_factory.TryGetIndex(columnName, out int index))
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, "Column not found: {0}.", columnName),
                    nameof(columnName));

            return CreateField(index);
        }
    }

    public IEnumerable<StringFieldWrapper> Fields =>
        Enumerable.Range(0, _factory.ColumnNames.Length).Select(CreateField);

    private StringFieldWrapper CreateField(int index) =>
        new(
            _factory.ColumnNames[index],
            index < _fields.Length ? _fields[index] : null);

    #endregion
}