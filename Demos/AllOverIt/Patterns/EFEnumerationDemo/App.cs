using AllOverIt.Assertion;
using AllOverIt.GenericHost;
using EFEnumerationDemo.Entities;
using EFEnumerationDemo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EFEnumerationDemo
{
    public sealed class App : ConsoleAppBase
    {
        private readonly BloggingContext _dbContext;
        private readonly ILogger<App> _logger;

        public App(BloggingContext dbContext, ILogger<App> logger)
        {
            _dbContext = dbContext.WhenNotNull(nameof(dbContext));
            _logger = logger.WhenNotNull(nameof(logger));
        }

        public override async Task StartAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("StartAsync");

            await _dbContext.Database.MigrateAsync(cancellationToken);

            await CreateDataIfRequired(_dbContext);

            var query =
                from blog in _dbContext.Blogs
                from post in blog.Posts
                where blog.Id > 10 && blog.Id <= 15
                select new {blog.Description, post.Rating, post.Content};

            var results = await query.ToListAsync(cancellationToken);

            foreach (var result in results)
            {
                Console.WriteLine($"{result.Description} - {result.Rating.Value} - {result.Rating.Name} - {result.Content}");
            }

            ExitCode = 0;

            Console.WriteLine();
            Console.ReadKey();
        }

        public override void OnStopping()
        {
            _logger.LogInformation("App is stopping");
        }

        public override void OnStopped()
        {
            _logger.LogInformation("App is stopped");
        }

        private static async Task CreateDataIfRequired(BloggingContext dbContext)
        {
            var blogCount = await dbContext.Blogs.CountAsync();

            if (blogCount == 0)
            {
                var index = 0;
                var blogs = new List<Blog>();

                for (var blogIndex = 1; blogIndex <= 1000; blogIndex++)
                {
                    var blog = new Blog
                    {
                        Description = $"Description #{blogIndex}"
                    };

                    var posts = new List<Post>();

                    for (var postIndex = 1; postIndex <= 10; postIndex++)
                    {
                        var post = new Post
                        {
                            Title = $"Title #{postIndex}",
                            Content = $"Content #{postIndex}",
                            Rating = PostRating.From(index % 3)
                        };

                        posts.Add(post);
                        index++;
                    }

                    blog.Posts = posts;
                    blogs.Add(blog);
                }

                dbContext.Blogs.AddRange(blogs);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}