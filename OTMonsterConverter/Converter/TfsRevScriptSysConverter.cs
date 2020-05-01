using OTMonsterConverter.MonsterTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace OTMonsterConverter.Converter
{
    public class TfsRevScriptSysConverter : IMonsterConverter
    {
        const uint MAX_LOOTCHANCE = 100000;

        IDictionary<Condition, string> ConditioToTFsConstants = new Dictionary<Condition, string>
        {
            {Condition.Poison,      "CONDITION_POISON"},
            {Condition.Fire,        "CONDITION_FIRE"},
            {Condition.Energy,      "CONDITION_ENERGY"},
            {Condition.Bleeding,    "CONDITION_BLEEDING"},
            {Condition.Paralyze,    "CONDITION_POISON"},
            {Condition.Drown,       "CONDITION_DROWN"},
            {Condition.Freezing,    "CONDITION_FREEZING"},
            {Condition.Dazzled,     "CONDITION_DAZZLED"},
            {Condition.Cursed,      "CONDITION_CURSED"}
        };

        IDictionary<Condition, string> ConditionToString = new Dictionary<Condition, string>
        {
            {Condition.Poison,      "poison"},
            {Condition.Fire,        "fire"},
            {Condition.Energy,      "energy"},
            {Condition.Bleeding,    "bleeding"},
            {Condition.Paralyze,    "poison"},
            {Condition.Drown,       "drown"},
            {Condition.Freezing,    "freezing"},
            {Condition.Dazzled,     "dazzled"},
            {Condition.Cursed,      "cursed"}
        };

        IDictionary<CombatDamage, string> CombatDamageNames = new Dictionary<CombatDamage, string>
        {
            {CombatDamage.Physical,     "COMBAT_PHYSICAL"},
            {CombatDamage.Energy,       "COMBAT_ENERGY"},
            {CombatDamage.Earth,        "COMBAT_EARTH"},
            {CombatDamage.Fire,         "COMBAT_FIRE"},
            {CombatDamage.LifeDrain,    "COMBAT_LIFEDRAIN"},
            {CombatDamage.ManaDrain,    "COMBAT_MANADRAIN"},
            {CombatDamage.Healing,      "COMBAT_HEALING"},
            {CombatDamage.Drown,        "COMBAT_DROWN"},
            {CombatDamage.Ice,          "COMBAT_ICE"},
            {CombatDamage.Holy,         "COMBAT_HOLY"},
            {CombatDamage.Death,        "COMBAT_DEATH"}
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

        IDictionary<Animation, string> shootTypeNames = new Dictionary<Animation, string>
        {
            {Animation.None,             "CONST_ANI_NONE"},
            {Animation.Spear,            "CONST_ANI_SPEAR"},
            {Animation.Bolt,             "CONST_ANI_BOLT"},
            {Animation.Arrow,            "CONST_ANI_ARROW"},
            {Animation.Fire,             "CONST_ANI_FIRE"},
            {Animation.Energy,           "CONST_ANI_ENERGY"},
            {Animation.PoisonArrow,      "CONST_ANI_POISONARROW"},
            {Animation.BurstArrow,       "CONST_ANI_BURSTARROW"},
            {Animation.ThrowingStar,     "CONST_ANI_THROWINGSTAR"},
            {Animation.ThrowingKnife,    "CONST_ANI_THROWINGKNIFE"},
            {Animation.SmallStone,       "CONST_ANI_SMALLSTONE"},
            {Animation.Death,            "CONST_ANI_DEATH"},
            {Animation.LargeRock,        "CONST_ANI_LARGEROCK"},
            {Animation.Snowball,         "CONST_ANI_SNOWBALL"},
            {Animation.PowerBolt,        "CONST_ANI_POWERBOLT"},
            {Animation.Poison,           "CONST_ANI_POISON"},
            {Animation.InfernalBolt,     "CONST_ANI_INFERNALBOLT"},
            {Animation.HuntingSpear,     "CONST_ANI_HUNTINGSPEAR"},
            {Animation.EnchantedSpear,   "CONST_ANI_ENCHANTEDSPEAR"},
            {Animation.RedStar,          "CONST_ANI_REDSTAR"},
            {Animation.GreenStar,        "CONST_ANI_GREENSTAR"},
            {Animation.RoyalSpear,       "CONST_ANI_ROYALSPEAR"},
            {Animation.SniperArrow,      "CONST_ANI_SNIPERARROW"},
            {Animation.OnyxArrow,        "CONST_ANI_ONYXARROW"},
            {Animation.PiercingBolt,     "CONST_ANI_PIERCINGBOLT"},
            {Animation.WhirlwindSword,   "CONST_ANI_WHIRLWINDSWORD"},
            {Animation.WhirlwindAxe,     "CONST_ANI_WHIRLWINDAXE"},
            {Animation.WhirlwindClub,    "CONST_ANI_WHIRLWINDCLUB"},
            {Animation.EtherealSpear,    "CONST_ANI_ETHEREALSPEAR"},
            {Animation.Ice,              "CONST_ANI_ICE"},
            {Animation.Earth,            "CONST_ANI_EARTH"},
            {Animation.Holy,             "CONST_ANI_HOLY"},
            {Animation.SuddenDeath,      "CONST_ANI_SUDDENDEATH"},
            {Animation.FlashArrow,       "CONST_ANI_FLASHARROW"},
            {Animation.FlammingArrow,    "CONST_ANI_FLAMMINGARROW"},
            {Animation.ShiverArrow,      "CONST_ANI_SHIVERARROW"},
            {Animation.EnergyBall,       "CONST_ANI_ENERGYBALL"},
            {Animation.SmallIce,         "CONST_ANI_SMALLICE"},
            {Animation.SmallHoly,        "CONST_ANI_SMALLHOLY"},
            {Animation.SmallEarth,       "CONST_ANI_SMALLEARTH"},
            {Animation.EarthArrow,       "CONST_ANI_EARTHARROW"},
            {Animation.Explosion,        "CONST_ANI_EXPLOSION"},
            {Animation.Cake,             "CONST_ANI_CAKE"},
            {Animation.TarsalArrow,      "CONST_ANI_TARSALARROW"},
            {Animation.VortexBolt,       "CONST_ANI_VORTEXBOLT"},
            {Animation.PrismaticBolt,    "CONST_ANI_PRISMATICBOLT"},
            {Animation.CrystallineArrow, "CONST_ANI_CRYSTALLINEARROW"},
            {Animation.DrillBolt,        "CONST_ANI_DRILLBOLT"},
            {Animation.EnvenomedArrow,   "CONST_ANI_ENVENOMEDARROW"},
            {Animation.GloothSpear,      "CONST_ANI_GLOOTHSPEAR"},
            {Animation.SimpleArrow,      "CONST_ANI_SIMPLEARROW"},
        };

        public string FileExtRegEx { get => "*.lua"; }

        public bool WriteMonster(string directory, ref Monster monster)
        {
            string fileName = Path.Combine(directory, monster.FileName + ".lua");

            using (var fstream = File.OpenWrite(fileName))
            using (var dest = new StreamWriter(fstream))
            {
                fstream.SetLength(0);

                dest.WriteLine($"local mType = Game.createMonsterType(\"{monster.Name}\")");
                dest.WriteLine("local monster = {}");
                dest.WriteLine("");
                //"monster.eventFile = true -- will try to load the file example.lua in data/scripts/monsters/events",
                //"monster.eventFile = "test" -- will try to load the file test.lua in data/scripts/monsters/events",
                dest.WriteLine($"monster.description = \"{monster.Description}\"");
                dest.WriteLine($"monster.experience = {monster.Experience}");
                dest.WriteLine("monster.outfit = {");
                if (monster.ItemIdLookType != 0)
                {
                    dest.WriteLine($"	lookTypeEx = {monster.ItemIdLookType}");
                }
                else
                {
                    dest.WriteLine($"	lookType = {monster.OutfitIdLookType},");
                    dest.WriteLine($"	lookHead = {monster.LookTypeDetails.Head},");
                    dest.WriteLine($"	lookBody = {monster.LookTypeDetails.Body},");
                    dest.WriteLine($"	lookLegs = {monster.LookTypeDetails.Legs},");
                    dest.WriteLine($"	lookFeet = {monster.LookTypeDetails.Feet},");
                    dest.WriteLine($"	lookAddons = {monster.LookTypeDetails.Addons},");
                    dest.WriteLine($"	lookMount = {monster.LookTypeDetails.Mount}");
                }
                dest.WriteLine("}");
                dest.WriteLine("");
                dest.WriteLine($"monster.health = {monster.Health}");
                dest.WriteLine($"monster.maxHealth = {monster.Health}");
                dest.WriteLine($"monster.race = \"{monster.Race}\""); // TODO check if mapping is neeeded
                dest.WriteLine($"monster.corpse = {monster.CorpseId}");
                dest.WriteLine($"monster.speed = {monster.Speed}");
                dest.WriteLine($"monster.summonCost = {monster.SummonCost}");
                dest.WriteLine($"monster.maxSummons = {monster.MaxSummons}");
                dest.WriteLine("");

                dest.WriteLine("monster.changeTarget = {");
                dest.WriteLine($"	interval = {monster.RetargetInterval},");
                dest.WriteLine($"	chance = {monster.RetargetChance}"); // Todo 20 = 20%, don't use 0.2 for 20%
                dest.WriteLine("}");
                dest.WriteLine("");

                // Flags
                dest.WriteLine("monster.flags = {");
                if (monster.SummonCost > 0)
                {
                    dest.WriteLine("	isSummonable = true,");
                }
                else
                {
                    dest.WriteLine("	isSummonable = false,");
                }
                dest.WriteLine($"	isAttackable = true,");
                dest.WriteLine($"	isHostile = {monster.Hostile.ToString().ToLower()},");
                if (monster.ConvinceCost > 0)
                {
                    dest.WriteLine($"	isConvinceable = true,");
                }
                else
                {
                    dest.WriteLine($"	isConvinceable = false,");
                }
                dest.WriteLine($"	isPushable = {monster.Pushable.ToString().ToLower()},");
                dest.WriteLine($"	isBoss = {monster.IsBoss.ToString().ToLower()},");
                dest.WriteLine($"	illusionable = {monster.Illusionable.ToString().ToLower()},");
                dest.WriteLine($"	canPushItems = {monster.PushItems.ToString().ToLower()},");
                dest.WriteLine($"	canPushCreatures = {monster.PushCreatures.ToString().ToLower()},");
                dest.WriteLine($"	staticAttackChance = {monster.StaticAttack},");
                dest.WriteLine($"	targetdistance = {monster.TargetDistance},");
                dest.WriteLine($"	runHealth = {monster.RunOnHealth},");
                dest.WriteLine($"	isHealthHidden = {monster.HideHealth.ToString().ToLower()},");
                bool canwalk = !monster.AvoidEnergy;
                dest.WriteLine($"	canwalkonenergy = {canwalk.ToString().ToLower()},");
                canwalk = !monster.AvoidEnergy;
                dest.WriteLine($"	canwalkonfire = {canwalk.ToString().ToLower()},");
                canwalk = !monster.AvoidPoison;
                dest.WriteLine($"	canwalkonpoison = {canwalk.ToString().ToLower()}");
                dest.WriteLine("}");
                dest.WriteLine("");

                // Light
                dest.WriteLine("monster.light = {");
                dest.WriteLine($"	level = {monster.LightLevel},");
                dest.WriteLine($"	color = {monster.LightColor}");
                dest.WriteLine("}");
                dest.WriteLine("");

                // Summons
                if (monster.Summons.Count > 0)
                {
                    dest.WriteLine("monster.summons = {");
                    string summon;
                    for (int i = 0; i < monster.Summons.Count; i++)
                    {
                        summon = $"	{{name = \"{monster.Summons[i].Name}\", chance = {monster.Summons[i].Chance * 100}, interval = {monster.Summons[i].Rate}";
                        if (monster.Summons[i].Max > 0) {
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
                        } else {
                            summon += ",";
                        }
                        dest.WriteLine(summon);
                    }
                    dest.WriteLine("}");
                    dest.WriteLine("");
                }

                // Voices
                dest.WriteLine("monster.voices = {");
                dest.WriteLine("	interval = 5000,");
                dest.WriteLine("	chance = 10,");
                string voice;
                for (int i = 0; i < monster.Voices.Count; i++)
                {
                    bool yell = false;
                    if (monster.Voices[i].SoundLevel == SoundLevel.Yell)
                    {
                        yell = true;
                    }
                    voice = $"	{{text = \"{monster.Voices[i].Sound}\", yell = {yell.ToString().ToLower()}}},";
                    if (i == monster.Voices.Count - 1)
                    {
                        voice = voice.TrimEnd(',');
                    }
                    dest.WriteLine(voice);
                }
                dest.WriteLine("}");
                dest.WriteLine("");

                // Loot
                dest.WriteLine("monster.loot = {");
                string loot;
                for (int i = 0; i < monster.Items.Count; i++)
                {
                    string item;
                    if (int.TryParse(monster.Items[i].Item, out int itemid))
                    {
                        item = itemid.ToString();
                    }
                    else
                    {
                        item = $"\"{monster.Items[i].Item}\"";
                    }

                    decimal chance = monster.Items[i].Chance * MAX_LOOTCHANCE;
                    if (monster.Items[i].Count > 1)
                    {
                        loot = $"	{{id = {item}, chance = {chance:0}, maxCount = {monster.Items[i].Count}}},";
                    }
                    else
                    {
                        loot = $"	{{id = {item}, chance = {chance:0}}},";
                    }
                    if (i == monster.Items.Count - 1)
                    {
                        loot = loot.TrimEnd(',');
                    }
                    dest.WriteLine(loot);
                }
                dest.WriteLine("}");
                dest.WriteLine("");

                dest.WriteLine("monster.attacks = {");
                string attack = string.Empty;
                for (int i = 0; i < monster.Attacks.Count; i++)
                {
                    Spell spell = monster.Attacks[i];

                    if (spell.Name == "melee")
                    {
                        attack = $"	{{name =\"combat\", interval = {spell.Interval}, chance = {spell.Chance}";

                        if ((spell.MinDamage != null) && (spell.MaxDamage != null))
                        {
                            attack += $", minDamage = {spell.MinDamage}, maxDamage = {spell.MaxDamage}";
                        }
                        else if (spell.MaxDamage != null)
                        {
                            attack += $", minDamage = {spell.MinDamage}";
                        }
                        else if ((spell.AttackValue != null) && (spell.Skill != null))
                        {
                            attack += $", skill = {spell.Skill}, attack = {spell.AttackValue}";
                        }
                        //else continue which we should never hit?

                        attack += $", effect = {magicEffectNames[Effect.DrawBlood]}";

                        // Conditions only ever appear on melee damage?
                        if (spell.Condition != null)
                        {
                            attack += $", condition = {{type = {ConditioToTFsConstants[(Condition)spell.Condition]}, startDamage = {spell.StartDamage}, interval = {spell.Tick}}}";
                        }
                    }
                    else if (spell.Name == "speed")
                    {
                        attack = $"	{{name =\"{spell.Name}\", interval = {spell.Interval}, chance = {spell.Chance}";

                        if ((spell.SpeedChange != null) && (spell.Duration != null))
                        {
                            attack += $", SpeedChange = {spell.SpeedChange}, Duration = {spell.Duration}";
                        }
                    }
                    else if (spell.Name.Contains("invisible"))
                    {
                        attack = $"	{{name =\"{spell.Name}\", interval = {spell.Interval}, chance = {spell.Chance}";
                        if (spell.AreaEffect != null)
                        {
                            attack += $", effect = {magicEffectNames[(Effect)spell.AreaEffect]}";
                        }
                    }
                    else
                    {
                        if (spell.Name.Contains("field") ||
                            spell.Name.Contains("drunk") ||
                            spell.Name.Contains("outfit"))
                        {
                            attack = $"	{{name =\"{spell.Name}\", interval = {spell.Interval}, chance = {spell.Chance}";
                        }
                        else if (spell.Name.Contains("condition"))
                        {
                            string condition = spell.Name.Replace("condition", "");
                            attack = $"	{{name =\"{condition}\", interval = {spell.Interval}, chance = {spell.Chance}";
                        }
                        else
                        {
                            attack = $"	{{name =\"combat\", interval = {spell.Interval}, chance = {spell.Chance}";
                        }
                        if (spell.Name != "outfit")
                        {
                            if ((spell.MinDamage != null) && (spell.MaxDamage != null))
                            {
                                attack += $", minDamage = {spell.MinDamage}, maxDamage = {spell.MaxDamage}";
                            }
                            else if (spell.MaxDamage != null)
                            {
                                attack += $", minDamage = {spell.MinDamage}";
                            }
                            if (spell.DamageElement != null)
                            {
                                attack += $", type = {CombatDamageNames[(CombatDamage)spell.DamageElement]}";
                            }
                            if (spell.Range != null)
                            {
                                attack += $", range = {spell.Range}";
                            }
                            if (spell.Radius != null)
                            {
                                attack += $", radius = {spell.Radius}";
                            }
                            if (spell.Length != null)
                            {
                                attack += $", length = {spell.Length}";
                            }
                            if (spell.Spread != null)
                            {
                                attack += $", spread = {spell.Spread}";
                            }
                            if (spell.ShootEffect != null)
                            {
                                attack += $", ShootEffect = {shootTypeNames[(Animation)spell.ShootEffect]}";
                            }
                            if (spell.AreaEffect != null)
                            {
                                attack += $", effect = {magicEffectNames[(Effect)spell.AreaEffect]}";
                            }
                            if (spell.Target != null)
                            {
                                attack += $", target = {spell.Target.ToString().ToLower()}";
                            }
                            if (spell.Duration != null)
                            {
                                attack += $", duration = {spell.Duration}";
                            }
                            if (spell.Name.Contains("condition"))
                            {
                                // Not implemented?
                                if (spell.Tick != null)
                                {
                                    attack += $", tick = {spell.Tick}";
                                }
                                // Not implemented?
                                if (spell.StartDamage != null)
                                {
                                    attack += $", start = {spell.StartDamage}";
                                }
                            }
                            if (spell.Name == "outfit")
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
                        }
                    }
                    attack += "},";

                    if (i == monster.Attacks.Count - 1)
                    {
                        attack = attack.TrimEnd(',');
                    }
                    dest.WriteLine(attack);
                }
                dest.WriteLine("}");
                dest.WriteLine("");

                dest.WriteLine("monster.defenses = {");
                dest.WriteLine($"	defense = {monster.Shielding},");
                dest.WriteLine($"	armor = {monster.TotalArmor}");
                //"	{name = "combat", type = COMBAT_HEALING, chance = 15, interval = 2*1000, minDamage = 180, maxDamage = 250, effect = ","CONST_ME_MAGIC_BLUE},",
                //"	{name = "speed", chance = 15, interval = 2*1000, speed = 320, effect = CONST_ME_MAGIC_RED}",
                dest.WriteLine("}");
                dest.WriteLine("");

                dest.WriteLine("monster.elements = {");
                dest.WriteLine($"	{{type = COMBAT_PHYSICALDAMAGE, percent = {GenericToTfsRevScriptSysElemementPercent(monster.Physical)}}},");
                dest.WriteLine($"	{{type = COMBAT_ENERGYDAMAGE, percent = {GenericToTfsRevScriptSysElemementPercent(monster.Energy)}}},");
                dest.WriteLine($"	{{type = COMBAT_EARTHDAMAGE, percent = {GenericToTfsRevScriptSysElemementPercent(monster.Earth)}}},");
                dest.WriteLine($"	{{type = COMBAT_FIREDAMAGE, percent = {GenericToTfsRevScriptSysElemementPercent(monster.Fire)}}},");
                dest.WriteLine($"	{{type = COMBAT_LIFEDRAIN, percent = {GenericToTfsRevScriptSysElemementPercent(monster.LifeDrain)}}},");
                dest.WriteLine($"	{{type = COMBAT_MANADRAIN, percent = {GenericToTfsRevScriptSysElemementPercent(monster.ManaDrain)}}},");
                //dest.WriteLine($"	{{type = COMBAT_HEALING, percent = {GenericToTfsElemementPercent(monster.XXXX)}}},");
                dest.WriteLine($"	{{type = COMBAT_DROWNDAMAGE, percent = {GenericToTfsRevScriptSysElemementPercent(monster.Drown)}}},");
                dest.WriteLine($"	{{type = COMBAT_ICEDAMAGE, percent = {GenericToTfsRevScriptSysElemementPercent(monster.Ice)}}},");
                dest.WriteLine($"	{{type = COMBAT_HOLYDAMAGE , percent = {GenericToTfsRevScriptSysElemementPercent(monster.Holy)}}},");
                dest.WriteLine($"	{{type = COMBAT_DEATHDAMAGE , percent = {GenericToTfsRevScriptSysElemementPercent(monster.Death)}}}");
                dest.WriteLine("}");
                dest.WriteLine("");

                dest.WriteLine("monster.immunities = {");
                dest.WriteLine($"	{{type = \"paralyze\", condition = {monster.IgnoreParalyze.ToString().ToLower()}}},");
                dest.WriteLine($"	{{type = \"outfit\", condition = {monster.IgnoreOutfit.ToString().ToLower()}}},");
                dest.WriteLine($"	{{type = \"invisible\", condition = {monster.IgnoreInvisible.ToString().ToLower()}}},");
                dest.WriteLine($"	{{type = \"bleed\", condition = {monster.IgnoreBleed.ToString().ToLower()}}}");
                dest.WriteLine("}");
                dest.WriteLine("");

                dest.WriteLine("mType:register(monster)");
            }

            return true;
        }

        public bool ReadMonster(string filename, out Monster monster)
        {
            throw new NotImplementedException();
        }

        double GenericToTfsRevScriptSysElemementPercent(double percent)
        {
            double value = (1 - percent) * 100;
            return Math.Round(value);
        }
    }
}
