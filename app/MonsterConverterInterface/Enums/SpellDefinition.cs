using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterConverterInterface.MonsterTypes
{
    /// <summary>
    /// This enum is is used to identify how spells are written
    ///
    /// Ideally the plugins would define their spell definition but using an enum here is easier.
    /// Since this program has limited scope and there are such few engine types defining the enum in the interface is fine.
    /// </summary>
    public enum SpellDefinition
    {
        /// <summary>
        /// Spell is defined in a raw parsable format that is easy to convert between different monster formats
        /// </summary>
        Raw,
        /// <summary>
        /// Spell is defined in an external lua script, spell like this can't be parsed from TFS XML to pyOT
        /// </summary>
        TfsLuaScript
    }
}
