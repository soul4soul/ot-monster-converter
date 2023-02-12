using MonsterConverterInterface;
using MonsterConverterInterface.MonsterTypes;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.IO;
using MoonSharp.Interpreter.Loaders;
using System.Diagnostics;

namespace MonsterConverterTfsRevScriptSys
{
    [Export(typeof(IMonsterConverter))]
    public class TfsRevScriptSysConverter : MonsterConverter
    {
        MoonSharp.Interpreter.Script script = null;

        const uint MAX_LOOTCHANCE = 100000;

        IDictionary<ConditionType, string> ConditionToTfsConstant = new Dictionary<ConditionType, string>
        {
            {ConditionType.Poison,      "CONDITION_POISON"},
            {ConditionType.Fire,        "CONDITION_FIRE"},
            {ConditionType.Energy,      "CONDITION_ENERGY"},
            {ConditionType.Bleeding,    "CONDITION_BLEEDING"},
            {ConditionType.Paralyze,    "CONDITION_POISON"},
            {ConditionType.Drown,       "CONDITION_DROWN"},
            {ConditionType.Freezing,    "CONDITION_FREEZING"},
            {ConditionType.Dazzled,     "CONDITION_DAZZLED"},
            {ConditionType.Cursed,      "CONDITION_CURSED"}
        };

        IDictionary<ConditionType, string> ConditionToString = new Dictionary<ConditionType, string>
        {
            {ConditionType.Poison,      "poison"},
            {ConditionType.Fire,        "fire"},
            {ConditionType.Energy,      "energy"},
            {ConditionType.Bleeding,    "bleeding"},
            {ConditionType.Paralyze,    "poison"},
            {ConditionType.Drown,       "drown"},
            {ConditionType.Freezing,    "freezing"},
            {ConditionType.Dazzled,     "dazzled"},
            {ConditionType.Cursed,      "cursed"}
        };

        IDictionary<CombatDamage, string> CombatDamageNames = new Dictionary<CombatDamage, string>
        {
            {CombatDamage.Physical,     "COMBAT_PHYSICALDAMAGE"},
            {CombatDamage.Energy,       "COMBAT_ENERGYDAMAGE"},
            {CombatDamage.Earth,        "COMBAT_EARTHDAMAGE"},
            {CombatDamage.Fire,         "COMBAT_FIREDAMAGE"},
            {CombatDamage.LifeDrain,    "COMBAT_LIFEDRAIN"},
            {CombatDamage.ManaDrain,    "COMBAT_MANADRAIN"},
            {CombatDamage.Healing,      "COMBAT_HEALING"},
            {CombatDamage.Drown,        "COMBAT_DROWNDAMAGE"},
            {CombatDamage.Ice,          "COMBAT_ICEDAMAGE"},
            {CombatDamage.Holy,         "COMBAT_HOLYDAMAGE"},
            {CombatDamage.Death,        "COMBAT_DEATHDAMAGE"}
            //{"undefined",   CombatDamage.Undefined}
        };

