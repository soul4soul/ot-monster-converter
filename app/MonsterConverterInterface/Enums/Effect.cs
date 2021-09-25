﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MonsterConverterInterface.MonsterTypes
{
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
        StepsHorizontal, // TFS compat
        BloodyHandsHorizontal = StepsHorizontal,
        BloodySteps,
        StepsVertical, // TFS compat
        BloodyHandsVertical = StepsVertical,
        YalahariGhost,
        Bats,
        Smoke,
        Insects,
        Dragonhead,
        OrcShaman,
        OrcShamanFire,
        Thunder, // Has whitelight
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
        EarlyThunder = 171, // Has bluelight
        RagiazBoneCapsule = 172,
        CriticalDamage = 173,
        // 174 is empty
        PlungingFish = 175,
        BlueChain,
        OrangeChain,
        GreenChain,
        PurpleChain,
        GreyChain,
        YellowChain,
        YellowSparkles,
        FaeExplosion = 184,
        FaeComing,
        FaeGoing,
        BigCloudsSingleSpace = 188,
        StonesSingleSpace,
        BlueGhost = 191,
        PointOfInterest = 193,
        MapEffect,
        GreenConfetti,
        OrangeConfetti,
        PurpleConfetti,
        TurquoiseConfetti,
        TheCube = 201,
        BlackBlood,
        PrismaticSparkles,
        Thaian,
        ThaianGhost,
        GhostSmoke,
        FloatingBlock = 208,
        Block,
        Rooting,
        SunPriest,
        Werelion,
        GhostlyScratch,
        GhostlyBite,
        BigScratching,
        Slash,
        Bite,
        Challenge = 219,
        DivineDazzle,
        ElectricalSpark,
        PurpleTeleport,
        RedTeleport,
        OrangeTeleport,
        GreyTeleport,
        LightBlueTeleport
    }
}
