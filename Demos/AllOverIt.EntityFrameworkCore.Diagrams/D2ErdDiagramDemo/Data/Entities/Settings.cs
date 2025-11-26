#nullable disable           // If enabled, string without [Required] would need to be changed to string?

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace D2ErdDiagramDemo.Data.Entities
{
    [Table(nameof(Settings))]       // Enforce the name rather than take on the DbSet<> property name
    public class Settings : EntityBase
    {
        [Required]
        public string JsonConfig { get; set; }

        // Applies a 1 to 0..1 relationship
        [Required]
        [ForeignKey("WebSiteId")]
        public WebSiteEntity WebSite { get; set; }
    }
}