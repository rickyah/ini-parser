using System.Collections;
using System.Collections.Generic;

namespace IniParser.Model
{
    /// <summary>
    ///     Represents a collection of Keydata.
    /// </summary>
    public class PropertyCollection : IDeepCloneable<PropertyCollection>, IEnumerable<Property>
    {
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
            _properties = new Dictionary<string, Property>(_searchComparer);
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
        public PropertyCollection(PropertyCollection ori, IEqualityComparer<string> searchComparer)
            : this(searchComparer)
        {
            foreach (Property property in ori)
            {
                if (_properties.ContainsKey(property.Key))
                {
                    _properties[property.Key] = property.DeepClone();
                }
                else
                {
                    _properties.Add(property.Key, property.DeepClone());
                }
            }
        }

        #endregion

        /// <summary>
        ///     Gets or sets the value of a property.
        /// </summary>
        /// <remarks>
        ///     If we try to assign the value of a property which doesn't exists,
        ///     a new one is added with the kay and the value specified
        /// </remarks>
        /// <param name="keyName">
        ///     key of the property
        /// </param>
        public string this[string keyName]
        {
            get
            {
                if (_properties.ContainsKey(keyName))
                    return _properties[keyName].Value;

                return null;
            }

            set
            {
                if (!_properties.ContainsKey(keyName))
                {
                    Add(keyName);
                }

                _properties[keyName].Value = value;

            }
        }

        /// <summary>
        ///     Return the number of keys in the collection
        /// </summary>
        public int Count
        {
            get { return _properties.Count; }
        }

        /// <summary>
        ///     Adds a new key with the specified name and empty value and comments
        /// </summary>
        /// <param name="key">
        ///     New key to be added.
        /// </param>
        /// <returns>
        ///     true if the key was added  false if a key with the same name already exist 
        ///     in the collection
        /// </returns>
        public bool Add(string key)
        {
            if (!_properties.ContainsKey(key))
            {
                AddPropertyInternal(new Property(key, string.Empty));
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Adds a new property to the collection
        /// </summary>
        /// <param name="property">
        ///     Property instance.
        /// </param>
        /// <returns>
        ///     true if the property was added  false if a property with the
        ///     same key already exist in the collection
        /// </returns>
        public bool Add(Property property)
        {
            if (!_properties.ContainsKey(property.Key))
            {
                AddPropertyInternal(property);
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Adds a new property with the specified key and value to the collection
        /// </summary>
        /// <param name="key">
        ///     key of the new property to be added.
        /// </param>
        /// <param name="value">
        ///     Value associated to the property.
        /// </param>
        /// <returns>
        ///     true if the property was added, false if a key with the same 
        ///     name already exist in the collection.
        /// </returns>
        public bool Add(string key, string value)
        {
            if (!_properties.ContainsKey(key))
            {
                AddPropertyInternal(new Property(key, value));
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
        ///     Gets if a specified property with the given key name exists in 
        ///     the collection.
        /// </summary>
        /// <param name="keyName">
        ///     Key name to search
        /// </param>
        /// <returns>
        ///     true if a property with the givne exists in the collectoin
        ///     false otherwise
        /// </returns>
        public bool Contains(string keyName)
        {
            return _properties.ContainsKey(keyName);
        }

        /// <summary>
        /// Retrieves the data for a specified key given its name
        /// </summary>
        /// <param name="keyName">Name of the key to retrieve.</param>
        /// <returns>
        /// A <see cref="Property"/> instance holding
        /// the key information or <c>null</c> if the key wasn't found.
        /// </returns>
        public Property FindByKey(string keyName)
        {
            if (_properties.ContainsKey(keyName))
                return _properties[keyName];
            return null;
        }
       
        /// <summary>
        ///     Merges other Property into this, adding new properties if they 
        ///     did not existed or overwriting values if the properties already 
        ///     existed.
        /// </summary>
        /// <remarks>
        ///     Comments are also merged but they are always added, not overwritten.
        /// </remarks>
        /// <param name="propertyToMerge"></param>
        public void Merge(PropertyCollection propertyToMerge)
        {
            foreach (var keyData in propertyToMerge)
            {
                Add(keyData.Key);
                this[keyData.Key] = keyData.Value;
                FindByKey(keyData.Key).Comments.AddRange(keyData.Comments);
            }
        }

        /// <summary>
        /// 	Deletes all properties in this collection.
        /// </summary>
        public void Clear()
        {
            _properties.Clear();
        }

        /// <summary>
        /// Deletes a previously existing key, including its associated data.
        /// </summary>
        /// <param name="keyName">The key to be removed.</param>
        /// <returns>
        /// true if a key with the specified name was removed 
        /// false otherwise.
        /// </returns>
        public bool Remove(string keyName)
        {
            return _properties.Remove(keyName);
        }

        #region IEnumerable<KeyData> Members

        /// <summary>
        /// Allows iteration througt the collection.
        /// </summary>
        /// <returns>A strong-typed IEnumerator </returns>
        public IEnumerator<Property> GetEnumerator()
        {
            foreach (string key in _properties.Keys)
                yield return _properties[key];
        }

        #region IEnumerable Members

        /// <summary>
        /// Implementation needed
        /// </summary>
        /// <returns>A weak-typed IEnumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _properties.GetEnumerator();
        }

        #endregion

        #endregion

        #region IDeepCloneable Members

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public PropertyCollection DeepClone()
        {
            return new PropertyCollection(this, _searchComparer);
        }

        #endregion

        #region Helpers
        // Adds a property w/out checking if it is already contained in the dictionary
        internal void AddPropertyInternal(Property property)
        {
            _lastAdded = property;
            _properties.Add(property.Key, property);
        }
        #endregion

        #region Non-public Members
        // Hack for getting the last property value (if exists) w/out using LINQ
        // that will cause allocations
        Property _lastAdded;
        internal Property GetLast()
        {
            return _lastAdded;
        }

        /// <summary>
        /// Collection of Property for a given section
        /// </summary>
        private readonly Dictionary<string, Property> _properties;

        readonly IEqualityComparer<string> _searchComparer;
        #endregion
    }
}