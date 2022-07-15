using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Filtering.Extensions;
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
        private readonly IDbContextFactory<BloggingContext> _dbContextFactory;
        private readonly ILogger<App> _logger;

        public App(IDbContextFactory<BloggingContext> dbContextFactory, ILogger<App> logger)
        {
            _dbContextFactory = dbContextFactory.WhenNotNull(nameof(dbContextFactory));
            _logger = logger.WhenNotNull(nameof(logger));
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StartAsync");

            using (var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken))
            {
                dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                //await dbContext.Database.EnsureDeletedAsync(cancellationToken);
                await dbContext.Database.MigrateAsync(cancellationToken);

                await CreateDataIfRequired();

                // properties are initialized using implicit operators
                var filter = new BlogFilter
                {
                    EqualToInt = 4,
                    EqualToString = "abc",
                    GreaterThan = 9,
                    GreaterThanOrEqual = 10,
                    LessThan = 15,                  // same as new LessThan<int>(15)
                    LessThanOrEqual = 13,
                    Contains = "#12",
                    StartsWith = "#1",
                    EndsWith = "##",
                    In = new List<int>(new[] { 10, 11 })
                };

                var query = dbContext.Blogs

                    // WHERE ((`b`.`Id` > 9) AND (`b`.`Id` < 15)) OR ((`b`.`Description` LIKE '%#12%') OR (`b`.`Description` LIKE '#1%'))
                    .ApplyFilter(filter, (specificationBuilder, filterBuilder) =>
                    {
                        // Use Create() for building a single specification against a property
                        var s1 = specificationBuilder.Create(blog => blog.Id, f => f.In);

                        // These methods require the two operations to act on the same property, hence must support the same type
                        var s2 = specificationBuilder.And(blog => blog.Id, f => f.GreaterThan, f => f.LessThan);
                        var s3 = specificationBuilder.Or(blog => blog.Description, f => f.Contains, f => f.StartsWith);
                        var s4 = specificationBuilder.Or(blog => blog.Id, f => f.In, f => f.LessThan);

                        var s5 = specificationBuilder.Create(blog => blog.Description, f => f.EndsWith);
                        var s6 = specificationBuilder.Create(blog => blog.Id, f => f.EqualToInt);

                        // A mismatch on property type will result in:
                        // System.InvalidOperationException: Unknown operation EqualTo<Int32> for blog => blog.Description.
                        // var s7 = specificationBuilder.Create(blog => blog.Description, f => f.EqualToInt);

                        var s7 = specificationBuilder.Create(blog => blog.Description, f => f.EqualToString);
                        var s8 = specificationBuilder.Create(blog => blog.Id, f => f.GreaterThanOrEqual);
                        var s9 = specificationBuilder.Create(blog => blog.Id, f => f.LessThanOrEqual);


                        filterBuilder
                            .Where(s1)
                            .And(s2)
                            .Or(s3)
                            .Or(s4)
                            .Or(s5)
                            .Or(s6)
                            .Or(s7)
                            .Or(s8)
                            .Or(s9);
                    })

                    .Join(dbContext.Posts,
                        blog => blog.Id,
                        post => post.Blog.Id,
                        (blog, post) => new
                        {
                            blog.Id,
                            blog.Description,
                            post.Rating,
                            post.Content
                        });

                var results = await query.ToListAsync(cancellationToken);

                Console.WriteLine();

                foreach (var result in results.Take(1))     // TEMP Take()
                {
                    Console.WriteLine($"{result.Id} - {result.Description} - {result.Rating.Value} - {result.Rating.Name} - {result.Content}");
                }
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

        private async Task CreateDataIfRequired()
        {
            using (var dbContext = await _dbContextFactory.CreateDbContextAsync())
            {
                var blogCount = await dbContext.Blogs.CountAsync();

                if (blogCount == 0)
                {
                    var postIndex = 0;
                    var blogs = new List<Blog>();

                    for (var blogIndex = 1; blogIndex <= 1000; blogIndex++)
                    {
                        var blog = new Blog
                        {
                            Description = $"Description #{blogIndex}",
                            Status1 = BlogStatus.From((postIndex + blogIndex) % 5),
                            Status3 = BlogStatus.From((postIndex + blogIndex + 5) % 5),
                        };

                        if (blogIndex % 2 == 0)
                        {
                            blog.Status2 = BlogStatus.From((postIndex + blogIndex + 3) % 5);
                        }

                        var posts = new List<Post>();

                        for (var idx = 1; idx <= 10; idx++)
                        {
                            var post = new Post
                            {
                                Title = $"Title #{idx}",
                                Content = $"Content #{idx}",
                                Rating = PostRating.From(postIndex % 3),
                                Status = PublishedStatus.From((postIndex + 2) % 3)
                            };

                            post.StatusValue = post.Status;
                            post.RatingValue = post.Rating;

                            posts.Add(post);
                            postIndex++;
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
}