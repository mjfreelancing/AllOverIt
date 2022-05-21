using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KeysetPaginationConsole.Entities
{
    [Index(nameof(Description))]
    [Index(nameof(Description), nameof(Id))]
    public class Blog
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        public ICollection<Post> Posts { get; set; }
    }
}