﻿using System;
using System.Runtime.Serialization;

namespace AllOverIt.Exceptions
{
    /// <summary>Represents a validation error that occurred while initializing a <see cref="ValueObject"/>.</summary>
    [Serializable]
    public sealed class ValueObjectValidationException : Exception
    {
        public object AttemptedValue { get; }

        /// <summary>Default constructor.</summary>
        public ValueObjectValidationException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public ValueObjectValidationException(string message)
            : base(message)
        {
        }

        public ValueObjectValidationException(object attemptedValue, string message)
            : base(message)
        {
            AttemptedValue = attemptedValue;
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ValueObjectValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(AttemptedValue), AttemptedValue);

            base.GetObjectData(info, context);
        }

        private ValueObjectValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            AttemptedValue = (object) info.GetValue(nameof(AttemptedValue), typeof(object));
        }
    }
}