        IDictionary<Effect, string> magicEffectNames = new Dictionary<Effect, string>
        {
            {Effect.None,               "CONST_ME_NONE"},
            {Effect.DrawBlood,          "CONST_ME_DRAWBLOOD"},
            {Effect.LoseEnergy,         "CONST_ME_LOSEENERGY"},
            {Effect.Poff,               "CONST_ME_POFF"},
            {Effect.BlockHit,           "CONST_ME_BLOCKHIT"},
            {Effect.ExplosionArea,      "CONST_ME_EXPLOSIONAREA"},
            {Effect.ExplosionHit,       "CONST_ME_EXPLOSIONHIT"},
            {Effect.FireArea,           "CONST_ME_FIREAREA"},
            {Effect.YellowRings,        "CONST_ME_YELLOW_RINGS"},
            {Effect.GreenRings,         "CONST_ME_GREEN_RINGS"},
            {Effect.HitArea,            "CONST_ME_HITAREA"},
            {Effect.Teleport,           "CONST_ME_TELEPORT"},
            {Effect.EnergyHit,          "CONST_ME_ENERGYHIT"},
            {Effect.MagicBlue,          "CONST_ME_MAGIC_BLUE"},
            {Effect.MagicRed,           "CONST_ME_MAGIC_RED"},
            {Effect.MagicGreen,         "CONST_ME_MAGIC_GREEN"},
            {Effect.HitByFire,          "CONST_ME_HITBYFIRE"},
            {Effect.HitByPoison,        "CONST_ME_HITBYPOISON"},
            {Effect.MortArea,           "CONST_ME_MORTAREA"},
            {Effect.SoundGreen,         "CONST_ME_SOUND_GREEN"},
            {Effect.SoundRed,           "CONST_ME_SOUND_RED"},
            {Effect.PoisonArea,         "CONST_ME_POISONAREA"},
            {Effect.SoundYellow,        "CONST_ME_SOUND_YELLOW"},
            {Effect.SoundPurple,        "CONST_ME_SOUND_PURPLE"},
            {Effect.SoundBlue,          "CONST_ME_SOUND_BLUE"},
            {Effect.SoundWhite,         "CONST_ME_SOUND_WHITE"},
            {Effect.Bubbles,            "CONST_ME_BUBBLES"},
            {Effect.Craps,              "CONST_ME_CRAPS"},
            {Effect.GiftWraps,          "CONST_ME_GIFT_WRAPS"},
            {Effect.FireworkYellow,     "CONST_ME_FIREWORK_YELLOW"},
            {Effect.FireworkRed,        "CONST_ME_FIREWORK_RED"},
            {Effect.FireworkBlue,       "CONST_ME_FIREWORK_BLUE"},
            {Effect.Stun,               "CONST_ME_STUN"},
            {Effect.Sleep,              "CONST_ME_SLEEP"},
            {Effect.WaterCreature,      "CONST_ME_WATERCREATURE"},
            {Effect.GroundShaker,       "CONST_ME_GROUNDSHAKER"},
            {Effect.Hearts,             "CONST_ME_HEARTS"},
            {Effect.FireAttack,         "CONST_ME_FIREATTACK"},
            {Effect.EnergyArea,         "CONST_ME_ENERGYAREA"},
            {Effect.SmallClouds,        "CONST_ME_SMALLCLOUDS"},
            {Effect.HolyDamage,         "CONST_ME_HOLYDAMAGE"},
            {Effect.BigClouds,          "CONST_ME_BIGCLOUDS"},
            {Effect.IceArea,            "CONST_ME_ICEAREA"},
            {Effect.IceTornado,         "CONST_ME_ICETORNADO"},
            {Effect.IceAttack,          "CONST_ME_ICEATTACK"},
            {Effect.Stones,             "CONST_ME_STONES"},
            {Effect.SmallPlants,        "CONST_ME_SMALLPLANTS"},
            {Effect.Carniphila,         "CONST_ME_CARNIPHILA"},
            {Effect.PurpleEnergy,       "CONST_ME_PURPLEENERGY"},
            {Effect.YellowEnergy,       "CONST_ME_YELLOWENERGY"},
            {Effect.HolyArea,           "CONST_ME_HOLYAREA"},
            {Effect.BigPlants,          "CONST_ME_BIGPLANTS"},
            {Effect.Cake,               "CONST_ME_CAKE"},
            {Effect.GiantIce,           "CONST_ME_GIANTICE"},
            {Effect.WaterSplash,        "CONST_ME_WATERSPLASH"},
            {Effect.PlantAttack,        "CONST_ME_PLANTATTACK"},
            {Effect.TutorialArrow,      "CONST_ME_TUTORIALARROW"},
            {Effect.TutorialSquare,     "CONST_ME_TUTORIALSQUARE"},
            {Effect.MirrorHorizontal,   "CONST_ME_MIRRORHORIZONTAL"},
            {Effect.MirrorVertical,     "CONST_ME_MIRRORVERTICAL"},
            {Effect.SkullHorizontal,    "CONST_ME_SKULLHORIZONTAL"},
            {Effect.SkullVertical,      "CONST_ME_SKULLVERTICAL"},
            {Effect.Assassin,           "CONST_ME_ASSASSIN"},
            {Effect.StepsHorizontal,    "CONST_ME_STEPSHORIZONTAL"},
            {Effect.BloodySteps,        "CONST_ME_BLOODYSTEPS"},
            {Effect.StepsVertical,      "CONST_ME_STEPSVERTICAL"},
            {Effect.YalahariGhost,      "CONST_ME_YALAHARIGHOST"},
            {Effect.Bats,               "CONST_ME_BATS"},
            {Effect.Smoke,              "CONST_ME_SMOKE"},
            {Effect.Insects,            "CONST_ME_INSECTS"},
            {Effect.Dragonhead,         "CONST_ME_DRAGONHEAD"},
            {Effect.OrcShaman,          "CONST_ME_ORCSHAMAN"},
            {Effect.OrcShamanFire,      "CONST_ME_ORCSHAMAN_FIRE"},
            {Effect.Thunder,            "CONST_ME_THUNDER"},
            {Effect.Ferumbras,          "CONST_ME_FERUMBRAS"},
            {Effect.ConfettiHorizontal, "CONST_ME_CONFETTI_HORIZONTAL"},
            {Effect.ConfettiVertical,   "CONST_ME_CONFETTI_VERTICAL"},
            {Effect.BlackSmoke,         "CONST_ME_BLACKSMOKE"},
            {Effect.RedSmoke,           "CONST_ME_REDSMOKE"},
            {Effect.YellowSmoke,        "CONST_ME_YELLOWSMOKE"},
            {Effect.GreenSmoke,         "CONST_ME_GREENSMOKE"},
            {Effect.PurpleSmoke,        "CONST_ME_PURPLESMOKE"},
            {Effect.EarlyThunder,       "CONST_ME_EARLY_THUNDER"},
            {Effect.RagiazBoneCapsule,  "CONST_ME_RAGIAZ_BONECAPSULE"},
            {Effect.CriticalDamage,     "CONST_ME_CRITICAL_DAMAGE"},
            {Effect.PlungingFish,       "CONST_ME_PLUNGING_FISH"},
            {Effect.BlueChain, "CONST_ME_BLUECHAIN"},
            {Effect.OrangeChain, "CONST_ME_ORANGECHAIN"},
            {Effect.GreenChain, "CONST_ME_GREENCHAIN"},
            {Effect.PurpleChain, "CONST_ME_PURPLECHAIN"},
            {Effect.GreyChain, "CONST_ME_GREYCHAIN"},
            {Effect.YellowChain, "CONST_ME_YELLOWCHAIN"},
            {Effect.YellowSparkles, "CONST_ME_YELLOWSPARKLES"},
            {Effect.FaeExplosion, "CONST_ME_FAEEXPLOSION"},
            {Effect.FaeComing, "CONST_ME_FAECOMING"},
            {Effect.FaeGoing, "CONST_ME_FAEGOING"},
            {Effect.BigCloudsSingleSpace, "CONST_ME_BIGCLOUDSSINGLESPACE"},
            {Effect.StonesSingleSpace, "CONST_ME_STONESSINGLESPACE"},
            {Effect.BlueGhost, "CONST_ME_BLUEGHOST"},
            {Effect.PointOfInterest, "CONST_ME_POINTOFINTEREST"},
            {Effect.MapEffect, "CONST_ME_MAPEFFECT"},
            {Effect.PointOfInterestFound, "CONST_ME_PINKSPARK"},
            {Effect.GreenFirework, "CONST_ME_FIREWORK_GREEN"},
            {Effect.OrangeFirework, "CONST_ME_FIREWORK_ORANGE"},
            {Effect.PurpleFirework, "CONST_ME_FIREWORK_PURPLE"},
            {Effect.TurquoiseFirework, "CONST_ME_FIREWORK_TURQUOISE"},
            {Effect.TheCube, "CONST_ME_THECUBE"},
            {Effect.BlackBlood, "CONST_ME_DRAWINK"},
            {Effect.PrismaticSparkles, "CONST_ME_PRISMATICSPARKLES"},
            {Effect.Thaian, "CONST_ME_THAIAN"},
            {Effect.ThaianGhost, "CONST_ME_THAIANGHOST"},
            {Effect.GhostSmoke, "CONST_ME_GHOSTSMOKE"},
            {Effect.FloatingBlock, "CONST_ME_FLOATINGBLOCK"},
            {Effect.Block, "CONST_ME_BLOCK"},
            {Effect.Rooting, "CONST_ME_ROOTING"},
            {Effect.SunPriest, "CONST_ME_SUNPRIEST"},
            {Effect.Werelion, "CONST_ME_WERELION"},
            {Effect.GhostlyScratch, "CONST_ME_GHOSTLYSCRATCH"},
            {Effect.GhostlyBite, "CONST_ME_GHOSTLYBITE"},
            {Effect.BigScratching, "CONST_ME_BIGSCRATCHING"},
            {Effect.Slash, "CONST_ME_SLASH"},
            {Effect.Bite, "CONST_ME_BITE"},
            {Effect.Challenge, "CONST_ME_CHIVALRIOUSCHALLENGE"},
            {Effect.DivineDazzle, "CONST_ME_DIVINEDAZZLE"},
            {Effect.ElectricalSpark, "CONST_ME_ELECTRICALSPARK"},
            {Effect.PurpleTeleport, "CONST_ME_PURPLETELEPORT"},
            {Effect.RedTeleport, "CONST_ME_REDTELEPORT"},
            {Effect.OrangeTeleport, "CONST_ME_ORANGETELEPORT"},
            {Effect.GreyTeleport, "CONST_ME_GREYTELEPORT"},
            {Effect.LightBlueTeleport, "CONST_ME_LIGHTBLUETELEPORT"},
            {Effect.Onslaught, "CONST_ME_FATAL"},
            {Effect.Ruse, "CONST_ME_DODGE"},
            {Effect.Momentum, "CONST_ME_HOURGLASS"},
            {Effect.FireworksStar, "CONST_ME_FIREWORKSSTAR"},
            {Effect.FireworksCircle, "CONST_ME_FIREWORKSCIRCLE"},
            {Effect.FerumbrasVessel, "CONST_ME_FERUMBRAS_1"},
            {Effect.Gazharagoth, "CONST_ME_GAZHARAGOTH"},
            {Effect.MadMage, "CONST_ME_MAD_MAGE"},
            {Effect.Horestis, "CONST_ME_HORESTIS"},
            {Effect.Devovorga, "CONST_ME_DEVOVORGA"},
            {Effect.Ferumbras2, "CONST_ME_FERUMBRAS_2"},
        };

