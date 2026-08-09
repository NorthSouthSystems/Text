public class T_StringFixedExtensions_SplitRepeating
{
    [Fact]
    public void Basic()
    {
        string[][] rowsFields = "123".SplitFixedRepeating(new[] { 1, 1, 1 }).ToArray();
        rowsFields.Length.Should().Be(1);
        rowsFields[0].Should().Equal("1", "2", "3");

        rowsFields = "123456".SplitFixedRepeating(new[] { 1, 1, 1 }).ToArray();
        rowsFields.Length.Should().Be(2);
        rowsFields[0].Should().Equal("1", "2", "3");
        rowsFields[1].Should().Equal("4", "5", "6");

        rowsFields = "123456789".SplitFixedRepeating(new[] { 1, 1, 1 }).ToArray();
        rowsFields.Length.Should().Be(3);
        rowsFields[0].Should().Equal("1", "2", "3");
        rowsFields[1].Should().Equal("4", "5", "6");
        rowsFields[2].Should().Equal("7", "8", "9");
    }

    [Fact]
    public void FillTrim()
    {
        string[][] rowsFields = "1 34 67 9".SplitFixedRepeating(new[] { 1, 1, 1 }).ToArray();
        rowsFields.Length.Should().Be(3);
        rowsFields[0].Should().Equal("1", "", "3");
        rowsFields[1].Should().Equal("4", "", "6");
        rowsFields[2].Should().Equal("7", "", "9");

        rowsFields = "1-34-67-9".SplitFixedRepeating(new[] { 1, 1, 1 }).ToArray();
        rowsFields.Length.Should().Be(3);
        rowsFields[0].Should().Equal("1", "-", "3");
        rowsFields[1].Should().Equal("4", "-", "6");
        rowsFields[2].Should().Equal("7", "-", "9");

        rowsFields = "1-34-67-9".SplitFixedRepeating(new[] { 1, 1, 1 }, '-').ToArray();
        rowsFields.Length.Should().Be(3);
        rowsFields[0].Should().Equal("1", "", "3");
        rowsFields[1].Should().Equal("4", "", "6");
        rowsFields[2].Should().Equal("7", "", "9");
    }

    [Fact]
    public void Exceptions()
    {
        Action act = null;

        act = () => ((string)null).SplitFixedRepeating(new[] { 1 }).ToArray();
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => "1".SplitFixedRepeating(new[] { 2 }).ToArray();
        act.Should().ThrowExactly<ArgumentOutOfRangeException>("LengthColumnWidthSumMismatch");

        act = () => "12".SplitFixedRepeating(new[] { 3 }).ToArray();
        act.Should().ThrowExactly<ArgumentOutOfRangeException>("LengthColumnWidthSumMismatch");

        act = () => "12".SplitFixedRepeating(new[] { 1, 2 }).ToArray();
        act.Should().ThrowExactly<ArgumentOutOfRangeException>("LengthColumnWidthSumMismatch");

        act = () => "123".SplitFixedRepeating(new[] { 2 }).ToArray();
        act.Should().ThrowExactly<ArgumentOutOfRangeException>("LengthColumnWidthSumMismatch");

        act = () => "123".SplitFixedRepeating(new[] { 1, 1 }).ToArray();
        act.Should().ThrowExactly<ArgumentOutOfRangeException>("LengthColumnWidthSumMismatch");

        act = () => "12345".SplitFixedRepeating(new[] { 1, 2 }).ToArray();
        act.Should().ThrowExactly<ArgumentOutOfRangeException>("LengthColumnWidthSumMismatch");

        act = () => "1234567".SplitFixedRepeating(new[] { 1, 2 }).ToArray();
        act.Should().ThrowExactly<ArgumentOutOfRangeException>("LengthColumnWidthSumMismatch");
    }
}
