public class T_StringFieldWrapper
{
    [Fact]
    public void Equality()
    {
        True(default, default);
        True(default, new(null, null));
        True(new(null, null), new(null, null));
        True(new(string.Empty, null), new(string.Empty, null));
        True(new(null, string.Empty), new(null, string.Empty));
        True(new("Foo", null), new("Foo", null));
        True(new(null, "Bar"), new(null, "Bar"));
        True(new("Foo", "Bar"), new("Foo", "Bar"));

        False(default, new(string.Empty, null));
        False(default, new(null, string.Empty));
        False(default, new("Foo", null));
        False(default, new(null, "Bar"));
        False(new("Foo", null), new("Bar", null));
        False(new("Foo", "Bar"), new("Foo", "Rab"));
        False(new("Foo", "Bar"), new("Oof", "Bar"));

        static void True(StringFieldWrapper wrapperLeft, StringFieldWrapper wrapperRight)
        {
            TrueImpl(wrapperLeft, wrapperRight);
            TrueImpl(wrapperRight, wrapperLeft);
        }

        static void TrueImpl(StringFieldWrapper wrapperLeft, StringFieldWrapper wrapperRight)
        {
            wrapperLeft.Equals((object)wrapperRight).Should().BeTrue();
            wrapperLeft.Equals(wrapperRight).Should().BeTrue();
            (wrapperLeft == wrapperRight).Should().BeTrue();
            (wrapperLeft != wrapperRight).Should().BeFalse();
            wrapperLeft.GetHashCode().Should().Be(wrapperRight.GetHashCode());
        }

        static void False(StringFieldWrapper wrapperLeft, StringFieldWrapper wrapperRight)
        {
            FalseImpl(wrapperLeft, wrapperRight);
            FalseImpl(wrapperRight, wrapperLeft);
        }

        static void FalseImpl(StringFieldWrapper wrapperLeft, StringFieldWrapper wrapperRight)
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
        StringFieldWrapper wrapper;

        wrapper = new("A", "1");
        wrapper.ColumnName.Should().Be("A");
        wrapper.Value.Should().Be("1");
        wrapper.ToString().Should().Be("1");

        wrapper = new("A", null);
        wrapper.ToString().Should().BeEmpty();
    }

    [Fact]
    public void ConversionBool()
    {
        StringFieldWrapper wrapper;

        wrapper = new("A", "TRUE");
        ((string)wrapper).Should().Be("TRUE");
        ((bool)wrapper).Should().BeTrue();
        ((bool?)wrapper).Value.Should().BeTrue();

        wrapper = new("A", "true");
        ((string)wrapper).Should().Be("true");
        ((bool)wrapper).Should().BeTrue();
        ((bool?)wrapper).Value.Should().BeTrue();

        wrapper = new("A", "FALSE");
        ((string)wrapper).Should().Be("FALSE");
        ((bool)wrapper).Should().BeFalse();
        ((bool?)wrapper).Value.Should().BeFalse();
    }

    [Fact]
    public void ConversionNumeric()
    {
        StringFieldWrapper wrapper;

        wrapper = new("A", "7");
        ((string)wrapper).Should().Be("7");
        ((int)wrapper).Should().Be(7);
        ((int?)wrapper).Value.Should().Be(7);
        ((uint)wrapper).Should().Be(7u);
        ((uint?)wrapper).Value.Should().Be(7u);
        ((long)wrapper).Should().Be(7L);
        ((long?)wrapper).Value.Should().Be(7L);
        ((ulong)wrapper).Should().Be(7ul);
        ((ulong?)wrapper).Value.Should().Be(7ul);
        ((float)wrapper).Should().Be(7f);
        ((float?)wrapper).Value.Should().Be(7f);
        ((double)wrapper).Should().Be(7d);
        ((double?)wrapper).Value.Should().Be(7d);
        ((decimal)wrapper).Should().Be(7m);
        ((decimal?)wrapper).Value.Should().Be(7m);

        wrapper = new("A", "7000000000");
        ((string)wrapper).Should().Be("7000000000");
        ((long)wrapper).Should().Be(7000000000L);
        ((long?)wrapper).Value.Should().Be(7000000000L);
        ((ulong)wrapper).Should().Be(7000000000ul);
        ((ulong?)wrapper).Value.Should().Be(7000000000ul);

        wrapper = new("A", "7.7");
        ((string)wrapper).Should().Be("7.7");
        ((float)wrapper).Should().Be(7.7f);
        ((float?)wrapper).Value.Should().Be(7.7f);
        ((double)wrapper).Should().Be(7.7d);
        ((double?)wrapper).Value.Should().Be(7.7d);
        ((decimal)wrapper).Should().Be(7.7m);
        ((decimal?)wrapper).Value.Should().Be(7.7m);
    }

