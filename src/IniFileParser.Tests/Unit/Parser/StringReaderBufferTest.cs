using NUnit.Framework;
using IniParser.Parser;
using System.IO;

namespace IniFileParser.Tests
{
    [TestFixture]
    public class StringReaderBufferTest
    {
        StringReadBuffer buffer;

        private string InitBufferAndReadLine(string str)
        {
            buffer.Reset(new StringReader(str));
            Assert.That(buffer.ReadLine(), Is.True);
            return str;
        }

        [SetUp] public void Setup()
        {
            buffer = new StringReadBuffer(256);
        }

        [Test] public void check_default_initialization()
        {
            var emptyBuffer = new StringReadBuffer(0);

            Assert.That(emptyBuffer.ToString(), Is.EqualTo(string.Empty));
            Assert.That(emptyBuffer.Count, Is.EqualTo(0));
            Assert.That(emptyBuffer.ReadLine(), Is.False);
        }

        [Test] public void check_default_reset()
        {
            buffer = new StringReadBuffer(0);

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
            var str = InitBufferAndReadLine("hello world!");

            var result = buffer.FindSubstring("world");
            Assert.That(result.start, Is.EqualTo(6));
            Assert.That(result.size, Is.EqualTo(5));

            result = buffer.FindSubstring("");
            Assert.That(result.start, Is.EqualTo(0));
            Assert.That(result.size, Is.EqualTo(0));


            result = buffer.FindSubstring("d!");
            Assert.That(result.start, Is.EqualTo(10));
            Assert.That(result.size, Is.EqualTo(2));
        }


        [Test] public void substring()
        {
            var str = InitBufferAndReadLine("hello world!");
            var range = StringReadBuffer.Range.WithIndexes(3, 9);
            Assert.That(buffer.Substring(range), Is.EqualTo("lo worl"));

            range = StringReadBuffer.Range.WithIndexes(0, 0);
            Assert.That(buffer.Substring(range), Is.EqualTo("h"));

            range = StringReadBuffer.Range.Empty();
            Assert.That(buffer.Substring(range), Is.EqualTo(string.Empty));

            range = StringReadBuffer.Range.WithIndexes(-1, -1);
            Assert.That(buffer.Substring(range), Is.EqualTo(string.Empty));

            range = StringReadBuffer.Range.FromIndexWithSize(-1, 4);
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
            var str = InitBufferAndReadLine("   hello world!");
            buffer.Trim();


            buffer.ResizeBetweenIndexes(6, 6);
            Assert.That(buffer.ToString(), Is.EqualTo("hello world!"));

            buffer.ResizeBetweenIndexes(9, 6);
            Assert.That(buffer.ToString(), Is.EqualTo("hello world!"));

            buffer.ResizeBetweenIndexes(5, 11);
            Assert.That(buffer.ToString(), Is.EqualTo(" world"));

            buffer.ResizeBetweenIndexes(-1, 11);
            Assert.That(buffer.ToString(), Is.EqualTo(" world"));

            buffer.ResizeBetweenIndexes(10, 211);
            Assert.That(buffer.ToString(), Is.EqualTo(" world"));

            buffer.ResizeBetweenIndexes(1, 2);
            Assert.That(buffer.ToString(), Is.EqualTo("w"));

        }
    }

}
