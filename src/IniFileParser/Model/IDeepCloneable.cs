namespace IniParser
{
    public interface IDeepCloneable<T> where T : class
    {
        /// <summary>
        /// Returns a deep copy of an object of type T
        /// </summary>
        /// <returns>
        ///     A new instance of the object T where all its properties and fields
        ///     have been copied recusively.
        /// </returns>
        T DeepClone();
    }
}
