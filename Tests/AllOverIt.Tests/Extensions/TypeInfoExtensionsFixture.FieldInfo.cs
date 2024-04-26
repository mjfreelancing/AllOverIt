using AllOverIt.Extensions;
using FluentAssertions;
using System.Reflection;

namespace AllOverIt.Tests.Extensions
{
    public partial class TypeInfoExtensionsFixture
    {
        private class FieldBaseClass
        {
            public int Field1;
            public string Field2;
            private static double Field5;
        }

        private class FieldSuperClass : FieldBaseClass
        {
            public long Field3;

            private long Field4;
        }

        public class GetFieldInfo_All : TypeInfoExtensionsFixture
        {
            [Fact]
            public void Should_Get_All_Fields_Of_Super()
            {
                var typeInfo = typeof(FieldSuperClass).GetTypeInfo();

                var actual = TypeInfoExtensions.GetFieldInfo(typeInfo, false);

                var expected = new[]
                {
                    new {Name = "Field1", FieldType = typeof(int)},
                    new {Name = "Field2", FieldType = typeof(string)},
                    new {Name = "Field3", FieldType = typeof(long)},
                    new {Name = "Field4", FieldType = typeof(long)},
                    new {Name = "Field5", FieldType = typeof(double)}
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Get_All_Fields_Of_Base()
            {
                var typeInfo = typeof(FieldBaseClass).GetTypeInfo();

                var actual = TypeInfoExtensions.GetFieldInfo(typeInfo, false);

                var expected = new[]
                {
                    new {Name = "Field1", FieldType = typeof(int)},
                    new {Name = "Field2", FieldType = typeof(string)},
                    new {Name = "Field5", FieldType = typeof(double)}
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Get_Declared_Fields_Only()
            {
                var typeInfo = typeof(FieldSuperClass).GetTypeInfo();

                var actual = TypeInfoExtensions.GetFieldInfo(typeInfo, true);

                var expected = new[]
                {
                    new {Name = "Field3", FieldType = typeof(long)},
                    new {Name = "Field4", FieldType = typeof(long)}
                };

                actual.Should().BeEquivalentTo(expected);
            }
        }

        public class GetFieldInfo_Field : TypeInfoExtensionsFixture
        {
            [Fact]
            public void Should_Get_Field_Of_Super()
            {
                var typeInfo = typeof(FieldSuperClass).GetTypeInfo();

                // cast is required for BeEquivalentTo()
                var actual = (object) TypeInfoExtensions.GetFieldInfo(typeInfo, "Field4");

                var expected = new { Name = "Field4", FieldType = typeof(long) };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Get_Field_Of_Base()
            {
                var typeInfo = typeof(FieldBaseClass).GetTypeInfo();

                var actual = (object) TypeInfoExtensions.GetFieldInfo(typeInfo, "Field1");

                var expected = new { Name = "Field1", FieldType = typeof(int) };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Not_Get_Field()
            {
                var typeInfo = typeof(FieldSuperClass).GetTypeInfo();

                var actual = (object) TypeInfoExtensions.GetFieldInfo(typeInfo, "PropXYZ");

                actual.Should().BeNull();
            }
        }
    }
}