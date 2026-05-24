using AllOverIt.EntityFrameworkCore.Diagrams.D2;
using AllOverIt.Fixture;
using Shouldly;

namespace AllOverIt.EntityFrameworkCore.Diagrams.Tests
{
    public class ErdGeneratorFixture : FixtureBase
    {
        public class Create : ErdGeneratorFixture
        {
            [Fact]
            public void Should_Create_ErdGenerator()
            {
                var actual = ErdGenerator.Create<D2ErdGenerator>(options => { });

                actual.ShouldBeOfType<D2ErdGenerator>();
            }

            [Fact]
            public void Should_Configure_Options()
            {
                ErdOptions actual = null;

                _ = ErdGenerator.Create<D2ErdGenerator>(options =>
                {
                    actual = options;
                });

                actual.ShouldBeOfType<ErdOptions>();
            }

            [Fact]
            public void Should_Not_Throw_When_Configure_Null()
            {
                Should.NotThrow(() =>
                {
                    _ = ErdGenerator.Create<D2ErdGenerator>(null);
                });
            }
        }
    }
}