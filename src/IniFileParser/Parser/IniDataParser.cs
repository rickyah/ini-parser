using System;
using System.Collections.Generic;
using IniParser.Exceptions;
using IniParser.Model;
using IniParser.Model.Configuration;

namespace IniParser.Parser
{
	/// <summary>
	/// 	Responsible for parsing an string from an ini file, and creating
	/// 	an <see cref="IniData"/> structure.
	/// </summary>
    public class IniDataParser
    {
        #region Initialization
        /// <summary>
        ///     Ctor
        /// </summary>
        /// <remarks>
        ///     The parser uses a <see cref="DefaultIniParserConfiguration"/> by default
        /// </remarks>
        public IniDataParser()
            : this(new DefaultIniParserConfiguration())
        { }

        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="parserConfiguration">
        ///     Parser's <see cref="IIniParserConfiguration"/> instance.
        /// </param>
        public IniDataParser(IIniParserConfiguration parserConfiguration)
        {
            if (parserConfiguration == null)
                throw new ArgumentNullException("parserConfiguration");

            Configuration = parserConfiguration;
        }

        #endregion

        #region State
        /// <summary>
        ///     Configuration that defines the behaviour and constraints
        ///     that the parser must follow.
        /// </summary>
        public IIniParserConfiguration Configuration { get; private set; }
		#endregion

