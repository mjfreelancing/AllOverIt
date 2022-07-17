using AllOverIt.Assertion;
using AllOverIt.Filtering.Builders.Extensions;
using AllOverIt.Filtering.Extensions;
using AllOverIt.Filtering.Filters;
using AllOverIt.GenericHost;
using AllOverIt.Patterns.Specification;
using AllOverIt.Patterns.Specification.Extensions;
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

                var filter = new BlogFilter
                {
                    Id = {
                        EqualTo = 5,
                        NotEqualTo = 6,
                        GreaterThan = 10,
                        GreaterThanOrEqual = 15,
                        LessThan = 5,
                        LessThanOrEqual = 7,
                        In = new List<int>(new[]{1, 2, 3}),         // implicit conversion
                        NotIn = new NotIn<int>(new[]{4, 5, 6})      // constructor
                    },
                    Description = {
                        EqualTo = "#10",
                        NotEqualTo = "#100",
                        Contains = "2",
                        NotContains = "3",
                        StartsWith = "#",
                        EndsWith = "55"
                    }
                };

                // Or can initialize the above using normal property assignment, such as:
                // filter.Id.EqualTo = 5;

                var blogQuery = CreateFilteredBlogQuery(dbContext, filter);

                // The 'where blog.Id == 100' is included just to limit the results returned
                //
                //   SELECT `b`.`Id`, `b`.`Description`, `p`.`Rating`, `p`.`Content`
                //   FROM `Blogs` AS `b`
                //   INNER JOIN `Posts` AS `p` ON `b`.`Id` = `p`.`BlogId`
                //   WHERE (((((((`b`.`Id` = 5) AND(`b`.`Id` <> 6)) AND
                //     ((`b`.`Id` > 10) AND(`b`.`Id` < 5))) AND
                //     ((`b`.`Id` >= 15) OR(`b`.`Id` <= 7))) AND
                //     `b`.`Id` IN(1, 2, 3)) OR
                //     `b`.`Id` NOT IN(1, 2, 3)) OR
                //     ((((((`b`.`Description` = '#10') AND
                //     (`b`.`Description` <> '#100')) AND
                //     (`b`.`Description` LIKE '%2%')) AND
                //     NOT(`b`.`Description` LIKE '%3%')) AND
                //     (`b`.`Description` LIKE '#%')) AND
                //     (`b`.`Description` LIKE '%55'))) AND
                //     (`b`.`Id` = 100)
                var query = from blog in blogQuery
                            from post in blog.Posts
                            where blog.Id == 100
                            select new
                            {
                                blog.Id,
                                blog.Description,
                                post.Rating,
                                post.Content
                            };

                var queryString = query.ToQueryString();

                var results = await query.ToListAsync(cancellationToken);

                Console.WriteLine();

                foreach (var result in results)
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

        private static IQueryable<Blog> CreateFilteredBlogQuery(BloggingContext dbContext, BlogFilter blogFilter)
        {
            // This demo shows different approaches to combining options - the demo doesn't logically
            // make sense, but it's here to show different construct options. It assumes each option
            // has been initialized - so there's no null checking.
            //
            // Each call to filterBuilder.Where() will AND the criteria with the previous criteria.
            return dbContext.Blogs
                .AsQueryable()
                .ApplyFilter(blogFilter, (specificationBuilder, filterBuilder) =>
                {
                    // Id EqualTo / NotEqualTo - using individual specifications
                    var s1 = specificationBuilder.Create(blog => blog.Id, filter => filter.Id.EqualTo);
                    var s2 = specificationBuilder.Create(blog => blog.Id, filter => filter.Id.NotEqualTo);
                    filterBuilder.Where(s1.And(s2));

                    // Id GreaterThan / LessThan - using specificationBuilder.And()
                    var s3 = specificationBuilder.And(blog => blog.Id, filter => filter.Id.GreaterThan, filter => filter.Id.LessThan);
                    filterBuilder.Where(s3);

                    // Id GreaterThanOrEqual / LessThanOrEqual - using specificationBuilder.Or()
                    var s4 = specificationBuilder.Or(blog => blog.Id, filter => filter.Id.GreaterThanOrEqual, filter => filter.Id.LessThanOrEqual);
                    filterBuilder.Where(s4);

                    // Id In / NotIn - chaining filterBuilder.Where() and Or() without using an explicit specification.
                    // Since sequential Where() calls results in an AND, this criteria will result in:
                    //  (previous_criteria) && (Id IN (1, 2, 3)) || (Id NOT IN (4, 5, 6))
                    // That is, chained methods are simply appended and operator precedence is automatically applied.
                    filterBuilder.Where(blog => blog.Id, filter => filter.Id.In)
                                 .Or(blog => blog.Id, filter => filter.Id.NotIn);

                    // Description EqualTo / NotEqualTo / Contains / NotContains / StartsWith / EndsWith - combining them as
                    // AND using the specificationBuilder then OR to the previous criteria added to the filter builder.
                    // Also shows using 'Current' to allow for continued chaining as an OR / AND can only otherwise follow a Where().
                    // (Note, Current will be null if there have been no previous criteria applied)
                    var s5 = specificationBuilder.Create(blog => blog.Description, filter => filter.Description.EqualTo);
                    var s6 = specificationBuilder.Create(blog => blog.Description, filter => filter.Description.NotEqualTo);
                    var s7 = specificationBuilder.Create(blog => blog.Description, filter => filter.Description.Contains);
                    var s8 = specificationBuilder.Create(blog => blog.Description, filter => filter.Description.NotContains);
                    var s9 = specificationBuilder.Create(blog => blog.Description, filter => filter.Description.StartsWith);
                    var s10 = specificationBuilder.Create(blog => blog.Description, filter => filter.Description.EndsWith);
                    var combined = s5.And(s6).And(s7).And(s8).And(s9).And(s10);

                    filterBuilder.Current.Or(combined);

                    // Output the generated query as readable text:
                    // (((((((Id == 5) AND (Id != 6)) AND ((Id > 10) AND (Id < 5))) AND ((Id >= 15) OR (Id <= 7))) AND (1, 2, 3) Contains Id) OR  NOT ((4, 5, 6) Contains Id)) OR ((((((Description == '#10') AND (Description != '#100')) AND Description Contains '2') AND  NOT (Description Contains '3')) AND Description StartsWith '#') AND Description EndsWith '55'))
                    var queryString = filterBuilder.AsQueryString();

                    Console.WriteLine();
                    Console.WriteLine($"Filter as query string: {queryString}");
                    Console.WriteLine();
                });
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