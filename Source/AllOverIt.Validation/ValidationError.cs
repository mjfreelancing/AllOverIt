using AllOverIt.Helpers;

namespace AllOverIt.Validation
{
    public sealed class ValidationError
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string PropertyName { get; set; }
        public string AttemptedValue { get; set; }

        public ValidationError(ValidationErrorCode code, string message, string property, object attemptedValue)
        {
            Code = $"{code}";
            Message = message.WhenNotNull(nameof(message));
            PropertyName = property.WhenNotNullOrEmpty(nameof(property));
            AttemptedValue = $"{attemptedValue}";
        }
    }
}