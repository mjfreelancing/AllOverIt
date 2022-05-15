using AllOverIt.Patterns.Enumeration;
using System.Runtime.CompilerServices;

namespace KeysetPaginationConsole.Models
{
    public sealed class PostRating : EnrichedEnum<PostRating>
    {
        public static readonly PostRating Terrible = new(0);
        public static readonly PostRating Decent = new(1);
        public static readonly PostRating Awesome = new(2);

        // Required for EF
        private PostRating()
        {
        }

        private PostRating(int value, [CallerMemberName] string name = null)
            : base(value, name)
        {
        }
    }
}