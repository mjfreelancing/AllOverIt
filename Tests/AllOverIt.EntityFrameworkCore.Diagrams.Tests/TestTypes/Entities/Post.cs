using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllOverIt.EntityFrameworkCore.Diagrams.Tests.TestTypes.Entities
{
    [Table(nameof(Post))]       // Enforce the name rather than take on the DbSet<> property name
    public class Post
    {
        public int Id { get; set; }

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