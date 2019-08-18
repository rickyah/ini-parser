using System;
using System.Collections.Generic;

namespace IniParser.Model
{
    /// <summary>
    ///     Information associated to a section in a INI File
    ///     Includes both the value and the comments associated to the key.
    /// </summary>
    public class Section : ICloneable
    {
        IEqualityComparer<string> _searchComparer;
        #region Initialization

        public Section(string sectionName)
            :this(sectionName, EqualityComparer<string>.Default)
        {
            
        }
        /// <summary>
        ///     Initializes a new instance of the <see cref="Section"/> class.
        /// </summary>
        public Section(string sectionName, IEqualityComparer<string> searchComparer)
        {
            _searchComparer = searchComparer;

            if (string.IsNullOrEmpty(sectionName))
                throw new ArgumentException("section name can not be empty");

            _comments = new List<string>();
            _keyDataCollection = new PropertyCollection(_searchComparer);
            SectionName = sectionName;
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="Section"/> class
        ///     from a previous instance of <see cref="Section"/>.
        /// </summary>
        /// <remarks>
        ///     Data is deeply copied
        /// </remarks>
        /// <param name="ori">
        ///     The instance of the <see cref="Section"/> class 
        ///     used to create the new instance.
        /// </param>
        /// <param name="searchComparer">
        ///     Search comparer.
        /// </param>
        public Section(Section ori, IEqualityComparer<string> searchComparer = null)
        {
            SectionName = ori.SectionName;

            _searchComparer = searchComparer;
            _comments = new List<string>(ori._comments);
            _keyDataCollection = new PropertyCollection(ori._keyDataCollection, searchComparer ?? ori._searchComparer);
        }

        #endregion

		#region Operations

        /// <summary>
        ///     Deletes all comments in this section and in all the key/value pairs it contains
        /// </summary>
        public void ClearComments()
        {
            Comments.Clear();
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
        public void Merge(Section toMergeSection)
        {  
            Keys.Merge(toMergeSection.Keys);

            foreach(var comment in toMergeSection.Comments) 
                Comments.Add(comment);
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
				return _comments;
            }

            internal set
            {
                _comments = value;
            }



        }

        /// <summary>
        ///     Gets or sets the keys associated to this section.
        /// </summary>
        /// <value>
        ///     A collection of Property objects.
        /// </value>
        public PropertyCollection Keys
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
            return new Section(this);
        }

        #endregion

        #region Non-public members

        // Comments associated to this section
        private List<string> _comments;
        private List<string> _trailingComments = new List<string>();

        // Keys associated to this section
        private PropertyCollection _keyDataCollection;

        private string _sectionName;
        #endregion
    }
}