using System;
using System.Collections;
using System.Collections.Generic;

namespace IniParser.Model
{
    /// <summary>
    ///     Represents a collection of Keydata.
    /// </summary>
    public class PropertyCollection : ICloneable, IEnumerable<Property>
    {
        IEqualityComparer<string> _searchComparer;
        #region Initialization

        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyCollection"/> class.
        /// </summary>
        public PropertyCollection()
            : this(EqualityComparer<string>.Default)
        { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyCollection"/> class with a given
        ///     search comparer
        /// </summary>
        /// <param name="searchComparer">
        ///     Search comparer used to find the key by name in the collection
        /// </param>
        public PropertyCollection(IEqualityComparer<string> searchComparer)
        {
            _searchComparer = searchComparer;
            _keyData = new Dictionary<string, Property>(_searchComparer);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyCollection"/> class
        ///     from a previous instance of <see cref="PropertyCollection"/>.
        /// </summary>
        /// <remarks>
        ///     Data from the original KeyDataCollection instance is deeply copied
        /// </remarks>
        /// <param name="ori">
        ///     The instance of the <see cref="PropertyCollection"/> class 
        ///     used to create the new instance.
        /// </param>
        /// <param name="searchComparer">
        ///     Allows using a custom comparision strategy when looking up for keys
        ///     e.g case sensitive search (default)
        /// </param>
        public PropertyCollection(PropertyCollection ori, IEqualityComparer<string> searchComparer)
            : this(searchComparer)
        {
            foreach (Property key in ori)
            {
                if (_keyData.ContainsKey(key.KeyName))
                {
                    _keyData[key.KeyName] = (Property)key.Clone();
                }
                else
                {
                    _keyData.Add(key.KeyName, (Property)key.Clone());
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the value of a concrete key.
        /// </summary>
        /// <remarks>
        ///     If we try to assign the value of a key which doesn't exists,
        ///     a new key is added with the name and the value is assigned to it.
        /// </remarks>
        /// <param name="keyName">
        ///     Name of the key
        /// </param>
        /// <returns>
        ///     The string with key's value or null if the key was not found.
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
                    this.AddKey(keyName);
                }

                _keyData[keyName].Value = value;

            }
        }

        /// <summary>
        ///     Return the number of keys in the collection
        /// </summary>
        public int Count
        {
            get { return _keyData.Count; }
        }

        #endregion

        #region Operations

        /// <summary>
        ///     Adds a new key with the specified name and empty value and comments
        /// </summary>
        /// <param name="keyName">
        ///     New key to be added.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the key was added  <c>false</c> if a key with the same name already exist 
        ///     in the collection
        /// </returns>
        public bool AddKey(string keyName)
        {
            if (!_keyData.ContainsKey(keyName))
            {
                _keyData.Add(keyName, new Property(keyName));
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Adds a new key to the collection
        /// </summary>
        /// <param name="keyData">
        ///     KeyData instance.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the key was added  <c>false</c> if a key with the same name already exist 
        ///     in the collection
        /// </returns>
        public bool AddKey(Property keyData)
        {
            if (AddKey(keyData.KeyName))
            {
                _keyData[keyData.KeyName] = keyData;
                return true;
            }

            return false;
        }
        /// <summary>
        ///     Adds a new key with the specified name and value to the collection
        /// </summary>
        /// <param name="keyName">
        ///     Name of the new key to be added.
        /// </param>
        /// <param name="keyValue">
        ///     Value associated to the key.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the key was added  <c>false</c> if a key with the same name already exist 
        ///     in the collection.
        /// </returns>
        public bool AddKey(string keyName, string keyValue)
        {
            if (AddKey(keyName))
            {
                _keyData[keyName].Value = keyValue;
                return true;
            }

            return false;

        }

        /// <summary>
        ///     Clears all comments of this section
        /// </summary>
        public void ClearComments()
        {
            foreach (var keydata in this)
            {
                keydata.Comments.Clear();
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
        /// Retrieves the data for a specified key given its name
        /// </summary>
        /// <param name="keyName">Name of the key to retrieve.</param>
        /// <returns>
        /// A <see cref="Property"/> instance holding
        /// the key information or <c>null</c> if the key wasn't found.
        /// </returns>
        public Property GetKeyData(string keyName)
        {
            if (_keyData.ContainsKey(keyName))
                return _keyData[keyName];
            return null;
        }

        public void Merge(PropertyCollection keyDataToMerge)
        {
            foreach (var keyData in keyDataToMerge)
            {
                AddKey(keyData.KeyName);
                GetKeyData(keyData.KeyName).Comments.AddRange(keyData.Comments);
                this[keyData.KeyName] = keyData.Value;
            }

        }

        /// <summary>
        /// 	Deletes all keys in this collection.
        /// </summary>
        public void RemoveAllKeys()
        {
            _keyData.Clear();
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
        /// <summary>
        /// Sets the key data associated to a specified key.
        /// </summary>
        /// <param name="data">The new <see cref="Property"/> for the key.</param>
        public void SetKeyData(Property data)
        {
            if (data == null) return;

            if (_keyData.ContainsKey(data.KeyName))
                RemoveKey(data.KeyName);

            AddKey(data);
        }

        #endregion

        #region IEnumerable<KeyData> Members

        /// <summary>
        /// Allows iteration througt the collection.
        /// </summary>
        /// <returns>A strong-typed IEnumerator </returns>
        public IEnumerator<Property> GetEnumerator()
        {
            foreach (string key in _keyData.Keys)
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
            return new PropertyCollection(this, _searchComparer);
        }

        #endregion

        #region Non-public Members
        // Hack for getting the last key value (if exists) w/out using LINQ
        // and maintain support for earlier versions of .NET
        internal Property GetLast()
        {
            Property result = null;
            if (_keyData.Keys.Count <= 0) return result;


            foreach (var k in _keyData.Keys) result = _keyData[k];
            return result;
        }

        /// <summary>
        /// Collection of KeyData for a given section
        /// </summary>
        private readonly Dictionary<string, Property> _keyData;

        #endregion

    }
}