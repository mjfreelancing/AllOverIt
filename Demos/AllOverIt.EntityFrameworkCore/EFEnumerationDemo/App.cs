using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Filtering.Extensions;
using AllOverIt.Filtering.Operations;
using AllOverIt.GenericHost;
using AllOverIt.Patterns.Specification;
using AllOverIt.Patterns.Specification.Extensions;
using EFEnumerationDemo.Entities;
using EFEnumerationDemo.Filtering;
using EFEnumerationDemo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace EFEnumerationDemo
{

    public interface IFilterSpecificationBuilder<TType, TFilter>
        where TType : class
        where TFilter : class, IFilter
    {
        ILinqSpecification<TType> And(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation);
        ILinqSpecification<TType> And(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation1, Func<TFilter, IStringOperation> operation2);
        ILinqSpecification<TType> And<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation);
        ILinqSpecification<TType> And<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation1, Func<TFilter, IOperation> operation2);

        ILinqSpecification<TType> Or(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation);
        ILinqSpecification<TType> Or(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation1, Func<TFilter, IStringOperation> operation2);
        ILinqSpecification<TType> Or<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation);
        ILinqSpecification<TType> Or<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation1, Func<TFilter, IOperation> operation2);
    }

    public class FilterSpecificationBuilder<TType, TFilter> : IFilterSpecificationBuilder<TType, TFilter>
        where TType : class
        where TFilter : class, IFilter
    {
        private readonly TFilter _filter;

        public FilterSpecificationBuilder(TFilter filter)
        {
            _filter = filter.WhenNotNull(nameof(filter));
        }

        #region AND Operations
        public ILinqSpecification<TType> And(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation)
        {
            return GetOperationSpecification(propertyExpression, operation);
        }

        public ILinqSpecification<TType> And(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation1, Func<TFilter, IStringOperation> operation2)
        {
            var specification1 = GetOperationSpecification(propertyExpression, operation1);
            var specification2 = GetOperationSpecification(propertyExpression, operation2);
            
            return specification1.And(specification2);
        }

        public ILinqSpecification<TType> And<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation)
        {
            return GetOperationSpecification(propertyExpression, operation);
        }

        public ILinqSpecification<TType> And<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation1, Func<TFilter, IOperation> operation2)
        {
            var specification1 = GetOperationSpecification(propertyExpression, operation1);
            var specification2 = GetOperationSpecification(propertyExpression, operation2);
            
            return specification1.And(specification2);
        }
        #endregion

        #region OR Operations
        // string related operation
        public ILinqSpecification<TType> Or(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation)
        {
            return GetOperationSpecification(propertyExpression, operation);
        }

        // string related operations
        public ILinqSpecification<TType> Or(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation1, Func<TFilter, IStringOperation> operation2)
        {
            var specification1 = GetOperationSpecification(propertyExpression, operation1);
            var specification2 = GetOperationSpecification(propertyExpression, operation2);
            
            return specification1.Or(specification2);
        }


        public ILinqSpecification<TType> Or<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation)
        {
            return GetOperationSpecification(propertyExpression, operation);
        }

        public ILinqSpecification<TType> Or<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation1, Func<TFilter, IOperation> operation2)
        {
            var specification1 = GetOperationSpecification(propertyExpression, operation1);
            var specification2 = GetOperationSpecification(propertyExpression, operation2);
            
            return specification1.Or(specification2);
        }
        #endregion

        private ILinqSpecification<TType> GetOperationSpecification<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation)
        {
            var operand = operation.Invoke(_filter);

            if (operand is IGreaterThan<TProperty> greaterThan)
            {
                return new GreaterThan<TType, TProperty>(propertyExpression, greaterThan.Value);
            }
            else if (operand is ILessThan<TProperty> lessThan)
            {
                return new LessThan<TType, TProperty>(propertyExpression, lessThan.Value);
            }

            throw new InvalidOperationException("Unknown operation.");
        }

        // string related operations
        private ILinqSpecification<TType> GetOperationSpecification(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation)
        {
            var operand = operation.Invoke(_filter);

            if (operand is IContains contains)
            {
                return new Contains<TType>(propertyExpression, contains.Value);
            }
            else if (operand is IStartsWith startsWith)
            {
                return new StartsWith<TType>(propertyExpression, startsWith.Value);
            }

            throw new InvalidOperationException("Unknown operation.");
        }
    }



    public interface IFilterBuilder<TType, TFilter>
       where TType : class
       where TFilter : class, IFilter
    {
        IFilterBuilder<TType, TFilter> And(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation);
        IFilterBuilder<TType, TFilter> And(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation1, Func<TFilter, IStringOperation> operation2);
        IFilterBuilder<TType, TFilter> And<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation);
        IFilterBuilder<TType, TFilter> And<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation1, Func<TFilter, IOperation> operation2);
        IFilterBuilder<TType, TFilter> And(ILinqSpecification<TType> specification);
        IFilterBuilder<TType, TFilter> And(ILinqSpecification<TType> specification1, ILinqSpecification<TType> specification2);

        IFilterBuilder<TType, TFilter> Or(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation);
        IFilterBuilder<TType, TFilter> Or(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation1, Func<TFilter, IStringOperation> operation2);
        IFilterBuilder<TType, TFilter> Or<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation);
        IFilterBuilder<TType, TFilter> Or<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation1, Func<TFilter, IOperation> operation2);
        IFilterBuilder<TType, TFilter> Or(ILinqSpecification<TType> specification);
        IFilterBuilder<TType, TFilter> Or(ILinqSpecification<TType> specification1, ILinqSpecification<TType> specification2);
    }

    public sealed class FilterBuilder<TType, TFilter> : IFilterBuilder<TType, TFilter>
        where TType : class
        where TFilter : class, IFilter
    {
        private readonly IFilterSpecificationBuilder<TType, TFilter> _specificationBuilder;

        private ILinqSpecification<TType> _currentSpecification;

        public ILinqSpecification<TType> QuerySpecification => _currentSpecification;

        public FilterBuilder(IFilterSpecificationBuilder<TType, TFilter> specificationBuilder)
        {
            _specificationBuilder = specificationBuilder.WhenNotNull(nameof(specificationBuilder));
        }

        #region AND Operations
        // string related operation
        public IFilterBuilder<TType, TFilter> And(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation)
        {
            var specification = _specificationBuilder.And(propertyExpression, operation);

            ApplyToCurrentSpecification(specification, LinqSpecificationExtensions.And);

            return this;
        }

        // string related operation
        public IFilterBuilder<TType, TFilter> And(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation1, Func<TFilter, IStringOperation> operation2)
        {
            var specification = _specificationBuilder.And(propertyExpression, operation1, operation2);

            ApplyToCurrentSpecification(specification, LinqSpecificationExtensions.And);

            return this;
        }

        public IFilterBuilder<TType, TFilter> And<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation)
        {
            var specification = _specificationBuilder.And(propertyExpression, operation);

            ApplyToCurrentSpecification(specification, LinqSpecificationExtensions.And);

            return this;
        }

        public IFilterBuilder<TType, TFilter> And<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation1, Func<TFilter, IOperation> operation2)
        {
            var specification = _specificationBuilder.And(propertyExpression, operation1, operation2);

            ApplyToCurrentSpecification(specification, LinqSpecificationExtensions.And);

            return this;
        }

        public IFilterBuilder<TType, TFilter> And(ILinqSpecification<TType> specification)
        {
            ApplyToCurrentSpecification(specification, LinqSpecificationExtensions.And);

            return this;
        }

        public IFilterBuilder<TType, TFilter> And(ILinqSpecification<TType> specification1, ILinqSpecification<TType> specification2)
        {
            var combined = specification1.And(specification2);

            ApplyToCurrentSpecification(combined, LinqSpecificationExtensions.And);

            return this;
        }
        #endregion

        #region OR Operations
        // string related operation
        public IFilterBuilder<TType, TFilter> Or(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation)
        {
            var specification = _specificationBuilder.Or(propertyExpression, operation);

            ApplyToCurrentSpecification(specification, LinqSpecificationExtensions.Or);

            return this;
        }

        // string related operations
        public IFilterBuilder<TType, TFilter> Or(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation1, Func<TFilter, IStringOperation> operation2)
        {
            var specification = _specificationBuilder.Or(propertyExpression, operation1, operation2);

            ApplyToCurrentSpecification(specification, LinqSpecificationExtensions.And);

            return this;
        }


        public IFilterBuilder<TType, TFilter> Or<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation)
        {
            var specification = _specificationBuilder.Or(propertyExpression, operation);

            ApplyToCurrentSpecification(specification, LinqSpecificationExtensions.Or);

            return this;
        }

        public IFilterBuilder<TType, TFilter> Or<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation1, Func<TFilter, IOperation> operation2)
        {
            var specification = _specificationBuilder.Or(propertyExpression, operation1, operation2);

            ApplyToCurrentSpecification(specification, LinqSpecificationExtensions.And);

            return this;
        }

        public IFilterBuilder<TType, TFilter> Or(ILinqSpecification<TType> specification)
        {
            ApplyToCurrentSpecification(specification, LinqSpecificationExtensions.Or);

            return this;
        }

        public IFilterBuilder<TType, TFilter> Or(ILinqSpecification<TType> specification1, ILinqSpecification<TType> specification2)
        {
            var combined = specification1.Or(specification2);

            ApplyToCurrentSpecification(combined, LinqSpecificationExtensions.And);

            return this;
        }
        #endregion




        private void ApplyToCurrentSpecification(ILinqSpecification<TType> specification, Func<ILinqSpecification<TType>, ILinqSpecification<TType>, ILinqSpecification<TType>> operation)
        {
            if (_currentSpecification == null)
            {
                _currentSpecification = specification;
                return;
            }

            // operation could be:
            //   public static ILinqSpecification<TType> And<TType>(this ILinqSpecification<TType> leftSpecification, ILinqSpecification<TType> rightSpecification)
            //   public static ILinqSpecification<TType> AndNot<TType>(this ILinqSpecification<TType> leftSpecification, ILinqSpecification<TType> rightSpecification)
            //   public static ILinqSpecification<TType> Or<TType>(this ILinqSpecification<TType> leftSpecification, ILinqSpecification<TType> rightSpecification)
            //   public static ILinqSpecification<TType> OrNot<TType>(this ILinqSpecification<TType> leftSpecification, ILinqSpecification<TType> rightSpecification)

            // There is also a 'Not' to consider
            //   public static ILinqSpecification<TType> Not<TType>(this ILinqSpecification<TType> specification)

            _currentSpecification = operation.Invoke(_currentSpecification, specification);
        }
    }

    public static class QueryableExtensions
    {
        public static IQueryable<TType> ApplyFilter<TType, TFilter>(this IQueryable<TType> queryable, TFilter filter,
            Action<IFilterBuilder<TType, TFilter>> action)
            where TType : class
            where TFilter : class, IFilter
        {
            return ApplyFilter(queryable, filter, (_, filterBuilder) =>
            {
                action.Invoke(filterBuilder);
            });
        }

        public static IQueryable<TType> ApplyFilter<TType, TFilter>(this IQueryable<TType> queryable, TFilter filter,
            Action<IFilterSpecificationBuilder<TType, TFilter>, IFilterBuilder<TType, TFilter>> action)
            where TType : class
            where TFilter : class, IFilter
        {
            _ = filter.WhenNotNull(nameof(filter));

            var specificationBuilder = new FilterSpecificationBuilder<TType, TFilter>(filter);
            var builder = new FilterBuilder<TType, TFilter>(specificationBuilder);

            action.Invoke(specificationBuilder, builder);

            return queryable.Where(builder.QuerySpecification);
        }
    }





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




                // Demonstrating how to use LinqSpecification based filtering
                var filter = new Filter
                {
                    GreaterThan = (GreaterThan<int>) 9,         // explicit operator
                    LessThan = new LessThan<int>(12),           // constructor                  
                    Contains = { Value = "#12" },               // object initialization
                    StartsWith = { Value = "#" }
                };


                var query = dbContext.Blogs

                    // If starting with a single operation, using And() / Or() will have the same outcome since there is no initial specification
                    //builder.And(blog => blog.Id, f => f.GreaterThan);

                    // Option 1: All operations are chained
                    //.ApplyFilter(filter, filterBuilder =>
                    //{
                    //    // All individual and chained calls are combined as AND

                    //    // Example: (Id > 1 AND Id < 2) AND (Description %value% OR Description value%)
                    //    filterBuilder
                    //        .And(blog => blog.Id, f => f.GreaterThan, f => f.LessThan)
                    //        .Or(blog => blog.Description, f => f.Contains, f => f.StartsWith);
                    //})



                    // Option 2: Can build expressions to build more elaborate logic
                    .ApplyFilter(filter, (specificationBuilder, filterBuilder) =>
                    {
                        // Example: (Id > 1 AND Id < 2) OR (Description %value% AND Description value%)
                        var specification1 = specificationBuilder.And(blog => blog.Id, f => f.GreaterThan, f => f.LessThan);
                        var specification2 = specificationBuilder.And(blog => blog.Description, f => f.Contains, f => f.StartsWith);

                        // either:
                        //var combined = specification1.Or(specification2);             // OR's specification1 and specification2, then
                        //filterBuilder.And(combined);                                  // AND's this with any previous expressions

                        // or: - exactly the same as above
                        filterBuilder.Or(specification1, specification2);
                    })

                    /*
                    .ApplyFilter(filter, builder =>
                    {
                        // ( g && l ) || c
                        builder.And(blog => blog.Id, g => g.GreaterThan, g => g.LessThan)
                               .Or(blog => blog.Description, g => g.Contains)

                        // ( g || l ) && c
                        builder.Or(blog => blog.Id, g => g.GreaterThan, g => g.LessThan)
                               .And(blog => blog.Description, g => g.Contains)

                        // ( g || l ) && (c || sw)
                        var g1 = builder.Or(blog => blog.Id, g => g.GreaterThan, g => g.LessThan)
                        var g2 = builder.Or(blog => blog.Description, g => g.Contains, g => g.StartsWith)

                        builder.And(g1, g2)
                    })
                    */

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