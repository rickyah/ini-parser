using System;
using IniParser.Model.Configuration;

namespace IniParser.Model
{
    /// <summary>
    ///     Represents all data from an INI file
    /// </summary>
    public class IniData : IDeepCloneable<IniData>
    {
        #region Non-Public Members
        /// <summary>
        ///     Represents all sections from an INI file
        /// </summary>
        protected IniScheme _schemeInternal;
        #endregion

        #region Initialization

        /// <summary>
        ///     Initializes an empty IniData instance.
        /// </summary>
        public IniData() 
        {
            Global = new PropertyCollection();
            Sections = new SectionCollection();
            _schemeInternal = new IniScheme();
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
            Sections = sdc.DeepClone();
        }

        public IniData(IniData ori)
        {
            Sections = ori.Sections.DeepClone();
            Global = ori.Global.DeepClone();
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
                if (!Sections.ContainsSection(sectionName))
                    if (CreateSectionsIfTheyDontExist)
                        Sections.AddSection(sectionName);
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
    }
}