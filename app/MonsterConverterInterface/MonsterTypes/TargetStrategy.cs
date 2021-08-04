using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterConverterInterface.MonsterTypes
{
    public class TargetStrategy
    {
        public int Closest { get; set; }
        public int Weakest { get; set; }
        public int Strongest { get; set; }
        public int Random { get; set; } = 100;
    }
}
