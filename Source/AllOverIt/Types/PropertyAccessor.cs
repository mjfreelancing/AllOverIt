namespace AllOverIt.Types
{
    /// <summary>Flags used to determine if obtaining information about a property is for its' getter, setter, or both.</summary>
    [Flags]
    public enum PropertyAccessor
    {
        /// <summary>Indicates the property's getter.</summary>
        Get = 1,

        /// <summary>Indicates the property's setter.</summary>
        Set = 2
    }
}