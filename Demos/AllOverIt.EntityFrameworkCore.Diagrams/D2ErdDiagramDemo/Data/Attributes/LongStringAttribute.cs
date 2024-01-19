using System.ComponentModel.DataAnnotations;

namespace D2ErdDiagramDemo.Data.Attributes
{
    public sealed class LongStringAttribute : MaxLengthAttribute
    {
        public LongStringAttribute()
            : base(1024)
        {
        }
    }
}