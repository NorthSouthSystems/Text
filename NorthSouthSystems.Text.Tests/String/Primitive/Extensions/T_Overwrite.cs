public class T_StringExtensions_Overwrite
{
    [Fact]
    public void Basic()
    {
        "a".Overwrite(0, "z").Should().Be("z");
        "a".Overwrite(1, "z").Should().Be("az");

        "ab".Overwrite(0, "z").Should().Be("zb");
        "ab".Overwrite(1, "z").Should().Be("az");
        "ab".Overwrite(2, "z").Should().Be("abz");

        "abc".Overwrite(0, "z").Should().Be("zbc");
        "abc".Overwrite(1, "z").Should().Be("azc");
        "abc".Overwrite(2, "z").Should().Be("abz");
        "abc".Overwrite(3, "z").Should().Be("abcz");

        "a".Overwrite(0, "zy").Should().Be("zy");
        "a".Overwrite(1, "zy").Should().Be("azy");

        "ab".Overwrite(0, "zy").Should().Be("zy");
        "ab".Overwrite(1, "zy").Should().Be("azy");
        "ab".Overwrite(2, "zy").Should().Be("abzy");

        "abc".Overwrite(0, "zy").Should().Be("zyc");
        "abc".Overwrite(1, "zy").Should().Be("azy");
        "abc".Overwrite(2, "zy").Should().Be("abzy");
        "abc".Overwrite(3, "zy").Should().Be("abczy");

        "abcd".Overwrite(0, "zy").Should().Be("zycd");
        "abcd".Overwrite(1, "zy").Should().Be("azyd");
        "abcd".Overwrite(2, "zy").Should().Be("abzy");
        "abcd".Overwrite(3, "zy").Should().Be("abczy");
        "abcd".Overwrite(4, "zy").Should().Be("abcdzy");
    }

    [Fact]
    public void Exceptions()
    {
        Action act;

        act = () => ((string)null).Overwrite(0, "z");
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => "a".Overwrite(-1, "z");
        act.Should().ThrowExactly<ArgumentOutOfRangeException>();

        act = () => "a".Overwrite(2, "z");
        act.Should().ThrowExactly<ArgumentOutOfRangeException>();

        act = () => "a".Overwrite(0, null);
        act.Should().ThrowExactly<ArgumentNullException>();
    }
}
