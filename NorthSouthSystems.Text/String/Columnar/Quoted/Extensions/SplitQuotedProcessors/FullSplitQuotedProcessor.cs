using System.Text;

namespace NorthSouthSystems.Text;

public static partial class StringQuotedExtensions
{
    private sealed class FullSplitQuotedProcessor : ISplitQuotedProcessor
    {
        internal FullSplitQuotedProcessor(StringQuotedSignals signals)
        {
            _signals = signals;

            _isNewRowTolerant = signals.IsNewRowTolerant;

            _delimiterTracker = StringSignalTracker.Create(_signals.Delimiters);
            _newRowTracker = StringSignalTracker.Create(_isNewRowTolerant ? null : _signals.NewRows);
            _quoteTracker = StringSignalTracker.Create(_signals.Quote);
            _escapeTracker = StringSignalTracker.Create(_signals.Escape);

            _quoteQuoteTracker = StringSignalTracker.Create(_signals.Quote + _signals.Quote);
        }

        private bool _inRow;
        private readonly List<string> _fields = new();

        private readonly StringBuilder _fieldBuilder = new();

        private bool _quoteInQuotes;
        private int _quoteQuoteCount;

        private bool _escaped;

        private readonly StringQuotedSignals _signals;

        private readonly IStringSignalTracker _delimiterTracker;
        private readonly IStringSignalTracker _newRowTracker;
        private readonly IStringSignalTracker _quoteTracker;
        private readonly IStringSignalTracker _escapeTracker;

        private readonly IStringSignalTracker _quoteQuoteTracker;

        // Do NOT Reset _newRowTolerantWasCarriageReturn because it needs to survive the "premature"
        // yielding of rows upon seeing the \r of a \r\n (Windows NewRow) in the case that the \r is
        // not followed by the \n.
        private readonly bool _isNewRowTolerant;
        private bool _newRowTolerantWasCarriageReturn;

        private void Reset()
        {
            _inRow = false;
            _fields.Clear();

            ResetField();
            ResetTrackers();
        }

        private void ResetField()
        {
            _fieldBuilder.Clear();

            _quoteInQuotes = false;
            _quoteQuoteCount = 0;

            _escaped = false;
        }

        private void ResetTrackers(bool wasQuoteTrackerTriggered = false)
        {
            _delimiterTracker.Reset();
            _newRowTracker.Reset();
            _quoteTracker.Reset();
            _escapeTracker.Reset();

            if (!wasQuoteTrackerTriggered)
                _quoteQuoteTracker.Reset();
        }

        public IEnumerable<string[]> Process(IEnumerable<char> rows)
        {
            foreach (char c in rows)
            {
                if (ProcessReturnsYieldRow(c))
                {
                    yield return _fields.ToArray();

                    Reset();
                }
            }

            if (_inRow)
                FlushField();

            if (_fields.Count > 0)
                yield return _fields.ToArray();

            Reset();
        }

        private bool ProcessReturnsYieldRow(char c)
        {
            // Because we immediately yield a row upon seeing a \r, we need to conditionally
            // ignore the immediately following \n in the case of a Windows NewRow (\r\n).
            if (_newRowTolerantWasCarriageReturn)
            {
                _newRowTolerantWasCarriageReturn = false;

                if (c == '\n')
                    return false;
            }

            _inRow = true;
            _fieldBuilder.Append(c);

            if (_escaped)
            {
                _escaped = false;

                return false;
            }

            int triggeredLength;

            if ((triggeredLength = _delimiterTracker.ProcessCharReturnsTriggeredLength(c)) > 0)
            {
                ResetTrackers();

                if (_quoteInQuotes)
                    return false;

                RewindField(triggeredLength);
                FlushField();
                ResetField();

                return false;
            }

            if (_quoteQuoteTracker.ProcessCharReturnsTriggeredLength(c) > 0)
            {
                ResetTrackers();

                _quoteInQuotes = !_quoteInQuotes;
                _quoteQuoteCount++;

                return false;
            }

            if ((triggeredLength = _quoteTracker.ProcessCharReturnsTriggeredLength(c)) > 0)
            {
                ResetTrackers(wasQuoteTrackerTriggered: true);
                RewindField(triggeredLength);

                _quoteInQuotes = !_quoteInQuotes;

                return false;
            }

            if (_isNewRowTolerant && (c == '\n' || c == '\r'))
            {
                ResetTrackers();

                if (_quoteInQuotes)
                    return false;

                RewindField(1);
                FlushField();

                _newRowTolerantWasCarriageReturn = c == '\r';

                return true;
            }

            if ((triggeredLength = _newRowTracker.ProcessCharReturnsTriggeredLength(c)) > 0)
            {
                ResetTrackers();

                if (_quoteInQuotes)
                    return false;

                RewindField(triggeredLength);
                FlushField();

                return true;
            }

            if ((triggeredLength = _escapeTracker.ProcessCharReturnsTriggeredLength(c)) > 0)
            {
                ResetTrackers();
                RewindField(triggeredLength);

                _escaped = true;

                return false;
            }

            return false;
        }

        private void RewindField(int length) => _fieldBuilder.Length -= length;

        private void FlushField()
        {
            // An empty field that is Quoted or a Quoted field containing only Quotes will never properly detect that the field
            // itself is Quoted because two consecutive quotes results in a Quote at the end of _fieldBuilder with _quoteInQuotes false;
            // therefore, _fieldBuilder will contain an extra Quote. E.G.
            //     a,"",c
            //     a,"""",c
            //     a,"""""",c
            if (_quoteQuoteCount > 0 && _fieldBuilder.Length == _signals.Quote.Length * _quoteQuoteCount)
                RewindField(_signals.Quote.Length);

            _fields.Add(_fieldBuilder.ToString());
        }
    }
}
