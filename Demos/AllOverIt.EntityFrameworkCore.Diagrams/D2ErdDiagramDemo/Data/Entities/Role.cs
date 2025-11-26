#nullable disable           // If enabled, string without [Required] would need to be changed to string?

using Microsoft.EntityFrameworkCore;

namespace D2ErdDiagramDemo.Data.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public sealed class Role : EntityBase
    {
        public required string Name { get; set; }

        public ICollection<Author> Authors { get; set; } = [];                // Skip navigation property (skips join table)
        public ICollection<Permission> Permissions { get; set; } = [];    // Skip navigation property (skips join table)
    }

}