using System.ComponentModel.DataAnnotations;

namespace AllOverIt.EntityFrameworkCore.Diagrams.Tests.TestTypes.Attributes
{
    public sealed class LongStringAttribute : MaxLengthAttribute
    {
        public LongStringAttribute()
            : base(1024)
        {
        }
    }
}