public class T_StringExtensions_CamelCase
{
    [Theory]
    [InlineData("DanTerry", "danTerry")]
    [InlineData(" DanTerry", " danTerry")]
    public void ToLower(string value, string shouldBe)
    {
        value.ToLowerCamelCase().Should().Be(shouldBe);
        shouldBe.ToLowerCamelCase().Should().Be(shouldBe);

        new string(value.AsEnumerable().ToLowerCamelCase().ToArray()).Should().Be(shouldBe);
        new string(shouldBe.AsEnumerable().ToLowerCamelCase().ToArray()).Should().Be(shouldBe);
    }

    [Theory]
    [InlineData("danTerry", "DanTerry")]
    [InlineData(" danTerry", " DanTerry")]
    public void ToUpper(string value, string shouldBe)
    {
        value.ToUpperCamelCase().Should().Be(shouldBe);
        shouldBe.ToUpperCamelCase().Should().Be(shouldBe);

        new string(value.AsEnumerable().ToUpperCamelCase().ToArray()).Should().Be(shouldBe);
        new string(shouldBe.AsEnumerable().ToUpperCamelCase().ToArray()).Should().Be(shouldBe);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("DanTerry", "Dan Terry")]
    [InlineData("DanTerry DanTerry", "Dan Terry Dan Terry")]
    [InlineData("DanTerryDan DanTerryDan", "Dan Terry Dan Dan Terry Dan")]
    [InlineData("1A", "1 A")]
    [InlineData("123A", "123 A")]
    [InlineData("123a", "123 a")]
    [InlineData("A1", "A 1")]
    [InlineData("A123", "A 123")]
    [InlineData("a123", "a 123")]
    [InlineData("A1A", "A 1 A")]
    [InlineData("A123A", "A 123 A")]
    [InlineData("a123a", "a 123 a")]
    public void Space(string value, string shouldBe)
    {
        value.SpaceCamelCase().Should().Be(shouldBe);
        shouldBe.SpaceCamelCase().Should().Be(shouldBe);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("DanTerry", "Dan-Terry")]
    [InlineData("DanTerry DanTerry", "Dan-Terry Dan-Terry")]
    [InlineData("DanTerryDan DanTerryDan", "Dan-Terry-Dan Dan-Terry-Dan")]
    [InlineData("1A", "1-A")]
    [InlineData("123A", "123-A")]
    [InlineData("123a", "123-a")]
    [InlineData("A1", "A-1")]
    [InlineData("A123", "A-123")]
    [InlineData("a123", "a-123")]
    [InlineData("A1A", "A-1-A")]
    [InlineData("A123A", "A-123-A")]
    [InlineData("a123a", "a-123-a")]
    public void Dash(string value, string shouldBe)
    {
        value.DelimitCamelCase("-").Should().Be(shouldBe);
        shouldBe.DelimitCamelCase("-").Should().Be(shouldBe);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("DanTerry", "Dan<>Terry")]
    [InlineData("DanTerry DanTerry", "Dan<>Terry Dan<>Terry")]
    [InlineData("DanTerryDan DanTerryDan", "Dan<>Terry<>Dan Dan<>Terry<>Dan")]
    [InlineData("1A", "1<>A")]
    [InlineData("123A", "123<>A")]
    [InlineData("123a", "123<>a")]
    [InlineData("A1", "A<>1")]
    [InlineData("A123", "A<>123")]
    [InlineData("a123", "a<>123")]
    [InlineData("A1A", "A<>1<>A")]
    [InlineData("A123A", "A<>123<>A")]
    [InlineData("a123a", "a<>123<>a")]
    public void Angles(string value, string shouldBe)
    {
        value.DelimitCamelCase("<>").Should().Be(shouldBe);
        shouldBe.DelimitCamelCase("<>").Should().Be(shouldBe);
    }

    [Fact]
    public void Exceptions()
    {
        Action act;

        act = () => ((string)null).ToLowerCamelCase();
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => ((string)null).ToUpperCamelCase();
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => ((string)null).SpaceCamelCase();
        act.Should().ThrowExactly<ArgumentNullException>();
    }
}
