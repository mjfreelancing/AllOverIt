#nullable disable           // If enabled, string without [Required] would need to be changed to string?

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace D2ErdDiagramDemo.Data.Entities
{
    [Table(nameof(Post))]       // Enforce the name rather than take on the DbSet<> property name
    public class Post : EntityBase
    {
        [Required]
        [MaxLength(500)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public Blog Blog { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}