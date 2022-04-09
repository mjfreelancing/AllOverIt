﻿using AllOverIt.DependencyInjection.Extensions;
using AllOverIt.DependencyInjection.Tests.Types;
using AllOverIt.Fixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using AllOverIt.Extensions;
using AllOverIt.Fixture.Extensions;
using Xunit;

namespace AllOverIt.DependencyInjection.Tests.Extensions
{
    public enum RegistrationMode
    {
        Singleton,
        Scoped,
        Transient
    }

    internal static class FixtureHelper
    {
        public static IServiceCollection AutoRegisterUsingMode<TServiceRegistrar, TServiceType>(RegistrationMode mode, IServiceCollection services,
            Action<IServiceRegistrarOptions> configure = default)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            return mode switch
            {
                RegistrationMode.Singleton => ServiceCollectionExtensions.AutoRegisterSingleton<TServiceRegistrar, TServiceType>(services, configure),
                
                RegistrationMode.Scoped => ServiceCollectionExtensions.AutoRegisterScoped<TServiceRegistrar, TServiceType>(services, configure),

                RegistrationMode.Transient => ServiceCollectionExtensions.AutoRegisterTransient<TServiceRegistrar, TServiceType>(services, configure),

                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }

        public static IServiceCollection AutoRegisterUsingMode<TServiceRegistrar, TServiceType>(this IServiceCollection services, RegistrationMode mode,
            Action<IServiceRegistrarOptions> configure = default)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            return AutoRegisterUsingMode<TServiceRegistrar, TServiceType>(mode, services, configure);
        }

        public static IServiceCollection AutoRegisterUsingMode<TServiceRegistrar>(RegistrationMode mode, IServiceCollection services,
            IEnumerable<Type> serviceTypes, Action<IServiceRegistrarOptions> configure = default)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            return mode switch
            {
                RegistrationMode.Singleton => ServiceCollectionExtensions.AutoRegisterSingleton<TServiceRegistrar>(services, serviceTypes, configure),

                RegistrationMode.Scoped => ServiceCollectionExtensions.AutoRegisterScoped<TServiceRegistrar>(services, serviceTypes, configure),

                RegistrationMode.Transient => ServiceCollectionExtensions.AutoRegisterTransient<TServiceRegistrar>(services, serviceTypes, configure),

                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }

