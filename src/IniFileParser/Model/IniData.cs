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
        private SectionDataCollection _sections;

		/// <summary>
		/// 	Formatter applied by default when calling ToString() in this instance.
		/// </summary>
		IniDataFormatter _defaultIniDataFormatter;
        #endregion

        #region Initialization

        /// <summary>
        ///     Initializes an empty IniData instance.
        /// </summary>
        public IniData()
        {
			Global = new KeyDataCollection();
			SchemeInternal = new IniScheme();
			_sections = new SectionDataCollection();
			_defaultIniDataFormatter = new IniDataFormatter(new IniFormattingConfiguration(SchemeInternal));
		}

        /// <summary>
        ///     Initializes a new IniData instance using a previous
        ///     <see cref="SectionDataCollection"/>.
        /// </summary>
        /// <param name="sdc">
        ///     <see cref="SectionDataCollection"/> object containing the
        ///     data with the sections of the file
        /// </param>
		public IniData(SectionDataCollection sdc): this()
        {
            _sections = (SectionDataCollection)sdc.Clone();
        }

        public IniData(IniData ori)
        {
			SchemeInternal = (IniScheme)ori.SchemeInternal.Clone();
            Global = (KeyDataCollection)ori.Global.Clone();
			_sections = (SectionDataCollection)ori._sections.Clone();
			_defaultIniDataFormatter = new IniDataFormatter(ori._defaultIniDataFormatter);
        }
        #endregion

        #region Properties

		public IIniScheme Scheme { get { return SchemeInternal; } }
		internal IniScheme SchemeInternal { get; set; }

        /// <summary>
        /// 	Global sections. Contains key/value pairs which are not
        /// 	enclosed in any section (i.e. they are defined at the beginning 
        /// 	of the file, before any section.
        /// </summary>
        public KeyDataCollection Global { get; protected set; }

        /// <summary>
        /// Gets the <see cref="KeyDataCollection"/> instance 
        /// with the specified section name.
        /// </summary>
        public KeyDataCollection this[string sectionName]
        {
            get
            {
                if (!_sections.ContainsSection(sectionName))
                {
                    _sections.AddSection(sectionName);
                }

                return _sections[sectionName];
            }
        }

        /// <summary>
        /// Gets or sets all the <see cref="SectionData"/> 
        /// for this IniData instance.
        /// </summary>
        public SectionDataCollection Sections
        {
            get { return _sections; }
            set { _sections = value; }
        }

        #endregion

        #region Object Methods
        public override string ToString()
        {
			return ToString(_defaultIniDataFormatter);
        }

        public virtual string ToString(IIniDataFormatter formatter)
        {
            return formatter.IniDataToString(this);
        }

		public virtual string ToString(IniFormattingConfiguration format)
		{
			return ToString(new IniDataFormatter(format));
		}

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

        /// <summary>
        ///     Deletes all comments in all sections and key values
        /// </summary>
        public void ClearAllComments()
        {
            Global.ClearComments();

            foreach(var section in Sections)
            {
                section.ClearComments();
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
        ///     Merge the sections into this by overwriting this sections.
        /// </summary>
        private void MergeSection(SectionData otherSection)
        {
            // no overlap -> create no section
            if (!Sections.ContainsSection(otherSection.SectionName))
            {
                Sections.AddSection(otherSection.SectionName);
            }

            // merge section into the new one
            Sections.GetSectionData(otherSection.SectionName).Merge(otherSection);
        }

        /// <summary>
        ///     Merges the given global values into this globals by overwriting existing values.
        /// </summary>
        private void MergeGlobal(KeyDataCollection globals)
        {
            foreach (var globalValue in globals)
            {
                Global[globalValue.KeyName] = globalValue.Value;
            }
        }
    }
}