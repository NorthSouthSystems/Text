public class T_StringExtensions_PrefixSuffix
{
    [Theory]
    [InlineData("foobar", "foo", (StringComparison)default, "bar")]
    [InlineData("foobar", "bar", (StringComparison)default, "foobar")]
    [InlineData("foobar", "FOO", (StringComparison)default, "foobar")]
    [InlineData("foobar", "FOO", StringComparison.OrdinalIgnoreCase, "bar")]
    [InlineData("foo", "foo", (StringComparison)default, "")]
    public void RemovePrefix(string value, string prefix, StringComparison comparison, string shouldBe) =>
        value.RemovePrefix(prefix, comparison).Should().Be(shouldBe);

    [Theory]
    [InlineData("foobar", "foo", (StringComparison)default, "foobar")]
    [InlineData("foobar", "bar", (StringComparison)default, "foo")]
    [InlineData("foobar", "BAR", (StringComparison)default, "foobar")]
    [InlineData("foobar", "BAR", StringComparison.OrdinalIgnoreCase, "foo")]
    [InlineData("bar", "bar", (StringComparison)default, "")]
    public void RemoveSuffix(string value, string suffix, StringComparison comparison, string shouldBe) =>
        value.RemoveSuffix(suffix, comparison).Should().Be(shouldBe);

    [Theory]
    [InlineData("foobar", "foo", "gee", (StringComparison)default, "geebar")]
    [InlineData("foobar", "bar", "gee", (StringComparison)default, "foobar")]
    [InlineData("foobar", "FOO", "gee", (StringComparison)default, "foobar")]
    [InlineData("foobar", "FOO", "gee", StringComparison.OrdinalIgnoreCase, "geebar")]
    [InlineData("foobar", "foo", "", (StringComparison)default, "bar")]
    [InlineData("foobar", "foo", null, (StringComparison)default, "bar")]
    [InlineData("foo", "foo", "", (StringComparison)default, "")]
    [InlineData("foo", "foo", null, (StringComparison)default, "")]
    public void ReplacePrefix(
        string value,
        string prefix,
        string replacement,
        StringComparison comparison,
        string shouldBe) =>
        value.ReplacePrefix(prefix, replacement, comparison).Should().Be(shouldBe);

    [Theory]
    [InlineData("foobar", "foo", "gee", (StringComparison)default, "foobar")]
    [InlineData("foobar", "bar", "gee", (StringComparison)default, "foogee")]
    [InlineData("foobar", "BAR", "gee", (StringComparison)default, "foobar")]
    [InlineData("foobar", "BAR", "gee", StringComparison.OrdinalIgnoreCase, "foogee")]
    [InlineData("foobar", "bar", "", (StringComparison)default, "foo")]
    [InlineData("foobar", "bar", null, (StringComparison)default, "foo")]
    [InlineData("bar", "bar", "", (StringComparison)default, "")]
    [InlineData("bar", "bar", null, (StringComparison)default, "")]
    public void ReplaceSuffix(
        string value,
        string suffix,
        string replacement,
        StringComparison comparison,
        string shouldBe) =>
        value.ReplaceSuffix(suffix, replacement, comparison).Should().Be(shouldBe);

    [Fact]
    public void Exceptions()
    {
        Action act;

        // RemovePrefix

        act = () => ((string)null).RemovePrefix("foo");
        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("value");

        act = () => "foobar".RemovePrefix(null);
        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("prefix");

        act = () => "foobar".RemovePrefix(string.Empty);
        act.Should().ThrowExactly<ArgumentException>().WithParameterName("prefix");

        // RemoveSuffix

        act = () => ((string)null).RemoveSuffix("foo");
        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("value");

        act = () => "foobar".RemoveSuffix(null);
        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("suffix");

        act = () => "foobar".RemoveSuffix(string.Empty);
        act.Should().ThrowExactly<ArgumentException>().WithParameterName("suffix");

        // ReplacePrefix

        act = () => ((string)null).ReplacePrefix("foo", "gee");
        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("value");

        act = () => "foobar".ReplacePrefix(null, "gee");
        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("prefix");

        act = () => "foobar".ReplacePrefix(string.Empty, "gee");
        act.Should().ThrowExactly<ArgumentException>().WithParameterName("prefix");

        // ReplaceSuffix

        act = () => ((string)null).ReplaceSuffix("bar", "gee");
        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("value");

        act = () => "foobar".ReplaceSuffix(null, "gee");
        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("suffix");

        act = () => "foobar".ReplaceSuffix(string.Empty, "gee");
        act.Should().ThrowExactly<ArgumentException>().WithParameterName("suffix");
    }
}
