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
			//TODO remove this
			SectionKeySeparator = '.';
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
        ///     Attempts to retrieve a key, using a single string combining section and 
        ///     key name.
        /// </summary>
        /// <param name="key">
        ///     The section and key name to retrieve, separated by <see cref="IniParserConfiguration.SectionKeySeparator"/>.
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
        ///     The section and key name to retrieve, separated by <see cref="IniParserConfiguration.SectionKeySeparator"/>.
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