using System;
namespace IniParser.Configuration
{
    public class IniFormattingConfiguration : IDeepCloneable<IniFormattingConfiguration>
    {
        public IniFormattingConfiguration(/*IIniScheme scheme*/)
        {
            //Scheme = scheme;

            AssigmentSpacer = " ";
            NewLineStr = Environment.NewLine;
        }

        /// <summary>
        ///     Gets or sets the string to use as new line string when formating an IniData structure using a
        ///     IIniDataFormatter. Parsing an ini-file accepts any new line character (Unix/windows)
        /// </summary>
        /// <remarks>
        ///     This allows to write a file with unix new line characters on windows (and vice versa)
        /// </remarks>
        /// <value>Defaults to value Environment.NewLine</value>
        public string NewLineStr
        {
            get; set;
        }

        /// <summary>
        ///     Sets the string around KeyValuesAssignmentChar
        /// </summary>
        /// <remarks>
        ///     Defaults to string ' '
        /// </remarks>
        public string AssigmentSpacer { get; set; }

        // TODO: Add property NewLinesBeforeSection (https://github.com/rickyah/ini-parser/issues/121)

        //public IIniScheme Scheme{ get; private set;}

        #region IDeepCloneable<T> Members
        public IniFormattingConfiguration DeepClone()
        {
            return this.MemberwiseClone() as IniFormattingConfiguration;
        }

        #endregion
    }

}
