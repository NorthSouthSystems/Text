public class T_StringSchemaExtensions_Split
{
    [Fact]
    public void Basic()
    {
        var schema = new StringSchema();
        schema.AddEntry(new("A", new[] { 1, 1, 1 }));
        schema.AddEntry(new("B", new[] { 2, 2, 2 }));
        schema.AddEntry(new("CD", new[] { 3, 3, 3 }));

        StringSchemaSplitResult split;

        split = "A123".SplitSchemaRow(schema);
        split.Entry.Header.Should().Be("A");
        split.Result.Fields.Select(field => field.Value).Should().Equal("1", "2", "3");

        split = "B123456".SplitSchemaRow(schema);
        split.Entry.Header.Should().Be("B");
        split.Result.Fields.Select(field => field.Value).Should().Equal("12", "34", "56");

        split = "CD123456789".SplitSchemaRow(schema);
        split.Entry.Header.Should().Be("CD");
        split.Result.Fields.Select(field => field.Value).Should().Equal("123", "456", "789");
    }

    [Fact]
    public void Exceptions()
    {
        Action act;

        act = () => ((string)null).SplitSchemaRow(new());
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => "Afoo".SplitSchemaRow(null);
        act.Should().ThrowExactly<ArgumentNullException>();
    }
}
