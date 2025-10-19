using AllOverIt.EntityFrameworkCore.Diagrams.D2;
using AllOverIt.EntityFrameworkCore.Diagrams.Exceptions;
using AllOverIt.EntityFrameworkCore.Diagrams.Tests.TestTypes.Entities;
using AllOverIt.Fixture;
using FluentAssertions;
using System.Collections;

namespace AllOverIt.EntityFrameworkCore.Diagrams.Tests
{
    public class ErdOptionsFixture : FixtureBase
    {
        [Fact]
        public void Should_Set_Defaults()
        {
            _ = ErdGenerator.Create<D2ErdGenerator>(options =>
            {
                // Same shape as ErdOptions
                var expected = new
                {
                    Direction = ErdOptions.DiagramDirection.Left,

                    Entities = new
                    {
                        PreserveColumnOrder = true,

                        Nullable = new
                        {
                            IsVisible = true,
                            Mode = NullableColumnMode.NotNull,
                            IsNullLabel = "[NULL]",
                            NotNullLabel = "[NOT NULL]"
                        },

                        ShowMaxLength = true,

                        ShapeStyle = new
                        {
                            Fill = (string)null,
                            Stroke = (string)null,
                            StrokeWidth = (int?)default,
                            StrokeDash = (int?)default,
                            Opacity = (double?)default,
                        }
                    },
                    Cardinality = new
                    {
                        ShowCrowsFoot = true,
                        LabelStyle = new
                        {
                            IsVisible = true,
                            FontSize = (int?)default,
                            FontColor = (string)default,
                            Bold = (bool?)default,
                            Underline = (bool?)default,
                            Italic = (bool?)default
                        },
                        OneToOneLabel = "ONE-TO-ONE",
                        OneToManyLabel = "ONE-TO-MANY"
                    },
                    Groups = new object[0]      // IEnumerable<KeyValuePair<string, EntityGroup>>
                };

                expected.Should().BeEquivalentTo(options);
            });
        }

        public class Group_No_Style : ErdOptionsFixture
        {
            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("  ")]
            public void Should_Set_Empty_Title(string title)
            {
                var alias = Create<string>();
                var groupStyle = Create<ShapeStyle>();

                var options = new ErdOptions();

                options.Group(alias, null, grp =>
                {
                    grp.Add<Author>();
                    grp.Add<PostEntity>();

                    grp.ShapeStyle.CopyFrom(groupStyle);
                });

                var grp = options.Groups.Single();

                grp.Key.Should().Be(alias);

                var entityGroup = grp.Value;

                entityGroup.Title.Should().Be("\"\"");
            }

            [Fact]
            public void Should_Configure_Group()
            {
                var alias = Create<string>();
                var title = Create<string>();
                var groupStyle = Create<ShapeStyle>();

                var options = new ErdOptions();

                options.Group(alias, title, grp =>
                {
                    grp.Add<Author>();
                    grp.Add<PostEntity>();

                    grp.ShapeStyle.CopyFrom(groupStyle);
                });

                var grp = options.Groups.Single();

                grp.Key.Should().Be(alias);

                var entityGroup = grp.Value;

                entityGroup.Title.Should().Be(title);

                entityGroup.ShapeStyle.Should().NotBeSameAs(groupStyle);        // Must be copied
                entityGroup.ShapeStyle.Should().BeEquivalentTo(groupStyle);

                entityGroup.EntityTypes.Should().BeEquivalentTo(new[] { typeof(Author), typeof(PostEntity) });
            }

            [Fact]
            public void Should_Throw_When_Alias_Exists()
            {
                var alias = Create<string>();

                var options = new ErdOptions();

                options.Group(alias, Create<string>(), grp =>
                {
                });

                Invoking(() =>
                {
                    options.Group(alias, Create<string>(), grp =>
                    {
                    });
                })
                .Should()
                .Throw<DiagramException>()
                .WithMessage($"The group alias '{alias}' already exists.");
            }

            [Fact]
            public void Should_Throw_When_Entity_Already_Associated_With_An_Alias()
            {
                var alias = Create<string>();

                var options = new ErdOptions();

                options.Group(alias, Create<string>(), grp =>
                {
                    grp.Add<Author>();
                });

                Invoking(() =>
                {
                    options.Group(Create<string>(), Create<string>(), grp =>
                    {
                        grp.Add<Author>();
                    });
                })
                .Should()
                .Throw<DiagramException>()
                .WithMessage($"The entity type 'Author' is already associated with group alias '{alias}'.");
            }

            [Fact]
            public void Should_Get_Alias()
            {
                var expected = Create<string>();

                var options = new ErdOptions();

                options.Group(expected, Create<string>(), grp =>
                {
                    grp.Add<Author>();
                });

                var alias = options.Groups.GetAlias(typeof(Author));

                alias.Should().Be(expected);
            }

