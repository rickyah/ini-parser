using System;
using System.Collections.Generic;
using IniParser.Exceptions;
using IniParser.Model;
using IniParser.Parser.Configurations;

namespace IniParser.Parser
{
    public class IniDataParser
    {
        public IniDataParser() : this(new DefaultParserConfiguration()) 
        {}

        public IniDataParser(IParserConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            Configuration = configuration;
        }

        public IParserConfiguration Configuration { get; set; }

        public IniData Parse(string iniDataString)
        {

            if (string.IsNullOrEmpty(iniDataString))
            {
                if(Configuration.ThrowExceptionsOnError)
                    throw new ArgumentException("iniDataString", "data string is null or empty");

                return null;
            }


            IniData iniData = new IniData();
            _currentCommentListTemp.Clear();
            _currentSectionNameTemp = null;

            try
            {
                foreach (var line in iniDataString.Split(Environment.NewLine.ToCharArray()))
                    ProcessLine(line, iniData);
            }
            catch
            {
                if (Configuration.ThrowExceptionsOnError)
                    throw;

                return null;
            }

            return (IniData)iniData.Clone();
        }

        #region Helpers

        #region Matchers
        
        
        /// <summary>
        /// Checks if a given string contains a comment.
        /// </summary>
        /// <param name="line">The string to be checked.</param>
        /// <returns>
        /// <c>true</c> if any substring from s is a comment, <c>false</c> otherwise.
        /// </returns>
        private bool LineContainsAComment(string line)
        {
            return !string.IsNullOrEmpty(line) && Configuration.CommentRegex.Match(line).Success;
        }

        /// <summary>
        /// Checks if a given string represents a section delimiter.
        /// </summary>
        /// <param name="line">The string to be checked.</param>
        /// <returns>
        /// <c>true</c> if the string represents a section, <c>false</c> otherwise.
        /// </returns>
        private bool LineMatchesASection(string line)
        {
            return !string.IsNullOrEmpty(line) && Configuration.SectionRegex.Match(line).Success;
        }

        /// <summary>
        /// Checks if a given string represents a key / value pair.
        /// </summary>
        /// <param name="s">The string to be checked.</param>
        /// <returns>
        /// <c>true</c> if the string represents a key / value pair, <c>false</c> otherwise.
        /// </returns>
        private bool LineMatchesAKeyValuePair(string line)
        {
            return !string.IsNullOrEmpty(line) && line.Contains(Configuration.KeyValueAssigmentChar.ToString());
        }

        #endregion

        /// <summary>
        /// Removes a comment from a string if exist, and returns the string without
        /// the comment substring.
        /// </summary>
        /// <param name="line">The string we want to remove the comments from.</param>
        /// <returns>The string s without comments</returns>
        private string ExtractComment(string line)
        {
            string comment = Configuration.CommentRegex.Match(line).Value.Trim();

            _currentCommentListTemp.Add(comment.Substring(1, comment.Length - 1));

            return line.Replace(comment, "").Trim();
        }

        /// <summary>
        /// Processes one line and parses INI data.
        /// </summary>
        /// <param name="currentLine">The current line.</param>
        private void ProcessLine(string currentLine, IniData currentIniData)
        {
            currentLine = currentLine.Trim();

            // Extract comments from current line and store them in a tmp field
            if (LineContainsAComment(currentLine))
                currentLine = ExtractComment(currentLine);

            // If the entire line is a comment now should be empty,
            // so no further processing is needed.
            if (currentLine == String.Empty)
                return;

            //Process sections
            if (LineMatchesASection(currentLine))
            {
                ProcessSection(currentLine, currentIniData);
                return;
            }

            //Process keys
            if (LineMatchesAKeyValuePair(currentLine))
            {
                ProcessKeyValuePair(currentLine, currentIniData);
                return;
            }

            throw new ParsingException(
                "Unknown file format. Couldn't parse the line: '" + currentLine + "'.");
        }

