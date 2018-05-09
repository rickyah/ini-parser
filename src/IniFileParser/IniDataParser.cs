using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using IniParser.Exceptions;
using IniParser.Model;
using IniParser.Configuration;

namespace IniParser
{
    /// <summary>
    ///     Responsible for parsing an string from an ini file, and creating
    ///     an <see cref="IniData"/> structure.
    /// </summary>
    public class IniDataParser
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

        public IniParserConfiguration Configuration { get; protected set; }

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
        public IniData Parse(TextReader stringReader)
        {
            IniData iniData = Configuration.CaseInsensitive ?
                                new IniDataCaseInsensitive()
                                : new IniData();

            return Parse(stringReader, ref iniData);
        }
        
        public IniData Parse(TextReader stringReader, ref IniData iniData) 
        {
            iniData.Scheme = Scheme.DeepClone();

            _errorExceptions.Clear();
            _currentCommentListTemp.Clear();
            _currentSectionNameTemp = null;
            _currentLineNumber = 1;

            var currentLine = stringReader.ReadLine(); 
            while (currentLine != null)
            {
                try
                {
                    ProcessLine(currentLine.Trim(), iniData);
                }
                catch (Exception ex)
                {
                    //var errorEx = new ParsingException(ex.Message,
                                                       //_mBuffer.LineNumber + 1,
                                                       //_mBuffer.ToString(),
                                                       //ex);
                    _errorExceptions.Add(ex);
                    if (Configuration.ThrowExceptionsOnError)
                    {
                        throw;
                    }
                }

                currentLine = stringReader.ReadLine();
                _currentLineNumber++;
            }


            // Orphan comments, assing to last section/key value
            if (_currentCommentListTemp.Count > 0)
            {
                // Check if there are actually sections in the file
                if (iniData.Sections.Count > 0)
                {
                    var sections = iniData.Sections;
                    var section = sections.GetSectionData(_currentSectionNameTemp);
                    section.Comments.AddRange(_currentCommentListTemp);
                }

                // No sections, put the comment in the last key value pair
                // but only if the ini file contains at least one key-value
                // pair
                else if (iniData.Global.Count > 0)
                {
                    iniData.Global.GetLast().Comments
                        .AddRange(_currentCommentListTemp);
                }


                _currentCommentListTemp.Clear();
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
        ///     Checks if a given string represents a section delimiter.
        /// </summary>
        /// <param name="line">
        ///     The string to be checked.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the string represents a section,
        ///     <c>false</c> otherwise.
        /// </returns>
        protected virtual bool LineMatchesASection(string line)
        {
            return !string.IsNullOrEmpty(line)
                   && Scheme.SectionRegex.Match(line).Success;
        }

        /// <summary>
        ///     Checks if a given string represents a key / value pair.
        /// </summary>
        /// <param name="line">
        ///     The string to be checked.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the string represents a key / value pair,
        ///     <c>false</c> otherwise.
        /// </returns>
        protected virtual bool LineMatchesAKeyValuePair(string line)
        {
            return !string.IsNullOrEmpty(line)
                   && line.Contains(Scheme.PropertyDelimiterString);
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
        protected virtual (bool result, string comment) ExtractComment(ref string line)
        {
            var resultIdx = line.IndexOf(Scheme.CommentString);
            if (resultIdx < 0) return (false, string.Empty);

            var comment = line.Substring(resultIdx + Scheme.CommentString.Length);
            line = line.Substring(0, resultIdx).Trim();
            return (true, comment);
        }

        /// <summary>
        ///     Processes one line and parses the data found in that line
        ///     (section or key/value pair who may or may not have comments)
        /// </summary>
        protected virtual void ProcessLine(string currentLine,
                                           IniData iniData)
        {

            if (string.IsNullOrWhiteSpace(currentLine)) return;

            // TODO: change this to a global (IniData level) array of comments
            // Extract comments from current line and store them in a tmp list
            if (ProcessComment(ref currentLine, iniData)) return;

            if (ProcessSection(currentLine, iniData)) return;

            if (ProcessProperty(currentLine, iniData)) return;

            if (Configuration.SkipInvalidLines) return;

            var errorFormat = "Couldn't parse text: '{0}'. Please see configuration option {1}.{2} to ignore this error.";
            var errorMsg = string.Format(errorFormat,
                                         currentLine,
                                         Configuration.GetType().Name,
                                         ParsingException.GetPropertyName(() => Configuration.SkipInvalidLines));


            throw new ParsingException(errorMsg,
                                       _currentLineNumber,
                                       currentLine);
        }

        protected bool ProcessComment(ref string currentLine, IniData iniData)
        {
            var extraction = ExtractComment(ref currentLine);

            if (!extraction.result) return false;
            
            _currentCommentListTemp.Add(extraction.comment);
            // If the complete line was a comment now it should be empty,
            // so no further processing is needed.
            return string.IsNullOrWhiteSpace(currentLine);
        }

        /// <summary>
        ///     Proccess a string which contains an ini section.%
        /// </summary>
        /// <param name="line">
        ///     The string to be processed
        /// </param>
        protected virtual bool ProcessSection(string currentLine, IniData iniData)
        {
            var result = Scheme.SectionRegex.Match(currentLine);
            if (!result.Success) return false;

            int startIdx = Scheme.SectionStartString.Length;
            int endIdx = result.Value.Length 
                       - Scheme.SectionStartString.Length
                       - Scheme.SectionEndString.Length;
            
            string sectionName = result.Value.Substring(startIdx, endIdx);
           
            if (string.IsNullOrWhiteSpace(sectionName))
            {
                return false;

                    // TODO Remove this error?
                    //var errorFormat = "Could not parse section: section name is empty. Please see configuration option {0}.{1} to ignore this error.";
                    //var errorMsg = string.Format(errorFormat,
                    //                             Configuration.GetType().Name,
                    //                             ParsingException.GetPropertyName(() => Configuration.SkipInvalidLines));

                    //throw new ParsingException(errorMsg,
                                               //_currentLineNumber,
                                               //currentLine);
            }

            // Temporally save section name so we know to which section add
            // properties we may parse in following lines
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
                                                 Configuration.GetType().Name,
                                                 ParsingException.GetPropertyName(() => Configuration.SkipInvalidLines));

                    throw new ParsingException(errorMsg,
                                               _currentLineNumber,
                                               currentLine);
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

        protected virtual bool ProcessProperty(string currentLine, IniData iniData)
        {
            var property = ExtractProperty(currentLine);

            if (!property.result)
            {
                if (Configuration.SkipInvalidLines) return false;

                var errorFormat = "Found property without key. Please see configuration option {0}.{1} to ignore this error";
                var errorMsg = string.Format(errorFormat,
                                             Configuration.GetType().Name,
                                             ParsingException.GetPropertyName(() => Configuration.SkipInvalidLines));

                throw new ParsingException(errorMsg,
                                           _currentLineNumber,
                                           currentLine);
            }

            // Check if we haven't read any section yet
            if (string.IsNullOrEmpty(_currentSectionNameTemp))
            {
                if (!Configuration.AllowKeysWithoutSection)
                {
                    var errorFormat = "Properties must be contained inside a section. Please see configuration option {0}.{1} to ignore this error.";
                    var errorMsg = string.Format(errorFormat,
                                                Configuration.GetType().Name,
                                                ParsingException.GetPropertyName(() => Configuration.AllowKeysWithoutSection));

                    throw new ParsingException(errorMsg,
                                               _currentLineNumber,
                                               currentLine);
                }

                AddPropertyToCollection(property.key, 
                                           property.value,
                                           iniData.Global,
                                           "global");
            }
            else
            {
                var currentSection = iniData.Sections.GetSectionData(_currentSectionNameTemp);

                AddPropertyToCollection(property.key, 
                                           property.value,
                                           currentSection.Keys, 
                                           _currentSectionNameTemp);
            }


            return true;
        }

        
        /// <summary>
        ///     Extracts the key and value  from a string containing a property.
        /// </summary>
        /// <param name="s">    
        ///     The string to be processed, which contains a property pair
        /// </param>
        /// <returns>
        ///     A tuple containing a boolean with the result: 
        ///     true if the operation succeded, false otherwise.
        ///     A string with the key  and a string with the value.
        ///     If the extraction of the components of a property fails 
        ///     both strings will be empty.
        /// </returns>
        protected virtual (bool result, string key, string value) ExtractProperty(string s)
        {
            int index = s.IndexOf(Scheme.PropertyDelimiterString, 0);

            if (index < 0)
            {
                return (false, string.Empty, string.Empty);
            }
            
            var key = s.Substring(0, index).Trim();

            if (string.IsNullOrEmpty(key))
            {
                return (false, string.Empty, string.Empty);
            }
            
            var value = s.Substring(index + Scheme.PropertyDelimiterString.Length, 
                                    s.Length - index - Scheme.PropertyDelimiterString.Length).Trim();

            return (true, key, value);
        }

        /// <summary>
        ///     Abstract Method that decides what to do in case we are trying to add a duplicated key to a section
        /// </summary>
        protected virtual void HandleDuplicatedKeyInCollection(string key,
                                                               string value,
                                                               PropertyCollection keyDataCollection,
                                                               string sectionName)
        {
            if (!Configuration.AllowDuplicateKeys)
            {
                var errorMsg = string.Format("Duplicated key '{0}' found in section '{1}", key, sectionName);
                throw new ParsingException(errorMsg, _currentLineNumber);
            }
            else if (Configuration.OverrideDuplicateKeys)
            {
                keyDataCollection[key] = value;
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
        private void AddPropertyToCollection(string key,
                                             string value,
                                             PropertyCollection keyDataCollection,
                                             string sectionName)
        {
        
            //TODO: Refactor this, sectionName parameter only needed to error handling
            // Check for duplicated keys
            if (keyDataCollection.ContainsKey(key))
            {
                // We already have a key with the same name defined in the current section
                HandleDuplicatedKeyInCollection(key, value, keyDataCollection, sectionName);
            }
            else
            {
                keyDataCollection.AddKey(key, value);
            }

            keyDataCollection.GetKeyData(key).Comments.AddRange(_currentCommentListTemp);
            _currentCommentListTemp.Clear();
        }

        #endregion

        #region Fields


        uint _currentLineNumber;

        // Holds a list of the exceptions catched while parsing
        // TODO: rename this to _errors
        List<Exception> _errorExceptions;

        // Temp list of comments
        readonly List<string> _currentCommentListTemp = new List<string>();

        // Tmp var with the name of the seccion which is being process
        string _currentSectionNameTemp;
        #endregion
    }
}
