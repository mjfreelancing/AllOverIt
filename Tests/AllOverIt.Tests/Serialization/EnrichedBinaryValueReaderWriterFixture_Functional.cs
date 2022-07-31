using AllOverIt.Fixture;
using AllOverIt.Serialization.Binary;
using AllOverIt.Tests.Serialization.Binary.FunctionalTypes.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace AllOverIt.Tests.Serialization
{
    public class EnrichedBinaryValueReaderWriterFixture : FixtureBase
    {
        [Fact]
        public void Should_Read_Write_Using_Custom_Reader_Writer()
        {
            var classrooms = CreateMany<Classroom>();

        }
    }
}