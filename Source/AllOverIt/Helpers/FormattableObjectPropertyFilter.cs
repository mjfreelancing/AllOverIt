namespace AllOverIt.Helpers
{
    public abstract class FormattableObjectPropertyFilter : ObjectPropertyFilter, IFormattableObjectPropertyFilter
    {
        public virtual string OnFormatValue(string value)
        {
            return value;
        }
    }
}