		#region Operations
        /// <summary>
        ///     Parses a string containing valid ini data
        /// </summary>
        /// <param name="iniDataString">
        ///     String with data
        /// </param>
        /// <returns>
        ///     An <see cref="IniData"/> instance with the data contained in
        ///     the <paramref name="iniDataString"/> correctly parsed an structured.
        /// </returns>
        /// <exception cref="ParsingException">
        ///     Thrown if the data could not be parsed
        /// </exception>
        public IniData Parse(string iniDataString)
        {
            IniData iniData = new IniData();
            iniData.Configuration = this.Configuration.Clone();

            if (string.IsNullOrEmpty(iniDataString))
            {
                return iniData;
            }

            _currentCommentListTemp.Clear();
            _currentSectionNameTemp = null;

            try
            {
                foreach (var line in iniDataString.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
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
        #endregion

        #region Template Method Design Pattern 
        // All this methods controls the parsing behaviour, so it can be modified 
        // in derived classes.
        // See http://www.dofactory.com/Patterns/PatternTemplate.aspx for an explanation of this pattern.
        // Probably for the most common cases you can change the parsing behavior using a custom configuration
        // object rather than creating derived classes.
        // See IIniParserConfiguration interface, and IniDataParser constructor to change the default
		// configuration.

        /// <summary>
        ///     Checks if a given string contains a comment.
        /// </summary>
        /// <param name="line">
        ///     The string to be checked.
        /// </param>
        /// <returns>
        ///     <c>true</c> if any substring from s is a comment, <c>false</c> otherwise.
        /// </returns>
        protected bool LineContainsAComment(string line)
        {
            return !string.IsNullOrEmpty(line) && Configuration.CommentRegex.Match(line).Success;
        }

        /// <summary>
        ///     Checks if a given string represents a section delimiter.
        /// </summary>
        /// <param name="line">
        ///     The string to be checked.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the string represents a section, <c>false</c> otherwise.
        /// </returns>
        protected bool LineMatchesASection(string line)
        {
            return !string.IsNullOrEmpty(line) && Configuration.SectionRegex.Match(line).Success;
        }

        /// <summary>
        ///     Checks if a given string represents a key / value pair.
        /// </summary>
        /// <param name="line">
        ///     The string to be checked.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the string represents a key / value pair, <c>false</c> otherwise.
        /// </returns>
        protected bool LineMatchesAKeyValuePair(string line)
        {
            return !string.IsNullOrEmpty(line) && line.Contains(Configuration.KeyValueAssigmentChar.ToString());
        }

        /// <summary>
        ///     Removes a comment from a string if exist, and returns the string without
        ///     the comment substring.
        /// </summary>
        /// <param name="line">
        ///     The string we want to remove the comments from.
        /// </param>
        /// <returns>
        ///     The string s without comments.
        /// </returns>
        protected string ExtractComment(string line)
        {
            string comment = Configuration.CommentRegex.Match(line).Value.Trim();

            _currentCommentListTemp.Add(comment.Substring(1, comment.Length - 1));

            return line.Replace(comment, "").Trim();
        }

        /// <summary>
        ///     Processes one line and parses the data found in that line
        ///     (section or key/value pair who may or may not have comments)
        /// </summary>
        /// <param name="currentLine">The string with the line to process</param>
        protected void ProcessLine(string currentLine, IniData currentIniData)
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

            if (Configuration.SkipInvalidLines)
                return;

            throw new ParsingException(
                "Unknown file format. Couldn't parse the line: '" + currentLine + "'.");
        }

        /// <summary>
        ///     Proccess a string which contains an ini section.
        /// </summary>
        /// <param name="line">
        ///     The string to be processed
        /// </param>
        protected void ProcessSection(string line, IniData currentIniData)
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
        ///     Processes a string containing an ini key/value pair.
        /// </summary>
        /// <param name="line">
        ///     The string to be processed
        /// </param>
        protected void ProcessKeyValuePair(string line, IniData currentIniData)
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

        /// <summary>
        ///     Extracts the key portion of a string containing a key/value pair..
        /// </summary>
        /// <param name="s">    
        ///     The string to be processed, which contains a key/value pair
        /// </param>
        /// <returns>
        ///     The name of the extracted key.
        /// </returns>
        protected string ExtractKey(string s)
        {
            int index = s.IndexOf(Configuration.KeyValueAssigmentChar, 0);

            return s.Substring(0, index).Trim();
        }

        /// <summary>
        ///     Extracts the value portion of a string containing a key/value pair..
        /// </summary>
        /// <param name="s">
        ///     The string to be processed, which contains a key/value pair
        /// </param>
        /// <returns>
        ///     The name of the extracted value.
        /// </returns>
        protected string ExtractValue(string s)
        {
            int index = s.IndexOf(Configuration.KeyValueAssigmentChar, 0);

            return s.Substring(index + 1, s.Length - index - 1).Trim();
        }

        #endregion

        #region Helpers
        /// <summary>
        ///     Adds a key to a concrete <see cref="KeyDataCollection"/> instance, checking
        ///     if duplicate keys are allowed in the configuration
        /// </summary>
        /// <param name="key">
        ///     Key name
        /// </param>
        /// <param name="value">
        ///     Key's value
        /// </param>
        /// <param name="keyDataCollection">
        ///     <see cref="KeyData"/> collection where the key should be inserted
        /// </param>
        /// <param name="sectionName">
        ///     Name of the section where the <see cref="KeyDataCollection"/> is contained. 
        ///     Used only for logging purposes.
        /// </param>
        private void AddKeyToKeyValueCollection(string key, string value, KeyDataCollection keyDataCollection, string sectionName)
        {
            // Check for duplicated keys
            if (keyDataCollection.ContainsKey(key))
            {
                // We already have a key with the same name defined in the current section

                if (!Configuration.AllowDuplicateKeys)
                {
                    throw new ParsingException(string.Format("Duplicated key '{0}' found in section '{1}", key, sectionName));
                }
                else if(Configuration.OverrideDuplicateKeys)
                {
                    keyDataCollection[key] = value;
                }
            }
            else
            {
                // Save the keys
                keyDataCollection.AddKey(key, value);
            }

            keyDataCollection.GetKeyData(key).Comments = _currentCommentListTemp;
            _currentCommentListTemp.Clear();
        }
        #endregion

        #region Fields

        /// <summary>
        ///     Temp list of comments
        /// </summary>
        private readonly List<string> _currentCommentListTemp = new List<string>();

        /// <summary>
        ///     Tmp var with the name of the seccion which is being process
        /// </summary>
        private string _currentSectionNameTemp;
        #endregion
    }
}