        IDictionary<Missile, string> shootTypeNames = new Dictionary<Missile, string>
        {
            {Missile.None,             "CONST_ANI_NONE"},
            {Missile.Spear,            "CONST_ANI_SPEAR"},
            {Missile.Bolt,             "CONST_ANI_BOLT"},
            {Missile.Arrow,            "CONST_ANI_ARROW"},
            {Missile.Fire,             "CONST_ANI_FIRE"},
            {Missile.Energy,           "CONST_ANI_ENERGY"},
            {Missile.PoisonArrow,      "CONST_ANI_POISONARROW"},
            {Missile.BurstArrow,       "CONST_ANI_BURSTARROW"},
            {Missile.ThrowingStar,     "CONST_ANI_THROWINGSTAR"},
            {Missile.ThrowingKnife,    "CONST_ANI_THROWINGKNIFE"},
            {Missile.SmallStone,       "CONST_ANI_SMALLSTONE"},
            {Missile.Death,            "CONST_ANI_DEATH"},
            {Missile.LargeRock,        "CONST_ANI_LARGEROCK"},
            {Missile.Snowball,         "CONST_ANI_SNOWBALL"},
            {Missile.PowerBolt,        "CONST_ANI_POWERBOLT"},
            {Missile.Poison,           "CONST_ANI_POISON"},
            {Missile.InfernalBolt,     "CONST_ANI_INFERNALBOLT"},
            {Missile.HuntingSpear,     "CONST_ANI_HUNTINGSPEAR"},
            {Missile.EnchantedSpear,   "CONST_ANI_ENCHANTEDSPEAR"},
            {Missile.RedStar,          "CONST_ANI_REDSTAR"},
            {Missile.GreenStar,        "CONST_ANI_GREENSTAR"},
            {Missile.RoyalSpear,       "CONST_ANI_ROYALSPEAR"},
            {Missile.SniperArrow,      "CONST_ANI_SNIPERARROW"},
            {Missile.OnyxArrow,        "CONST_ANI_ONYXARROW"},
            {Missile.PiercingBolt,     "CONST_ANI_PIERCINGBOLT"},
            {Missile.WhirlwindSword,   "CONST_ANI_WHIRLWINDSWORD"},
            {Missile.WhirlwindAxe,     "CONST_ANI_WHIRLWINDAXE"},
            {Missile.WhirlwindClub,    "CONST_ANI_WHIRLWINDCLUB"},
            {Missile.EtherealSpear,    "CONST_ANI_ETHEREALSPEAR"},
            {Missile.Ice,              "CONST_ANI_ICE"},
            {Missile.Earth,            "CONST_ANI_EARTH"},
            {Missile.Holy,             "CONST_ANI_HOLY"},
            {Missile.SuddenDeath,      "CONST_ANI_SUDDENDEATH"},
            {Missile.FlashArrow,       "CONST_ANI_FLASHARROW"},
            {Missile.FlammingArrow,    "CONST_ANI_FLAMMINGARROW"},
            {Missile.ShiverArrow,      "CONST_ANI_SHIVERARROW"},
            {Missile.EnergyBall,       "CONST_ANI_ENERGYBALL"},
            {Missile.SmallIce,         "CONST_ANI_SMALLICE"},
            {Missile.SmallHoly,        "CONST_ANI_SMALLHOLY"},
            {Missile.SmallEarth,       "CONST_ANI_SMALLEARTH"},
            {Missile.EarthArrow,       "CONST_ANI_EARTHARROW"},
            {Missile.Explosion,        "CONST_ANI_EXPLOSION"},
            {Missile.Cake,             "CONST_ANI_CAKE"},
            {Missile.TarsalArrow,      "CONST_ANI_TARSALARROW"},
            {Missile.VortexBolt,       "CONST_ANI_VORTEXBOLT"},
            {Missile.PrismaticBolt,    "CONST_ANI_PRISMATICBOLT"},
            {Missile.CrystallineArrow, "CONST_ANI_CRYSTALLINEARROW"},
            {Missile.DrillBolt,        "CONST_ANI_DRILLBOLT"},
            {Missile.EnvenomedArrow,   "CONST_ANI_ENVENOMEDARROW"},
            {Missile.GloothSpear,      "CONST_ANI_GLOOTHSPEAR"},
            {Missile.SimpleArrow,      "CONST_ANI_SIMPLEARROW"},
            {Missile.LeafStar,         "CONST_ANI_LEAFSTAR"},
            {Missile.DiamondArrow,     "CONST_ANI_DIAMONDARROW"},
            {Missile.SpectralBolt,     "CONST_ANI_SPECTRALBOLT"},
            {Missile.RoyalStar,        "CONST_ANI_ROYALSTAR"},
        };

        IDictionary<Blood, string> BloodToRace = new Dictionary<Blood, string>()
        {
            { Blood.blood, "blood" },
            { Blood.venom, "venom" },
            { Blood.undead, "undead" },
            { Blood.fire, "fire" },
            { Blood.energy, "energy" }
        };

        public override string ConverterName { get => "TFS RevScriptSys"; }

        public override string FileExt { get => "lua"; }

        public override ItemIdType ItemIdType { get => ItemIdType.Server; }

        public override bool IsReadSupported { get => true; }

        public override bool IsWriteSupported { get => true; }

