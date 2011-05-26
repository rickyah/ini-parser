using System;
using System.Collections;
using System.Collections.Generic;

namespace IniParser
{
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
            _keyData = new Dictionary<string, KeyData>();
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
            _keyData = new Dictionary<string, KeyData>();
            foreach ( string key in _keyData.Keys )
                _keyData.Add(key, (KeyData)ori._keyData[key].Clone() );
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
        /// The string with key's value or null
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
                    return;

                _keyData[keyName].Value = value;

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
            //if ( !Assert.StringHasNoBlankSpaces(keyName) )
            //    throw new ArgumentException("Key name is not valid");

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
        /// <param name="keyData">KeyData instance.</param>
        /// <returns>
        /// <c>true</c> if a new empty key was added 
        /// <c>false</c> otherwise.
        /// </returns>
        /// <exception cref="ArgumentException">If the key name is not valid.</exception>
        public bool AddKey(string keyName, KeyData keyData)
        {
            if (AddKey(keyName))
            {
                _keyData[keyName] = keyData;
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
        /// <param name="keyValue">Value associated to the kyy.</param>
        /// <returns>
        /// <c>true</c> if a new empty key was added 
        /// <c>false</c> otherwise.
        /// </returns>
        /// <exception cref="ArgumentException">If the key name is not valid.</exception>
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
            return null;
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

                AddKey(data.KeyName, data);
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
        private readonly Dictionary<string, KeyData> _keyData;

        #endregion

    }
}