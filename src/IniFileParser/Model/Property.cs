using System;
using System.Collections.Generic;

namespace IniParser.Model
{
    /// <summary>
    ///     Information associated to a key from an INI file.
    ///     Includes both the value and the comments associated to the key.
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
                throw new ArgumentException("key name can not be empty", nameof(KeyName));

            _comments = new List<string>();
            Value = value;
            KeyName = keyName;
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
            KeyName = ori.KeyName;
            _comments = new List<string>(ori._comments);
        }

        #endregion Constructors 

        #region Properties 

        /// <summary>
        /// Gets or sets the comment list associated to this key.
        /// Makes a copy og the values when set
        /// </summary>
        public List<string> Comments
        {
            get { return _comments; }
            set { _comments = new List<string> (value) ; }
        }

        /// <summary>
        ///     Gets or sets the value associated to this key.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        ///     Gets or sets the name of the key.
        /// </summary>
        public string KeyName { get; set; }


        #endregion Properties 

        #region IDeepCloneable<T> Members

        public Property DeepClone()
        {
            return new Property(this);
        }

        #endregion

        #region Non-public Members

        // List with comment lines associated to this key 
        private List<string> _comments;

        #endregion
    }
}