        public override string[] GetFilesForConversion(string directory)
        {
             // Init lua environment
            if (script == null)
            {
                UserData.RegisterType<MockTfsMonsterType>();
                UserData.RegisterType<MockTfsGame>();
                script = new MoonSharp.Interpreter.Script();
                script.Options.DebugPrint = s => { Debug.WriteLine(s); };

                script.Globals["Game"] = typeof(MockTfsGame);

                script.Globals["COMBAT_NONE"] = CombatDamage.None;
                script.Globals["COMBAT_PHYSICALDAMAGE"] = CombatDamage.Physical;
                script.Globals["COMBAT_ENERGYDAMAGE"] = CombatDamage.Energy;
                script.Globals["COMBAT_EARTHDAMAGE"] = CombatDamage.Earth;
                script.Globals["COMBAT_FIREDAMAGE"] = CombatDamage.Fire;
                //script.Globals["COMBAT_UNDEFINEDDAMAGE"] = CombatDamage.None;
                script.Globals["COMBAT_LIFEDRAIN"] = CombatDamage.LifeDrain;
                script.Globals["COMBAT_MANADRAIN"] = CombatDamage.ManaDrain;
                script.Globals["COMBAT_HEALING"] = CombatDamage.Healing;
                script.Globals["COMBAT_DROWNDAMAGE"] = CombatDamage.Drown;
                script.Globals["COMBAT_ICEDAMAGE"] = CombatDamage.Ice;
                script.Globals["COMBAT_HOLYDAMAGE"] = CombatDamage.Holy;
                script.Globals["COMBAT_DEATHDAMAGE"] = CombatDamage.Death;

                script.Globals["CONST_ME_NONE"] = Effect.None;
                script.Globals["CONST_ME_DRAWBLOOD"] = Effect.DrawBlood;
                script.Globals["CONST_ME_LOSEENERGY"] = Effect.LoseEnergy;
                script.Globals["CONST_ME_POFF"] = Effect.Poff;
                script.Globals["CONST_ME_BLOCKHIT"] = Effect.BlockHit;
                script.Globals["CONST_ME_EXPLOSIONAREA"] = Effect.ExplosionArea;
                script.Globals["CONST_ME_EXPLOSIONHIT"] = Effect.ExplosionHit;
                script.Globals["CONST_ME_FIREAREA"] = Effect.FireArea;
                script.Globals["CONST_ME_YELLOW_RINGS"] = Effect.YellowRings;
                script.Globals["CONST_ME_GREEN_RINGS"] = Effect.GreenRings;
                script.Globals["CONST_ME_HITAREA"] = Effect.HitArea;
                script.Globals["CONST_ME_TELEPORT"] = Effect.Teleport;
                script.Globals["CONST_ME_ENERGYHIT"] = Effect.EnergyHit;
                script.Globals["CONST_ME_MAGIC_BLUE"] = Effect.MagicBlue;
                script.Globals["CONST_ME_MAGIC_RED"] = Effect.MagicRed;
                script.Globals["CONST_ME_MAGIC_GREEN"] = Effect.MagicGreen;
                script.Globals["CONST_ME_HITBYFIRE"] = Effect.HitByFire;
                script.Globals["CONST_ME_HITBYPOISON"] = Effect.HitByPoison;
                script.Globals["CONST_ME_MORTAREA"] = Effect.MortArea;
                script.Globals["CONST_ME_SOUND_GREEN"] = Effect.SoundGreen;
                script.Globals["CONST_ME_SOUND_RED"] = Effect.SoundRed;
                script.Globals["CONST_ME_POISONAREA"] = Effect.PoisonArea;
                script.Globals["CONST_ME_SOUND_YELLOW"] = Effect.SoundYellow;
                script.Globals["CONST_ME_SOUND_PURPLE"] = Effect.SoundPurple;
                script.Globals["CONST_ME_SOUND_BLUE"] = Effect.SoundBlue;
                script.Globals["CONST_ME_SOUND_WHITE"] = Effect.SoundWhite;
                script.Globals["CONST_ME_BUBBLES"] = Effect.Bubbles;
                script.Globals["CONST_ME_CRAPS"] = Effect.Craps;
                script.Globals["CONST_ME_GIFT_WRAPS"] = Effect.GiftWraps;
                script.Globals["CONST_ME_FIREWORK_YELLOW"] = Effect.FireworkYellow;
                script.Globals["CONST_ME_FIREWORK_RED"] = Effect.FireworkRed;
                script.Globals["CONST_ME_FIREWORK_BLUE"] = Effect.FireworkBlue;
                script.Globals["CONST_ME_STUN"] = Effect.Stun;
                script.Globals["CONST_ME_SLEEP"] = Effect.Sleep;
                script.Globals["CONST_ME_WATERCREATURE"] = Effect.WaterCreature;
                script.Globals["CONST_ME_GROUNDSHAKER"] = Effect.GroundShaker;
                script.Globals["CONST_ME_HEARTS"] = Effect.Hearts;
                script.Globals["CONST_ME_FIREATTACK"] = Effect.FireAttack;
                script.Globals["CONST_ME_ENERGYAREA"] = Effect.EnergyArea;
                script.Globals["CONST_ME_SMALLCLOUDS"] = Effect.SmallClouds;
                script.Globals["CONST_ME_HOLYDAMAGE"] = Effect.HolyDamage;
                script.Globals["CONST_ME_BIGCLOUDS"] = Effect.BigClouds;
                script.Globals["CONST_ME_ICEAREA"] = Effect.IceArea;
                script.Globals["CONST_ME_ICETORNADO"] = Effect.IceTornado;
                script.Globals["CONST_ME_ICEATTACK"] = Effect.IceAttack;
                script.Globals["CONST_ME_STONES"] = Effect.Stones;
                script.Globals["CONST_ME_SMALLPLANTS"] = Effect.SmallPlants;
                script.Globals["CONST_ME_CARNIPHILA"] = Effect.Carniphila;
                script.Globals["CONST_ME_PURPLEENERGY"] = Effect.PurpleEnergy;
                script.Globals["CONST_ME_YELLOWENERGY"] = Effect.YellowRings;
                script.Globals["CONST_ME_HOLYAREA"] = Effect.HolyArea;
                script.Globals["CONST_ME_BIGPLANTS"] = Effect.BigPlants;
                script.Globals["CONST_ME_CAKE"] = Effect.Cake;
                script.Globals["CONST_ME_GIANTICE"] = Effect.GiantIce;
                script.Globals["CONST_ME_WATERSPLASH"] = Effect.WaterSplash;
                script.Globals["CONST_ME_PLANTATTACK"] = Effect.PlantAttack;
                script.Globals["CONST_ME_TUTORIALARROW"] = Effect.TutorialArrow;
                script.Globals["CONST_ME_TUTORIALSQUARE"] = Effect.TutorialSquare;
                script.Globals["CONST_ME_MIRRORHORIZONTAL"] = Effect.MirrorHorizontal;
                script.Globals["CONST_ME_MIRRORVERTICAL"] = Effect.MirrorVertical;
                script.Globals["CONST_ME_SKULLHORIZONTAL"] = Effect.SkullHorizontal;
                script.Globals["CONST_ME_SKULLVERTICAL"] = Effect.SkullVertical;
                script.Globals["CONST_ME_ASSASSIN"] = Effect.Assassin;
                script.Globals["CONST_ME_STEPSHORIZONTAL"] = Effect.StepsHorizontal;
                script.Globals["CONST_ME_BLOODYSTEPS"] = Effect.BloodySteps;
                script.Globals["CONST_ME_STEPSVERTICAL"] = Effect.StepsVertical;
                script.Globals["CONST_ME_YALAHARIGHOST"] = Effect.YalahariGhost;
                script.Globals["CONST_ME_BATS"] = Effect.Bats;
                script.Globals["CONST_ME_SMOKE"] = Effect.Smoke;
                script.Globals["CONST_ME_INSECTS"] = Effect.Insects;
                script.Globals["CONST_ME_DRAGONHEAD"] = Effect.Dragonhead;
                script.Globals["CONST_ME_ORCSHAMAN"] = Effect.OrcShaman;
                script.Globals["CONST_ME_ORCSHAMAN_FIRE"] = Effect.OrcShamanFire;
                script.Globals["CONST_ME_THUNDER"] = Effect.Thunder;
                script.Globals["CONST_ME_FERUMBRAS"] = Effect.Ferumbras;
                script.Globals["CONST_ME_CONFETTI_HORIZONTAL"] = Effect.ConfettiHorizontal;
                script.Globals["CONST_ME_CONFETTI_VERTICAL"] = Effect.ConfettiVertical;
                script.Globals["CONST_ME_BLACKSMOKE"] = Effect.BlackSmoke;
                script.Globals["CONST_ME_REDSMOKE"] = Effect.RedSmoke;
                script.Globals["CONST_ME_YELLOWSMOKE"] = Effect.YellowSmoke;
                script.Globals["CONST_ME_GREENSMOKE"] = Effect.GreenSmoke;
                script.Globals["CONST_ME_PURPLESMOKE"] = Effect.PurpleSmoke;
                script.Globals["CONST_ME_EARLY_THUNDER"] = Effect.EarlyThunder;
                script.Globals["CONST_ME_RAGIAZ_BONECAPSULE"] = Effect.RagiazBoneCapsule;
                script.Globals["CONST_ME_CRITICAL_DAMAGE"] = Effect.CriticalDamage;
                script.Globals["CONST_ME_PLUNGING_FISH"] = Effect.PlungingFish;
                script.Globals["CONST_ME_BLUECHAIN"] = Effect.BlueChain;
                script.Globals["CONST_ME_ORANGECHAIN"] = Effect.OrangeChain;
                script.Globals["CONST_ME_GREENCHAIN"] = Effect.GreenChain;
                script.Globals["CONST_ME_PURPLECHAIN"] = Effect.PurpleChain;
                script.Globals["CONST_ME_GREYCHAIN"] = Effect.GreyChain;
                script.Globals["CONST_ME_YELLOWCHAIN"] = Effect.YellowChain;
                script.Globals["CONST_ME_YELLOWSPARKLES"] = Effect.YellowSparkles;
                script.Globals["CONST_ME_FAEEXPLOSION"] = Effect.FaeExplosion;
                script.Globals["CONST_ME_FAECOMING"] = Effect.FaeComing;
                script.Globals["CONST_ME_FAEGOING"] = Effect.FaeGoing;
                script.Globals["CONST_ME_BIGCLOUDSSINGLESPACE"] = Effect.BigCloudsSingleSpace;
                script.Globals["CONST_ME_STONESSINGLESPACE"] = Effect.StonesSingleSpace;
                script.Globals["CONST_ME_BLUEGHOST"] = Effect.BlueGhost;
                script.Globals["CONST_ME_POINTOFINTEREST"] = Effect.PointOfInterest;
                script.Globals["CONST_ME_MAPEFFECT"] = Effect.MapEffect;
                script.Globals["CONST_ME_PINKSPARK"] = Effect.PointOfInterestFound;
                script.Globals["CONST_ME_FIREWORK_GREEN"] = Effect.GreenFirework;
                script.Globals["CONST_ME_FIREWORK_ORANGE"] = Effect.OrangeFirework;
                script.Globals["CONST_ME_FIREWORK_PURPLE"] = Effect.PurpleFirework;
                script.Globals["CONST_ME_FIREWORK_TURQUOISE"] = Effect.TurquoiseFirework;
                script.Globals["CONST_ME_THECUBE"] = Effect.TheCube;
                script.Globals["CONST_ME_DRAWINK"] = Effect.BlackBlood;
                script.Globals["CONST_ME_PRISMATICSPARKLES"] = Effect.PrismaticSparkles;
                script.Globals["CONST_ME_THAIAN"] = Effect.Thaian;
                script.Globals["CONST_ME_THAIANGHOST"] = Effect.ThaianGhost;
                script.Globals["CONST_ME_GHOSTSMOKE"] = Effect.GhostSmoke;
                script.Globals["CONST_ME_FLOATINGBLOCK"] = Effect.FloatingBlock;
                script.Globals["CONST_ME_BLOCK"] = Effect.Block;
                script.Globals["CONST_ME_ROOTING"] = Effect.Rooting;
                script.Globals["CONST_ME_GHOSTLYSCRATCH"] = Effect.GhostlyScratch;
                script.Globals["CONST_ME_GHOSTLYBITE"] = Effect.GhostlyBite;
                script.Globals["CONST_ME_BIGSCRATCHING"] = Effect.BigScratching;
                script.Globals["CONST_ME_SLASH"] = Effect.Slash;
                script.Globals["CONST_ME_BITE"] = Effect.Bite;
                script.Globals["CONST_ME_CHIVALRIOUSCHALLENGE"] = Effect.Challenge;
                script.Globals["CONST_ME_DIVINEDAZZLE"] = Effect.DivineDazzle;
                script.Globals["CONST_ME_ELECTRICALSPARK"] = Effect.ElectricalSpark;
                script.Globals["CONST_ME_PURPLETELEPORT"] = Effect.PurpleTeleport;
                script.Globals["CONST_ME_REDTELEPORT"] = Effect.RedTeleport;
                script.Globals["CONST_ME_ORANGETELEPORT"] = Effect.OrangeTeleport;
                script.Globals["CONST_ME_GREYTELEPORT"] = Effect.GreyTeleport;
                script.Globals["CONST_ME_LIGHTBLUETELEPORT"] = Effect.LightBlueTeleport;
                script.Globals["CONST_ME_FATAL"] = Effect.Onslaught;
                script.Globals["CONST_ME_DODGE"] = Effect.Ruse;
                script.Globals["CONST_ME_HOURGLASS"] = Effect.Momentum;
                script.Globals["CONST_ME_FERUMBRAS_1"] = Effect.FerumbrasVessel;
                script.Globals["CONST_ME_GAZHARAGOTH"] = Effect.Gazharagoth;
                script.Globals["CONST_ME_MAD_MAGE"] = Effect.MadMage;
                script.Globals["CONST_ME_HORESTIS"] = Effect.Horestis;
                script.Globals["CONST_ME_DEVOVORGA"] = Effect.Devovorga;
                script.Globals["CONST_ME_FERUMBRAS_2"] = Effect.Ferumbras2;

                script.Globals["CONST_ANI_SPEAR"] = Missile.Spear;
                script.Globals["CONST_ANI_BOLT"] = Missile.Bolt;
                script.Globals["CONST_ANI_ARROW"] = Missile.Arrow;
                script.Globals["CONST_ANI_FIRE"] = Missile.Fire;
                script.Globals["CONST_ANI_ENERGY"] = Missile.Energy;
                script.Globals["CONST_ANI_POISONARROW"] = Missile.PoisonArrow;
                script.Globals["CONST_ANI_BURSTARROW"] = Missile.BurstArrow;
                script.Globals["CONST_ANI_THROWINGSTAR"] = Missile.ThrowingStar;
                script.Globals["CONST_ANI_THROWINGKNIFE"] = Missile.ThrowingKnife;
                script.Globals["CONST_ANI_SMALLSTONE"] = Missile.SmallStone;
                script.Globals["CONST_ANI_DEATH"] = Missile.Death;
                script.Globals["CONST_ANI_LARGEROCK"] = Missile.LargeRock;
                script.Globals["CONST_ANI_SNOWBALL"] = Missile.Snowball;
                script.Globals["CONST_ANI_POWERBOLT"] = Missile.PowerBolt;
                script.Globals["CONST_ANI_POISON"] = Missile.Poison;
                script.Globals["CONST_ANI_INFERNALBOLT"] = Missile.InfernalBolt;
                script.Globals["CONST_ANI_HUNTINGSPEAR"] = Missile.HuntingSpear;
                script.Globals["CONST_ANI_ENCHANTEDSPEAR"] = Missile.EnchantedSpear;
                script.Globals["CONST_ANI_REDSTAR"] = Missile.RedStar;
                script.Globals["CONST_ANI_GREENSTAR"] = Missile.GreenStar;
                script.Globals["CONST_ANI_ROYALSPEAR"] = Missile.RoyalSpear;
                script.Globals["CONST_ANI_SNIPERARROW"] = Missile.SniperArrow;
                script.Globals["CONST_ANI_ONYXARROW"] = Missile.OnyxArrow;
                script.Globals["CONST_ANI_PIERCINGBOLT"] = Missile.PiercingBolt;
                script.Globals["CONST_ANI_WHIRLWINDSWORD"] = Missile.WhirlwindSword;
                script.Globals["CONST_ANI_WHIRLWINDAXE"] = Missile.WhirlwindAxe;
                script.Globals["CONST_ANI_WHIRLWINDCLUB"] = Missile.WhirlwindClub;
                script.Globals["CONST_ANI_ETHEREALSPEAR"] = Missile.EtherealSpear;
                script.Globals["CONST_ANI_ICE"] = Missile.Ice;
                script.Globals["CONST_ANI_EARTH"] = Missile.Earth;
                script.Globals["CONST_ANI_HOLY"] = Missile.Holy;
                script.Globals["CONST_ANI_SUDDENDEATH"] = Missile.SuddenDeath;
                script.Globals["CONST_ANI_FLASHARROW"] = Missile.FlashArrow;
                script.Globals["CONST_ANI_FLAMMINGARROW"] = Missile.FlammingArrow;
                script.Globals["CONST_ANI_SHIVERARROW"] = Missile.ShiverArrow;
                script.Globals["CONST_ANI_ENERGYBALL"] = Missile.EnergyBall;
                script.Globals["CONST_ANI_SMALLICE"] = Missile.SmallIce;
                script.Globals["CONST_ANI_SMALLHOLY"] = Missile.SmallHoly;
                script.Globals["CONST_ANI_SMALLEARTH"] = Missile.SmallEarth;
                script.Globals["CONST_ANI_EARTHARROW"] = Missile.EarthArrow;
                script.Globals["CONST_ANI_EXPLOSION"] = Missile.Explosion;
                script.Globals["CONST_ANI_CAKE"] = Missile.Cake;
                script.Globals["CONST_ANI_TARSALARROW"] = Missile.TarsalArrow;
                script.Globals["CONST_ANI_VORTEXBOLT"] = Missile.VortexBolt;
                script.Globals["CONST_ANI_PRISMATICBOLT"] = Missile.PrismaticBolt;
                script.Globals["CONST_ANI_CRYSTALLINEARROW"] = Missile.CrystallineArrow;
                script.Globals["CONST_ANI_DRILLBOLT"] = Missile.DrillBolt;
                script.Globals["CONST_ANI_ENVENOMEDARROW"] = Missile.EnvenomedArrow;
                script.Globals["CONST_ANI_GLOOTHSPEAR"] = Missile.GloothSpear;
                script.Globals["CONST_ANI_SIMPLEARROW"] = Missile.SimpleArrow;
                script.Globals["CONST_ANI_LEAFSTAR"] = Missile.LeafStar;
                script.Globals["CONST_ANI_DIAMONDARROW"] = Missile.DiamondArrow;
                script.Globals["CONST_ANI_SPECTRALBOLT"] = Missile.SpectralBolt;
                script.Globals["CONST_ANI_ROYALSTAR"] = Missile.RoyalStar;
            }

            return base.GetFilesForConversion(directory);
        }

