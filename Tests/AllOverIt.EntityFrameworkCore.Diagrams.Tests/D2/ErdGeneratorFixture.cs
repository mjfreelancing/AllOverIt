using AllOverIt.EntityFrameworkCore.Diagrams.D2;
using AllOverIt.Fixture;
using FluentAssertions;

namespace AllOverIt.EntityFrameworkCore.Diagrams.Tests.D2
{
    public class ErdGeneratorFixture : FixtureBase
    {
        public class Create : ErdGeneratorFixture
        {
            [Fact]
            public void Should_Create_ErdGenerator()
            {
                var actual = ErdGenerator.Create<D2ErdGenerator>(options => { });

                actual.Should().BeOfType<D2ErdGenerator>();
            }

            [Fact]
            public void Should_Configure_Options()
            {
                ErdOptions actual = null;

                _ = ErdGenerator.Create<D2ErdGenerator>(options =>
                {
                    actual = options;
                });

                actual.Should().BeOfType<ErdOptions>();
            }

            [Fact]
            public void Should_Not_Throw_When_Configure_Null()
            {
                Invoking(() =>
                {
                    _ = ErdGenerator.Create<D2ErdGenerator>(null);
                })
                .Should()
                .NotThrow();
            }
        }
    }
}