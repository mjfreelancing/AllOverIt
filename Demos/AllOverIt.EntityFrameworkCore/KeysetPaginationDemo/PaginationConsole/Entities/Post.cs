using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KeysetPaginationConsole.Entities
{
    [Index(nameof(Title), nameof(Id))]
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
    }
}