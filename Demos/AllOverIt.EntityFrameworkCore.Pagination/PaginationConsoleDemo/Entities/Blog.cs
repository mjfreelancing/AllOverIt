using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PaginationConsoleDemo.Entities
{
    [Index(nameof(Description), nameof(Id))]
    public class Blog
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        public ICollection<Post> Posts { get; set; }
    }
}