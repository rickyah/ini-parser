using System;
using System.Collections.Generic;
using IniParser.Exceptions;
using IniParser.Model;
using IniParser.Model.Configuration;
using System.Collections.ObjectModel;
using System.IO;
using static IniParser.Parser.StringBuffer;

namespace IniParser.Parser
{
    /// <summary>
	/// 	Responsible for parsing an string from an ini file, and creating
	/// 	an <see cref="IniData"/> structure.
	/// </summary>
    public partial class IniDataParser
    {
        #region Initialization
        /// <summary>
        ///     Ctor
        /// </summary>
        public IniDataParser()
        {
            Scheme = new IniScheme();
            Configuration = new IniParserConfiguration();
            _errorExceptions = new List<Exception>();
        }

        #endregion

        #region State

        public virtual IniParserConfiguration Configuration { get; protected set; }

        /// <summary>
        ///     Scheme that defines the structure for the ini file to be parsed
        /// </summary>
        public IniScheme Scheme { get; protected set; }

        /// <summary>
        /// True is the parsing operation encounter any problem
        /// </summary>
        public bool HasError { get { return _errorExceptions.Count > 0; } }

        /// <summary>
        /// Returns the list of errors found while parsing the ini file.
        /// </summary>
        /// <remarks>
        /// If the configuration option ThrowExceptionOnError is false it
        /// can contain one element for each problem found while parsing;
        /// otherwise it will only contain the very same exception that was
        /// raised.
        /// </remarks>
        public ReadOnlyCollection<Exception> Errors
        {
            get { return _errorExceptions.AsReadOnly(); }
        }
        #endregion
        /// <summary>
        ///     Reads data in INI format from a stream.
        /// </summary>
        /// <param name="reader">Reader stream.</param>
        /// <returns>
        ///     And <see cref="IniData"/> instance with the readed ini data parsed.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="reader"/> is <c>null</c>.
        /// </exception>
        public IniData Parse(StreamReader reader)
        { 
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            return Parse(new StringReader(reader.ReadToEnd()));
        }

        /// <summary>
        ///     Parses a string containing valid ini data
        /// </summary>
        /// <param name="iniString">
        ///     String with data in INI format
        /// </param>
        public IniData Parse(string iniString)
        {
            return Parse(new StringReader(iniString));
        }

        /// <summary>
        ///     Parses a string containing valid ini data
        /// </summary>
        /// <param name="stringReader">
        ///     Text reader for the source string contaninig the ini data
        /// </param>
        /// <returns>
        ///     An <see cref="IniData"/> instance containing the data readed
        ///     from the source
        /// </returns>
        /// <exception cref="ParsingException">
        ///     Thrown if the data could not be parsed
        /// </exception>
        public IniData Parse(StringReader stringReader)
        {
            IniData iniData = Configuration.CaseInsensitive ?
                              new IniDataCaseInsensitive(Scheme)
                              : new IniData(Scheme);

            _errorExceptions.Clear();
            _currentCommentListTemp.Clear();
            _currentSectionNameTemp = null;
            _mBuffer.Reset(stringReader);
            _currentLineNumber = 0;

            while (_mBuffer.ReadLine())
            {
                _currentLineNumber++;

                try
                {
                    ProcessLine(_mBuffer, iniData);
                }
                catch (Exception ex)
                {
                    _errorExceptions.Add(ex);
                    if (Configuration.ThrowExceptionsOnError)
                    {
                        throw;
                    }
                }
            }

            // TODO: is this try necessary?
            try
            {

                // Orphan comments, assing to last section/key value
                if (_currentCommentListTemp.Count > 0)
                {
                    if (iniData.Sections.Count > 0)
                    {
                        // Check if there are actually sections in the file
                        var sections = iniData.Sections;
                        var section = sections.GetSectionData(_currentSectionNameTemp);
                        section.Comments.AddRange(_currentCommentListTemp);
                    }
                    else if (iniData.Global.Count > 0)
                    {
                        // No sections, put the comment in the last key value pair
                        // but only if the ini file contains at least one key-value pair
                        iniData.Global.GetLast().Comments.AddRange(_currentCommentListTemp);
                    }

                    _currentCommentListTemp.Clear();
                }

            }
            catch (Exception ex)
            {
                _errorExceptions.Add(ex);
                if (Configuration.ThrowExceptionsOnError)
                {
                    throw;
                }
            }


            if (HasError) return null;
            return iniData;
        }

        #region Template Method Design Pattern
        // All this methods controls the parsing behaviour, so it can be
        // modified in derived classes.
        // See http://www.dofactory.com/Patterns/PatternTemplate.aspx for an
        // explanation of this pattern.
        // Probably for the most common cases you can change the parsing
        // behavior using a custom configuration object rather than creating
        // derived classes.
        // See IniParserConfiguration interface, and IniDataParser constructor
        // to change the default configuration.

  

