using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTMonsterConverter
{
    public interface IGenericMonster
    {
        // Properties
            // Generic
        string Name { get; set; }
        string Description { get; set; }
        uint Health { get; set; }
        uint Experience { get; set; }
        uint Speed { get; set; }
        List<string> Voices { get; set; }

            // Look
        uint CorpseId { get; set; }
        uint LookId { get; set; }
        IDetailedLookType LookTypeDetails { get; set; }

            // Behavior
        uint Summonable { get; set; }
        bool Hostile { get; set; }
        bool Illusionable { get; set; }
        uint Convinceable { get; set; }
        bool Pushable { get; set; }
        bool PushItems { get; set; }
        bool PushCreatures { get; set; }
        bool TargetDistance { get; set; }
        uint RunOnHealth { get; set; }

            // Defeneses
        uint Armor { get; set; }
        uint Fire { get; set; }
        uint Earth { get; set; }
        uint Energy { get; set; }
        uint Ice { get; set; }
        uint Holy { get; set; }
        uint Death { get; set; }
        uint Physical { get; set; }
        uint Drown { get; set; }

            // Immunities
        bool Paralyze { get; set; }
        bool Invisible { get; set; }
        bool LifeDrain { get; set; }
        bool Drunk { get; set; }
        bool ManaDrain { get; set; }
    }

    public interface IDetailedLookType
    {
        ushort Head { get; set; }
        ushort Body  { get; set; }
        ushort Legs  { get; set; }
        ushort Feet  { get; set; }
    }
}
