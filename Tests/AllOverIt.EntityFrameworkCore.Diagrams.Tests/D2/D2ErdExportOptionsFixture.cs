﻿using AllOverIt.EntityFrameworkCore.Diagrams.D2;
using FluentAssertions;
using System.Collections.Generic;
using System.Diagnostics;
using TestUtils;

namespace AllOverIt.EntityFrameworkCore.Diagrams.Tests.D2
{
    public class D2ErdExportOptionsFixture : NSubstituteFixtureBase
    {
        public class Constructor : D2ErdExportOptionsFixture
        {
            [Fact]
            public void Should_Set_Defaults()
            {
                var actual = new D2ErdExportOptions();

                var expected = new
                {
                    DefaultTheme = Theme.Neutral,
                    DiagramFileName = (string) null,
                    Theme = Theme.Neutral,
                    LayoutEngine = "elk",
                    Formats = (IEnumerable<ExportFormat>) null,
                    StandardOutputHandler = (DataReceivedEventHandler) null,
                    ErrorOutputHandler = (DataReceivedEventHandler) null
                };

                expected.Should().BeEquivalentTo(actual);
            }
        }
    }
}