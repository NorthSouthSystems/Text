public class T_StringExtensions_AnyPermutationPair
{
    [Theory]
    [InlineData(false, "")]
    [InlineData(false, "a")]
    [InlineData(false, "a", "b")]
    [InlineData(false, "a", "b", "c")]
    [InlineData(false, "A", "ab")]
    [InlineData(false, "ab", "A")]
    [InlineData(false, "B", "ba")]
    [InlineData(false, "ba", "B")]
    [InlineData(false, "A", "c", "ab")]
    [InlineData(false, "A", "ab", "c")]
    [InlineData(false, "ab", "A", "c")]
    [InlineData(true, "", "a")]
    [InlineData(true, "a", "")]
    [InlineData(true, "a", "a")]
    [InlineData(true, "ab", "ab")]
    [InlineData(true, "a", "ab")]
    [InlineData(true, "ab", "a")]
    [InlineData(true, "b", "ab")]
    [InlineData(true, "ab", "b")]
    [InlineData(true, "a", "ba")]
    [InlineData(true, "ba", "a")]
    [InlineData(true, "b", "ba")]
    [InlineData(true, "ba", "b")]
    [InlineData(true, "abc", "b")]
    [InlineData(true, "b", "abc")]
    [InlineData(true, "a", "c", "ab")]
    [InlineData(true, "a", "ab", "c")]
    [InlineData(true, "ab", "a", "c")]
    [InlineData(true, "b", "d", "abc")]
    [InlineData(true, "b", "abc", "d")]
    [InlineData(true, "abc", "b", "d")]
    public void Contains(bool shouldBe, params string[] values) =>
        StringExtensions.AnyPermutationPairContains(values).Should().Be(shouldBe);

    [Theory]
    [InlineData(false, "")]
    [InlineData(false, "a")]
    [InlineData(false, "a", "b")]
    [InlineData(false, "a", "b", "c")]
    [InlineData(false, "A", "ab")]
    [InlineData(false, "ab", "A")]
    [InlineData(false, "B", "ba")]
    [InlineData(false, "ba", "B")]
    [InlineData(false, "A", "c", "ab")]
    [InlineData(false, "A", "ab", "c")]
    [InlineData(false, "ab", "A", "c")]
    [InlineData(true, "", "a")]
    [InlineData(true, "a", "")]
    [InlineData(true, "a", "a")]
    [InlineData(true, "ab", "ab")]
    [InlineData(true, "a", "ab")]
    [InlineData(true, "ab", "a")]
    [InlineData(true, "b", "ba")]
    [InlineData(true, "ba", "b")]
    [InlineData(true, "a", "c", "ab")]
    [InlineData(true, "a", "ab", "c")]
    [InlineData(true, "ab", "a", "c")]
    public void StartsWith(bool shouldBe, params string[] values) =>
        StringExtensions.AnyPermutationPairStartsWith(values, StringComparison.Ordinal).Should().Be(shouldBe);

    [Theory]
    [InlineData(false, "")]
    [InlineData(false, "a")]
    [InlineData(false, "a", "b")]
    [InlineData(false, "a", "b", "c")]
    [InlineData(true, "A", "ab")]
    [InlineData(true, "ab", "A")]
    [InlineData(true, "B", "ba")]
    [InlineData(true, "ba", "B")]
    [InlineData(true, "A", "c", "ab")]
    [InlineData(true, "A", "ab", "c")]
    [InlineData(true, "ab", "A", "c")]
    [InlineData(true, "", "a")]
    [InlineData(true, "a", "")]
    [InlineData(true, "a", "a")]
    [InlineData(true, "ab", "ab")]
    [InlineData(true, "a", "ab")]
    [InlineData(true, "ab", "a")]
    [InlineData(true, "b", "ba")]
    [InlineData(true, "ba", "b")]
    [InlineData(true, "a", "c", "ab")]
    [InlineData(true, "a", "ab", "c")]
    [InlineData(true, "ab", "a", "c")]
    public void StartsWithIgnoreCase(bool shouldBe, params string[] values) =>
        StringExtensions.AnyPermutationPairStartsWith(values, StringComparison.OrdinalIgnoreCase).Should().Be(shouldBe);

    [Theory]
    [InlineData(false, "")]
    [InlineData(false, "a")]
    [InlineData(false, "a", "b")]
    [InlineData(false, "a", "b", "c")]
    [InlineData(false, "B", "ab")]
    [InlineData(false, "ab", "B")]
    [InlineData(false, "A", "ba")]
    [InlineData(false, "ba", "A")]
    [InlineData(false, "B", "c", "ab")]
    [InlineData(false, "B", "ab", "c")]
    [InlineData(false, "ab", "B", "c")]
    [InlineData(true, "", "a")]
    [InlineData(true, "a", "")]
    [InlineData(true, "a", "a")]
    [InlineData(true, "ab", "ab")]
    [InlineData(true, "b", "ab")]
    [InlineData(true, "ab", "b")]
    [InlineData(true, "a", "ba")]
    [InlineData(true, "ba", "a")]
    [InlineData(true, "b", "c", "ab")]
    [InlineData(true, "b", "ab", "c")]
    [InlineData(true, "ab", "b", "c")]
    public void EndsWith(bool shouldBe, params string[] values) =>
        StringExtensions.AnyPermutationPairEndsWith(values, StringComparison.Ordinal).Should().Be(shouldBe);

    [Theory]
    [InlineData(false, "")]
    [InlineData(false, "a")]
    [InlineData(false, "a", "b")]
    [InlineData(false, "a", "b", "c")]
    [InlineData(true, "B", "ab")]
    [InlineData(true, "ab", "B")]
    [InlineData(true, "A", "ba")]
    [InlineData(true, "ba", "A")]
    [InlineData(true, "B", "c", "ab")]
    [InlineData(true, "B", "ab", "c")]
    [InlineData(true, "ab", "B", "c")]
    [InlineData(true, "", "a")]
    [InlineData(true, "a", "")]
    [InlineData(true, "a", "a")]
    [InlineData(true, "ab", "ab")]
    [InlineData(true, "b", "ab")]
    [InlineData(true, "ab", "b")]
    [InlineData(true, "a", "ba")]
    [InlineData(true, "ba", "a")]
    [InlineData(true, "b", "c", "ab")]
    [InlineData(true, "b", "ab", "c")]
    [InlineData(true, "ab", "b", "c")]
    public void EndsWithIgnoreCase(bool shouldBe, params string[] values) =>
        StringExtensions.AnyPermutationPairEndsWith(values, StringComparison.OrdinalIgnoreCase).Should().Be(shouldBe);

    [Fact]
    public void Exceptions()
    {
        Action act;

        act = () => StringExtensions.AnyPermutationPairContains(null);
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => StringExtensions.AnyPermutationPairStartsWith(null);
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => StringExtensions.AnyPermutationPairEndsWith(null);
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => StringExtensions.AnyPermutationPair(["a"], null);
        act.Should().ThrowExactly<ArgumentNullException>();
    }
}
