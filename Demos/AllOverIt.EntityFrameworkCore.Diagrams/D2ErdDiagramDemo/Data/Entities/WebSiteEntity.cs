#nullable disable           // If enabled, string without [Required] would need to be changed to string?

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace D2ErdDiagramDemo.Data.Entities
{
    [Table("WebSite")]       // Enforce the name rather than take on the DbSet<> property name
    public class WebSiteEntity : EntityBase
    {
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        public Settings Settings { get; set; }

        public ICollection<Blog> Blogs { get; set; }
    }
}