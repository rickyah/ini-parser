using System;
using System.Collections.Generic;

namespace IniParser.Model
{
    /// <summary>
    ///     Information associated to a section in a INI File
    ///     Includes both the value and the comments associated to the key.
    /// </summary>
    public class SectionData : ICloneable
    {
        IEqualityComparer<string> _searchComparer;
        #region Initialization

        public SectionData(string sectionName)
            :this(sectionName, EqualityComparer<string>.Default)
        {
            
        }
        /// <summary>
        ///     Initializes a new instance of the <see cref="SectionData"/> class.
        /// </summary>
        public SectionData(string sectionName, IEqualityComparer<string> searchComparer)
        {
            _searchComparer = searchComparer;

            if (string.IsNullOrEmpty(sectionName))
                throw new ArgumentException("section name can not be empty");

            _leadingComments = new List<string>();
            _keyDataCollection = new KeyDataCollection(_searchComparer);
            SectionName = sectionName;
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="SectionData"/> class
        ///     from a previous instance of <see cref="SectionData"/>.
        /// </summary>
        /// <remarks>
        ///     Data is deeply copied
        /// </remarks>
        /// <param name="ori">
        ///     The instance of the <see cref="SectionData"/> class 
        ///     used to create the new instance.
        /// </param>
        /// <param name="searchComparer">
        ///     Search comparer.
        /// </param>
        public SectionData(SectionData ori, IEqualityComparer<string> searchComparer = null)
        {
            SectionName = ori.SectionName;

            _searchComparer = searchComparer;
            _leadingComments = new List<string>(ori._leadingComments);
            _keyDataCollection = new KeyDataCollection(ori._keyDataCollection, searchComparer ?? ori._searchComparer);
        }

        #endregion

		#region Operations

        /// <summary>
        ///     Deletes all comments in this section and key/value pairs
        /// </summary>
        public void ClearComments()
        {
            LeadingComments.Clear();
            TrailingComments.Clear();
            Keys.ClearComments();
        }

        /// <summary>
        /// Deletes all the key-value pairs in this section.
        /// </summary>
		public void ClearKeyData()
		{
			Keys.RemoveAllKeys();
		}

        /// <summary>
        ///     Merges otherSection into this, adding new keys if they don't exists
        ///     or overwriting values if the key already exists.
        /// Comments get appended.
        /// </summary>
        /// <remarks>
        ///     Comments are also merged but they are always added, not overwritten.
        /// </remarks>
        /// <param name="toMergeSection"></param>
        public void Merge(SectionData toMergeSection)
        {
            foreach (var comment in toMergeSection.LeadingComments) 
                LeadingComments.Add(comment);
                
            Keys.Merge(toMergeSection.Keys);

            foreach(var comment in toMergeSection.TrailingComments) 
                TrailingComments.Add(comment);
        }

		#endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the name of the section.
        /// </summary>
        /// <value>
        ///     The name of the section
        /// </value>
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


		[Obsolete("Do not use this property, use property Comments instead")]
        public List<string> LeadingComments
        {
            get
            {
                return _leadingComments;
            }

            internal set
            {
                _leadingComments = new List<string>(value);
            }
        }

        /// <summary>
        ///     Gets or sets the comment list associated to this section.
        /// </summary>
        /// <value>
        ///     A list of strings.
        /// </value>
        public List<string> Comments
        {
            get
            {
				return _leadingComments;
            }


        }

		[Obsolete("Do not use this property, use property Comments instead")]
        public List<string> TrailingComments
        {
            get
            {
                return _trailingComments;
            }

            internal set
            {
                _trailingComments = new List<string>(value);
            }
        }
        /// <summary>
        ///     Gets or sets the keys associated to this section.
        /// </summary>
        /// <value>
        ///     A collection of KeyData objects.
        /// </value>
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
        ///     Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///     A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return new SectionData(this);
        }

        #endregion

        #region Non-public members

        // Comments associated to this section
        private List<string> _leadingComments;
        private List<string> _trailingComments = new List<string>();

        // Keys associated to this section
        private KeyDataCollection _keyDataCollection;

        private string _sectionName;
        #endregion



    }
}