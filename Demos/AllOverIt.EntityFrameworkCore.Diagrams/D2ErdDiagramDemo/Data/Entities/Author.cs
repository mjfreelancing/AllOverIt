#nullable disable           // If enabled, string without [Required] would need to be changed to string?

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace D2ErdDiagramDemo.Data.Entities
{
    [Table(nameof(Author))]       // Enforce the name rather than take on the DbSet<> property name
    public class Author : EntityBase
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        // Not [Required] to show 'NULL' on the ERD
        // If the file has nullable enabled then this needs to change the string? and [Required] could be removed from 'string' columns
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(50)]
        public string Email { get; set; }

        public ICollection<Comment> Comments { get; set; }
        public ICollection<AuthorBlog> AuthorBlogs { get; set; }
    }
}