using AllOverIt.EntityFrameworkCore.Diagrams.D2;
using AllOverIt.EntityFrameworkCore.Diagrams.Tests.TestTypes;
using AllOverIt.EntityFrameworkCore.Diagrams.Tests.TestTypes.Entities;
using FluentAssertions;
using TestUtils;

namespace AllOverIt.EntityFrameworkCore.Diagrams.Tests.D2
{
    public class D2ErdGeneratorFixture : NSubstituteFixtureBase
    {
        [Fact]
        public void Should_Create_Default_Diagram()
        {
            // Defaults are:
            // * Show non-nullable columns as [NOT NULL]
            // * Show MaxLength
            // * Show 1:1 cardinality as "ONE-TO-ONE"
            // * Show 1:N cardinality as "ONE-TO-MANY"
            // * Cardinality label style is as per D2 defaults
            AssertDiagram(options => { }, DiagramExpectations.DefaultDiagram);
        }

        [Fact]
        public void Should_Set_Direction()
        {
            var direction = Create<ErdOptions.DiagramDirection>();

            AssertDiagram(options =>
            {
                options.Direction = direction;
            }, DiagramExpectations.WithDirection(direction));
        }

        [Fact]
        public void Should_Set_Global_Not_PreserveColumnOrder()
        {
            AssertDiagram(options =>
            {
                options.Entities.PreserveColumnOrder = false;
            }, DiagramExpectations.WithGlobalNotPreserveColumnOrder());
        }

        [Fact]
        public void Should_Set_Global_Nullable_Not_Visible()
        {
            AssertDiagram(options =>
            {
                options.Entities.Nullable.IsVisible = false;
            }, DiagramExpectations.WithGlobalNullableNotVisible());
        }

        [Fact]
        public void Should_Set_Global_Nullable_IsNull()
        {
            AssertDiagram(options =>
            {
                options.Entities.Nullable.Mode = NullableColumnMode.IsNull;
            }, DiagramExpectations.WithGlobalNullableIsNull());
        }

        [Fact]
        public void Should_Set_Global_Nullable_NotNull_Label()
        {
            AssertDiagram(options =>
            {
                options.Entities.Nullable.Mode = NullableColumnMode.NotNull;
                options.Entities.Nullable.NotNullLabel = "*Is Not Null*";
            }, DiagramExpectations.WithGlobalNullableIsNotNullLabel());
        }

        [Fact]
        public void Should_Set_Global_Nullable_IsNull_Label()
        {
            AssertDiagram(options =>
            {
                options.Entities.Nullable.Mode = NullableColumnMode.IsNull;
                options.Entities.Nullable.IsNullLabel = "#Is Null#";
            }, DiagramExpectations.WithGlobalNullableIsNullLabel());
        }

        [Fact]
        public void Should_Set_Global_Nullable_No_ShowMaxLength()
        {
            AssertDiagram(options =>
            {
                options.Entities.ShowMaxLength = false;
            }, DiagramExpectations.WithGlobalNullableNoShowMaxLength());
        }

        [Fact]
        public void Should_Set_Global_Cardinality_No_ShowCrowsFoot()
        {
            AssertDiagram(options =>
            {
                options.Cardinality.ShowCrowsFoot = false;
            }, DiagramExpectations.WithGlobalCardinalityNoShowCrowsFoot());
        }

        [Fact]
        public void Should_Set_Global_Cardinality_LabelStyle_Not_Visible()
        {
            AssertDiagram(options =>
            {
                options.Cardinality.LabelStyle.IsVisible = false;
            }, DiagramExpectations.WithGlobalCardinalityLabelStyleNotVisible());
        }

        [Fact]
        public void Should_Set_Global_Cardinality_LabelStyle_Font_Attributes()
        {
            AssertDiagram(options =>
            {
                options.Cardinality.LabelStyle.FontColor = "#f01234";
                options.Cardinality.LabelStyle.FontSize = 11;
                options.Cardinality.LabelStyle.Bold = true;
                options.Cardinality.LabelStyle.Italic = true;
                options.Cardinality.LabelStyle.Underline = true;
            }, DiagramExpectations.WithGlobalCardinalityLabelStyleFontAttributes());
        }

        [Fact]
        public void Should_Set_Global_Cardinality_OneToOneLabel()
        {
            AssertDiagram(options =>
            {
                options.Cardinality.OneToOneLabel = "^One-To-One^";
            }, DiagramExpectations.WithGlobalCardinalityOneToOneLabel());
        }

        [Fact]
        public void Should_Set_Global_Cardinality_OneToManyLabel()
        {
            AssertDiagram(options =>
            {
                options.Cardinality.OneToManyLabel = "<One-To-Many>";
            }, DiagramExpectations.WithGlobalCardinalityOneToManyLabel());
        }

        [Fact]
        public void Should_Set_Group_Default_Style()
        {
            AssertDiagram(options =>
            {
                options.Group("alias", "title", entityGroup =>
                {
                    entityGroup.Add<Author>();
                    entityGroup.Add<Blog>();
                    entityGroup.Add<AuthorBlog>();
                });
            }, DiagramExpectations.WithGroupDefaultStyle());
        }

        [Fact]
        public void Should_Set_Group_Custom_Style()
        {
            AssertDiagram(options =>
            {
                var style = new ShapeStyle()
                {
                    Fill = "red",
                    Stroke = "#00ff00",
                    Opacity = 0.6,
                    StrokeDash = 4,
                    StrokeWidth = 2
                };

                options.Group("alias", "title", style, entityGroup =>
                {
                    entityGroup.Add<Author>();
                    entityGroup.Add<Blog>();
                    entityGroup.Add<AuthorBlog>();
                });
            }, DiagramExpectations.WithGroupCustomStyle());
        }

        [Fact]
        public void Should_Set_Entity_Custom_Style()
        {
            AssertDiagram(options =>
            {
                var entityOptions = options.Entity<Blog>();

                entityOptions.ShapeStyle = new ShapeStyle
                {
                    Fill = "black",
                    Stroke = "#ccbb88",
                    Opacity = 0.4,
                    StrokeDash = 2,
                    StrokeWidth = 4
                };
            }, DiagramExpectations.WithEntityCustomStyle());
        }

        [Fact]
        public void Should_Set_All_Entities_Style()
        {
            AssertDiagram(options =>
            {
                options.Entities.ShapeStyle = new ShapeStyle
                {
                    Fill = "black",
                    Stroke = "#ccbb88",
                    Opacity = 0.4,
                    StrokeDash = 2,
                    StrokeWidth = 4
                };
            }, DiagramExpectations.WithAllEntitiesStyle());
        }

        private static void AddEntityGroups(ErdOptions options)
        {
            // The first, without an explicit style...
            options.Group("web", "Web", new ShapeStyle(), entities =>
            {
                entities
                    .Add<WebSite>()
                    .Add<Settings>();
            });

            // The second...
            var blogGroupStyle = new ShapeStyle
            {
                Fill = "#ffd8d8",
                Stroke = "blue",
                StrokeWidth = 4
            };

            options.Group("blog", null, blogGroupStyle, entities =>
            {
                entities
                    .Add<Author>()
                    .Add<Blog>()
                    .Add<AuthorBlog>()
                    .Add<PostEntity>();
            });
        }

        private static void AssertDiagram(Action<ErdOptions> action, string expected)
        {
            var options = new ErdOptions();

            action.Invoke(options);

            var erdFormatter = new D2ErdGenerator(options);

            var dbContext = new TestDbContext();

            var actual = erdFormatter.Generate(dbContext).Trim();

            actual.Should().Be(expected.Trim());
        }
    }
}