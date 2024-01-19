using D2ErdDiagramDemo.Data.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace D2ErdDiagramDemo.Data.Entities
{
    [Table(nameof(Comment))]       // Enforce the name rather than take on the DbSet<> property name
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        [LongString]
        public string Content { get; set; }

        [Required]
        public Post Post { get; set; }

        [Required]
        public Author Author { get; set; }
    }
}