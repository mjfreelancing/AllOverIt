namespace AllOverIt.Types
{
    /// <summary>Flags used to determine if obtaining information about a property is for its' getter, setter, or both.</summary>
    [Flags]
    public enum PropertyAccessor : byte
    {
        /// <summary>Refers to a property's getter.</summary>
        Get = 1,

        /// <summary>Refers to a property's setter (as a set).</summary>
        Set = 2,

        /// <summary>Refers to a property's setter (as an init).</summary>
        Init = 4
    }
}