        /// <summary>
        ///     Processes one line and parses the data found in that line
        ///     (section or key/value pair who may or may not have comments)
        /// </summary>
        protected virtual void ProcessLine(StringBuffer currentLine,
                                           IniData iniData)
        {
            currentLine.Trim();

            if (currentLine.IsEmpty) return;

            // TODO: change this to a global (IniData level) array of comments
            // Extract comments from current line and store them in a tmp list

            if (ProcessComment(currentLine)) return;

            if (ProcessSection(currentLine, iniData)) return;

            if (ProcessProperty(currentLine, iniData)) return;

            if (Configuration.SkipInvalidLines) return;

            var errorFormat = "Couldn't parse text: '{0}'. Please see configuration option {1}.{2} to ignore this error.";
            var errorMsg = string.Format(errorFormat,
                                         currentLine,
                                         nameof(Configuration),
                                         nameof(Configuration.SkipInvalidLines));

            throw new ParsingException(errorMsg,
                                       _currentLineNumber,
                                       currentLine.DiscardChanges().ToString());
        }

        protected virtual bool ProcessComment(StringBuffer currentLine)
        {
            // Line is  med when it came here, so we only need to check if
            // the first characters are those of the comments
            if (currentLine.StartsWith(Scheme.CommentString))
            {
                var commentRange = currentLine.FindSubstring(Scheme.CommentString);
                if (commentRange.IsEmpty) return false;

                var startIdx = commentRange.start + Scheme.CommentString.Length;
                var size = currentLine.Count - Scheme.CommentString.Length;
                var range = Range.FromIndexWithSize(startIdx, size);
                var commentStr = currentLine.ToString(range);

                _currentCommentListTemp.Add(commentStr);
                currentLine.Resize(commentRange.start);
            }

            // If the line was a comment now it should be empty,
            // so no further processing is needed.
            return currentLine.Count <= 0;
        }

        /// <summary>
        ///     Proccess a string which contains an ini section.%
        /// </summary>
        /// <param name="currentLine">
        ///     The string to be processed
        /// </param>
        protected virtual bool ProcessSection(StringBuffer currentLine, IniData iniData)
        {
            if (currentLine.Count <= 0) return false;

            var sectionStartRange = currentLine.FindSubstring(Scheme.SectionStartString);

            if (sectionStartRange.IsEmpty) return false;
            
            var sectionEndRange = currentLine.FindSubstring(Scheme.SectionEndString, sectionStartRange.size);
            if (sectionEndRange.IsEmpty)
            {
                if (Configuration.SkipInvalidLines) return false;


                var errorFormat = "No closing section value. Please see configuration option {0}.{1} to ignore this error.";
                var errorMsg = string.Format(errorFormat,
                                             nameof(Configuration),
                                             nameof(Configuration.SkipInvalidLines));

                throw new ParsingException(errorMsg,
                                           _currentLineNumber,
                                           currentLine.DiscardChanges().ToString());
            }

            var startIdx = sectionStartRange.start + Scheme.SectionStartString.Length;
            var endIdx = sectionEndRange.end - 1;
            currentLine.ResizeBetweenIndexes(startIdx, endIdx);

            if (Configuration.TrimSections)
            {
                currentLine.Trim();
            }

            var sectionName = currentLine.ToString();

            // Temporally save section name.
            _currentSectionNameTemp = sectionName;

            //Checks if the section already exists
            if (!Configuration.AllowDuplicateSections)
            {
                if (iniData.Sections.ContainsSection(sectionName))
                {
                    if (Configuration.SkipInvalidLines) return false;

                    var errorFormat = "Duplicate section with name '{0}'. Please see configuration option {1}.{2} to ignore this error.";
                    var errorMsg = string.Format(errorFormat,
                                                 sectionName,
                                                 nameof(Configuration),
                                                 nameof(Configuration.SkipInvalidLines));

                    throw new ParsingException(errorMsg,
                                               _currentLineNumber,
                                               currentLine.DiscardChanges().ToString());
                }
            }

            // If the section does not exists, add it to the ini data
            iniData.Sections.AddSection(sectionName);

            // Save comments read until now and assign them to this section
            var sections = iniData.Sections;
            var sectionData = sections.GetSectionData(sectionName);
            sectionData.Comments.AddRange(_currentCommentListTemp);
            _currentCommentListTemp.Clear();

            return true;
        }