        /// <summary>
        /// Proccess a string defining a new section.
        /// </summary>
        /// <param name="line">The string to be processed</param>
        private void ProcessSection(string line, IniData currentIniData)
        {
            // Get section name with delimiters from line...
            string sectionName = Configuration.SectionRegex.Match(line).Value.Trim();

            // ... and remove section's delimiters to get just the name
            sectionName = sectionName.Substring(1, sectionName.Length - 2).Trim();

            // Check that the section's name is not empty
            if (sectionName == string.Empty)
            {
                throw new ParsingException("Section name is empty");
            }

            // Temporally save section name.
            _currentSectionNameTemp = sectionName;

            //Checks if the section already exists
            if (currentIniData.Sections.ContainsSection(sectionName))
            {
                if (Configuration.AllowDuplicateSections)
                {
                    return;
                }

                throw new ParsingException(string.Format("Duplicate section with name '{0}' on line '{1}'", sectionName, line));
            }


            // If the section does not exists, add it to the ini data
            currentIniData.Sections.AddSection(sectionName);
            
            // Save comments read until now and assign them to this section
            currentIniData.Sections.GetSectionData(sectionName).Comments = _currentCommentListTemp;
            _currentCommentListTemp.Clear();
            
        }

        /// <summary>
        /// Processes a string containing a key/value pair.
        /// </summary>
        /// <param name="line">The string to be processed</param>
        private void ProcessKeyValuePair(string line, IniData currentIniData)
        {
            //string sectionToUse = _currentSectionNameTemp;

            // get key and value data
            string key = ExtractKey(line);
            string value = ExtractValue(line);

            // Check if we haven't read any section yet
            if (string.IsNullOrEmpty(_currentSectionNameTemp))
            {
                if (!Configuration.AllowKeysWithoutSection)
                {
                    throw new ParsingException("key value pairs must be enclosed in a section");
                }

                AddKeyToKeyValueCollection(key, value, currentIniData.Global, "global");
            }
            else
            {
                var currentSection = currentIniData.Sections.GetSectionData(_currentSectionNameTemp);

                AddKeyToKeyValueCollection(key, value, currentSection.Keys, _currentSectionNameTemp);
            }
        }

        private void AddKeyToKeyValueCollection(string key, string value, KeyDataCollection keyDataCollection, string sectionName)
        {
            // Check for duplicated keys
            if (keyDataCollection.ContainsKey(key))
            {
                if (!Configuration.AllowDuplicateKeys)
                {
                    throw new ParsingException(string.Format("Duplicated key '{0}' found in section '{1}", key, sectionName));
                }
            }

            // Save the keys
            keyDataCollection.AddKey(key, value);
            keyDataCollection.GetKeyData(key).Comments = _currentCommentListTemp;
            _currentCommentListTemp.Clear();
        }

        /// <summary>
        /// Extracts the key portion of a string containing a key/value pair..
        /// </summary>
        /// <param name="s">The string to be processed, which contains a key/value pair</param>
        /// <returns>The name of the extracted key.</returns>
        private string ExtractKey(string s)
        {
            int index = s.IndexOf(Configuration.KeyValueAssigmentChar, 0);

            return s.Substring(0, index).Trim();
        }

        /// <summary>
        /// Extracts the value portion of a string containing a key/value pair..
        /// </summary>
        /// <param name="s">The string to be processed, which contains a key/value pair</param>
        /// <returns>The name of the extracted value.</returns>
        private string ExtractValue(string s)
        {
            int index = s.IndexOf(Configuration.KeyValueAssigmentChar, 0);

            return s.Substring(index + 1, s.Length - index - 1).Trim();
        }

        #endregion

        #region Fields
        
        /// <summary>
        /// Temp list of comments
        /// </summary>
        private readonly List<string> _currentCommentListTemp = new List<string>();

        /// <summary>
        /// Tmp var with the name of the seccion which is being process
        /// </summary>
        private string _currentSectionNameTemp;
        #endregion
    }
}
