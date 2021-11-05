using AllOverIt.Patterns.Enumeration;
using System.Runtime.CompilerServices;

namespace EFEnumerationDemo.Models
{
    public sealed class BlogStatus : EnrichedEnum<BlogStatus>
    {
        public static readonly BlogStatus Submitted = new(0);
        public static readonly BlogStatus Scheduled = new(1);
        public static readonly BlogStatus Progressing = new(2);
        public static readonly BlogStatus Review = new(3);
        public static readonly BlogStatus Approved = new(4);

        // Required for EF
        private BlogStatus()
        {
        }

        private BlogStatus(int value, [CallerMemberName] string name = null)
            : base(value, name!.ToUpper())
        {
        }
    }
}