namespace AllOverIt.Evaluator.Variables
{
    /// <summary>Determines how variable lookup operations are performed.</summary>
    public enum VariableLookupMode
    {
        /// <summary>The lookup process should only return explicitly referenced or referencing variables.</summary>
        Explicit,

        /// <summary>The lookup process should return all (explicit and implicit) referenced or referencing variables.</summary>
        All
    }
}