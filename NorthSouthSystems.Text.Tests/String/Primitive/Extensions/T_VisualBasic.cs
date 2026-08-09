using VBStrings = Microsoft.VisualBasic.Strings;

public class T_StringExtensions_VisualBasic
{
    [Theory]
    [InlineData("", 0)]
    [InlineData("a", 0)]
    [InlineData("a", 1)]
    [InlineData("a", 2)]
    [InlineData("ab", 0)]
    [InlineData("ab", 1)]
    [InlineData("ab", 2)]
    [InlineData("ab", 3)]
    [InlineData("abc", 0)]
    [InlineData("abc", 1)]
    [InlineData("abc", 2)]
    [InlineData("abc", 3)]
    [InlineData("abc", 4)]
    public void Left(string value, int length) =>
        StringExtensions.Left(value, length).Should().Be(VBStrings.Left(value, length));

    [Theory]
    [InlineData("", 0)]
    [InlineData("a", 0)]
    [InlineData("a", 1)]
    [InlineData("a", 2)]
    [InlineData("ab", 0)]
    [InlineData("ab", 1)]
    [InlineData("ab", 2)]
    [InlineData("ab", 3)]
    [InlineData("abc", 0)]
    [InlineData("abc", 1)]
    [InlineData("abc", 2)]
    [InlineData("abc", 3)]
    [InlineData("abc", 4)]
    public void Right(string value, int length) =>
        StringExtensions.Right(value, length).Should().Be(VBStrings.Right(value, length));

    [Fact]
    public void Exceptions()
    {
        Action act;

        act = () => ((string)null).Left(1);
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => "a".Left(-1);
        act.Should().ThrowExactly<ArgumentOutOfRangeException>();

        act = () => ((string)null).Right(1);
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => "a".Right(-1);
        act.Should().ThrowExactly<ArgumentOutOfRangeException>();
    }
}
