public class T_StringExtensions_Coalesce
{
    [Fact]
    public void EmptyToNull()
    {
        foreach (string s in new[] { null, string.Empty })
            s.EmptyToNull().Should().BeNull();

        foreach (string s in new[] { " ", "a", "A", "1", "abc", "ABC", "123" })
            s.EmptyToNull().Should().Be(s);
    }

    [Fact]
    public void NullToEmpty()
    {
        foreach (string s in new[] { null, string.Empty })
            s.NullToEmpty().Should().BeEmpty();

        foreach (string s in new[] { " ", "a", "A", "1", "abc", "ABC", "123" })
            s.NullToEmpty().Should().Be(s);
    }

    [Fact]
    public void WhiteSpaceToNull()
    {
        foreach (string s in new[] { null, string.Empty, " ", "  ", "\t", "\n", "\r\n", "\t \r\n " })
            s.WhiteSpaceToNull().Should().BeNull();

        foreach (string s in new[] { "a", "A", "1", "abc", "ABC", "123" })
            s.WhiteSpaceToNull().Should().Be(s);
    }

    [Fact]
    public void ToStringNullToEmpty()
    {
        ((object)null).ToStringNullToEmpty().Should().BeEmpty();
        1.ToStringNullToEmpty().Should().Be("1");
    }
}
