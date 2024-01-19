using System.Collections.Generic;

namespace PropertyNavigationDemo.Models
{
    public class Level2
    {
        public IEnumerable<Level3> Level3 { get; }
    }
}