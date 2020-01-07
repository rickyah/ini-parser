using NUnit.Framework;
using IniParser.Parser;
using System.IO;
using System;

namespace IniParser.Tests.Model
{
    [TestFixture]
    public class StringReaderBufferTest
    {
        private StringBuffer InitBufferAndReadLine(string str)
        {
            var buffer = new StringBuffer(str.Length);
            buffer.Reset(new StringReader(str));
            Assert.That(buffer.ReadLine(), Is.True);
            return buffer;
        }

        [Test]
        public void test_range_creation()
        {
            var r1 = StringBuffer.Range.FromIndexWithSize(4, 6);
            Assert.That(r1.start, Is.EqualTo(4));
            Assert.That(r1.end, Is.EqualTo(9));
            Assert.That(r1.size, Is.EqualTo(6));

            var r2 = StringBuffer.Range.WithIndexes(4, 4);
            Assert.That(r2.start, Is.EqualTo(4));
            Assert.That(r2.end, Is.EqualTo(4));
            Assert.That(r2.size, Is.EqualTo(1));
        }

        [Test]
        public void test_empty_range()
        {
            var r1 = StringBuffer.Range.FromIndexWithSize(4, 0);
            Assert.That(r1.start, Is.EqualTo(0));
            Assert.That(r1.end, Is.EqualTo(0));
            Assert.That(r1.size, Is.EqualTo(0));

            var r2 = StringBuffer.Range.WithIndexes(4, 2);
            Assert.That(r2.start, Is.EqualTo(0));
            Assert.That(r2.end, Is.EqualTo(0));
            Assert.That(r2.size, Is.EqualTo(0));
        }
            
        [Test]
        public void check_default_initialization()
        {
            var emptyBuffer = new StringBuffer(0);

            Assert.That(emptyBuffer.ToString(), Is.EqualTo(string.Empty));
            Assert.That(emptyBuffer.Count, Is.EqualTo(0));
            Assert.That(emptyBuffer.ReadLine(), Is.False);
        }

        [Test]
        public void check_default_reset()
        {
            var buffer = new StringBuffer(0);

            buffer.Reset(new StringReader(string.Empty));

            Assert.That(buffer.ToString(), Is.EqualTo(string.Empty));
            Assert.That(buffer.Count, Is.EqualTo(0));
            Assert.That(buffer.ReadLine(), Is.False);
        }

        [Test]
        public void check_auto_increases_capacity()
        {
            var str = "hello world!";
            var smallBuffer = new StringBuffer(2);
            smallBuffer.Reset(new StringReader(str));
            Assert.That(smallBuffer.ReadLine(), Is.True);
            Assert.That(smallBuffer.ToString(), Is.EqualTo(str));
        }

        [Test]
        public void check_automatic_buffer_resize()
        {
            var str = "hello world!";
            var buffer = InitBufferAndReadLine(str);

            Assert.That(buffer.Count, Is.EqualTo(str.Length));
        }

        [Test]
        public void check_shallow_copy()
        {
            var str = "  hello world!   ";
            var buffer = InitBufferAndReadLine(str);
            var copy = buffer.SwallowCopy();
            buffer.Trim();

            Assert.That(copy.ToString(), Is.EqualTo(str));
            Assert.That(copy.ToString(), Is.Not.EqualTo(buffer.ToString()));
        }

        [Test]
        public void trim_end()
        {
            var str = "   hello world!  ";
            var buffer = InitBufferAndReadLine(str);

            buffer.TrimEnd();
            Assert.That(buffer.ToString(), Is.EqualTo(str.TrimEnd()));

            var buffer2 = InitBufferAndReadLine("      ");
            buffer2.TrimEnd();
            Assert.That(buffer2.IsEmpty, Is.True);

            var str2 = "hello   world";
            var buffer3 = InitBufferAndReadLine(str2);
            buffer3.TrimEnd();
            Assert.That(buffer3.ToString(), Is.EqualTo(str2));
        }