            [Fact]
            public void Should_Not_Get_Alias()
            {
                var expected = Create<string>();

                var options = new ErdOptions();

                options.Group(expected, Create<string>(), grp =>
                {
                    grp.Add<Author>();
                });

                var alias = options.Groups.GetAlias(typeof(Blog));

                alias.Should().BeNull();
            }

            [Fact]
            public void Should_Get_All_Groups()
            {
                var options = new ErdOptions();

                options.Group(Create<string>(), Create<string>(), grp =>
                {
                    grp.Add<Author>();
                });

                options.Group(Create<string>(), Create<string>(), grp =>
                {
                    grp.Add<Blog>();
                });

                var expected = options.Groups.ToArray();

                IEnumerable enumerable = options.Groups;

                var enumerator = enumerable.GetEnumerator();
                var index = 0;

                while (enumerator.MoveNext())
                {
                    enumerator.Current.Should().BeEquivalentTo(expected[index++]);
                }

                index.Should().Be(2);
            }
        }

        public class Group_With_Style : ErdOptionsFixture
        {
            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("  ")]
            public void Should_Set_Empty_Title(string title)
            {
                var alias = Create<string>();
                var groupStyle = Create<ShapeStyle>();

                var options = new ErdOptions();

                options.Group(alias, null, groupStyle, grp =>
                {
                    grp.Add<Author>();
                    grp.Add<PostEntity>();
                });

                var grp = options.Groups.Single();

                grp.Key.Should().Be(alias);

                var entityGroup = grp.Value;

                entityGroup.Title.Should().Be("\"\"");
            }

            [Fact]
            public void Should_Configure_Group()
            {
                var alias = Create<string>();
                var title = Create<string>();
                var groupStyle = Create<ShapeStyle>();

                var options = new ErdOptions();

                options.Group(alias, title, groupStyle, grp =>
                {
                    grp.Add("Author");          // Shadow entity by table name
                    grp.Add<PostEntity>();      // Regular entity by type
                });

                var grp = options.Groups.Single();

                grp.Key.Should().Be(alias);

                var entityGroup = grp.Value;

                entityGroup.Title.Should().Be(title);

                entityGroup.ShapeStyle.Should().BeSameAs(groupStyle);           // Must be assigned
                entityGroup.ShapeStyle.Should().BeEquivalentTo(groupStyle);

                entityGroup.EntityTypes.Should().BeEquivalentTo(new[] { typeof(PostEntity) });
                entityGroup.TableNames.Should().BeEquivalentTo(new[] { "Author" });
            }

            [Fact]
            public void Should_Throw_When_Alias_Exists()
            {
                var alias = Create<string>();

                var options = new ErdOptions();

                options.Group(alias, Create<string>(), Create<ShapeStyle>(), grp =>
                {
                });

                Invoking(() =>
                {
                    options.Group(alias, Create<string>(), grp =>
                    {
                    });
                })
                .Should()
                .Throw<DiagramException>()
                .WithMessage($"The group alias '{alias}' already exists.");
            }

            [Fact]
            public void Should_Throw_When_Entity_Already_Associated_With_An_Alias()
            {
                var alias = Create<string>();

                var options = new ErdOptions();

                options.Group(alias, Create<string>(), Create<ShapeStyle>(), grp =>
                {
                    grp.Add<Author>();
                });

                Invoking(() =>
                {
                    options.Group(Create<string>(), Create<string>(), grp =>
                    {
                        grp.Add<Author>();
                    });
                })
                .Should()
                .Throw<DiagramException>()
                .WithMessage($"The entity type 'Author' is already associated with group alias '{alias}'.");
            }

            [Fact]
            public void Should_Get_Alias()
            {
                var expected = Create<string>();

                var options = new ErdOptions();

                options.Group(expected, Create<string>(), Create<ShapeStyle>(), grp =>
                {
                    grp.Add<Author>();
                });

                var alias = options.Groups.GetAlias(typeof(Author));

                alias.Should().Be(expected);
            }

            [Fact]
            public void Should_Not_Get_Alias()
            {
                var expected = Create<string>();

                var options = new ErdOptions();

                options.Group(expected, Create<string>(), Create<ShapeStyle>(), grp =>
                {
                    grp.Add<Author>();
                });

                var alias = options.Groups.GetAlias(typeof(Blog));

                alias.Should().BeNull();
            }

