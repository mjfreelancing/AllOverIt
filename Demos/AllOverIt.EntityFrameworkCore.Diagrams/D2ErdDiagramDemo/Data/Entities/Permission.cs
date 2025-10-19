#nullable disable           // If enabled, string without [Required] would need to be changed to string?

using Microsoft.EntityFrameworkCore;

namespace D2ErdDiagramDemo.Data.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public sealed class Permission : EntityBase
    {
        public required string Name { get; set; }

        // Not strictly necessary (since not used), but makes it clearer this is part of a many-to-many relationship
        public ICollection<Role> Roles { get; set; } = [];   // Skip navigation property (skips join table)
    }

}