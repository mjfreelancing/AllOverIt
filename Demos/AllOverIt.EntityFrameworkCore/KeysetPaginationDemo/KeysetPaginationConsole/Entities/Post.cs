using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KeysetPaginationConsole.Entities
{
    // Includes several EnrichedEnum types that use a value converter to read/write the name value
    [Index(nameof(Id))]
    [Index(nameof(Title))]
    public class Post
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public Blog Blog { get; set; }
    }
}