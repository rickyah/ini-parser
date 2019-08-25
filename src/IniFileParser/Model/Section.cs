using System;
using System.Collections.Generic;

namespace IniParser.Model
{
    /// <summary>
    ///     Information associated to a section in a INI File
    ///     Includes both the properties and the comments associated to the section.
    /// </summary>
    public class Section : IDeepCloneable<Section>
    {
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
                throw new ArgumentException("section name can not be empty", nameof(sectionName));

            Properties = new PropertyCollection(_searchComparer);
            Name = sectionName;
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
            Name = ori.Name;

            _searchComparer = searchComparer;
            Comments = ori.Comments;
            Properties = new PropertyCollection(ori.Properties, searchComparer ?? ori._searchComparer);
        }

        #endregion


        /// <summary>
        ///     Gets or sets the name of the section.
        /// </summary>
        /// <value>
        ///     The name of the section
        /// </value>
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                    _name = value;
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
        ///     Gets or sets the properties associated to this section.
        /// </summary>
        /// <value>
        ///     A collection of Property objects.
        /// </value>
        public PropertyCollection Properties { get; set; }

        /// <summary>
        ///     Deletes all comments and properties from this Section
        /// </summary>
        public void Clear()
        {
            ClearProperties();
            ClearComments();
        }

        /// <summary>
        ///     Deletes all comments in this section and in all the properties pairs it contains
        /// </summary>
        public void ClearComments()
        {
            Comments.Clear();
            Properties.ClearComments();
        }

        /// <summary>
        /// Deletes all the properties pairs in this section.
        /// </summary>
		public void ClearProperties()
        {
            Properties.Clear();
        }

        /// <summary>
        ///     Merges otherSection into this, adding new properties if they 
        ///     did not existed or overwriting values if the properties already 
        ///     existed.
        /// </summary>
        /// <remarks>
        ///     Comments are also merged but they are always added, not overwritten.
        /// </remarks>
        /// <param name="toMergeSection"></param>
        public void Merge(Section toMergeSection)
        {
            Properties.Merge(toMergeSection.Properties);

            foreach (var comment in toMergeSection.Comments)
                Comments.Add(comment);
        }

        #region IDeepCloneable<T> Members
        public Section DeepClone()
        {
            return new Section(this);
        }
        #endregion

        #region Fields
        List<string> _comments;
        private string _name;
        readonly IEqualityComparer<string> _searchComparer;
        #endregion
    }
}