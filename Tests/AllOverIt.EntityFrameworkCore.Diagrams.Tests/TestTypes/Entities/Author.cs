using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllOverIt.EntityFrameworkCore.Diagrams.Tests.TestTypes.Entities
{
    [Table(nameof(Author))]       // Enforce the name rather than take on the DbSet<> property name
    public class Author
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        // Not [Required] to show 'NULL' on the ERD
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(50)]
        public string Email { get; set; }

        public ICollection<Comment> Comments { get; set; }
        public ICollection<AuthorBlog> AuthorBlogs { get; set; }
    }
}