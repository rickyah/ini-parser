using System;

namespace IniParser
{
    /// <summary>
    /// Represents all data from an INI file
    /// </summary>
    public class IniData : ICloneable
    {
        #region Non-Public Members
        /// <summary>
        /// Represents all sections from an INI file
        /// </summary>
        private SectionDataCollection _sections;
        #endregion

        /// <summary>
        /// Initializes an empty IniData instance.
        /// </summary>
        public IniData()
        {
            _sections = new SectionDataCollection();
        }

        /// <summary>
        /// Initializes a new IniData instance using a previous
        /// <see cref="SectionDataCollection"/>.
        /// </summary>
        /// <param name="sdc">
        /// <see cref="SectionDataCollection"/> object containing the
        /// data with the sections of the file</param>
        public IniData(SectionDataCollection sdc)
        {
            _sections = (SectionDataCollection)sdc.Clone();
        }

        #region Properties

        /// <summary>
        /// Gets the <see cref="IniParser.KeyDataCollection"/> instance 
        /// with the specified section name.
        /// </summary>
        public KeyDataCollection this[string sectionName]
        {
            get
            {
                if (_sections.ContainsSection(sectionName))
                    return _sections[sectionName];
                return null;
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


        #region ICloneable Members

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return new IniData(Sections);
        }

        #endregion
    }
}