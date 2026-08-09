public class T_String
{
    /// <summary>
    /// This test method demonstrates that String.Join allows one of the joined values to be null; therefore, our
    /// Join methods should allow the same.
    /// </summary>
    [Theory]
    [InlineData(new[] { null, "b", "c" }, ",b,c")]
    [InlineData(new[] { "a", null, "c" }, "a,,c")]
    [InlineData(new[] { "a", "b", null }, "a,b,")]
    [InlineData(new[] { "a", "b", "c" }, "a,b,c")]
    public void JoinWithANullValue(string[] values, string expectionJoin) =>
        string.Join(",", values).Should().Be(expectionJoin);
}
