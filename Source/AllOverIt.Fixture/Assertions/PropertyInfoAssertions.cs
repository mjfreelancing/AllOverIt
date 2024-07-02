#nullable enable

using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Types;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AllOverIt.Fixture.Assertions
{
    /// <summary>Provides a variety of property assertions.</summary>
    [DebuggerNonUserCode]
    public class PropertyInfoAssertions
    {
        private const string Public = "public";
        private const string Protected = "protected";
        private const string Private = "private";
        private const string Internal = "internal";

        private readonly PropertyInfo _subject;

        internal PropertyInfoAssertions(PropertyInfo subject)
        {
            _subject = subject;
        }

        /// <summary>Asserts that a property is readable (get).</summary>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public PropertyInfoAssertions IsReadable(string because = "", params object[] becauseArgs)
        {
            if (SubjectIsNotNull())
            {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(_subject.CanRead)
                    .FailWith("Expected {context} to be readable{reason}, but it isn't.");
            }

            return this;
        }

        /// <summary>Asserts that a property is not readable (get).</summary>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public PropertyInfoAssertions IsNotReadable(string because = "", params object[] becauseArgs)
        {
            if (SubjectIsNotNull())
            {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(!_subject.CanRead)
                    .FailWith("Expected {context} not to be readable{reason}, but it is.");
            }

            return this;
        }

        /// <summary>Asserts that a property is writable (set or init).</summary>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public PropertyInfoAssertions IsWritable(string because = "", params object[] becauseArgs)
        {
            if (SubjectIsNotNull())
            {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(_subject.CanWrite)
                    .FailWith("Expected {context} to be writable{reason}, but it isn't.");
            }

            return this;
        }

        /// <summary>Asserts that a property is not writable (set or init).</summary>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public PropertyInfoAssertions IsNotWritable(string because = "", params object[] becauseArgs)
        {
            if (SubjectIsNotNull())
            {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(!_subject.CanWrite)
                    .FailWith("Expected {context} not to be writable{reason}, but it is.");
            }

            return this;
        }

        /// <summary>Asserts that a property accessor (get, set, or init) is public.</summary>
        /// <param name="accessor">Indicates if the assertion applies to the getter, setter, or both.</param>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public PropertyInfoAssertions IsPublic(PropertyAccessor accessor, string because = "", params object[] becauseArgs)
        {
            HasPropertyAccess(Public, accessor, _subject.IsPublic, because, becauseArgs);

            return this;
        }

        /// <summary>Asserts that a property accessor (get, set, or init) is not public.</summary>
        /// <param name="accessor">Indicates if the assertion applies to the getter, setter, or both.</param>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public PropertyInfoAssertions IsNotPublic(PropertyAccessor accessor, string because = "", params object[] becauseArgs)
        {
            HasNotPropertyAccess(Public, accessor, _subject.IsPublic, because, becauseArgs);

            return this;
        }

        /// <summary>Asserts that a property accessor (get, set, or init) is protected.</summary>
        /// <param name="accessor">Indicates if the assertion applies to the getter, setter, or both.</param>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public PropertyInfoAssertions IsProtected(PropertyAccessor accessor, string because = "", params object[] becauseArgs)
        {
            HasPropertyAccess(Protected, accessor, _subject.IsProtected, because, becauseArgs);

            return this;
        }

        /// <summary>Asserts that a property accessor (get, set, or init) is not protected.</summary>
        /// <param name="accessor">Indicates if the assertion applies to the getter, setter, or both.</param>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public PropertyInfoAssertions IsNotProtected(PropertyAccessor accessor, string because = "", params object[] becauseArgs)
        {
            HasNotPropertyAccess(Protected, accessor, _subject.IsProtected, because, becauseArgs);

            return this;
        }

        /// <summary>Asserts that a property accessor (get, set, or init) is private.</summary>
        /// <param name="accessor">Indicates if the assertion applies to the getter, setter, or both.</param>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public PropertyInfoAssertions IsPrivate(PropertyAccessor accessor, string because = "", params object[] becauseArgs)
        {
            HasPropertyAccess(Private, accessor, _subject.IsPrivate, because, becauseArgs);

            return this;
        }

        /// <summary>Asserts that a property accessor (get, set, or init) is not private.</summary>
        /// <param name="accessor">Indicates if the assertion applies to the getter, setter, or both.</param>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public PropertyInfoAssertions IsNotPrivate(PropertyAccessor accessor, string because = "", params object[] becauseArgs)
        {
            HasNotPropertyAccess(Private, accessor, _subject.IsPrivate, because, becauseArgs);

            return this;
        }

        /// <summary>Asserts that a property accessor (get, set, or init) is internal.</summary>
        /// <param name="accessor">Indicates if the assertion applies to the getter, setter, or both.</param>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public PropertyInfoAssertions IsInternal(PropertyAccessor accessor, string because = "", params object[] becauseArgs)
        {
            HasPropertyAccess(Internal, accessor, _subject.IsInternal, because, becauseArgs);

            return this;
        }

        /// <summary>Asserts that a property accessor (get, set, or init) is not internal.</summary>
        /// <param name="accessor">Indicates if the assertion applies to the getter, setter, or both.</param>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public PropertyInfoAssertions IsNotInternal(PropertyAccessor accessor, string because = "", params object[] becauseArgs)
        {
            HasNotPropertyAccess(Internal, accessor, _subject.IsInternal, because, becauseArgs);

            return this;
        }

        /// <summary>Asserts that a property is an indexer.</summary>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public PropertyInfoAssertions IsAnIndexer(string because = "", params object[] becauseArgs)
        {
            if (SubjectIsNotNull())
            {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(_subject.IsIndexer())
                    .FailWith("Expected {context} to be an indexer{reason}, but it isn't.");
            }

            return this;
        }

        /// <summary>Asserts that a property is not an indexer.</summary>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public PropertyInfoAssertions IsNotAnIndexer(string because = "", params object[] becauseArgs)
        {
            if (SubjectIsNotNull())
            {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(!_subject.IsIndexer())
                    .FailWith("Expected {context} not to be an indexer{reason}, but it is.");
            }

            return this;
        }

        /// <summary>Asserts that a property is static.</summary>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public PropertyInfoAssertions IsStatic(string because = "", params object[] becauseArgs)
        {
            if (SubjectIsNotNull())
            {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(_subject.IsStatic())
                    .FailWith("Expected {context} to be static{reason}, but it isn't.");
            }

            return this;
        }

        /// <summary>Asserts that a property is not static.</summary>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public PropertyInfoAssertions IsNotStatic(string because = "", params object[] becauseArgs)
        {
            if (SubjectIsNotNull())
            {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(!_subject.IsStatic())
                    .FailWith("Expected {context} not to be static{reason}, but it is.");
            }

            return this;
        }

        /// <summary>Asserts that a property is abstract.</summary>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public PropertyInfoAssertions IsAbstract(string because = "", params object[] becauseArgs)
        {
            if (SubjectIsNotNull())
            {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(_subject.IsAbstract())
                    .FailWith("Expected {context} to be abstract{reason}, but it isn't.");
            }

            return this;
        }

        /// <summary>Asserts that a property is not abstract.</summary>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public PropertyInfoAssertions IsNotAbstract(string because = "", params object[] becauseArgs)
        {
            if (SubjectIsNotNull())
            {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(!_subject.IsAbstract())
                    .FailWith("Expected {context} not to be abstract{reason}, but it is.");
            }

            return this;
        }

        /// <summary>Asserts that a property is virtual.</summary>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public PropertyInfoAssertions IsVirtual(string because = "", params object[] becauseArgs)
        {
            if (SubjectIsNotNull())
            {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(_subject.IsVirtual())
                    .FailWith("Expected {context} to be virtual{reason}, but it isn't.");
            }

            return this;
        }

        /// <summary>Asserts that a property is not virtual.</summary>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public PropertyInfoAssertions IsNotVirtual(string because = "", params object[] becauseArgs)
        {
            if (SubjectIsNotNull())
            {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(!_subject.IsVirtual())
                    .FailWith("Expected {context} not to be virtual{reason}, but it is.");
            }

            return this;
        }

#if NET8_0_OR_GREATER

        /// <summary>Asserts that a property has a required modifier.</summary>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public PropertyInfoAssertions HasRequiredModifier(string because = "", params object[] becauseArgs)
        {
            if (SubjectIsNotNull())
            {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(_subject.GetCustomAttribute<RequiredMemberAttribute>() is not null)
                    .FailWith("Expected {context} to have a required modifier{reason}, but it doesn't.");
            }

            return this;
        }

        /// <summary>Asserts that a property does not have a required modifier.</summary>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public PropertyInfoAssertions HasNoRequiredModifier(string because = "", params object[] becauseArgs)
        {
            if (SubjectIsNotNull())
            {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(_subject.GetCustomAttribute<RequiredMemberAttribute>() is null)
                    .FailWith("Expected {context} not to have a required modifier{reason}, but it does.");
            }

            return this;
        }

#endif

        /// <summary>Asserts that the property's <see cref="PropertyInfo"/> meets a specified criteria.</summary>
        /// <param name="predicate">The predicate that must evaluate to <see langword="True"/>.</param>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public PropertyInfoAssertions MeetsCriteria(Func<PropertyInfo, bool> predicate, string because = "", params object[] becauseArgs)
        {
            if (SubjectIsNotNull())
            {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(predicate.Invoke(_subject))
                    .FailWith("Expected {context} to meet a specified criteria{reason}, but it did not.");
            }

            return this;
        }

        /// <summary>Asserts that a property is of a specified type.</summary>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public PropertyInfoAssertions IsOfType<TType>(string because = "", params object[] becauseArgs)
        {
            if (SubjectIsNotNull())
            {
                // Can't use  _subject.PropertyType.Should().Be() as FluentAssertions reports as "Expected type..."

                var expectedType = typeof(TType);

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(_subject.PropertyType == expectedType)
                    .FailWith("Expected {context} to be of type {0}{reason}, but found {1}.", expectedType.GetFriendlyName(true), _subject.PropertyType.GetFriendlyName(true));
            }

            return this;
        }

        /// <summary>Asserts that a property is of a specified type and invokes an action that provides the property nullability
        /// info for performing additional assertion checks.</summary>
        /// <typeparam name="TType">The expected property type.</typeparam>
        /// <param name="nullabilityAssertions">An action that allows for assertions to be applied against the property's nullability info.</param>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public PropertyInfoAssertions IsOfType<TType>(Action<NullabilityInfoAssertions> nullabilityAssertions, string because = "", params object[] becauseArgs)
        {
            _ = nullabilityAssertions.WhenNotNull();

            if (SubjectIsNotNull())
            {
                using var _ = new AssertionScope();

                var expectedType = typeof(TType);

                if (_subject.PropertyType != expectedType)
                {
                    Execute.Assertion
                        .BecauseOf(because, becauseArgs)
                        .FailWith("Expected {context} to be of type {0}{reason}, but found {1}.", expectedType.GetFriendlyName(true), _subject.PropertyType.GetFriendlyName(true));
                }
                else
                {
                    using var scope = new AssertionScope();

                    var nullabilityInfo = new NullabilityInfoContext().Create(_subject);
                    var assertions = new NullabilityInfoAssertions(nullabilityInfo);

                    nullabilityAssertions.Invoke(assertions);
                }
            }

            return this;
        }

        /// <summary>Asserts that a property is assignable to a specified type.</summary>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public PropertyInfoAssertions IsAssignableTo<TType>(string because = "", params object[] becauseArgs)
        {
            if (SubjectIsNotNull())
            {
                // Can't use  _subject.PropertyType.Should().Be() as FluentAssertions reports as "Expected type..."

                var expectedType = typeof(TType);

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(_subject.PropertyType.IsAssignableTo(expectedType))
                    .FailWith(
                        "Expected {context} to be assignable to type {0}{reason}, but found {1}.",
                        expectedType.GetFriendlyName(true),
                        _subject.PropertyType.GetFriendlyName(true));
            }

            return this;
        }

        /// <summary>Asserts that a property is assignable from a specified type.</summary>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public PropertyInfoAssertions IsAssignableFrom<TType>(string because = "", params object[] becauseArgs)
        {
            if (SubjectIsNotNull())
            {
                // Can't use  _subject.PropertyType.Should().Be() as FluentAssertions reports as "Expected type..."

                var expectedType = typeof(TType);

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(_subject.PropertyType.IsAssignableFrom(expectedType))
                    .FailWith(
                        "Expected {context} to be assignable from type {0}{reason}, but found {1}.",
                        expectedType.GetFriendlyName(true),
                        _subject.PropertyType.GetFriendlyName(true));
            }

            return this;
        }

        /// <summary>Asserts that a property is nullable, including reference types.</summary>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public PropertyInfoAssertions IsNullable(string because = "", params object[] becauseArgs)
        {
            if (SubjectIsNotNull())
            {
                var nullabilityInfo = new NullabilityInfoContext().Create(_subject);

                var state = nullabilityInfo.ReadState != NullabilityState.Unknown
                   ? nullabilityInfo.ReadState
                   : nullabilityInfo.WriteState;

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(state == NullabilityState.Nullable)
                    .FailWith("Expected {context} to be {0}{reason}, but it is {1}.", GetNullableString(false), GetNullableString(true));
            }

            return this;
        }

        /// <summary>Asserts that a property is not nullable, including reference types.</summary>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public PropertyInfoAssertions IsNotNullable(string because = "", params object[] becauseArgs)
        {
            if (SubjectIsNotNull())
            {
                var nullabilityInfo = new NullabilityInfoContext().Create(_subject);

                var state = nullabilityInfo.ReadState != NullabilityState.Unknown
                   ? nullabilityInfo.ReadState
                   : nullabilityInfo.WriteState;

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(state == NullabilityState.NotNull)
                    .FailWith("Expected {context} to be {0}{reason}, but it is {1}.", GetNullableString(true), GetNullableString(false));
            }

            return this;
        }

        private bool SubjectIsNotNull()
        {
            return Execute.Assertion
               .ForCondition(_subject is not null)
               .FailWith("Cannot validate {context:property} when its <PropertyInfo> is <null>.");
        }

        private void HasPropertyAccess(string visibility, PropertyAccessor accessor, Func<PropertyAccessor, bool> predicate, string because = "", params object[] becauseArgs)
        {
            if (SubjectIsNotNull())
            {
                using var _ = new AssertionScope();

                if (accessor.HasFlag(PropertyAccessor.Set) && accessor.HasFlag(PropertyAccessor.Init))
                {
                    Execute.Assertion
                        .BecauseOf(because, becauseArgs)
                        .FailWith($"Property access validation cannot include '{nameof(PropertyAccessor.Set)}' and '{nameof(PropertyAccessor.Init)}'.");
                }
                else
                {
                    // Must have Get and the required visibility
                    if (accessor.HasFlag(PropertyAccessor.Get))
                    {
                        Execute.Assertion
                            .BecauseOf(because, becauseArgs)
                            .ForCondition(_subject.GetMethod is not null && predicate.Invoke(PropertyAccessor.Get))
                            .FailWith(GetAccessorFailMessage(visibility, PropertyAccessor.Get));
                    }

                    // Must have Set and the required visibility
                    if (accessor.HasFlag(PropertyAccessor.Set))
                    {
                        Execute.Assertion
                            .BecauseOf(because, becauseArgs)
                            .ForCondition(_subject.SetMethod is not null && predicate.Invoke(PropertyAccessor.Set) && !_subject.IsInitOnly())
                            .FailWith(GetAccessorFailMessage(visibility, PropertyAccessor.Set));
                    }

                    // Must have Init and the required visibility
                    if (accessor.HasFlag(PropertyAccessor.Init))
                    {
                        Execute.Assertion
                            .BecauseOf(because, becauseArgs)
                            .ForCondition(_subject.IsInitOnly() && predicate.Invoke(PropertyAccessor.Set))
                            .FailWith(GetAccessorFailMessage(visibility, PropertyAccessor.Init));
                    }
                }
            }
        }

        private void HasNotPropertyAccess(string visibility, PropertyAccessor accessor, Func<PropertyAccessor, bool> predicate, string because = "", params object[] becauseArgs)
        {
            if (SubjectIsNotNull())
            {
                using var _ = new AssertionScope();

                if (accessor.HasFlag(PropertyAccessor.Set) && accessor.HasFlag(PropertyAccessor.Init))
                {
                    Execute.Assertion
                        .BecauseOf(because, becauseArgs)
                        .FailWith($"Property access validation cannot include '{nameof(PropertyAccessor.Set)}' and '{nameof(PropertyAccessor.Init)}'.");
                }
                else
                {
                    // Must have Get and not the required visibility
                    if (accessor.HasFlag(PropertyAccessor.Get))
                    {
                        Execute.Assertion
                            .BecauseOf(because, becauseArgs)
                            .ForCondition(_subject.GetMethod is not null && !predicate.Invoke(PropertyAccessor.Get))
                            .FailWith(GetNotAccessorFailMessage(visibility, PropertyAccessor.Get));
                    }

                    // Must have Get and not the required visibility
                    if (accessor.HasFlag(PropertyAccessor.Set))
                    {
                        Execute.Assertion
                            .BecauseOf(because, becauseArgs)
                            .ForCondition(_subject.SetMethod is not null && !predicate.Invoke(PropertyAccessor.Set) && !_subject.IsInitOnly())
                            .FailWith(GetNotAccessorFailMessage(visibility, PropertyAccessor.Set));
                    }

                    // Must have Get and not the required visibility
                    if (accessor.HasFlag(PropertyAccessor.Init))
                    {
                        Execute.Assertion
                            .BecauseOf(because, becauseArgs)
                            .ForCondition(_subject.IsInitOnly() && !predicate.Invoke(PropertyAccessor.Set))
                            .FailWith(GetNotAccessorFailMessage(visibility, PropertyAccessor.Init));
                    }
                }
            }
        }

        private string GetAccessorFailMessage(string visibility, PropertyAccessor accessor)
        {
            var accessorStr = $"{accessor}".ToLower();

            var found = GetSubjectAccessorVisibilityDescription(accessor);

            return $"Expected {{context}} to have {GetVisibilityStringWithAPrefix(visibility)} {accessorStr} accessor{{reason}}, but found it has {found}.";
        }

        private string GetNotAccessorFailMessage(string visibility, PropertyAccessor accessor)
        {
            var accessorStr = $"{accessor}".ToLower();

            var found = GetSubjectAccessorVisibilityDescription(accessor);

            return $"Expected {{context}} not to have {GetVisibilityStringWithAPrefix(visibility)} {accessorStr} accessor{{reason}}, but found it has {found}.";
        }

        private string GetSubjectAccessorVisibilityDescription(PropertyAccessor accessor)
        {
            var found = "no matching accessor";

            switch (accessor)
            {
                case PropertyAccessor.Get:
                    if (_subject.GetMethod is not null)
                    {
                        found = $"{GetSubjectAccessorVisibilityString(PropertyAccessor.Get)} get accessor";
                    }
                    break;

                case PropertyAccessor.Set:
                case PropertyAccessor.Init:
                    if (_subject.SetMethod is not null)
                    {
                        var actualVisibility = GetSubjectAccessorVisibilityString(PropertyAccessor.Set);

                        if (_subject.IsInitOnly())
                        {
                            found = $"{actualVisibility} init only accessor";
                        }
                        else
                        {
                            found = $"{actualVisibility} set accessor";
                        }
                    }
                    break;
            }

            return found;
        }

        private string GetSubjectAccessorVisibilityString(PropertyAccessor propertyAccessor)
        {
            if (_subject.IsPublic(propertyAccessor))
            {
                return GetVisibilityStringWithAPrefix(Public);
            }
            else if (_subject.IsProtected(propertyAccessor))
            {
                return GetVisibilityStringWithAPrefix(Protected);
            }
            else if (_subject.IsPrivate(propertyAccessor))
            {
                return GetVisibilityStringWithAPrefix(Private);
            }
            else if (_subject.IsInternal(propertyAccessor))
            {
                return GetVisibilityStringWithAPrefix(Internal);
            }

            throw new InvalidOperationException($"Unhandled property accessor '{propertyAccessor}'.");
        }

        private static string GetVisibilityStringWithAPrefix(string visibility)
        {
            return visibility switch
            {
                Public => "a public",
                Protected => "a protected",
                Private => "a private",
                Internal => "an internal",
                _ => throw new InvalidOperationException($"Unhandled visibility '{visibility}'.")
            };
        }

        private static string GetNullableString(bool inverted)
        {
            return inverted ? "Not Nullable" : "Nullable";
        }
    }
}
