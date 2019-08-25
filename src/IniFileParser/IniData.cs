using IniParser.Configuration;
using IniParser.Model;

namespace IniParser
{
    /// <summary>
    ///     Represents all data from an INI file
    /// </summary>
    public class IniData : IDeepCloneable<IniData>
    {
        #region Initialization

        /// <summary>
        ///     Initializes an empty IniData instance.
        /// </summary>
        public IniData() 
        {
            Global = new PropertyCollection();
            Sections = new SectionCollection();
            _scheme = new IniScheme();
        }
        
        /// <summary>
        ///     Initialzes an IniData instance with a given scheme
        /// </summary>
        /// <param name="scheme"></param>
        public IniData(IniScheme scheme)
         : this()
        {
            _scheme = scheme.DeepClone();
        }

        public IniData(IniData ori)
        {
            Sections = ori.Sections.DeepClone();
            Global = ori.Global.DeepClone();
            Configuration = ori.Configuration.DeepClone();
        }
        #endregion

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

            set
            {
                _configuration = value.DeepClone();
            }
        }

        public IniScheme Scheme
        {
            get
            {
                // Lazy initialization
                if (_scheme == null)
                {
                    _scheme = new IniScheme();
                }

                return _scheme;
            }

            set
            {
                _scheme = value.DeepClone();
            }
        }

        /// <summary>
        /// 	Global sections. Contains properties which are not
        /// 	enclosed in any section (i.e. they are defined at the beginning 
        /// 	of the file, before any section.
        /// </summary>
        public PropertyCollection Global { get; protected set; }

        /// <summary>
        /// Gets the <see cref="PropertyCollection"/> instance 
        /// with the specified section name.
        /// with the specified section name.
        /// </summary>
        public PropertyCollection this[string sectionName]
        {
            get
            {
                if (!Sections.Contains(sectionName))
                    if (CreateSectionsIfTheyDontExist)
                        Sections.Add(sectionName);
                    else
                        return null;

                return Sections[sectionName];
            }
        }

        /// <summary>
        /// Gets or sets all the <see cref="Section"/> 
        /// for this IniData instance.
        /// </summary>
        public SectionCollection Sections { get; set; }

        /// <summary>
        ///     Deletes all data
        /// </summary>
        public void Clear()
        {
            Global.Clear();
            Sections.Clear();
        }
        /// <summary>
        ///     Deletes all comments in all sections and properties values
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

        #region IDeelCloneable<T> Members

        /// <summary>
        ///     Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///     A new object that is a copy of this instance.
        /// </returns>
        public IniData DeepClone()
        {
            return new IniData(this);
        }

        #endregion

        #region Fields
        /// <summary>
        ///     See property <see cref="Configuration"/> for more information. 
        /// </summary>
        private IniParserConfiguration _configuration;
        /// <summary>
        ///     Represents all sections from an INI file
        /// </summary>
        protected IniScheme _scheme;

        #endregion
    }
}