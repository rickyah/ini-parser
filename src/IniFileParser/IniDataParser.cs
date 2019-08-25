using System;
using System.Collections.Generic;
using IniParser.Exceptions;
using IniParser.Configuration;
using System.Collections.ObjectModel;
using System.IO;
using IniParser.Parser;
using IniParser.Model;
using static IniParser.Parser.StringBuffer;

namespace IniParser
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
        /// <param name="textReader">
        ///     Text reader for the source string contaninig the ini data
        /// </param>
        /// <returns>
        ///     An <see cref="IniData"/> instance containing the data readed
        ///     from the source
        /// </returns>
        /// <exception cref="ParsingException">
        ///     Thrown if the data could not be parsed
        /// </exception>
        public IniData Parse(TextReader textReader)
        {
            IniData iniData = Configuration.CaseInsensitive ?
                  new IniDataCaseInsensitive(Scheme)
                  : new IniData(Scheme);

            Parse(textReader, ref iniData);

            return iniData;
        }

        /// <summary>
        ///     Parses a string containing valid ini data
        /// </summary>
        /// <param name="textReader">
        ///     Text reader for the source string contaninig the ini data
        /// </param>
        /// <returns>
        ///     An <see cref="IniData"/> instance containing the data readed
        ///     from the source
        /// </returns>
        /// <exception cref="ParsingException">
        ///     Thrown if the data could not be parsed
        /// </exception>       
        public void Parse(TextReader textReader, ref IniData iniData)
        {
            iniData.Clear();

            iniData.Scheme = Scheme.DeepClone();

            _errorExceptions.Clear();
            if (Configuration.ParseComments)
            {
                CurrentCommentListTemp.Clear();
            }
            _currentSectionNameTemp = null;
            _mBuffer.Reset(textReader);
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
                if (Configuration.ParseComments && CurrentCommentListTemp.Count > 0)
                {
                    if (iniData.Sections.Count > 0)
                    {
                        // Check if there are actually sections in the file
                        var sections = iniData.Sections;
                        var section = sections.FindByName(_currentSectionNameTemp);
                        section.Comments.AddRange(CurrentCommentListTemp);
                    }
                    else if (iniData.Global.Count > 0)
                    {
                        // No sections, put the comment in the last key value pair
                        // but only if the ini file contains at least one key-value pair
                        iniData.Global.GetLast().Comments.AddRange(CurrentCommentListTemp);
                    }

                    CurrentCommentListTemp.Clear();
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

            if (HasError)
            {
                iniData.Clear();
            }
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
            if (currentLine.IsEmpty || currentLine.IsWhitespace) return;

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
            var currentLineTrimmed = currentLine.SwallowCopy();
            currentLineTrimmed.TrimStart();

            if (!currentLineTrimmed.StartsWith(Scheme.CommentString))
            {
                return false;
            }

            if (!Configuration.ParseComments)
            {
                return true;
            }

            currentLineTrimmed.TrimEnd();

            var commentRange = currentLineTrimmed.FindSubstring(Scheme.CommentString);
            // Exctract the range of the string that contains the comment but not
            // the comment delimiter
            var startIdx = commentRange.start + Scheme.CommentString.Length;
            var size = currentLineTrimmed.Count - Scheme.CommentString.Length;
            var range = Range.FromIndexWithSize(startIdx, size);

            var comment = currentLineTrimmed.Substring(range);
            if (Configuration.TrimComments)
            {
                comment.Trim();
            }
            
            CurrentCommentListTemp.Add(comment.ToString());

            return true;
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
            var endIdx = sectionEndRange.end - Scheme.SectionEndString.Length;
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
                if (iniData.Sections.Contains(sectionName))
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
            iniData.Sections.Add(sectionName);

            // Save comments read until now and assign them to this section
            if (Configuration.ParseComments)
            {
                var sections = iniData.Sections;
                var sectionData = sections.FindByName(sectionName);
                sectionData.Comments.AddRange(CurrentCommentListTemp);
                CurrentCommentListTemp.Clear();
            }

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
                var currentSection = iniData.Sections.FindByName(_currentSectionNameTemp);

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
                    keyDataCollection[key] += Configuration.ConcatenateDuplicatePropertiesString + value; 
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
            if (keyDataCollection.Contains(key))
            {
                // We already have a key with the same name defined in the current section
                HandleDuplicatedKeyInCollection(key, value, keyDataCollection, sectionName);
            }
            else
            {
                // Save the keys
                keyDataCollection.Add(key, value);
            }

            if (Configuration.ParseComments)
            {
                keyDataCollection.FindByKey(key).Comments = CurrentCommentListTemp;
                CurrentCommentListTemp.Clear();
            }
        }

        #endregion

        #region Fields
        uint _currentLineNumber;

        // Holds a list of the exceptions catched while parsing
        readonly List<Exception> _errorExceptions;

        // Temp list of comments
        public List<string> CurrentCommentListTemp
        {
            get
            {
                if (_currentCommentListTemp == null)
                {
                    _currentCommentListTemp = new List<string>();
                }

                return _currentCommentListTemp;
            }

            internal set
            {
                _currentCommentListTemp = value;
            }
        }
        List<string> _currentCommentListTemp;

        // Tmp var with the name of the seccion which is being process
        string _currentSectionNameTemp;

        // Buffer used to hold the current line being processed.
        // Saves allocating a new string
        readonly StringBuffer _mBuffer = new StringBuffer(256);
        #endregion
    }
}
