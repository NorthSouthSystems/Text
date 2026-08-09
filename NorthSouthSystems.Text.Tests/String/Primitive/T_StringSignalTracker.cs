public class T_StringSignalTracker
{
    [Fact]
    public void Empty()
    {
        IStringSignalTracker tracker;

        tracker = StringSignalTracker.Create();
        Base();
        Base();

        tracker = StringSignalTracker.Create(string.Empty);
        Base();
        Base();

        tracker = StringSignalTracker.Create(null);
        Base();
        Base();

        tracker = StringSignalTracker.Create(string.Empty, null);
        Base();
        Base();

        void Base()
        {
            tracker.ProcessCharReturnsTriggeredLength('a').Should().Be(0);
            tracker.ProcessCharReturnsTriggeredLength('a').Should().Be(0);
            tracker.ProcessCharReturnsTriggeredLength('b').Should().Be(0);
            tracker.ProcessCharReturnsTriggeredLength('b').Should().Be(0);
            tracker.Reset();
        }
    }

    [Fact]
    public void SingleChar()
    {
        IStringSignalTracker tracker;

        tracker = StringSignalTracker.Create("a");
        Base();
        Base();

        void Base()
        {
            tracker.ProcessCharReturnsTriggeredLength('a').Should().Be(1);
            tracker.Reset();

            tracker.ProcessCharReturnsTriggeredLength('b').Should().Be(0);
            tracker.ProcessCharReturnsTriggeredLength('a').Should().Be(1);
            tracker.Reset();
        }
    }

    [Fact]
    public void MultiCharSimple()
    {
        IStringSignalTracker tracker;

        tracker = StringSignalTracker.Create("ab");
        Base();
        Base();

        void Base()
        {
            tracker.ProcessCharReturnsTriggeredLength('a').Should().Be(0);
            tracker.ProcessCharReturnsTriggeredLength('b').Should().Be(2);
            tracker.Reset();

            tracker.ProcessCharReturnsTriggeredLength('b').Should().Be(0);
            tracker.ProcessCharReturnsTriggeredLength('a').Should().Be(0);
            tracker.ProcessCharReturnsTriggeredLength('c').Should().Be(0);
            tracker.ProcessCharReturnsTriggeredLength('a').Should().Be(0);
            tracker.ProcessCharReturnsTriggeredLength('b').Should().Be(2);
            tracker.Reset();
        }
    }

    [Fact]
    public void MultiCharComplex()
    {
        IStringSignalTracker tracker;

        tracker = StringSignalTracker.Create("abac");
        Base();
        Base();

        void Base()
        {
            tracker.ProcessCharReturnsTriggeredLength('a').Should().Be(0);
            tracker.ProcessCharReturnsTriggeredLength('b').Should().Be(0);
            tracker.ProcessCharReturnsTriggeredLength('a').Should().Be(0);
            tracker.ProcessCharReturnsTriggeredLength('c').Should().Be(4);
            tracker.Reset();

            tracker.ProcessCharReturnsTriggeredLength('b').Should().Be(0);
            tracker.ProcessCharReturnsTriggeredLength('a').Should().Be(0);
            tracker.ProcessCharReturnsTriggeredLength('c').Should().Be(0);
            tracker.ProcessCharReturnsTriggeredLength('a').Should().Be(0);
            tracker.ProcessCharReturnsTriggeredLength('b').Should().Be(0);
            tracker.ProcessCharReturnsTriggeredLength('a').Should().Be(0);
            tracker.ProcessCharReturnsTriggeredLength('b').Should().Be(0);
            tracker.ProcessCharReturnsTriggeredLength('a').Should().Be(0);
            tracker.ProcessCharReturnsTriggeredLength('c').Should().Be(4);
            tracker.Reset();
        }
    }

    [Fact]
    public void Composite()
    {
        IStringSignalTracker tracker;

        tracker = StringSignalTracker.Create("ab", "b");
        Base();
        Base();

        tracker = StringSignalTracker.Create("b", "ab");
        Base();
        Base();

        void Base()
        {
            tracker.ProcessCharReturnsTriggeredLength('b').Should().Be(1);
            tracker.Reset();

            tracker.ProcessCharReturnsTriggeredLength('c').Should().Be(0);
            tracker.ProcessCharReturnsTriggeredLength('b').Should().Be(1);
            tracker.Reset();

            tracker.ProcessCharReturnsTriggeredLength('a').Should().Be(0);
            tracker.ProcessCharReturnsTriggeredLength('b').Should().Be(2);
            tracker.Reset();

            tracker.ProcessCharReturnsTriggeredLength('a').Should().Be(0);
            tracker.ProcessCharReturnsTriggeredLength('c').Should().Be(0);
            tracker.ProcessCharReturnsTriggeredLength('a').Should().Be(0);
            tracker.ProcessCharReturnsTriggeredLength('a').Should().Be(0);
            tracker.ProcessCharReturnsTriggeredLength('b').Should().Be(2);
            tracker.Reset();
        }
    }
}
