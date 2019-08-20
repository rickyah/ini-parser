using System;
using System.Collections;
using System.Collections.Generic;

namespace IniParser.Model
{
    /// <summary>
    /// <para>Represents a collection of Section.</para>
    /// </summary>
    public class SectionCollection : IDeepCloneable<SectionCollection>, IEnumerable<Section>
    {
        IEqualityComparer<string> _searchComparer;
        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionCollection"/> class.
        /// </summary>
        public SectionCollection()
            :this(EqualityComparer<string>.Default)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="IniParser.Model.SectionCollection"/> class.
        /// </summary>
        /// <param name="searchComparer">
        ///     StringComparer used when accessing section names
        /// </param>
        public SectionCollection(IEqualityComparer<string> searchComparer)
        {
            _searchComparer = searchComparer;

            _sectionData = new Dictionary<string, Section>(_searchComparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionCollection"/> class
        /// from a previous instance of <see cref="SectionCollection"/>.
        /// </summary>
        /// <remarks>
        /// Data is deeply copied
        /// </remarks>
        /// <param name="ori">
        /// The instance of the <see cref="SectionCollection"/> class 
        /// used to create the new instance.</param>
        public SectionCollection(SectionCollection ori, IEqualityComparer<string> searchComparer)
        {
            _searchComparer = searchComparer ?? EqualityComparer<string>.Default;
                
            _sectionData = new Dictionary<string, Section>(_searchComparer);
            foreach(var sectionData in ori)
            {
                _sectionData.Add(sectionData.SectionName, sectionData.DeepClone());
            };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the number of Section elements in the collection
        /// </summary>
        public int Count { get { return _sectionData.Count; } }

        /// <summary>
        /// Gets the key data associated to a specified section name.
        /// </summary>
        /// <value>An instance of as <see cref="PropertyCollection"/> class 
        /// holding the key data from the current parsed INI data, or a <c>null</c>
        /// value if the section doesn't exist.</value>
        public PropertyCollection this[string sectionName]
        {
            get
            {
                if ( _sectionData.ContainsKey(sectionName) )
                    return _sectionData[sectionName].Properties;

                return null;
            }
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Creates a new section with empty data.
        /// </summary>
        /// <remarks>
        /// <para>If a section with the same name exists, this operation has no effect.</para>
        /// </remarks>
        /// <param name="sectionName">Name of the section to be created</param>
        /// <return>true if the a new section with the specified name was added,
        /// false otherwise</return>
        /// <exception cref="ArgumentException">If the section name is not valid.</exception>
        public bool AddSection(string sectionName)
        {
            if ( !ContainsSection(sectionName) )
            {
                _sectionData.Add( sectionName, new Section(sectionName, _searchComparer) );
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Adds a new Section instance to the collection
        /// </summary>
        /// <param name="data">Data.</param>
        public void Add(Section data)
        {
            if (ContainsSection(data.SectionName))
            {
                SetSectionData(data.SectionName, new Section(data, _searchComparer));
            }
            else
            {
                _sectionData.Add(data.SectionName, new Section(data, _searchComparer));
            }
        }

        /// <summary>
        /// Removes all entries from this collection
        /// </summary>
        public void Clear()
        {
            _sectionData.Clear();
        }


        /// <summary>
        /// Gets if a section with a specified name exists in the collection.
        /// </summary>
        /// <param name="keyName">Name of the section to search</param>
        /// <returns>
        /// true if a section with the specified name exists in the
        ///  collection false otherwise
        /// </returns>
        public bool ContainsSection(string keyName)
        {
            return _sectionData.ContainsKey(keyName);
        }

        /// <summary>
        /// Returns the section data from a specify section given its name.
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        /// <returns>
        /// An instance of a <see cref="Section"/> class 
        /// holding the section data for the currently INI data
        /// </returns>
        public Section GetSectionData(string sectionName)
        {
            if (_sectionData.ContainsKey(sectionName))
                return _sectionData[sectionName];

            return null;
        }

        public void Merge(SectionCollection sectionsToMerge)
        {
            foreach(var sectionDataToMerge in sectionsToMerge)
            {
                var sectionDataInThis = GetSectionData(sectionDataToMerge.SectionName);

                if (sectionDataInThis == null)
                {
                    AddSection(sectionDataToMerge.SectionName);
                }

                this[sectionDataToMerge.SectionName].Merge(sectionDataToMerge.Properties);
            }
        }

        /// <summary>
        /// Sets the section data for given a section name.
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="data">The new <see cref="Section"/>instance.</param>
        public void SetSectionData(string sectionName, Section data)
        {
            if ( data != null )
                _sectionData[sectionName] = data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyName"></param>
        /// <return>true if the section with the specified name was removed, 
        /// false otherwise</return>
        public bool RemoveSection(string keyName)
        {
            return _sectionData.Remove(keyName);
        }


        #endregion

        #region IEnumerable<SectionData> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<Section> GetEnumerator()
        {
            foreach (string sectionName in _sectionData.Keys)
                yield return _sectionData[sectionName];
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IDeepCloneable Members

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public SectionCollection DeepClone()
        {
            return new SectionCollection(this, _searchComparer);
        }

        #endregion

        #region Non-public Members

        /// <summary>
        /// Data associated to this section
        /// </summary>
        private readonly Dictionary<string, Section> _sectionData;

        #endregion

    }
}