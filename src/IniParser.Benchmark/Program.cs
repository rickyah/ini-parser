using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

using IniParser;
using IniParser.Model;
using IniParser.Configuration;

namespace IniParser_Benchmark
{
    [MemoryDiagnoser]
    public class IniParserBenchmark 
    {

        string iniDataStr;
        IniData data = new IniData();
        IniDataParser parser = new IniDataParser();
        StringReader stringReader;
        

        [IterationSetup]
        public void Setup()
        {
            stringReader = new StringReader("");
        }
        
        [Benchmark]
        public IniDataParser TestCreatingParser() 
        {
            return new IniDataParser();
        }       
        
        [Benchmark]
        public IniData TestCreatingIniData() 
        {
            return new IniData();
        }
        
        [Benchmark]
        public IniData TestParsingIniSmall() 
        {
            return parser.Parse(stringReader, ref data);
        }
 
        [Benchmark]
        public IniData TestParserIniMedium() 
        {
            parser.Scheme.CommentString = "#";
            return parser.Parse(stringReader, ref data);
        }     
       
        [Benchmark]
        public IniData TestParserIniBig() 
        {
            parser.Scheme.CommentString= "#";
            return parser.Parse(stringReader, ref data);
        }

        [IterationSetup(Target = nameof(TestParsingIniSmall))]
        public void SetupTestParsingIniSmall()
        {
            data = new IniData();
            stringReader = new StringReader("key = value");
        }
        
        
        
        [IterationSetup(Target = nameof(TestParserIniMedium))]
        public void SetupTestParserIniMedium()
        {
            iniDataStr = @"#This section provides the general configuration of the application
[GeneralConfiguration] 

#Update rate in msecs
setUpdate = 100

#Maximun errors before quit
setMaxErrors = 2

#Users allowed to access the system
#format: user = pass
[Users]
ricky = rickypass
patty = pattypass ";
            stringReader = new StringReader(iniDataStr);
            data = new IniData();
        }
        
        
        [IterationSetup(Target = nameof(TestParserIniBig))]
        public void SetupTestParserIniBig()
        {
            iniDataStr = @"#This section provides the general configuration of the application
[GeneralConfiguration] 

#Update rate in msecs
setUpdate = 100

#Maximun errors before quit
setMaxErrors = 2

#Users allowed to access the system
#format: user = pass
[Users]
ricky = rickypass
patty = pattypass 
#This section provides the general configuration of the application
[GeneralConfiguration2] 

#Update rate in msecs
setUpdate = 100

#Maximun errors before quit
setMaxErrors = 2

#Users allowed to access the system
#format: user = pass
[Users2]
ricky = rickypass
patty = pattypass 
#This section provides the general configuration of the application
[GeneralConfiguration3] 

#Update rate in msecs
setUpdate = 100

#Maximun errors before quit
setMaxErrors = 2

#Users allowed to access the system
#format: user = pass
[Users3]
ricky = rickypass
patty = pattypass 
#This section provides the general configuration of the application
[GeneralConfiguration4] 

#Update rate in msecs
setUpdate = 100

#Maximun errors before quit
setMaxErrors = 2

#Users allowed to access the system
#format: user = pass
[Users4]
ricky = rickypass
patty = pattypass ";

            stringReader = new StringReader(iniDataStr);
            data = new IniData();
        }
                
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<IniParserBenchmark>();
            Console.WriteLine(summary);
        }
    }
}
