using AllOverIt.EntityFrameworkCore.Diagrams;
using AllOverIt.EntityFrameworkCore.Diagrams.D2;
using AllOverIt.EntityFrameworkCore.Diagrams.D2.Extensions;
using AllOverIt.EntityFrameworkCore.Diagrams.Extensions;
using D2ErdDiagramDemo.Data;
using D2ErdDiagramDemo.Data.Entities;
using System.Diagnostics;

namespace D2ErdDiagramDemo
{
    internal class Program
    {
        static async Task Main()
        {
            // Defaults are:
            // * Show non-nullable columns as [NOT NULL]
            // * Show MaxLength
            // * Show 1:1 cardinality as "ONE-TO-ONE"
            // * Show 1:N cardinality as "ONE-TO-MANY"
            // * Cardinality label style is as per D2 defaults
            var erdFormatter = ErdGenerator
                .Create<D2ErdGenerator>(options =>
                {
                    options.Direction = ErdOptions.DiagramDirection.Right;

                    AddEntityGroups(options);

                    // Define the global Entities options before creating the entity specific options (so they can be pre-filled).
                    options.Entities.Nullable.IsVisible = true;
                    options.Entities.Nullable.Mode = NullableColumnMode.NotNull;

                    // config.Entity.IsNullLabel = ...
                    // config.Entity.NotNullLabel = ...;

                    // This is the default
                    options.Entities.ShowMaxLength = true;

                    // Testing turning off features for one entity
                    var commentOptions = options.Entity<Comment>();

                    commentOptions.ShowMaxLength = false;
                    commentOptions.Nullable.IsVisible = false;

                    var globalShapeStyle = new ShapeStyle
                    {
                        Stroke = "#ccffcc"
                    };

                    // Set individual properties
                    options.Entities.ShapeStyle.Stroke = globalShapeStyle.Stroke;

                    //// .. or like this
                    //options.Entities.SetShapeStyle(style =>
                    //{
                    //    style.Stroke = "#ccffcc";
                    //});

                    //// .. or like this
                    //options.Entities.SetShapeStyle(globalShapeStyle);


                    // Selectively style an entity
                    var shapeStyle = new ShapeStyle
                    {
                        Fill = "black",
                        Stroke = "#99ccff", // pale blue
                        StrokeWidth = 4,
                        StrokeDash = 2
                    };

                    // Can update individual properties - the default is to initialize using the same as the global options
                    options.Entity<Author>(/*true*/).SetShapeStyle(style =>
                    {
                        style.Fill = shapeStyle.Fill;
                        style.Stroke = shapeStyle.Stroke;
                        style.StrokeWidth = shapeStyle.StrokeWidth;
                        style.StrokeDash = shapeStyle.StrokeDash;
                    });

                    // .. or completely replace the style
                    options.Entity<Blog>(false).SetShapeStyle(shapeStyle);


                    // Individual properties
                    options.Cardinality.LabelStyle.FontSize = 18;

                    // .. or via this extension method
                    options.Cardinality.SetLabelStyle(style =>
                    {
                        style.IsVisible = false;

                        style.FontSize = 18;
                        style.FontColor = "blue";
                        style.Bold = true;
                        // style.Underline = false;
                        // style.Italic = false;
                    });

                    // .. or replace the entire style
                    //var labelStyle = new LabelStyle
                    //{
                    //    FontSize = 18,
                    //    FontColor = "blue",
                    //    Bold = true
                    //};


                    //options.Cardinality.SetLabelStyle(labelStyle);


                    // Can optionally change the cardinality labels
                    //config.Cardinality.OneToOneLabel = ...;
                    //config.Cardinality.OneToManyLabel = ...;
                });

            var dbContext = new AppDbContext();

            // This generates the diagram as text
            var erd = erdFormatter.Generate(dbContext);
            Console.WriteLine(erd);

            // This generates the diagram, saves it as a text file and exports to SVG, PNG, PDF
            var exportOptions = new D2ErdExportOptions
            {
                DiagramFileName = "..\\..\\..\\Output Examples\\sample_erd.d2",
                LayoutEngine = "elk",
                Theme = Theme.Neutral,
                Formats = [ExportFormat.Svg, ExportFormat.Png, ExportFormat.Pdf],
                StandardOutputHandler = LogOutput,
                ErrorOutputHandler = LogOutput          // Note: d2.exe seems to log everything to the error output
            };

            await erdFormatter.ExportAsync(dbContext, exportOptions);

            Console.WriteLine();
            Console.WriteLine("Done.");
            Console.ReadKey(true);
        }

        private static void AddEntityGroups(ErdOptions options)
        {
            // The first, without an explicit style...
            options.Group("web", "Web", new ShapeStyle(), entities =>
            {
                entities
                    .Add<WebSiteEntity>()
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

            // The third
            var rolePermGroupStyle = new ShapeStyle
            {
                Fill = "#ffffcc", // pale yellow
                Stroke = "#cccc99" // darker yellow
            };

            options.Group("role_perm", "Roles / Permissions", rolePermGroupStyle, entities =>
            {
                entities
                    .Add<Role>()
                    .Add<Permission>()
                    .Add("AuthorRole")          // There's no entity for the join tables
                    .Add("RolePermission");
            });
        }

        private static void LogOutput(object sender, DataReceivedEventArgs e)
        {
            if (e.Data is not null)
            {
                Console.WriteLine(e.Data);
            }
        }
    }
}