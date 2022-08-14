using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterConverterInterface.MonsterTypes
{
    [Flags]
    public enum StrengthSkills
    {
        None      = 0,
        Fist      = 1 << 0,
        Club      = 1 << 1,
        Sword     = 1 << 2,
        Axe       = 1 << 3,
        Distance  = 1 << 4,
        Magic     = 1 << 5,
        Shielding = 1 << 6,
        Melee     = Fist | Club | Sword | Axe,
    }
}