    [Fact]
    public void ConversionDateTime()
    {
        StringFieldWrapper wrapper;

        var now = DateTime.Now;
        var nowOffset = DateTimeOffset.Now;

        wrapper = new("A", now.ToString("o"));
        ((string)wrapper).Should().Be(now.ToString("o"));
        ((DateTime)wrapper).Should().Be(now);
        ((DateTime?)wrapper).Value.Should().Be(now);

        wrapper = new("A", nowOffset.ToString("o"));
        ((string)wrapper).Should().Be(nowOffset.ToString("o"));
        ((DateTimeOffset)wrapper).Should().Be(nowOffset);
        ((DateTimeOffset?)wrapper).Value.Should().Be(nowOffset);

        var ts = now - now.Date;
        ts = new(ts.Hours, ts.Minutes, ts.Seconds);
        string tsXsd = string.Format("PT{0}H{1}M{2}S", ts.Hours, ts.Minutes, ts.Seconds);

        wrapper = new("A", tsXsd);
        ((string)wrapper).Should().Be(tsXsd);
        ((TimeSpan)wrapper).Should().Be(ts);
        ((TimeSpan?)wrapper).Value.Should().Be(ts);
    }

    [Fact]
    public void ConversionGuid()
    {
        StringFieldWrapper wrapper;

        var guid = Guid.NewGuid();

        wrapper = new("A", guid.ToString());
        ((string)wrapper).Should().Be(guid.ToString());
        ((Guid)wrapper).Should().Be(guid);
        ((Guid?)wrapper).Value.Should().Be(guid);
    }

    [Fact]
    public void ConversionIsNull()
    {
        StringFieldWrapper wrapper;

        wrapper = default;
        ((string)wrapper).Should().BeNull();
        ShouldBeNull(default);

        wrapper = new("A", null);
        ((string)wrapper).Should().BeNull();
        ShouldBeNull(wrapper);

        wrapper = new("A", string.Empty);
        ShouldBeNull(wrapper);

        wrapper = new("A", " ");
        ShouldBeNull(wrapper);

        wrapper = new("A", "  ");
        ShouldBeNull(wrapper);

        static void ShouldBeNull(StringFieldWrapper w)
        {
            ((bool?)w).Should().BeNull();
            ((int?)w).Should().BeNull();
            ((uint?)w).Should().BeNull();
            ((long?)w).Should().BeNull();
            ((ulong?)w).Should().BeNull();
            ((float?)w).Should().BeNull();
            ((double?)w).Should().BeNull();
            ((decimal?)w).Should().BeNull();
            ((DateTime?)w).Should().BeNull();
            ((DateTimeOffset?)w).Should().BeNull();
            ((TimeSpan?)w).Should().BeNull();
            ((Guid?)w).Should().BeNull();
        }
    }

    [Fact]
    public void Exceptions()
    {
        Action act;
        object x;

        act = () => x = (bool)new StringFieldWrapper("A", null);
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => x = (int)new StringFieldWrapper("A", null);
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => x = (uint)new StringFieldWrapper("A", null);
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => x = (long)new StringFieldWrapper("A", null);
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => x = (ulong)new StringFieldWrapper("A", null);
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => x = (float)new StringFieldWrapper("A", null);
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => x = (double)new StringFieldWrapper("A", null);
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => x = (decimal)new StringFieldWrapper("A", null);
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => x = (DateTime)new StringFieldWrapper("A", null);
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => x = (DateTimeOffset)new StringFieldWrapper("A", null);
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => x = (TimeSpan)new StringFieldWrapper("A", null);
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => x = (Guid)new StringFieldWrapper("A", null);
        act.Should().ThrowExactly<ArgumentNullException>();
    }
}
