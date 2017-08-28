namespace IniParser
{
    public interface IOverwritable<T>
    {
        /// <summary>
        /// Replaces contents the contents of the callr instance with the values
        /// from the instance passed as parameter.
        /// </summary>
        /// <param name="ori">
        ///     Object instance the caller object is to be overwritten with.
        ///     If ori is <c>null<c> nothing is overwritten, and the call has
        ///     no effect
        /// </param>
        void OverwriteWith(T ori);
    }
}
