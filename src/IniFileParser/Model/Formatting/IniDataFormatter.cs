using System;
using System.Collections.Generic;
using System.Text;
using IniParser.Model.Configuration;

namespace IniParser.Model.Formatting
{

    public class IniDataFormatter : IIniDataFormatter
    {

        #region Initialization
        public IniDataFormatter() : this(new IniFormattingConfiguration(new IniScheme()))
        { }

        public IniDataFormatter(IniFormattingConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            Format = configuration;
        }

        public IniDataFormatter(IniDataFormatter ori) : this(ori.Format)
        { }

        #endregion

        public virtual string IniDataToString(IniData iniData)
        {
            var sb = new StringBuilder();

            // Write global key/value data
            WriteKeyValueData(iniData.Global, sb);

            //Write sections
            foreach (SectionData section in iniData.Sections)
            {
                //Write current section
                WriteSection(section, sb);
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Configuration used to write an ini file with the proper
        ///     delimiter characters and data.
        /// </summary>
        /// <remarks>
        ///     If the <see cref="IniData"/> instance was created by a parser,
        ///     this instance is a copy of the <see cref="IniParserConfiguration"/> used
        ///     by the parser (i.e. different objects instances)
        ///     If this instance is created programatically without using a parser, this
        ///     property returns an instance of <see cref=" IniParserConfiguration"/>
        /// </remarks>
        public IniFormattingConfiguration Format
        {
            get { return _configuration; }
            set { _configuration = value.Clone(); }
        }

        IniFormattingConfiguration _configuration;

        #region Helpers

        private void WriteSection(SectionData section, StringBuilder sb)
        {
            // Write blank line before section, but not if it is the first line
            if (sb.Length > 0) sb.Append(Format.NewLineStr);

            // Leading comments
            WriteComments(section.LeadingComments, sb);

            //Write section name
            sb.Append(string.Format("{0}{1}{2}{3}",
                                    Format.Scheme.SectionStartString,
                                    section.SectionName,
                                    Format.Scheme.SectionEndString,
                                    Format.NewLineStr));

            WriteKeyValueData(section.Keys, sb);

            // Trailing comments
            WriteComments(section.TrailingComments, sb);
        }

        private void WriteKeyValueData(KeyDataCollection keyDataCollection, StringBuilder sb)
        {

            foreach (KeyData keyData in keyDataCollection)
            {
                // Add a blank line if the key value pair has comments
                if (keyData.Comments.Count > 0) sb.Append(Format.NewLineStr);

                // Write key comments
                WriteComments(keyData.Comments, sb);

                //Write key and value
                sb.Append(string.Format("{0}{3}{1}{3}{2}{4}",
                                        keyData.KeyName,
                                        Format.Scheme.KeyValueAssigmentString,
                                        keyData.Value,
                                        Format.AssigmentSpacer,
                                        Format.NewLineStr));
            }
        }

        private void WriteComments(List<string> comments, StringBuilder sb)
        {
            foreach (string comment in comments)
                sb.Append(string.Format("{0}{1}{2}", Format.Scheme.CommentString, comment, Format.NewLineStr));
        }
        #endregion

    }

}