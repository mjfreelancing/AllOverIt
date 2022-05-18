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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KeysetPaginationConsole
{
    public sealed class App : ConsoleAppBase
    {
        private class ContinuationTokens
        {
            public string Current { get; set; }
            public string Next { get; set; }
            public string Previous { get; set; }
        }

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

            var filename = @"C:\temp\paginated_results.txt";
            var fs = File.Create(filename);

            void WriteFileStreamLine(string description, int? id = default)
            {
                var bytes = id.HasValue
                    ? Encoding.UTF8.GetBytes($"{description} : {id}{Environment.NewLine}")
                    : Encoding.UTF8.GetBytes($"{description}");

                fs.Write(bytes, 0, bytes.Length);
            }

            using (var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken))
            {
                dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                //await dbContext.Database.EnsureDeletedAsync(cancellationToken);

                await dbContext.Database.MigrateAsync(cancellationToken);

                await CreateDataIfRequired(2_000);

                Console.WriteLine("Starting...");
                Console.WriteLine();

                var rowsToRead = 50;
                var pageSize = 25;

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

                var paginationBuilder = query
                    .KeysetPaginate(pageSize, PaginationDirection.Forward)
                    .ColumnAscending(item => item.Description)
                    .ColumnAscending(item => item.BlogId);
                    //.Build()

                var stopwatch = Stopwatch.StartNew();

                var continuationTokens = new ContinuationTokens();

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
                async Task<bool> ReadKeysetPaginated(int readSoFar, bool forward)
                {
                    var lastCheckpoint = stopwatch.ElapsedMilliseconds;
                    double elapsed;
                    var firstId = 0;
                    var lastId = 0;

                    do
                    {
                        var continuationToken = forward
                            ? continuationTokens.Next
                            : continuationTokens.Previous;

                        var paginatedQuery = paginationBuilder.Build(continuationToken);

                        //var paginatedQueryString = paginatedQuery.ToQueryString();

                        var paginatedResults = await paginatedQuery.ToListAsync(cancellationToken);

                        if (!paginatedResults.Any())
                        {
                            return false;
                        }

                        WriteFileStreamLine($"*** Page Start ***{Environment.NewLine}{Environment.NewLine}");

                        foreach (var result in paginatedResults)
                        {
                            WriteFileStreamLine(result.Description, result.BlogId);
                        }

                        WriteFileStreamLine($"{Environment.NewLine}*** Page End ***{Environment.NewLine}");

                        readSoFar += pageSize;

                        continuationTokens.Current = continuationToken;
                        continuationTokens.Next = paginationBuilder.CreateContinuationToken(ContinuationDirection.NextPage, paginatedResults);
                        continuationTokens.Previous = paginationBuilder.CreateContinuationToken(ContinuationDirection.PreviousPage, paginatedResults);

                        if (readSoFar % rowsToRead == 0)
                        {
                            firstId = paginatedResults.First().BlogId;
                            lastId = paginatedResults.Last().BlogId;
                        }

                    } while (readSoFar % rowsToRead != 0);

                    elapsed = stopwatch.ElapsedMilliseconds - lastCheckpoint;

                    LogCheckpoint(readSoFar, elapsed, null, firstId, lastId, false);

                    return true;
                }

                var totalRead = 0;
                var paginatedCount = 0;
                var forward = true;

                while (true)
                {
                    // TODO: How to know if I'm at the first page or if there is a next page - not sure it can be done without making another round trip to try and get a single row.
                    if (! await ReadKeysetPaginated(totalRead, forward))
                    {
                        // for now....
                        forward = true;
                        continuationTokens.Next = null;

                        //break;
                    }

                    var wasForward = forward;

                    if (forward)
                    {
                        paginatedCount += rowsToRead;

                        forward = paginatedCount != rowsToRead * 2;
                    }
                    else
                    {
                        paginatedCount -= rowsToRead;

                        forward = paginatedCount == 0;
                    }

                    if (forward != wasForward)
                    {
                        Console.WriteLine($"{Environment.NewLine}Reversing{Environment.NewLine}{Environment.NewLine}");
                        WriteFileStreamLine($"{Environment.NewLine}Reversing{Environment.NewLine}{Environment.NewLine}");
                        await fs.FlushAsync();
                    }

                    totalRead += rowsToRead;

                    Console.WriteLine();
                }
            }

            fs.Flush();

            ExitCode = 0;

            Console.WriteLine("DONE");
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