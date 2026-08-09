public class T_StringExtensions_NormalizeWhiteSpace
{
    [Theory]
    [InlineData("")]
    [InlineData("A")]
    [InlineData("No Changes")]
    [InlineData("No Changes At All")]
    public void NoChanges(string value) =>
        value.NormalizeWhiteSpace().Should().Be(value);

    [Theory]
    [InlineData(" ", "")]
    [InlineData(" A", "A")]
    [InlineData("A ", "A")]
    [InlineData(" A ", "A")]
    [InlineData("  A  ", "A")]
    [InlineData(" Changes ", "Changes")]
    [InlineData("Lots  Of   Changes", "Lots Of Changes")]
    [InlineData("a{Environment.NewLine}b", "a{Environment.NewLine}b")]
    [InlineData("a {Environment.NewLine}b", "a{Environment.NewLine}b")]
    [InlineData("a{Environment.NewLine} b", "a{Environment.NewLine}b")]
    [InlineData("a {Environment.NewLine} b", "a{Environment.NewLine}b")]
    [InlineData("Lots\tOf{Environment.NewLine}Changes", "Lots Of{Environment.NewLine}Changes")]
    [InlineData(" Lots \t Of {Environment.NewLine} Changes ", "Lots Of{Environment.NewLine}Changes")]
    public void ChangesNewLineRespect(string value, string shouldBe)
    {
        value = value.Replace("{Environment.NewLine}", Environment.NewLine);
        shouldBe = shouldBe.Replace("{Environment.NewLine}", Environment.NewLine);

        value.NormalizeWhiteSpace().Should().Be(shouldBe);
        new string(value.AsEnumerable().NormalizeWhiteSpace().ToArray()).Should().Be(shouldBe);
    }

    [Theory]
    [InlineData("\r", "")]
    [InlineData("\n", "")]
    [InlineData("\r\n", "")]
    [InlineData("{Environment.NewLine}", "")]
    [InlineData("a\rb", "a b")]
    [InlineData("a\r b", "a b")]
    [InlineData("a \rb", "a b")]
    [InlineData("a \r b", "a b")]
    [InlineData("a\nb", "a b")]
    [InlineData("a\n b", "a b")]
    [InlineData("a \nb", "a b")]
    [InlineData("a \n b", "a b")]
    [InlineData("a\r\nb", "a b")]
    [InlineData("a\r\n b", "a b")]
    [InlineData("a \r\nb", "a b")]
    [InlineData("a \r\n b", "a b")]
    [InlineData("Lots\tOf{Environment.NewLine}Changes", "Lots Of Changes")]
    [InlineData(" Lots \t Of {Environment.NewLine} Changes ", "Lots Of Changes")]
    public void ChangesNewLineNoRespect(string value, string shouldBe)
    {
        value = value.Replace("{Environment.NewLine}", Environment.NewLine);
        shouldBe = shouldBe.Replace("{Environment.NewLine}", Environment.NewLine);

        value.NormalizeWhiteSpace(null).Should().Be(shouldBe);
        new string(value.AsEnumerable().NormalizeWhiteSpace(null).ToArray()).Should().Be(shouldBe);
    }

    [Fact]
    public void Exceptions()
    {
        Action act;

        act = () => ((string)null).NormalizeWhiteSpace();
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => "A".NormalizeWhiteSpace("a");
        act.Should().ThrowExactly<ArgumentException>();
    }
}
