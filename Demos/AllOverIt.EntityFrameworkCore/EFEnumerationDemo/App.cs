﻿using AllOverIt.Assertion;
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
        ILinqSpecification<TType> GetSpecification(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation);
        ILinqSpecification<TType> GetSpecification<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation);

        ILinqSpecification<TType> And(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation1, Func<TFilter, IStringOperation> operation2);
        ILinqSpecification<TType> And<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation1, Func<TFilter, IOperation> operation2);

        ILinqSpecification<TType> Or(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation1, Func<TFilter, IStringOperation> operation2);
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

        public ILinqSpecification<TType> GetSpecification(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation)
        {
            return GetOperationSpecification(propertyExpression, operation);
        }

        public ILinqSpecification<TType> GetSpecification<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation)
        {
            return GetOperationSpecification(propertyExpression, operation);
        }

        #region AND Operations
        public ILinqSpecification<TType> And(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation1, Func<TFilter, IStringOperation> operation2)
        {
            var specification1 = GetOperationSpecification(propertyExpression, operation1);
            var specification2 = GetOperationSpecification(propertyExpression, operation2);
            
            return specification1.And(specification2);
        }

        public ILinqSpecification<TType> And<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation1, Func<TFilter, IOperation> operation2)
        {
            var specification1 = GetOperationSpecification(propertyExpression, operation1);
            var specification2 = GetOperationSpecification(propertyExpression, operation2);
            
            return specification1.And(specification2);
        }
        #endregion

        #region OR Operations
        public ILinqSpecification<TType> Or(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation1, Func<TFilter, IStringOperation> operation2)
        {
            var specification1 = GetOperationSpecification(propertyExpression, operation1);
            var specification2 = GetOperationSpecification(propertyExpression, operation2);
            
            return specification1.Or(specification2);
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



    public interface ILogicalFilterBuilder<TType, TFilter>
      where TType : class
      where TFilter : class, IFilter
    {
        ILogicalFilterBuilder<TType, TFilter> And(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation);
        ILogicalFilterBuilder<TType, TFilter> And<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation);
        ILogicalFilterBuilder<TType, TFilter> And(ILinqSpecification<TType> specification);


        ILogicalFilterBuilder<TType, TFilter> Or(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation);
        ILogicalFilterBuilder<TType, TFilter> Or<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation);
        ILogicalFilterBuilder<TType, TFilter> Or(ILinqSpecification<TType> specification);
    }

    public interface IFilterBuilder<TType, TFilter>
       where TType : class
       where TFilter : class, IFilter
    {
        ILogicalFilterBuilder<TType, TFilter> Where(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation);
        ILogicalFilterBuilder<TType, TFilter> Where<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation);
        ILogicalFilterBuilder<TType, TFilter> Where(ILinqSpecification<TType> specification);
    }

    public sealed class FilterBuilder<TType, TFilter> : IFilterBuilder<TType, TFilter>, ILogicalFilterBuilder<TType, TFilter>
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

        #region WHERE Operations
        public ILogicalFilterBuilder<TType, TFilter> Where(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation)
        {
            _currentSpecification = _specificationBuilder.GetSpecification(propertyExpression, operation);

            return this;
        }

        public ILogicalFilterBuilder<TType, TFilter> Where<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation)
        {
            _currentSpecification = _specificationBuilder.GetSpecification(propertyExpression, operation);

            return this;
        }

        public ILogicalFilterBuilder<TType, TFilter> Where(ILinqSpecification<TType> specification)
        {
            _currentSpecification = specification;

            return this;
        }
        #endregion

        #region AND Operations
        public ILogicalFilterBuilder<TType, TFilter> And(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation)
        {
            var specification = _specificationBuilder.GetSpecification(propertyExpression, operation);
            _currentSpecification = _currentSpecification.And(specification);

            return this;
        }

        public ILogicalFilterBuilder<TType, TFilter> And<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation)
        {
            var specification = _specificationBuilder.GetSpecification(propertyExpression, operation);
            _currentSpecification = _currentSpecification.And(specification);

            return this;
        }

        public ILogicalFilterBuilder<TType, TFilter> And(ILinqSpecification<TType> specification)
        {
            _currentSpecification = _currentSpecification.And(specification);

            return this;
        }
        #endregion

        #region OR Operations
        public ILogicalFilterBuilder<TType, TFilter> Or(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation)
        {
            var specification = _specificationBuilder.GetSpecification(propertyExpression, operation);
            _currentSpecification = _currentSpecification.Or(specification);

            return this;
        }


        public ILogicalFilterBuilder<TType, TFilter> Or<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation)
        {
            var specification = _specificationBuilder.GetSpecification(propertyExpression, operation);
            _currentSpecification = _currentSpecification.Or(specification);

            return this;
        }

        public ILogicalFilterBuilder<TType, TFilter> Or(ILinqSpecification<TType> specification)
        {
            _currentSpecification = _currentSpecification.Or(specification);

            return this;
        }
        #endregion
    }

    public static class QueryableExtensions
    {
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
                    StartsWith = { Value = "#1" }
                };


                var query = dbContext.Blogs

                    // WHERE ((`b`.`Id` > 9) AND (`b`.`Id` < 12)) OR ((`b`.`Description` LIKE '%#12%') OR (`b`.`Description` LIKE '#1%'))
                    .ApplyFilter(filter, (specificationBuilder, filterBuilder) =>
                    {
                        var s1 = specificationBuilder.And(blog => blog.Id, f => f.GreaterThan, f => f.LessThan);
                        var s2 = specificationBuilder.Or(blog => blog.Description, f => f.Contains, f => f.StartsWith);
                        
                        filterBuilder
                            .Where(s1)
                            .Or(s2);            // can chain more And() / Or() calls
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