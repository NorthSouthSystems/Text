public class T_StringRowWrapper
{
    [Fact]
    public void Equality()
    {
        var factoryA = new StringRowWrapperFactory(["A"]);
        var factoryB = new StringRowWrapperFactory(["B"]);

        string[] fieldsY = ["Y"];
        string[] fieldsZ = ["Z"];

        True(default, default);
        True(default, new(null, null));
        True(new(null, null), new(null, null));
        True(new(factoryA, null), new(factoryA, null));
        True(new(null, fieldsY), new(null, fieldsY));
        True(new(factoryA, fieldsY), new(factoryA, fieldsY));
        True(new(factoryA, fieldsZ), new(factoryA, fieldsZ));
        True(new(factoryB, fieldsY), new(factoryB, fieldsY));
        True(new(factoryB, fieldsZ), new(factoryB, fieldsZ));

        False(default, new(factoryA, null));
        False(default, new(null, fieldsY));
        False(new(factoryA, null), new(null, fieldsY));
        False(new(factoryA, fieldsY), new(factoryA, fieldsZ));
        False(new(factoryA, fieldsY), new(factoryB, fieldsY));
        False(new(factoryB, fieldsY), new(factoryB, fieldsZ));

        static void True(StringRowWrapper wrapperLeft, StringRowWrapper wrapperRight)
        {
            TrueImpl(wrapperLeft, wrapperRight);
            TrueImpl(wrapperRight, wrapperLeft);
        }

        static void TrueImpl(StringRowWrapper wrapperLeft, StringRowWrapper wrapperRight)
        {
            wrapperLeft.Equals((object)wrapperRight).Should().BeTrue();
            wrapperLeft.Equals(wrapperRight).Should().BeTrue();
            (wrapperLeft == wrapperRight).Should().BeTrue();
            (wrapperLeft != wrapperRight).Should().BeFalse();
            wrapperLeft.GetHashCode().Should().Be(wrapperRight.GetHashCode());
        }

        static void False(StringRowWrapper wrapperLeft, StringRowWrapper wrapperRight)
        {
            FalseImpl(wrapperLeft, wrapperRight);
            FalseImpl(wrapperRight, wrapperLeft);
        }

        static void FalseImpl(StringRowWrapper wrapperLeft, StringRowWrapper wrapperRight)
        {
            wrapperLeft.Equals((object)wrapperRight).Should().BeFalse();
            wrapperLeft.Equals(wrapperRight).Should().BeFalse();
            (wrapperLeft == wrapperRight).Should().BeFalse();
            (wrapperLeft != wrapperRight).Should().BeTrue();
            wrapperLeft.GetHashCode().Should().NotBe(wrapperRight.GetHashCode());
        }
    }

    [Fact]
    public void Basic()
    {
        var factory = new StringRowWrapperFactory(new[] { "C0", "C1", "C2" });
        var row = factory.Wrap(new[] { "F0", "F1" });

        row[0].Value.Should().Be("F0");
        row[1].Value.Should().Be("F1");
        row[2].Value.Should().Be(null);

        row["C0"].Value.Should().Be("F0");
        row["C1"].Value.Should().Be("F1");
        row["C2"].Value.Should().Be(null);

        var fields = row.Fields.ToArray();

        fields.Length.Should().Be(3);
        fields[0].Value.Should().Be("F0");
        fields[1].Value.Should().Be("F1");
        fields[2].Value.Should().Be(null);
    }

    [Fact]
    public void Exceptions()
    {
        Action act;

        act = () =>
        {
            var factory = new StringRowWrapperFactory(new[] { "C0", "C1", "C2" });
            var row = factory.Wrap(new[] { "F0", "F1" });
            var field = row[-1];
        };
        act.Should().ThrowExactly<ArgumentOutOfRangeException>("IndexOutOfRange");

        act = () =>
        {
            var factory = new StringRowWrapperFactory(new[] { "C0", "C1", "C2" });
            var row = factory.Wrap(new[] { "F0", "F1" });
            var field = row[3];
        };
        act.Should().ThrowExactly<ArgumentOutOfRangeException>("IndexOutOfRange");

        act = () =>
        {
            var factory = new StringRowWrapperFactory(new[] { "C0", "C1", "C2" });
            var row = factory.Wrap(new[] { "F0", "F1" });
            var field = row["C3"];
        };
        act.Should().ThrowExactly<ArgumentException>("ColumnNotFound");
    }
}
