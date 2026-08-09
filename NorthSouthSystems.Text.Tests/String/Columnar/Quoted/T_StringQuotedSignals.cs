public class T_StringQuotedSignals
{
    [Fact]
    public void Basic()
    {
        var signals = new StringQuotedSignals([","], [Environment.NewLine], null, string.Empty);

        signals.DelimiterIsSpecified.Should().BeTrue();
        signals.Delimiter.Should().Be(",");

        signals.QuoteIsSpecified.Should().BeFalse();
        signals.Quote.Should().BeEmpty();

        signals.NewRowIsSpecified.Should().BeTrue();
        signals.NewRow.Should().Be(Environment.NewLine);

        signals.EscapeIsSpecified.Should().BeFalse();
        signals.Escape.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null, "\"", "\n", "\\")]
    [InlineData(null, "\"", "\r", "\\")]
    [InlineData(null, "\"", "\r\n", "\\")]
    [InlineData("", "\"", "\n", "\\")]
    [InlineData("", "\"", "\r", "\\")]
    [InlineData("", "\"", "\r\n", "\\")]
    public void Exceptions(string delimiter, string quote, string newRow, string escape)
    {
        Action act;

        act = () => new StringQuotedSignals([delimiter], [quote], newRow, escape);
        act.Should().ThrowExactly<ArgumentException>();

        act = () => new StringQuotedSignals([delimiter, null], [quote], newRow, escape);
        act.Should().ThrowExactly<ArgumentException>();

        act = () => new StringQuotedSignals([delimiter, string.Empty], [quote], newRow, escape);
        act.Should().ThrowExactly<ArgumentException>();
    }
}
