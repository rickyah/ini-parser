using System;
using System.Collections.Generic;
using System.IO;

namespace IniParser.Parser
{
    public sealed class StringBuffer
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
                    _start = value < 0 ? 0 : value;
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
                    _size = value < 0 ? 0 : value;
                }
            }

            public int end
            {
                get
                {
                    return _size <= 0 ? 0 : _start + (_size - 1);
                }
            }

            public bool IsEmpty { get { return _size == 0; } }

            public void Reset()
            {
                start = 0;
                size = 0;
            }

            public static Range FromIndexWithSize(int start, int size)
            {
                if (start < 0 || size < 0) return new Range();

                return new Range { start = start, size = size };
            }

            public static Range WithIndexes(int start, int end)
            {
                if (start < 0 || end < 0) return new Range();

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

        public StringBuffer(int capacity)
        {
            _buffer = new List<char>(capacity);
        }

        public int Count { get { return _bufferIndexes.size; } }

        public bool IsEmpty
        {
            get { return _bufferIndexes.IsEmpty; }
        }

        public char this[int idx]
        {
            get
            {
                return _buffer[idx + _bufferIndexes.start];
            }
        }

        public StringBuffer DiscardChanges()
        {
            _bufferIndexes = Range.FromIndexWithSize(0, _buffer.Count);
            return this;
        }

        public Range FindSubstring(string subString, int startingIndex = 0)
        {
            int subStringLength = subString.Length;

            if (subStringLength <= 0 || Count < subStringLength)
            {
                return new Range();
            }

            startingIndex += _bufferIndexes.start;

            // Search the first char of the substring
            for (int firstCharIdx = startingIndex; firstCharIdx <= _bufferIndexes.end; ++firstCharIdx)
            {
                if (_buffer[firstCharIdx] != subString[0])
                {
                    continue;
                }

                // Fail now if the substring can't fit given the size of the
                // buffer and the search start index
                if (firstCharIdx + subStringLength - 1 > _bufferIndexes.end)
                {
                    return new Range();
                }

                bool isSubstringMismatch = false;
                // Check if the substring matches starting at the index
                for (int currentIdx = 0; currentIdx < subStringLength; ++currentIdx)
                {
                    if (_buffer[firstCharIdx + currentIdx] != subString[currentIdx])
                    {
                        isSubstringMismatch = true;
                        break;
                    }
                }

                if (isSubstringMismatch)
                {
                    continue;
                }

                 return Range.FromIndexWithSize(firstCharIdx - _bufferIndexes.start, subStringLength);
            }

            return new Range();
        }

        public bool ReadLine()
        {
            if (_dataSource == null) return false;

            _buffer.Clear();
            int c = _dataSource.Read();

            // Read until new line ('\n') or EOF (-1)
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

        public void Reset(StringReader dataSource)
        {
            _dataSource = dataSource;
            _bufferIndexes.Reset();
            _buffer.Clear();
        }

        public void Resize(int newSize)
        {
            // TODO: not clear what this resizes, 
            if (newSize < 0) return;
            if (newSize >= _bufferIndexes.size) return;

            _bufferIndexes.size = newSize;
        }

        public void ResizeBetweenIndexes(int startIdx, int endIdx)
        {
            var indexesSize = endIdx - startIdx + 1;

            if (indexesSize <= 0) return;
            if (indexesSize > _bufferIndexes.size) return;

            _bufferIndexes.start += startIdx;
            _bufferIndexes.size = endIdx - startIdx + 1;
        }

        public string Substring(Range range)
        {
            return new string(_buffer.ToArray(),
                              _bufferIndexes.start + range.start,
                              range.size);
        }

        public void TrimRange(ref Range range)
        {
            CheckRange(range);

            if (range.size <= 0) return;

            int endIdx = range.end;
            for (; endIdx >= range.start; endIdx--)
            {
                if (!char.IsWhiteSpace(_buffer[_bufferIndexes.start + endIdx]))
                {
                    break;
                }
            }

            int startIdx = range.start;
            for (; startIdx <= endIdx; startIdx++)
            {
  
                if (!char.IsWhiteSpace(_buffer[startIdx + _bufferIndexes.start]))
                {
                    range.start = startIdx;
                    break;
                }
            }

            range.size = endIdx - startIdx + 1;
        }

        public void Trim()
        {
            TrimRange(ref _bufferIndexes);
        }

        public override string ToString()
        {
            return new string(_buffer.ToArray(),
                               _bufferIndexes.start,
                               _bufferIndexes.size);
        }

        public string ToString(Range range)
        {
            CheckRange(range);
            return new string(_buffer.ToArray(),
                              _bufferIndexes.start + range.start,
                              range.size);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private void CheckRange(Range range)
        {
            if ( range.start < 0 
             || range.size > _bufferIndexes.size
             || range.start + _bufferIndexes.start > _bufferIndexes.end)
            {
                throw new IndexOutOfRangeException();
            }
        }
        readonly List<char> _buffer;
        StringReader _dataSource;

        Range _bufferIndexes;

        public bool StartsWith(string str)
        {
            if (string.IsNullOrEmpty(str)) return false;

            for(int idx = _bufferIndexes.start; idx < str.Length; ++idx)
            {
                if (str[idx] != _buffer[idx]) return false;
            }

            return true;
        }
    }
}
