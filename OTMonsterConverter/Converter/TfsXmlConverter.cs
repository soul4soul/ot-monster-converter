using OTMonsterConverter.MonsterTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace OTMonsterConverter.Converter
{
    //https://github.com/otland/forgottenserver/blob/master/src/monsters.cpp

    class TfsXmlConverter : IMonsterConverter
    {
        const uint MAX_LOOTCHANCE = 100000;
        const uint ATTACK_INTERVAL_DEFAULT = 2000;

        IDictionary<string, Effect> magicEffectNames = new Dictionary<string, Effect>
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

        IDictionary<string, Animation> shootTypeNames = new Dictionary<string, Animation>
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

        IDictionary<string, CombatDamage> CombatDamageNames = new Dictionary<string, CombatDamage>
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

        public string FileExtRegEx { get => "*.xml"; }

        // Functions
        public bool ReadMonster(string filename, out Monster monster)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TFSXmlMonster));

            serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
            serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);

            // A FileStream is needed to read the XML document.
            FileStream fs = new FileStream(filename, FileMode.Open);

            // Use the Deserialize method to restore the object's state with data from the XML document.
            TFSXmlMonster tfsMonster = (TFSXmlMonster)serializer.Deserialize(fs);

            // convert from xml monster classes to generic class
            xmlToGeneric(tfsMonster, out monster);
            monster.FileName = Path.GetFileNameWithoutExtension(filename);

            return true;
        }

        public bool WriteMonster(string directory, ref Monster monster)
        {
            string fileName = Path.Combine(directory, monster.Name.ToLower());

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
                Race = tfsToGenericBlood(tfsMonster.race),
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
                        Chance = (uint)Math.Min(100, summon.chance),
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
                monster.Attacks = XmlSpellsToGeneric(tfsMonster.attacks.attack);
            }

            // Defenses
            if (tfsMonster.defenses != null)
            {
                monster.TotalArmor = tfsMonster.defenses.armor;
                monster.Shielding = tfsMonster.defenses.defense;
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
                            monster.Physical = tfstoGenericElementalPercent(value);
                        }
                        else if (x.attr[0].Name == "icePercent")
                        {
                            monster.Ice = tfstoGenericElementalPercent(value);
                        }
                        else if (x.attr[0].Name == "poisonPercent")
                        {
                            monster.Earth = tfstoGenericElementalPercent(value);
                        }
                        else if (x.attr[0].Name == "earthPercent")
                        {
                            monster.Earth = tfstoGenericElementalPercent(value);
                        }
                        else if (x.attr[0].Name == "firePercent")
                        {
                            monster.Fire = tfstoGenericElementalPercent(value);
                        }
                        else if (x.attr[0].Name == "energyPercent")
                        {
                            monster.Energy = tfstoGenericElementalPercent(value);
                        }
                        else if (x.attr[0].Name == "holyPercent")
                        {
                            monster.Holy = tfstoGenericElementalPercent(value);
                        }
                        else if (x.attr[0].Name == "deathPercent")
                        {
                            monster.Death = tfstoGenericElementalPercent(value);
                        }
                        else if (x.attr[0].Name == "drownPercent")
                        {
                            monster.Drown = tfstoGenericElementalPercent(value);
                        }
                        else if (x.attr[0].Name == "lifedrainPercent")
                        {
                            monster.LifeDrain = tfstoGenericElementalPercent(value);
                        }
                        else if (x.attr[0].Name == "manadrainPercent")
                        {
                            monster.ManaDrain = tfstoGenericElementalPercent(value);
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

        private Blood tfsToGenericBlood(string blood)
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

        private IList<Spell> XmlSpellsToGeneric(Attack[] spells)
        {
            IList<Spell> monSpells = new List<Spell>();
            if (spells != null)
            {
                foreach (var attack in spells)
                {
                    Spell spell = new Spell();
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

                    spell.Chance = (uint)attack.chance;

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
                            spell.Condition = Condition.Fire;
                            spell.StartDamage = attack.poison;
                            spell.Tick = (attack.tick != 0) ? attack.tick : 9000;
                        }
                        else if (attack.poison != 0)
                        {
                            spell.Condition = Condition.Poison;
                            spell.StartDamage = attack.poison;
                            spell.Tick = (attack.tick != 0) ? attack.tick : 4000;
                        }
                        else if (attack.energy != 0)
                        {
                            spell.Condition = Condition.Energy;
                            spell.StartDamage = attack.energy;
                            spell.Tick = (attack.tick != 0) ? attack.tick : 10000;
                        }
                        else if (attack.drown != 0)
                        {
                            spell.Condition = Condition.Drown;
                            spell.StartDamage = attack.drown;
                            spell.Tick = (attack.tick != 0) ? attack.tick : 5000;
                        }
                        else if (attack.dazzle != 0)
                        {
                            spell.Condition = Condition.Dazzled;
                            spell.StartDamage = attack.dazzle;
                            spell.Tick = (attack.tick != 0) ? attack.tick : 10000;
                        }
                        else if (attack.curse != 0)
                        {
                            spell.Condition = Condition.Cursed;
                            spell.StartDamage = attack.curse;
                            spell.Tick = (attack.tick != 0) ? attack.tick : 4000;
                        }
                        else if (attack.bleed != 0)
                        {
                            spell.Condition = Condition.Bleeding;
                            spell.StartDamage = attack.bleed;
                            spell.Tick = (attack.tick != 0) ? attack.tick : 4000;
                        }
                        else if (attack.physical != 0)
                        {
                            spell.Condition = Condition.Bleeding;
                            spell.StartDamage = attack.physical;
                            spell.Tick = (attack.tick != 0) ? attack.tick : 4000;
                        }
                    }
                    else if (attack.name == "speed")
                    {
                        spell.SpeedChange = attack.speedchange;
                        spell.Duration = attack.duration;
                        if (attack.duration == 0)
                        {
                            spell.Duration = 10000; // Default when no duration set
                        }
                    }
                    else
                    {
                        // Always default both if max is included to be explicit
                        // Some spells don't have damage so don't include either of them
                        if (attack.max != 0)
                        {
                            spell.MinDamage = attack.min;
                            spell.MaxDamage = attack.max;
                        }

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
                        spell.Target = (attack.target == 1);

                        if (CombatDamageNames.ContainsKey(spell.Name))
                        {
                            spell.DamageElement = CombatDamageNames[spell.Name];
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

                    monSpells.Add(spell);
                }
            }
            return monSpells;
        }

        private string generictoTfsBlood(Blood race)
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

        private double tfstoGenericElementalPercent(int percent)
        {
            return (1 - ((double)percent / 100));
        }

        private void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Unknown Node:" + e.Name + "\t" + e.Text);
        }

        private void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            System.Xml.XmlAttribute attr = e.Attr;
            System.Diagnostics.Debug.WriteLine("Unknown attribute " + attr.Name + "='" + attr.Value + "'");
        }
    }

    #region XML serializer classes
    public enum namedImmunityXml
    {
        physical,
        energy,
        fire,
        poison,
        earth = namedImmunityXml.poison,
        drown,
        ice,
        holy,
        death,
        lifedrain,
        manadrain,
        paralyze,
        outfit,
        drunk,
        invisible,
        invisibility = namedImmunityXml.invisible,
        bleed,
        NA
    }

    enum skullsXml
    {
        none = 0,
        yellow,
        green,
        white,
        red,
        black,
        orange
    }

    [Serializable, XmlRoot("monster")]
    public class TFSXmlMonster
    {
        [XmlAttribute]
        public string name;
        [XmlAttribute]
        public string nameDescription;
        [XmlAttribute]
        public string namedescription;
        [XmlAttribute]
        public string race = "blood";
        [XmlAttribute]
        public int experience = 0;
        [XmlAttribute]
        public int speed = 200;
        [XmlAttribute]
        public int manacost = 0;
        [XmlAttribute]
        public string skull = "none"; //uses strings, "none", "yellow", "green", "white", "red", "black", "orange"
        //[XmlAttribute]
        //public int script; //todo: how to handle

        public TFSXmlHealth health;
        public Flags flags;
        public Look look;
        public TargetChange targetchange;
        public Attacks attacks;
        public Defenses defenses;
        public Immunities immunities;
        public Voices voices;
        public TfsXmlLoot loot;
        public Elements elements;
        public TFSXmlSummons summons;
    }

    [XmlRoot(ElementName = "health")]
    public class TFSXmlHealth
    {
        [XmlAttribute]
        public int now = 100;
        [XmlAttribute]
        public int max = 100;
    }

    [XmlRoot(ElementName = "flags")]
    public class Flags
    {
        [XmlElementAttribute]
        public MultiAttr[] flag;
    }

    public class MultiAttr
    {
        [XmlAnyAttribute]
        public XmlAttribute[] attr;
    }

    public class TargetChange
    {
        [XmlAttribute]
        public int interval = 0; //interval and speed are the same, default is 0
        [XmlAttribute]
        public int speed = 0; //interval and speed are the same, default is 0
        [XmlAttribute]
        public int chance = 0; //default is 0
    }

    public class Look
    {
        [XmlAttribute]
        public int type = 0;
        [XmlAttribute]
        public int head = 0; //only can exist if type exists
        [XmlAttribute]
        public int body = 0; //only can exist if type exists
        [XmlAttribute]
        public int legs = 0; //only can exist if type exists
        [XmlAttribute]
        public int feet = 0; //only can exist if type exists
        [XmlAttribute]
        public int addons = 0; //only can exist if type exists
        [XmlAttribute]
        public int typeex = 0;
        [XmlAttribute]
        public int mount = 0;
        [XmlAttribute]
        public int corpse = 0;
    }

    public class TfsXmlSpellAttributes
    {
        [XmlAttribute]
        public string key { get; set; }

        [XmlAttribute]
        public string value { get; set; }
    }

    public class Attacks
    {
        [XmlElementAttribute]
        public Attack[] attack;
    }

    public class Attack
    {
        // only script or name not both
        //[XmlAttribute]
        //public string script;
        [XmlAttribute]
        public string name;

        // Only one should exist, they represent the same information
        [XmlAttribute]
        public int interval = 0; //defaults to 2000 if missing, default is handled in parsing
        [XmlAttribute]
        public int speed = 0; //defaults to 2000 if missing, default is handled in parsing

        [XmlAttribute]
        public int chance = 100; //defaults to 100 if missing
        [XmlAttribute]
        public int range = 0; //defaults to 0 if missing
        [XmlAttribute]
        public int min = 0; //defaults to 0 if missing
        [XmlAttribute]
        public int max = 0; //defaults to 0 if missing
        [XmlAttribute]
        public int length = 0; //if length exists spread defaults to 3
        [XmlAttribute]
        public int spread = 0; //if length exists spread defaults to 3
        [XmlAttribute]
        public int radius = 0;
        [XmlAttribute]
        public int target = 0; // Defaults to 0 if missing

        [XmlAttribute]
        public int speedchange = 0;
        [XmlAttribute]
        public int duration = 0;

        [XmlElementAttribute(ElementName = "attribute")]
        public TfsXmlSpellAttributes[] attribute { get; set; }

        // the following only exist when attack name is melee
        // when melee exists minMax and Max are set to 0
        [XmlAttribute]
        public int skill;
        [XmlAttribute]
        public int attack;
        [XmlAttribute]
        public int fire;
        [XmlAttribute]
        public int poison;
        [XmlAttribute]
        public int energy;
        [XmlAttribute]
        public int drown;
        [XmlAttribute]
        public int freeze;
        [XmlAttribute]
        public int dazzle;
        [XmlAttribute]
        public int curse;
        [XmlAttribute]
        public int bleed; //bleed and physical are the same
        [XmlAttribute]
        public int physical; //bleed and physical are the same

        [XmlAttribute]
        public int tick; //only used if a condition is set each type has its own default tick which can be overriden with this attr
        [XmlAttribute]
        public int start; //Start condition damage

        [XmlAttribute]
        public string monster;
        [XmlAttribute]
        public int item;
    }

    public class Defenses
    {
        [XmlAttribute]
        public uint defense;
        [XmlAttribute]
        public uint armor;

        [XmlElementAttribute(ElementName = "defense")]
        public Attack[] defenses;
    }

    public class Immunities
    {
        [XmlElementAttribute]
        public Immunity[] immunity;
    }

    public class Immunity
    {
        [XmlAttribute]
        public namedImmunityXml name = namedImmunityXml.NA;
        [XmlAttribute]
        public int physical = 0; //Immune to physical and bleeding condition
        [XmlAttribute]
        public int energy = 0;
        [XmlAttribute]
        public int fire = 0;
        [XmlAttribute]
        public int poison = 0; //poison and earth are the same
        [XmlAttribute]
        public int earth = 0; //poison and earth are the same
        [XmlAttribute]
        public int drown = 0;
        [XmlAttribute]
        public int ice = 0;
        [XmlAttribute]
        public int holy = 0;
        [XmlAttribute]
        public int death = 0;
        [XmlAttribute]
        public int lifedrain = 0;
        [XmlAttribute]
        public int manadrain = 0;
        [XmlAttribute]
        public int paralyze = 0;
        [XmlAttribute]
        public int outfit = 0; // TODO should be true by default?
        [XmlAttribute]
        public int bleed = 0; // immue to only bleed condition
        [XmlAttribute]
        public int drunk = 0;
        [XmlAttribute]
        public int invisible = 0; //invisible and invisibility are the same
        [XmlAttribute]
        public int invisibility = 0; //invisible and invisibility are the same
    }

    public class Voices
    {
        [XmlAttribute]
        public int interval; //interval and speed are the same
        [XmlAttribute]
        public int speed; //interval and speed are the same
        [XmlAttribute]
        public int chance;
        [XmlElementAttribute]
        public VoiceXml[] voice;
    }

    public class VoiceXml
    {
        [XmlAttribute]
        public string sentence;
        /// <summary>
        /// Can be 1 or true
        /// if it doesnt exist the value is false
        /// </summary>
        [XmlAttribute]
        public string yell;
    }

    [Serializable, XmlRoot("Loot")]
    public class TfsXmlLoot
    {
        [XmlElementAttribute]
        public Item[] item;
    }

    public class Item
    {
        // Only name or ID will be used not both
        [XmlAttribute]
        public string name;
        [XmlAttribute]
        public int id;
        [XmlAttribute]
        public int countmax = 1; //default value is 1
        [XmlAttribute]
        public int chance; //chance and chance1 are the same
        [XmlAttribute]
        public int chance1; //chance and chance1 are the same

        //optional
        //[XmlAttribute]
        //public int subtype; //used for charges?
        //[XmlAttribute]
        //public int actionId;
        //[XmlAttribute]
        //public string test; //used for? //Id guess to override the fault item name string?
    }

    public class Elements
    {
        [XmlElementAttribute]
        public MultiAttr[] element;
    }

    [XmlRoot(ElementName = "summons")]
    public class TFSXmlSummons
    {
        [XmlAttribute]
        public int maxSummons;
        [XmlElementAttribute]
        public TFSXmlSummon[] summon;
    }

    [XmlRoot(ElementName = "summon")]
    public class TFSXmlSummon
    {
        [XmlAttribute]
        public string name;
        [XmlAttribute]
        public int interval = 1000; //interval and speed are the same //defaults to 1000 if missing
        [XmlAttribute]
        public int speed = 1000; //interval and speed are the same //defaults to 1000 if missing
        [XmlAttribute]
        public int chance = 100; //defaults to 100 if missing
        [XmlAttribute]
        public int max;
        [XmlAttribute]
        public bool force;
    }
    #endregion
}
