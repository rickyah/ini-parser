using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace IniParser
{

    /// <summary>
    /// <para>Information associated to a key from an INI file.</para>
    /// <para>Includes both the value and the comments associated to the key.</para>
    /// </summary>
    public class KeyData : ICloneable
    {
		#region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyData"/> class.
        /// </summary>
        public KeyData(string keyName)
        {
            this._comments = new List<string>();
            this._value = string.Empty;
            this._keyName = keyName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyData"/> class
        /// from a previous instance of <see cref="KeyData"/>.
        /// </summary>
        /// <remarks>
        /// Data is deeply copied
        /// </remarks>
        /// <param name="ori">
        /// The instance of the <see cref="KeyData"/> class 
        /// used to create the new instance.</param>
        public KeyData(KeyData ori)
        {
            this._value = ori._value;

            this._comments = new List<string>(_comments);
        }

		#endregion Constructors 

		#region Properties 

        /// <summary>
        /// Gets or sets the comment list associated to this key.
        /// </summary>
        public List<string> Comments
        {
            get { return _comments; }
            set { _comments = new List<string> (value) ; }
        }

        /// <summary>
        /// Gets or sets the value associated to this key.
        /// </summary>
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// Gets or sets the name of the key.
        /// </summary>
        public string KeyName
        {
            get
            {
                return _keyName;
            }

            set
            {
                if (value != string.Empty)
                    _keyName = value;
            }

        }

		#endregion Properties 

        #region ICloneable Members

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return new KeyData(this);
        }

        #endregion

        #region Non-public Members

        /// <summary>
        /// List with comment lines associated to this key 
        /// </summary>
        private List<string> _comments;

        /// <summary>
        /// Unique value associated to this key
        /// </summary>
        private string _value;

        /// <summary>
        /// Name of the current key
        /// </summary>
        private string _keyName;

        #endregion
    }

    /// <summary>
    /// <para>Information associated to a section in a INI File</para>
    /// <para>Includes both the value and the comments associated to the key.</para>
    /// </summary>
    public class SectionData : ICloneable
    {
        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionData"/> class.
        /// </summary>
        public SectionData(string sectionName)
        {
            this._comments = new List<string>();
            this._keyDataCollection = new KeyDataCollection();
            this.SectionName = sectionName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionData"/> class
        /// from a previous instance of <see cref="SectionData"/>.
        /// </summary>
        /// <remarks>
        /// Data is deeply copied
        /// </remarks>
        /// <param name="ori">
        /// The instance of the <see cref="SectionData"/> class 
        /// used to create the new instance.</param>
        public SectionData(SectionData ori)
        {
            this._comments = new List<string>(ori._comments);
            this._keyDataCollection = new KeyDataCollection(ori._keyDataCollection);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the section.
        /// </summary>
        /// <value>The name of the section.</value>
        public string SectionName
        {
            get
            {
                return _sectionName;
            }

            set
            {
                if (value != string.Empty)
                    _sectionName = value;
            }
        }

        /// <summary>
        /// Gets or sets the comment list associated to this section.
        /// </summary>
        /// <value>A list of strings.</value>
        public List<string> Comments
        {
            get
            {
                return _comments;
            }

            set
            {
                _comments = new List<string>(value);
            }
        }

        /// <summary>
        /// Gets or sets the keys associated to this section.
        /// </summary>
        /// <value>A collection of KeyData objects.</value>
        public KeyDataCollection Keys
        {
            get
            {
                return _keyDataCollection;
            }

            set
            {
                _keyDataCollection = value;
            }
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
            return new SectionData(this);
        }

        #endregion

        #region Non-public members

        /// <summary>
        /// Comments associated to this section
        /// </summary>
        private List<string> _comments;

        /// <summary>
        /// Keys associated to this section
        /// </summary>
        private KeyDataCollection _keyDataCollection;

        /// <summary>
        /// Name of the u
        /// </summary>
        private string _sectionName;
        #endregion

    }

    /// <summary>
    /// <para>Represents a collection of Keydata.</para>
    /// </summary>
    public class KeyDataCollection : ICloneable, IEnumerable<KeyData>
    {
        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyDataCollection"/> class.
        /// </summary>
        public KeyDataCollection()
        {
            this._keyData = new Dictionary<string, KeyData>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyDataCollection"/> class
        /// from a previous instance of <see cref="KeyDataCollection"/>.
        /// </summary>
        /// <remarks>
        /// Data is deeply copied
        /// </remarks>
        /// <param name="ori">
        /// The instance of the <see cref="KeyDataCollection"/> class 
        /// used to create the new instance.</param>
        public KeyDataCollection(KeyDataCollection ori)
        {
            this._keyData = new Dictionary<string, KeyData>();
            foreach ( string key in _keyData.Keys )
                this._keyData.Add(key, (KeyData)ori._keyData[key].Clone() );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the value of a concrete key.
        /// </summary>
        /// <remarks>
        /// If we try to assign the value of a key which doesn't exists,
        /// a new key is added with the name and the value is assigned to it.
        /// </remarks>
        /// <param name="keyName">Name of the key</param>
        /// <returns>
        /// The string with key's value or <c>null</c>
        /// if the key was not found.
        /// </returns>
        public string this[string keyName]
        {
            get
            {
                if (_keyData.ContainsKey(keyName))
                    return _keyData[keyName].Value;

                return null;
            }

            set
            {
                if (!_keyData.ContainsKey(keyName))
                {
                    this.AddKey(keyName, value);
                    return;
                }
                else
                {
                    _keyData[keyName].Value = value;
                }

            }
        }

        /// <summary>
        /// Return the number of keys in the collection
        /// </summary>
        /// <value>An integer with the number of keys in the collection.</value>
        public int Count
        {
            get { return _keyData.Count; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a new key with the specified name and empty value and comments
        /// </summary>
        /// <remarks>
        /// A valid key name is a string with NO blank spaces.
        /// </remarks>
        /// <param name="keyName">New key to be added.</param>
        /// <returns>
        /// <c>true</c> if a new empty key was added 
        /// <c>false</c> otherwise.
        /// </returns>
        /// <exception cref="ArgumentException">If the key name is not valid.</exception>
        public bool AddKey(string keyName)
        {
            //Checks valid key name
            if ( !Assert.StringHasNoBlankSpaces(keyName) )
                throw new ArgumentException("Key name is not valid");

            if ( !_keyData.ContainsKey(keyName) )
            {
                _keyData.Add(keyName, new KeyData(keyName));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds a new key with the specified name and value and comments
        /// </summary>
        /// <remarks>
        /// A valid key name is a string with NO blank spaces.
        /// </remarks>
        /// <param name="keyName">New key to be added.</param>
        /// <param name="keyValue">Value associated to the kay.</param>
        /// <returns>
        /// <c>true</c> if a new empty key was added 
        /// <c>false</c> otherwise.
        /// </returns>
        /// <exception cref="ArgumentException">If the key name is not valid.</exception>
        public bool AddKey(string keyName, string keyValue)
        {
            if ( AddKey(keyName) )
            {
                _keyData[keyName].Value = keyValue;
                return true;
            }

            return false;

        }

        /// <summary>
        /// Retrieves the data for a specified key given its name
        /// </summary>
        /// <param name="keyName">Name of the key to retrieve.</param>
        /// <returns>
        /// A <see cref="KeyData"/> instance holding
        /// the key information or <c>null</c> if the key wasn't found.
        /// </returns>
        public KeyData GetKeyData(string keyName)
        {
            if (_keyData.ContainsKey(keyName))
                return _keyData[keyName];
            else return null;
        }

        /// <summary>
        /// Sets the key data associated to a specified key.
        /// </summary>
        /// <param name="data">The new <see cref="KeyData"/> for the key.</param>
        public void SetKeyData(KeyData data)
        {
            if (data != null)
            {
                if (_keyData.ContainsKey(data.KeyName))
                    RemoveKey(data.KeyName);

                AddKey(data.KeyName, data.Value);
            }
        }

        /// <summary>
        /// Gets if a specifyed key name exists in the collection.
        /// </summary>
        /// <param name="keyName">Key name to search</param>
        /// <returns><c>true</c> if a key with the specified name exists in the collectoin
        /// <c>false</c> otherwise</returns>
        public bool ContainsKey(string keyName)
        {
            return _keyData.ContainsKey(keyName);
        }

        /// <summary>
        /// Deletes a previously existing key, including its associated data.
        /// </summary>
        /// <param name="keyName">The key to be removed.</param>
        /// <returns>
        /// <c>true</c> if a key with the specified name was removed 
        /// <c>false</c> otherwise.
        /// </returns>
        public bool RemoveKey(string keyName)
        {
             return _keyData.Remove(keyName);
        }

        #endregion

        #region IEnumerable<KeyData> Members

        /// <summary>
        /// Allows iteration througt the collection.
        /// </summary>
        /// <returns>A strong-typed IEnumerator </returns>
        public IEnumerator<KeyData> GetEnumerator()
        {
            foreach ( string key in _keyData.Keys )
                yield return _keyData[key];
        }

        #region IEnumerable Members

        /// <summary>
        /// Implementation needed
        /// </summary>
        /// <returns>A weak-typed IEnumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _keyData.GetEnumerator();
        }

        #endregion

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
            return new KeyDataCollection(this);
        }

        #endregion

        #region Non-public Members

        /// <summary>
        /// Collection of KeyData for a given section
        /// </summary>
        private Dictionary<string, KeyData> _keyData;

                #endregion

    }

    /// <summary>
    /// <para>Represents a collection of SectionData.</para>
    /// </summary>
    public class SectionDataCollection : ICloneable, IEnumerable<SectionData>
    {
        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionDataCollection"/> class.
        /// </summary>
        public SectionDataCollection()
        {
            this._sectionData = new Dictionary<string, SectionData>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionDataCollection"/> class
        /// from a previous instance of <see cref="SectionDataCollection"/>.
        /// </summary>
        /// <remarks>
        /// Data is deeply copied
        /// </remarks>
        /// <param name="ori">
        /// The instance of the <see cref="SectionDataCollection"/> class 
        /// used to create the new instance.</param>
        public SectionDataCollection(SectionDataCollection ori)
        {
            this._sectionData = new Dictionary<string, SectionData> (ori._sectionData);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the number of SectionData elements in the collection
        /// </summary>
        public int Count
        {
            get { return _sectionData.Count; }
        }

        /// <summary>
        /// Gets the key data associated to a specified section name.
        /// </summary>
        /// <value>An instance of as <see cref="KeyDataCollection"/> class 
        /// holding the key data from the current parsed INI data, or a <c>null</c>
        /// value if the section doesn't exist.</value>
        public KeyDataCollection this[string sectionName]
        {
            get
            {
                if ( _sectionData.ContainsKey(sectionName) )
                    return _sectionData[sectionName].Keys;

                return null;
            }
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Creates a new section with empty data.
        /// </summary>
        /// <remarks>
        /// <para>A valid section name is a string with NO blank spaces.</para>
        /// <para>If a section with the same name exists, this operation has no effect.</para>
        /// </remarks>
        /// <param name="keyName">Name of the section to be created</param>
        /// <return><c>true</c> if the a new section with the specified name was added,
        /// <c>false</c> otherwise</return>
        /// <exception cref="ArgumentException">If the section name is not valid.</exception>
        public bool AddSection(string keyName)
        {
            //Checks valid section name
            if ( !Assert.StringHasNoBlankSpaces(keyName) )
                throw new ArgumentException("Key name is not valid");

            if ( !ContainsSection(keyName) )
            {
                _sectionData.Add( keyName, new SectionData(keyName) );
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets if a section with a specified name exists in the collection.
        /// </summary>
        /// <param name="keyName">Name of the section to search</param>
        /// <returns>
        /// <c>true</c> if a section with the specified name exists in the
        ///  collection <c>false</c> otherwise
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
        /// An instance of a <see cref="SectionData"/> class 
        /// holding the section data for the currently INI data
        /// </returns>
        public SectionData GetSectionData(string sectionName)
        {
            if (_sectionData.ContainsKey(sectionName))
                return _sectionData[sectionName];

            return null;
        }


        /// <summary>
        /// Sets the section data for given a section name.
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="data">The new <see cref="SectionData"/>instance.</param>
        public void SetSectionData( string sectionName, SectionData data)
        {
            if ( data != null )
                _sectionData[sectionName] = data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyName"></param>
        /// <return><c>true</c> if the section with the specified name was removed, 
        /// <c>false</c> otherwise</return>
         public bool RemoveSection(string keyName)
         {
                return _sectionData.Remove(keyName);
         }

        /// <summary>
        /// Removes all entries from this collection
        /// </summary>
        public void Clear()
        {
            _sectionData.Clear();
        }

        #endregion

        #region IEnumerable<SectionData> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<SectionData> GetEnumerator()
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
            return this.GetEnumerator();
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
            return new SectionDataCollection(this);
        }

        #endregion

        #region Non-public Members

        /// <summary>
        /// Data associated to this section
        /// </summary>
        private Dictionary<string, SectionData> _sectionData;

        #endregion

    }

    /// <summary>
    /// Represents all data from an INI file
    /// </summary>
    public class IniData
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
            _sections = sdc;
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

    }

    internal static class Assert
    {
        /// <summary>
        /// Asserts that a strings has no blank spaces.
        /// </summary>
        /// <param name="s">The string to be checked.</param>
        /// <returns></returns>
        internal static bool StringHasNoBlankSpaces(string s)
        {
            return !s.Contains(" ");
        }
    }

}