        public override ConvertResultEventArgs WriteMonster(string directory, ref Monster monster)
        {
            string fileName = Path.Combine(directory, monster.FileName + "." + FileExt);
            ConvertResultEventArgs result = new ConvertResultEventArgs(fileName);
            if (monster.TargetStrategy.Random != 100)
            {
                result.AppendMessage("unsupported target strategy, only random is supported");
                result.IncreaseError(ConvertError.Warning);
            }
            if ((monster.SummonCost > 0) && (monster.ConvinceCost > 0) && (monster.SummonCost != monster.ConvinceCost))
            {
                result.AppendMessage("format doesn't support summon and coninvce mana costs being different, defaulting to highest value");
                result.IncreaseError(ConvertError.Warning);
            }
            int manaCost = monster.SummonCost;
            if (monster.ConvinceCost > manaCost)
            {
                manaCost = monster.ConvinceCost;
            }

            using (var fstream = File.OpenWrite(fileName))
            using (var dest = new StreamWriter(fstream))
            {
                fstream.SetLength(0);

                dest.WriteLine($"local mType = Game.createMonsterType(\"{monster.RegisteredName}\")");
                dest.WriteLine("local monster = {}");
                dest.WriteLine("");

                dest.WriteLine($"monster.name = \"{monster.Name}\"");
                dest.WriteLine($"monster.description = \"{monster.Description}\"");
                dest.WriteLine($"monster.experience = {monster.Experience}");
                dest.WriteLine("monster.outfit = {");
                if (monster.Look.LookType == LookType.Item)
                {
                    dest.WriteLine($"	lookTypeEx = {monster.Look.LookId}");
                }
                else if (monster.Look.LookType == LookType.Outfit)
                {
                    dest.WriteLine($"	lookType = {monster.Look.LookId},");
                    dest.WriteLine($"	lookHead = {monster.Look.Head},");
                    dest.WriteLine($"	lookBody = {monster.Look.Body},");
                    dest.WriteLine($"	lookLegs = {monster.Look.Legs},");
                    dest.WriteLine($"	lookFeet = {monster.Look.Feet},");
                    dest.WriteLine($"	lookAddons = {monster.Look.Addons},");
                    dest.WriteLine($"	lookMount = {monster.Look.Mount}");
                }
                else if (monster.Look.LookType == LookType.Invisible)
                {
                    result.IncreaseError(ConvertError.Warning);
                    result.AppendMessage("Invisible look type not supported");
                }
                dest.WriteLine("}");
                dest.WriteLine("");
                dest.WriteLine($"monster.health = {monster.Health}");
                dest.WriteLine($"monster.maxHealth = {monster.Health}");
                dest.WriteLine($"monster.runHealth = {monster.RunOnHealth}");
                dest.WriteLine($"monster.race = \"{BloodToRace[monster.Race]}\"");
                dest.WriteLine($"monster.corpse = {monster.Look.CorpseId}");
                dest.WriteLine($"monster.speed = {monster.Speed}");
                dest.WriteLine($"monster.summonCost = {manaCost}");
                dest.WriteLine("");

                dest.WriteLine("monster.changeTarget = {");
                dest.WriteLine($"	interval = {monster.RetargetInterval},");
                dest.WriteLine($"	chance = {monster.RetargetChance * 100:0}");
                dest.WriteLine("}");
                dest.WriteLine("");

                // Flags
                dest.WriteLine("monster.flags = {");
                dest.WriteLine($"	attackable = {monster.Attackable.ToString().ToLower()},");
                dest.WriteLine($"	hostile = {monster.IsHostile.ToString().ToLower()},");
                dest.WriteLine($"	summonable = {(monster.SummonCost > 0).ToString().ToLower()},");
                dest.WriteLine($"	convinceable = {(monster.ConvinceCost > 0).ToString().ToLower()},");
                dest.WriteLine($"	illusionable = {monster.IsIllusionable.ToString().ToLower()},");
                dest.WriteLine($"	boss = {monster.IsBoss.ToString().ToLower()},");
                dest.WriteLine($"	ignoreSpawnBlock = {monster.IgnoreSpawnBlock.ToString().ToLower()},");
                dest.WriteLine($"	pushable = {monster.IsPushable.ToString().ToLower()},");
                dest.WriteLine($"	canPushItems = {monster.PushItems.ToString().ToLower()},");
                dest.WriteLine($"	canPushCreatures = {monster.PushCreatures.ToString().ToLower()},");
                dest.WriteLine($"	staticAttackChance = {monster.StaticAttackChance},");
                dest.WriteLine($"	targetDistance = {monster.TargetDistance},");
                dest.WriteLine($"	healthHidden = {monster.HideHealth.ToString().ToLower()},");
                dest.WriteLine($"	canWalkOnEnergy = {(!monster.AvoidEnergy).ToString().ToLower()},");
                dest.WriteLine($"	canWalkOnFire = {(!monster.AvoidFire).ToString().ToLower()},");
                dest.WriteLine($"	canWalkOnPoison = {(!monster.AvoidPoison).ToString().ToLower()}");
                dest.WriteLine("}");
                dest.WriteLine("");

                // Light
                dest.WriteLine("monster.light = {");
                dest.WriteLine($"	level = {monster.LightLevel},");
                dest.WriteLine($"	color = {monster.LightColor}");
                dest.WriteLine("}");
                dest.WriteLine("");

                // Voices
                dest.WriteLine("monster.voices = {");
                dest.WriteLine($"	interval = {monster.VoiceInterval},");
                dest.WriteLine($"	chance = {monster.VoiceChance * 100:0},");
                for (int i = 0; i < monster.Voices.Count; i++)
                {
                    bool yell = false;
                    if (monster.Voices[i].SoundLevel == SoundLevel.Yell)
                    {
                        yell = true;
                    }
                    if (monster.Voices[i].SoundLevel == SoundLevel.Whisper)
                    {
                        result.AppendMessage("Whisper sound not supported, defaulting to say");
                        result.IncreaseError(ConvertError.Warning);
                    }
                    string voice = $"	{{text = \"{monster.Voices[i].Sound}\", yell = {yell.ToString().ToLower()}}},";
                    if (i == monster.Voices.Count - 1)
                    {
                        voice = voice.TrimEnd(',');
                    }
                    dest.WriteLine(voice);
                }
                dest.WriteLine("}");
                dest.WriteLine("");

                dest.WriteLine("monster.immunities = {");
                dest.WriteLine($"	{{type = \"paralyze\", condition = {monster.IgnoreParalyze.ToString().ToLower()}}},");
                dest.WriteLine($"	{{type = \"outfit\", condition = {monster.IgnoreOutfit.ToString().ToLower()}}},");
                dest.WriteLine($"	{{type = \"invisible\", condition = {monster.IgnoreInvisible.ToString().ToLower()}}},");
                dest.WriteLine($"	{{type = \"drunk\", condition = {monster.IgnoreDrunk.ToString().ToLower()}}},");
                dest.WriteLine($"	{{type = \"bleed\", condition = {monster.IgnoreBleed.ToString().ToLower()}}}");
                dest.WriteLine("}");
                dest.WriteLine("");

                dest.WriteLine("monster.elements = {");
                dest.WriteLine($"	{{type = {CombatDamageNames[CombatDamage.Physical]}, percent = {GenericToTfsRevScriptSysElemementPercent(monster.PhysicalDmgMod)}}},");
                dest.WriteLine($"	{{type = {CombatDamageNames[CombatDamage.Energy]}, percent = {GenericToTfsRevScriptSysElemementPercent(monster.EnergyDmgMod)}}},");
                dest.WriteLine($"	{{type = {CombatDamageNames[CombatDamage.Earth]}, percent = {GenericToTfsRevScriptSysElemementPercent(monster.EarthDmgMod)}}},");
                dest.WriteLine($"	{{type = {CombatDamageNames[CombatDamage.Fire]}, percent = {GenericToTfsRevScriptSysElemementPercent(monster.FireDmgMod)}}},");
                dest.WriteLine($"	{{type = {CombatDamageNames[CombatDamage.LifeDrain]}, percent = {GenericToTfsRevScriptSysElemementPercent(monster.LifeDrainDmgMod)}}},");
                dest.WriteLine($"	{{type = {CombatDamageNames[CombatDamage.ManaDrain]}, percent = {GenericToTfsRevScriptSysElemementPercent(monster.ManaDrainDmgMod)}}},");
                dest.WriteLine($"	{{type = {CombatDamageNames[CombatDamage.Drown]}, percent = {GenericToTfsRevScriptSysElemementPercent(monster.DrownDmgMod)}}},");
                dest.WriteLine($"	{{type = {CombatDamageNames[CombatDamage.Ice]}, percent = {GenericToTfsRevScriptSysElemementPercent(monster.IceDmgMod)}}},");
                dest.WriteLine($"	{{type = {CombatDamageNames[CombatDamage.Holy]} , percent = {GenericToTfsRevScriptSysElemementPercent(monster.HolyDmgMod)}}},");
                dest.WriteLine($"	{{type = {CombatDamageNames[CombatDamage.Death]} , percent = {GenericToTfsRevScriptSysElemementPercent(monster.DeathDmgMod)}}}");
                if (monster.HealingMod != 1)
                {
                    result.IncreaseError(ConvertError.Warning);
                    result.AppendMessage("Can't convert unsupported healing combat modifier");
                    //dest.WriteLine($"	{{type = { CombatDamageNames[CombatDamage.Healing]}, percent = {GenericToTfsElemementPercent(monster.Healing)}}},");
                }

                dest.WriteLine("}");
                dest.WriteLine("");

                // abilities
                IList<string> attacks = new List<string>();
                IList<string> defenses = new List<string>();
                foreach (var spell in monster.Attacks)
                {
                    var revSpell = GenericToTfsRevScriptSysSpells(spell);
                    if (revSpell.Item1 != ConvertError.Success)
                    {
                        result.IncreaseError(revSpell.Item1);
                        result.AppendMessage(revSpell.Item2);
                        continue;
                    }

                    if (spell.SpellCategory == SpellCategory.Offensive)
                    {
                        attacks.Add(revSpell.Item2);
                    }
                    else
                    {
                        defenses.Add(revSpell.Item2);
                    }
                }

                // Write offensive
                dest.WriteLine("monster.attacks = {");
                for (int i = 0; i < attacks.Count; i++)
                {
                    if (i == attacks.Count - 1)
                    {
                        dest.WriteLine($"{attacks[i]}");
                    }
                    else
                    {
                        dest.WriteLine($"{attacks[i]},");
                    }
                }
                dest.WriteLine("}");
                dest.WriteLine("");

                // Write Defensive
                dest.WriteLine("monster.defenses = {");
                dest.WriteLine($"	defense = {monster.Shielding},");
                if (defenses.Count > 0)
                {
                    dest.WriteLine($"	armor = {monster.TotalArmor},");
                }
                else
                {
                    dest.WriteLine($"	armor = {monster.TotalArmor}");
                }

                for (int i = 0; i < defenses.Count; i++)
                {
                    if (i == defenses.Count - 1)
                    {
                        dest.WriteLine($"{defenses[i]}");
                    }
                    else
                    {
                        dest.WriteLine($"{defenses[i]},");
                    }
                }
                dest.WriteLine("}");
                dest.WriteLine("");

                // Summons
                if (monster.Summons.Count > 0)
                {
                    dest.WriteLine($"monster.maxSummons = {monster.MaxSummons}");
                    dest.WriteLine("monster.summons = {");
                    string summon;
                    for (int i = 0; i < monster.Summons.Count; i++)
                    {
                        summon = $"	{{name = \"{monster.Summons[i].Name}\", chance = {monster.Summons[i].Chance * 100:0}, interval = {monster.Summons[i].Interval}";
                        if (monster.Summons[i].Max > 0)
                        {
                            summon += $", max = {monster.Summons[i].Max}";
                        }

                        if (monster.Summons[i].Force)
                        {
                            summon += $", force = {monster.Summons[i].Force.ToString().ToLower()}";
                        }
                        summon += "}";

                        if (i == monster.Summons.Count - 1)
                        {
                            summon = summon.TrimEnd(',');
                        }
                        else
                        {
                            summon += ",";
                        }
                        dest.WriteLine(summon);
                    }
                    dest.WriteLine("}");
                    dest.WriteLine("");
                }

                if (monster.Scripts.Count(s => s.Type != ScriptType.OnDeath) > 0)
                {
                    result.IncreaseError(ConvertError.Warning);
                    result.AppendMessage("Unable to convert scripts");
                }
                var writableEvents = monster.Scripts.Where(s => s.Type == ScriptType.OnDeath).ToList();
                if (writableEvents.Count > 0)
                {
                    dest.WriteLine("monster.events = {");
                    for (int i = 0; i < writableEvents.Count; i++)
                    {
                        if (i == writableEvents.Count - 1)
                        {
                            dest.WriteLine($"	\"{writableEvents[i].Name}\"");
                        }
                        else
                        {
                            dest.WriteLine($"	\"{writableEvents[i].Name}\",");
                        }
                    }
                    dest.WriteLine("}");
                    dest.WriteLine("");
                }

                // Loot
                dest.WriteLine("monster.loot = {");
                if (monster.Items.Count > 0)
                {
                    dest.WriteLine(LootListToRevScriptSysFormat(monster.Items));
                }
                dest.WriteLine("}");
                dest.WriteLine("");

                dest.WriteLine("mType:register(monster)");
            }

            return result;
        }

