using NUnit.Framework;
using IniParser.Parser;
using System.IO;

namespace IniFileParser.Tests
{
    [TestFixture]
    public class StringReaderBufferTest
    {
        StringBuffer buffer;

        private string InitBufferAndReadLine(string str)
        {
            buffer.Reset(new StringReader(str));
            Assert.That(buffer.ReadLine(), Is.True);
            return str;
        }

        [SetUp] public void Setup()
        {
            buffer = new StringBuffer(256);
        }

        [Test] public void check_default_initialization()
        {
            var emptyBuffer = new StringBuffer(0);

            Assert.That(emptyBuffer.ToString(), Is.EqualTo(string.Empty));
            Assert.That(emptyBuffer.Count, Is.EqualTo(0));
            Assert.That(emptyBuffer.ReadLine(), Is.False);
        }

        [Test] public void check_default_reset()
        {
            buffer = new StringBuffer(0);

            buffer.Reset(new StringReader(string.Empty));

            Assert.That(buffer.ToString(), Is.EqualTo(string.Empty));
            Assert.That(buffer.Count, Is.EqualTo(0));
            Assert.That(buffer.ReadLine(), Is.False);
        }

        [Test] public void check_automatic_buffer_resize()
        {
            var str = InitBufferAndReadLine("hello world!");

            Assert.That(buffer.Count, Is.EqualTo(str.Length));
        }

        [Test] public void trim_buffer()
        {
            var str = InitBufferAndReadLine("   hello world!  ");

            Assert.That(buffer.Count, Is.EqualTo(str.Length));
            buffer.Trim();
            Assert.That(buffer.Count, Is.EqualTo(str.Trim().Length));
        }

        [Test] public void trimming_whitespace_buffer()
        {
            var str = InitBufferAndReadLine("    ");

            Assert.That(buffer.Count, Is.EqualTo(str.Length));
            buffer.Trim();
            Assert.That(buffer.Count, Is.EqualTo(str.Trim().Length));
        }

        [Test] public void check_trimming_with_new_line()
        {
            var str = InitBufferAndReadLine(@"   hello world!
");

            Assert.That(buffer.Count, Is.EqualTo(str.Length -1 ));
            buffer.Trim();
            Assert.That(buffer.Count, Is.EqualTo(str.Trim().Length));
            Assert.That(buffer.ToString(), Is.EqualTo("hello world!"));
        }

        [Test] public void find_substring()
        {
            InitBufferAndReadLine("hello wor world!");

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
            InitBufferAndReadLine("    hello wor world!   ");

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
        }

        [Test] public void substring()
        {
            InitBufferAndReadLine("    hello world!");
            buffer.Trim();

            var range = StringBuffer.Range.WithIndexes(3, 9);

            Assert.That(buffer.Substring(range), Is.EqualTo("lo worl"));

            range = StringBuffer.Range.WithIndexes(0, 0);
            Assert.That(buffer.Substring(range), Is.EqualTo("h"));

            range = StringBuffer.Range.Empty();
            Assert.That(buffer.Substring(range), Is.EqualTo(string.Empty));

            range = StringBuffer.Range.WithIndexes(-1, -1);
            Assert.That(buffer.Substring(range), Is.EqualTo(string.Empty));

            range = StringBuffer.Range.FromIndexWithSize(-1, 4);
            Assert.That(buffer.Substring(range), Is.EqualTo(string.Empty));
        }

        [Test] public void resize()
        {
            var str = InitBufferAndReadLine("hello world!");
            buffer.Resize(5);
            Assert.That(buffer.ToString(), Is.EqualTo("hello"));

            buffer.Resize(50);
            Assert.That(buffer.ToString(), Is.EqualTo("hello"));

            buffer.Resize(-1);
            Assert.That(buffer.ToString(), Is.EqualTo("hello"));
        }

        [Test] public void resize_between_indexes()
        {
            InitBufferAndReadLine("   hello world!");
            buffer.Trim();

            buffer.ResizeBetweenIndexes(5, 11);
            Assert.That(buffer.ToString(), Is.EqualTo(" world!"));

            buffer.ResizeBetweenIndexes(1, 2);
            Assert.That(buffer.ToString(), Is.EqualTo("wo"));

            InitBufferAndReadLine("hello world!");
            buffer.ResizeBetweenIndexes(6, 6);
            Assert.That(buffer.ToString(), Is.EqualTo("w"));
        }

        [Test] public void resize_between_invalid_indexes_does_not_changes_buffer()
        {
            InitBufferAndReadLine("hello world!");

            buffer.ResizeBetweenIndexes(9, 6);
            Assert.That(buffer.ToString(), Is.EqualTo("hello world!"));

            buffer.ResizeBetweenIndexes(-1, 11);
            Assert.That(buffer.ToString(), Is.EqualTo("hello world!"));

            buffer.ResizeBetweenIndexes(10, 211);
            Assert.That(buffer.ToString(), Is.EqualTo("hello world!"));
        }

        [Test] public void discard_changes_in_buffer()
        {
            InitBufferAndReadLine("  hello world! ");

            buffer.Trim();
            Assert.That(buffer.ToString(), Is.EqualTo("hello world!"));

            buffer.ResizeBetweenIndexes(3, 8);
            Assert.That(buffer.ToString(), Is.EqualTo("lo wor"));

            buffer.DiscardChanges();
            Assert.That(buffer.ToString(), Is.EqualTo("  hello world! "));

        }
    }
}
