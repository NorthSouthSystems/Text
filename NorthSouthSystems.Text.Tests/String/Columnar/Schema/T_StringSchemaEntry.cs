public class T_StringSchemaEntry
{
    [Fact]
    public void Construction()
    {
        StringSchemaEntry entry;

        entry = new("A", new[] { 1 }, ' ', new[] { "Column0" });
        entry.Header.Should().Be("A");
        entry.Widths.Length.Should().Be(1);
        entry.Widths[0].Should().Be(1);
        entry.FillCharacter.Should().Be(' ');
        entry.RowWrapperFactory.ColumnNames.Length.Should().Be(1);
        entry.RowWrapperFactory.ColumnNames[0].Should().Be("Column0");

        entry = new("B", new[] { 2 }, '-', null);
        entry.Header.Should().Be("B");
        entry.Widths.Length.Should().Be(1);
        entry.Widths[0].Should().Be(2);
        entry.FillCharacter.Should().Be('-');
        entry.RowWrapperFactory.ColumnNames.Length.Should().Be(1);
        entry.RowWrapperFactory.ColumnNames[0].Should().Be("0");

        entry = new("C", new[] { 3, 4 }, '.', Array.Empty<string>());
        entry.Header.Should().Be("C");
        entry.Widths.Length.Should().Be(2);
        entry.Widths[0].Should().Be(3);
        entry.Widths[1].Should().Be(4);
        entry.FillCharacter.Should().Be('.');
        entry.RowWrapperFactory.ColumnNames.Length.Should().Be(2);
        entry.RowWrapperFactory.ColumnNames[0].Should().Be("0");
        entry.RowWrapperFactory.ColumnNames[1].Should().Be("1");
    }

    [Fact]
    public void Exceptions()
    {
        Action act;

        act = () => new StringSchemaEntry(null, new[] { 1 });
        act.Should().ThrowExactly<ArgumentException>("ConstructionNullHeader");

        act = () => new StringSchemaEntry(string.Empty, new[] { 1 });
        act.Should().ThrowExactly<ArgumentException>("ConstructionEmptyHeader");

        act = () => new StringSchemaEntry("A", new[] { 1, 1 }, ' ', new[] { "Column0" });
        act.Should().ThrowExactly<ArgumentException>("ConstructionColumnWidthsAndNamesLengthMismatch");
    }
}
