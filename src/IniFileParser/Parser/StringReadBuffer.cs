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
                    return _start + _size;
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
                return string.Format("[{0},{1}]", start, size);
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

        public void Trim()
        {
            int idx = _bufferIndexes.start;
            for (; idx < _bufferIndexes.size; idx++)
            {
                if (_buffer[idx] != ' ')
                {
                    _bufferIndexes.start = idx;
                    break;
                }
            }

            idx = _bufferIndexes.size - 1;
            for (; idx >= _bufferIndexes.start; idx--)
            {
                if (_buffer[idx] != ' ')
                {
                    break;
                }
            }

            _bufferIndexes.size = idx - _bufferIndexes.start +1;
        }

        public Range FindSubstring(string subString)
        {
            int subStringLength = subString.Length;

            if (subStringLength <= 0 || Count < subStringLength)
            {
                return Range.Empty();
            }

            int startIdx = -1;
            int currentIdx;
            for (currentIdx = 0; currentIdx < Count; ++currentIdx)
            {
                if (this[currentIdx] == subString[0])
                {
                    startIdx = currentIdx;
                    break;
                }
            }

            if (startIdx == -1) return Range.Empty();

            for (currentIdx = 0; currentIdx < subStringLength; ++currentIdx)
            {
                if (this[startIdx + currentIdx] != subString[currentIdx])
                {
                    return Range.Empty();
                }
            }

            return Range.FromIndexWithSize(startIdx, subStringLength);
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

            var isEmpty = _buffer.Count != 0;

            if (!isEmpty) LineNumber++;

            return isEmpty;
        }

        public void Resize(int newSize)
        {
            if (newSize < 0) return;
            if (newSize >= _bufferIndexes.size) return;

            _bufferIndexes.size = newSize;
        }

        public void ResizeBetweenIndexes(int startIdx, int endIdx)
        {
            if (endIdx <= startIdx) return;
            if (startIdx < 0) return;
            if (endIdx > _bufferIndexes.end) return;

            _bufferIndexes.start = _bufferIndexes.start + startIdx;
            _bufferIndexes.size = endIdx - startIdx;
        }

        public string Substring(Range range)
        {
            return new string(_buffer.ToArray(), range.start, range.size);
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
