using System.Collections.Generic;

namespace PropertyNavigation.Models
{
    public class Level3
    {
        public IEnumerable<Level4a> Level4a { get; }
        public IEnumerable<Level4b> Level4b { get; }
        public IEnumerable<Level4c> Level4c { get; }
        public IEnumerable<Level4d> Level4d { get; }
        public IEnumerable<Level4e> Level4e { get; }

        public int GetValue()
        {
            return 0;
        }
    }
}