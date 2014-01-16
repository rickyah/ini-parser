using NUnit.Framework;
using System;
using IniParser.Model;

namespace IniFileParser.Tests.Unit.Model
{
    [TestFixture()]
    public class IniDataTests
    {
        [Test()]
        public void CreateIniFileProgramatically()
        {
			var iniData = new IniData();
			iniData.Global.AddKey("UseSeparateRepositoryForAssets", true.ToString());
			
			iniData.Sections.AddSection("MainRepository");
			iniData["MainRepository"]["Type"] = "git";
			iniData["MainRepository"]["RelativePath"] = ".";
			
			Assert.That(iniData["MainRepository"].ContainsKey("Type"));
			Assert.That(iniData["MainRepository"].ContainsKey("RelativePath"));
			
			iniData.Sections.AddSection("AssetsRepository");
			iniData["AssetsRepository"]["Type"] = "svn";
			iniData["AssetsRepository"]["RelativePath"] = "./Assets";
			
			Assert.That(iniData["AssetsRepository"].ContainsKey("Type"));
			Assert.That(iniData["AssetsRepository"].ContainsKey("RelativePath"));
			
			Console.WriteLine(iniData.ToString());
        }
    }
}

