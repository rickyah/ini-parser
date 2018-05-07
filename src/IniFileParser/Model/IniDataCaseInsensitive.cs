using System;
using IniParser.Configuration;
using IniParser.Formatting;

namespace IniParser.Model
{
    /// <summary>
    ///     Represents all data from an INI file exactly as the <see cref="IniData"/>
    ///     class, but searching for sections and keys names is done with
    ///     a case insensitive search.
    /// </summary>
    public class IniDataCaseInsensitive : IniData
    {
        /// <summary>
        ///     Initializes an empty IniData instance.
        /// </summary>
        public IniDataCaseInsensitive()
            : base (new SectionCollection(StringComparer.OrdinalIgnoreCase))
        {
            Global = new PropertyCollection(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     Initializes a new IniData instance using a previous
        ///     <see cref="SectionCollection"/>.
        /// </summary>
        /// <param name="sdc">
        ///     <see cref="SectionCollection"/> object containing the
        ///     data with the sections of the file
        /// </param>
        public IniDataCaseInsensitive(SectionCollection sdc)
            : base (new SectionCollection(sdc, StringComparer.OrdinalIgnoreCase))
        {
            Global = new PropertyCollection(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Copies an instance of the <see cref="IniParser.Model.IniDataCaseInsensitive"/> class
        /// </summary>
        /// <param name="ori">Original </param>
        public IniDataCaseInsensitive(IniData ori)
            : this(new SectionCollection(ori.Sections, StringComparer.OrdinalIgnoreCase))
        {
            Global = (PropertyCollection) ori.Global.Clone();
            //Configuration = ori.Configuration.Clone();
        }
    }
    
    
        public class IniDataCaseInsensitive2 : IniData
    {
        /// <summary>
        ///     Initializes an empty IniData instance.
        /// </summary>
        public IniDataCaseInsensitive2()
            : base (new SectionCollection(StringComparer.OrdinalIgnoreCase))
        {
            Global = new PropertyCollection(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     Initializes a new IniData instance using a previous
        ///     <see cref="SectionCollection"/>.
        /// </summary>
        /// <param name="sdc">
        ///     <see cref="SectionCollection"/> object containing the
        ///     data with the sections of the file
        /// </param>
        public IniDataCaseInsensitive2(SectionCollection sdc)
            : base (new SectionCollection(sdc, StringComparer.OrdinalIgnoreCase))
        {
            Global = new PropertyCollection(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Copies an instance of the <see cref="IniParser.Model.IniDataCaseInsensitive"/> class
        /// </summary>
        /// <param name="ori">Original </param>
        public IniDataCaseInsensitive2(IniData ori)
            : this(new SectionCollection(ori.Sections, StringComparer.OrdinalIgnoreCase))
        {
            Global = (PropertyCollection) ori.Global.Clone();
            //Configuration = ori.Configuration.Clone();
        }
    }
    
} 