        private static string LootListToRevScriptSysFormat(IList<LootItem> items, int tabDepth = 1)
        {
            string output = "";
            for (int i = 0; i < items.Count; i++)
            {
                string loot = LootItemToRevScriptSysFormat(items[i], tabDepth);
                if (i != items.Count - 1)
                {
                    loot += $",{Environment.NewLine}";
                }
                output += loot;
            }
            return output;
        }

        private static string LootItemToRevScriptSysFormat(LootItem loot, int tabDepth)
        {
            string tabIndent = string.Concat(Enumerable.Repeat("\t", tabDepth));
            string rssLootLine;

            string itemQuoted;
            if (loot.Id > 0)
            {
                itemQuoted = loot.Id.ToString();
            }
            else
            {
                itemQuoted = $"\"{loot.Name}\"";
            }

            decimal chance = loot.Chance * MAX_LOOTCHANCE;
            rssLootLine = $"{tabIndent}{{id = {itemQuoted}, chance = {chance:0}";

            if (loot.Count > 1)
            {
                rssLootLine += $", maxCount = {loot.Count}";
            }

            if (loot.SubType > 0)
            {
                rssLootLine += $", subType = {loot.SubType}";
            }

            if (loot.ActionId > 0)
            {
                rssLootLine += $", actionId = {loot.ActionId}";
            }

            if (!string.IsNullOrWhiteSpace(loot.Text))
            {
                rssLootLine += $", text = {loot.Text}";
            }

            if (!string.IsNullOrWhiteSpace(loot.Description))
            {
                rssLootLine += $", description2 = \"{loot.Description}\"";
            }

            if (loot.NestedLoot.Count > 0)
            {
                rssLootLine += $", child = {{{Environment.NewLine}";
                rssLootLine += LootListToRevScriptSysFormat(loot.NestedLoot, tabDepth + 2);
                rssLootLine += $"{Environment.NewLine}{tabIndent}\t}}{Environment.NewLine}{tabIndent}";
            }

            rssLootLine += "}";

            return rssLootLine;
        }

