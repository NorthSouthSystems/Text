public class T_StringFixedExtensions_SplitRow
{
    [Fact]
    public void Basic()
    {
        "ABC".SplitFixedRow(new[] { 1, 1, 1 })
            .Should().Equal("A", "B", "C");

        "ABC".SplitFixedRow(new[] { 1, 2 })
            .Should().Equal("A", "BC");

        "ABCD".SplitFixedRow(new[] { 1, 2, 1 })
            .Should().Equal("A", "BC", "D");

        "ABCDEF".SplitFixedRow(new[] { 2, 2, 2 })
            .Should().Equal("AB", "CD", "EF");

        "A B C ".SplitFixedRow(new[] { 2, 2, 2 })
            .Should().Equal("A", "B", "C");

        "A-B-C-".SplitFixedRow(new[] { 2, 2, 2 })
            .Should().Equal("A-", "B-", "C-");

        "A-B-C-".SplitFixedRow(new[] { 2, 2, 2 }, '-')
            .Should().Equal("A", "B", "C");

        "A B   ".SplitFixedRow(new[] { 2, 2, 2 })
            .Should().Equal("A", "B", "");

        "A-B---".SplitFixedRow(new[] { 2, 2, 2 })
            .Should().Equal("A-", "B-", "--");

        "A-B---".SplitFixedRow(new[] { 2, 2, 2 }, '-')
            .Should().Equal("A", "B", "");
    }

    [Fact]
    public void Exceptions()
    {
        Action act;

        act = () => ((string)null).SplitFixedRow(new[] { 1 });
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => string.Empty.SplitFixedRow(new[] { 1 });
        act.Should().ThrowExactly<ArgumentException>();

        act = () => "1".SplitFixedRow(new[] { 2 });
        act.Should().ThrowExactly<ArgumentOutOfRangeException>();

        act = () => "12".SplitFixedRow(new[] { 3 });
        act.Should().ThrowExactly<ArgumentOutOfRangeException>();

        act = () => "12".SplitFixedRow(new[] { 1, 2 });
        act.Should().ThrowExactly<ArgumentOutOfRangeException>();

        act = () => "1234".SplitFixedRow(new[] { 1, 2 });
        act.Should().ThrowExactly<ArgumentOutOfRangeException>();
    }
}
