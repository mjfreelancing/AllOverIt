using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllOverIt.EntityFrameworkCore.Diagrams.Tests.TestTypes.Entities
{
    [Table(nameof(WebSite))]       // Enforce the name rather than take on the DbSet<> property name
    public class WebSite
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        public Settings Settings { get; set; }

        public ICollection<Blog> Blogs { get; set; }
    }
}