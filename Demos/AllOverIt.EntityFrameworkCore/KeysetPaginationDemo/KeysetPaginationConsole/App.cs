using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.GenericHost;
using KeysetPaginationConsole.Entities;
using KeysetPaginationConsole.KeysetPagination;
using KeysetPaginationConsole.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
                dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                //await dbContext.Database.EnsureDeletedAsync(cancellationToken);

                await dbContext.Database.MigrateAsync(cancellationToken);

                await CreateDataIfRequired(10_000_000);

                Console.WriteLine("Starting...");
                Console.WriteLine();

                var rowsToRead = 1000;
                var pageSize = 100;

                // Base query
                var query =
                    from blog in dbContext.Blogs
                    select new
                    {
                        BlogId = blog.Id,
                        blog.Description,
                        blog.Reference,
                        blog.AnotherId
                    };

                var offsetQuery =
                   from blog in dbContext.Blogs
                   orderby blog.Description, blog.Id
                   select new
                   {
                       BlogId = blog.Id,
                       blog.Description,
                       blog.Reference,
                       blog.AnotherId
                   };

                var paginationBuilder = query
                    .KeysetPaginate(pageSize, PaginationDirection.Forward)
                    .ColumnAscending(item => item.Description)
                    .ColumnAscending(item => item.BlogId);
                    //.Build()

                var continuationToken = string.Empty;

                var stopwatch = Stopwatch.StartNew();

                void LogCheckpoint(int recordsRead, double elapsed, string queryString, int firstId, int lastId, bool newline)
                {
                    if (queryString != null)
                    {
                        Console.WriteLine(queryString);
                    }

                    Console.WriteLine($"Checkpoint after {recordsRead} rows, first Id={firstId}, last Id={lastId} ({elapsed}ms)");

                    if (newline)
                    {
                        Console.WriteLine();
                    }
                }

                // Keyset paginated, with the overhead of creating a continuation token
                async Task<bool> ReadKeysetPaginated(int readSoFar)
                {
                    var lastCheckpoint = stopwatch.ElapsedMilliseconds;
                    double elapsed;
                    var firstId = 0;
                    var lastId = 0;

                    do
                    {
                        var paginatedQuery = paginationBuilder.Build(continuationToken);

                        //var paginatedQueryString = paginatedQuery.ToQueryString();

                        var paginatedResults = await paginatedQuery.ToListAsync(cancellationToken);

                        if (!paginatedResults.Any())
                        {
                            return false;
                        }

                        readSoFar += pageSize;
                        continuationToken = paginationBuilder.CreateContinuationToken(paginatedResults);

                        if (readSoFar % rowsToRead == 0)
                        {
                            firstId = paginatedResults.First().BlogId;
                            lastId = paginatedResults.Last().BlogId;
                        }
                    } while (readSoFar % rowsToRead != 0);

                    elapsed = stopwatch.ElapsedMilliseconds - lastCheckpoint;

                    // paginatedQueryString
                    LogCheckpoint(readSoFar, elapsed, null, firstId, lastId, false);

                    return true;
                }

                async Task<bool> ReadOffsetPaginated(int readSoFar)
                {
                    var lastCheckpoint = stopwatch.ElapsedMilliseconds;
                    double elapsed;
                    var firstId = 0;
                    var lastId = 0;

                    do
                    {
                        var pagedOffsetQuery = offsetQuery.Skip(readSoFar).Take(pageSize);

                        //var pagedOffsetQueryString = pagedOffsetQuery.ToQueryString();

                        var paginatedResults = await pagedOffsetQuery.ToListAsync(cancellationToken);

                        if (!paginatedResults.Any())
                        {
                            return false;
                        }

                        readSoFar += pageSize;

                        if (readSoFar % rowsToRead == 0)
                        {
                            firstId = paginatedResults.First().BlogId;
                            lastId = paginatedResults.Last().BlogId;
                        }
                    } while (readSoFar % rowsToRead != 0);

                    elapsed = stopwatch.ElapsedMilliseconds - lastCheckpoint;

                    // pagedOffsetQueryString
                    LogCheckpoint(readSoFar, elapsed, null, firstId, lastId, false);

                    return true;
                }

                var totalRead = 0;

                while (true)
                {
                    if (! await ReadKeysetPaginated(totalRead))
                    {
                        break;
                    }

                    await ReadOffsetPaginated(totalRead);

                    totalRead += rowsToRead;

                    Console.WriteLine();


                    //foreach (var result in paginatedResults)
                    //{
                    //    Console.WriteLine($"{result.BlogId} - {result.AnotherId} - {result.Description} - {result.Reference}");
                    //}

                    //Console.WriteLine();

                    //Console.WriteLine(continuationToken);
                    //Console.WriteLine();

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