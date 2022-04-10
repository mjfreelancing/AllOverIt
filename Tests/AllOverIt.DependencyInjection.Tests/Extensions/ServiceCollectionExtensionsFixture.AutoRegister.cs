using AllOverIt.DependencyInjection.Tests.Types;
using AllOverIt.Fixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using AllOverIt.DependencyInjection.Tests.Helpers;
using AllOverIt.Extensions;
using AllOverIt.Fixture.Extensions;
using Xunit;

namespace AllOverIt.DependencyInjection.Tests.Extensions
{
    public partial class ServiceCollectionExtensionsFixture : FixtureBase
    {
        private readonly IServiceCollection _serviceCollection;

        protected ServiceCollectionExtensionsFixture()
        {
            _serviceCollection = new ServiceCollection();
        }

        public class AutoRegister_TServiceRegistrar_TServiceType : ServiceCollectionExtensionsFixture
        {
            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Throw_When_Services_Null(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime<ExternalDependenciesRegistrar, AbstractClassA>(lifetime, null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("serviceCollection");
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Not_Throw_When_Configure_Null(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime<ExternalDependenciesRegistrar, AbstractClassA>(lifetime, _serviceCollection, null);
                    })
                    .Should()
                    .NotThrow();
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Register_No_Exclude_Or_Filter(ServiceLifetime lifetime)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime<LocalDependenciesRegistrar, AbstractClassA>(lifetime, _serviceCollection)

                    .AutoRegisterUsingServiceLifetime<ExternalDependenciesRegistrar, AbstractClassA>(lifetime)

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Register_With_Filter(ServiceLifetime lifetime)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime<LocalDependenciesRegistrar, AbstractClassA>(lifetime, _serviceCollection)

                    .AutoRegisterUsingServiceLifetime<ExternalDependenciesRegistrar, AbstractClassA>(lifetime, config =>
                        config.Filter((service, implementation) => implementation != typeof(ConcreteClassE)))

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton, true)]
            [InlineData(ServiceLifetime.Scoped, true)]
            [InlineData(ServiceLifetime.Transient, false)]
            public void Should_Resolve_When_In_Same_Scope(ServiceLifetime lifetime, bool expected)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime<LocalDependenciesRegistrar, AbstractClassA>(lifetime, _serviceCollection)

