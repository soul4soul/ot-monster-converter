using MonsterConverterInterface;
using MonsterConverterInterface.MonsterTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace MonsterConverterTfsXml
{
    //https://github.com/otland/forgottenserver/blob/master/src/monsters.cpp
    [Export(typeof(IMonsterConverter))]
    public class TfsXmlConverter : MonsterConverter
    {
        const int MAX_LOOTCHANCE = 100000;
        const int ATTACK_INTERVAL_DEFAULT = 2000;

        private readonly BiDictionary<string, Effect> magicEffectNames = new BiDictionary<string, Effect>
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

        private readonly BiDictionary<string, Animation> shootTypeNames = new BiDictionary<string, Animation>
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

        private readonly IDictionary<string, CombatDamage> combatNamesToCombatDamage = new Dictionary<string, CombatDamage>
        {
            {"physical",    CombatDamage.Physical},
            {"energy",      CombatDamage.Energy},
            {"earth",       CombatDamage.Earth},
            {"poison",      CombatDamage.Earth},
            {"fire",        CombatDamage.Fire},
            {"lifedrain",   CombatDamage.LifeDrain},
            {"manadrain",   CombatDamage.ManaDrain},
            {"healing",     CombatDamage.Healing},
            {"drown",       CombatDamage.Drown},
            {"ice",         CombatDamage.Ice},
            {"holy",        CombatDamage.Holy},
            {"death",       CombatDamage.Death}
        };

        private readonly IDictionary<CombatDamage, string> combatDamageToAttackName = new Dictionary<CombatDamage, string>
        {
            {CombatDamage.Physical, "physical"},
            {CombatDamage.Energy, "energy"},
            {CombatDamage.Earth, "earth"},
            {CombatDamage.Fire, "fire"},
            {CombatDamage.LifeDrain, "lifedrain"},
            {CombatDamage.ManaDrain, "manadrain"},
            {CombatDamage.Healing, "healing"},
            {CombatDamage.Drown, "drown"},
            {CombatDamage.Ice, "ice"},
            {CombatDamage.Holy, "holy"},
            {CombatDamage.Death, "death"}
        };

        private readonly IDictionary<string, ConditionType> conditionNamesToConditionType = new Dictionary<string, ConditionType>
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

        private readonly IDictionary<ConditionType, string> conditionTypeToAttackName = new Dictionary<ConditionType, string>
        {
            {ConditionType.Bleeding, "physicalcondition"},
            {ConditionType.Energy, "energycondition"},
            {ConditionType.Poison, "earthcondition"},
            {ConditionType.Fire, "firecondition"},
            {ConditionType.Drown, "drowncondition"},
            {ConditionType.Freezing, "icecondition"},
            {ConditionType.Dazzled, "holycondition"},
            {ConditionType.Cursed, "cursecondition"}
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

        public override string ConverterName { get => "TFS XML"; }

        public override string FileExt { get => "xml"; }

        public override ItemIdType ItemIdType { get => ItemIdType.Server; }

        public override bool IsReadSupported { get => true; }

        public override bool IsWriteSupported { get => true; }

        // Using this won't work we ever parallize processing
        private static string CurrentFileName { get; set; }

        // Functions
        public override ConvertResultEventArgs ReadMonster(string fileName, out Monster monster)
        {
            try
            {
                CurrentFileName = fileName;
                XmlSerializer serializer = new XmlSerializer(typeof(TFSXmlMonster));

                serializer.UnknownNode += new XmlNodeEventHandler(Serializer_UnknownNode);
                serializer.UnknownAttribute += new XmlAttributeEventHandler(Serializer_UnknownAttribute);

                IList<string> indexedLootComments = ReadLootComments(fileName);
                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                {
                    // Use the Deserialize method to restore the object's state with data from the XML document
                    TFSXmlMonster tfsMonster = (TFSXmlMonster)serializer.Deserialize(fs);
                    TfsXmlToGeneric(tfsMonster, indexedLootComments, out monster);
                }

                monster.FileName = Path.GetFileNameWithoutExtension(fileName);
                // Guess the registered name, they are actually defined in "monsters.xml" but we don't parse that file...
                monster.RegisteredName = monster.FileName.Replace('_', ' ');

                return new ConvertResultEventArgs(fileName, ConvertError.Success);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error pasring {fileName}. Exception {ex.Message}");
                monster = null;
                return new ConvertResultEventArgs(fileName, ConvertError.Error, ex.Message);
            }
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
                result.AppendMessage("format doesn't support summon and coninvce mana costs being different");
                result.IncreaseError(ConvertError.Warning);
            }

            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.IndentChars = "\t";

            using (XmlWriter xw = XmlWriter.Create(fileName, xws))
            {
                XElement monsterElement = new XElement("monster",
                    new XAttribute("name", monster.Name),
                    new XAttribute("nameDescription", monster.Description ?? ""),
                    new XAttribute("experience", monster.Experience),
                    new XAttribute("speed", monster.Speed),
                    new XAttribute("manacost", monster.SummonCost),
                    new XElement("health",
                        new XAttribute("now", monster.Health),
                        new XAttribute("max", monster.Health)),
                    LookGenericToTfsXml(monster.Look, ref result),
                    new XElement("targetchange",
                        new XAttribute("interval", monster.RetargetInterval),
                        new XAttribute("chance", Math.Round(monster.RetargetChance * 100))),
                    new XElement("flags",
                        new XElement("flag",
                            new XAttribute("attackable", monster.Attackable ? 1 : 0)),
                        new XElement("flag",
                            new XAttribute("hostile", monster.IsHostile ? 1 : 0)),
                        new XElement("flag",
                            new XAttribute("summonable", monster.SummonCost > 0 ? 1 : 0)),
                        new XElement("flag",
                            new XAttribute("convinceable", monster.ConvinceCost > 0 ? 1 : 0)),
                        new XElement("flag",
                            new XAttribute("illusionable", monster.IsIllusionable ? 1 : 0)),
                        new XElement("flag",
                            new XAttribute("isboss", monster.IsBoss ? 1 : 0)),
                        new XElement("flag",
                            new XAttribute("ignorespawnblock", monster.IgnoreSpawnBlock ? 1 : 0)),
                        new XElement("flag",
                            new XAttribute("pushable", monster.IsPushable ? 1 : 0)),
                        new XElement("flag",
                            new XAttribute("canpushitems", monster.PushItems ? 1 : 0)),
                        new XElement("flag",
                            new XAttribute("canpushcreatures", monster.PushCreatures ? 1 : 0)),
                        new XElement("flag",
                            new XAttribute("staticattack", monster.StaticAttackChance)),
                        new XElement("flag",
                            new XAttribute("targetdistance", monster.TargetDistance)),
                        new XElement("flag",
                            new XAttribute("healthHidden", monster.HideHealth ? 1 : 0)),
                        new XElement("flag",
                            new XAttribute("canWalkOnEnergy", monster.AvoidEnergy ? 0 : 1)),
                        new XElement("flag",
                            new XAttribute("canWalkOnFire", monster.AvoidFire ? 0 : 1)),
                        new XElement("flag",
                            new XAttribute("canWalkOnPoison", monster.AvoidPoison ? 0 : 1))),
                    new XElement("immunities",
                        new XElement("immunity",
                                new XAttribute("paralyze", monster.IgnoreParalyze ? 1 : 0)),
                        new XElement("immunity",
                                new XAttribute("outfit", monster.IgnoreOutfit ? 1 : 0)),
                        new XElement("immunity",
                                new XAttribute("invisible", monster.IgnoreInvisible ? 1 : 0)),
                        new XElement("immunity",
                                new XAttribute("drunk", monster.IgnoreDrunk ? 1 : 0)),
                        new XElement("immunity",
                                new XAttribute("bleed", monster.IgnoreBleed ? 1 : 0))),
                    new XElement("elements",
                        new XElement("element",
                                    new XAttribute("physicalPercent", ElemementPercentGenericToTfsXml(monster.PhysicalDmgMod))),
                        new XElement("element",
                                    new XAttribute("energyPercent", ElemementPercentGenericToTfsXml(monster.EnergyDmgMod))),
                        new XElement("element",
                                    new XAttribute("earthPercent", ElemementPercentGenericToTfsXml(monster.EarthDmgMod))),
                        new XElement("element",
                                    new XAttribute("firePercent", ElemementPercentGenericToTfsXml(monster.FireDmgMod))),
                        new XElement("element",
                                    new XAttribute("lifedrainPercent", ElemementPercentGenericToTfsXml(monster.LifeDrainDmgMod))),
                        new XElement("element",
                                    new XAttribute("manadrainPercent", ElemementPercentGenericToTfsXml(monster.ManaDrainDmgMod))),
                        new XElement("element",
                                    new XAttribute("drownPercent", ElemementPercentGenericToTfsXml(monster.DrownDmgMod))),
                        new XElement("element",
                                    new XAttribute("icePercent", ElemementPercentGenericToTfsXml(monster.IceDmgMod))),
                        new XElement("element",
                                    new XAttribute("holyPercent", ElemementPercentGenericToTfsXml(monster.HolyDmgMod))),
                        new XElement("element",
                                    new XAttribute("deathPercent", ElemementPercentGenericToTfsXml(monster.DeathDmgMod))))
                    );
                AbilitiesGenericToTfsXmlAttacks(monster, ref monsterElement);
                monsterElement.Add(AbilitiesGenericToTfsXmlDefense(monster));
                VoiceGenericToTfsXml(monster, ref monsterElement);
                SummonGenericToTfsXml(monster, ref monsterElement);
                monsterElement.Add(LootGenericToTfsXml(monster.Items));
                XDocument doc = new XDocument(monsterElement);
                doc.WriteTo(xw);
            }

            return result;
        }

        private void AbilitiesGenericToTfsXmlAttacks(Monster monster, ref XElement monsterElement)
        {
            XElement attacks = new XElement("attacks");
            foreach (var s in monster.Attacks)
            {
                if (s.SpellCategory == SpellCategory.Offensive)
                {
                    attacks.Add(SpellGenericToTfsXml(s, "attack"));
                }
            }
            if (attacks.HasElements)
            {
                monsterElement.Add(attacks);
            }
        }

        private XElement AbilitiesGenericToTfsXmlDefense(Monster monster)
        {
            XElement defenses = new XElement("defenses",
                new XAttribute("armor", monster.TotalArmor),
                new XAttribute("defense", monster.Shielding));
            foreach (var s in monster.Attacks)
            {
                if (s.SpellCategory == SpellCategory.Defensive)
                {
                    defenses.Add(SpellGenericToTfsXml(s, "defense"));
                }
            }
            return defenses;
        }

        private XElement SpellGenericToTfsXml(Spell spell, string elementName)
        {
            XElement ability = new XElement(elementName);

            if (spell.DefinitionStyle == SpellDefinition.TfsLuaScript)
            {
                ability.Add(new XAttribute("script", spell.Name));
                ability.Add(new XAttribute("interval", spell.Interval));
                ability.Add(new XAttribute("chance", Math.Round(spell.Chance * 100)));

                if ((spell.MinDamage != null) && (spell.MaxDamage != null))
                {
                    ability.Add(new XAttribute("min", spell.MinDamage));
                    ability.Add(new XAttribute("max", spell.MaxDamage));
                }
                else if (spell.MaxDamage != null)
                {
                    ability.Add(new XAttribute("max", spell.MaxDamage));
                }
                if (spell.OnTarget != null)
                {
                    ability.Add(new XAttribute("target", (spell.OnTarget == true) ? 1 : 0));
                }
                if (spell.IsDirectional != null)
                {
                    ability.Add(new XAttribute("direction", (spell.IsDirectional == true) ? 1 : 0));
                }
            }
            else if (spell.DefinitionStyle == SpellDefinition.Raw)
            {
                if (spell.Name == "melee")
                {
                    ability.Add(new XAttribute("name", spell.Name));
                    ability.Add(new XAttribute("interval", spell.Interval));
                    ability.Add(new XAttribute("chance", Math.Round(spell.Chance * 100)));

                    if ((spell.MinDamage != null) && (spell.MaxDamage != null))
                    {
                        ability.Add(new XAttribute("min", spell.MinDamage));
                        ability.Add(new XAttribute("max", spell.MaxDamage));
                    }
                    else if (spell.MaxDamage != null)
                    {
                        ability.Add(new XAttribute("max", spell.MaxDamage));
                    }
                    else if ((spell.AttackValue != null) && (spell.Skill != null))
                    {
                        ability.Add(new XAttribute("skill", spell.Skill));
                        ability.Add(new XAttribute("attack", spell.AttackValue));
                    }
                    //else continue which we should never hit?

                    if (spell.Condition != ConditionType.None)
                    {
                        ability.Add(new XAttribute(conditionTypeToAttackName[spell.Condition], spell.StartDamage));
                        ability.Add(new XAttribute("tick", spell.Tick));
                    }
                }
                else
                {
                    if (spell.Name == "speed")
                    {
                        ability.Add(new XAttribute("name", spell.Name));
                        ability.Add(new XAttribute("interval", spell.Interval));
                        ability.Add(new XAttribute("chance", Math.Round(spell.Chance * 100)));
                        ability.Add(new XAttribute("minspeedchange", spell.MinSpeedChange));
                        ability.Add(new XAttribute("maxspeedchange", spell.MaxSpeedChange));
                    }
                    else if (spell.Name == "condition")
                    {
                        ability.Add(new XAttribute("name", conditionTypeToAttackName[spell.Condition]));
                        ability.Add(new XAttribute("interval", spell.Interval));
                        ability.Add(new XAttribute("chance", Math.Round(spell.Chance * 100)));
                        ability.Add(new XAttribute("tick", spell.Tick));
                        ability.Add(new XAttribute("start", spell.StartDamage));
                    }
                    else if (spell.Name == "outfit")
                    {
                        ability.Add(new XAttribute("name", spell.Name));
                        ability.Add(new XAttribute("interval", spell.Interval));
                        ability.Add(new XAttribute("chance", Math.Round(spell.Chance * 100)));
                        if (!string.IsNullOrEmpty(spell.MonsterName))
                        {
                            ability.Add(new XAttribute("monster", spell.MonsterName));
                        }
                        else if (spell.ItemId != null)
                        {
                            ability.Add(new XAttribute("item", spell.ItemId));
                        }
                    }
                    else if ((spell.Name == "combat") && (spell.DamageElement != CombatDamage.None))
                    {
                        ability.Add(new XAttribute("name", combatDamageToAttackName[spell.DamageElement]));
                        ability.Add(new XAttribute("interval", spell.Interval));
                        ability.Add(new XAttribute("chance", Math.Round(spell.Chance * 100)));
                    }
                    else if (spell.Name == "drunk")
                    {
                        ability.Add(new XAttribute("name", spell.Name));
                        ability.Add(new XAttribute("interval", spell.Interval));
                        ability.Add(new XAttribute("chance", Math.Round(spell.Chance * 100)));
                        ability.Add(new XAttribute("drunkenness", spell.Drunkenness * 100));
                    }
                    else
                    {
                        ability.Add(new XAttribute("name", spell.Name));
                        ability.Add(new XAttribute("interval", spell.Interval));
                        ability.Add(new XAttribute("chance", Math.Round(spell.Chance * 100)));
                    }

                    if ((spell.MinDamage != null) && (spell.MaxDamage != null))
                    {
                        ability.Add(new XAttribute("min", spell.MinDamage));
                        ability.Add(new XAttribute("max", spell.MaxDamage));
                    }
                    else if (spell.MaxDamage != null)
                    {
                        ability.Add(new XAttribute("max", spell.MaxDamage));
                    }
                    if (spell.Duration != null)
                    {
                        ability.Add(new XAttribute("duration", spell.Duration));
                    }
                    if (spell.Range != null)
                    {
                        ability.Add(new XAttribute("range", spell.Range));
                    }
                    if (spell.Radius != null)
                    {
                        ability.Add(new XAttribute("radius", spell.Radius));
                        ability.Add(new XAttribute("target", (spell.OnTarget == true) ? 1 : 0));
                    }
                    if (spell.Length != null)
                    {
                        ability.Add(new XAttribute("length", spell.Length));
                    }
                    if (spell.Spread != null)
                    {
                        ability.Add(new XAttribute("spread", spell.Spread));
                    }
                    if (spell.ShootEffect != Animation.None)
                    {
                        ability.Add(new XElement("attribute",
                            new XAttribute("key", "shootEffect"),
                            new XAttribute("value", shootTypeNames.Reverse[spell.ShootEffect])));
                    }
                    if (spell.AreaEffect != Effect.None)
                    {
                        ability.Add(new XElement("attribute",
                            new XAttribute("key", "areaEffect"),
                            new XAttribute("value", magicEffectNames.Reverse[spell.AreaEffect])));
                    }
                }
            }

            return ability;
        }

        private static void SummonGenericToTfsXml(Monster monster, ref XElement monsterElement)
        {
            XElement summons = new XElement("summons",
                new XAttribute("maxSummons", monster.MaxSummons));
            foreach (var s in monster.Summons)
            {
                XElement summon = new XElement("voice",
                    new XAttribute("name", s.Name),
                    new XAttribute("interval", s.Interval),
                    new XAttribute("chance", Math.Round(s.Chance * 100)));

                if (s.Max > 0)
                {
                    summon.Add(new XAttribute("max", s.Max));
                }

                if (s.Force)
                {
                    summon.Add(new XAttribute("force", s.Force));
                }

                summons.Add(summon);
            }
            if (summons.HasElements)
            {
                monsterElement.Add(summons);
            }
        }

        private static double ElemementPercentGenericToTfsXml(double percent)
        {
            double value = (1 - percent) * 100;
            return Math.Round(value);
        }

        private static XElement LookGenericToTfsXml(LookData look, ref ConvertResultEventArgs result)
        {
            if (look.LookType == LookType.Outfit)
            {
                return new XElement("look",
                    new XAttribute("type", look.LookId),
                    new XAttribute("head", look.Head),
                    new XAttribute("body", look.Body),
                    new XAttribute("legs", look.Legs),
                    new XAttribute("feet", look.Feet),
                    new XAttribute("addons", look.Addons),
                    new XAttribute("mount", look.Mount),
                    new XAttribute("corpse", look.CorpseId));
            }
            else if (look.LookType == LookType.Item)
            {
                return new XElement("look", 
                    new XAttribute("typeex", look.LookId),
                    new XAttribute("corpse", look.CorpseId));
            }
            else
            {
                result.AppendMessage("Invisible look type not supported");
                result.IncreaseError(ConvertError.Warning);
                return new XElement("look");
            }
        }

        private static void VoiceGenericToTfsXml(Monster monster, ref XElement monsterElement)
        {
            XElement voices = new XElement("voices",
                new XAttribute("interval", monster.VoiceInterval),
                new XAttribute("chance", Math.Round(monster.VoiceChance * 100)));
            foreach (var v in monster.Voices)
            {
                voices.Add(new XElement("voice", 
                    new XAttribute("sentence", v.Sound),
                    new XAttribute("yell", v.SoundLevel == SoundLevel.Yell)));
            }
            if (voices.HasElements)
            {
                monsterElement.Add(voices);
            }
        }

        private static XElement LootGenericToTfsXml(IList<Loot> items)
        {
            XElement loot = new XElement("loot");
            foreach (var i in items)
            {
                loot.Add(NestedLootGenericToTfsXml(i));
            }
            return loot;
        }

        private static XElement NestedLootGenericToTfsXml(Loot i)
        {
            string itemType = int.TryParse(i.Item, out _) ? "id" : "name";

            XElement item = new XElement("item",
                new XAttribute(itemType, i.Item),
                new XAttribute("chance", Math.Round(i.Chance * MAX_LOOTCHANCE)));

            if (i.Count > 1)
            {
                item.Add(new XAttribute("countmax", i.Count));
            }

            if (i.SubType > 0)
            {
                item.Add(new XAttribute("subtype", i.SubType));
            }

            if (i.ActionId > 0)
            {
                item.Add(new XAttribute("actionId", i.ActionId));
            }

            if (!string.IsNullOrWhiteSpace(i.Text))
            {
                item.Add(new XAttribute("text", i.Text));
            }

            if (!string.IsNullOrWhiteSpace(i.Description))
            {
                item.Add(new XAttribute("description", i.Description));
            }

            if (i.NestedLoot.Count > 0)
            {
                foreach (var ni in i.NestedLoot)
                {
                    item.Add(NestedLootGenericToTfsXml(ni));
                }
            }

            return item;
        }

        private void TfsXmlToGeneric(TFSXmlMonster tfsMonster, IList<string> indexedLootComments, out Monster monster)
        {
            monster = new Monster()
            {
                Name = tfsMonster.name,
                Health = tfsMonster.health.max,
                Experience = tfsMonster.experience,
                Speed = tfsMonster.speed,
                Race = TfsXmlToGenericBlood(tfsMonster.race),
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
                monster.RetargetChance = tfsMonster.targetchange.chance / 100.0; ;

                if ((tfsMonster.targetchange.interval != 0) &&
                    (tfsMonster.targetchange.speed == 0))
                {
                    monster.RetargetInterval = tfsMonster.targetchange.interval;
                }
                else if ((tfsMonster.targetchange.interval == 0) &&
                         (tfsMonster.targetchange.speed != 0))
                {
                    monster.RetargetInterval = tfsMonster.targetchange.speed;
                }
                else if ((tfsMonster.targetchange.interval != 0) &&
                    (tfsMonster.targetchange.speed != 0))
                {
                    System.Diagnostics.Debug.WriteLine("Warning duplicate target speed and target interval");
                }
            }

            if (tfsMonster.look != null)
            {
                if (tfsMonster.look.type != 0)
                {
                    monster.Look.LookType = LookType.Outfit;
                    monster.Look.LookId = tfsMonster.look.type;
                    monster.Look.Head = tfsMonster.look.head;
                    monster.Look.Body = tfsMonster.look.body;
                    monster.Look.Legs = tfsMonster.look.legs;
                    monster.Look.Feet = tfsMonster.look.feet;
                    monster.Look.Addons = tfsMonster.look.addons;
                    monster.Look.Mount = tfsMonster.look.mount;
                }
                else if (tfsMonster.look.typeex != 0)
                {
                    monster.Look.LookType = LookType.Item;
                    monster.Look.LookId = tfsMonster.look.typeex;
                }
                else
                {
                    monster.Look.LookType = LookType.Invisible;
                }

                monster.Look.CorpseId = tfsMonster.look.corpse;
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
                            monster.SummonCost = tfsMonster.manacost;
                        }
                        else if (x.attr[0].Name == "attackable")
                        {
                            monster.Attackable = value == 1;
                        }
                        else if (x.attr[0].Name == "hostile")
                        {
                            monster.IsHostile = value == 1;
                        }
                        else if (x.attr[0].Name == "illusionable")
                        {
                            monster.IsIllusionable = value == 1;
                        }
                        else if (x.attr[0].Name == "convinceable")
                        {
                            monster.ConvinceCost = tfsMonster.manacost;
                        }
                        else if (x.attr[0].Name == "pushable")
                        {
                            monster.IsPushable = value == 1;
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
                            monster.TargetDistance = value;
                        }
                        else if (x.attr[0].Name == "staticattack")
                        {
                            monster.StaticAttackChance = value;
                        }
                        else if (x.attr[0].Name == "lightlevel")
                        {
                            monster.LightLevel = value;
                        }
                        else if (x.attr[0].Name == "lightcolor")
                        {
                            monster.LightColor = value;
                        }
                        else if (x.attr[0].Name == "runonhealth")
                        {
                            monster.RunOnHealth = value;
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
                        else if (x.attr[0].Name == "ignorespawnblock")
                        {
                            monster.IgnoreSpawnBlock = value == 1;
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
                // No default vaule in TFS, warning given if missing
                if (tfsMonster.voices.interval > 0)
                {
                    monster.VoiceInterval = tfsMonster.voices.interval;
                }

                if (tfsMonster.voices.speed > 0)
                {
                    monster.VoiceInterval = tfsMonster.voices.speed;
                }

                if (tfsMonster.voices.chance > 0)
                {
                    monster.VoiceChance = tfsMonster.voices.chance / 100;
                }

                foreach (VoiceXml sound in tfsMonster.voices.voice)
                {
                    Voice voice = new Voice(sound.sentence);
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
                monster.MaxSummons = tfsMonster.summons.maxSummons;
                foreach (TFSXmlSummon summon in tfsMonster.summons.summon)
                {
                    monster.Summons.Add(new Summon()
                    {
                        Name = summon.name,
                        Chance = Math.Min(1, summon.chance / 100),
                        Interval = (summon.interval > 0) ? summon.interval : summon.speed,
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
                TfsXmlSpellsToGeneric(ref monster, tfsMonster.attacks.attack, SpellCategory.Offensive);
            }

            // Defenses
            if (tfsMonster.defenses != null)
            {
                monster.TotalArmor = tfsMonster.defenses.armor;
                monster.Shielding = tfsMonster.defenses.defense;
                TfsXmlSpellsToGeneric(ref monster, tfsMonster.defenses.defenses, SpellCategory.Defensive);
            }

            // parseElements
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
                            monster.PhysicalDmgMod = TfsXmltoGenericElementalPercent(value);
                        }
                        else if (x.attr[0].Name == "icePercent")
                        {
                            monster.IceDmgMod = TfsXmltoGenericElementalPercent(value);
                        }
                        else if (x.attr[0].Name == "poisonPercent")
                        {
                            monster.EarthDmgMod = TfsXmltoGenericElementalPercent(value);
                        }
                        else if (x.attr[0].Name == "earthPercent")
                        {
                            monster.EarthDmgMod = TfsXmltoGenericElementalPercent(value);
                        }
                        else if (x.attr[0].Name == "firePercent")
                        {
                            monster.FireDmgMod = TfsXmltoGenericElementalPercent(value);
                        }
                        else if (x.attr[0].Name == "energyPercent")
                        {
                            monster.EnergyDmgMod = TfsXmltoGenericElementalPercent(value);
                        }
                        else if (x.attr[0].Name == "holyPercent")
                        {
                            monster.HolyDmgMod = TfsXmltoGenericElementalPercent(value);
                        }
                        else if (x.attr[0].Name == "deathPercent")
                        {
                            monster.DeathDmgMod = TfsXmltoGenericElementalPercent(value);
                        }
                        else if (x.attr[0].Name == "drownPercent")
                        {
                            monster.DrownDmgMod = TfsXmltoGenericElementalPercent(value);
                        }
                        else if (x.attr[0].Name == "lifedrainPercent")
                        {
                            monster.LifeDrainDmgMod = TfsXmltoGenericElementalPercent(value);
                        }
                        else if (x.attr[0].Name == "manadrainPercent")
                        {
                            monster.ManaDrainDmgMod = TfsXmltoGenericElementalPercent(value);
                        }
                    }
                }
            }

            // paraseImmunities
            if ((tfsMonster.immunities != null) &&
                (tfsMonster.immunities.immunity != null))
            {
                foreach (Immunity immunity in tfsMonster.immunities.immunity)
                {
                    if (immunity.name != TfsXmlNamedImmunity.NA)
                    {
                        switch (immunity.name)
                        {
                            case TfsXmlNamedImmunity.physical:
                                monster.PhysicalDmgMod = 0;
                                break;
                            case TfsXmlNamedImmunity.energy:
                                monster.EnergyDmgMod = 0;
                                break;
                            case TfsXmlNamedImmunity.fire:
                                monster.FireDmgMod = 0;
                                break;
                            case TfsXmlNamedImmunity.poison: //namedImmunityXml.earth
                                monster.EarthDmgMod = 0;
                                break;
                            case TfsXmlNamedImmunity.drown:
                                monster.DrownDmgMod = 0;
                                break;
                            case TfsXmlNamedImmunity.ice:
                                monster.IceDmgMod = 0;
                                break;
                            case TfsXmlNamedImmunity.holy:
                                monster.HolyDmgMod = 0;
                                break;
                            case TfsXmlNamedImmunity.death:
                                monster.DeathDmgMod = 0;
                                break;
                            case TfsXmlNamedImmunity.lifedrain:
                                monster.LifeDrainDmgMod = 0;
                                break;
                            case TfsXmlNamedImmunity.manadrain:
                                monster.ManaDrainDmgMod = 0;
                                break;
                            case TfsXmlNamedImmunity.paralyze:
                                monster.IgnoreParalyze = true;
                                break;
                            case TfsXmlNamedImmunity.outfit:
                                monster.IgnoreOutfit = true;
                                break;
                            case TfsXmlNamedImmunity.drunk:
                                monster.IgnoreDrunk = true;
                                break;
                            case TfsXmlNamedImmunity.invisible: //namedImmunityXml.invisibility
                                monster.IgnoreInvisible = true;
                                break;
                            case TfsXmlNamedImmunity.bleed:
                                monster.IgnoreBleed = true;
                                break;
                        }
                    }
                    else if (immunity.physical != 0)
                    {
                        monster.PhysicalDmgMod = 0;
                    }
                    else if (immunity.energy != 0)
                    {
                        monster.EnergyDmgMod = 0;
                    }
                    else if (immunity.fire != 0)
                    {
                        monster.FireDmgMod = 0;
                    }
                    else if (immunity.poison != 0) //poison and earth are the same
                    {
                        monster.EarthDmgMod = 0;
                    }
                    else if (immunity.earth != 0) //poison and earth are the same
                    {
                        monster.EarthDmgMod = 0;
                    }
                    else if (immunity.drown != 0)
                    {
                        monster.DrownDmgMod = 0;
                    }
                    else if (immunity.ice != 0)
                    {
                        monster.IceDmgMod = 0;
                    }
                    else if (immunity.holy != 0)
                    {
                        monster.HolyDmgMod = 0;
                    }
                    else if (immunity.death != 0)
                    {
                        monster.DeathDmgMod = 0;
                    }
                    else if (immunity.lifedrain != 0)
                    {
                        monster.LifeDrainDmgMod = 0;
                    }
                    else if (immunity.manadrain != 0)
                    {
                        monster.ManaDrainDmgMod = 0;
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

            // Loot
            if ((tfsMonster.loot != null) &&
                (tfsMonster.loot.item != null))
            {
                int itemIndex = 0;
                foreach (var item in tfsMonster.loot.item)
                {
                    Loot genericLoot = TfsToGenericLoot(item, indexedLootComments, itemIndex);
                    itemIndex++;

                    if ((item.NestedItems != null) &&
                        (item.NestedItems.Length > 0))
                    {
                        ParseNestedLoot(ref genericLoot, item.NestedItems, indexedLootComments, ref itemIndex);
                    }

                    if ((item.Inside != null) &&
                        (item.Inside.NestedItems != null) &&
                        (item.Inside.NestedItems.Length > 0))
                    {
                        ParseNestedLoot(ref genericLoot, item.Inside.NestedItems, indexedLootComments, ref itemIndex);
                    }

                    monster.Items.Add(genericLoot);
                }
            }

            // Scripts
            // For TFS this is a script which has one or more of the following onThink, onAppear, onDisappear, onMove, onSay
            // We can't tell which of them are actually registered in the script but we can save and report it for manual inspection
            if (!string.IsNullOrWhiteSpace(tfsMonster.script))
            {
                monster.Scripts.Add(new Script() { Name = tfsMonster.script, Type = ScriptType.Unknown });
            }

            // For TFS this is OnDeath Event only?
            if (tfsMonster.scripts != null)
            {
                foreach (var te in tfsMonster.scripts.Event)
                {
                    monster.Scripts.Add(new Script() { Name = te.Name, Type = ScriptType.OnDeath });
                }
            }
        }

        private void ParseNestedLoot(ref Loot lootContainer, Item[] items, IList<string> indexedLootComments, ref int itemIndex)
        {
            foreach (var item in items)
            {
                Loot genericLoot = TfsToGenericLoot(item, indexedLootComments, itemIndex);
                itemIndex++;

                if ((item.NestedItems != null) &&
                    (item.NestedItems.Length > 0))
                {
                    ParseNestedLoot(ref genericLoot, item.NestedItems, indexedLootComments, ref itemIndex);
                }

                if ((item.Inside != null) &&
                    (item.Inside.NestedItems != null) &&
                    (item.Inside.NestedItems.Length > 0))
                {
                    ParseNestedLoot(ref genericLoot, item.Inside.NestedItems, indexedLootComments, ref itemIndex);
                }

                lootContainer.NestedLoot.Add(genericLoot);
            }
        }

        private Loot TfsToGenericLoot(Item item, IList<string> indexedLootComments, int itemIndex)
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

            return new Loot()
            {
                Item = itemType,
                Chance = chance,
                Count = item.countmax,
                SubType = item.subtype,
                ActionId = item.actionId,
                Text = item.text,
                Description = indexedLootComments[itemIndex]
            };
        }

        private static IList<string> ReadLootComments(string fileName)
        {
            IList<string> indexedLootComments = new List<string>();
            string fileContents = File.ReadAllText(fileName);
            var matches = Regex.Matches(fileContents, @"<item.*?>\s*(<!--(?<comment>.*?)-->)?", RegexOptions.Singleline);
            foreach (Match match in matches)
            {
                indexedLootComments.Add(match.Groups["comment"].Value.Trim());
            }

            return indexedLootComments;
        }

        private Blood TfsXmlToGenericBlood(string blood)
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

        private void TfsXmlSpellsToGeneric(ref Monster monster, Attack[] spells, SpellCategory category)
        {
            if (spells != null)
            {
                foreach (var attack in spells)
                {
                    Spell spell = new Spell() { SpellCategory = category };

                    if (!string.IsNullOrWhiteSpace(attack.script))
                    {
                        spell.Name = attack.script;
                        spell.DefinitionStyle = SpellDefinition.TfsLuaScript;
                    }
                    else
                    {
                        // Pure XML spell name or spell name registered within spell system
                        spell.Name = attack.name;
                        spell.DefinitionStyle = SpellDefinition.Raw;
                    }

                    if (attack.interval != 0)
                    {
                        spell.Interval = attack.interval;
                    }
                    else if (attack.speed != 0)
                    {
                        spell.Interval = attack.speed;
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
                        spell.Range = (int?)attack.range;
                    }

                    if (attack.length > 0)
                    {
                        spell.Length = (int?)attack.length;
                        spell.Spread = (attack.spread == -1) ? 3 : attack.spread;
                    }

                    if (attack.radius > 0)
                    {
                        spell.Radius = attack.radius;
                        spell.OnTarget = (attack.target == 1);
                    }

                    if (attack.target != -1)
                    {
                        spell.OnTarget = (attack.target == 1);
                    }

                    if (attack.direction != -1)
                    {
                        spell.IsDirectional = (attack.direction == 1);
                    }

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
                            spell.Condition = conditionNamesToConditionType[attack.name];
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

                        if (combatNamesToCombatDamage.ContainsKey(spell.Name))
                        {
                            spell.DamageElement = combatNamesToCombatDamage[spell.Name];
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

        private double TfsXmltoGenericElementalPercent(int percent)
        {
            return (1 - ((double)percent / 100));
        }

        private void Serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"{CurrentFileName} Unknown Node:{e.Name} = {e.Text}");
        }

        private void Serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            System.Xml.XmlAttribute attr = e.Attr;
            System.Diagnostics.Debug.WriteLine($"{CurrentFileName} Unknown attribute:{attr.Name} = {attr.Value}");
        }
    }
}
