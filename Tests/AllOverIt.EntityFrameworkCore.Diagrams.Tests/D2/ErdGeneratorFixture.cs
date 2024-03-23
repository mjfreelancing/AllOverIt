using AllOverIt.EntityFrameworkCore.Diagrams.D2;
using AllOverIt.EntityFrameworkCore.Diagrams.Tests.TestTypes;
using AllOverIt.EntityFrameworkCore.Diagrams.Tests.TestTypes.Entities;
using AllOverIt.Fixture;
using FluentAssertions;
using System;

namespace AllOverIt.EntityFrameworkCore.Diagrams.Tests.D2
{
    public class ErdGeneratorFixture : FixtureBase
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
        public void Should_Set_Diagram_Direction()
        {
            var direction = Create<ErdOptions.DiagramDirection>();

            AssertDiagram(options =>
            {
                options.Direction = direction;
            }, DiagramExpectations.WithDirection(direction));
        }

        /*
            public bool PreserveColumnOrder { get; private set; } = true;

            public NullableColumn Nullable { get; } = new();

            public bool ShowMaxLength { get; set; } = true;
         */

        [Fact]
        public void Should_Set_Global_Not_PreserveColumnOrder()
        {
            AssertDiagram(options =>
            {
                options.Entities.PreserveColumnOrder = false;
            }, DiagramExpectations.WithNoPreserveOrderGlobal());
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
                    .Add<Post>();
            });
        }

        private static void AssertDiagram(Action<ErdOptions> action, string expected)
        {
            var erdFormatter = ErdGenerator.Create<D2ErdGenerator>(action);

            var dbContext = new TestDbContext();

            var actual = erdFormatter.Generate(dbContext).Trim();

            actual.Should().Be(expected.Trim());
        }
    }
}