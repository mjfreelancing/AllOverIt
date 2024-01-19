using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace D2ErdDiagramDemo.Data.Entities
{
    [Table(nameof(Blog))]       // Enforce the name rather than take on the DbSet<> property name
    public class Blog
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public WebSite WebSite { get; set; }

        public ICollection<Post> Posts { get; set; }
        public ICollection<AuthorBlog> AuthorBlogs { get; set; }
    }
}