namespace AllOverIt.EntityFrameworkCore.Diagrams
{
    /// <summary>Defines how the diagram generator will handle nullable and non-nullable columns.</summary>
    public enum NullableColumnMode
    {
        /// <summary>Indicates the generated diagram will indicate which columns are nullable.</summary>
        IsNull,

        /// <summary>Indicates the generated diagram will indicate which columns are non-nullable.</summary>
        NotNull
    }
}