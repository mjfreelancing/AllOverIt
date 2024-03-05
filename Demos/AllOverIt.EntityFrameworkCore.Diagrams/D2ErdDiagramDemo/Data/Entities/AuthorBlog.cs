using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace D2ErdDiagramDemo.Data.Entities
{
    [Table(nameof(AuthorBlog))]       // Enforce the name rather than take on the DbSet<> property name
    public class AuthorBlog
    {
        public int Id { get; set; }

        [Required]
        public Author Author { get; set; }

        [Required]
        public Blog Blogger { get; set; }       // Deliberately using a name that is different from the type
    }
}