using System.IO;
using NUnit.Framework;

namespace IniFileParser.Tests
{
    [SetUpFixture]
    public class TestsSetup
    {
        [OneTimeSetUp]
        public void SetWorkingDirForTest()
        {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
        }
    }
}
