using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.GenericHost;
using AllOverIt.Pagination;
using AllOverIt.Pagination.Extensions;
using Bogus;
using KeysetPaginationConsole;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PaginationConsole.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PaginationConsole
{
    public sealed class App : ConsoleAppBase
    {
        private readonly IDbContextFactory<BloggingContext> _dbContextFactory;
        private readonly IQueryPaginatorFactory _queryPaginatorFactory;
        private readonly ILogger<App> _logger;

        public App(IDbContextFactory<BloggingContext> dbContextFactory, IQueryPaginatorFactory queryPaginatorFactory, ILogger<App> logger)
        {
            _dbContextFactory = dbContextFactory.WhenNotNull(nameof(dbContextFactory));
            _queryPaginatorFactory = queryPaginatorFactory.WhenNotNull(nameof(queryPaginatorFactory));
            _logger = logger.WhenNotNull(nameof(logger));
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StartAsync");

            using (var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken))
            {
                dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                if (DemoStartupOptions.RecreateData)
                {
                    await dbContext.Database.EnsureDeletedAsync(cancellationToken);

                    switch (DemoStartupOptions.Use)
                    {
                        case DatabaseChoice.Mysql:
                        case DatabaseChoice.PostgreSql:
                            await dbContext.Database.MigrateAsync(cancellationToken);
                            break;

                        case DatabaseChoice.Sqlite:
                            await dbContext.Database.EnsureCreatedAsync(cancellationToken);
                            break;

                        default:
                            throw new NotImplementedException($"Unknown database type {DemoStartupOptions.Use}");
                    }
                }

                const int pageSize = 100;

                await CreateDataIfRequired();

                Console.WriteLine("Starting...");
                Console.WriteLine();

                // Base query
                var query =
                    from blog in dbContext.Blogs
                    from post in blog.Posts
                    select new
                    {
                        BlogId = blog.Id,
                        blog.Description,
                        PostId = post.Id,
                        post.Title
                    };

                var paginatorConfig = new QueryPaginatorConfiguration
                {
                    PageSize = pageSize,
                    PaginationDirection = PaginationDirection.Forward,      // This is the default
                    UseParameterizedQueries = true                          // Recommended for EF queries to avoid SQL injection and EF cache improvements
                };

                // Paginated queries require the last column be a unique Id, hence including the PostId
                // (could be BlogId if we were only querying the Blogs table)
                var queryPaginator = _queryPaginatorFactory
                    .CreatePaginator(query, paginatorConfig)
                    .ColumnAscending(item => item.Description, item => item.BlogId, item => item.PostId);

                string continuationToken = default;
                var key = 'n';

                var stopwatch = Stopwatch.StartNew();
                long? lastTokenGenerationTime = default;
                var performanceRecordCount = 0;

                while (key != 'q')
                {
                    if (!DemoStartupOptions.RunPerformanceOnly)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Querying...");
                        Console.WriteLine();
                    }

                    stopwatch.Restart();

                    // Including this here for worst case scenario
                    var totalRecords = await query.CountAsync(cancellationToken);

                    var countElapsed = stopwatch.ElapsedMilliseconds;
                    stopwatch.Restart();

                    var pageQuery = queryPaginator.GetPageQuery(continuationToken);

                    var buildQueryElapsed = stopwatch.ElapsedMilliseconds;
                    stopwatch.Restart();

                    var pageResults = await pageQuery.ToListAsync(cancellationToken);

                    var resultsElapsed = stopwatch.ElapsedMilliseconds;
                    stopwatch.Restart();

                    // HasPreviousPageAsync() is EF specific - faster than using HasPreviousPage() as it won't need to return all rows
                    var hasPrevious = pageResults.Any() && await queryPaginator.HasPreviousPageAsync(pageResults.First(), cancellationToken);

                    var previousElapsed = stopwatch.ElapsedMilliseconds;
                    stopwatch.Restart();

                    // HasNextPageAsync() is EF specific - faster than using HasNextPage() as it won't need to return all rows
                    var hasNext = pageResults.Any() && await queryPaginator.HasNextPageAsync(pageResults.Last(), cancellationToken);

                    var nextElapsed = stopwatch.ElapsedMilliseconds;
                    stopwatch.Restart();

                    var totalElapsed = countElapsed + buildQueryElapsed + resultsElapsed + previousElapsed + nextElapsed;

                    if (!DemoStartupOptions.RunPerformanceOnly)
                    {
                        pageResults.ForEach(result =>
                        {
                            Console.WriteLine($"{result.BlogId}, {result.Description}, {result.PostId}, {result.Title}");
                        });

                        if (lastTokenGenerationTime.HasValue)
                        {
                            Console.WriteLine();
                            Console.WriteLine($"ContinuationToken Generation time: {lastTokenGenerationTime}ms");
                        }

                        Console.WriteLine();
                        Console.WriteLine($"Using ContinuationToken:");
                        Console.WriteLine(continuationToken);
                        Console.WriteLine();

                        Console.WriteLine();
                        Console.WriteLine($"{pageQuery.ToQueryString()}");
                        Console.WriteLine();

                        Console.WriteLine();
                        Console.WriteLine($"{pageSize} of {totalRecords} rows. Execution time: {totalElapsed}ms");
                        Console.WriteLine($"  > Get Total Count: {countElapsed}ms");
                        Console.WriteLine($"  > Build Query: {buildQueryElapsed}ms");
                        Console.WriteLine($"  > Get Results: {resultsElapsed}ms");
                        Console.WriteLine($"  > Has Previous: {previousElapsed}ms");
                        Console.WriteLine($"  > Has Next: {nextElapsed}ms");
                        Console.WriteLine($"    >> Total: {countElapsed + buildQueryElapsed + resultsElapsed + previousElapsed + nextElapsed}ms");
                        Console.WriteLine();

                        key = GetUserInput(hasPrevious, hasNext);
                    }
                    else
                    {
                        performanceRecordCount += pageSize;

                        Console.WriteLine($"{performanceRecordCount} of {totalRecords}, Total: {countElapsed + buildQueryElapsed + resultsElapsed + previousElapsed + nextElapsed}ms");

                        if (!hasNext)
                        {
                            break;
                        }

                        key = 'n';
                    }

                    stopwatch.Restart();

                    switch (key)
                    {
                        case 'f':
                            continuationToken = queryPaginator.ContinuationTokenEncoder.EncodeFirstPage();      // could also just set to null or string.Empty
                            break;

                        case 'p':
                            continuationToken = queryPaginator.ContinuationTokenEncoder.EncodePreviousPage(pageResults);
                            break;

                        case 'n':
                            continuationToken = queryPaginator.ContinuationTokenEncoder.EncodeNextPage(pageResults);
                            break;

                        case 'l':
                            continuationToken = queryPaginator.ContinuationTokenEncoder.EncodeLastPage();
                            break;

                        case 'q':
                            Console.WriteLine();
                            Console.WriteLine("Done");
                            break;
                    }

                    lastTokenGenerationTime = stopwatch.ElapsedMilliseconds;
                }
            }

            ExitCode = 0;

            Console.WriteLine("DONE");
            Console.WriteLine();
            Console.ReadKey();
        }

        private async Task CreateDataIfRequired()
        {
            using (var dbContext = await _dbContextFactory.CreateDbContextAsync())
            {
                var blogCount = await dbContext.Blogs.CountAsync();

                if (blogCount != 0)
                {
                    return;
                }
            }

            var totalCount = 1_000_001;
            var batchSize = 100;
            var batchCount = (int) Math.Ceiling(totalCount / (double) batchSize);

            await Enumerable
                .Range(1, batchCount)
                .ForEachAsParallelAsync(async index =>
                {
                    var rows = index * batchSize > totalCount
                        ? totalCount - (index - 1) * batchSize
                        : batchSize;

                    await CreateDataBatch(index, rows);
                }, 25);
        }

        private async Task CreateDataBatch(int index, int batchSize)
        {
            Console.WriteLine($"Processing index {index}");

            var blogFaker = new Faker<Blog>()
                .RuleFor(blog => blog.Description, faker => faker.Lorem.Sentence(8));

            var postFaker = new Faker<Post>()
                .RuleFor(post => post.Title, faker => faker.Lorem.Sentence(4))
                .RuleFor(post => post.Content, faker => faker.Lorem.Paragraph());

            using (var dbContext = await _dbContextFactory.CreateDbContextAsync())
            {
                var blogs = new List<Blog>();

                for (var blogIndex = 0; blogIndex < batchSize; blogIndex++)
                {
                    var blog = blogFaker.Generate();
                    var posts = postFaker.Generate(6);

                    blog.Posts = posts;
                    blogs.Add(blog);
                }

                dbContext.Blogs.AddRange(blogs);
                await dbContext.SaveChangesAsync();
            }
        }

        private static char GetUserInput(bool hasPrevious, bool hasNext)
        {
            Console.WriteLine();

            var sb = new StringBuilder();

            sb.Append("(F)irst, ");

            if (hasPrevious)
            {
                sb.Append("(P)revious, ");
            }

            if (hasNext)
            {
                sb.Append("(N)ext, ");
            }

            sb.Append("(L)ast, ");

            sb.Append("(Q)uit");

            Console.WriteLine();
            Console.WriteLine($"{sb}");
            Console.WriteLine();

            char key;

            do
            {
                key = char.ToLower(Console.ReadKey(true).KeyChar);
            } while ((key != 'p' || !hasPrevious) && (key != 'n' || !hasNext) && key != 'f' && key != 'l' && key != 'q');

            return key;
        }
    }
}