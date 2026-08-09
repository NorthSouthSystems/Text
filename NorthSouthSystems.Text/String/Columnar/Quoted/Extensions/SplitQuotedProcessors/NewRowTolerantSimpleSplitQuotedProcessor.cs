using System.Text;

namespace NorthSouthSystems.Text;

public static partial class StringQuotedExtensions
{
    private sealed class NewRowTolerantSimpleSplitQuotedProcessor : ISplitQuotedProcessor
    {
        internal NewRowTolerantSimpleSplitQuotedProcessor(StringQuotedSignals signals)
        {
            _delimiter = signals.Delimiters.Single().Single();
            _quote = signals.Quote.SingleOrDefault();
            _escape = signals.Escape.SingleOrDefault();
        }

        private bool _inRow;
        private readonly List<string> _fields = new();

        private readonly StringBuilder _fieldBuilder = new();

        private bool _quoteWasTriggered;

        private bool _quoteInQuotes;
        private int _quoteQuoteCount;

        private bool _escaped;

        private readonly char _delimiter;
        private readonly char _quote;
        private readonly char _escape;

        // Do NOT Reset _newRowTolerantWasCarriageReturn because it needs to survive the "premature"
        // yielding of rows upon seeing the \r of a \r\n (Windows NewRow) in the case that the \r is
        // not followed by the \n.
        private bool _newRowTolerantWasCarriageReturn;

        private void Reset()
        {
            _inRow = false;
            _fields.Clear();

            ResetField();
        }

        private void ResetField()
        {
            _fieldBuilder.Clear();

            _quoteWasTriggered = false;

            _quoteInQuotes = false;
            _quoteQuoteCount = 0;

            _escaped = false;
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

            if (c == _delimiter)
            {
                _quoteWasTriggered = false;

                if (_quoteInQuotes)
                    return false;

                RewindField(1);
                FlushField();
                ResetField();

                return false;
            }

            if (_quoteWasTriggered && c == _quote)
            {
                _quoteWasTriggered = false;

                _quoteInQuotes = !_quoteInQuotes;
                _quoteQuoteCount++;

                return false;
            }

            if (c == _quote)
            {
                _quoteWasTriggered = true;

                RewindField(1);

                _quoteInQuotes = !_quoteInQuotes;

                return false;
            }

            if (c == '\n' || c == '\r')
            {
                _quoteWasTriggered = false;

                if (_quoteInQuotes)
                    return false;

                RewindField(1);
                FlushField();

                _newRowTolerantWasCarriageReturn = c == '\r';

                return true;
            }

            if (c == _escape)
            {
                _quoteWasTriggered = false;

                RewindField(1);

                _escaped = true;

                return false;
            }

            _quoteWasTriggered = false;

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
            if (_quoteQuoteCount > 0 && _fieldBuilder.Length == _quoteQuoteCount)
                RewindField(1);

            _fields.Add(_fieldBuilder.ToString());
        }
    }
}