                    .AutoRegisterUsingServiceLifetime<ExternalDependenciesRegistrar, AbstractClassA>(lifetime)

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassG) });

                var instances1 = provider.GetRequiredService<IEnumerable<AbstractClassA>>();
                var instances2 = provider.GetRequiredService<IEnumerable<AbstractClassA>>();

                DependencyHelper.AssertInstanceEquality(instances1, instances2, expected);
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton, true)]
            [InlineData(ServiceLifetime.Scoped, false)]
            [InlineData(ServiceLifetime.Transient, false)]
            public void Should_Resolve_When_In_Different_Scope(ServiceLifetime lifetime, bool expected)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime<LocalDependenciesRegistrar, AbstractClassA>(lifetime, _serviceCollection)

                    .AutoRegisterUsingServiceLifetime<ExternalDependenciesRegistrar, AbstractClassA>(lifetime)

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassG) });

                var instances1 = provider.GetRequiredService<IEnumerable<AbstractClassA>>();

                using (var scopedProvider = provider.CreateScope())
                {
                    var instances2 = scopedProvider.ServiceProvider.GetRequiredService<IEnumerable<AbstractClassA>>();

                    DependencyHelper.AssertInstanceEquality(instances1, instances2, expected);
                }
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Resolve_All_Interfaces(ServiceLifetime lifetime)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime<LocalDependenciesRegistrar, IBaseInterface1>(lifetime, _serviceCollection)

                    .AutoRegisterUsingServiceLifetime<ExternalDependenciesRegistrar, IBaseInterface1>(lifetime)

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<IBaseInterface1>(
                    provider,
                    new[] { typeof(ConcreteClassA), typeof(ConcreteClassB), typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassF), typeof(ConcreteClassG) });

                // ConcreteClassE implements IBaseInterface2 BUT it is not registered because it does not inherit IBaseInterface1
                DependencyHelper.AssertExpectation<IBaseInterface2>(
                    provider,
                    Enumerable.Empty<Type>());

                DependencyHelper.AssertExpectation<IBaseInterface5>(
                    provider,
                    new[] { typeof(ConcreteClassF) });

                DependencyHelper.AssertExpectation<IBaseInterface4>(
                    provider,
                    new[] { typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Resolve_All_Interfaces_Except_Registered_Service(ServiceLifetime lifetime)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime<LocalDependenciesRegistrar, IBaseInterface1>(lifetime, _serviceCollection,
                        config => config.Filter((service, implementation) => service != typeof(IBaseInterface1)))

                    .AutoRegisterUsingServiceLifetime<ExternalDependenciesRegistrar, IBaseInterface1>(lifetime,
                        config => config.Filter((service, implementation) => service != typeof(IBaseInterface1)))

                    .BuildServiceProvider();

                // IBaseInterface1 has been filtered out
                DependencyHelper.AssertExpectation<IBaseInterface1>(
                    provider,
                    Enumerable.Empty<Type>());

                DependencyHelper.AssertExpectation<IBaseInterface5>(
                    provider,
                    new[] { typeof(ConcreteClassF) });

                DependencyHelper.AssertExpectation<IBaseInterface4>(
                    provider,
                    new[] { typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Not_Resolve_Abstract_Class_When_Register_Interface(ServiceLifetime lifetime)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime<LocalDependenciesRegistrar, IBaseInterface1>(lifetime, _serviceCollection)

                    .AutoRegisterUsingServiceLifetime<ExternalDependenciesRegistrar, IBaseInterface1>(lifetime)

                    .BuildServiceProvider();

                // Not currently supporting the ability to do this - it could be extended to resolve ConcreteClassD, ConcreteClassE, ConcreteClassG
                // on the basis they all inherit AbstractClassA which inherits IBaseInterface1 but at this time you either register an abstract
                // class or an interface.
                DependencyHelper.AssertExpectation<AbstractClassA>(
                    provider,
                    Enumerable.Empty<Type>());
            }
        }

        public class AutoRegister_TServiceRegistrar_ServiceTypes : ServiceCollectionExtensionsFixture
        {
            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Throw_When_Services_Null(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime<ExternalDependenciesRegistrar>(lifetime, null, new[] {typeof(AbstractClassA), typeof(IBaseInterface2)});
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("serviceCollection");
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Throw_When_ServiceTypes_Null(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime<ExternalDependenciesRegistrar>(lifetime, _serviceCollection, (IEnumerable<Type>)null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("serviceTypes");
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Throw_When_ServiceTypes_Empty(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime<ExternalDependenciesRegistrar>(lifetime, _serviceCollection, new List<Type>());
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("serviceTypes");
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Not_Throw_When_Configure_Null(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime<ExternalDependenciesRegistrar>(lifetime, _serviceCollection,
                            new[] {typeof(AbstractClassA), typeof(IBaseInterface2)});
                    })
                    .Should()
                    .NotThrow();
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Register_No_Exclude_Or_Filter(ServiceLifetime lifetime)
            {
                var provider = DependencyHelper
                    
                    .AutoRegisterUsingServiceLifetime<LocalDependenciesRegistrar>(lifetime, _serviceCollection, new[] {typeof(AbstractClassA), typeof(IBaseInterface2) })

                    .AutoRegisterUsingServiceLifetime<ExternalDependenciesRegistrar>(lifetime, new[] {typeof(AbstractClassA), typeof(IBaseInterface2) })

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<AbstractClassA>(provider, new[] {typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassG)});
                DependencyHelper.AssertExpectation<IBaseInterface2>(provider, new[] { typeof(ConcreteClassA), typeof(ConcreteClassC), typeof(ConcreteClassE), typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Register_With_Filter(ServiceLifetime lifetime)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime<LocalDependenciesRegistrar>(lifetime, _serviceCollection, new[] { typeof(AbstractClassA), typeof(IBaseInterface2) })

                    .AutoRegisterUsingServiceLifetime<ExternalDependenciesRegistrar>(lifetime, new[] { typeof(AbstractClassA), typeof(IBaseInterface2) }, config =>
                        config.Filter((service, implementation) => implementation != typeof(ConcreteClassE)))

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassG) });
                DependencyHelper.AssertExpectation<IBaseInterface2>(provider, new[] { typeof(ConcreteClassA), typeof(ConcreteClassC), typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton, true)]
            [InlineData(ServiceLifetime.Scoped, true)]
            [InlineData(ServiceLifetime.Transient, false)]
            public void Should_Resolve_When_In_Same_Scope(ServiceLifetime lifetime, bool expected)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime<LocalDependenciesRegistrar>(lifetime, _serviceCollection, new[] { typeof(AbstractClassA), typeof(IBaseInterface2) })

                    .AutoRegisterUsingServiceLifetime<ExternalDependenciesRegistrar>(lifetime, new[] { typeof(AbstractClassA), typeof(IBaseInterface2) })

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassG) });
                DependencyHelper.AssertExpectation<IBaseInterface2>(provider, new[] { typeof(ConcreteClassA), typeof(ConcreteClassC), typeof(ConcreteClassE), typeof(ConcreteClassG) });

                var instances1a = provider.GetRequiredService<IEnumerable<AbstractClassA>>();
                var instances1b = provider.GetRequiredService<IEnumerable<AbstractClassA>>();

                DependencyHelper.AssertInstanceEquality(instances1a, instances1b, expected);

                var instances2a = provider.GetRequiredService<IEnumerable<IBaseInterface2>>();
                var instances2b = provider.GetRequiredService<IEnumerable<IBaseInterface2>>();

                DependencyHelper.AssertInstanceEquality(instances2a, instances2b, expected);
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton, true)]
            [InlineData(ServiceLifetime.Scoped, false)]
            [InlineData(ServiceLifetime.Transient, false)]
            public void Should_Resolve_When_In_Different_Scope(ServiceLifetime lifetime, bool expected)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime<LocalDependenciesRegistrar>(lifetime, _serviceCollection, new[] { typeof(AbstractClassA), typeof(IBaseInterface2) })

                    .AutoRegisterUsingServiceLifetime<ExternalDependenciesRegistrar>(lifetime, new[] { typeof(AbstractClassA), typeof(IBaseInterface2) })

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassG) });
                DependencyHelper.AssertExpectation<IBaseInterface2>(provider, new[] { typeof(ConcreteClassA), typeof(ConcreteClassC), typeof(ConcreteClassE), typeof(ConcreteClassG) });

                var instances1a = provider.GetRequiredService<IEnumerable<AbstractClassA>>();
                var instances2a = provider.GetRequiredService<IEnumerable<IBaseInterface2>>();

                using (var scopedProvider = provider.CreateScope())
                {
                    var instances1b = scopedProvider.ServiceProvider.GetRequiredService<IEnumerable<AbstractClassA>>();
                    var instances2b = scopedProvider.ServiceProvider.GetRequiredService<IEnumerable<IBaseInterface2>>();

                    DependencyHelper.AssertInstanceEquality(instances1a, instances1b, expected);
                    DependencyHelper.AssertInstanceEquality(instances2a, instances2b, expected);
                }
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Resolve_All_Interfaces(ServiceLifetime lifetime)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime<LocalDependenciesRegistrar>(lifetime, _serviceCollection, new[] { typeof(IBaseInterface1), typeof(IBaseInterface2) })

                    .AutoRegisterUsingServiceLifetime<ExternalDependenciesRegistrar>(lifetime, new[] { typeof(IBaseInterface1), typeof(IBaseInterface2) })

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<IBaseInterface1>(
                    provider,
                    new[] { typeof(ConcreteClassA), typeof(ConcreteClassB), typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassF), typeof(ConcreteClassG) });

                DependencyHelper.AssertExpectation<IBaseInterface2>(
                    provider,
                    new[] { typeof(ConcreteClassA), typeof(ConcreteClassC), typeof(ConcreteClassE), typeof(ConcreteClassG) });

                DependencyHelper.AssertExpectation<IBaseInterface5>(
                    provider,
                    new[] { typeof(ConcreteClassF) });

                DependencyHelper.AssertExpectation<IBaseInterface4>(
                    provider,
                    new[] { typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Resolve_All_Interfaces_Except_Registered_Service(ServiceLifetime lifetime)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime<LocalDependenciesRegistrar>(lifetime, _serviceCollection, new[] { typeof(IBaseInterface1), typeof(IBaseInterface2) },
                        config => config.Filter((service, implementation) => service != typeof(IBaseInterface1)))

                    .AutoRegisterUsingServiceLifetime<ExternalDependenciesRegistrar>(lifetime, new[] { typeof(IBaseInterface1), typeof(IBaseInterface2) },
                        config => config.Filter((service, implementation) => service != typeof(IBaseInterface1)))

                    .BuildServiceProvider();

                // IBaseInterface1 has been filtered out
                DependencyHelper.AssertExpectation<IBaseInterface1>(
                    provider,
                    Enumerable.Empty<Type>());

                DependencyHelper.AssertExpectation<IBaseInterface2>(
                    provider,
                    new[] { typeof(ConcreteClassA), typeof(ConcreteClassC), typeof(ConcreteClassE), typeof(ConcreteClassG) });

                DependencyHelper.AssertExpectation<IBaseInterface5>(
                    provider,
                    new[] { typeof(ConcreteClassF) });

                DependencyHelper.AssertExpectation<IBaseInterface4>(
                    provider,
                    new[] { typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Not_Resolve_Abstract_Class_When_Register_Interface(ServiceLifetime lifetime)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime<LocalDependenciesRegistrar>(lifetime, _serviceCollection, new[] { typeof(IBaseInterface1), typeof(IBaseInterface2) })

                    .AutoRegisterUsingServiceLifetime<ExternalDependenciesRegistrar>(lifetime, new[] { typeof(IBaseInterface1), typeof(IBaseInterface2) })

                    .BuildServiceProvider();

                // Not currently supporting the ability to do this - it could be extended to resolve ConcreteClassD, ConcreteClassE, ConcreteClassG
                // on the basis they all inherit AbstractClassA which inherits IBaseInterface1 but at this time you either register an abstract
                // class or an interface.
                DependencyHelper.AssertExpectation<AbstractClassA>(
                    provider,
                    Enumerable.Empty<Type>());
            }
        }

        public class AutoRegister_TServiceType_ServiceRegistrar_Instance : ServiceCollectionExtensionsFixture
        {
            private readonly LocalDependenciesRegistrar _localRegistrar = new();
            private readonly ExternalDependenciesRegistrar _externalRegistrar = new();

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Throw_When_Services_Null(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime<AbstractClassA>(lifetime, null, _localRegistrar);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("serviceCollection");
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Throw_When_ServiceRegistrar_Null(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime<AbstractClassA>(lifetime, _serviceCollection, null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("serviceRegistrar");
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Not_Throw_When_Configure_Null(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime<AbstractClassA>(lifetime, _serviceCollection, _localRegistrar, null);
                    })
                    .Should()
                    .NotThrow();
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Register_No_Exclude_Or_Filter(ServiceLifetime lifetime)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime<AbstractClassA>(lifetime, _serviceCollection, _localRegistrar)

                    .AutoRegisterUsingServiceLifetime<AbstractClassA>(lifetime, _externalRegistrar)

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Register_With_Filter(ServiceLifetime lifetime)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime<AbstractClassA>(lifetime, _serviceCollection, _localRegistrar)

                    .AutoRegisterUsingServiceLifetime<AbstractClassA>(lifetime, _externalRegistrar, config =>
                        config.Filter((service, implementation) => implementation != typeof(ConcreteClassE)))

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton, true)]
            [InlineData(ServiceLifetime.Scoped, true)]
            [InlineData(ServiceLifetime.Transient, false)]
            public void Should_Resolve_When_In_Same_Scope(ServiceLifetime lifetime, bool expected)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime<AbstractClassA>(lifetime, _serviceCollection, _localRegistrar)

                    .AutoRegisterUsingServiceLifetime<AbstractClassA>(lifetime, _externalRegistrar)

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassG) });

                var instances1 = provider.GetRequiredService<IEnumerable<AbstractClassA>>();
                var instances2 = provider.GetRequiredService<IEnumerable<AbstractClassA>>();

                DependencyHelper.AssertInstanceEquality(instances1, instances2, expected);
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton, true)]
            [InlineData(ServiceLifetime.Scoped, false)]
            [InlineData(ServiceLifetime.Transient, false)]
            public void Should_Resolve_When_In_Different_Scope(ServiceLifetime lifetime, bool expected)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime<AbstractClassA>(lifetime, _serviceCollection, _localRegistrar)

                    .AutoRegisterUsingServiceLifetime<AbstractClassA>(lifetime, _externalRegistrar)

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassG) });

                var instances1 = provider.GetRequiredService<IEnumerable<AbstractClassA>>();

                using (var scopedProvider = provider.CreateScope())
                {
                    var instances2 = scopedProvider.ServiceProvider.GetRequiredService<IEnumerable<AbstractClassA>>();

                    DependencyHelper.AssertInstanceEquality(instances1, instances2, expected);
                }
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Resolve_All_Interfaces(ServiceLifetime lifetime)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime<IBaseInterface1>(lifetime, _serviceCollection, _localRegistrar)

                    .AutoRegisterUsingServiceLifetime<IBaseInterface1>(lifetime, _externalRegistrar)

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<IBaseInterface1>(
                    provider,
                    new[] { typeof(ConcreteClassA), typeof(ConcreteClassB), typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassF), typeof(ConcreteClassG) });

                // ConcreteClassE implements IBaseInterface2 BUT it is not registered because it does not inherit IBaseInterface1
                DependencyHelper.AssertExpectation<IBaseInterface2>(
                    provider,
                    Enumerable.Empty<Type>());

                DependencyHelper.AssertExpectation<IBaseInterface5>(
                    provider,
                    new[] { typeof(ConcreteClassF) });

                DependencyHelper.AssertExpectation<IBaseInterface4>(
                    provider,
                    new[] { typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Resolve_All_Interfaces_Except_Registered_Service(ServiceLifetime lifetime)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime<IBaseInterface1>(lifetime, _serviceCollection, _localRegistrar,
                        config => config.Filter((service, implementation) => service != typeof(IBaseInterface1)))

                    .AutoRegisterUsingServiceLifetime<IBaseInterface1>(lifetime, _externalRegistrar,
                        config => config.Filter((service, implementation) => service != typeof(IBaseInterface1)))

                    .BuildServiceProvider();

                // IBaseInterface1 has been filtered out
                DependencyHelper.AssertExpectation<IBaseInterface1>(
                    provider,
                    Enumerable.Empty<Type>());

                DependencyHelper.AssertExpectation<IBaseInterface5>(
                    provider,
                    new[] { typeof(ConcreteClassF) });

                DependencyHelper.AssertExpectation<IBaseInterface4>(
                    provider,
                    new[] { typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Not_Resolve_Abstract_Class_When_Register_Interface(ServiceLifetime lifetime)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime<IBaseInterface1>(lifetime, _serviceCollection, _localRegistrar)

                    .AutoRegisterUsingServiceLifetime<IBaseInterface1>(lifetime, _externalRegistrar)

                    .BuildServiceProvider();

                // Not currently supporting the ability to do this - it could be extended to resolve ConcreteClassD, ConcreteClassE, ConcreteClassG
                // on the basis they all inherit AbstractClassA which inherits IBaseInterface1 but at this time you either register an abstract
                // class or an interface.
                DependencyHelper.AssertExpectation<AbstractClassA>(
                    provider,
                    Enumerable.Empty<Type>());
            }
        }

        public class AutoRegister_ServiceRegistrar_ServiceTypes : ServiceCollectionExtensionsFixture
        {
            private readonly LocalDependenciesRegistrar _localRegistrar = new();
            private readonly ExternalDependenciesRegistrar _externalRegistrar = new();

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Throw_When_Services_Null(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime(lifetime, null, _externalRegistrar, new[] { typeof(AbstractClassA), typeof(IBaseInterface2) });
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("serviceCollection");
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Throw_When_ServiceRegistrar_Null(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, (IServiceRegistrar)null, new[] { typeof(AbstractClassA), typeof(IBaseInterface2) });
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("serviceRegistrar");
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Throw_When_ServiceTypes_Null(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _externalRegistrar, (IEnumerable<Type>) null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("serviceTypes");
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Throw_When_ServiceTypes_Empty(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _externalRegistrar, new List<Type>());
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("serviceTypes");
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Not_Throw_When_Configure_Null(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _externalRegistrar,
                            new[] { typeof(AbstractClassA), typeof(IBaseInterface2) });
                    })
                    .Should()
                    .NotThrow();
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Register_No_Exclude_Or_Filter(ServiceLifetime lifetime)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _localRegistrar, new[] { typeof(AbstractClassA), typeof(IBaseInterface2) })

                    .AutoRegisterUsingServiceLifetime(lifetime, _externalRegistrar, new[] { typeof(AbstractClassA), typeof(IBaseInterface2) })

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassG) });
                DependencyHelper.AssertExpectation<IBaseInterface2>(provider, new[] { typeof(ConcreteClassA), typeof(ConcreteClassC), typeof(ConcreteClassE), typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Register_With_Filter(ServiceLifetime lifetime)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _localRegistrar, new[] { typeof(AbstractClassA), typeof(IBaseInterface2) })

                    .AutoRegisterUsingServiceLifetime(lifetime, _externalRegistrar, new[] { typeof(AbstractClassA), typeof(IBaseInterface2) }, config =>
                        config.Filter((service, implementation) => implementation != typeof(ConcreteClassE)))

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassG) });
                DependencyHelper.AssertExpectation<IBaseInterface2>(provider, new[] { typeof(ConcreteClassA), typeof(ConcreteClassC), typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton, true)]
            [InlineData(ServiceLifetime.Scoped, true)]
            [InlineData(ServiceLifetime.Transient, false)]
            public void Should_Resolve_When_In_Same_Scope(ServiceLifetime lifetime, bool expected)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _localRegistrar, new[] { typeof(AbstractClassA), typeof(IBaseInterface2) })

                    .AutoRegisterUsingServiceLifetime(lifetime, _externalRegistrar, new[] { typeof(AbstractClassA), typeof(IBaseInterface2) })

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassG) });
                DependencyHelper.AssertExpectation<IBaseInterface2>(provider, new[] { typeof(ConcreteClassA), typeof(ConcreteClassC), typeof(ConcreteClassE), typeof(ConcreteClassG) });

                var instances1a = provider.GetRequiredService<IEnumerable<AbstractClassA>>();
                var instances1b = provider.GetRequiredService<IEnumerable<AbstractClassA>>();

                DependencyHelper.AssertInstanceEquality(instances1a, instances1b, expected);

                var instances2a = provider.GetRequiredService<IEnumerable<IBaseInterface2>>();
                var instances2b = provider.GetRequiredService<IEnumerable<IBaseInterface2>>();

                DependencyHelper.AssertInstanceEquality(instances2a, instances2b, expected);
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton, true)]
            [InlineData(ServiceLifetime.Scoped, false)]
            [InlineData(ServiceLifetime.Transient, false)]
            public void Should_Resolve_When_In_Different_Scope(ServiceLifetime lifetime, bool expected)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _localRegistrar, new[] { typeof(AbstractClassA), typeof(IBaseInterface2) })

                    .AutoRegisterUsingServiceLifetime(lifetime, _externalRegistrar, new[] { typeof(AbstractClassA), typeof(IBaseInterface2) })

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassG) });
                DependencyHelper.AssertExpectation<IBaseInterface2>(provider, new[] { typeof(ConcreteClassA), typeof(ConcreteClassC), typeof(ConcreteClassE), typeof(ConcreteClassG) });

                var instances1a = provider.GetRequiredService<IEnumerable<AbstractClassA>>();
                var instances2a = provider.GetRequiredService<IEnumerable<IBaseInterface2>>();

                using (var scopedProvider = provider.CreateScope())
                {
                    var instances1b = scopedProvider.ServiceProvider.GetRequiredService<IEnumerable<AbstractClassA>>();
                    var instances2b = scopedProvider.ServiceProvider.GetRequiredService<IEnumerable<IBaseInterface2>>();

                    DependencyHelper.AssertInstanceEquality(instances1a, instances1b, expected);
                    DependencyHelper.AssertInstanceEquality(instances2a, instances2b, expected);
                }
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Resolve_All_Interfaces(ServiceLifetime lifetime)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _localRegistrar, new[] { typeof(IBaseInterface1), typeof(IBaseInterface2) })

                    .AutoRegisterUsingServiceLifetime(lifetime, _externalRegistrar, new[] { typeof(IBaseInterface1), typeof(IBaseInterface2) })

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<IBaseInterface1>(
                    provider,
                    new[] { typeof(ConcreteClassA), typeof(ConcreteClassB), typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassF), typeof(ConcreteClassG) });

                DependencyHelper.AssertExpectation<IBaseInterface2>(
                    provider,
                    new[] { typeof(ConcreteClassA), typeof(ConcreteClassC), typeof(ConcreteClassE), typeof(ConcreteClassG) });

                DependencyHelper.AssertExpectation<IBaseInterface5>(
                    provider,
                    new[] { typeof(ConcreteClassF) });

                DependencyHelper.AssertExpectation<IBaseInterface4>(
                    provider,
                    new[] { typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Resolve_All_Interfaces_Except_Registered_Service(ServiceLifetime lifetime)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _localRegistrar, new[] { typeof(IBaseInterface1), typeof(IBaseInterface2) },
                        config => config.Filter((service, implementation) => service != typeof(IBaseInterface1)))

                    .AutoRegisterUsingServiceLifetime(lifetime, _externalRegistrar, new[] { typeof(IBaseInterface1), typeof(IBaseInterface2) },
                        config => config.Filter((service, implementation) => service != typeof(IBaseInterface1)))

                    .BuildServiceProvider();

                // IBaseInterface1 has been filtered out
                DependencyHelper.AssertExpectation<IBaseInterface1>(
                    provider,
                    Enumerable.Empty<Type>());

                DependencyHelper.AssertExpectation<IBaseInterface2>(
                    provider,
                    new[] { typeof(ConcreteClassA), typeof(ConcreteClassC), typeof(ConcreteClassE), typeof(ConcreteClassG) });

                DependencyHelper.AssertExpectation<IBaseInterface5>(
                    provider,
                    new[] { typeof(ConcreteClassF) });

                DependencyHelper.AssertExpectation<IBaseInterface4>(
                    provider,
                    new[] { typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Not_Resolve_Abstract_Class_When_Register_Interface(ServiceLifetime lifetime)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _localRegistrar, new[] { typeof(IBaseInterface1), typeof(IBaseInterface2) })

                    .AutoRegisterUsingServiceLifetime(lifetime, _externalRegistrar, new[] { typeof(IBaseInterface1), typeof(IBaseInterface2) })

                    .BuildServiceProvider();

                // Not currently supporting the ability to do this - it could be extended to resolve ConcreteClassD, ConcreteClassE, ConcreteClassG
                // on the basis they all inherit AbstractClassA which inherits IBaseInterface1 but at this time you either register an abstract
                // class or an interface.
                DependencyHelper.AssertExpectation<AbstractClassA>(
                    provider,
                    Enumerable.Empty<Type>());
            }
        }

        public class AutoRegister_ServiceRegistrars_ServiceTypes : ServiceCollectionExtensionsFixture
        {
            private readonly IServiceRegistrar[] _registrars = new IServiceRegistrar[] { new LocalDependenciesRegistrar(), new ExternalDependenciesRegistrar()};

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Throw_When_Services_Null(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime(lifetime, null, _registrars, new[] { typeof(AbstractClassA), typeof(IBaseInterface2) });
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("serviceCollection");
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Throw_When_ServiceRegistrars_Null(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, (IServiceRegistrar[]) null, new[] { typeof(AbstractClassA), typeof(IBaseInterface2) });
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("serviceRegistrars");
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Throw_When_ServiceRegistrars_Empty(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, new List<IServiceRegistrar>(), new[] { typeof(AbstractClassA), typeof(IBaseInterface2) });
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("serviceRegistrars");
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Throw_When_ServiceTypes_Null(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _registrars, (IEnumerable<Type>) null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("serviceTypes");
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Throw_When_ServiceTypes_Empty(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _registrars, new List<Type>());
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("serviceTypes");
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Not_Throw_When_Configure_Null(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _registrars,
                            new[] { typeof(AbstractClassA), typeof(IBaseInterface2) });
                    })
                    .Should()
                    .NotThrow();
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Register_No_Exclude_Or_Filter(ServiceLifetime lifetime)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _registrars, new[] { typeof(AbstractClassA), typeof(IBaseInterface2) })
                    
                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassG) });
                DependencyHelper.AssertExpectation<IBaseInterface2>(provider, new[] { typeof(ConcreteClassA), typeof(ConcreteClassC), typeof(ConcreteClassE), typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Register_With_Filter(ServiceLifetime lifetime)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _registrars, new[] { typeof(AbstractClassA), typeof(IBaseInterface2) }, config =>
                        config.Filter((service, implementation) => implementation != typeof(ConcreteClassE)))

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassG) });
                DependencyHelper.AssertExpectation<IBaseInterface2>(provider, new[] { typeof(ConcreteClassA), typeof(ConcreteClassC), typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton, true)]
            [InlineData(ServiceLifetime.Scoped, true)]
            [InlineData(ServiceLifetime.Transient, false)]
            public void Should_Resolve_When_In_Same_Scope(ServiceLifetime lifetime, bool expected)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _registrars, new[] { typeof(AbstractClassA), typeof(IBaseInterface2) })

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassG) });
                DependencyHelper.AssertExpectation<IBaseInterface2>(provider, new[] { typeof(ConcreteClassA), typeof(ConcreteClassC), typeof(ConcreteClassE), typeof(ConcreteClassG) });

                var instances1a = provider.GetRequiredService<IEnumerable<AbstractClassA>>();
                var instances1b = provider.GetRequiredService<IEnumerable<AbstractClassA>>();

                DependencyHelper.AssertInstanceEquality(instances1a, instances1b, expected);

                var instances2a = provider.GetRequiredService<IEnumerable<IBaseInterface2>>();
                var instances2b = provider.GetRequiredService<IEnumerable<IBaseInterface2>>();

                DependencyHelper.AssertInstanceEquality(instances2a, instances2b, expected);
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton, true)]
            [InlineData(ServiceLifetime.Scoped, false)]
            [InlineData(ServiceLifetime.Transient, false)]
            public void Should_Resolve_When_In_Different_Scope(ServiceLifetime lifetime, bool expected)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _registrars, new[] { typeof(AbstractClassA), typeof(IBaseInterface2) })

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassG) });
                DependencyHelper.AssertExpectation<IBaseInterface2>(provider, new[] { typeof(ConcreteClassA), typeof(ConcreteClassC), typeof(ConcreteClassE), typeof(ConcreteClassG) });

                var instances1a = provider.GetRequiredService<IEnumerable<AbstractClassA>>();
                var instances2a = provider.GetRequiredService<IEnumerable<IBaseInterface2>>();

                using (var scopedProvider = provider.CreateScope())
                {
                    var instances1b = scopedProvider.ServiceProvider.GetRequiredService<IEnumerable<AbstractClassA>>();
                    var instances2b = scopedProvider.ServiceProvider.GetRequiredService<IEnumerable<IBaseInterface2>>();

                    DependencyHelper.AssertInstanceEquality(instances1a, instances1b, expected);
                    DependencyHelper.AssertInstanceEquality(instances2a, instances2b, expected);
                }
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Resolve_All_Interfaces(ServiceLifetime lifetime)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _registrars, new[] { typeof(IBaseInterface1), typeof(IBaseInterface2) })

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<IBaseInterface1>(
                    provider,
                    new[] { typeof(ConcreteClassA), typeof(ConcreteClassB), typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassF), typeof(ConcreteClassG) });

                DependencyHelper.AssertExpectation<IBaseInterface2>(
                    provider,
                    new[] { typeof(ConcreteClassA), typeof(ConcreteClassC), typeof(ConcreteClassE), typeof(ConcreteClassG) });

                DependencyHelper.AssertExpectation<IBaseInterface5>(
                    provider,
                    new[] { typeof(ConcreteClassF) });

                DependencyHelper.AssertExpectation<IBaseInterface4>(
                    provider,
                    new[] { typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Resolve_All_Interfaces_Except_Registered_Service(ServiceLifetime lifetime)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _registrars, new[] { typeof(IBaseInterface1), typeof(IBaseInterface2) },
                        config => config.Filter((service, implementation) => service != typeof(IBaseInterface1)))

                    .BuildServiceProvider();

                // IBaseInterface1 has been filtered out
                DependencyHelper.AssertExpectation<IBaseInterface1>(
                    provider,
                    Enumerable.Empty<Type>());

                DependencyHelper.AssertExpectation<IBaseInterface2>(
                    provider,
                    new[] { typeof(ConcreteClassA), typeof(ConcreteClassC), typeof(ConcreteClassE), typeof(ConcreteClassG) });

                DependencyHelper.AssertExpectation<IBaseInterface5>(
                    provider,
                    new[] { typeof(ConcreteClassF) });

                DependencyHelper.AssertExpectation<IBaseInterface4>(
                    provider,
                    new[] { typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Not_Resolve_Abstract_Class_When_Register_Interface(ServiceLifetime lifetime)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _registrars, new[] { typeof(IBaseInterface1), typeof(IBaseInterface2) })

                    .BuildServiceProvider();

                // Not currently supporting the ability to do this - it could be extended to resolve ConcreteClassD, ConcreteClassE, ConcreteClassG
                // on the basis they all inherit AbstractClassA which inherits IBaseInterface1 but at this time you either register an abstract
                // class or an interface.
                DependencyHelper.AssertExpectation<AbstractClassA>(
                    provider,
                    Enumerable.Empty<Type>());
            }
        }

        public class AutoRegister_TServiceRegistrar_ServiceTypes_ConstructorArgs : ServiceCollectionExtensionsFixture
        {
            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Throw_When_Services_Null(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime<ExternalDependenciesRegistrar>(lifetime, null, new[] { typeof(IBaseInterface3) },
                            (provider, serviceType) => new object[]{Create<int>()});
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("serviceCollection");
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Throw_When_ServiceTypes_Null(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime<ExternalDependenciesRegistrar>(lifetime, _serviceCollection,
                            (IEnumerable<Type>) null, (provider, serviceType) => new object[] {Create<int>()});
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("serviceTypes");
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Throw_When_ServiceTypes_Empty(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime<ExternalDependenciesRegistrar>(lifetime, _serviceCollection,
                            new List<Type>(), (provider, serviceType) => new object[] {Create<int>()});
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("serviceTypes");
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Throw_When_ConstructorArgs_Null(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime<ExternalDependenciesRegistrar>(lifetime, _serviceCollection, new[] { typeof(IBaseInterface3) }, null, null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("constructorArgsResolver");
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Not_Throw_When_Configure_Null(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime<ExternalDependenciesRegistrar>(lifetime, _serviceCollection,
                            new[] {typeof(IBaseInterface3)}, (provider, serviceType) => new object[] {Create<int>()}, null);
                    })
                    .Should()
                    .NotThrow();
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Register_No_Exclude_Or_Filter(ServiceLifetime lifetime)
            {
                var value = Create<int>();

                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime<ExternalDependenciesRegistrar>(lifetime, _serviceCollection, new[] { typeof(IBaseInterface3) },
                        (provider, serviceType) => new object[] { value })

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<IBaseInterface3>(provider, new[] { typeof(ConcreteClassH), typeof(ConcreteClassI) });

                var instances = provider.GetService<IEnumerable<IBaseInterface3>>().AsReadOnlyCollection();

                instances
                    .Single(instance => instance is ConcreteClassH)
                    .Value
                    .Should().Be(value);

                instances
                    .Single(instance => instance is ConcreteClassI)
                    .Value
                    .Should().Be(value * 100);
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Register_With_Filter(ServiceLifetime lifetime)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime<ExternalDependenciesRegistrar>(lifetime, _serviceCollection, new[] { typeof(IBaseInterface3) },
                        (provider, serviceType) => new object[] { Create<int>() },
                        config => config.Filter((service, implementation) => implementation != typeof(ConcreteClassH)))

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<IBaseInterface3>(provider, new[] { typeof(ConcreteClassI) });
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton, true)]
            [InlineData(ServiceLifetime.Scoped, true)]
            [InlineData(ServiceLifetime.Transient, false)]
            public void Should_Resolve_When_In_Same_Scope(ServiceLifetime lifetime, bool expected)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime<ExternalDependenciesRegistrar>(lifetime, _serviceCollection, new[] { typeof(IBaseInterface3) },
                        (provider, serviceType) => new object[] { Create<int>() })

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<IBaseInterface3>(provider, new[] { typeof(ConcreteClassH), typeof(ConcreteClassI) });

                var instances1a = provider.GetRequiredService<IEnumerable<IBaseInterface3>>();
                var instances1b = provider.GetRequiredService<IEnumerable<IBaseInterface3>>();

                DependencyHelper.AssertInstanceEquality(instances1a, instances1b, expected);
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton, true)]
            [InlineData(ServiceLifetime.Scoped, false)]
            [InlineData(ServiceLifetime.Transient, false)]
            public void Should_Resolve_When_In_Different_Scope(ServiceLifetime lifetime, bool expected)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime<ExternalDependenciesRegistrar>(lifetime, _serviceCollection, new[] { typeof(IBaseInterface3) },
                        (provider, serviceType) => new object[] { Create<int>() })

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<IBaseInterface3>(provider, new[] { typeof(ConcreteClassH), typeof(ConcreteClassI) });

                var instances1a = provider.GetRequiredService<IEnumerable<IBaseInterface3>>();

                using (var scopedProvider = provider.CreateScope())
                {
                    var instances1b = scopedProvider.ServiceProvider.GetRequiredService<IEnumerable<IBaseInterface3>>();

                    DependencyHelper.AssertInstanceEquality(instances1a, instances1b, expected);
                }
            }
        }

        public class AutoRegister_ServiceRegistrar_ServiceTypes_ConstructorArgs : ServiceCollectionExtensionsFixture
        {
            private readonly ExternalDependenciesRegistrar _registrar = new ();

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Throw_When_Services_Null(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime(lifetime, null, _registrar, new[] { typeof(IBaseInterface3) },
                            (provider, serviceType) => new object[] { Create<int>() });
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("serviceCollection");
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Throw_When_ServiceRegistrar_Null(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, (IServiceRegistrar)null, new[] { typeof(IBaseInterface3) },
                            (provider, serviceType) => new object[] { Create<int>() });
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("serviceRegistrar");
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Throw_When_ServiceTypes_Null(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _registrar, (IEnumerable<Type>) null,
                            (provider, serviceType) => new object[] { Create<int>() });
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("serviceTypes");
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Throw_When_ServiceTypes_Empty(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _registrar, new List<Type>(),
                            (provider, serviceType) => new object[] { Create<int>() });
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("serviceTypes");
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Throw_When_ConstructorArgs_Null(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _registrar, new[] { typeof(IBaseInterface3) }, null, null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("constructorArgsResolver");
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Not_Throw_When_Configure_Null(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _registrar, new[] { typeof(IBaseInterface3) },
                            (provider, serviceType) => new object[] { Create<int>() }, null);
                    })
                    .Should()
                    .NotThrow();
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Register_No_Exclude_Or_Filter(ServiceLifetime lifetime)
            {
                var value = Create<int>();

                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _registrar, new[] { typeof(IBaseInterface3) },
                        (provider, serviceType) => new object[] { value })

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<IBaseInterface3>(provider, new[] { typeof(ConcreteClassH), typeof(ConcreteClassI) });

                var instances = provider.GetService<IEnumerable<IBaseInterface3>>().AsReadOnlyCollection();

                instances
                    .Single(instance => instance is ConcreteClassH)
                    .Value
                    .Should().Be(value);

                instances
                    .Single(instance => instance is ConcreteClassI)
                    .Value
                    .Should().Be(value * 100);
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Register_With_Filter(ServiceLifetime lifetime)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _registrar, new[] { typeof(IBaseInterface3) },
                        (provider, serviceType) => new object[] { Create<int>() },
                        config => config.Filter((service, implementation) => implementation != typeof(ConcreteClassH)))

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<IBaseInterface3>(provider, new[] { typeof(ConcreteClassI) });
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton, true)]
            [InlineData(ServiceLifetime.Scoped, true)]
            [InlineData(ServiceLifetime.Transient, false)]
            public void Should_Resolve_When_In_Same_Scope(ServiceLifetime lifetime, bool expected)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _registrar, new[] { typeof(IBaseInterface3) },
                        (provider, serviceType) => new object[] { Create<int>() })

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<IBaseInterface3>(provider, new[] { typeof(ConcreteClassH), typeof(ConcreteClassI) });

                var instances1a = provider.GetRequiredService<IEnumerable<IBaseInterface3>>();
                var instances1b = provider.GetRequiredService<IEnumerable<IBaseInterface3>>();

                DependencyHelper.AssertInstanceEquality(instances1a, instances1b, expected);
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton, true)]
            [InlineData(ServiceLifetime.Scoped, false)]
            [InlineData(ServiceLifetime.Transient, false)]
            public void Should_Resolve_When_In_Different_Scope(ServiceLifetime lifetime, bool expected)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _registrar, new[] { typeof(IBaseInterface3) },
                        (provider, serviceType) => new object[] { Create<int>() })

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<IBaseInterface3>(provider, new[] { typeof(ConcreteClassH), typeof(ConcreteClassI) });

                var instances1a = provider.GetRequiredService<IEnumerable<IBaseInterface3>>();

                using (var scopedProvider = provider.CreateScope())
                {
                    var instances1b = scopedProvider.ServiceProvider.GetRequiredService<IEnumerable<IBaseInterface3>>();

                    DependencyHelper.AssertInstanceEquality(instances1a, instances1b, expected);
                }
            }
        }
        
        public class AutoRegister_ServiceRegistrars_ServiceTypes_ConstructorArgs : ServiceCollectionExtensionsFixture
        {
            private readonly IServiceRegistrar[] _registrars = new[] {(IServiceRegistrar) new LocalDependenciesRegistrar(), new ExternalDependenciesRegistrar()};

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Throw_When_Services_Null(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime(lifetime, null, _registrars, new[] { typeof(IBaseInterface3) },
                            (provider, serviceType) => new object[] { Create<int>() });
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("serviceCollection");
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Throw_When_ServiceRegistrars_Null(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, (IServiceRegistrar[]) null, new[] { typeof(IBaseInterface3) },
                            (provider, serviceType) => new object[] { Create<int>() });
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("serviceRegistrars");
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Throw_When_ServiceRegistrars_Empty(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, new List<IServiceRegistrar>(), new[] { typeof(IBaseInterface3) },
                            (provider, serviceType) => new object[] { Create<int>() });
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("serviceRegistrars");
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Throw_When_ServiceTypes_Null(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _registrars, (IEnumerable<Type>) null,
                            (provider, serviceType) => new object[] { Create<int>() });
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("serviceTypes");
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Throw_When_ServiceTypes_Empty(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _registrars, new List<Type>(),
                            (provider, serviceType) => new object[] { Create<int>() });
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("serviceTypes");
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Throw_When_ConstructorArgs_Null(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _registrars, new[] { typeof(IBaseInterface3) }, null, null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("constructorArgsResolver");
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Not_Throw_When_Configure_Null(ServiceLifetime lifetime)
            {
                Invoking(() =>
                    {
                        DependencyHelper.AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _registrars, new[] { typeof(IBaseInterface3) },
                            (provider, serviceType) => new object[] { Create<int>() }, null);
                    })
                    .Should()
                    .NotThrow();
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Register_No_Exclude_Or_Filter(ServiceLifetime lifetime)
            {
                var value = Create<int>();

                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _registrars, new[] { typeof(IBaseInterface3) },
                        (provider, serviceType) => new object[] { value })

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<IBaseInterface3>(provider, new[] { typeof(ConcreteClassH), typeof(ConcreteClassI), typeof(ConcreteClassJ) });

                var instances = provider.GetService<IEnumerable<IBaseInterface3>>().AsReadOnlyCollection();

                instances
                    .Single(instance => instance is ConcreteClassH)
                    .Value
                    .Should().Be(value);

                instances
                    .Single(instance => instance is ConcreteClassI)
                    .Value
                    .Should().Be(value * 100);

                instances
                    .Single(instance => instance is ConcreteClassJ)
                    .Value
                    .Should().Be(value - 100);
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Register_With_Filter(ServiceLifetime lifetime)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _registrars, new[] { typeof(IBaseInterface3) },
                        (provider, serviceType) => new object[] { Create<int>() },
                        config => config.Filter((service, implementation) => implementation != typeof(ConcreteClassH)))

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<IBaseInterface3>(provider, new[] { typeof(ConcreteClassI), typeof(ConcreteClassJ) });
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton, true)]
            [InlineData(ServiceLifetime.Scoped, true)]
            [InlineData(ServiceLifetime.Transient, false)]
            public void Should_Resolve_When_In_Same_Scope(ServiceLifetime lifetime, bool expected)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _registrars, new[] { typeof(IBaseInterface3) },
                        (provider, serviceType) => new object[] { Create<int>() })

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<IBaseInterface3>(provider, new[] { typeof(ConcreteClassH), typeof(ConcreteClassI), typeof(ConcreteClassJ) });

                var instances1a = provider.GetRequiredService<IEnumerable<IBaseInterface3>>();
                var instances1b = provider.GetRequiredService<IEnumerable<IBaseInterface3>>();

                DependencyHelper.AssertInstanceEquality(instances1a, instances1b, expected);
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton, true)]
            [InlineData(ServiceLifetime.Scoped, false)]
            [InlineData(ServiceLifetime.Transient, false)]
            public void Should_Resolve_When_In_Different_Scope(ServiceLifetime lifetime, bool expected)
            {
                var provider = DependencyHelper

                    .AutoRegisterUsingServiceLifetime(lifetime, _serviceCollection, _registrars, new[] { typeof(IBaseInterface3) },
                        (provider, serviceType) => new object[] { Create<int>() })

                    .BuildServiceProvider();

                DependencyHelper.AssertExpectation<IBaseInterface3>(provider, new[] { typeof(ConcreteClassH), typeof(ConcreteClassI), typeof(ConcreteClassJ) });

                var instances1a = provider.GetRequiredService<IEnumerable<IBaseInterface3>>();

                using (var scopedProvider = provider.CreateScope())
                {
                    var instances1b = scopedProvider.ServiceProvider.GetRequiredService<IEnumerable<IBaseInterface3>>();

                    DependencyHelper.AssertInstanceEquality(instances1a, instances1b, expected);
                }
            }
        }
    }
}