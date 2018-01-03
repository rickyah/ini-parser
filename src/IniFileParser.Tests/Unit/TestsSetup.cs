using System.IO;
using NUnit.Framework;

namespace IniParser.Tests.Unit
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
