public class T_StringSchema
{
    [Fact]
    public void Basic()
    {
        var schema = new StringSchema();
        schema.AddEntry(new("A", new[] { 3 }));
        schema.AddEntry(new("B", new[] { 6 }));
        schema.AddEntry(new("C", new[] { 3, 3 }));
        schema.AddEntry(new("DE", new[] { 4, 4 }));

        StringSchemaEntry entry;

        entry = schema.GetEntryForRow("Afoo");
        entry.Header.Should().Be("A");
        entry.Widths.Should().Equal(3);

        entry = schema["A"];
        entry.Header.Should().Be("A");
        entry.Widths.Should().Equal(3);

        entry = schema.GetEntryForRow("Bfoobar");
        entry.Header.Should().Be("B");
        entry.Widths.Should().Equal(6);

        entry = schema["B"];
        entry.Header.Should().Be("B");
        entry.Widths.Should().Equal(6);

        entry = schema.GetEntryForRow("Cfoobar");
        entry.Header.Should().Be("C");
        entry.Widths.Should().Equal(3, 3);

        entry = schema["C"];
        entry.Header.Should().Be("C");
        entry.Widths.Should().Equal(3, 3);

        entry = schema.GetEntryForRow("DEfoosbars");
        entry.Header.Should().Be("DE");
        entry.Widths.Should().Equal(4, 4);

        entry = schema["DE"];
        entry.Header.Should().Be("DE");
        entry.Widths.Should().Equal(4, 4);
    }

    [Fact]
    public void Exceptions()
    {
        Action act;

        act = () => new StringSchema().AddEntry(null);
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () =>
        {
            var schema = new StringSchema();
            var entry = new StringSchemaEntry("A", new[] { 1 });
            schema.AddEntry(entry);
            schema.GetEntryForRow("Bfoo");
        };
        act.Should().ThrowExactly<ArgumentOutOfRangeException>("GetEntryForValueNoEntry");

        act = () =>
        {
            var schema = new StringSchema();
            var entry = new StringSchemaEntry("AB", new[] { 1 });
            schema.AddEntry(entry);
            schema.GetEntryForRow("Afoo");
        };
        act.Should().ThrowExactly<ArgumentOutOfRangeException>("GetEntryForValueNoEntry");

        act = () =>
        {
            var schema = new StringSchema();
            var entry = new StringSchemaEntry("A", new[] { 1 });
            schema.AddEntry(entry);
            entry = new("AB", new[] { 1 });
            schema.AddEntry(entry);
        };
        act.Should().ThrowExactly<ArgumentOutOfRangeException>("VerifyEntryOverlappedHeader");

        act = () =>
        {
            var schema = new StringSchema();
            var entry = new StringSchemaEntry("AB", new[] { 1 });
            schema.AddEntry(entry);
            entry = new("A", new[] { 1 });
            schema.AddEntry(entry);
        };
        act.Should().ThrowExactly<ArgumentOutOfRangeException>("VerifyEntryOverlappedHeader");
    }
}
