using System;
using IniParser.Configuration;

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
        {
            Sections = new SectionCollection(StringComparer.OrdinalIgnoreCase);
            Global = new PropertyCollection(StringComparer.OrdinalIgnoreCase);
            _scheme = new IniScheme();
        }

        public IniDataCaseInsensitive(IniScheme scheme)
        {
            Sections = new SectionCollection(StringComparer.OrdinalIgnoreCase);
            Global = new PropertyCollection(StringComparer.OrdinalIgnoreCase);
            _scheme = scheme.DeepClone();
        }


        /// <summary>
        /// Copies an instance of the <see cref="IniParser.Model.IniDataCaseInsensitive"/> class
        /// </summary>
        /// <param name="ori">Original </param>
        public IniDataCaseInsensitive(IniData ori)
            : this()
        {
            Global = ori.Global.DeepClone();
            Configuration = ori.Configuration.DeepClone();
            Sections = new SectionCollection(ori.Sections, StringComparer.OrdinalIgnoreCase);
        }
    }
    
} 