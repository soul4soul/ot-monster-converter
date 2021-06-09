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
    /// Defining "TfsLuaScript" here is not extensible. Defining just "LuaScript" is not flexible enough becuase other engines
    /// such as otserv-br could technically have a different lua interface that is not compatible. Since this program has
    /// limited scope and there are such few engine types defining the enum like this should be fine.
    /// </summary>
    public enum SpellDefinition
    {
        Raw, // Write in a raw parsable format that is easy to convert between different monster formats
        TfsLuaScript // Spell is defined in an external lua script, spell like this can't be parsed from TFS XML to pyOT
    }
}
