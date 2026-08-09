public class T_CharExtensions_Ascii
{
    [Theory]
    [InlineData('a')]
    [InlineData('1')]
    [InlineData('.')]
    [InlineData(' ')]
    [InlineData('*')]
    [InlineData('@')]
    [InlineData('\t')]
    [InlineData('\r')]
    [InlineData('\n')]
    public void IsAsciiTrue(char c) =>
        c.IsAscii().Should().BeTrue();

    [Theory]
    [InlineData('\u2714')]
    public void IsAsciiFalse(char c) =>
        c.IsAscii().Should().BeFalse();

    [Theory]
    [InlineData('a')]
    [InlineData('1')]
    [InlineData('.')]
    [InlineData(' ')]
    [InlineData('*')]
    [InlineData('@')]
    public void IsAsciiPrintableTrue(char c) =>
        c.IsAsciiPrintable().Should().BeTrue();

    [Theory]
    [InlineData('\t')]
    [InlineData('\r')]
    [InlineData('\n')]
    [InlineData('\u2714')]
    public void IsAsciiPrintableFalse(char c) =>
        c.IsAsciiPrintable().Should().BeFalse();
}
