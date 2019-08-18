namespace IniParser
{
    /// <summary>
    /// Creates a deep copy of the type T, meaning that all reference types get
    ///  copied too instead of copying the reference.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDeepCloneable<T> where T : class                                             
    {
        T DeepClone();
    }
}
