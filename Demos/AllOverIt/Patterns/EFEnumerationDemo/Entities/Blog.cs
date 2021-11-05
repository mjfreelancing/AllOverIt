using EFEnumerationDemo.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EFEnumerationDemo.Entities
{
    public class Blog
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        public BlogStatus Status1 { get; set; }
        public BlogStatus Status2 { get; set; }
        public BlogStatus Status3 { get; set; }

        public ICollection<Post> Posts { get; set; }
    }
}