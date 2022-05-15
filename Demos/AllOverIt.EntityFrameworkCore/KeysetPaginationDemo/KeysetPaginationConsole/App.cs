using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.GenericHost;
using KeysetPaginationConsole.Entities;
using KeysetPaginationConsole.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AllOverIt.Serialization.NewtonsoftJson;
using KeysetPaginationConsole.KeysetPagination;

namespace KeysetPaginationConsole
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
                //await dbContext.Database.EnsureDeletedAsync(cancellationToken);

                await dbContext.Database.MigrateAsync(cancellationToken);

                await CreateDataIfRequired(100_000);

                Console.WriteLine("Starting...");
                Console.WriteLine();

                // Initial query
                var query =
                    from blog in dbContext.Blogs
                    select new
                    {
                        BlogId = blog.Id,
                        blog.Description,
                        blog.Reference,
                        blog.AnotherId
                    };

                var continuationToken = string.Empty;
                var totalRead = 0;

                var stopwatch = Stopwatch.StartNew();
                var lastCheckpoint = stopwatch.ElapsedMilliseconds;

                while (true)
                {
                    // pagination builder - can call Build() just to get the modified query or, as shown below, build to get the query
                    // and then use the builder to create a continuation token after the results are obtained.

                    var paginationBuilder = query
                        .KeysetPaginate(PaginationDirection.Forward, 25, continuationToken)         // There is an overload that defaults to forward
                        .ColumnAscending(item => item.Description)
                        .ColumnAscending(item => item.BlogId);

                    var paginatedQuery = paginationBuilder.Build();     // **** If we pass the continuationToken here then the above could be cached ****

                    //var queryString = paginatedQuery.ToQueryString();

                    var paginatedResults = await paginatedQuery.ToListAsync(cancellationToken);

                    if (!paginatedResults.Any())
                    {
                        break;
                    }

                    totalRead += paginatedResults.Count;

                    continuationToken = paginationBuilder.CreateContinuationToken(paginatedResults);

                    if (totalRead % 1000 == 0)
                    {
                        var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                        Console.WriteLine($"Checkpoint at {stopwatch.Elapsed} ({elapsedMilliseconds - lastCheckpoint})");
                        lastCheckpoint = elapsedMilliseconds;

                        //foreach (var result in paginatedResults)
                        //{
                        //    Console.WriteLine($"{result.BlogId} - {result.AnotherId} - {result.Description} - {result.Reference}");
                        //}

                        //Console.WriteLine();

                        //Console.WriteLine(continuationToken);
                        //Console.WriteLine();
                    }

                    //Console.WriteLine($"TOTAL: {totalRead}");
                    //Console.WriteLine();

                    //Console.ReadKey(true);
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

        private async Task CreateDataIfRequired(int totalCount)
        {
            using (var dbContext = await _dbContextFactory.CreateDbContextAsync())
            {
                var blogCount = await dbContext.Blogs.CountAsync();

                if (blogCount != 0)
                {
                    return;
                }
            }

            await Enumerable
                .Range(1, totalCount / 100)
                .ForEachAsParallelAsync(async index => await CreateDataBatch(index), 25);
        }

        private async Task CreateDataBatch(int index)
        {
            Console.WriteLine($"Processing index {index}");

            var guids = Enumerable
                .Range(1, 10)
                .Select(_ => Guid.NewGuid())
                .ToList();

            using (var dbContext = await _dbContextFactory.CreateDbContextAsync())
            {
                var startIndex = 100 * index - 99;
                var postIndex = 0;
                var blogs = new List<Blog>();

                for (var blogIndex = startIndex; blogIndex < startIndex + 100; blogIndex++)
                {
                    var guid = guids[blogIndex % 10];

                    var blog = new Blog
                    {
                        AnotherId = (int)DateTime.Now.Ticks,
                        Description = $"Description #{guid}",
                        Status1 = BlogStatus.From((postIndex + blogIndex) % 5),
                        Status3 = BlogStatus.From((postIndex + blogIndex + 5) % 5),
                        Reference = Guid.NewGuid()
                    };

                    if (blogIndex % 2 == 0)
                    {
                        blog.Status2 = BlogStatus.From((postIndex + blogIndex + 3) % 5);
                    }

                    var posts = new List<Post>();

                    for (var idx = 0; idx < 5; idx++)
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