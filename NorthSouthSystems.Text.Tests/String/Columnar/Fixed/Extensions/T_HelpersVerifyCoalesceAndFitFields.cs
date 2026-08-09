public class T_StringFixedExtensions_VerifyCoalesceAndFitFields
{
    [Fact]
    public void Basic()
    {
        string[] fields = new[] { "A", "BC", "DEF" };
        int[] columnWidths = new[] { 1, 2, 3 };

        string[] fieldsExpected = new string[fields.Length];
        Array.Copy(fields, fieldsExpected, fields.Length);

        StringFixedExtensions.VerifyCoalesceAndFitFields(fields, columnWidths, false);
        fields.Should().Equal(fieldsExpected);

        StringFixedExtensions.VerifyCoalesceAndFitFields(fields, columnWidths, true);
        fields.Should().Equal(fieldsExpected);
    }

    [Fact]
    public void Coalesce()
    {
        string[] fields;
        int[] columnWidths = new[] { 1, 1, 1 };

        StringFixedExtensions.VerifyCoalesceAndFitFields(fields = new[] { null, "B", "C" }, columnWidths, false);
        fields.Should().Equal(string.Empty, "B", "C");

        StringFixedExtensions.VerifyCoalesceAndFitFields(fields = new[] { "A", null, "C" }, columnWidths, false);
        fields.Should().Equal("A", string.Empty, "C");

        StringFixedExtensions.VerifyCoalesceAndFitFields(fields = new[] { "A", "B", null }, columnWidths, false);
        fields.Should().Equal("A", "B", string.Empty);
    }

    [Fact]
    public void LeftToFit()
    {
        string[] fields = new[] { "A", "BC", "DEF" };
        int[] columnWidths = new[] { 1, 2, 2 };

        StringFixedExtensions.VerifyCoalesceAndFitFields(fields, columnWidths, true);
        fields.Should().Equal("A", "BC", "DE");
    }

    [Fact]
    public void Exceptions()
    {
        Action act = null;

        act = () => StringFixedExtensions.VerifyCoalesceAndFitFields(null, new[] { 1 }, false);
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => StringFixedExtensions.VerifyCoalesceAndFitFields(new[] { "A", "B", "C" }, new[] { 1, 1 }, false);
        act.Should().ThrowExactly<ArgumentException>("FieldsAndColumnWidthsLengthMismatch");

        act = () => StringFixedExtensions.VerifyCoalesceAndFitFields(new[] { "A", "B" }, new[] { 1, 2, 3 }, false);
        act.Should().ThrowExactly<ArgumentException>("FieldsAndColumnWidthsLengthMismatch");

        act = () => StringFixedExtensions.VerifyCoalesceAndFitFields(new[] { "AB", "CD" }, new[] { 1, 2 }, false);
        act.Should().ThrowExactly<ArgumentOutOfRangeException>("FieldBiggerThanCorrespondingColumnWidth");

        act = () => StringFixedExtensions.VerifyCoalesceAndFitFields(new[] { "AB", "CD" }, new[] { 2, 1 }, false);
        act.Should().ThrowExactly<ArgumentOutOfRangeException>("FieldBiggerThanCorrespondingColumnWidth");
    }
}
