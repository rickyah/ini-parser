using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using IniParser.Model;
using NUnit.Framework;

namespace IniFileParser.Tests.Unit.Model
{
    [TestFixture, NUnit.Framework.Category("Test of data used to hold information retrieved for an INI file")]
    public class DataTests
    {
        [Test]
        public void check_to_string()
        {
            Data data = "data value";
            Assert.That((string)data, Is.EqualTo("data value"));
        }

        [Test]
        public void check_to_char_from_char()
        {
            Data data = 'd';
            Assert.That((char)data, Is.EqualTo('d'));
        }

        [Test]
        public void check_to_char_from_string()
        {
            Data data = "d";
            Assert.That((char)data, Is.EqualTo('d'));
        }

        [Test]
        public void check_to_int16_from_int()
        {
            Data data = 42;
            Assert.That((Int16)data, Is.EqualTo((Int16)42));
        }

        [Test]
        public void check_to_int16_from_string()
        {
            Data data = "42";
            Assert.That((Int16)data, Is.EqualTo((Int16)42));
        }

        [Test]
        public void check_to_int32_from_int()
        {
            Data data = 42;
            Assert.That((Int32)data, Is.EqualTo(42));
        }

        [Test]
        public void check_to_int32_from_string()
        {
            Data data = "42";
            Assert.That((Int32)data, Is.EqualTo(42));
        }

        [Test]
        public void check_to_int64_from_int()
        {
            Data data = 42;
            Assert.That((Int64)data, Is.EqualTo((Int64)42));
        }

        [Test]
        public void check_to_int64_from_string()
        {
            Data data = "42";
            Assert.That((Int64)data, Is.EqualTo((Int64)42));
        }

        [Test]
        public void check_to_bool_from_bool()
        {
            Data data = true;
            Assert.That((bool)data, Is.EqualTo(true));
        }

        [Test]
        public void check_to_bool_from_string()
        {
            Data data = "True";
            Assert.That((bool)data, Is.EqualTo(true));
        }

        [Test]
        public void check_to_byte_from_byte()
        {
            Data data = 42;
            Assert.That((byte)data, Is.EqualTo((byte)42));
        }

        [Test]
        public void check_to_byte_from_string()
        {
            Data data = "42";
            Assert.That((byte)data, Is.EqualTo((byte)42));
        }

        [Test]
        public void check_to_DateTime_from_DateTime()
        {
            Data data = new DateTime(2014, 06, 22, 15, 04, 33);
            Assert.That((DateTime)data, Is.EqualTo(new DateTime(2014, 06, 22, 15, 04, 33)));
        }

        [Test]
        public void check_to_DateTime_from_string()
        {
            Data data = "06/22/2014 15:04:33";
            Assert.That((DateTime)data, Is.EqualTo(new DateTime(2014, 06, 22, 15, 04, 33)));
        }

        [Test]
        public void check_to_decimal_from_decimal()
        {
            Data data = 4.2;
            Assert.That((decimal)data, Is.EqualTo((decimal)4.2));
        }

        [Test]
        public void check_to_decimal_from_string()
        {
            Data data = "4.2";
            Assert.That((decimal)data, Is.EqualTo((decimal)4.2));
        }

        [Test]
        public void check_to_double_from_double()
        {
            Data data = 4.2;
            Assert.That((double)data, Is.EqualTo(4.2));
        }

        [Test]
        public void check_to_double_from_string()
        {
            Data data = "4.2";
            Assert.That((double)data, Is.EqualTo(4.2));
        }

        [Test]
        public void check_to_single_from_single()
        {
            Data data = 4.2;
            Assert.That((float)data, Is.EqualTo((float)4.2));
        }

        [Test]
        public void check_to_single_from_string()
        {
            Data data = "4.2";
            Assert.That((float)data, Is.EqualTo((float)4.2));
        }
    }
}