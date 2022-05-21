using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.GenericHost;
using AllOverIt.Pagination;
using AllOverIt.Pagination.Extensions;
using Bogus;
using KeysetPaginationConsole.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KeysetPaginationConsole
{
    public static class QueryPaginatorExtensions
    {

        // EF Specific
        public static Task<bool> HasPreviousPageAsync<TEntity>(this IQueryPaginator<TEntity> paginator, TEntity reference,
            CancellationToken cancellation = default) where TEntity : class
        {
            return paginator.HasPreviousPageAsync(reference, async (queryable, predicate, token) =>
            {
                return await queryable.AnyAsync(predicate, token);
            }, cancellation);
        }

        // EF Specific
        public static Task<bool> HasNextPageAsync<TEntity>(this IQueryPaginator<TEntity> paginator, TEntity reference,
            CancellationToken cancellation = default) where TEntity : class
        {
            return paginator.HasNextPageAsync(reference, async (queryable, predicate, token) =>
            {
                return await queryable.AnyAsync(predicate, token);
            }, cancellation);
        }
    }



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
                if (DatabaseProvider.RecreateData)
                {
                    if (DatabaseProvider.Use == DatabaseChoice.Mysql)
                    {
                        await dbContext.Database.EnsureDeletedAsync(cancellationToken);
                        await dbContext.Database.MigrateAsync(cancellationToken);
                    }
                    else if (DatabaseProvider.Use == DatabaseChoice.Sqlite)
                    {
                        dbContext.Database.EnsureDeleted();
                        dbContext.Database.EnsureCreated();
                    }
                    else
                    {
                        throw new NotImplementedException($"Unknown database type {DatabaseProvider.Use}");
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

                var queryPaginator = _queryPaginatorFactory
                    .CreatePaginator(query, pageSize)
                    .ColumnAscending(item => item.Description, item => item.BlogId);

                string continuationToken = default;
                var key = 'n';

                var stopwatch = Stopwatch.StartNew();

                while (key != 'q')
                {
                    Console.WriteLine();
                    Console.WriteLine("Querying...");
                    Console.WriteLine();

                    stopwatch.Restart();

                    // Including this here for worst case scenario
                    var totalRecords = await query.CountAsync(cancellationToken);

                            var countElapsed = stopwatch.ElapsedMilliseconds;
                            stopwatch.Restart();

                    var pageQuery = queryPaginator.BuildPageQuery(continuationToken);

                            var buildQueryElapsed = stopwatch.ElapsedMilliseconds;
                            stopwatch.Restart();

                    var pageResults = await pageQuery.ToListAsync(cancellationToken);

                            var resultsElapsed = stopwatch.ElapsedMilliseconds;
                            stopwatch.Restart();

                    //var hasPrevious = pageResults.Any() && queryPaginator.HasPreviousPage(pageResults.First());
                    var hasPrevious = pageResults.Any() && await queryPaginator.HasPreviousPageAsync(pageResults.First(), cancellationToken);

                            var previousElapsed = stopwatch.ElapsedMilliseconds;
                            stopwatch.Restart();

                    //var hasNext = pageResults.Any() && queryPaginator.HasNextPage(pageResults.Last());
                    var hasNext = pageResults.Any() && await queryPaginator.HasNextPageAsync(pageResults.Last(), cancellationToken);

                            var nextElapsed = stopwatch.ElapsedMilliseconds;
                            stopwatch.Restart();

                    var totalElapsed = countElapsed + buildQueryElapsed + resultsElapsed + previousElapsed + nextElapsed;

                    pageResults.ForEach(result =>
                    {
                        Console.WriteLine($"{result.BlogId}, {result.Description}, {result.PostId}, {result.Title}");
                    });

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

                    stopwatch.Restart();

                    switch (key)
                    {
                        case 'f':
                            continuationToken = queryPaginator.CreateFirstPageContinuationToken();      // could also just set to null or string.Empty
                            break;

                        case 'p':
                            continuationToken = queryPaginator.CreateContinuationToken(ContinuationDirection.PreviousPage, pageResults);
                            break;

                        case 'n':
                            continuationToken = queryPaginator.CreateContinuationToken(ContinuationDirection.NextPage, pageResults);
                            break;

                        case 'l':
                            continuationToken = queryPaginator.CreateLastPageContinuationToken();
                            break;

                        case 'q':
                            Console.WriteLine();
                            Console.WriteLine("Done");
                            break;
                    }

                    Console.WriteLine($"Continuation token generation time: {stopwatch.ElapsedMilliseconds}ms");
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

            var totalCount = 1_234_567;
            var batchSize = 100;
            var batchCount = (int)Math.Ceiling(totalCount / (double)batchSize);

            await Enumerable
                .Range(1, batchCount)
                .ForEachAsParallelAsync(async index =>
                {
                    var rows = index * batchSize > totalCount
                        ? totalCount - (((index-1) * batchSize))
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
                    var posts = postFaker.Generate(5);

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