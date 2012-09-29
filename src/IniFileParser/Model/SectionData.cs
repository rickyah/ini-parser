using System;
using System.Collections.Generic;

namespace IniParser.Model
{
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
            if (string.IsNullOrEmpty(sectionName))
                throw new ArgumentException("section name can not be empty");

            _comments = new List<string>();
            _keyDataCollection = new KeyDataCollection();
            SectionName = sectionName;
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
            _comments = new List<string>(ori._comments);
            _keyDataCollection = new KeyDataCollection(ori._keyDataCollection);
        }

        #endregion

		#region Operations
		public void ClearKeyData()
		{
			_keyDataCollection.RemoveAllKeys();
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
                if (!string.IsNullOrEmpty(value))
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
}