        protected virtual bool ProcessProperty(StringBuffer currentLine, IniData iniData)
        {
            if (currentLine.Count <= 0) return false;

            var propertyAssigmentIdx = currentLine.FindSubstring(Scheme.PropertyAssigmentString);

            if (propertyAssigmentIdx.IsEmpty) return false;

            var keyRange = Range.WithIndexes(0, propertyAssigmentIdx.start - 1);
            var valueStartIdx = propertyAssigmentIdx.end + 1;
            var valueSize = currentLine.Count - propertyAssigmentIdx.end - 1;
            var valueRange = Range.FromIndexWithSize(valueStartIdx, valueSize);

            var key = currentLine.Substring(keyRange);
            var value = currentLine.Substring(valueRange);

            if (Configuration.TrimProperties)
            {
                key.Trim();
                value.Trim();
            }

            if (key.IsEmpty)
            {
                if (Configuration.SkipInvalidLines) return false;

                var errorFormat = "Found property without key. Please see configuration option {0}.{1} to ignore this error";
                var errorMsg = string.Format(errorFormat,
                                             nameof(Configuration),
                                             nameof(Configuration.SkipInvalidLines));

                throw new ParsingException(errorMsg,
                                           _currentLineNumber,
                                           currentLine.DiscardChanges().ToString());
            }

            // Check if we haven't read any section yet
            if (string.IsNullOrEmpty(_currentSectionNameTemp))
            {
                if (!Configuration.AllowKeysWithoutSection)
                {
                    var errorFormat = "Properties must be contained inside a section. Please see configuration option {0}.{1} to ignore this error.";
                    var errorMsg = string.Format(errorFormat,
                                                nameof(Configuration),
                                                nameof(Configuration.AllowKeysWithoutSection));

                    throw new ParsingException(errorMsg,
                                               _currentLineNumber,
                                               currentLine.DiscardChanges().ToString());
                }

                AddKeyToKeyValueCollection(key.ToString(), 
                                           value.ToString(),
                                           iniData.Global, 
                                           "global");
            }
            else
            {
                var currentSection = iniData.Sections.GetSectionData(_currentSectionNameTemp);

                AddKeyToKeyValueCollection(key.ToString(),
                                           value.ToString(), 
                                           currentSection.Properties,
                                           _currentSectionNameTemp);
            }


            return true;
        }


        /// <summary>
        ///     Abstract Method that decides what to do in case we are trying 
        ///     to add a duplicated key to a section
        /// </summary>
        void HandleDuplicatedKeyInCollection(string key,
                                             string value,
                                             PropertyCollection keyDataCollection,
                                             string sectionName)
        {
            switch(Configuration.DuplicatePropertiesBehaviour)
            {
                case IniParserConfiguration.EDuplicatePropertiesBehaviour.DisallowAndStopWithError:
                    var errorMsg = string.Format("Duplicated key '{0}' found in section '{1}", key, sectionName);
                    throw new ParsingException(errorMsg, _currentLineNumber);
                case IniParserConfiguration.EDuplicatePropertiesBehaviour.AllowAndKeepFirstValue:
                    // Nothing to do here: we already have the first value assigned
                    break;
                case IniParserConfiguration.EDuplicatePropertiesBehaviour.AllowAndKeepLastValue:
                    // Override the current value when the parsing is finished we will end up
                    // with the last value.
                    keyDataCollection[key] = value;
                    break;
                case IniParserConfiguration.EDuplicatePropertiesBehaviour.AllowAndConcatenateValues:
                    break;
            }
        }
        #endregion

        #region Helpers

        /// <summary>
        ///     Adds a key to a concrete <see cref="PropertyCollection"/> instance, checking
        ///     if duplicate keys are allowed in the configuration
        /// </summary>
        /// <param name="key">
        ///     Key name
        /// </param>
        /// <param name="value">
        ///     Key's value
        /// </param>
        /// <param name="keyDataCollection">
        ///     <see cref="Property"/> collection where the key should be inserted
        /// </param>
        /// <param name="sectionName">
        ///     Name of the section where the <see cref="PropertyCollection"/> is contained.
        ///     Used only for logging purposes.
        /// </param>
        private void AddKeyToKeyValueCollection(string key, string value, PropertyCollection keyDataCollection, string sectionName)
        {
            // Check for duplicated keys
            if (keyDataCollection.ContainsKey(key))
            {
                // We already have a key with the same name defined in the current section
                HandleDuplicatedKeyInCollection(key, value, keyDataCollection, sectionName);
            }
            else
            {
                // Save the keys
                keyDataCollection.AddKeyAndValue(key, value);
            }

            keyDataCollection.GetKeyData(key).Comments = new List<string>(_currentCommentListTemp);
            _currentCommentListTemp.Clear();
        }

        #endregion

        #region Fields
        uint _currentLineNumber;

        // Holds a list of the exceptions catched while parsing
        readonly List<Exception> _errorExceptions;

        // Temp list of comments
        readonly List<string> _currentCommentListTemp = new List<string>();

        // Tmp var with the name of the seccion which is being process
        string _currentSectionNameTemp;

        // Buffer used to hold the current line being processed.
        // Saves allocating a new string
        readonly StringBuffer _mBuffer = new StringBuffer(256);
        #endregion
    }
}
