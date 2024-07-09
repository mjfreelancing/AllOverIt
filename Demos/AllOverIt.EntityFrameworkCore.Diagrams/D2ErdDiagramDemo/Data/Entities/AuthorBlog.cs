#nullable disable           // If enabled, string without [Required] would need to be changed to string?

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace D2ErdDiagramDemo.Data.Entities
{
    [Table(nameof(AuthorBlog))]       // Enforce the name rather than take on the DbSet<> property name
    public class AuthorBlog : EntityBase
    {
        // Deliberately using a name that is different from the type, and before 'Author' to test preserving column order
        [Required]
        public Blog Blogger { get; set; }

        [Required]
        public Author Author { get; set; }
    }
}