using AllOverIt.EntityFrameworkCore.Diagrams.Tests.TestTypes;
using AllOverIt.EntityFrameworkCore.Diagrams.Tests.TestTypes.Entities;
using AllOverIt.Extensions;
using FluentAssertions;
using System.Collections.Generic;
using TestUtils;

namespace AllOverIt.EntityFrameworkCore.Diagrams.Tests
{
    public class ColumnDescriptorFixture : NSubstituteFixtureBase
    {
        private readonly Author _author = new();
        private readonly Blog _blog = new();
        private readonly TestDbContext _dbContext = new();

        public class Constructor : ColumnDescriptorFixture
        {
            [Fact]
            public void Should_Set_Members_No_ForeignKeys()
            {
                _dbContext.Authors.Add(_author);

                var descriptors = _dbContext.Model
                    .FindEntityType(typeof(Author))
                    .GetProperties()
                    .SelectAsReadOnlyCollection(ColumnDescriptor.Create);

                foreach (var descriptor in descriptors)
                {
                    object expected = null;

                    if (descriptor.ColumnName == nameof(Author.Id))
                    {
                        expected = new
                        {
                            ColumnName = nameof(Author.Id),
                            ColumnType = "INTEGER",
                            IsNullable = false,
                            MaxLength = (int?) null,
                            Constraint = ConstraintType.PrimaryKey,
                            ForeignKeyPrincipals = (IReadOnlyCollection<PrincipalForeignKey>) null
                        };
                    }
                    else if (descriptor.ColumnName == nameof(Author.FirstName))
                    {
                        expected = new
                        {
                            ColumnName = nameof(Author.FirstName),
                            ColumnType = "TEXT",
                            IsNullable = false,
                            MaxLength = 50,
                            Constraint = ConstraintType.None,
                            ForeignKeyPrincipals = (IReadOnlyCollection<PrincipalForeignKey>) null
                        };
                    }
                    else if (descriptor.ColumnName == nameof(Author.LastName))
                    {
                        expected = new
                        {
                            ColumnName = nameof(Author.LastName),
                            ColumnType = "TEXT",
                            IsNullable = true,
                            MaxLength = 50,
                            Constraint = ConstraintType.None,
                            ForeignKeyPrincipals = (IReadOnlyCollection<PrincipalForeignKey>) null
                        };
                    }
                    else if (descriptor.ColumnName == nameof(Author.Email))
                    {
                        expected = new
                        {
                            ColumnName = nameof(Author.Email),
                            ColumnType = "TEXT",
                            IsNullable = false,
                            MaxLength = 50,
                            Constraint = ConstraintType.None,
                            ForeignKeyPrincipals = (IReadOnlyCollection<PrincipalForeignKey>) null
                        };
                    }

                    expected.Should().BeEquivalentTo(descriptor);
                }
            }

            [Fact]
            public void Should_Set_Members_With_ForeignKeys()
            {
                _dbContext.Authors.Add(_author);
                _dbContext.Blogs.Add(_blog);

                var authorBlog = new AuthorBlog
                {
                    Author = _author,
                    Blogger = _blog
                };

                _dbContext.AuthorBlogs.Add(authorBlog);

                var descriptors = _dbContext.Model
                    .FindEntityType(typeof(AuthorBlog))
                    .GetProperties()
                    .SelectAsReadOnlyCollection(ColumnDescriptor.Create);

                foreach (var descriptor in descriptors)
                {
                    object expected = null;

                    if (descriptor.ColumnName == nameof(AuthorBlog.Id))
                    {
                        expected = new
                        {
                            ColumnName = nameof(AuthorBlog.Id),
                            ColumnType = "INTEGER",
                            IsNullable = false,
                            MaxLength = (int?) null,
                            Constraint = ConstraintType.PrimaryKey,
                            ForeignKeyPrincipals = (IReadOnlyCollection<PrincipalForeignKey>) null
                        };
                    }
                    else if (descriptor.ColumnName == $"{nameof(AuthorBlog.Author)}Id")
                    {
                        expected = new
                        {
                            ColumnName = $"{nameof(AuthorBlog.Author)}Id",
                            ColumnType = "INTEGER",
                            IsNullable = false,
                            MaxLength = (int?) null,
                            Constraint = ConstraintType.ForeignKey,
                            ForeignKeyPrincipals = new[]
                            {
                                new
                                {
                                    ColumnName = nameof(Author.Id),
                                    EntityName = nameof(Author),
                                    IsOneToMany = true,
                                    Type = typeof(Author)
                                }
                            }
                        };
                    }
                    else if (descriptor.ColumnName == $"{nameof(AuthorBlog.Blogger)}Id")
                    {
                        expected = new
                        {
                            ColumnName = $"{nameof(AuthorBlog.Blogger)}Id",
                            ColumnType = "INTEGER",
                            IsNullable = false,
                            MaxLength = (int?) null,
                            Constraint = ConstraintType.ForeignKey,
                            ForeignKeyPrincipals = new[]
                            {
                                new
                                {
                                    ColumnName = nameof(Blog.Id),
                                    EntityName = nameof(Blog),
                                    IsOneToMany = true,
                                    Type = typeof(Blog)
                                }
                            }
                        };
                    }

                    expected.Should().BeEquivalentTo(descriptor);
                }
            }
        }
    }
}