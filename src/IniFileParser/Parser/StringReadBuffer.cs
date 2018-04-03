using System;
using System.Collections.Generic;
using System.IO;

namespace IniParser.Parser
{


    public sealed class StringReadBuffer
    {
        public struct Range
        {
            int _start, _size;

            public int start
            {
                get
                {
                    return _start;
                }

                set
                {
                    _start =  value < 0 ? 0 : value;
                }
            }

            public int size
            {
                get
                {
                    return _size;
                }

                set
                {
                    _size =  value < 0 ? 0 : value;
                }
            }

            public int end
            {
                get
                {
                    return _size <= 0 ? 0 : _start + (_size-1);
                }
            }

            public bool IsEmpty { get { return _size == 0; } }

            public static Range Empty()
            {
                return new Range { start = 0, size = 0 };
            }

            public static Range FromIndexWithSize(int start, int size)
            {
                if (start < 0 || size < 0) return Empty();

                return new Range { start = start, size = size };
            }

            public static Range WithIndexes(int start, int end)
            {
                if (start < 0 || end < 0) return Empty();

                return new Range { start = start, size = end - start + 1 };
            }

            public override string ToString()
            {
                return string.Format("[start:{0}, end:{1} size: {2}]",
                                     start,
                                     end,
                                     size);
            }
        }

        public StringReadBuffer(int capacity)
        {
            _buffer = new List<char>(capacity);
        }

        public uint LineNumber { get; private set; }

        public int Count { get { return _bufferIndexes.size; } }

        public char this[int idx]
        {
            get
            {
                return _buffer[idx + _bufferIndexes.start];
            }
        }

        public void Reset(StringReader dataSource)
        {
            _dataSource = dataSource;
            LineNumber = 0;
            _bufferIndexes = Range.Empty();
            _buffer.Clear();
        }

        public void TrimRange(ref Range range)
        {
            if (range.size <= 0) return;

            int endIdx = range.end;
            for (; endIdx >= range.start; endIdx--)
            {
                if (!char.IsWhiteSpace(this[endIdx]))
                {
                    break;
                }
            }


            int startIdx = range.start;
            for (; startIdx <= endIdx; startIdx++)
            {
                if (!char.IsWhiteSpace(this[startIdx]))
                {
                    range.start = startIdx;
                    break;
                }
            }

            range.size = endIdx - range.start +1;
        }

        public void Trim()
        {
            TrimRange(ref _bufferIndexes);
        }

        public Range FindSubstring(string subString)
        {
            int subStringLength = subString.Length;

            if (subStringLength <= 0 || Count < subStringLength)
            {
                return Range.Empty();
            }

            // Search the first char of the substring
            for (int firstCharIdx = 0; firstCharIdx < Count; ++firstCharIdx)
            {
                if (this[firstCharIdx] != subString[0])
                {
                    continue;
                }

                // Fail now if the substring can't fit given the size of the
                // buffer and the search start index
                if (firstCharIdx + subStringLength - 1 > Count)
                {
                    return Range.Empty();
                }

                bool isSubstringMismatch = false;
                // Check if the substring matches starting at the index
                for (int currentIdx = 0; currentIdx < subStringLength; ++currentIdx)
                {
                    if (this[firstCharIdx + currentIdx] != subString[currentIdx])
                    {
                        isSubstringMismatch = true;
                        break;
                    }
                }

                if (isSubstringMismatch)
                {
                    continue;
                }

                return Range.FromIndexWithSize(firstCharIdx, subStringLength);
            }

            return Range.Empty();
        }

        public bool ReadLine()
        {
            if (_dataSource == null) return false;

            _buffer.Clear();
            int c = _dataSource.Read();

            while (c != '\n' && c != -1)
            {
                if (c != '\r')
                {
                    _buffer.Add((char)c);
                }

                c = _dataSource.Read();
            }

            _bufferIndexes = Range.FromIndexWithSize(0, _buffer.Count);

            return _buffer.Count > 0 || c != -1;
        }

        public void Resize(int newSize)
        {
            if (newSize < 0) return;
            if (newSize >= _bufferIndexes.size) return;

            _bufferIndexes.size = newSize;
        }

        public void ResizeBetweenIndexes(int startIdx, int endIdx)
        {
            if (endIdx < startIdx) return;
            if (startIdx < 0) return;
            if (_bufferIndexes.start + endIdx > _bufferIndexes.end) return;

            _bufferIndexes.start = _bufferIndexes.start + startIdx;
            _bufferIndexes.size = endIdx - startIdx + 1;
        }

        public string Substring(Range range)
        {
            return new string(_buffer.ToArray(), range.start, range.size);
        }

        public bool IsEmpty
        {
            get { return _bufferIndexes.IsEmpty; }
        }

        public override string ToString()
        {
            return Substring(_bufferIndexes);
        }


        readonly List<char> _buffer;
        StringReader _dataSource;
        Range _bufferIndexes;

    }
}
