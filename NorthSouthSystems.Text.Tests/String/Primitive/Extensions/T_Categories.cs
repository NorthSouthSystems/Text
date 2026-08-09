public class T_StringExtensions_WhereIsInAnyCategory
{
    [Theory]
    [InlineData("a1b2c3d", CharCategories.Digit | CharCategories.Letter, "a1b2c3d")]
    [InlineData("a1b2c3d", CharCategories.Digit, "123")]
    [InlineData("a1b2c3d", CharCategories.Letter, "abcd")]
    [InlineData("a1b2c3d", CharCategories.Punctuation | CharCategories.WhiteSpace, "")]
    [InlineData("a1b2-c3d", CharCategories.Digit | CharCategories.Letter, "a1b2c3d")]
    [InlineData("a1b2-c3d", CharCategories.Punctuation, "-")]
    public void Basic(string value, CharCategories categories, string shouldBe)
    {
        value.WhereIsInAnyCategory(categories).Should().Be(shouldBe);

        value.WhereIsInAnyCategory(CharCategories.All).Should().Be(value);
        shouldBe.WhereIsInAnyCategory(categories).Should().Be(shouldBe);
    }

    [Fact]
    public void Exceptions()
    {
        Action act;

        act = () => ((string)null).WhereIsInAnyCategory(CharCategories.All);
        act.Should().ThrowExactly<ArgumentNullException>();
    }
}
