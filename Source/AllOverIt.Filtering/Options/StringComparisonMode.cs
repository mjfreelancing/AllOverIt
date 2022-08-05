namespace AllOverIt.Filtering.Options
{
    /// <summary>Provides the option to selectively enable case-insensitive string comparisons when required.</summary>
    public enum StringComparisonMode
    {
        /// <summary>Apply no modification to string values.</summary>
        None,

        /// <summary>Will lowercase input values (in advance) and lowercase each data row so string comparisons become case-insensitive.</summary>
        ToLower,

        /// <summary>Will uppercase input values (in advance) and uppercase each data row so string comparisons become case-insensitive.</summary>
        ToUpper
    }
}