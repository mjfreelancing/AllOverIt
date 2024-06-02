#nullable enable

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

        /// <summary>Asserts that a property is readable.</summary>
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

        /// <summary>Asserts that a property is not readable.</summary>
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

        /// <summary>Asserts that a property is writable.</summary>
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

        /// <summary>Asserts that a property is not writable.</summary>
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

        /// <summary>Asserts that a property is public.</summary>
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

        /// <summary>Asserts that a property is not public.</summary>
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

        /// <summary>Asserts that a property is protected.</summary>
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

        /// <summary>Asserts that a property is not protected.</summary>
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

        /// <summary>Asserts that a property is private.</summary>
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

        /// <summary>Asserts that a property is not private.</summary>
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

        /// <summary>Asserts that a property is internal.</summary>
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

        /// <summary>Asserts that a property is not internal.</summary>
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

        /// <summary>Asserts that a property has an InitOnly accessor.</summary>
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
        public PropertyInfoAssertions HasInitAccessor(string because = "", params object[] becauseArgs)
        {
            if (SubjectIsNotNull())
            {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(_subject.IsInitOnly())
                    .FailWith("Expected {context} to have an init accessor{reason}, but it doesn't.");
            }

            return this;
        }

        /// <summary>Asserts that a property does not have an InitOnly accessor.</summary>
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
        public PropertyInfoAssertions HasNoInitAccessor(string because = "", params object[] becauseArgs)
        {
            if (SubjectIsNotNull())
            {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(!_subject.IsInitOnly())
                    .FailWith("Expected {context} not to have an init accessor{reason}, but it does.");
            }

            return this;
        }

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

        /// <summary>Asserts that a property is of a specific type.</summary>
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

#if !NETSTANDARD2_1
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
#endif

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

        [CustomAssertion]
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

                if (accessor.HasFlag(PropertyAccessor.Get))
                {
                    Execute.Assertion
                        .BecauseOf(because, becauseArgs)
                        .ForCondition(_subject.GetMethod is not null && predicate.Invoke(PropertyAccessor.Get))
                        .FailWith(GetAccessorFailMessage(visibility, PropertyAccessor.Get));
                }

                if (accessor.HasFlag(PropertyAccessor.Set))
                {
                    Execute.Assertion
                        .BecauseOf(because, becauseArgs)
                        .ForCondition(_subject.SetMethod is not null && predicate.Invoke(PropertyAccessor.Set))
                        .FailWith(GetAccessorFailMessage(visibility, PropertyAccessor.Set));
                }
            }
        }

        private void HasNotPropertyAccess(string visibility, PropertyAccessor accessor, Func<PropertyAccessor, bool> predicate, string because = "", params object[] becauseArgs)
        {
            if (SubjectIsNotNull())
            {
                using var _ = new AssertionScope();

                if (accessor.HasFlag(PropertyAccessor.Get))
                {
                    Execute.Assertion
                        .BecauseOf(because, becauseArgs)
                        .ForCondition(_subject.GetMethod is not null && !predicate.Invoke(PropertyAccessor.Get))
                        .FailWith(GetNotAccessorFailMessage(visibility, PropertyAccessor.Get));
                }

                if (accessor.HasFlag(PropertyAccessor.Set))
                {
                    Execute.Assertion
                        .BecauseOf(because, becauseArgs)
                        .ForCondition(_subject.SetMethod is not null && !predicate.Invoke(PropertyAccessor.Set))
                        .FailWith(GetNotAccessorFailMessage(visibility, PropertyAccessor.Set));
                }
            }
        }

        private string GetAccessorFailMessage(string visibility, PropertyAccessor accessor)
        {
            var accessorStr = $"{accessor}".ToLower();

            return $"Expected {{context}} to have {GetAVisibility(visibility)} {accessorStr} accessor{{reason}}, but found {GetAccessorVisibility(accessor)}.";
        }

        private string GetNotAccessorFailMessage(string visibility, PropertyAccessor accessor)
        {
            var accessorStr = $"{accessor}".ToLower();

            return $"Expected {{context}} not to have {GetAVisibility(visibility)} {accessorStr} accessor{{reason}}, but found {GetAccessorVisibility(accessor)}.";
        }

        private static string GetAVisibility(string visibility)
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

        private string GetAccessorVisibility(PropertyAccessor accessor)
        {
            if (_subject.IsPublic(accessor))
            {
                return "it to be public";
            }

            if (_subject.IsProtected(accessor))
            {
                return "it to be protected";
            }

            if (_subject.IsPrivate(accessor))
            {
                return "it to be private";
            }

            if (_subject.IsInternal(accessor))
            {
                return "it to be internal";
            }

            return "no accessor";
        }
    }
}
