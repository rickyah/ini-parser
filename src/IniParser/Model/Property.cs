using System;
using System.Collections.Generic;

namespace IniParser.Model
{
    /// <summary>
    ///     Information associated to a property from an INI file.
    ///     Includes both the key, the value and the comments associated to 
    ///     the property.
    /// </summary>
    public class Property : IDeepCloneable<Property>
    {
        #region Initialization

        /// <summary>
        ///     Initializes a new instance of the <see cref="Property"/> class.
        /// </summary>
        public Property(string keyName, string value = "")
        {
            if (string.IsNullOrEmpty(keyName))
                throw new ArgumentException("key name can not be empty", nameof(Key));

            Value = value;
            Key = keyName;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Property"/> class
        ///     from a previous instance of <see cref="Property"/>.
        /// </summary>
        /// <remarks>
        ///     Data is deeply copied
        /// </remarks>
        /// <param name="ori">
        ///     The instance of the <see cref="Property"/> class 
        ///     used to create the new instance.
        /// </param>
        public Property(Property ori)
        {
            Value = ori.Value;
            Key = ori.Key;
            Comments = ori.Comments;
        }   

        #endregion Constructors 

        #region Properties 

        /// <summary>
        /// Gets or sets the comment list associated to this property.
        /// Makes a copy og the values when set
        /// </summary>
        public List<string> Comments
        {
            get
            {
                if (_comments == null)
                {
                    _comments = new List<string>();
                }

                return _comments;
            }

            set
            {
                if (_comments == null)
                {
                    _comments = new List<string>();
                }
                _comments.Clear();
                _comments.AddRange(value);
            }
        }

        /// <summary>
        ///     Gets or sets the value associated to this property.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        ///     Gets or sets the name of this property.
        /// </summary>
        public string Key { get; set; }


        #endregion Properties 

        #region IDeepCloneable<T> Members

        public Property DeepClone()
        {
            return new Property(this);
        }

        #endregion

        #region Non-public Members

        // List with comment lines associated to this property 
        private List<string> _comments;

        #endregion
    }
}
