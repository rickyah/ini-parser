using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.SyntaxHelpers;

using IniParser;

namespace IniParserTestNamespace
{
    [TestFixture, Category("Test of data structures used to hold information retrieved for an INI file")]
    public class 
        IniDataStructuresTest
    {

        [Test]
        public void CreateKeyDataProgramaticlyTest()
        {
            string strTest = "Test String";
            string strKeyTest = "Mykey";

            List<string> commentListTest = new List<string> ( new string [] {"testComment 1", "testComment 2" } );

            //Create a key data
            KeyData kd = new KeyData(strKeyTest);

            //Assert not null and empty
            Assert.That(kd, Is.Not.Null );
            Assert.That(kd.Comments, Is.Empty );
            Assert.That(kd.Value, Is.Empty );
            
            //Set key name
            Assert.That(kd.KeyName == strKeyTest);
            kd.KeyName = "";
            Assert.That(kd.KeyName == strKeyTest);
            kd.KeyName = strKeyTest+"_new";
            Assert.That(kd.KeyName == strKeyTest+"_new");

            //Set value data
            kd.Value = strTest;
            Assert.That(kd.Value, Is.EqualTo(strTest) );

            //Add comments
            kd.Comments.Add(strTest);
            Assert.That(kd.Comments.Count, Is.EqualTo(1));
            Assert.That(kd.Comments[0], Is.EqualTo(strTest));

            //Assign comments
            kd.Comments = commentListTest;
            Assert.That(kd.Comments, Is.Not.Null);
            Assert.That(kd.Comments, Is.Not.Empty);
            Assert.That(kd.Comments.Count, Is.EqualTo(commentListTest.Count) );
            Assert.That(kd.Comments[0], Is.EqualTo(commentListTest[0]));
            Assert.That(kd.Comments[1], Is.EqualTo(commentListTest[1]));
           
        }

        [Test]
        public void CreateSectionDataProgramaticlyTest()
        {
            string strSectionKeyTest = "MySection";
            string strKeyTest = "Mykey";
            string strValueTest = "My value";

            //Create a section data
            SectionData sd = new SectionData(strSectionKeyTest);

            Assert.That(sd.SectionName == strSectionKeyTest);

            //Assert not null and empty
            Assert.That(sd.Comments, Is.Empty );
            Assert.That(sd.Keys, Is.Empty);

            //Change name
            sd.SectionName = "";
            Assert.That(sd.SectionName == strSectionKeyTest);
            sd.SectionName = strSectionKeyTest + "_new";
            Assert.That(sd.SectionName == strSectionKeyTest+"_new");

            //Add key
            sd.Keys.AddKey(strKeyTest);
            Assert.That(sd.Keys.Count, Is.EqualTo(1));
            Assert.That(sd.Keys.ContainsKey(strKeyTest), Is.True);

            //Assign value
            sd.Keys.GetKeyData(strKeyTest).Value = strValueTest;
            Assert.That(sd.Keys.GetKeyData(strKeyTest).Value, Is.EqualTo(strValueTest));

            //Add repeated key
            sd.Keys.AddKey(strKeyTest);
            Assert.That(sd.Keys.Count, Is.EqualTo(1));

            //Remove non existing key
            sd.Keys.RemoveKey("asfd");
            Assert.That(sd.Keys.Count, Is.EqualTo(1));

            //Remove key
            sd.Keys.RemoveKey(strKeyTest);
            Assert.That(sd.Keys.Count, Is.EqualTo(0));
            Assert.That(sd.Keys.ContainsKey(strKeyTest), Is.False);

            //Add invalid key
            try
            {
                sd.Keys.AddKey("invalid key");
                Assert.That(false);
            }
            catch ( Exception ex )
            {
                Assert.That(ex, Is.TypeOf(typeof(ArgumentException)));
            }

            //Add invalid key
            try
            {
                sd.Keys.AddKey(" invalidKey");
                Assert.That(false);
            }
            catch ( Exception ex )
            {
                Assert.That(ex, Is.TypeOf(typeof(ArgumentException)));
            }

            //Access invalid keydata
            Assert.That(sd.Keys["asdf"], Is.Null);

        }

        [Test]
        public void CreateSectionDataCollectionProgramaticlyTest()
        {
            string strSectionTest = "MySection";
            string strKeyTest = "Mykey";
            string strValueTest = "My value";
            string strComment = "comment";
            List<string> commentListTest = new List<string>(new string[] { "testComment 1", "testComment 2" });


            //Creation
            SectionDataCollection sdc = new SectionDataCollection();
            Assert.That(sdc, Is.Empty);

            //Add sectoin
            sdc.AddSection(strSectionTest);
            sdc.AddSection(strSectionTest);
            Assert.That(sdc.Count, Is.EqualTo(1));


            //Check access
            Assert.That(sdc.GetSectionData(strSectionTest), Is.Not.Null);
            Assert.That(sdc.GetSectionData(strSectionTest).Comments, Is.Empty);
            Assert.That(sdc.GetSectionData(strSectionTest).Keys.Count, Is.EqualTo(0));

            //Check add coments
            sdc.GetSectionData(strSectionTest).Comments.Add(strComment);
            Assert.That(sdc.GetSectionData(strSectionTest).Comments.Count, Is.EqualTo(1));
            sdc.GetSectionData(strSectionTest).Comments = commentListTest;

            Assert.That(sdc.GetSectionData(strSectionTest).Comments.Count, Is.EqualTo(commentListTest.Count));


            //Remove section
            sdc.RemoveSection("asdf");
            Assert.That(sdc.Count, Is.EqualTo(1));

            sdc.RemoveSection(strSectionTest);
            Assert.That(sdc.Count, Is.EqualTo(0));

            //Check access
            Assert.That(sdc[strSectionTest], Is.Null);
        }
    }
}
