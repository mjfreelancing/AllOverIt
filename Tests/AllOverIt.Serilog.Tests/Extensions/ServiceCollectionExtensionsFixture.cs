using AllOverIt.Fixture;
using Microsoft.Extensions.DependencyInjection;

namespace AllOverIt.Serilog.Tests.Extensions
{
    public partial class ServiceCollectionExtensionsFixture : FixtureBase
    {
        private readonly IServiceCollection _services = new ServiceCollection();
    }
}