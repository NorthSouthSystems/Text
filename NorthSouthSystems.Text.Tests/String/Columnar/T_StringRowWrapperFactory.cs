public class T_StringRowWrapperFactory
{
    [Fact]
    public void Exceptions()
    {
        Action act;

        act = () => new StringRowWrapperFactory(null);
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => new StringRowWrapperFactory(new[] { "A", "A" });
        act.Should().ThrowExactly<ArgumentException>("ConstructionDuplicateColumnNames");

        act = () => new StringRowWrapperFactory(new[] { "A", "B", "B" });
        act.Should().ThrowExactly<ArgumentException>("ConstructionDuplicateColumnNames");

        act = () => new StringRowWrapperFactory(new[] { "A", "B", "C", "A" });
        act.Should().ThrowExactly<ArgumentException>("ConstructionDuplicateColumnNames");

        act = () => new StringRowWrapperFactory(new[] { "A", "B", "C" }).Wrap(null);
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => new StringRowWrapperFactory(new[] { "A", "B", "C" }).Wrap(new[] { "1", "2", "3", "4" });
        act.Should().ThrowExactly<ArgumentException>("WrapFieldsTooMany");
    }
}
