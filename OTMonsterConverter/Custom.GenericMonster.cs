using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTMonsterConverter
{
    public enum CombatDamage
    {
        Physical,
        Energy,
        Earth,
        Fire,
        LifeDrain,
        ManaDrain,
        Healing,
        Drown,
        Ice,
        Holy,
        Death
    }

    public enum TargetType
    {
        Direction,
        Self,
        Area
    }

    public enum Effect
    {
        DrawBlood,
        LoseEnergy,
        Poff,
        BlockHit,
        ExplosionArea,
        ExplosionHit,
        FireArea,
        YellowRings,
        GreenRings,
        HitArea,
        Teleport,
        EnergyHit,
        MagicBlue,
        MagicRed,
        MagicGreen,
        HitByFire,
        HitByPoison,
        MortArea,
        SoundGreen,
        SoundRed,
        PoisonArea,
        SoundYellow,
        SoundPurple,
        SoundBlue,
        SoundWhite,
        Bubbles,
        Craps,
        GiftWraps,
        FireworkYellow,
        FireworkRed,
        FireworkClue,
        Stun,
        Sleep,
        WaterCreature,
        GroundShaker,
        Hearts,
        FireAttack,
        EnergyArea,
        SmallClouds,
        HolyDamage,
        BigClouds,
        IceArea,
        IceTornado,
        IceAttack,
        Stones,
        SmallPlants,
        Carniphila,
        PurpleEnergy,
        YellowEnergy,
        HolyArea,
        BigPlants,
        Cake,
        Giantice,
        WaterSplash,
        PlantAttack,
        TutorialArrow,
        TutorialSquare,
        MirrorHorizontal,
        MirrorVertical,
        SkullHorizontal,
        SkullVertical,
        Assassin,
        StepsHorizontal,
        BloodySteps,
        StepsVertical,
        YalahariGhost,
        Bats,
        Smoke,
        Insects,
        Dragonhead,
        OrcShaman,
        OrcShamanFire,
        Thunder,
        Ferumbras,
        ConfettiHorizontal,
        ConfettiVertical,
        // 77-157 are empty
        BlackSmoke = 158,
        // 159-166 are empty
        RedSmoke = 167,
        YellowSmoke = 168,
        GreenSmoke = 169,
        PurpleSmoke = 170,
        EarlyThunder = 171,
        RagiazBoneCapsule = 172,
        CriticalDamage = 173,
        // 174 is empty
        PlungingFish = 175,
        None = 0
    }

    public enum Animation
    {
        Spear,
        Bolt,
        Arrow,
        Fire,
        Energy,
        PoisonArrow,
        BurstArrow,
        ThrowingStar,
        ThrowingKnife,
        SmallStone,
        Death,
        LargeRock,
        SnowBall,
        PowerBolt,
        Poison,
        InfernalBolt,
        HuntingSpear,
        EnchantedSpear,
        RedStar,
        GreenStar,
        RoyalSpear,
        SniperArrow,
        OnyxArrow,
        PiercingBolt,
        WhirlwindSword,
        WhirlwindAxe,
        WhirlwindClub,
        EtherealSpear,
        Ice,
        Earth,
        Holy,
        SuddenSeath,
        FlashArrow,
        FlammingArrow,
        ShiverArrow,
        EnergyBall,
        SmallEce,
        SmallHoly,
        SmallEarth,
        EarthArrow,
        Explosion,
        Cake,
        TarsalArrow = 44,
        VortexBolt = 45,
        PrismaticBolt,
        CrystallineArrow,
        DrillBolt,
        EnvenomedArrow,
        GloothSpear,
        SimpleArrow,
        None = 0
    }

    public enum Blood
    {
        blood,
        venom,
        undead,
        fire,
        energy
    }

    public enum SoundLevel
    {
        Whisper,
        Say,
        Yell
    }

    //todo should we add outfit ID to this class?
    public class DetailedLookType : IDetailedLookType
    {
        // Variables
        private const ushort MAX_COLOR = 132; //todo is this correct?

        private ushort _Head;
        private ushort _Body;
        private ushort _Legs;
        private ushort _Feet;

        // Properties
        public ushort Head
        {
            get { return _Head; }
            set { _Head = (value > MAX_COLOR) ? MAX_COLOR : value; }
        }
        public ushort Body
        {
            get { return _Body; }
            set { _Body = (value > MAX_COLOR) ? MAX_COLOR : value; }
        }
        public ushort Legs
        {
            get { return _Legs; }
            set { _Legs = (value > MAX_COLOR) ? MAX_COLOR : value; }
        }
        public ushort Feet
        {
            get { return _Feet; }
            set { _Feet = (value > MAX_COLOR) ? MAX_COLOR : value; }
        }
        public ushort Addons { get; set; }
        public ushort Mount { get; set; }
    }

    public class CustomSummon : ICustomSummon
    {
        public string Name { get; set; }
        public uint Rate { get; set; }
        public double Chance { get; set; }
    }

    public class CustomVoice : ICustomVoice
    {
        public string Sound { get; set; }
        public SoundLevel SoundLevel { get; set; }
    }

    public class Loot : ILoot
    {
        public string Item { get; set; }
        public decimal Chance { get; set; }
        public decimal Count { get; set; }
    }

    public class Spells : ISpells
    {
        public string Name { get; set; }
        public uint MinDamage { get; set; }
        public uint MaxDamage { get; set; }
        public TargetType TargetStyle { get; set; }
        public CombatDamage DamageElement { get; set; }
        public Effect AreaEffect { get; set; }
        public Animation ShootEffect { get; set; }
        public uint Chance { get; set; }
        public uint Interval { get; set; }
        public uint Range { get; set; }
    }

    public class CustomMonster : ICustomMonster
    {
        // Member Variables

        // Constructors
        public CustomMonster()
        {
            Voices = new List<ICustomVoice>();
            MaxSummons = 0;
            Summons = new List<ICustomSummon>();
            Items = new List<ILoot>();
            LookTypeDetails = new DetailedLookType();
            Attacks = new List<ISpells>();

            SummonCost     = 0;
            Attackable     = true;
            Hostile        = true;
            Illusionable   = false;
            ConvinceCost   = 0;
            Pushable       = false;
            PushItems      = false;
            PushCreatures  = false;
            TargetDistance = 1;
            StaticAttack   = 95;
            LightLevel     = 0;
            LightColor     = 0;
            RunOnHealth    = 0;
            HideHealth     = false;
            AvoidFire      = true;
            AvoidEnergy    = true;
            AvoidPoison    = true;

            // Immunities
            IgnoreParalyze  = false;
            IgnoreInvisible = false;
            IgnoreDrunk     = false;
            IgnoreOutfit    = false;

            // Defences
            TotalArmor = 10;
            Shielding  = 5;

            // Elements
            Fire      = 1;
            Earth     = 1;
            Energy    = 1;
            Ice       = 1;
            Holy      = 1;
            Death     = 1;
            Physical  = 1;
            Drown     = 1;
            LifeDrain = 1;
            ManaDrain = 1;
        }

        // Events

        // Properties
        // Generic
        public string Name { get; set; }
        public string Description { get; set; }
        public uint Health { get; set; }
        public uint Experience { get; set; }
        public uint Speed { get; set; }
        public IList<ICustomVoice> Voices { get; set; }
        public Blood Race { get; set; }
        public uint ManaCost { get; set; }
        public uint RetargetInterval { get; set; }
        public uint RetargetChance { get; set; }
        public uint MaxSummons { get; set; }
        public IList<ICustomSummon> Summons { get; set; }

            // Look
        public uint CorpseId { get; set; }
        public uint OutfitIdLookType { get; set; }
        public uint ItemIdLookType { get; set; } // none 0 means creature looks like an item
        public IDetailedLookType LookTypeDetails { get; set; }

            // Behavior
        public uint SummonCost { get; set; }
        public bool Attackable { get; set; }
        public bool Hostile { get; set; }
        public bool Illusionable { get; set; }
        public uint ConvinceCost { get; set; }
        public bool Pushable { get; set; }
        public bool PushItems { get; set; }
        public bool PushCreatures { get; set; }
        public uint TargetDistance { get; set; }
        public uint RunOnHealth { get; set; }
        public uint StaticAttack { get; set; }
        public uint LightLevel { get; set; }
        public uint LightColor { get; set; }
        public bool HideHealth { get; set; }


        // Walk Behavior
        public bool AvoidFire { get; set; }
        public bool AvoidEnergy { get; set; }
        public bool AvoidPoison { get; set; }

        // Attacks
        public IList<ISpells> Attacks { get; set; }


        // Defeneses
        public uint TotalArmor { get; set; }
        public uint Shielding { get; set; }
        public double Fire { get; set; }
        public double Earth { get; set; }
        public double Energy { get; set; }
        public double Ice { get; set; }
        public double Holy { get; set; }
        public double Death { get; set; }
        public double Physical { get; set; }
        public double Drown { get; set; }
        public double LifeDrain { get; set; }
        public double ManaDrain { get; set; }

            // Immunities
        public bool IgnoreParalyze { get; set; }
        public bool IgnoreInvisible { get; set; }
        public bool IgnoreDrunk { get; set; }
        public bool IgnoreOutfit { get; set; }
        public bool IgnoreBleed { get; set; }

            // Loot
        public IList<ILoot> Items { get; set; }
    }
}
