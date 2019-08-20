using IniParser.Configuration;

namespace IniParser.Format
{
    /// <summary>
    ///     Formats a IniData structure to an string
    /// </summary>
    public interface IIniDataFormatter
    {
        /// <summary>
        ///     Produces an string for a given <see cref="IniData"/> structure
        /// </summary>
        /// <returns>String that represents an <see cref="IniData"/>.</returns>
        /// <param name="iniData">Ini data.</param>
        /// <param name="format">
        ///     Configuration used by this formatter when converting IniData
        ///     to an string
        /// </param>
        string Format(IniData iniData, IniFormattingConfiguration format);
    }
}