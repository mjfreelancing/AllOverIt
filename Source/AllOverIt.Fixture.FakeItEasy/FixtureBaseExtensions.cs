using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Fixture.FakeItEasy
{
    /// <summary>Provides a variety of extension methods for <see cref="FixtureBase"/>.</summary>
    public static class FixtureBaseExtensions
    {
        /// <summary>Adds a customization for FakeItEasy.</summary>
        /// <param name="fixtureBase">The <see cref="FixtureBase"/> instance.</param>
        /// <param name="customization">The customization to add. If null, an <see cref="AutoFakeItEasyCustomization"/> with
        /// 'GenerateDelegates' set to true will be added.</param>
        public static void UseFakeItEasy(this FixtureBase fixtureBase, ICustomization customization = null)
        {
            customization ??= new AutoFakeItEasyCustomization { GenerateDelegates = true };
            fixtureBase.Fixture.Customize(customization);
        }

        /// <summary>Creates a Fake of the specified type.</summary>
        /// <typeparam name="TType">The type to be faked.</typeparam>
        /// <param name="fixtureBase">The <see cref="FixtureBase"/> instance.</param>
        /// <returns>A Fake of the specified type.</returns>
        public static Fake<TType> CreateFake<TType>(this FixtureBase fixtureBase)
            where TType : class
        {
            return CreateFake<TType>(fixtureBase, false);
        }

        /// <summary>Creates a Fake of the specified type.</summary>
        /// <typeparam name="TType">The type to be faked.</typeparam>
        /// <param name="fixtureBase">The <see cref="FixtureBase"/> instance.</param>
        /// <param name="freeze">If True, the same Fake instance will be returned each time an instance is requested.</param>
        /// <returns>A Fake of the specified type.</returns>
        public static Fake<TType> CreateFake<TType>(this FixtureBase fixtureBase, bool freeze)
            where TType : class
        {
            return freeze
              ? fixtureBase.Fixture.Freeze<Fake<TType>>()
              : fixtureBase.Fixture.Create<Fake<TType>>();
        }

        /// <summary>Creates 5 Fake instances of the specified type.</summary>
        /// <typeparam name="TType">The type to be faked.</typeparam>
        /// <param name="fixtureBase">The <see cref="FixtureBase"/> instance.</param>
        /// <returns>Five Fake instances of the specified type.</returns>
        public static IReadOnlyList<Fake<TType>> CreateManyFakes<TType>(this FixtureBase fixtureBase)
            where TType : class
        {
            return CreateManyFakes<TType>(fixtureBase, 5);
        }

        /// <summary>Creates multiple Fake instances of the specified type.</summary>
        /// <typeparam name="TType">The type to be faked.</typeparam>
        /// <param name="fixtureBase">The <see cref="FixtureBase"/> instance.</param>
        /// <param name="count">The number of Fake instances to create.</param>
        /// <returns>Multiple Fake instances of the specified type.</returns>
        public static IReadOnlyList<Fake<TType>> CreateManyFakes<TType>(this FixtureBase fixtureBase, int count)
            where TType : class
        {
            return fixtureBase.Fixture.CreateMany<Fake<TType>>(count).ToList();
        }


        /// <summary>Creates a stub of the specified type.</summary>
        /// <typeparam name="TType">The type to be faked.</typeparam>
        /// <param name="fixtureBase">The <see cref="FixtureBase"/> instance.</param>
        /// <returns>A stub of the specified type.</returns>
        /// <remarks>A stub is an object that holds pre-defined data or setup that is used to satisfy calls made during a test.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for documentation")]
        public static TType CreateStub<TType>(this FixtureBase fixtureBase)
            where TType : class
        {
            return A.Fake<TType>();
        }

        /// <summary>Creates a stub of the specified type.</summary>
        /// <typeparam name="TType">The type to be faked.</typeparam>
        /// <param name="fixtureBase">The <see cref="FixtureBase"/> instance.</param>
        /// <param name="modifier">An action that allows the stub to be configured.</param>
        /// <returns>A stub of the specified type.</returns>
        /// <remarks>A stub is an object that holds pre-defined data or setup that is used to satisfy calls made during a test.</remarks>
        public static TType CreateStub<TType>(this FixtureBase fixtureBase, Action<Fake<TType>> modifier)
          where TType : class
        {
            if (modifier == null)
            {
                throw new ArgumentNullException(nameof(modifier));
            }

            var fake = fixtureBase.Fixture.Create<Fake<TType>>();
            modifier.Invoke(fake);

            return fake.FakedObject;
        }

        /// <summary>Creates 5 stubs of the specified type.</summary>
        /// <typeparam name="TType">The type to be faked.</typeparam>
        /// <param name="fixtureBase">The <see cref="FixtureBase"/> instance.</param>
        /// <returns>Many stubs of the specified type.</returns>
        /// <remarks>A stub is an object that holds pre-defined data or setup that is used to satisfy calls made during a test.</remarks>
        public static IReadOnlyList<TType> CreateManyStubs<TType>(this FixtureBase fixtureBase)
            where TType : class
        {
            return CreateManyStubs<TType>(fixtureBase, 5);
        }

        /// <summary>Creates many stubs of the specified type.</summary>
        /// <typeparam name="TType">The type to be faked.</typeparam>
        /// <param name="fixtureBase">The <see cref="FixtureBase"/> instance.</param>
        /// <param name="count">The number of stubs to create.</param>
        /// <returns>Many stubs of the specified type.</returns>
        /// <remarks>A stub is an object that holds pre-defined data or setup that is used to satisfy calls made during a test.</remarks>
        public static IReadOnlyList<TType> CreateManyStubs<TType>(this FixtureBase fixtureBase, int count)
            where TType : class
        {
            var stubs = new List<TType>();

            for (var i = 0; i < count; ++i)
            {
                stubs.Add(CreateStub<TType>(fixtureBase));
            }

            return stubs;
        }

        /// <summary>Creates 5 stubs of the specified type.</summary>
        /// <typeparam name="TType">The type to be faked.</typeparam>
        /// <param name="fixtureBase">The <see cref="FixtureBase"/> instance.</param>
        /// <param name="modifier">An action that allows each stub to be configured.</param>
        /// <returns>Many stubs of the specified type.</returns>
        /// <remarks>A stub is an object that holds pre-defined data or setup that is used to satisfy calls made during a test.</remarks>
        public static IReadOnlyList<TType> CreateManyStubs<TType>(this FixtureBase fixtureBase, Action<Fake<TType>, int> modifier)
            where TType : class
        {
            return CreateManyStubs(fixtureBase, modifier, 5);
        }

        /// <summary>Creates many stubs of the specified type.</summary>
        /// <typeparam name="TType">The type to be faked.</typeparam>
        /// <param name="fixtureBase">The <see cref="FixtureBase"/> instance.</param>
        /// <param name="modifier">An action that allows each stub to be configured.</param>
        /// <param name="count">The number of stubs to create.</param>
        /// <returns>Many stubs of the specified type.</returns>
        /// <remarks>A stub is an object that holds pre-defined data or setup that is used to satisfy calls made during a test.</remarks>
        public static IReadOnlyList<TType> CreateManyStubs<TType>(this FixtureBase fixtureBase, Action<Fake<TType>, int> modifier, int count)
            where TType : class
        {
            if (modifier == null)
            {
                throw new ArgumentNullException(nameof(modifier));
            }

            var stubs = new List<TType>();

            for (var i = 0; i < count; ++i)
            {
                var fake = fixtureBase.Fixture.Create<Fake<TType>>();
                modifier.Invoke(fake, i);

                stubs.Add(fake.FakedObject);
            }

            return stubs;
        }
    }
}