public class T_StringFixedExtensions_Join
{
    [Fact]
    public void Basic()
    {
        new[] { "A", "B", "C" }.JoinFixedRow(new[] { 1, 1, 1 })
            .Should().Be("ABC");

        new[] { "A", "B", "C" }.JoinFixedRow(new[] { 1, 1, 1 }, '-', false)
            .Should().Be("ABC");

        new[] { "A", "B", "C" }.JoinFixedRow(new[] { 2, 1, 1 })
            .Should().Be("A BC");

        new[] { "A", "B", "C" }.JoinFixedRow(new[] { 2, 1, 1 }, '-', false)
            .Should().Be("A-BC");

        new[] { "A", "B", "C" }.JoinFixedRow(new[] { 2, 2, 2 })
            .Should().Be("A B C ");

        new[] { "AB", "CD", "EF" }.JoinFixedRow(new[] { 2, 2, 2 })
            .Should().Be("ABCDEF");

        new[] { "AB", "CD", "EF" }.JoinFixedRow(new[] { 3, 2, 2 }, '-', false)
            .Should().Be("AB-CDEF");

        new[] { "AB", "CD", "EF" }.JoinFixedRow(new[] { 4, 4, 4 }, '-', false)
            .Should().Be("AB--CD--EF--");
    }

    [Fact]
    public void NullsAndEmptys()
    {
        new[] { null, "B", "C" }.JoinFixedRow(new[] { 1, 1, 1 })
            .Should().Be(" BC");

        new[] { null, "B", "C" }.JoinFixedRow(new[] { 1, 1, 1 }, '-')
            .Should().Be("-BC");

        new[] { "A", "B", string.Empty }.JoinFixedRow(new[] { 1, 1, 1 })
            .Should().Be("AB ");

        new[] { "A", "B", string.Empty }.JoinFixedRow(new[] { 1, 1, 1 }, '-')
            .Should().Be("AB-");
    }

    [Fact]
    public void LeftToFit()
    {
        new[] { "AB", "CD", "EF" }.JoinFixedRow(new[] { 1, 1, 1 }, '-', true)
            .Should().Be("ACE");

        new[] { "AB", "CD", "EF" }.JoinFixedRow(new[] { 3, 3, 1 }, '-', true)
            .Should().Be("AB-CD-E");
    }
}
