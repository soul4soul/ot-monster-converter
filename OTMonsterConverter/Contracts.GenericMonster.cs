﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTMonsterConverter
{
    public interface ICustomMonster
    {
        // Properties
            // Generic
        string Name { get; set; }
        string Description { get; set; }
        uint Health { get; set; }
        uint Experience { get; set; }
        uint Speed { get; set; }
        List<string> Voices { get; set; }
        Blood Race { get; set; }
        uint retargetChance { get; set; }

            // Look
        uint CorpseId { get; set; }
        uint OutfitIdLookType { get; set; }
        uint ItemIdLookType { get; set; }
        IDetailedLookType LookTypeDetails { get; set; }

            // Behavior
        uint SummonCost { get; set; }
        bool Hostile { get; set; }
        bool Illusionable { get; set; }
        uint ConvinceCost { get; set; }
        bool Pushable { get; set; }
        bool PushItems { get; set; }
        bool PushCreatures { get; set; }
        uint TargetDistance { get; set; }
        uint RunOnHealth { get; set; }

            // Walk Behavior
        bool avoidFire { get; set; }
        bool avoidEnergy { get; set; }
        bool avoidPoison { get; set; }

            // Immunities Behavior
        bool IgnoreParalyze { get; set; }
        bool IgnoreInvisible { get; set; }
        bool IgnoreDrunk { get; set; }
        bool IgnoreOutfit { get; set; }

            // Defeneses
        uint TotalArmor { get; set; } // equal to what would be a players total armor
        uint Shielding { get; set; }  // equal to what would be the shield of a player
        double Fire { get; set; }
        double Earth { get; set; }
        double Energy { get; set; }
        double Ice { get; set; }
        double Holy { get; set; }
        double Death { get; set; }
        double Physical { get; set; }
        double Drown { get; set; }
        double LifeDrain { get; set; }
        double ManaDrain { get; set; }
    }

    public interface IDetailedLookType
    {
        ushort Head { get; set; }
        ushort Body  { get; set; }
        ushort Legs  { get; set; }
        ushort Feet  { get; set; }
    }

    public interface ICustomSummon
    {
        string Name { get; set; }
        ushort Rate { get; set; }
        ushort Chance { get; set; }
    }
}
