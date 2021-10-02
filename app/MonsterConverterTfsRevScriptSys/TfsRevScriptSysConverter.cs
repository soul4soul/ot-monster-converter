using MonsterConverterInterface;
using MonsterConverterInterface.MonsterTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.IO;

namespace MonsterConverterTfsRevScriptSys
{
    [Export(typeof(IMonsterConverter))]
    public class TfsRevScriptSysConverter : MonsterConverter
    {
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
            {Effect.PlungingFish,       "CONST_ME_PLUNGING_FISH"}
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

        public override bool IsReadSupported { get => false; }

        public override bool IsWriteSupported { get => true; }

        public override ConvertResultEventArgs WriteMonster(string directory, ref Monster monster)
        {
            string fileName = Path.Combine(directory, monster.FileName + "." + FileExt);
            ConvertResultEventArgs result = new ConvertResultEventArgs(fileName);

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
                dest.WriteLine($"monster.summonCost = {monster.SummonCost}");
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
                rssLootLine += $", description = \"{loot.Description}\"";
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
            throw new NotImplementedException();
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
                        attack += $", ShootEffect = {shootTypeNames[spell.ShootEffect]}";
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
