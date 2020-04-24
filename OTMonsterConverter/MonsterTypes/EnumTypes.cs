using System;
using System.Collections.Generic;
using System.Text;

namespace OTMonsterConverter.MonsterTypes
{
    public enum Condition
    {
        None,
        Poison,
        Fire,
        Energy,
        Bleeding,
        Haste,
        Paralyze,
        Outfit,
        Invisible,
        Light,
        ManaShield,
        InFight,
        Drunk,
        ExhaustWeapon, // unused
        Regeneration,
        Soul,
        Drown,
        Muted,
        ChannelMutedTicks,
        YellTicks,
        Attributes,
        Freezing,
        Dazzled,
        Cursed,
        ExhaustCombat, // unused
        Exhaust_heal, // unused
        Pacified,
        SpellCoolDown,
        SpellGroupCoolDown
    }

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
        None = 0,
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
        FireworkBlue,
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
        GiantIce,
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
        PlungingFish = 175
    }

    public enum Animation
    {
        None = 0,
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
        Snowball,
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
        SuddenDeath,
        FlashArrow,
        FlammingArrow,
        ShiverArrow,
        EnergyBall,
        SmallIce,
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
        SimpleArrow
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
}
