using EFEnumerationDemo.Models;
using System.ComponentModel.DataAnnotations;

namespace EFEnumerationDemo.Entities
{
    public class Post
    {
        public int Id { get; set; }
     
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        [MaxLength(25)]
        public PostRating Rating { get; set; }      // This is an EnrichedEnum that uses a value converter to read/write the name value

        [Required]
        public Blog Blog { get; set; }
    }
}