        [Test]
        public void trim_start()
        {
            var str = "   hello world!  ";
            var buffer = InitBufferAndReadLine(str);

            buffer.TrimStart();
            Assert.That(buffer.ToString(), Is.EqualTo(str.TrimStart()));

            var buffer2 = InitBufferAndReadLine("      ");
            buffer2.TrimStart();
            Assert.That(buffer2.IsEmpty, Is.True);

            var str2 = "hello   world";
            var buffer3 = InitBufferAndReadLine(str2);
            buffer3.TrimStart();
            Assert.That(buffer3.ToString(), Is.EqualTo(str2));
        }

        [Test]
        public void trim()
        {
            var str = "   hello world!  ";
            var buffer = InitBufferAndReadLine(str);

            Assert.That(buffer.Count, Is.EqualTo(str.Length));
            buffer.Trim();
            Assert.That(buffer.Count, Is.EqualTo(str.Trim().Length));
        }

        [Test]
        public void trimming_whitespace_buffer()
        {
            var str = "    ";
            var buffer = InitBufferAndReadLine(str);

            Assert.That(buffer.Count, Is.EqualTo(str.Length));
            buffer.Trim();
            Assert.That(buffer.Count, Is.EqualTo(str.Trim().Length));
        }

        [Test]
        public void check_trimming_with_new_line()
        {
            var str = @"   hello world!
";
            var buffer = InitBufferAndReadLine(str);
            Assert.That(buffer.Count, Is.EqualTo(str.Length - Environment.NewLine.Length));
            buffer.Trim();
            Assert.That(buffer.Count, Is.EqualTo(str.Trim().Length));
            Assert.That(buffer.ToString(), Is.EqualTo("hello world!"));
        }

        [Test]
        public void test_whitespace_buffer()
        {
            var str = @"   ";
            var buffer = InitBufferAndReadLine(str);

            Assert.That(buffer.IsWhitespace, Is.True);
        }

        [Test]
        public void find_substring()
        {
            var str = "hello wor world!";
            var buffer = InitBufferAndReadLine(str);

            var result = buffer.FindSubstring("world");
            Assert.That(result.start, Is.EqualTo(10));
            Assert.That(result.size, Is.EqualTo(5));

            result = buffer.FindSubstring("");
            Assert.That(result.start, Is.EqualTo(0));
            Assert.That(result.size, Is.EqualTo(0));


            result = buffer.FindSubstring("d!");
            Assert.That(result.start, Is.EqualTo(14));
            Assert.That(result.size, Is.EqualTo(2));
        }

        [Test]
        public void find_trimmed_substring()
        {
            var buffer = InitBufferAndReadLine("    hello wor world!   ");

            buffer.Trim();
            var result = buffer.FindSubstring("world");
            Assert.That(result.start, Is.EqualTo(10));
            Assert.That(result.size, Is.EqualTo(5));

            result = buffer.FindSubstring("");
            Assert.That(result.start, Is.EqualTo(0));
            Assert.That(result.size, Is.EqualTo(0));

            result = buffer.FindSubstring("d!");
            Assert.That(result.start, Is.EqualTo(14));
            Assert.That(result.size, Is.EqualTo(2));

            result = buffer.FindSubstring(" ");
            Assert.That(result.start, Is.EqualTo(5));
            Assert.That(result.size, Is.EqualTo(1));

            result = buffer.FindSubstring("!");
            Assert.That(result.start, Is.EqualTo(15));
            Assert.That(result.size, Is.EqualTo(1));
        }

