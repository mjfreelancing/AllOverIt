namespace AllOverIt.EntityFrameworkCore.Diagrams
{
    /// <summary>Defines constraint types.</summary>
    public enum ConstraintType
    {
        /// <summary>No constraint.</summary>
        None,

        /// <summary>A primary key.</summary>
        PrimaryKey,

        /// <summary>A foreign key.</summary>
        ForeignKey
    }
}