        public static IServiceCollection AutoRegisterUsingMode<TServiceRegistrar>(this IServiceCollection services, RegistrationMode mode,
            IEnumerable<Type> serviceTypes, Action<IServiceRegistrarOptions> configure = default)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            return AutoRegisterUsingMode<TServiceRegistrar>(mode, services, serviceTypes, configure);
        }

        public static IServiceCollection AutoRegisterUsingMode<TServiceType>(RegistrationMode mode, IServiceCollection services,
            IServiceRegistrar serviceRegistrar, Action<IServiceRegistrarOptions> configure = default)
        {
            return mode switch
            {
                RegistrationMode.Singleton => ServiceCollectionExtensions.AutoRegisterSingleton<TServiceType>(services, serviceRegistrar, configure),

                RegistrationMode.Scoped => ServiceCollectionExtensions.AutoRegisterScoped<TServiceType>(services, serviceRegistrar, configure),

                RegistrationMode.Transient => ServiceCollectionExtensions.AutoRegisterTransient<TServiceType>(services, serviceRegistrar, configure),

                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }

        public static IServiceCollection AutoRegisterUsingMode<TServiceType>(this IServiceCollection services, RegistrationMode mode,
            IServiceRegistrar serviceRegistrar, Action<IServiceRegistrarOptions> configure = default)
        {
            return AutoRegisterUsingMode<TServiceType>(mode, services, serviceRegistrar, configure);
        }
    }
    
    public class ServiceCollectionExtensionsFixture : FixtureBase
    {
        private readonly IServiceCollection _serviceCollection;

        protected ServiceCollectionExtensionsFixture()
        {
            _serviceCollection = new ServiceCollection();
        }

        public class AutoRegister_TServiceRegistrar_TServiceType : ServiceCollectionExtensionsFixture
        {
            [Theory]
            [InlineData(RegistrationMode.Singleton)]
            [InlineData(RegistrationMode.Scoped)]
            [InlineData(RegistrationMode.Transient)]
            public void Should_Throw_When_Services_Null(RegistrationMode mode)
            {
                Invoking(() =>
                    {
                        FixtureHelper.AutoRegisterUsingMode<ExternalDependenciesRegistrar, AbstractClassA>(mode, null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("services");
            }

            [Theory]
            [InlineData(RegistrationMode.Singleton)]
            [InlineData(RegistrationMode.Scoped)]
            [InlineData(RegistrationMode.Transient)]
            public void Should_Not_Throw_When_Configure_Null(RegistrationMode mode)
            {
                Invoking(() =>
                    {
                        FixtureHelper.AutoRegisterUsingMode<ExternalDependenciesRegistrar, AbstractClassA>(mode, _serviceCollection, null);
                    })
                    .Should()
                    .NotThrow();
            }

            [Theory]
            [InlineData(RegistrationMode.Singleton)]
            [InlineData(RegistrationMode.Scoped)]
            [InlineData(RegistrationMode.Transient)]
            public void Should_Register_No_Exclude_Or_Filter(RegistrationMode mode)
            {
                var provider = FixtureHelper

                    .AutoRegisterUsingMode<LocalDependenciesRegistrar, AbstractClassA>(mode, _serviceCollection)

                    .AutoRegisterUsingMode<ExternalDependenciesRegistrar, AbstractClassA>(mode)

                    .BuildServiceProvider();

                AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(RegistrationMode.Singleton)]
            [InlineData(RegistrationMode.Scoped)]
            [InlineData(RegistrationMode.Transient)]
            public void Should_Register_With_Exclude(RegistrationMode mode)
            {
                var provider = FixtureHelper
                    
                    .AutoRegisterUsingMode<LocalDependenciesRegistrar, AbstractClassA>(mode, _serviceCollection)

                    .AutoRegisterUsingMode<ExternalDependenciesRegistrar, AbstractClassA>(mode, config => config.ExcludeTypes(typeof(ConcreteClassE)))

                    .BuildServiceProvider();

                AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(RegistrationMode.Singleton)]
            [InlineData(RegistrationMode.Scoped)]
            [InlineData(RegistrationMode.Transient)]
            public void Should_Register_With_Filter(RegistrationMode mode)
            {
                var provider = FixtureHelper

                    .AutoRegisterUsingMode<LocalDependenciesRegistrar, AbstractClassA>(mode, _serviceCollection)

                    .AutoRegisterUsingMode<ExternalDependenciesRegistrar, AbstractClassA>(mode, config =>
                        config.Filter((service, implementation) => implementation != typeof(ConcreteClassE)))

                    .BuildServiceProvider();

                AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(RegistrationMode.Singleton, true)]
            [InlineData(RegistrationMode.Scoped, true)]
            [InlineData(RegistrationMode.Transient, false)]
            public void Should_Resolve_When_In_Same_Scope(RegistrationMode mode, bool expected)
            {
                var provider = FixtureHelper

                    .AutoRegisterUsingMode<LocalDependenciesRegistrar, AbstractClassA>(mode, _serviceCollection)

                    .AutoRegisterUsingMode<ExternalDependenciesRegistrar, AbstractClassA>(mode)

                    .BuildServiceProvider();

                AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassG) });

                var instances1 = provider.GetRequiredService<IEnumerable<AbstractClassA>>();
                var instances2 = provider.GetRequiredService<IEnumerable<AbstractClassA>>();

                AssertInstanceEquality(instances1, instances2, expected);
            }

            [Theory]
            [InlineData(RegistrationMode.Singleton, true)]
            [InlineData(RegistrationMode.Scoped, false)]
            [InlineData(RegistrationMode.Transient, false)]
            public void Should_Resolve_When_In_Different_Scope(RegistrationMode mode, bool expected)
            {
                var provider = FixtureHelper

                    .AutoRegisterUsingMode<LocalDependenciesRegistrar, AbstractClassA>(mode, _serviceCollection)

                    .AutoRegisterUsingMode<ExternalDependenciesRegistrar, AbstractClassA>(mode)

                    .BuildServiceProvider();

                AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassG) });

                var instances1 = provider.GetRequiredService<IEnumerable<AbstractClassA>>();

                using (var scopedProvider = provider.CreateScope())
                {
                    var instances2 = scopedProvider.ServiceProvider.GetRequiredService<IEnumerable<AbstractClassA>>();

                    AssertInstanceEquality(instances1, instances2, expected);
                }
            }

            [Theory]
            [InlineData(RegistrationMode.Singleton)]
            [InlineData(RegistrationMode.Scoped)]
            [InlineData(RegistrationMode.Transient)]
            public void Should_Resolve_All_Interfaces(RegistrationMode mode)
            {
                var provider = FixtureHelper

                    .AutoRegisterUsingMode<LocalDependenciesRegistrar, IBaseInterface1>(mode, _serviceCollection)

                    .AutoRegisterUsingMode<ExternalDependenciesRegistrar, IBaseInterface1>(mode)

                    .BuildServiceProvider();

                AssertExpectation<IBaseInterface1>(
                    provider,
                    new[] { typeof(ConcreteClassA), typeof(ConcreteClassB), typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassF), typeof(ConcreteClassG) });

                // ConcreteClassE implements IBaseInterface2 BUT it is not registered because it does not inherit IBaseInterface1
                AssertExpectation<IBaseInterface2>(
                    provider,
                    Enumerable.Empty<Type>());

                AssertExpectation<IInterface3>(
                    provider,
                    new[] { typeof(ConcreteClassF) });

                AssertExpectation<IInterface4>(
                    provider,
                    new[] { typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(RegistrationMode.Singleton)]
            [InlineData(RegistrationMode.Scoped)]
            [InlineData(RegistrationMode.Transient)]
            public void Should_Resolve_All_Interfaces_Except_Registered_Service(RegistrationMode mode)
            {
                var provider = FixtureHelper

                    .AutoRegisterUsingMode<LocalDependenciesRegistrar, IBaseInterface1>(mode, _serviceCollection,
                        config => config.Filter((service, implementation) => service != typeof(IBaseInterface1)))

                    .AutoRegisterUsingMode<ExternalDependenciesRegistrar, IBaseInterface1>(mode,
                        config => config.Filter((service, implementation) => service != typeof(IBaseInterface1)))

                    .BuildServiceProvider();

                // IBaseInterface1 has been filtered out
                AssertExpectation<IBaseInterface1>(
                    provider,
                    Enumerable.Empty<Type>());

                AssertExpectation<IInterface3>(
                    provider,
                    new[] { typeof(ConcreteClassF) });

                AssertExpectation<IInterface4>(
                    provider,
                    new[] { typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(RegistrationMode.Singleton)]
            [InlineData(RegistrationMode.Scoped)]
            [InlineData(RegistrationMode.Transient)]
            public void Should_Not_Resolve_Abstract_Class_When_Register_Interface(RegistrationMode mode)
            {
                var provider = FixtureHelper

                    .AutoRegisterUsingMode<LocalDependenciesRegistrar, IBaseInterface1>(mode, _serviceCollection)

                    .AutoRegisterUsingMode<ExternalDependenciesRegistrar, IBaseInterface1>(mode)

                    .BuildServiceProvider();

                // Not currently supporting the ability to do this - it could be extended to resolve ConcreteClassD, ConcreteClassE, ConcreteClassG
                // on the basis they all inherit AbstractClassA which inherits IBaseInterface1 but at this time you either register an abstract
                // class or an interface.
                AssertExpectation<AbstractClassA>(
                    provider,
                    Enumerable.Empty<Type>());
            }
        }

        public class AutoRegister_TServiceRegistrar_ServiceTypes : ServiceCollectionExtensionsFixture
        {
            [Theory]
            [InlineData(RegistrationMode.Singleton)]
            [InlineData(RegistrationMode.Scoped)]
            [InlineData(RegistrationMode.Transient)]
            public void Should_Throw_When_Services_Null(RegistrationMode mode)
            {
                Invoking(() =>
                    {
                        FixtureHelper.AutoRegisterUsingMode<ExternalDependenciesRegistrar>(mode, null, new[] {typeof(AbstractClassA), typeof(IBaseInterface2)});
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("services");
            }

            [Theory]
            [InlineData(RegistrationMode.Singleton)]
            [InlineData(RegistrationMode.Scoped)]
            [InlineData(RegistrationMode.Transient)]
            public void Should_Throw_When_ServiceTypes_Null(RegistrationMode mode)
            {
                Invoking(() =>
                    {
                        FixtureHelper.AutoRegisterUsingMode<ExternalDependenciesRegistrar>(mode, _serviceCollection, (IEnumerable<Type>)null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("serviceTypes");
            }

            [Theory]
            [InlineData(RegistrationMode.Singleton)]
            [InlineData(RegistrationMode.Scoped)]
            [InlineData(RegistrationMode.Transient)]
            public void Should_Throw_When_ServiceTypes_Empty(RegistrationMode mode)
            {
                Invoking(() =>
                    {
                        FixtureHelper.AutoRegisterUsingMode<ExternalDependenciesRegistrar>(mode, _serviceCollection, new List<Type>());
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("serviceTypes");
            }

            [Theory]
            [InlineData(RegistrationMode.Singleton)]
            [InlineData(RegistrationMode.Scoped)]
            [InlineData(RegistrationMode.Transient)]
            public void Should_Not_Throw_When_Configure_Null(RegistrationMode mode)
            {
                Invoking(() =>
                    {
                        FixtureHelper.AutoRegisterUsingMode<ExternalDependenciesRegistrar>(mode, _serviceCollection,
                            new[] {typeof(AbstractClassA), typeof(IBaseInterface2)});
                    })
                    .Should()
                    .NotThrow();
            }

            [Theory]
            [InlineData(RegistrationMode.Singleton)]
            [InlineData(RegistrationMode.Scoped)]
            [InlineData(RegistrationMode.Transient)]
            public void Should_Register_No_Exclude_Or_Filter(RegistrationMode mode)
            {
                var provider = FixtureHelper
                    
                    .AutoRegisterUsingMode<LocalDependenciesRegistrar>(mode, _serviceCollection, new[] {typeof(AbstractClassA), typeof(IBaseInterface2) })

                    .AutoRegisterUsingMode<ExternalDependenciesRegistrar>(mode, new[] {typeof(AbstractClassA), typeof(IBaseInterface2) })

                    .BuildServiceProvider();

                AssertExpectation<AbstractClassA>(provider, new[] {typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassG)});
                AssertExpectation<IBaseInterface2>(provider, new[] { typeof(ConcreteClassA), typeof(ConcreteClassC), typeof(ConcreteClassE), typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(RegistrationMode.Singleton)]
            [InlineData(RegistrationMode.Scoped)]
            [InlineData(RegistrationMode.Transient)]
            public void Should_Register_With_Exclude(RegistrationMode mode)
            {
                var provider = FixtureHelper

                    .AutoRegisterUsingMode<LocalDependenciesRegistrar>(mode, _serviceCollection, new[] { typeof(AbstractClassA), typeof(IBaseInterface2) })

                    .AutoRegisterUsingMode<ExternalDependenciesRegistrar>(mode, new[] { typeof(AbstractClassA), typeof(IBaseInterface2) },
                        config => config.ExcludeTypes(typeof(ConcreteClassE)))

                    .BuildServiceProvider();

                AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassG) });
                AssertExpectation<IBaseInterface2>(provider, new[] { typeof(ConcreteClassA), typeof(ConcreteClassC), typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(RegistrationMode.Singleton)]
            [InlineData(RegistrationMode.Scoped)]
            [InlineData(RegistrationMode.Transient)]
            public void Should_Register_With_Filter(RegistrationMode mode)
            {
                var provider = FixtureHelper

                    .AutoRegisterUsingMode<LocalDependenciesRegistrar>(mode, _serviceCollection, new[] { typeof(AbstractClassA), typeof(IBaseInterface2) })

                    .AutoRegisterUsingMode<ExternalDependenciesRegistrar>(mode, new[] { typeof(AbstractClassA), typeof(IBaseInterface2) }, config =>
                        config.Filter((service, implementation) => implementation != typeof(ConcreteClassE)))

                    .BuildServiceProvider();

                AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassG) });
                AssertExpectation<IBaseInterface2>(provider, new[] { typeof(ConcreteClassA), typeof(ConcreteClassC), typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(RegistrationMode.Singleton, true)]
            [InlineData(RegistrationMode.Scoped, true)]
            [InlineData(RegistrationMode.Transient, false)]
            public void Should_Resolve_When_In_Same_Scope(RegistrationMode mode, bool expected)
            {
                var provider = FixtureHelper

                    .AutoRegisterUsingMode<LocalDependenciesRegistrar>(mode, _serviceCollection, new[] { typeof(AbstractClassA), typeof(IBaseInterface2) })

                    .AutoRegisterUsingMode<ExternalDependenciesRegistrar>(mode, new[] { typeof(AbstractClassA), typeof(IBaseInterface2) })

                    .BuildServiceProvider();

                AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassG) });
                AssertExpectation<IBaseInterface2>(provider, new[] { typeof(ConcreteClassA), typeof(ConcreteClassC), typeof(ConcreteClassE), typeof(ConcreteClassG) });

                var instances1a = provider.GetRequiredService<IEnumerable<AbstractClassA>>();
                var instances1b = provider.GetRequiredService<IEnumerable<AbstractClassA>>();

                AssertInstanceEquality(instances1a, instances1b, expected);

                var instances2a = provider.GetRequiredService<IEnumerable<IBaseInterface2>>();
                var instances2b = provider.GetRequiredService<IEnumerable<IBaseInterface2>>();

                AssertInstanceEquality(instances2a, instances2b, expected);
            }

            [Theory]
            [InlineData(RegistrationMode.Singleton, true)]
            [InlineData(RegistrationMode.Scoped, false)]
            [InlineData(RegistrationMode.Transient, false)]
            public void Should_Resolve_When_In_Different_Scope(RegistrationMode mode, bool expected)
            {
                var provider = FixtureHelper

                    .AutoRegisterUsingMode<LocalDependenciesRegistrar>(mode, _serviceCollection, new[] { typeof(AbstractClassA), typeof(IBaseInterface2) })

                    .AutoRegisterUsingMode<ExternalDependenciesRegistrar>(mode, new[] { typeof(AbstractClassA), typeof(IBaseInterface2) })

                    .BuildServiceProvider();

                AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassG) });
                AssertExpectation<IBaseInterface2>(provider, new[] { typeof(ConcreteClassA), typeof(ConcreteClassC), typeof(ConcreteClassE), typeof(ConcreteClassG) });

                var instances1a = provider.GetRequiredService<IEnumerable<AbstractClassA>>();
                var instances2a = provider.GetRequiredService<IEnumerable<IBaseInterface2>>();

                using (var scopedProvider = provider.CreateScope())
                {
                    var instances1b = scopedProvider.ServiceProvider.GetRequiredService<IEnumerable<AbstractClassA>>();
                    var instances2b = scopedProvider.ServiceProvider.GetRequiredService<IEnumerable<IBaseInterface2>>();

                    AssertInstanceEquality(instances1a, instances1b, expected);
                    AssertInstanceEquality(instances2a, instances2b, expected);
                }
            }

            [Theory]
            [InlineData(RegistrationMode.Singleton)]
            [InlineData(RegistrationMode.Scoped)]
            [InlineData(RegistrationMode.Transient)]
            public void Should_Resolve_All_Interfaces(RegistrationMode mode)
            {
                var provider = FixtureHelper

                    .AutoRegisterUsingMode<LocalDependenciesRegistrar>(mode, _serviceCollection, new[] { typeof(IBaseInterface1), typeof(IBaseInterface2) })

                    .AutoRegisterUsingMode<ExternalDependenciesRegistrar>(mode, new[] { typeof(IBaseInterface1), typeof(IBaseInterface2) })

                    .BuildServiceProvider();

                AssertExpectation<IBaseInterface1>(
                    provider,
                    new[] { typeof(ConcreteClassA), typeof(ConcreteClassB), typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassF), typeof(ConcreteClassG) });

                AssertExpectation<IBaseInterface2>(
                    provider,
                    new[] { typeof(ConcreteClassA), typeof(ConcreteClassC), typeof(ConcreteClassE), typeof(ConcreteClassG) });

                AssertExpectation<IInterface3>(
                    provider,
                    new[] { typeof(ConcreteClassF) });

                AssertExpectation<IInterface4>(
                    provider,
                    new[] { typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(RegistrationMode.Singleton)]
            [InlineData(RegistrationMode.Scoped)]
            [InlineData(RegistrationMode.Transient)]
            public void Should_Resolve_All_Interfaces_Except_Registered_Service(RegistrationMode mode)
            {
                var provider = FixtureHelper

                    .AutoRegisterUsingMode<LocalDependenciesRegistrar>(mode, _serviceCollection, new[] { typeof(IBaseInterface1), typeof(IBaseInterface2) },
                        config => config.Filter((service, implementation) => service != typeof(IBaseInterface1)))

                    .AutoRegisterUsingMode<ExternalDependenciesRegistrar>(mode, new[] { typeof(IBaseInterface1), typeof(IBaseInterface2) },
                        config => config.Filter((service, implementation) => service != typeof(IBaseInterface1)))

                    .BuildServiceProvider();

                // IBaseInterface1 has been filtered out
                AssertExpectation<IBaseInterface1>(
                    provider,
                    Enumerable.Empty<Type>());

                AssertExpectation<IBaseInterface2>(
                    provider,
                    new[] { typeof(ConcreteClassA), typeof(ConcreteClassC), typeof(ConcreteClassE), typeof(ConcreteClassG) });

                AssertExpectation<IInterface3>(
                    provider,
                    new[] { typeof(ConcreteClassF) });

                AssertExpectation<IInterface4>(
                    provider,
                    new[] { typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(RegistrationMode.Singleton)]
            [InlineData(RegistrationMode.Scoped)]
            [InlineData(RegistrationMode.Transient)]
            public void Should_Not_Resolve_Abstract_Class_When_Register_Interface(RegistrationMode mode)
            {
                var provider = FixtureHelper

                    .AutoRegisterUsingMode<LocalDependenciesRegistrar>(mode, _serviceCollection, new[] { typeof(IBaseInterface1), typeof(IBaseInterface2) })

                    .AutoRegisterUsingMode<ExternalDependenciesRegistrar>(mode, new[] { typeof(IBaseInterface1), typeof(IBaseInterface2) })

                    .BuildServiceProvider();

                // Not currently supporting the ability to do this - it could be extended to resolve ConcreteClassD, ConcreteClassE, ConcreteClassG
                // on the basis they all inherit AbstractClassA which inherits IBaseInterface1 but at this time you either register an abstract
                // class or an interface.
                AssertExpectation<AbstractClassA>(
                    provider,
                    Enumerable.Empty<Type>());
            }
        }

        public class AutoRegister_TServiceType_ServiceRegistrar_Instance : ServiceCollectionExtensionsFixture
        {
            private readonly LocalDependenciesRegistrar _localRegistrar = new();
            private readonly ExternalDependenciesRegistrar _externalRegistrar = new();

            [Theory]
            [InlineData(RegistrationMode.Singleton)]
            [InlineData(RegistrationMode.Scoped)]
            [InlineData(RegistrationMode.Transient)]
            public void Should_Throw_When_Services_Null(RegistrationMode mode)
            {
                Invoking(() =>
                {
                    FixtureHelper.AutoRegisterUsingMode<AbstractClassA>(mode, null, _localRegistrar);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("services");
            }

            [Theory]
            [InlineData(RegistrationMode.Singleton)]
            [InlineData(RegistrationMode.Scoped)]
            [InlineData(RegistrationMode.Transient)]
            public void Should_Throw_When_ServiceRegistrar_Null(RegistrationMode mode)
            {
                Invoking(() =>
                    {
                        FixtureHelper.AutoRegisterUsingMode<AbstractClassA>(mode, _serviceCollection, null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("serviceRegistrar");
            }

            [Theory]
            [InlineData(RegistrationMode.Singleton)]
            [InlineData(RegistrationMode.Scoped)]
            [InlineData(RegistrationMode.Transient)]
            public void Should_Not_Throw_When_Configure_Null(RegistrationMode mode)
            {
                Invoking(() =>
                    {
                        FixtureHelper.AutoRegisterUsingMode<AbstractClassA>(mode, _serviceCollection, _localRegistrar, null);
                    })
                    .Should()
                    .NotThrow();
            }

            [Theory]
            [InlineData(RegistrationMode.Singleton)]
            [InlineData(RegistrationMode.Scoped)]
            [InlineData(RegistrationMode.Transient)]
            public void Should_Register_No_Exclude_Or_Filter(RegistrationMode mode)
            {
                var provider = FixtureHelper

                    .AutoRegisterUsingMode<AbstractClassA>(mode, _serviceCollection, _localRegistrar)

                    .AutoRegisterUsingMode<AbstractClassA>(mode, _externalRegistrar)

                    .BuildServiceProvider();

                AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(RegistrationMode.Singleton)]
            [InlineData(RegistrationMode.Scoped)]
            [InlineData(RegistrationMode.Transient)]
            public void Should_Register_With_Exclude(RegistrationMode mode)
            {
                var provider = FixtureHelper

                    .AutoRegisterUsingMode<AbstractClassA>(mode, _serviceCollection, _localRegistrar)

                    .AutoRegisterUsingMode<AbstractClassA>(mode, _externalRegistrar, config => config.ExcludeTypes(typeof(ConcreteClassE)))

                    .BuildServiceProvider();

                AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(RegistrationMode.Singleton)]
            [InlineData(RegistrationMode.Scoped)]
            [InlineData(RegistrationMode.Transient)]
            public void Should_Register_With_Filter(RegistrationMode mode)
            {
                var provider = FixtureHelper

                    .AutoRegisterUsingMode<AbstractClassA>(mode, _serviceCollection, _localRegistrar)

                    .AutoRegisterUsingMode<AbstractClassA>(mode, _externalRegistrar, config =>
                        config.Filter((service, implementation) => implementation != typeof(ConcreteClassE)))

                    .BuildServiceProvider();

                AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(RegistrationMode.Singleton, true)]
            [InlineData(RegistrationMode.Scoped, true)]
            [InlineData(RegistrationMode.Transient, false)]
            public void Should_Resolve_When_In_Same_Scope(RegistrationMode mode, bool expected)
            {
                var provider = FixtureHelper

                    .AutoRegisterUsingMode<AbstractClassA>(mode, _serviceCollection, _localRegistrar)

                    .AutoRegisterUsingMode<AbstractClassA>(mode, _externalRegistrar)

                    .BuildServiceProvider();

                AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassG) });

                var instances1 = provider.GetRequiredService<IEnumerable<AbstractClassA>>();
                var instances2 = provider.GetRequiredService<IEnumerable<AbstractClassA>>();

                AssertInstanceEquality(instances1, instances2, expected);
            }

            [Theory]
            [InlineData(RegistrationMode.Singleton, true)]
            [InlineData(RegistrationMode.Scoped, false)]
            [InlineData(RegistrationMode.Transient, false)]
            public void Should_Resolve_When_In_Different_Scope(RegistrationMode mode, bool expected)
            {
                var provider = FixtureHelper

                    .AutoRegisterUsingMode<AbstractClassA>(mode, _serviceCollection, _localRegistrar)

                    .AutoRegisterUsingMode<AbstractClassA>(mode, _externalRegistrar)

                    .BuildServiceProvider();

                AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassG) });

                var instances1 = provider.GetRequiredService<IEnumerable<AbstractClassA>>();

                using (var scopedProvider = provider.CreateScope())
                {
                    var instances2 = scopedProvider.ServiceProvider.GetRequiredService<IEnumerable<AbstractClassA>>();

                    AssertInstanceEquality(instances1, instances2, expected);
                }
            }

            [Theory]
            [InlineData(RegistrationMode.Singleton)]
            [InlineData(RegistrationMode.Scoped)]
            [InlineData(RegistrationMode.Transient)]
            public void Should_Resolve_All_Interfaces(RegistrationMode mode)
            {
                var provider = FixtureHelper

                    .AutoRegisterUsingMode<IBaseInterface1>(mode, _serviceCollection, _localRegistrar)

                    .AutoRegisterUsingMode<IBaseInterface1>(mode, _externalRegistrar)

                    .BuildServiceProvider();

                AssertExpectation<IBaseInterface1>(
                    provider,
                    new[] { typeof(ConcreteClassA), typeof(ConcreteClassB), typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassF), typeof(ConcreteClassG) });

                // ConcreteClassE implements IBaseInterface2 BUT it is not registered because it does not inherit IBaseInterface1
                AssertExpectation<IBaseInterface2>(
                    provider,
                    Enumerable.Empty<Type>());

                AssertExpectation<IInterface3>(
                    provider,
                    new[] { typeof(ConcreteClassF) });

                AssertExpectation<IInterface4>(
                    provider,
                    new[] { typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(RegistrationMode.Singleton)]
            [InlineData(RegistrationMode.Scoped)]
            [InlineData(RegistrationMode.Transient)]
            public void Should_Resolve_All_Interfaces_Except_Registered_Service(RegistrationMode mode)
            {
                var provider = FixtureHelper

                    .AutoRegisterUsingMode<IBaseInterface1>(mode, _serviceCollection, _localRegistrar,
                        config => config.Filter((service, implementation) => service != typeof(IBaseInterface1)))

                    .AutoRegisterUsingMode<IBaseInterface1>(mode, _externalRegistrar,
                        config => config.Filter((service, implementation) => service != typeof(IBaseInterface1)))

                    .BuildServiceProvider();

                // IBaseInterface1 has been filtered out
                AssertExpectation<IBaseInterface1>(
                    provider,
                    Enumerable.Empty<Type>());

                AssertExpectation<IInterface3>(
                    provider,
                    new[] { typeof(ConcreteClassF) });

                AssertExpectation<IInterface4>(
                    provider,
                    new[] { typeof(ConcreteClassG) });
            }

            [Theory]
            [InlineData(RegistrationMode.Singleton)]
            [InlineData(RegistrationMode.Scoped)]
            [InlineData(RegistrationMode.Transient)]
            public void Should_Not_Resolve_Abstract_Class_When_Register_Interface(RegistrationMode mode)
            {
                var provider = FixtureHelper

                    .AutoRegisterUsingMode<IBaseInterface1>(mode, _serviceCollection, _localRegistrar)

                    .AutoRegisterUsingMode<IBaseInterface1>(mode, _externalRegistrar)

                    .BuildServiceProvider();

                // Not currently supporting the ability to do this - it could be extended to resolve ConcreteClassD, ConcreteClassE, ConcreteClassG
                // on the basis they all inherit AbstractClassA which inherits IBaseInterface1 but at this time you either register an abstract
                // class or an interface.
                AssertExpectation<AbstractClassA>(
                    provider,
                    Enumerable.Empty<Type>());
            }
        }





        private static void AssertInstanceEquality<TType>(IEnumerable<TType> items1, IEnumerable<TType> items2, bool expected)
        {
            var instances1 = items1.OrderBy(item => item.GetType().AssemblyQualifiedName);
            var instances2 = items2.OrderBy(item => item.GetType().AssemblyQualifiedName);

            instances1
                .Zip(instances2)
                .ForEach((instance, _) =>
                {
                    instance.First
                        .Should()
                        .BeOfType(instance.Second.GetType());

                    ReferenceEquals(instance.First, instance.Second).Should().Be(expected);
                });
        }

       






        //public class With_Generic_TServiceRegistrar : AutoRegisterSingleton
        //{
        //    [Fact]
        //    public void Should_Register_With_Excluded()
        //    {
        //        var provider = ServiceCollectionExtensions
        //            .AutoRegisterSingleton<LocalDependenciesRegistrar>(_serviceCollection, new[] { typeof(AbstractClassA) })
        //            .AutoRegisterSingleton<ExternalDependenciesRegistrar>(
        //                new[] { typeof(AbstractClassA) },
        //                config => config.ExcludeTypes(typeof(ConcreteClassE)))
        //            .BuildServiceProvider();

        //        AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassG) });
        //    }

        //    [Fact]
        //    public void Should_Register_With_Implementation_Filter()
        //    {
        //        var provider = ServiceCollectionExtensions
        //            .AutoRegisterSingleton<LocalDependenciesRegistrar>(_serviceCollection, new[] { typeof(AbstractClassA) })
        //            .AutoRegisterSingleton<ExternalDependenciesRegistrar>(
        //                new[] { typeof(AbstractClassA) },
        //                config => config.Filter((serviceType, implementationType) => implementationType != typeof(ConcreteClassD)))
        //            .BuildServiceProvider();

        //        AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassE), typeof(ConcreteClassG) });
        //    }

        //    [Fact]
        //    public void Should_Register_With_Service_Filter()
        //    {
        //        var provider = ServiceCollectionExtensions

        //            // All will be excluded
        //            .AutoRegisterSingleton<LocalDependenciesRegistrar>(_serviceCollection,
        //                new[] { typeof(IBaseInterface1) },
        //                config => config.Filter((serviceType, implementationType) => serviceType != typeof(IBaseInterface1)))

        //            // Will only register ConcreteClassG
        //            .AutoRegisterSingleton<ExternalDependenciesRegistrar>(
        //                new[] { typeof(IBaseInterface1) },
        //                config => config.Filter((serviceType, implementationType) => serviceType != typeof(IBaseInterface1)))

        //            .BuildServiceProvider();

        //        AssertExpectation<IBaseInterface1>(provider, Enumerable.Empty<Type>());
        //        AssertExpectation<IInterface4>(provider, new[] { typeof(ConcreteClassG) });
        //    }

        //    [Fact]
        //    public void Should_Register_Abstract_Class()
        //    {
        //        var provider = ServiceCollectionExtensions
        //            .AutoRegisterSingleton<LocalDependenciesRegistrar>(_serviceCollection, new[]{ typeof(AbstractClassA) })
        //            .AutoRegisterSingleton<ExternalDependenciesRegistrar>(new []{ typeof(AbstractClassA) })
        //            .BuildServiceProvider();

        //        AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassG) });
        //    }

        //    [Fact]
        //    public void Should_Register_Abstract_Class_Two_Registrar_Types()
        //    {
        //        var provider = ServiceCollectionExtensions
        //            .AutoRegisterSingleton(
        //                _serviceCollection,
        //                new IServiceRegistrar[] {new LocalDependenciesRegistrar(), new ExternalDependenciesRegistrar()},
        //                new[] {typeof(AbstractClassA)})
        //            .BuildServiceProvider();

        //        AssertExpectation<AbstractClassA>(provider, new[] { typeof(ConcreteClassD), typeof(ConcreteClassE), typeof(ConcreteClassG) });
        //    }


        //}




        private static void AssertExpectation<TServiceType>(IServiceProvider provider, IEnumerable<Type> expectedTypes)
        {
            var actual = provider.GetService<IEnumerable<TServiceType>>()!.Select(item => item.GetType());

            expectedTypes.Should().BeEquivalentTo(actual);
        }
    }
}