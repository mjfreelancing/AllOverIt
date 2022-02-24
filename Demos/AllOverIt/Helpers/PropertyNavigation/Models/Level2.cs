using System.Collections.Generic;

namespace PropertyNavigation.Models
{
    public class Level2
    {
        public IEnumerable<Level3> Level3 { get; }
    }
}