using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace IniParser
{
    /// <summary>
    /// Represents an INI data parser for streams.
    /// </summary>
    public class StreamIniDataParser
    {
        #region Default regular expression strings

        private const string strCommentRegex = @".*";
        private const string strSectionRegexStart = @"^(\s*?)";
        private const string strSectionRegexMiddle = @"{1}\s*[_\{\}\#\+\;\%\(\)\=\?\&\$\,\:\/\.\-\w\d\s]+\s*"; //@"{1}\s*[_\#\+\;\%\(\)\=\?\&\$\,\:\/\.\-\w\d\s]+\s*";
        private const string strSectionRegexEnd = @"(\s*?)$";
        private const string strKeyRegex = @"^(\s*[_\.\d\w]*\s*)";
        private const string strValueRegex = @"([\s\d\w\W\.]*)$";
        private const string strSpecialRegexChars = @"[\^$.|?*+()";
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamIniDataParser"/> class.
        /// </summary>
        /// <remarks>
        /// By default the various delimiters for the data are setted:
        /// <para>';' for one-line comments</para>
        /// <para>'[' ']' for delimiting a section</para>
        /// <para>'=' for linking key value pairs</para>
        /// <example>
        /// An example of well formed data with the default values:
        /// <para>
        /// ;section comment<br/>
        /// [section] ; section comment<br/>
        /// <br/>
        /// ; key comment<br/>
        /// key = value ;key comment<br/>
        /// <br/>
        /// ;key2 comment<br/>
        /// key2 = value<br/>
        /// </para>
        /// </example>
        /// </remarks>
        public StreamIniDataParser()
        {
            //Default delimiter values
            CommentDelimiter = ';';
            KeyValueDelimiter = '=';
            SectionDelimiters = new char[] { '[', ']' };

            _currentTmpData = new SectionDataCollection();
            
        }

        /// <summary>
        /// <para>Reads data in INI format from a stream.</para> 
        /// </summary>
        /// <param name="reader">Reader stream.</param>
        /// <returns></returns>
        public IniData ReadData(StreamReader reader)
        {
            return ReadData(reader, false);
        }

        /// <summary>
        /// <para>Reads data in INI format from a stream.</para> 
        /// </summary>
        /// <param name="reader">Reader stream.</param>
        /// <param name="relaxedIniFormat">
        ///     True to allow loading an IniFile with non-unique section or key, in which
        ///     case the repeating values are discarded
        /// </param>
        /// <returns></returns>
        public IniData ReadData(StreamReader reader, bool relaxedIniFormat)
        {
            _relaxedIniFormat = relaxedIniFormat;

            if (reader == null)
                throw new ArgumentNullException("reader", "The StreamReader object is null");

            if (relaxedIniFormat)
            {
                _currentTmpData.AddSection(IniData.GlobalSectionName);
            }

            try
            {
                while (!reader.EndOfStream)
                    ProcessLine(reader.ReadLine());

                return new IniData((SectionDataCollection)_currentTmpData.Clone());
            }
            catch (Exception ex)
            {
                throw new ParsingException("Parsing error: " + ex.Message, ex);
            }
            finally
            {
                _currentTmpData.Clear();
            }
        }

        /// <summary>
        /// Writes the ini data to a stream.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="iniData">An <see cref="IniData"/> instance .</param>
        public void WriteData(StreamWriter writer, IniData iniData)
        {

            SectionDataCollection sdc = iniData.Sections;


            //Write sections
            foreach (SectionData sd in sdc)
            {
                //Write section comments
                foreach (string sectionComment in sd.Comments)
                    writer.WriteLine(string.Format("{0}{1}", CommentDelimiter, sectionComment));

                //Write section name
                writer.WriteLine(string.Format(
                    "{0}{1}{2}",
                    SectionDelimiters[0], sd.SectionName, SectionDelimiters[1]));

                //Write section keys
                foreach (KeyData kd in sd.Keys)
                {
                    //Write key comments
                    foreach (string keyComment in kd.Comments)
                        writer.WriteLine(string.Format("{0}{1}", CommentDelimiter, keyComment));

                    //Write key and value
                    writer.WriteLine(string.Format(
                        "{0} {1} {2}",
                        kd.KeyName, KeyValueDelimiter, kd.Value));
                }

                writer.WriteLine();     //blank line

            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the comment delimiter.
        /// </summary>
        /// <value>The comment delimiter.</value>
        public char CommentDelimiter
        {
            get { return _commentDelimiter; }
            set
            {
                _commentRegex = new Regex(value + strCommentRegex);
                _commentDelimiter = value;
            }
        }

        /// <summary>
        /// Gets or sets the section delimiters.
        /// </summary>
        /// <value>The section delimiters.</value>
        public char[] SectionDelimiters
        {
            get { return _sectionDelimiters; }
            set
            {
                if (value == null || value.Length != 2)
                    return;

                string tmp = strSectionRegexStart;
                if (strSpecialRegexChars.Contains(new string(value[0], 1)))
                    tmp += "\\" + value[0];
                else tmp += value[0];

                tmp += strSectionRegexMiddle;

                if (strSpecialRegexChars.Contains(new string(value[1], 1)))
                    tmp += "\\" + value[1];
                else
                    tmp += value[1];

                tmp += strSectionRegexEnd;

                _sectionRegex = new Regex(tmp);
                _sectionDelimiters = value;
            }
        }

        /// <summary>
        /// Gets or sets the key value delimiter.
        /// </summary>
        /// <value>The key value delimiter.</value>
        public char KeyValueDelimiter
        {
            get { return _keyValueDelimiter; }
            set
            {
                _keyValuePairRegex = new Regex(strKeyRegex + value + strValueRegex);
                _keyValueDelimiter = value;
            }
        }

        /// <summary>
        /// Gets or sets the regular expression for matching
        /// a comment substring.
        /// </summary>
        /// <value>A string containing the regular expression.</value>
        public string CommentRegexString
        {
            get
            {
                if (_commentRegex == null)
                    return string.Empty;

                return _commentRegex.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the regular expression for matching
        /// a section substring.
        /// </summary>
        /// <value>A string containing the regular expression.</value>
        public string SectionRegexString
        {
            get
            {
                if (_sectionRegex == null)
                    return string.Empty;

                return _sectionRegex.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the regular expression for matching a
        /// key/value pair substring.
        /// </summary>
        /// <value>A string containing the regular expression.</value>
        public string KeyValuePairRegexString
        {
            get
            {
                if (_keyValuePairRegex == null)
                    return string.Empty;
                return _keyValuePairRegex.ToString();
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Checks if a given string contains a comment.
        /// </summary>
        /// <param name="s">The string to be checked.</param>
        /// <returns>
        /// <c>true</c> if any substring from s is a comment, <c>false</c> otherwise.
        /// </returns>
        private bool MatchComment(string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;

            return _commentRegex.Match(s).Success;
        }

        /// <summary>
        /// Checks if a given string represents a section delimiter.
        /// </summary>
        /// <param name="s">The string to be checked.</param>
        /// <returns>
        /// <c>true</c> if the string represents a section, <c>false</c> otherwise.
        /// </returns>
        private bool MatchSection(string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;

            return _sectionRegex.Match(s).Success;
        }

        /// <summary>
        /// Checks if a given string represents a key / value pair.
        /// </summary>
        /// <param name="s">The string to be checked.</param>
        /// <returns>
        /// <c>true</c> if the string represents a key / value pair, <c>false</c> otherwise.
        /// </returns>
        private bool MatchKeyValuePair(string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;

            return s.Contains(KeyValueDelimiter.ToString());
        }

        /// <summary>
        /// Removes a comment from a string if exist, and returns the string without
        /// the comment substring.
        /// </summary>
        /// <param name="s">The string we want to remove the comments from.</param>
        /// <returns>The string s without comments</returns>
        private string ExtractComment(string s)
        {
            string tmp = _commentRegex.Match(s).Value.Trim();

            _currentCommentList.Add(tmp.Substring(1, tmp.Length - 1));

            return s.Replace(tmp, "").Trim();
        }

        /// <summary>
        /// Processes the line.
        /// </summary>
        /// <param name="currentLine">The current line.</param>
        private void ProcessLine(string currentLine)
        {
            currentLine = currentLine.Trim();

            //Extract comments
            if (MatchComment(currentLine))
                currentLine = ExtractComment(currentLine);

            //Ignore empty lines
            if (currentLine == String.Empty)
                return;

            //Process sections
            if (MatchSection(currentLine))
            {
                ProcessSection(currentLine);
                return;
            }

            //Process keys
            if (MatchKeyValuePair(currentLine))
            {
                ProcessKeyValuePair(currentLine);
                return;
            }

            throw new ParsingException(
                "Couldn't parse string: " + currentLine + ". Unknown file format.");
        }

        /// <summary>
        /// Proccess a string defining a new section.
        /// </summary>
        /// <param name="s">The string to be processed</param>
        private void ProcessSection(string s)
        {
            OneSectionWasAlreadyProcessed = true;

            string tmp = _sectionRegex.Match(s).Value.Trim();

            //Check bad string
            if (tmp == string.Empty)
                throw new ParsingException("Error parsing section. String: \"" + s + "\"");

            //Remove section delimiters
            tmp = tmp.Substring(1, tmp.Length - 2).Trim();

            //Checks correct ini format
            if (_currentTmpData.ContainsSection(tmp))
            {
                if (_relaxedIniFormat)
                {
                    //OneSectionWasAlreadyProcessed = false;
                    _currentSectionName = tmp;
                    return;
                }

                throw new ParsingException(string.Format(
                    "Error parsing section: Another section with the name [{0}] exists!", s));
            }


            _currentSectionName = tmp;

            //Add new section & comments
            _currentTmpData.AddSection(tmp);
            _currentTmpData.GetSectionData(tmp).Comments = _currentCommentList;
            _currentCommentList.Clear();


        }

        private bool OneSectionWasAlreadyProcessed
        {
            get { return _oneSecion; }
            set { _oneSecion = value; }
        }

        /// <summary>
        /// Processes a string containing a key/value pair.
        /// </summary>
        /// <param name="s">The string to be processed</param>
        private void ProcessKeyValuePair(string s)
        {
            string sectionToUse = _currentSectionName;

            if (!OneSectionWasAlreadyProcessed)
            {
                if (_relaxedIniFormat)
                {
                    sectionToUse = IniData.GlobalSectionName;
                }
                else
                {
                    return;
                }
            } 

            if (sectionToUse == string.Empty)
            {
                throw new ParsingException("Bad file format: key doesn't match any section. String :" + s);
            }


            string key = ExtractKey(s);
            string value = ExtractValue(s);


            //Checks correct ini format
            if (_currentTmpData.GetSectionData(sectionToUse).Keys.ContainsKey(key))
            {
                if (_relaxedIniFormat)
                {
                    return;
                }

                throw new ParsingException(
                    string.Format(
                        "Error parsing section: Another key with the same name [{0}] already exists in section",
                        sectionToUse));
            }


            _currentTmpData.GetSectionData(sectionToUse).Keys.AddKey(key);
            _currentTmpData.GetSectionData(sectionToUse).Keys.GetKeyData(key).Value = value;
            _currentTmpData.GetSectionData(sectionToUse).Keys.GetKeyData(key).Comments = _currentCommentList;
            _currentCommentList.Clear();

        }
		
		/// <summary>
		/// 	Gets or sets a value indicating whether one section has been processed.
		/// </summary>
        protected bool OneSectionHasBeenProcessed
        {
            get;
            set;
        }

        /// <summary>
        /// Extracts the key portion of a string containing a key/value pair..
        /// </summary>
        /// <param name="s">The string to be processed, which contains a key/value pair</param>
        /// <returns>The name of the extracted key.</returns>
        private string ExtractKey(string s)
        {
            //string tmp = _keyValuePairRegex.Match(s).Value;
            //if (tmp == string.Empty)
            //    throw new ParsingException("Error extracting key. String: \"" + s + "\"");

            int index = s.IndexOf(_keyValueDelimiter, 0);

            return s.Substring(0, index).Trim();
        }

        /// <summary>
        /// Extracts the value portion of a string containing a key/value pair..
        /// </summary>
        /// <param name="s">The string to be processed, which contains a key/value pair</param>
        /// <returns>The name of the extracted value.</returns>
        private string ExtractValue(string s)
        {
            //string tmp = _keyValuePairRegex.Match(s).Value.Trim();
            //if (tmp == string.Empty)
            //    throw new ParsingException("Error extracting value. String: \"" + s + "\"");

            int index = s.IndexOf(_keyValueDelimiter, 0);

            return s.Substring(index + 1, s.Length - index - 1).Trim();
        }

        #endregion

        #region Fields

        /// <summary>
        /// Temp list of comments
        /// </summary>
        private readonly List<string> _currentCommentList = new List<string>();

        /// <summary>
        /// Tmp var with the name of the seccion which is being process
        /// </summary>
        private string _currentSectionName;

        /// <summary>
        /// Temporary data for the parsing
        /// </summary>
        private readonly SectionDataCollection _currentTmpData;

        /// <summary>
        /// Defines the character used as comment delimiter.
        /// </summary>
        private char _commentDelimiter;

        /// <summary>
        /// Defines the two characters used as section delimiters.
        /// </summary>
        private char[] _sectionDelimiters;

        /// <summary>
        /// Defines the character used as a key value pair link
        /// </summary>
        private char _keyValueDelimiter;

        /// <summary>
        /// Regular expression for matching a comment string
        /// </summary>
        private Regex _commentRegex;

        /// <summary>
        /// Regular expression for matching a section string
        /// </summary>
        private Regex _sectionRegex;

        /// <summary>
        /// Regular expression for matching a key / value pair string
        /// </summary>
        private Regex _keyValuePairRegex;

        /// <summary>
        ///     True to allow loading an IniFile with non-unique section or key
        /// </summary>
        private bool _relaxedIniFormat;

        private bool _oneSecion;

        
        #endregion
    }

}
