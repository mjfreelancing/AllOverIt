﻿using AllOverIt.Fixture.Exceptions;
using AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Fixture
{
    /// <summary>
    /// Acts as a base class for all fixtures, providing access to a variety of useful methods that help generate automated input values.
    /// </summary>
    public abstract class FixtureBase
    {
        private readonly Random _random = new();

        /// <summary> Provides access to the AutoFixture.Fixture being used.</summary>
        protected internal IFixture Fixture { get; } = new AutoFixture.Fixture();

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected FixtureBase()
        {
            // Note: cannot used <double> for the factory as it will result in infinite recursion
            var rnd = new Random();
            Fixture.Customize<float>(composer => composer.FromFactory<int>(value => value * (0.5f + (float) rnd.NextDouble())));
            Fixture.Customize<double>(composer => composer.FromFactory<int>(value => value * (0.5d + rnd.NextDouble())));
            Fixture.Customize<decimal>(composer => composer.FromFactory<int>(value => value * (0.5m + (decimal) rnd.NextDouble())));
        }

        /// <summary>
        /// Constructor that supports customization of AutoFixture's Fixture.
        /// </summary>
        /// <param name="customization">The customization instance.</param>
        protected FixtureBase(ICustomization customization)
            : this()
        {
            Fixture.Customize(customization);
        }

        /// <summary>Provides the ability to invoke an action so it can be chained with assertions provided by FluentAssertions.</summary>
        /// <param name="action">The action to be invoked.</param>
        /// <returns>The same action passed to the method.</returns>
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA1822 // Mark members as static
        protected Action Invoking(Action action)
#pragma warning restore CA1822 // Mark members as static
#pragma warning restore IDE0079 // Remove unnecessary suppression
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return action;
        }

        /// <summary>Provides the ability to invoke an action that returns a result.</summary>
        /// <typeparam name="TResult">The result type returned by the Func.</typeparam>
        /// <param name="action">The action to be invoked.</param>
        /// <returns>The result of the invoked action.</returns>
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA1822 // Mark members as static
        protected Func<TResult> Invoking<TResult>(Func<TResult> action)
#pragma warning restore CA1822 // Mark members as static
#pragma warning restore IDE0079 // Remove unnecessary suppression
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return action;
        }

        /// <summary>Injects a specific instance of a specified type that will be resolved as a shared (single) instance.</summary>
        /// <typeparam name="TType">The type being registered.</typeparam>
        /// <param name="type">The instance that will be resolved every time one of the Create() methods is called.</param>
        protected void Inject<TType>(TType type)
        {
            Fixture.Inject(type);
        }

        /// <summary>Registers a factory method for a specific type that requires no input parameters.</summary>
        /// <typeparam name="TType">The type created by the registered factory method.</typeparam>
        /// <param name="creator">The factory method used by AutoFixture when it is asked to create the required
        /// type <typeparamref name="TType"/>.</param>
        protected void Register<TType>(Func<TType> creator)
        {
            // throws ArgumentNullException if creator is null
            Fixture.Register(creator);
        }

        /// <summary>Registers a factory method for a specific type that requires a single input parameter.</summary>
        /// <typeparam name="TInput">The type of the input parameter.</typeparam>
        /// <typeparam name="TType">The type created by the registered factory method.</typeparam>
        /// <param name="creator">The factory method used by AutoFixture when it is asked to create the required
        /// type <typeparamref name="TType"/>.</param>
        protected void Register<TInput, TType>(Func<TInput, TType> creator)
        {
            // throws ArgumentNullException if creator is null
            Fixture.Register(creator);
        }

        /// <summary>Registers a factory method for a specific type that requires two input parameters.</summary>
        /// <typeparam name="TInput1">The type of the first input parameter.</typeparam>
        /// <typeparam name="TInput2">The type of the second input parameter.</typeparam>
        /// <typeparam name="TType">The type created by the registered factory method.</typeparam>
        /// <param name="creator">The factory method used by AutoFixture when it is asked to create the required
        /// type <typeparamref name="TType"/>.</param>
        protected void Register<TInput1, TInput2, TType>(Func<TInput1, TInput2, TType> creator)
        {
            // throws ArgumentNullException if creator is null
            Fixture.Register(creator);
        }

        /// <summary>Registers a factory method for a specific type that requires three input parameters.</summary>
        /// <typeparam name="TInput1">The type of the first input parameter.</typeparam>
        /// <typeparam name="TInput2">The type of the second input parameter.</typeparam>
        /// <typeparam name="TInput3">The type of the third input parameter.</typeparam>
        /// <typeparam name="TType">The type created by the registered factory method.</typeparam>
        /// <param name="creator">The factory method used by AutoFixture when it is asked to create the required
        /// type <typeparamref name="TType"/>.</param>
        protected void Register<TInput1, TInput2, TInput3, TType>(Func<TInput1, TInput2, TInput3, TType> creator)
        {
            // throws ArgumentNullException if creator is null
            Fixture.Register(creator);
        }

        /// <summary>Registers a factory method for a specific type that requires four input parameters.</summary>
        /// <typeparam name="TInput1">The type of the first input parameter.</typeparam>
        /// <typeparam name="TInput2">The type of the second input parameter.</typeparam>
        /// <typeparam name="TInput3">The type of the third input parameter.</typeparam>
        /// <typeparam name="TInput4">The type of the fourth input parameter.</typeparam>
        /// <typeparam name="TType">The type created by the registered factory method.</typeparam>
        /// <param name="creator">The factory method used by AutoFixture when it is asked to create the required
        /// type <typeparamref name="TType"/>.</param>
        protected void Register<TInput1, TInput2, TInput3, TInput4, TType>(Func<TInput1, TInput2, TInput3, TInput4, TType> creator)
        {
            // throws ArgumentNullException if creator is null
            Fixture.Register(creator);
        }

        /// <summary>Creates an anonymous instance of type <typeparamref name="TType"/>.</summary>
        /// <typeparam name="TType">The type of object to create.</typeparam>
        /// <returns>An anonymous object of type <typeparamref name="TType"/>.</returns>
        protected TType Create<TType>()
        {
            return CreateType<TType>();
        }

        /// <summary>Creates a randomly initialized value type that has some value other than what is specified.</summary>
        /// <typeparam name="TType">The value type to be created.</typeparam>
        /// <param name="excludes">One or more values that are excluded from the random initialization.</param>
        /// <returns>A randomly initialized value type that has some value other than what is specified by <paramref name="excludes"/>.</returns>
        protected TType CreateExcluding<TType>(params TType[] excludes)
        {
            var value = CreateType<TType>();

            while (excludes.Contains(value))
            {
                value = CreateType<TType>();
            }

            return value;
        }

        /// <summary>Creates one or more anonymous instances of type <typeparamref name="TType"/>.</summary>
        /// <typeparam name="TType">The type of object to create.</typeparam>
        /// <param name="count">The number of random instances to return. If not specified, five instances will be returned.</param>
        /// <returns>One or more anonymous instances of type <typeparamref name="TType"/>.</returns>
        protected IReadOnlyList<TType> CreateMany<TType>(int count = 5)
        {
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(count), $"{nameof(count)} must be greater than zero");
            }

            return CreateManyType<TType>(count);
        }

        /// <summary>Creates one or more anonymous instances of type <typeparamref name="TType"/>.</summary>
        /// <typeparam name="TType">The type of object to create.</typeparam>
        /// <param name="count">The number of random instances to return. If not specified, five instances will be returned.</param>
        /// <param name="allowDuplicates">Indicates if duplicate values are allowed.</param>
        /// <param name="excludes">One or more values that are excluded from the random initialization.</param>
        /// <returns>One or more anonymous instances of type <typeparamref name="TType"/>.</returns>
        protected IReadOnlyList<TType> CreateManyExcluding<TType>(int count = 5, bool allowDuplicates = true, params TType[] excludes)
        {
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(count), $"{nameof(count)} must be greater than zero");
            }

            var values = new List<TType>(count);

            while (values.Count != count)
            {
                var remaining = count - values.Count;

                var items = allowDuplicates
                  ? CreateManyType<TType>(remaining)
                  : CreateManyDistinct<TType>(remaining);

                if (!allowDuplicates)
                {
                    items = items.Except(values).Except(excludes).ToList();
                }

                values.AddRange(items);
            }

            return values;
        }

        /// <summary>Creates one or more anonymous, distinct, instances of type <typeparamref name="TType"/>.</summary>
        /// <typeparam name="TType">The type of object to create.</typeparam>
        /// <param name="count">The number of random instances to return. If not specified, five instances will be returned.</param>
        /// <returns>One or more anonymous, distinct, instances of type <typeparamref name="TType"/>.</returns>
        protected IReadOnlyList<TType> CreateManyDistinct<TType>(int count = 5)
        {
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(count), $"{nameof(count)} must be greater than zero");
            }

            var values = new List<TType>(count);

            while (values.Count != count)
            {
                var value = Create<TType>();

                if (!values.Contains(value))
                {
                    values.Add(value);
                }
            }

            return values;
        }

        /// <summary>Returns a random integer that is within a specified range.</summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The inclusive upper bound of the random number returned.</param>
        /// <remarks>maxValue must be greater than or equal to minValue.</remarks>
        /// <returns>A random integer that is within a specified range.</returns>
        protected int GetWithinRange(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(minValue), $"The {nameof(minValue)} must be less than {nameof(maxValue)}");
            }

            return _random.Next(minValue, maxValue + 1);
        }

        /// <summary>Returns a random double that is within a specified range.</summary>
        /// <param name="minValue">The inclusive double lower bound of the random number returned.</param>
        /// <param name="maxValue">The inclusive upper bound of the random number returned.</param>
        /// <returns>A random integer that is within a specified range.</returns>
        /// <remarks>maxValue must be greater than or equal to minValue.</remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minValue">minValue</paramref> is greater than <paramref name="maxValue">maxValue</paramref></exception>
        protected double GetWithinRange(double minValue, double maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(minValue), $"The {nameof(minValue)} must be less than {nameof(maxValue)}");
            }

            var value = _random.NextDouble();

            return (maxValue - minValue) * value + minValue;
        }

        /// <summary>Returns one or more random integers that is within a specified range.</summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The inclusive upper bound of the random number returned.</param>
        /// <param name="count">The number of random integers to return. If not specified, five values will be returned.</param>
        /// <param name="allowDuplicates">Indicates if duplicate values are allowed.</param>
        /// <remarks>maxValue must be greater than or equal to minValue.</remarks>
        /// <returns>One or more random integers that is within a specified range.</returns>
        protected IReadOnlyList<int> GetManyWithinRange(int minValue, int maxValue, int count = 5, bool allowDuplicates = true)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(minValue), $"The {nameof(minValue)} must be less than {nameof(maxValue)}");
            }

            if (count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(count), $"{nameof(count)} must be greater than zero");
            }

            var items = new List<int>(count);

            if (allowDuplicates)
            {
                var values = Enumerable.Range(1, count).Select(i => GetWithinRange(minValue, maxValue));
                items.AddRange(values);
            }
            else
            {
                while (items.Count < count)
                {
                    var value = GetWithinRange(minValue, maxValue);

                    if (!items.Contains(value))
                    {
                        items.Add(value);
                    }
                }
            }

            return items;
        }

        /// <summary>Returns one or more random doubles that is within a specified range.</summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The inclusive upper bound of the random number returned.</param>
        /// <param name="count">The number of random doubles to return. If not specified, five values will be returned.</param>
        /// <param name="allowDuplicates">Indicates if duplicate values are allowed.</param>
        /// <remarks>maxValue must be greater than or equal to minValue.</remarks>
        /// <returns>One or more random doubles that is within a specified range.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minValue">minValue</paramref> is greater than <paramref name="maxValue">maxValue</paramref></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count">count</paramref> is less than one</exception>
        protected IReadOnlyList<double> GetManyWithinRange(double minValue, double maxValue, int count = 5, bool allowDuplicates = true)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(minValue), $"The {nameof(minValue)} must be less than {nameof(maxValue)}");
            }

            if (count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(count), $"{nameof(count)} must be greater than zero");
            }

            var items = new List<double>(count);

            if (allowDuplicates)
            {
                var values = Enumerable.Range(1, count).Select(i => GetWithinRange(minValue, maxValue));
                items.AddRange(values);
            }
            else
            {
                while (items.Count < count)
                {
                    var value = GetWithinRange(minValue, maxValue);

                    if (!items.Contains(value))
                    {
                        items.Add(value);
                    }
                }
            }

            return items;
        }

        /// <summary>Asserts when a specified action is invoked that an AggregateException will be thrown and all expected exception
        /// types are handled.</summary>
        /// <param name="action">The action to invoke.</param>
        /// <param name="exceptionHandler">The handler invoked with each exception contained within an aggregate exception.</param>
        protected static void AssertHandledAggregateException(Action action, Func<Exception, bool> exceptionHandler)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (exceptionHandler == null)
            {
                throw new ArgumentNullException(nameof(exceptionHandler));
            }

            try
            {
                action.Invoke();

                throw new AggregateAssertionException("Expected an AggregateException but nothing was thrown");
            }
            catch (AggregateException aggregateException)
            {
                var exception = aggregateException.Flatten();

                try
                {
                    // if all exceptions are handled then no exception will be raised
                    exception.Handle(exceptionHandler.Invoke);
                }
                catch (AggregateException unhandledException)
                {
                    var exceptionTypes =
                      string.Join(", ", unhandledException.InnerExceptions.Select(item => $"{item.GetType()}"));

                    throw new AggregateAssertionException($"There were unhandled exceptions: {exceptionTypes}", aggregateException, unhandledException);
                }
            }
            catch (AggregateAssertionException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new AggregateAssertionException($"Expected an AggregateException but a {exception.GetType()} was thrown");
            }
        }

        private TType CreateType<TType>()
        {
            // Fixture.Create() doesn't randomize enum values - it uses a round-robin approach.
            if (!typeof(TType).IsEnum)
            {
                return Fixture.Create<TType>();
            }

            var enumValues = Enum.GetValues(typeof(TType));
            var enumCount = enumValues.Length;
            var index = _random.Next(1000) % enumCount;

            return (TType)enumValues.GetValue(index);
        }

        private IReadOnlyList<TType> CreateManyType<TType>(int count)
        {
            // Fixture.CreateMany() doesn't randomize enum values - it uses a round-robin approach.
            if (!typeof(TType).IsEnum)
            {
                return Fixture.CreateMany<TType>(count).ToList();
            }

            var enumValues = Enum.GetValues(typeof(TType));
            var enumCount = enumValues.Length;

            return Enumerable
              .Range(1, count)
              .Select(_ =>
              {
                  var index = _random.Next(1000) % enumCount;
                  return (TType)enumValues.GetValue(index);
              })
              .ToList();
        }
    }
}