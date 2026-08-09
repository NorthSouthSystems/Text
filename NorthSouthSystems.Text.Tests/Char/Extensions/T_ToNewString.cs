public class T_CharExtensions_ToNewString
{
    [Fact]
    public void Basic()
    {
        char[] chars = new[] { 'f', 'o', 'o', 'b', 'a', 'r' };
        chars.ToNewString().Should().Be(new(chars));
    }
}
