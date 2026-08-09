public class T_StringSchemaExtensions_Join
{
    [Fact]
    public void Basic()
    {
        var a = new StringSchemaEntry("A", new[] { 1, 1, 1 });
        var b = new StringSchemaEntry("B", new[] { 2, 2, 2 });
        var c = new StringSchemaEntry("C", new[] { 2, 2, 2 }, '-');

        new[] { "1", "2", "3" }.JoinSchemaRow(a)
            .Should().Be("A123");

        new[] { "12", "34", "56" }.JoinSchemaRow(b)
            .Should().Be("B123456");

        new[] { "1", "2", "3" }.JoinSchemaRow(b)
            .Should().Be("B1 2 3 ");

        new[] { "1", "2", "3" }.JoinSchemaRow(c)
            .Should().Be("C1-2-3-");
    }

    [Fact]
    public void NullsAndEmptys()
    {
        var a = new StringSchemaEntry("A", new[] { 1, 1, 1 });

        new[] { null, "2", "3" }.JoinSchemaRow(a)
            .Should().Be("A 23");

        new[] { "1", "2", string.Empty }.JoinSchemaRow(a)
            .Should().Be("A12 ");
    }

    [Fact]
    public void Exceptions()
    {
        Action act;

        act = () => ((string[])null).JoinSchemaRow(new("A", new[] { 1, 1, 1 }));
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => new[] { "1" }.JoinSchemaRow(null);
        act.Should().ThrowExactly<ArgumentNullException>();
    }
}
