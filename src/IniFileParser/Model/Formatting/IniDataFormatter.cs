using System;
using System.Collections.Generic;
using System.Text;
using IniParser.Model.Configuration;

namespace IniParser.Model.Formatting
{

    public class IniDataFormatter : IIniDataFormatter
    {
        public string Format(IniData iniData, IniFormattingConfiguration format)
        {
            var sb = new StringBuilder();

            // Write global key/value data
            WriteKeyValueData(iniData.Global, sb, iniData.Scheme, format);

            //Write sections
            foreach (var section in iniData.Sections)
            {
                //Write current section
                WriteSection(section, sb, iniData.Scheme, format);
            }

            return sb.ToString();
        }

        #region Helpers

        private void WriteSection(Section section,
                                  StringBuilder sb,
                                  IIniScheme scheme,
                                  IniFormattingConfiguration format)
        {
            // Write blank line before section, but not if it is the first line
            if (sb.Length > 0) sb.Append(format.NewLineStr);

            // Leading comments
            WriteComments(section.Comments, sb, scheme, format);

            //Write section name
            sb.Append(string.Format("{0}{1}{2}{3}",
                                    scheme.SectionStartString,
                                    section.SectionName,
                                    scheme.SectionEndString,
                                    format.NewLineStr));

            WriteKeyValueData(section.Keys, sb, scheme, format);

            // Trailing comments
            WriteComments(section.TrailingComments, sb, scheme, format);
        }

        private void WriteKeyValueData(KeyDataCollection keyDataCollection,
                                       StringBuilder sb,
                                       IIniScheme scheme,
                                       IniFormattingConfiguration format)
        {

            foreach (KeyData keyData in keyDataCollection)
            {
                // Add a blank line if the key value pair has comments
                if (keyData.Comments.Count > 0) sb.Append(format.NewLineStr);

                // Write key comments
                WriteComments(keyData.Comments, sb, scheme, format);

                //Write key and value
                sb.Append(string.Format("{0}{3}{1}{3}{2}{4}",
                                        keyData.KeyName,
                                        scheme.KeyValueAssigmentString,
                                        keyData.Value,
                                        format.AssigmentSpacer,
                                        format.NewLineStr));
            }
        }

        private void WriteComments(List<string> comments,
                                   StringBuilder sb,
                                   IIniScheme scheme,
                                   IniFormattingConfiguration format)
        {
            foreach (string comment in comments)
                sb.Append(string.Format("{0}{1}{2}",
                                        scheme.CommentString,
                                        comment,
                                        format.NewLineStr));
        }



        #endregion

    }

}