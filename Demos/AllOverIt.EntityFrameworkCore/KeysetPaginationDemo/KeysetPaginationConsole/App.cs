using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.GenericHost;
using AllOverIt.Pagination;
using AllOverIt.Pagination.Extensions;
using Bogus;
using KeysetPaginationConsole.Entities;
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
                // MySql
                //await dbContext.Database.EnsureDeletedAsync(cancellationToken);
                //await dbContext.Database.MigrateAsync(cancellationToken);

                // Sqlite
                //dbContext.Database.EnsureDeleted();
                //dbContext.Database.EnsureCreated();

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

                const int pageSize = 100;
                var totalRecords = await query.CountAsync(cancellationToken);

                var queryPaginator = _queryPaginatorFactory
                    .CreatePaginator(query, pageSize)
                    .ColumnAscending(item => item.Description, item => item.BlogId);

                var stopwatch = Stopwatch.StartNew();

                var continuationToken = string.Empty;
                var lastCheckpoint = 0L;
                var totalRead = 0;

                while (true)
                {
                    var paginatedQuery = queryPaginator.BuildPageQuery(continuationToken);

                    var paginatedResults = await paginatedQuery.ToListAsync(cancellationToken);

                    var lastRow = paginatedResults.Last();

                    var hasNext = await queryPaginator
                        .BuildForwardPageQuery(lastRow)
                        .AnyAsync(cancellationToken);

                    if (hasNext)
                    {
                        continuationToken = queryPaginator.CreateContinuationToken(ContinuationDirection.NextPage, lastRow);
                    }

                    var checkpoint = stopwatch.ElapsedMilliseconds;
                    var elapsed = checkpoint - lastCheckpoint;
                    lastCheckpoint = checkpoint;

                    totalRead += paginatedResults.Count;

                    Console.WriteLine();
                    Console.WriteLine($"Read of {paginatedResults.Count} rows took {elapsed}ms. ({totalRead} of {totalRecords})");
                    Console.WriteLine();

                    if (!hasNext)
                    {
                        break;
                    }
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

            var totalCount = 1_010_101;
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
    }
}