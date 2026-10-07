public class T_TakeColumnHeadersExtensions
{
    [Fact]
    public void Basic()
    {
        List<StringRowWrapper> result;
        string nr = StringQuotedSignals.CsvNewRowTolerantWindowsPrimaryRFC4180.NewRow;

        result = Result($"A{nr}1", new[] { "A" }, false);
        result.Count.Should().Be(1);
        result[0]["A"].Value.Should().Be("1");

        result = Result($"A{nr}1", new[] { "A" }, true);
        result.Count.Should().Be(1);
        result[0]["A"].Value.Should().Be("1");

        result = Result($"A,B{nr}1,2", new[] { "B", "A" }, false);
        result.Count.Should().Be(1);
        result[0]["A"].Value.Should().Be("1");
        result[0]["B"].Value.Should().Be("2");

        result = Result($"A,B{nr}1,2", new[] { "A", "B" }, true);
        result.Count.Should().Be(1);
        result[0]["A"].Value.Should().Be("1");
        result[0]["B"].Value.Should().Be("2");

        result = Result($"A,B,C{nr}1,2,3{nr}4,5,6", new[] { "A", "B", "C" }, true);
        result.Count.Should().Be(2);
        result[0]["A"].Value.Should().Be("1");
        result[0]["B"].Value.Should().Be("2");
        result[0]["C"].Value.Should().Be("3");
        result[1]["A"].Value.Should().Be("4");
        result[1]["B"].Value.Should().Be("5");
        result[1]["C"].Value.Should().Be("6");
    }

    [Fact]
    public void Exceptions()
    {
        Action act;
        string nr = StringQuotedSignals.CsvNewRowTolerantWindowsPrimaryRFC4180.NewRow;

        act = () => ((string[][])null).TakeColumnHeaders(new[] { "A" }).ToList();
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => Result($"A{nr}1", new[] { "A", "B" }, false);
        act.Should().ThrowExactly<ArgumentException>().WithMessage("Expected: B");

        act = () => Result($"A,B{nr}1,2", new[] { "A" }, false);
        act.Should().ThrowExactly<ArgumentException>().WithMessage("Unexpected: B");

        act = () => Result($"A,B{nr}1,2", new[] { "B", "A" }, true);
        act.Should().ThrowExactly<ArgumentException>().WithMessage("*expected order*");
    }

    private static List<StringRowWrapper> Result(
        string csv,
        IEnumerable<string> expectedColumnNames,
        bool enforceExpectedColumnNamesOrder) =>
        csv.SplitQuotedRows(StringQuotedSignals.CsvNewRowTolerantWindowsPrimaryRFC4180)
            .TakeColumnHeaders(expectedColumnNames, enforceExpectedColumnNamesOrder)
            .ToList();
}
