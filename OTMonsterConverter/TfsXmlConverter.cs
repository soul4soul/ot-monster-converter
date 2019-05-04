using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace OTMonsterConverter
{
    //https://github.com/otland/forgottenserver/blob/master/src/monsters.cpp

    class TfsXmlConverter : CommonConverter
    {
        const uint MAX_LOOTCHANCE = 100000;
        const uint ATTACK_INTERVAL_DEFAULT = 2000;

        // Constructor
        public TfsXmlConverter()
            : base()
        {
        }

        // Functions
        public override bool ReadMonster(string filename, out ICustomMonster monster)
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

            return true;
        }

        public override bool WriteMonster(string directory, ref ICustomMonster monster)
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

        private void xmlToGeneric(TFSXmlMonster tfsMonster, out ICustomMonster monster)
        {
            monster = new CustomMonster()
            {
                Name           = tfsMonster.name,
                Description    = tfsMonster.nameDescription,
                Health         = (uint)tfsMonster.health.max,
                Experience     = (uint)tfsMonster.experience,
                Speed          = (uint)tfsMonster.speed,
                Race           = tfsToGenericBlood(tfsMonster.race),
            };

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
                    Console.WriteLine("Warning duplicate target speed and target interval");
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
                            Console.WriteLine("Can't parse static attack yet");
                            // TODO
                            //monster.StaticAttack = (uint)value;
                        }
                        else if (x.attr[0].Name == "lightlevel")
                        {
                            Console.WriteLine("Can't parse light yet");
                            // TODO
                            //monster.LightLevel = (uint)value;
                        }
                        else if (x.attr[0].Name == "lightcolor")
                        {
                            Console.WriteLine("Can't parse light yet");
                            // TODO
                            //monster.TargetDistance = (uint)value;
                        }
                        else if (x.attr[0].Name == "runonhealth")
                        {
                            monster.RunOnHealth = (uint)value;
                        }
                        else if (x.attr[0].Name == "hidehealth")
                        {
                            Console.WriteLine("Can't parse hide health yet");
                            // TODO
                            //monster.HideHealth = (uint)value;
                        }
                        else if (x.attr[0].Name == "canwalkonenergy")
                        {
                            monster.AvoidEnergy = value == 1;
                        }
                        else if (x.attr[0].Name == "canwalkonfire")
                        {
                            monster.AvoidFire = value == 1;
                        }
                        else if (x.attr[0].Name == "canwalkonpoison")
                        {
                            monster.AvoidPoison = value == 1;
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
                foreach (Voice sound in tfsMonster.voices.voice)
                {
                    ICustomVoice voice = new CustomVoice();
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
                    monster.Summons.Add(new CustomSummon()
                                            {
                                                Name = summon.name,
                                                Chance = (double)summon.chance / 100,
                                                Rate = (uint)((summon.interval > 0) ? summon.interval : summon.speed)
                                            });
                }
            }
            else
            {
                monster.MaxSummons = 0;
            }

            if ((tfsMonster.attacks != null) &&
                (tfsMonster.attacks.attack != null))
            {
                foreach (var attack in tfsMonster.attacks.attack)
                {
                    ISpells spell = new Spells();
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
                        // Has chance?
                        spell.MinDamage = (uint)attack.min;
                        spell.MaxDamage = (uint)attack.max;
                    }
                    else if (attack.name == "speed")
                    {

                    }
                    else
                    {
                        if (attack.attribute != null)
                        {
                            foreach (var attr in attack.attribute)
                            {
                                if (attr.key == "shootEffect")
                                {

                                }
                                else if (attr.key == "areaEffect")
                                {

                                }
                                else
                                {
                                    Console.WriteLine("Warning unkown attack attribute");
                                }
                            }
                        }

                        if ((attack.length != 0) && (attack.spread == 0))
                        {
                        }
                    }
                }
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
                foreach(var item in tfsMonster.loot.item)
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

                    ILoot commonItem = new Loot()
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
            Console.WriteLine("Unknown Node:" + e.Name + "\t" + e.Text);
        }

        private void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            //System.Xml.XmlAttribute attr = e.Attr;
            //Console.WriteLine("Unknown attribute " + attr.Name + "='" + attr.Value + "'");
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

    [Serializable, XmlRoot("attribute")]
    public class TfsXmlSpellAttributes
    {
        public string key { get; set; }
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

        public TfsXmlSpellAttributes[] attribute { get; set; }

        // the following only exist when attack name is melee
        // when melee exists minMax and Max are set to 0
        // interval is set to 200
        ////[XmlAttribute]
        ////public int skill;
        ////[XmlAttribute]
        ////public int attack;
        //[XmlAttribute]
        //public int fire;
        //[XmlAttribute]
        //public int poison;
        //[XmlAttribute]
        //public int energy;
        //[XmlAttribute]
        //public int drown;
        //[XmlAttribute]
        //public int freeze;
        //[XmlAttribute]
        //public int dazzle;
        //[XmlAttribute]
        //public int curse;
        //[XmlAttribute]
        //public int bleed; //bleed and physical are the same
        //[XmlAttribute]
        //public int physical; //bleed and physical are the same
        //[XmlAttribute]
        //public int tick; //only used if a condition is set
    }

    public class Defenses
    {
        [XmlAttribute]
        public uint defense;
        [XmlAttribute]
        public uint armor;

        //public defense[] defense
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
        public Voice[] voice;
    }

    public class Voice
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
    }
    #endregion
}