        public override ConvertResultEventArgs ReadMonster(string filename, out Monster monster)
        {
            MockTfsGame.ConvertedMonsters.Clear();

            script.DoFile(filename);

            if (MockTfsGame.ConvertedMonsters.TryDequeue(out var result))
            {
                if (MockTfsGame.ConvertedMonsters.Count >= 1)
                {
                    monster = null;
                    return new ConvertResultEventArgs(filename, ConvertError.Error, "Unable to convert multiple monsters from the same file");
                }
                else
                {
                    monster = result.Item1;
                    result.Item2.File = filename;
                    return result.Item2;
                }
            }
            else
            {
                monster = null;
                return new ConvertResultEventArgs(filename, ConvertError.Error, "No monster data found within file");
            }
        }

        double GenericToTfsRevScriptSysElemementPercent(double percent)
        {
            double value = (1 - percent) * 100;
            return Math.Round(value);
        }

        /// <summary>
        /// Converts an ability from a generic format to TFS rev script system spell format
        /// </summary>
        /// <param name="spell"></param>
        /// <returns>ConvertError, attack in string format OR error message</returns>
        public Tuple<ConvertError, string> GenericToTfsRevScriptSysSpells(Spell spell)
        {
            ConvertError error = ConvertError.Success;
            string attack;
            if (spell.DefinitionStyle == SpellDefinition.TfsLuaScript)
            {
                attack = $"	{{script =\"{spell.Name}\", interval = {spell.Interval}, chance = {spell.Chance * 100:0}";

                if ((spell.MinDamage != null) && (spell.MaxDamage != null))
                {
                    attack += $", minDamage = {spell.MinDamage}, maxDamage = {spell.MaxDamage}";
                }
                else if (spell.MaxDamage != null)
                {
                    attack += $", maxDamage = {spell.MaxDamage}";
                }
                if (spell.OnTarget != null)
                {
                    attack += $", target = { spell.OnTarget.ToString().ToLower()}";
                }
                if (spell.IsDirectional != null)
                {
                    attack += $", direction = { spell.IsDirectional.ToString().ToLower()}";
                }
                attack += "}";
            }
            else if (spell.DefinitionStyle == SpellDefinition.Raw)
            {
                attack = $"	{{name =\"{spell.Name}\", interval = {spell.Interval}, chance = {spell.Chance * 100:0}";

                if (spell.Name == "melee")
                {
                    if ((spell.MinDamage != null) && (spell.MaxDamage != null))
                    {
                        attack += $", minDamage = {spell.MinDamage}, maxDamage = {spell.MaxDamage}";
                    }
                    else if (spell.MaxDamage != null)
                    {
                        attack += $", maxDamage = {spell.MaxDamage}";
                    }
                    else if ((spell.AttackValue != null) && (spell.Skill != null))
                    {
                        attack += $", skill = {spell.Skill}, attack = {spell.AttackValue}";
                    }
                    //else continue which we should never hit?

                    if (spell.Condition != ConditionType.None)
                    {
                        attack += $", condition = {{type = {ConditionToTfsConstant[spell.Condition]}, startDamage = {spell.StartDamage}, interval = {spell.Tick}}}";
                    }
                }
                else
                {
                    if (spell.Name == "strength")
                    {
                        error = ConvertError.Warning;
                        attack = $"Can't convert abilitiy name {spell} with flags {spell.Strengths} range {spell.MinSkillChange}-{spell.MaxSkillChange}";
                        return new(error, attack);
                    }
                    if (spell.Name == "speed")
                    {
                        attack += $", speed = {{min = {spell.MinSpeedChange}, max = {spell.MaxSpeedChange}}}";
                    }
                    else if (spell.Name == "condition")
                    {
                        attack += $", type = {ConditionToTfsConstant[spell.Condition]}, startDamage = {spell.StartDamage}, tick = {spell.Tick}";
                    }
                    else if (spell.Name == "outfit")
                    {
                        if (!string.IsNullOrEmpty(spell.MonsterName))
                        {
                            attack += $", monster = \"{spell.MonsterName}\"";
                        }
                        else if (spell.ItemId != null)
                        {
                            attack += $", item = {spell.ItemId}";
                        }
                    }
                    else if ((spell.Name == "combat") && (spell.DamageElement != CombatDamage.None))
                    {
                        attack += $", type = {CombatDamageNames[spell.DamageElement]}";
                    }
                    else if (spell.Name == "drunk")
                    {
                        attack += $", drunkenness = {spell.Drunkenness * 100:0}";
                    }

                    if ((spell.MinDamage != null) && (spell.MaxDamage != null))
                    {
                        attack += $", minDamage = {spell.MinDamage}, maxDamage = {spell.MaxDamage}";
                    }
                    else if (spell.MaxDamage != null)
                    {
                        attack += $", minDamage = {spell.MinDamage}";
                    }
                    if (spell.Duration != null)
                    {
                        attack += $", duration = {spell.Duration}";
                    }
                    if (spell.Range != null)
                    {
                        attack += $", range = {spell.Range}";
                    }
                    if (spell.Radius != null)
                    {
                        attack += $", radius = {spell.Radius}, target = {spell.OnTarget.ToString().ToLower()}";
                    }
                    if (spell.Ring != null)
                    {
                        attack += $", ring = {spell.Ring}, target = {spell.OnTarget.ToString().ToLower()}";
                    }
                    if (spell.Length != null)
                    {
                        attack += $", length = {spell.Length}";
                    }
                    if (spell.Spread != null)
                    {
                        attack += $", spread = {spell.Spread}";
                    }
                    if (spell.ShootEffect != Missile.None)
                    {
                        attack += $", shootEffect = {shootTypeNames[spell.ShootEffect]}";
                    }
                    if (spell.AreaEffect != Effect.None)
                    {
                        attack += $", effect = {magicEffectNames[spell.AreaEffect]}";
                    }
                }
                attack += "}";
            }
            else
            {
                error = ConvertError.Warning;
                attack = $"Can't convert abilitiy name {spell.Name} with DefinitionStyles {spell.DefinitionStyle}";
            }

            return new(error, attack);
        }
    }
}