            [Fact]
            public void Should_Get_All_Groups()
            {
                var options = new ErdOptions();

                options.Group(Create<string>(), Create<string>(), Create<ShapeStyle>(), grp =>
                {
                    grp.Add<Author>();
                });

                options.Group(Create<string>(), Create<string>(), Create<ShapeStyle>(), grp =>
                {
                    grp.Add<Blog>();
                });

                var expected = options.Groups.ToArray();

                IEnumerable enumerable = options.Groups;

                var enumerator = enumerable.GetEnumerator();
                var index = 0;

                while (enumerator.MoveNext())
                {
                    enumerator.Current.Should().BeEquivalentTo(expected[index++]);
                }

                index.Should().Be(2);
            }
        }

        public class Entity_Generic : ErdOptionsFixture
        {
            [Fact]
            public void Should_Copy_Global_Options()
            {
                var erdOptions = new ErdOptions();

                erdOptions.Entities.CopyFrom(Create<ErdOptions.EntityOptions>());

                var expected = erdOptions.Entities;

                var actual = erdOptions.Entity<Author>(true);

                expected.Should().BeEquivalentTo(actual);
                expected.Should().NotBeSameAs(actual);
            }

            [Fact]
            public void Should_Not_Copy_Global_Options()
            {
                var erdOptions = new ErdOptions();

                erdOptions.Entities.CopyFrom(Create<ErdOptions.EntityOptions>());

                var expected = erdOptions.Entities;

                var actual = erdOptions.Entity<Author>(false);

                expected.Should().NotBeEquivalentTo(actual);
            }
        }

        public class Entity_Type : ErdOptionsFixture
        {
            [Fact]
            public void Should_Copy_Global_Options()
            {
                var erdOptions = new ErdOptions();

                erdOptions.Entities.CopyFrom(Create<ErdOptions.EntityOptions>());

                var expected = erdOptions.Entities;

                var actual = erdOptions.Entity(typeof(PostEntity), true);

                expected.Should().BeEquivalentTo(actual);
                expected.Should().NotBeSameAs(actual);
            }

            [Fact]
            public void Should_Not_Copy_Global_Options()
            {
                var erdOptions = new ErdOptions();

                erdOptions.Entities.CopyFrom(Create<ErdOptions.EntityOptions>());

                var expected = erdOptions.Entities;

                var actual = erdOptions.Entity(typeof(PostEntity), false);

                expected.Should().NotBeEquivalentTo(actual);
            }
        }

        public class TableNames : ErdOptionsFixture
        {
            [Fact]
            public void Should_Add_Shadow_Entity_By_Table_Name()
            {
                var tableName = Create<string>();
                var options = new ErdOptions();

                options.Group(Create<string>(), Create<string>(), grp =>
                {
                    grp.Add(tableName);
                });

                var entityGroup = options.Groups.Single().Value;

                entityGroup.TableNames.Should().ContainSingle().Which.Should().Be(tableName);
                entityGroup.EntityTypes.Should().BeEmpty();
            }

            [Fact]
            public void Should_Add_Multiple_Shadow_Entities_By_Table_Name()
            {
                var tableName1 = Create<string>();
                var tableName2 = Create<string>();
                var options = new ErdOptions();

                options.Group(Create<string>(), Create<string>(), grp =>
                {
                    grp.Add(tableName1);
                    grp.Add(tableName2);
                });

                var entityGroup = options.Groups.Single().Value;

                entityGroup.TableNames.Should().BeEquivalentTo(new[] { tableName1, tableName2 });
                entityGroup.EntityTypes.Should().BeEmpty();
            }

            [Fact]
            public void Should_Get_Alias_By_Table_Name()
            {
                var expected = Create<string>();
                var tableName = Create<string>();
                var options = new ErdOptions();

                options.Group(expected, Create<string>(), grp =>
                {
                    grp.Add(tableName);
                });

                var alias = options.Groups.GetAlias(tableName);

                alias.Should().Be(expected);
            }

            [Fact]
            public void Should_Not_Get_Alias_By_Table_Name()
            {
                var options = new ErdOptions();

                options.Group(Create<string>(), Create<string>(), grp =>
                {
                    grp.Add("SomeTable");
                });

                var alias = options.Groups.GetAlias("OtherTable");

                alias.Should().BeNull();
            }

            [Fact]
            public void Should_Throw_When_Table_Name_Already_Associated_With_An_Alias()
            {
                var alias = Create<string>();
                var tableName = Create<string>();

                var options = new ErdOptions();

                options.Group(alias, Create<string>(), grp =>
                {
                    grp.Add(tableName);
                });

                Invoking(() =>
                {
                    options.Group(Create<string>(), Create<string>(), grp =>
                    {
                        grp.Add(tableName);
                    });
                })
                .Should()
                .Throw<DiagramException>()
                .WithMessage($"The table '{tableName}' is already associated with group alias '{alias}'.");
            }

            [Fact]
            public void Should_Add_Mix_Of_Types_And_Table_Names()
            {
                var tableName = Create<string>();
                var options = new ErdOptions();

                options.Group(Create<string>(), Create<string>(), grp =>
                {
                    grp.Add<Author>();
                    grp.Add(tableName);
                    grp.Add<PostEntity>();
                });

                var entityGroup = options.Groups.Single().Value;

                entityGroup.EntityTypes.Should().BeEquivalentTo(new[] { typeof(Author), typeof(PostEntity) });
                entityGroup.TableNames.Should().ContainSingle().Which.Should().Be(tableName);
            }
        }
    }
}