public class T_StringQuotedSignalsBuilder
{
    [Fact]
    public void NewRowTolerant()
    {
        StringQuotedSignals signals;

        signals = NewBuilder().NewRowTolerantEnvironmentPrimary().ToSignals();
        signals.NewRow.Should().Be(Environment.NewLine);
        signals.NewRows.First().Should().Be(Environment.NewLine);

        signals = NewBuilder().NewRowTolerantCarriageReturnPrimary().ToSignals();
        signals.NewRow.Should().Be("\r");
        signals.NewRows.First().Should().Be("\r");
        signals.NewRows.Skip(1).Should().BeEquivalentTo(["\n", "\r\n"]);

        signals = NewBuilder().NewRowTolerantLinuxPrimary().ToSignals();
        signals.NewRow.Should().Be("\n");
        signals.NewRows.First().Should().Be("\n");
        signals.NewRows.Skip(1).Should().BeEquivalentTo(["\r", "\r\n"]);

        signals = NewBuilder().NewRowTolerantWindowsPrimary().ToSignals();
        signals.NewRow.Should().Be("\r\n");
        signals.NewRows.First().Should().Be("\r\n");
        signals.NewRows.Skip(1).Should().BeEquivalentTo(["\n", "\r"]);

        StringQuotedSignalsBuilder NewBuilder() =>
            new StringQuotedSignalsBuilder()
                .Delimiter(",")
                .Quote("\"");
    }

    [Theory]
    [InlineData(null, "\n", "\"", "\\")]
    [InlineData(null, "\r", "\"", "\\")]
    [InlineData(null, "\r\n", "\"", "\\")]
    [InlineData("", "\n", "\"", "\\")]
    [InlineData("", "\r", "\"", "\\")]
    [InlineData("", "\r\n", "\"", "\\")]
    public void ExceptionsDelimiterNotSpecified(string delimiter, string newRow, string quote, string escape)
    {
        Action act;

        act = () => new StringQuotedSignalsBuilder()
            .Delimiter(delimiter)
            .NewRow(newRow)
            .Quote(quote)
            .Escape(escape)
            .ToSignals();

        act.Should().ThrowExactly<ArgumentException>();
    }

    [Theory]
    [InlineData("A", "AB", null, null)]
    [InlineData("A", null, "AB", null)]
    [InlineData("A", null, null, "AB")]
    [InlineData("AB", "A", null, null)]
    [InlineData("AB", null, "A", null)]
    [InlineData("AB", null, null, "A")]
    [InlineData(",", "A", "AB", null)]
    [InlineData(",", "A", null, "AB")]
    [InlineData(",", "AB", "A", null)]
    [InlineData(",", "AB", null, "A")]
    [InlineData(",", null, "A", "AB")]
    [InlineData(",", null, "AB", "A")]
    public void ExceptionsAnySignalContainsAnyOther(string delimiter, string newRow, string quote, string escape)
    {
        Action act;

        act = () => new StringQuotedSignalsBuilder()
            .Delimiter(delimiter)
            .NewRow(newRow)
            .Quote(quote)
            .Escape(escape)
            .ToSignals();

        act.Should().ThrowExactly<ArgumentException>();
    }

    [Theory]
    [InlineData("A", "AB")]
    [InlineData("A", "ABC")]
    [InlineData("B", "ABC")]
    [InlineData("AB", "ABC")]
    [InlineData("D", "AB", "A")]
    [InlineData("D", "ABC", "AB")]
    [InlineData("D", "ABC", "B")]
    public void ExceptionsMultiSignalHidesAnyOther(string primary, params string[] alternates)
    {
        Action act;

        act = () => new StringQuotedSignalsBuilder().Delimiter(primary, alternates);
        act.Should().ThrowExactly<ArgumentException>();

        act = () => new StringQuotedSignalsBuilder().NewRow(primary, alternates);
        act.Should().ThrowExactly<ArgumentException>();
    }
}
