using AllOverIt.Patterns.Enumeration;
using System.Runtime.CompilerServices;

namespace EFEnumerationDemo.Models
{
    public sealed class PublishedStatus : EnrichedEnum<PublishedStatus>
    {
        public static readonly PublishedStatus Draft = new(0);
        public static readonly PublishedStatus InReview = new(1);
        public static readonly PublishedStatus Published = new(2);

        // Required for EF
        private PublishedStatus()
        {
        }

        private PublishedStatus(int value, [CallerMemberName] string name = "")
            : base(value, name)
        {
        }
    }
}