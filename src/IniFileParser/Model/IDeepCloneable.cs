namespace IniParser
{
    public interface IDeepCloneable<T> where T : class
    {
        T DeepClone();
    }
}