        [Test]
        public void substring()
        {
            var buffer = InitBufferAndReadLine("    hello world!");
            buffer.Trim();

            var range = StringBuffer.Range.WithIndexes(3, 9);

            Assert.That(buffer.Substring(range).ToString(), Is.EqualTo("lo worl"));

            range = StringBuffer.Range.WithIndexes(0, 0);
            Assert.That(buffer.Substring(range).ToString(), Is.EqualTo("h"));

            range = new StringBuffer.Range();
            Assert.That(buffer.Substring(range).ToString(), Is.EqualTo(string.Empty));

            range = StringBuffer.Range.WithIndexes(-1, -1);
            Assert.That(buffer.Substring(range).ToString(), Is.EqualTo(string.Empty));

            range = StringBuffer.Range.FromIndexWithSize(-1, 4);
            Assert.That(buffer.Substring(range).ToString(), Is.EqualTo(string.Empty));
        }

        [Test]
        public void resize()
        {
            var str = "hello world!";
            var originalBuffer = InitBufferAndReadLine(str);

            var buffer = originalBuffer.SwallowCopy();
            buffer.Resize(5);
            Assert.That(buffer.ToString(), Is.EqualTo("hello"));

            buffer.Resize(50);
            Assert.That(buffer.ToString(), Is.EqualTo("hello"));

            buffer.Resize(-1);
            Assert.That(buffer.ToString(), Is.EqualTo("hello"));

            buffer = originalBuffer.SwallowCopy();
            buffer.Resize(StringBuffer.Range.FromIndexWithSize(6, 5));
            Assert.That(buffer.ToString(), Is.EqualTo("world"));

            buffer.Resize(StringBuffer.Range.FromIndexWithSize(1, 2));
            Assert.That(buffer.ToString(), Is.EqualTo("or"));

            buffer = originalBuffer.SwallowCopy();
            buffer.Resize(6, 5);
            Assert.That(buffer.ToString(), Is.EqualTo("world"));
        }

        [Test]
        public void resize_between_indexes()
        {
            var buffer = InitBufferAndReadLine("   hello world!");
            buffer.Trim();

            var copy = buffer.SwallowCopy();

            copy.ResizeBetweenIndexes(5, 11);
            Assert.That(copy.ToString(), Is.EqualTo(" world!"));

            copy.ResizeBetweenIndexes(1, 2);
            Assert.That(copy.ToString(), Is.EqualTo("wo"));

            copy = buffer.SwallowCopy();
            copy.ResizeBetweenIndexes(6, 6);
            Assert.That(copy.ToString(), Is.EqualTo("w"));
        }

        [Test]
        public void resize_between_invalid_indexes_does_not_changes_buffer()
        {
            var buffer = InitBufferAndReadLine("hello world!");

            buffer.ResizeBetweenIndexes(9, 6);
            Assert.That(buffer.ToString(), Is.EqualTo("hello world!"));

            buffer.ResizeBetweenIndexes(-1, 11);
            Assert.That(buffer.ToString(), Is.EqualTo("hello world!"));

            buffer.ResizeBetweenIndexes(10, 211);
            Assert.That(buffer.ToString(), Is.EqualTo("hello world!"));
        }

        [Test]
        public void discard_changes_in_buffer()
        {
            var buffer = InitBufferAndReadLine("  hello world! ");

            buffer.Trim();
            Assert.That(buffer.ToString(), Is.EqualTo("hello world!"));

            buffer.ResizeBetweenIndexes(3, 8);
            Assert.That(buffer.ToString(), Is.EqualTo("lo wor"));

            buffer.DiscardChanges();
            Assert.That(buffer.ToString(), Is.EqualTo("  hello world! "));
        }

        [Test]
        public void starts_with()
        {
            var buffer = InitBufferAndReadLine("  hello world! ");

            Assert.That(buffer.StartsWith("  h"), Is.True);

            Assert.That(buffer.StartsWith("hel"), Is.False);
        }

        [Test]
        public void convert_to_string()
        {
            var str = "hello world!";
            var buffer = InitBufferAndReadLine(str);

            Assert.That(buffer.ToString(), Is.EqualTo(str));
            var range = StringBuffer.Range.FromIndexWithSize(6, 5);
            Assert.That(buffer.ToString(range), Is.EqualTo("world"));
        }
    }
}
