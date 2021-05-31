using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Fixture.FakeItEasy
{
    public static class AoiFixtureBaseFakeExtensions
    {
        public static void UseFakeItEasy(this AoiFixtureBase fixtureBase, ICustomization customization = null)
        {
            customization = customization ?? new AutoFakeItEasyCustomization { GenerateDelegates = true };
            fixtureBase.Fixture.Customize(customization);
        }

        public static Fake<TType> CreateFake<TType>(this AoiFixtureBase fixtureBase)
            where TType : class
        {
            return CreateFake<TType>(fixtureBase, false);
        }

        public static Fake<TType> CreateFake<TType>(this AoiFixtureBase fixtureBase, bool freeze)
            where TType : class
        {
            return freeze
              ? fixtureBase.Fixture.Freeze<Fake<TType>>()
              : fixtureBase.Fixture.Create<Fake<TType>>();
        }

        public static IReadOnlyList<Fake<TType>> CreateManyFakes<TType>(this AoiFixtureBase fixtureBase)
            where TType : class
        {
            return CreateManyFakes<TType>(fixtureBase, 5);
        }

        public static IReadOnlyList<Fake<TType>> CreateManyFakes<TType>(this AoiFixtureBase fixtureBase, int count)
            where TType : class
        {
            return fixtureBase.Fixture.CreateMany<Fake<TType>>(count).ToList();
        }

        public static TType CreateStub<TType>(this AoiFixtureBase fixtureBase)
            where TType : class
        {
            return A.Fake<TType>();
        }

        public static TType CreateStub<TType>(this AoiFixtureBase fixtureBase, Action<Fake<TType>> modifier)
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

        public static IReadOnlyList<TType> CreateManyStubs<TType>(this AoiFixtureBase fixtureBase)
            where TType : class
        {
            return CreateManyStubs<TType>(fixtureBase, 5);
        }

        public static IReadOnlyList<TType> CreateManyStubs<TType>(this AoiFixtureBase fixtureBase, int count)
            where TType : class
        {
            var stubs = new List<TType>();

            for (var i = 0; i < count; ++i)
            {
                stubs.Add(CreateStub<TType>(fixtureBase));
            }

            return stubs;
        }

        public static IReadOnlyList<TType> CreateManyStubs<TType>(this AoiFixtureBase fixtureBase, Action<Fake<TType>, int> modifier)
            where TType : class
        {
            return CreateManyStubs(fixtureBase, modifier, 5);
        }

        public static IReadOnlyList<TType> CreateManyStubs<TType>(this AoiFixtureBase fixtureBase, Action<Fake<TType>, int> modifier, int count)
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