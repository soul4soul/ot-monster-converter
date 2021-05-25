using MonsterConverterInterface;
using MonsterConverterInterface.MonsterTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace MonsterConverterTfsXml
{
    //https://github.com/otland/forgottenserver/blob/master/src/monsters.cpp
    [Export(typeof(IMonsterConverter))]
    public class TfsXmlConverter : MonsterConverter
    {
        public override string ConverterName { get => "TFS XML"; }

        const uint MAX_LOOTCHANCE = 100000;
        const uint ATTACK_INTERVAL_DEFAULT = 2000;

        private readonly IDictionary<string, Effect> magicEffectNames = new Dictionary<string, Effect>
        {
            {"redspark",            Effect.DrawBlood},
            {"bluebubble",          Effect.LoseEnergy},
            {"poff",                Effect.Poff},
            {"yellowspark",         Effect.BlockHit},
            {"explosionarea",       Effect.ExplosionArea},
            {"explosion",           Effect.ExplosionHit},
            {"firearea",            Effect.FireArea},
            {"yellowbubble",        Effect.YellowRings},
            {"greenbubble",         Effect.GreenRings},
            {"blackspark",          Effect.HitArea},
            {"teleport",            Effect.Teleport},
            {"energy",              Effect.EnergyHit},
            {"blueshimmer",         Effect.MagicBlue},
            {"redshimmer",          Effect.MagicRed},
            {"greenshimmer",        Effect.MagicGreen},
            {"fire",                Effect.HitByFire},
            {"greenspark",          Effect.HitByPoison},
            {"mortarea",            Effect.MortArea},
            {"greennote",           Effect.SoundGreen},
            {"rednote",             Effect.SoundRed},
            {"poison",              Effect.PoisonArea},
            {"yellownote",          Effect.SoundYellow},
            {"purplenote",          Effect.SoundPurple},
            {"bluenote",            Effect.SoundBlue},
            {"whitenote",           Effect.SoundWhite},
            {"bubbles",             Effect.Bubbles},
            {"dice",                Effect.Craps},
            {"giftwraps",           Effect.GiftWraps},
            {"yellowfirework",      Effect.FireworkYellow},
            {"redfirework",         Effect.FireworkRed},
            {"bluefirework",        Effect.FireworkBlue},
            {"stun",                Effect.Stun},
            {"sleep",               Effect.Sleep},
            {"watercreature",       Effect.WaterCreature},
            {"groundshaker",        Effect.GroundShaker},
            {"hearts",              Effect.Hearts},
            {"fireattack",          Effect.FireAttack},
            {"energyarea",          Effect.EnergyArea},
            {"smallclouds",         Effect.SmallClouds},
            {"holydamage",          Effect.HolyDamage},
            {"bigclouds",           Effect.BigClouds},
            {"icearea",             Effect.IceArea},
            {"icetornado",          Effect.IceTornado},
            {"iceattack",           Effect.IceAttack},
            {"stones",              Effect.Stones},
            {"smallplants",         Effect.SmallPlants},
            {"carniphila",          Effect.Carniphila},
            {"purpleenergy",        Effect.PurpleEnergy},
            {"yellowenergy",        Effect.YellowEnergy},
            {"holyarea",            Effect.HolyArea},
            {"bigplants",           Effect.BigPlants},
            {"cake",                Effect.Cake},
            {"giantice",            Effect.GiantIce},
            {"watersplash",         Effect.WaterSplash},
            {"plantattack",         Effect.PlantAttack},
            {"tutorialarrow",       Effect.TutorialArrow},
            {"tutorialsquare",      Effect.TutorialSquare},
            {"mirrorhorizontal",    Effect.MirrorHorizontal},
            {"mirrorvertical",      Effect.MirrorVertical},
            {"skullhorizontal",     Effect.SkullHorizontal},
            {"skullvertical",       Effect.SkullVertical},
            {"assassin",            Effect.Assassin},
            {"stepshorizontal",     Effect.StepsHorizontal},
            {"bloodysteps",         Effect.BloodySteps},
            {"stepsvertical",       Effect.StepsVertical},
            {"yalaharighost",       Effect.YalahariGhost},
            {"bats",                Effect.Bats},
            {"smoke",               Effect.Smoke},
            {"insects",             Effect.Insects},
            {"dragonhead",          Effect.Dragonhead},
            {"orcshaman",           Effect.OrcShaman},
            {"orcshamanfire",       Effect.OrcShamanFire},
            {"thunder",             Effect.Thunder},
            {"ferumbras",           Effect.Ferumbras},
            {"confettihorizontal",  Effect.ConfettiHorizontal},
            {"confettivertical",    Effect.ConfettiVertical},
            {"blacksmoke",          Effect.BlackSmoke},
            {"redsmoke",            Effect.RedSmoke},
            {"yellowsmoke",         Effect.YellowSmoke},
            {"greensmoke",          Effect.GreenSmoke},
            {"purplesmoke",         Effect.PurpleSmoke}
        };

        private readonly IDictionary<string, Animation> shootTypeNames = new Dictionary<string, Animation>
        {
            {"spear",               Animation.Spear},
            {"bolt",                Animation.Bolt},
            {"arrow",               Animation.Arrow},
            {"fire",                Animation.Fire},
            {"energy",              Animation.Energy},
            {"poisonarrow",         Animation.PoisonArrow},
            {"burstarrow",          Animation.BurstArrow},
            {"throwingstar",        Animation.ThrowingStar},
            {"throwingknife",       Animation.ThrowingKnife},
            {"smallstone",          Animation.SmallStone},
            {"death",               Animation.Death},
            {"largerock",           Animation.LargeRock},
            {"snowball",            Animation.Snowball},
            {"powerbolt",           Animation.PowerBolt},
            {"poison",              Animation.Poison},
            {"infernalbolt",        Animation.InfernalBolt},
            {"huntingspear",        Animation.HuntingSpear},
            {"enchantedspear",      Animation.EnchantedSpear},
            {"redstar",             Animation.RedStar},
            {"greenstar",           Animation.GreenStar},
            {"royalspear",          Animation.RoyalSpear},
            {"sniperarrow",         Animation.SniperArrow},
            {"onyxarrow",           Animation.OnyxArrow},
            {"piercingbolt",        Animation.PiercingBolt},
            {"whirlwindsword",      Animation.WhirlwindSword},
            {"whirlwindaxe",        Animation.WhirlwindAxe},
            {"whirlwindclub",       Animation.WhirlwindClub},
            {"etherealspear",       Animation.EtherealSpear},
            {"ice",                 Animation.Ice},
            {"earth",               Animation.Earth},
            {"holy",                Animation.Holy},
            {"suddendeath",         Animation.SuddenDeath},
            {"flasharrow",          Animation.FlashArrow},
            {"flammingarrow",       Animation.FlammingArrow},
            {"shiverarrow",         Animation.ShiverArrow},
            {"energyball",          Animation.EnergyBall},
            {"smallice",            Animation.SmallIce},
            {"smallholy",           Animation.SmallHoly},
            {"smallearth",          Animation.SmallEarth},
            {"eartharrow",          Animation.EarthArrow},
            {"explosion",           Animation.Explosion},
            {"cake",                Animation.Cake},
            {"tarsalarrow",         Animation.TarsalArrow},
            {"vortexbolt",          Animation.VortexBolt},
            {"prismaticbolt",       Animation.PrismaticBolt},
            {"crystallinearrow",    Animation.CrystallineArrow},
            {"drillbolt",           Animation.DrillBolt},
            {"envenomedarrow",      Animation.EnvenomedArrow},
            {"gloothspear",         Animation.GloothSpear},
            {"simplearrow",         Animation.SimpleArrow}
        };

        private readonly IDictionary<string, CombatDamage> combatDamageNames = new Dictionary<string, CombatDamage>
        {
            {"physical",    CombatDamage.Physical},
            {"energy",      CombatDamage.Energy},
            {"earth",       CombatDamage.Earth},
            {"fire",        CombatDamage.Fire},
            {"lifedrain",   CombatDamage.LifeDrain},
            {"manadrain",   CombatDamage.ManaDrain},
            {"healing",     CombatDamage.Healing},
            {"drown",       CombatDamage.Drown},
            {"ice",         CombatDamage.Ice},
            {"holy",        CombatDamage.Holy},
            {"death",       CombatDamage.Death}
            //{"undefined",   CombatDamage.Undefined}
        };

        private readonly IDictionary<string, ConditionType> conditionDamageNames = new Dictionary<string, ConditionType>
        {
            {"physicalcondition",    ConditionType.Bleeding},
            {"bleedcondition",       ConditionType.Bleeding},
            {"energycondition",      ConditionType.Energy},
            {"poisoncondition",      ConditionType.Poison},
            {"earthcondition",       ConditionType.Poison},
            {"firecondition",        ConditionType.Fire},
            {"drowncondition",       ConditionType.Drown},
            {"icecondition",         ConditionType.Freezing},
            {"freezecondition",      ConditionType.Freezing},
            {"holycondition",        ConditionType.Dazzled},
            {"dazzledcondition",     ConditionType.Dazzled},
            {"cursecondition",       ConditionType.Cursed},
            {"deathcondition",       ConditionType.Cursed}
        };

        private readonly IDictionary<ConditionType, int> conditionDefaultTick = new Dictionary<ConditionType, int>
        {
            {ConditionType.Bleeding,    4000},
            {ConditionType.Energy,      10000},
            {ConditionType.Fire,        9000},
            {ConditionType.Poison,      4000},
            {ConditionType.Drown,       5000},
            {ConditionType.Freezing,    8000},
            {ConditionType.Dazzled,     10000},
            {ConditionType.Cursed,      4000},
        };

        public override string FileExt { get => "xml"; }

        public override bool IsReadSupported { get => true; }

        public override bool IsWriteSupported { get => false; }

        // Functions
        public override bool ReadMonster(string filename, out Monster monster)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TFSXmlMonster));

            serializer.UnknownNode += new XmlNodeEventHandler(Serializer_UnknownNode);
            serializer.UnknownAttribute += new XmlAttributeEventHandler(Serializer_UnknownAttribute);

            // A FileStream is needed to read the XML document.
            FileStream fs = new FileStream(filename, FileMode.Open);

            // Use the Deserialize method to restore the object's state with data from the XML document.
            TFSXmlMonster tfsMonster = (TFSXmlMonster)serializer.Deserialize(fs);

            // convert from xml monster classes to generic class
            xmlToGeneric(tfsMonster, out monster);
            monster.FileName = Path.GetFileNameWithoutExtension(filename);

            return true;
        }

        public override bool WriteMonster(string directory, ref Monster monster)
        {
            string fileName = Path.Combine(directory, monster.FileName + "." + FileExt);

            XDocument xDoc = XDocument.Load(fileName);
            xDoc.Root.Add(new XElement("monster",
                            new XAttribute("name", monster.Name),
                            new XAttribute("nameDescription", monster.Description),
                            new XAttribute("experience", monster.Experience),
                            new XAttribute("speed", monster.Speed)
                        ));
            xDoc.Save(fileName);

            return true;
        }

        private void xmlToGeneric(TFSXmlMonster tfsMonster, out Monster monster)
        {
            monster = new Monster()
            {
                Name = tfsMonster.name,
                Health = (uint)tfsMonster.health.max,
                Experience = (uint)tfsMonster.experience,
                Speed = (uint)tfsMonster.speed,
                Race = TfsToGenericBlood(tfsMonster.race),
            };

            if (!string.IsNullOrEmpty(tfsMonster.nameDescription))
            {
                monster.Description = tfsMonster.nameDescription;
            }
            if (!string.IsNullOrEmpty(tfsMonster.namedescription))
            {
                monster.Description = tfsMonster.namedescription;
            }

            if (tfsMonster.targetchange != null)
            {
                monster.RetargetChance = (uint)tfsMonster.targetchange.chance;

                if ((tfsMonster.targetchange.interval != 0) &&
                    (tfsMonster.targetchange.speed == 0))
                {
                    monster.RetargetInterval = (uint)tfsMonster.targetchange.interval;
                }
                else if ((tfsMonster.targetchange.interval == 0) &&
                         (tfsMonster.targetchange.speed != 0))
                {
                    monster.RetargetInterval = (uint)tfsMonster.targetchange.speed;
                }
                else if ((tfsMonster.targetchange.interval != 0) &&
                    (tfsMonster.targetchange.speed != 0))
                {
                    System.Diagnostics.Debug.WriteLine("Warning duplicate target speed and target interval");
                }
            }

            if (tfsMonster.look != null)
            {
                monster.CorpseId = (uint)tfsMonster.look.corpse;
                monster.OutfitIdLookType = (uint)tfsMonster.look.type;
                monster.LookTypeDetails = new DetailedLookType()
                {
                    Head = (ushort)tfsMonster.look.head,
                    Body = (ushort)tfsMonster.look.body,
                    Legs = (ushort)tfsMonster.look.legs,
                    Feet = (ushort)tfsMonster.look.feet,
                    Addons = (ushort)tfsMonster.look.addons,
                    Mount = (ushort)tfsMonster.look.mount
                };
                monster.ItemIdLookType = (uint)tfsMonster.look.typeex;
            }

            // flags
            if ((tfsMonster.flags != null) &&
                (tfsMonster.flags.flag != null))
            {
                foreach (var x in tfsMonster.flags.flag)
                {
                    int value;
                    if (int.TryParse(x.attr[0].Value, out value))
                    {
                        if (x.attr[0].Name == "summonable")
                        {
                            monster.SummonCost = (uint)tfsMonster.manacost;
                        }
                        else if (x.attr[0].Name == "attackable")
                        {
                            monster.Attackable = value == 1;
                        }
                        else if (x.attr[0].Name == "hostile")
                        {
                            monster.Hostile = value == 1;
                        }
                        else if (x.attr[0].Name == "illusionable")
                        {
                            monster.Illusionable = value == 1;
                        }
                        else if (x.attr[0].Name == "convinceable")
                        {
                            monster.ConvinceCost = (uint)tfsMonster.manacost;
                        }
                        else if (x.attr[0].Name == "pushable")
                        {
                            monster.Pushable = value == 1;
                        }
                        else if (x.attr[0].Name == "canpushitems")
                        {
                            monster.PushItems = value == 1;
                        }
                        else if (x.attr[0].Name == "canpushcreatures")
                        {
                            monster.PushCreatures = value == 1;
                        }
                        else if (x.attr[0].Name == "targetdistance")
                        {
                            monster.TargetDistance = (uint)value;
                        }
                        else if (x.attr[0].Name == "staticattack")
                        {
                            monster.StaticAttack = (uint)value;
                        }
                        else if (x.attr[0].Name == "lightlevel")
                        {
                            monster.LightLevel = (uint)value;
                        }
                        else if (x.attr[0].Name == "lightcolor")
                        {
                            monster.LightColor = (uint)value;
                        }
                        else if (x.attr[0].Name == "runonhealth")
                        {
                            monster.RunOnHealth = (uint)value;
                        }
                        else if (x.attr[0].Name == "hidehealth")
                        {
                            monster.HideHealth = value == 1;
                        }
                        else if (x.attr[0].Name == "canwalkonenergy")
                        {
                            monster.AvoidEnergy = value != 1;
                        }
                        else if (x.attr[0].Name == "canwalkonfire")
                        {
                            monster.AvoidFire = value != 1;
                        }
                        else if (x.attr[0].Name == "canwalkonpoison")
                        {
                            monster.AvoidPoison = value != 1;
                        }
                        else if (x.attr[0].Name == "isboss")
                        {
                            monster.IsBoss = value == 1;
                        }
                        else
                        {
                            Console.WriteLine($"Unknown name {x.attr[0].Name}");
                        }
                    }
                }
            }

            // sounds
            if ((tfsMonster.voices != null) &&
                (tfsMonster.voices.voice != null))
            {
                foreach (VoiceXml sound in tfsMonster.voices.voice)
                {
                    Voice voice = new Voice();
                    voice.Sound = sound.sentence;
                    if (!(string.IsNullOrEmpty(sound.yell)) &&
                        ((sound.yell == "1") || (sound.yell == "true")))
                    {
                        voice.SoundLevel = SoundLevel.Yell;
                    }
                    else
                    {
                        voice.SoundLevel = SoundLevel.Say;
                    }
                    monster.Voices.Add(voice);
                }
            }

            // summons
            if (tfsMonster.summons != null)
            {
                monster.MaxSummons = (uint)tfsMonster.summons.maxSummons;
                foreach (TFSXmlSummon summon in tfsMonster.summons.summon)
                {
                    monster.Summons.Add(new Summon()
                    {
                        Name = summon.name,
                        Chance = Math.Min(1, (double)summon.chance / 100),
                        Rate = (uint)((summon.interval > 0) ? summon.interval : summon.speed),
                        Max = summon.max,
                        Force = summon.force
                    });
                }
            }
            else
            {
                monster.MaxSummons = 0;
            }

            if (tfsMonster.attacks != null)
            {
                XmlSpellsToGeneric(ref monster, tfsMonster.attacks.attack, SpellCategory.Offensive);
            }

            // Defenses
            if (tfsMonster.defenses != null)
            {
                monster.TotalArmor = tfsMonster.defenses.armor;
                monster.Shielding = tfsMonster.defenses.defense;
                XmlSpellsToGeneric(ref monster, tfsMonster.defenses.defenses, SpellCategory.Defensive);
            }

            #region parseElements
            if ((tfsMonster.elements != null) &&
                (tfsMonster.elements.element != null))
            {
                foreach (var x in tfsMonster.elements.element)
                {
                    int value;
                    if (int.TryParse(x.attr[0].Value, out value))
                    {
                        if (x.attr[0].Name == "physicalPercent")
                        {
                            monster.Physical = TfstoGenericElementalPercent(value);
                        }
                        else if (x.attr[0].Name == "icePercent")
                        {
                            monster.Ice = TfstoGenericElementalPercent(value);
                        }
                        else if (x.attr[0].Name == "poisonPercent")
                        {
                            monster.Earth = TfstoGenericElementalPercent(value);
                        }
                        else if (x.attr[0].Name == "earthPercent")
                        {
                            monster.Earth = TfstoGenericElementalPercent(value);
                        }
                        else if (x.attr[0].Name == "firePercent")
                        {
                            monster.Fire = TfstoGenericElementalPercent(value);
                        }
                        else if (x.attr[0].Name == "energyPercent")
                        {
                            monster.Energy = TfstoGenericElementalPercent(value);
                        }
                        else if (x.attr[0].Name == "holyPercent")
                        {
                            monster.Holy = TfstoGenericElementalPercent(value);
                        }
                        else if (x.attr[0].Name == "deathPercent")
                        {
                            monster.Death = TfstoGenericElementalPercent(value);
                        }
                        else if (x.attr[0].Name == "drownPercent")
                        {
                            monster.Drown = TfstoGenericElementalPercent(value);
                        }
                        else if (x.attr[0].Name == "lifedrainPercent")
                        {
                            monster.LifeDrain = TfstoGenericElementalPercent(value);
                        }
                        else if (x.attr[0].Name == "manadrainPercent")
                        {
                            monster.ManaDrain = TfstoGenericElementalPercent(value);
                        }
                    }
                }
            }
            #endregion

            #region paraseImmunities
            if ((tfsMonster.immunities != null) &&
                (tfsMonster.immunities.immunity != null))
            {
                foreach (Immunity immunity in tfsMonster.immunities.immunity)
                {
                    if (immunity.name != namedImmunityXml.NA)
                    {
                        switch (immunity.name)
                        {
                            case namedImmunityXml.physical:
                                monster.Physical = 0;
                                break;
                            case namedImmunityXml.energy:
                                monster.Energy = 0;
                                break;
                            case namedImmunityXml.fire:
                                monster.Fire = 0;
                                break;
                            case namedImmunityXml.poison: //namedImmunityXml.earth
                                monster.Earth = 0;
                                break;
                            case namedImmunityXml.drown:
                                monster.Drown = 0;
                                break;
                            case namedImmunityXml.ice:
                                monster.Ice = 0;
                                break;
                            case namedImmunityXml.holy:
                                monster.Holy = 0;
                                break;
                            case namedImmunityXml.death:
                                monster.Death = 0;
                                break;
                            case namedImmunityXml.lifedrain:
                                monster.LifeDrain = 0;
                                break;
                            case namedImmunityXml.manadrain:
                                monster.ManaDrain = 0;
                                break;
                            case namedImmunityXml.paralyze:
                                monster.IgnoreParalyze = true;
                                break;
                            case namedImmunityXml.outfit:
                                monster.IgnoreOutfit = true;
                                break;
                            case namedImmunityXml.drunk:
                                monster.IgnoreDrunk = true;
                                break;
                            case namedImmunityXml.invisible: //namedImmunityXml.invisibility
                                monster.IgnoreInvisible = true;
                                break;
                            case namedImmunityXml.bleed:
                                monster.IgnoreBleed = true;
                                break;
                        }
                    }
                    else if (immunity.physical != 0)
                    {
                        monster.Physical = 0;
                    }
                    else if (immunity.energy != 0)
                    {
                        monster.Energy = 0;
                    }
                    else if (immunity.fire != 0)
                    {
                        monster.Fire = 0;
                    }
                    else if (immunity.poison != 0) //poison and earth are the same
                    {
                        monster.Earth = 0;
                    }
                    else if (immunity.earth != 0) //poison and earth are the same
                    {
                        monster.Earth = 0;
                    }
                    else if (immunity.drown != 0)
                    {
                        monster.Drown = 0;
                    }
                    else if (immunity.ice != 0)
                    {
                        monster.Ice = 0;
                    }
                    else if (immunity.holy != 0)
                    {
                        monster.Holy = 0;
                    }
                    else if (immunity.death != 0)
                    {
                        monster.Death = 0;
                    }
                    else if (immunity.lifedrain != 0)
                    {
                        monster.LifeDrain = 0;
                    }
                    else if (immunity.manadrain != 0)
                    {
                        monster.ManaDrain = 0;
                    }
                    else if (immunity.paralyze != 0)
                    {
                        monster.IgnoreParalyze = true;
                    }
                    else if (immunity.outfit != 0)
                    {
                        monster.IgnoreOutfit = true;
                    }
                    else if (immunity.bleed != 0)
                    {
                        monster.IgnoreBleed = true;
                    }
                    else if (immunity.drunk != 0)
                    {
                        monster.IgnoreDrunk = true;
                    }
                    else if (immunity.invisible != 0) //invisible and invisibility are the same
                    {
                        monster.IgnoreInvisible = true;
                    }
                    else if (immunity.invisibility != 0) //invisible and invisibility are the same
                    {
                        monster.IgnoreInvisible = true;
                    }
                }
            }
            #endregion

            // Loot
            if ((tfsMonster.loot != null) &&
                (tfsMonster.loot.item != null))
            {
                foreach (var item in tfsMonster.loot.item)
                {
                    string itemType = "";
                    if (!string.IsNullOrEmpty(item.name))
                    {
                        itemType = item.name;
                    }
                    else if (item.id > 0)
                    {
                        itemType = item.id.ToString();
                    }

                    decimal chance = 1;
                    if (item.chance > 0)
                    {
                        chance = item.chance;
                    }
                    else if (item.chance1 > 0)
                    {
                        chance = item.chance1;
                    }

                    chance /= MAX_LOOTCHANCE;

                    Loot commonItem = new Loot()
                    {
                        Item = itemType,
                        Chance = chance,
                        Count = item.countmax
                    };
                    monster.Items.Add(commonItem);
                }
            }
        }

        private Blood TfsToGenericBlood(string blood)
        {
            Blood race = Blood.blood; //default

            uint bloodId;
            if (!uint.TryParse(blood, out bloodId))
            {
                switch (blood)
                {
                    case "venom":
                        race = Blood.venom;
                        break;

                    case "blood":
                        race = Blood.blood;
                        break;

                    case "undead":
                        race = Blood.undead;
                        break;

                    case "fire":
                        race = Blood.fire;
                        break;

                    case "energy":
                        race = Blood.venom;
                        break;
                }
            }
            else
            {
                switch (bloodId)
                {
                    case 1:
                        race = Blood.venom;
                        break;

                    case 2:
                        race = Blood.blood;
                        break;

                    case 3:
                        race = Blood.undead;
                        break;

                    case 4:
                        race = Blood.fire;
                        break;

                    case 5:
                        race = Blood.venom;
                        break;
                }
            }


            return race;
        }

        private void XmlSpellsToGeneric(ref Monster monster, Attack[] spells, SpellCategory category)
        {
            if (spells != null)
            {
                foreach (var attack in spells)
                {
                    Spell spell = new Spell() { SpellCategory = category };
                    spell.Name = attack.name;
                    if (attack.interval != 0)
                    {
                        spell.Interval = (uint)attack.interval;
                    }
                    else if (attack.speed != 0)
                    {
                        spell.Interval = (uint)attack.speed;
                    }
                    else
                    {
                        spell.Interval = ATTACK_INTERVAL_DEFAULT;
                    }

                    spell.Chance = attack.chance / 100.0;

                    if (attack.attribute != null)
                    {
                        foreach (var attr in attack.attribute)
                        {
                            if (attr.key.ToLower() == "shootEffect".ToLower())
                            {
                                spell.ShootEffect = shootTypeNames[attr.value.ToLower()];
                            }
                            else if (attr.key.ToLower() == "areaEffect".ToLower())
                            {
                                spell.AreaEffect = magicEffectNames[attr.value.ToLower()];
                            }
                            else
                            {
                                Console.WriteLine($"Unkown attack attribute {attr.key}");
                            }
                        }
                    }

                    if (attack.range > 0)
                    {
                        spell.Range = (uint?)attack.range;
                    }

                    if (attack.length > 0)
                    {
                        spell.Length = (uint?)attack.length;
                        spell.Spread = (uint?)attack.spread;
                        if ((spell.Length > 3) && (spell.Spread == 0))
                        {
                            spell.Spread = 3;
                        }
                    }
                    spell.OnTarget = (attack.target == 1);

                    if (attack.name == "melee")
                    {
                        if ((attack.attack > 0) && (attack.skill > 0))
                        {
                            spell.AttackValue = attack.attack;
                            spell.Skill = attack.skill;
                        }
                        else
                        {
                            spell.MinDamage = attack.min;
                            spell.MaxDamage = attack.max;
                        }

                        if (attack.fire != 0)
                        {
                            spell.Condition = ConditionType.Fire;
                            spell.StartDamage = attack.poison;
                        }
                        else if (attack.poison != 0)
                        {
                            spell.Condition = ConditionType.Poison;
                            spell.StartDamage = attack.poison;
                        }
                        else if (attack.energy != 0)
                        {
                            spell.Condition = ConditionType.Energy;
                            spell.StartDamage = attack.energy;
                        }
                        else if (attack.drown != 0)
                        {
                            spell.Condition = ConditionType.Drown;
                            spell.StartDamage = attack.drown;
                        }
                        else if (attack.dazzle != 0)
                        {
                            spell.Condition = ConditionType.Dazzled;
                            spell.StartDamage = attack.dazzle;
                        }
                        else if (attack.curse != 0)
                        {
                            spell.Condition = ConditionType.Cursed;
                            spell.StartDamage = attack.curse;
                        }
                        else if (attack.bleed != 0)
                        {
                            spell.Condition = ConditionType.Bleeding;
                            spell.StartDamage = attack.bleed;
                        }
                        else if (attack.physical != 0)
                        {
                            spell.Condition = ConditionType.Bleeding;
                            spell.StartDamage = attack.physical;
                        }
                        else if (attack.freeze != 0)
                        {
                            spell.Condition = ConditionType.Freezing;
                            spell.StartDamage = attack.freeze;
                        }
                        if (spell.Condition != ConditionType.None)
                        {
                            spell.Tick = (attack.tick != 0) ? attack.tick : conditionDefaultTick[spell.Condition];
                        }
                    }
                    else if (attack.name == "speed")
                    {
                        if (attack.speedchange != 0)
                        {
                            spell.MinSpeedChange = attack.speedchange;
                            spell.MaxSpeedChange = attack.speedchange;
                        }
                        else
                        {
                            spell.MinSpeedChange = attack.minspeedchange;
                            spell.MaxSpeedChange = attack.maxspeedchange;
                        }
                    }
                    else if (attack.name == "drunk")
                    {
                        spell.Drunkenness = attack.drunkenness / 100.0;
                    }
                    else
                    {
                        if (attack.name.Contains("condition"))
                        {
                            spell.Condition = conditionDamageNames[attack.name];
                            spell.Name = "condition";
                            spell.Tick = (attack.tick != 0) ? attack.tick : conditionDefaultTick[spell.Condition];
                            spell.StartDamage = attack.start;
                        }

                        // Always default both if max is included to be explicit
                        // Some spells don't have damage so don't include either of them
                        if (attack.max != 0)
                        {
                            spell.MinDamage = attack.min;
                            spell.MaxDamage = attack.max;
                        }

                        if (combatDamageNames.ContainsKey(spell.Name))
                        {
                            spell.DamageElement = combatDamageNames[spell.Name];
                            spell.Name = "combat";
                        }

                        if (!string.IsNullOrEmpty(attack.monster))
                        {
                            spell.MonsterName = attack.monster;
                        }
                        else if (attack.item > 0)
                        {
                            spell.ItemId = attack.item;
                        }
                    }

                    if ((attack.name == "speed") || (attack.name == "outfit") || (attack.name == "invisible") || (attack.name == "drunk"))
                    {
                        spell.Duration = attack.duration;
                    }

                    monster.Attacks.Add(spell);
                }
            }
        }

        private string GenerictoTfsBlood(Blood race)
        {
            string bloodName = "blood";

            switch (race)
            {
                case Blood.venom:
                    bloodName = "venom";
                    break;

                case Blood.blood:
                    bloodName = "blood";
                    break;

                case Blood.undead:
                    bloodName = "undead";
                    break;

                case Blood.fire:
                    bloodName = "fire";
                    break;

                case Blood.energy:
                    bloodName = "energy";
                    break;
            }

            return bloodName;
        }

        private double TfstoGenericElementalPercent(int percent)
        {
            return (1 - ((double)percent / 100));
        }

        private void Serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Unknown Node:" + e.Name + "\t" + e.Text);
        }

        private void Serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            System.Xml.XmlAttribute attr = e.Attr;
            System.Diagnostics.Debug.WriteLine("Unknown attribute " + attr.Name + "='" + attr.Value + "'");
        }
    }
}
