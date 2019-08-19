using System;
using IniParser.Model.Configuration;
using IniParser.Model.Formatting;

namespace IniParser.Model
{
    /// <summary>
    ///     Represents all data from an INI file
    /// </summary>
    public class IniData : ICloneable
    {
        #region Non-Public Members
        /// <summary>
        ///     Represents all sections from an INI file
        /// </summary>
        SectionCollection _sections;
        protected IniScheme _schemeInternal;
        #endregion

        #region Initialization

        /// <summary>
        ///     Initializes an empty IniData instance.
        /// </summary>
        public IniData() 
        {
            Global = new PropertyCollection();
            _sections = new SectionCollection();
            _schemeInternal = new IniScheme();
            SectionKeySeparator = '.';
        }
        
        /// <summary>
        ///     Initialzes an IniData instance with a given scheme
        /// </summary>
        /// <param name="scheme"></param>
        public IniData(IniScheme scheme)
         : this()
        {
            _schemeInternal = scheme.DeepClone();
        }

        /// <summary>
        ///     Initializes a new IniData instance using a previous
        ///     <see cref="SectionCollection"/>.
        /// </summary>
        /// <param name="sdc">
        ///     <see cref="SectionCollection"/> object containing the
        ///     data with the sections of the file
        /// </param>
        public IniData(SectionCollection sdc)
            :this()
        {
            _sections = (SectionCollection)sdc.DeepClone();
        }

        public IniData(IniData ori)
        {
            _sections = ori._sections.DeepClone();
            Global = (PropertyCollection)ori.Global.DeepClone();
            Configuration = ori.Configuration.DeepClone();
        }
        #endregion

        #region Properties

        /// <summary>
        ///     If set to true, it will automatically create a section when you use the indexed 
        ///     access with a section name that does not exis.
        ///     If set to false, it will throw an exception if you try to access a section that 
        ///     does not exist with the index operator.
        /// </summary>
        /// <remarks>
        ///     Defaults to false.
        /// </remarks>
        public bool CreateSectionsIfTheyDontExist { get; set; } = false;


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
        public IniParserConfiguration Configuration
        {
            get
            {
                // Lazy initialization
                if (_configuration == null)
                {
                    _configuration = new IniParserConfiguration();
                }

                return _configuration;
            }

            set { _configuration = value.DeepClone(); }
        }

        public IIniScheme Scheme { get { return _schemeInternal; } }

        /// <summary>
        /// 	Global sections. Contains key/value pairs which are not
        /// 	enclosed in any section (i.e. they are defined at the beginning 
        /// 	of the file, before any section.
        /// </summary>
        public PropertyCollection Global { get; protected set; }

        /// <summary>
        /// Gets the <see cref="PropertyCollection"/> instance 
        /// with the specified section name.
        /// </summary>
        public PropertyCollection this[string sectionName]
        {
            get
            {
                if (!_sections.ContainsSection(sectionName))
                    if (CreateSectionsIfTheyDontExist)
                        _sections.AddSection(sectionName);
                    else
                        return null;

                return _sections[sectionName];
            }
        }

        /// <summary>
        /// Gets or sets all the <see cref="Section"/> 
        /// for this IniData instance.
        /// </summary>
        public SectionCollection Sections
        {
            get { return _sections; }
            set { _sections = value; }
        }

        /// <summary>
        ///     Used to mark the separation between the section name and the key name 
        ///     when using <see cref="IniData.TryGetKey"/>. 
        /// </summary>
        /// <remarks>
        ///     Defaults to '.'.
        /// </remarks>
        public char SectionKeySeparator { get; set; }
        #endregion

        #region Object Methods
        //public override string ToString()
        //{
        //    return ToString(new DefaultIniDataFormatter(Configuration));
        //}

        //public virtual string ToString(IIniDataFormatter formatter)
        //{
        //    return formatter.IniDataToString(this);
        //}
        #endregion

        #region ICloneable Members

        /// <summary>
        ///     Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///     A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return new IniData(this);
        }

        #endregion

        #region Fields
        /// <summary>
        ///     See property <see cref="Configuration"/> for more information. 
        /// </summary>
        private IniParserConfiguration _configuration;
        #endregion

        /// <summary>
        ///     Deletes all comments in all sections and key values
        /// </summary>
        public void ClearAllComments()
        {
            Global.ClearComments();
            foreach (var section in Sections)
            {
                section.ClearComments();
                section.Properties.ClearComments();
            }
        }

        /// <summary>
        ///     Merges the other iniData into this one by overwriting existing values.
        ///     Comments get appended.
        /// </summary>
        /// <param name="toMergeIniData">
        ///     IniData instance to merge into this. 
        ///     If it is null this operation does nothing.
        /// </param>
        public void Merge(IniData toMergeIniData)
        {
            if (toMergeIniData == null) return;

            Global.Merge(toMergeIniData.Global);
            Sections.Merge(toMergeIniData.Sections);
        }

        /// <summary>
        ///     Attempts to retrieve a key, using a single string combining section and 
        ///     key name.
        /// </summary>
        /// <param name="key">
        ///     The section and key name to retrieve, separated by <see cref="IniData.SectionKeySeparator"/>.
        /// 
        ///     If key contains no separator, it is treated as a key in the <see cref="Global"/> section.
        /// 
        ///     Key may contain no more than one separator character.
        /// </param>
        /// <param name="value">
        ///     If true is returned, is set to the value retrieved.  Otherwise, is set
        ///     to an empty string.
        /// </param>
        /// <returns>
        ///     True if key was found, otherwise false.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     key contained multiple separators.
        /// </exception>
        public bool TryGetKey(string key, out string value)
        {
            value = string.Empty;
            if (string.IsNullOrEmpty(key))
                return false;

            var splitKey = key.Split(SectionKeySeparator);
            var separatorCount = splitKey.Length - 1;
            if (separatorCount > 1)
                throw new ArgumentException("key contains multiple separators", "key");

            if (separatorCount == 0)
            {
                if (!Global.ContainsKey(key))
                    return false;

                value = Global[key];
                return true;
            }

            var section = splitKey[0];
            key = splitKey[1];

            if (!_sections.ContainsSection(section))
                return false;
            var sectionData = _sections[section];
            if (!sectionData.ContainsKey(key))
                return false;

            value = sectionData[key];
            return true;
        }

        /// <summary>
        ///     Retrieves a key using a single input string combining section and key name.
        /// </summary>
        /// <param name="key">
        ///     The section and key name to retrieve, separated by <see cref="IniData.SectionKeySeparator"/>.
        /// 
        ///     If key contains no separator, it is treated as a key in the <see cref="Global"/> section.
        /// 
        ///     Key may contain no more than one separator character.
        /// </param>
        /// <returns>
        ///     The key's value if it was found, otherwise null.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     key contained multiple separators.
        /// </exception>
        public string GetKey(string key)
        {
            string result;
            return TryGetKey(key, out result) ? result : null;
        }
    }
}