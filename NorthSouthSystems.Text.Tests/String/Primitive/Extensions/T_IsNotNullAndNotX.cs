public class T_StringIsNotNullAndNotXExtensions
{
    [Fact]
    public void IsNotNullAndNotEmpty()
    {
        string.IsNotNullAndNotEmpty(null).Should().BeFalse();
        string.IsNotNullAndNotEmpty(string.Empty).Should().BeFalse();
        string.IsNotNullAndNotEmpty("a").Should().BeTrue();
    }

    [Fact]
    public void IsNotNullAndNotWhiteSpace()
    {
        string.IsNotNullAndNotWhiteSpace(null).Should().BeFalse();
        string.IsNotNullAndNotWhiteSpace(string.Empty).Should().BeFalse();
        string.IsNotNullAndNotWhiteSpace(" ").Should().BeFalse();
        string.IsNotNullAndNotWhiteSpace(Environment.NewLine).Should().BeFalse();
        string.IsNotNullAndNotWhiteSpace("a").Should().BeTrue();
    }
}
