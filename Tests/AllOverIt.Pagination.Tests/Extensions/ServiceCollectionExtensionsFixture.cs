using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Pagination.TokenEncoding;
using Microsoft.Extensions.DependencyInjection;

namespace AllOverIt.Pagination.Tests.Extensions
{
    public class ServiceCollectionExtensionsFixture : FixtureBase
    {
        public class AddQueryPagination : ServiceCollectionExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_ServiceCollection_Null()
            {
                Invoking(() =>
                {
                    _ = AllOverIt.Pagination.Extensions.ServiceCollectionExtensions.AddQueryPagination(null);
                })
                .ShouldThrow<ArgumentNullException>()
                .WithNamedMessageWhenNull("serviceCollection");
            }

            [Fact]
            public void Should_Register_Services()
            {
                var services = new ServiceCollection();

                _ = AllOverIt.Pagination.Extensions.ServiceCollectionExtensions.AddQueryPagination(services);

                (Type ServiceType, Type ImplemenationType)[] expected = new[]
                {
                    (typeof(IContinuationTokenValidator), typeof(ContinuationTokenValidator)),
                    (typeof(IContinuationTokenSerializerFactory), typeof(ContinuationTokenSerializerFactory)),
                    (typeof(IContinuationTokenEncoderFactory), typeof(ContinuationTokenEncoderFactory)),
                    (typeof(IQueryPaginatorFactory), typeof(QueryPaginatorFactory))
                };

                foreach (var item in expected)
                {
                    services
                        .SingleOrDefault(descriptor => descriptor.ServiceType == item.ServiceType &&
                                         descriptor.ImplementationType == item.ImplemenationType)
                        .ShouldNotBeNull();
                }
            }

            [Fact]
            public void Should_Return_Same_Services()
            {
                var services = new ServiceCollection();

                var actual = AllOverIt.Pagination.Extensions.ServiceCollectionExtensions.AddQueryPagination(services);

                actual.ShouldBeSameAs(services);
            }
        }
    }
}




