using AllOverIt.EntityFrameworkCore.Diagrams.D2;
using AllOverIt.Fixture;
using AllOverIt.Shouldly.Extensions;
using System.Diagnostics;

namespace AllOverIt.EntityFrameworkCore.Diagrams.Tests.D2
{
    public class D2ErdExportOptionsFixture : FixtureBase
    {
        public class Constructor : D2ErdExportOptionsFixture
        {
            [Fact]
            public void Should_Set_Defaults()
            {
                var actual = new D2ErdExportOptions
                {
                    DiagramFileName = string.Empty
                };

                var expected = new
                {
                    DefaultTheme = Theme.Neutral,
                    DiagramFileName = string.Empty,
                    Theme = Theme.Neutral,
                    LayoutEngine = "elk",
                    Formats = (IEnumerable<ExportFormat>)null,
                    StandardOutputHandler = (DataReceivedEventHandler)null,
                    ErrorOutputHandler = (DataReceivedEventHandler)null
                };

                actual.ShouldBeEquivalentTo(expected);
            }
        }
    }
}