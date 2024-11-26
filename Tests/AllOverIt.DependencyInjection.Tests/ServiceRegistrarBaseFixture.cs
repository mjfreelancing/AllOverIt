using AllOverIt.DependencyInjection.Exceptions;
using AllOverIt.DependencyInjection.Tests.Helpers;
using AllOverIt.DependencyInjection.Tests.TestTypes;
using AllOverIt.DependencyInjection.Tests.Types;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace AllOverIt.DependencyInjection.Tests
{
    public class ServiceRegistrarBaseFixture : FixtureBase
    {
        private readonly IServiceCollection _serviceCollection;

        public ServiceRegistrarBaseFixture()
        {
            _serviceCollection = new ServiceCollection();
        }

        [Fact]
        public void Should_Register_Once()
        {
            // IBaseInterface1 will find ConcreteClassF and ConcreteClassG
            // IBaseInterface4 will find ConcreteClassG and also attempt to register it against IBaseInterface1
            // This test is to make sure IBaseInterface1 isn't registered twice for ConcreteClassG
            var serviceTypes = new[] { typeof(IBaseInterface1), typeof(IBaseInterface4) };

            var provider = DependencyHelper

                .AutoRegisterUsingServiceLifetime<LocalDependenciesRegistrar>(ServiceLifetime.Singleton, _serviceCollection, serviceTypes)

                .BuildServiceProvider();

            var int1Instance = provider.GetService<IEnumerable<IBaseInterface1>>()!.SelectToArray(item => item.GetType());

            int1Instance.Should().BeEquivalentTo(new[] { typeof(ConcreteClassF), typeof(ConcreteClassG) });

            var int4Instance = provider.GetService<IEnumerable<IBaseInterface4>>()!.SelectToArray(item => item.GetType());

            int4Instance.Should().BeEquivalentTo(new[] { typeof(ConcreteClassG) });
        }

        [Fact]
        public void Should_Throw_When_Registered_Different_Lifetime()
        {
            Invoking(() =>
                {
                    // IBaseInterface1 will find ConcreteClassF and ConcreteClassG
                    // IBaseInterface4 will find ConcreteClassG and also attempt to register it against IBaseInterface1 (with a different lifetime)
                    _ = DependencyHelper
                        .AutoRegisterUsingServiceLifetime<LocalDependenciesRegistrar, IBaseInterface1>(ServiceLifetime.Singleton, _serviceCollection)
                        .AutoRegisterUsingServiceLifetime<LocalDependenciesRegistrar, IBaseInterface4>(ServiceLifetime.Scoped)
                        .BuildServiceProvider();
                })
                .Should()
                .Throw<DependencyRegistrationException>()
                .WithMessage($"The service type {typeof(IBaseInterface4).GetFriendlyName()} is already registered to the implementation type {typeof(ConcreteClassG).GetFriendlyName()} but has a different lifetime ({ServiceLifetime.Singleton}).");
        }

        [Fact]
        public void Should_Register_All_Types()
        {
            var serviceTypes = new[] { typeof(IBaseInterface1), typeof(IBaseInterface2), typeof(IBaseInterface3), typeof(IBaseInterface4), typeof(IBaseInterface5), typeof(AbstractClassA) };

            var provider = DependencyHelper

                // Anything other than IBaseInterface3 will default construct the implementation type
                .AutoRegisterUsingServiceLifetime<LocalDependenciesRegistrar>(ServiceLifetime.Singleton, _serviceCollection, serviceTypes,
                    config => config.Filter((service, implementation) => service != typeof(IBaseInterface3)))

                // Anything other than IBaseInterface3 will default construct the implementation type
                .AutoRegisterUsingServiceLifetime<ExternalDependenciesRegistrar>(ServiceLifetime.Singleton, serviceTypes,
                    config => config.Filter((service, implementation) => service != typeof(IBaseInterface3)))

                // IBaseInterface3 implementations require an integer to be passed to their constructor
                .AutoRegisterUsingServiceLifetime<LocalDependenciesRegistrar>(ServiceLifetime.Singleton, serviceTypes,
                    (provider, serviceType) => new object[] { Create<int>() },
                    config => config.Filter((service, implementation) => service == typeof(IBaseInterface3)))

                // IBaseInterface3 implementations require an integer to be passed to their constructor
                .AutoRegisterUsingServiceLifetime<ExternalDependenciesRegistrar>(ServiceLifetime.Singleton, serviceTypes,
                    (provider, serviceType) => new object[] { Create<int>() },
                    config => config.Filter((service, implementation) => service == typeof(IBaseInterface3)))

                .BuildServiceProvider();

            DependencyHelper.AssertExpectation<IBaseInterface1>(provider, new[] { typeof(ConcreteClassA), typeof(ConcreteClassB), typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassF), typeof(ConcreteClassG) });

            DependencyHelper.AssertExpectation<IBaseInterface2>(provider, new[] { typeof(ConcreteClassA), typeof(ConcreteClassC), typeof(ConcreteClassE), typeof(ConcreteClassG) });

            DependencyHelper.AssertExpectation<IBaseInterface3>(provider, new[] { typeof(ConcreteClassH), typeof(ConcreteClassI), typeof(ConcreteClassJ) });

            DependencyHelper.AssertExpectation<IBaseInterface4>(provider, new[] { typeof(ConcreteClassG) });

            DependencyHelper.AssertExpectation<IBaseInterface5>(provider, new[] { typeof(ConcreteClassF) });

            DependencyHelper.AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassG) });
        }

        [Theory]
        [InlineData(ServiceLifetime.Singleton)]
        [InlineData(ServiceLifetime.Scoped)]
        [InlineData(ServiceLifetime.Transient)]
        public void Should_Register_As_Individual_Resolutions(ServiceLifetime lifetime)
        {
            var provider = DependencyHelper
                .AutoRegisterUsingServiceLifetime<LocalDependenciesRegistrar>(
                    lifetime,
                    _serviceCollection,
                    [typeof(IBaseInterface1), typeof(IBaseInterface2), typeof(IBaseInterface4), typeof(AbstractClassA)])
                .BuildServiceProvider();

            var concrete1 = provider.GetRequiredService<IBaseInterface1>();

            concrete1.Should().BeOfType<ConcreteClassG>();

            var concrete2 = provider.GetRequiredService<IBaseInterface2>();

            concrete2.Should().BeOfType<ConcreteClassG>();

            var concrete3 = provider.GetRequiredService<IBaseInterface4>();

            concrete3.Should().BeOfType<ConcreteClassG>();

            var concrete4 = provider.GetRequiredService<AbstractClassA>();

            concrete4.Should().BeOfType<ConcreteClassG>();

            concrete1.Should().NotBeSameAs(concrete2);
            concrete1.Should().NotBeSameAs(concrete3);
            concrete1.Should().NotBeSameAs(concrete4);
            concrete2.Should().NotBeSameAs(concrete3);
            concrete2.Should().NotBeSameAs(concrete4);
            concrete3.Should().NotBeSameAs(concrete4);
        }
    }
}