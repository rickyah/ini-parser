using System;
namespace IniParser.Configuration
{
    public class IniFormattingConfiguration : IDeepCloneable<IniFormattingConfiguration>
    {
        public enum ENewLine
        {
            Windows,
            Unix_Mac
        }

        public IniFormattingConfiguration()
        {
            NewLineType = Environment.NewLine == "\r\n" ? ENewLine.Windows : ENewLine.Unix_Mac;
            NumSpacesBetweenAssigmentAndValue = 1;
            NumSpacesBetweenKeyAndAssigment = 1;
        }

        /// <summary>
        ///     Gets or sets the string to use as new line string when formating an IniData structure using a
        ///     IIniDataFormatter. Parsing an ini-file accepts any new line character (Unix/windows)
        /// </summary>
        /// <remarks>
        ///     This allows to write a file with unix new line characters on windows (and vice versa)
        /// </remarks>
        /// <value>Defaults to value Environment.NewLine</value>
        public string NewLineString
        {
            get
            {
                switch (NewLineType)
                {
                    case ENewLine.Unix_Mac: return "\n";
                    case ENewLine.Windows: return "\r\n";
                    default: return "\n";
                }
            }
        }

        public ENewLine NewLineType { get; set; }

        /// <summary>
        ///     In a property sets the number of spaces between the end of the key  
        ///     and the beginning of the assignment string.
        ///     0 is a valid value.
        /// </summary>
        /// <remarks>
        ///     Defaults to 1 space
        /// </remarks>
        public uint NumSpacesBetweenKeyAndAssigment
        {
            set
            {
                _numSpacesBetweenKeyAndAssigment = value;
                SpacesBetweenKeyAndAssigment = new string(' ', (int)value);
            }
        }
        public string SpacesBetweenKeyAndAssigment { get; private set; }
        /// <summary>
        ///     In a property sets the number of spaces between the end of 
        ///     the assignment string and the beginning of the value.
        ///     0 is a valid value.
        /// </summary>
        /// <remarks>
        ///     Defaults to 1 space
        /// </remarks>
        public uint NumSpacesBetweenAssigmentAndValue
        {
            set
            {
                _numSpacesBetweenAssigmentAndValue = value;
                SpacesBetweenAssigmentAndValue = new string(' ', (int)value);
            }
        }
        public string SpacesBetweenAssigmentAndValue { get; private set; }
        public bool NewLineBeforeSection { get; set; } = false;
        public bool NewLineAfterSection { get; set; } = false;
        public bool NewLineAfterProperty { get; set; } = false;
        public bool NewLineBeforeProperty { get; set; } = false;

        
        #region IDeepCloneable<T> Members
        public IniFormattingConfiguration DeepClone()
        {
            return MemberwiseClone() as IniFormattingConfiguration;
        }

        #endregion

        #region Fields
        private uint _numSpacesBetweenKeyAndAssigment;
        private uint _numSpacesBetweenAssigmentAndValue;
        #endregion
    }

}
