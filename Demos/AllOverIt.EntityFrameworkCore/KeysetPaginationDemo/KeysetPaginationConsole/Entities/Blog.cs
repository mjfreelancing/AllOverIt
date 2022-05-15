using System;
using KeysetPaginationConsole.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace KeysetPaginationConsole.Entities
{
    [Index(nameof(Description))]
    [Index(nameof(Description), nameof(Id))]
    [Index(nameof(Reference))]
    [Index(nameof(Reference), nameof(Id))]
    [Index(nameof(AnotherId))]
    public class Blog
    {
        public int Id { get; set; }
        public int? AnotherId { get; set; }

        [Required]
        public string Description { get; set; }

        public BlogStatus Status1 { get; set; }
        public BlogStatus Status2 { get; set; }
        public BlogStatus Status3 { get; set; }

        public Guid Reference { get; set; }

        public ICollection<Post> Posts { get; set; }
    }
}