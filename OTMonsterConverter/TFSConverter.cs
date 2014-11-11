using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace OTMonsterConverter
{
    //https://github.com/otland/forgottenserver/blob/master/src/monsters.cpp

    class TFSConverter : CommonConverter
    {
        // Constructor
        public TFSConverter()
            : base()
        {
        }

        // Functions
        public override void ReadMonster(string filename, out ICustomMonster monster)
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
        }

        public override void WriteMonster(string filename, ref ICustomMonster monster)
        {
            XDocument xDoc = XDocument.Load(filename);
            xDoc.Root.Add(new XElement("monster",
                            new XAttribute("name", monster.Name),
                            new XAttribute("nameDescription", monster.Description),
                            new XAttribute("experience", monster.Experience),
                            new XAttribute("speed", monster.Speed)
                        ));
            xDoc.Save(filename);
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
                            CorpseId       = (uint)tfsMonster.look.corpse,
                            Race           = tfsToGenericBlood(tfsMonster.race),
                            RetargetChance = (uint)tfsMonster.targetchange.chance,
                            MaxSummons     = (uint)tfsMonster.summons.maxSummons
                        };

            if (tfsMonster.look.type != 0)
            {
                monster.OutfitIdLookType      = (uint)tfsMonster.look.type;
            }
            if (tfsMonster.look.head != -99)
            {
                monster.LookTypeDetails = new DetailedLookType()
                                            {
                                                Head = (ushort)tfsMonster.look.head,
                                                Body = (ushort)tfsMonster.look.body,
                                                Legs = (ushort)tfsMonster.look.legs,
                                                Feet = (ushort)tfsMonster.look.feet
                                            };
            }

            // sounds
            if (tfsMonster.voices != null)
            {
                foreach (Voice sound in tfsMonster.voices.voice)
                {
                    monster.Voices.Add(sound.sentence);
                }
            }

            // summons
            if (tfsMonster.summons.summon != null)
            {
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

            // Defenses
            monster.TotalArmor = tfsMonster.defenses.armor;
            monster.TotalArmor = tfsMonster.defenses.defense;
            #region parseElements
            foreach (Element element in tfsMonster.elements.element)
            {
                if (element.physicalPercent != -9999)
                {
                    monster.Physical = tfstoGenericElementalPercent(element.physicalPercent);
                }
                else if (element.icePercent != -9999)
                {
                    monster.Ice = tfstoGenericElementalPercent(element.icePercent);
                }
                else if (element.poisonPercent != -9999) //poisonPercent and earthPercent are the same
                {
                    monster.Earth = tfstoGenericElementalPercent(element.poisonPercent);
                }
                else if (element.earthPercent != -9999) //poisonPercent and earthPercent are the same
                {
                    monster.Earth = tfstoGenericElementalPercent(element.earthPercent);
                }
                else if (element.firePercent != -9999)
                {
                    monster.Fire = tfstoGenericElementalPercent(element.firePercent);
                }
                else if (element.energyPercent != -9999)
                {
                    monster.Energy = tfstoGenericElementalPercent(element.energyPercent);
                }
                else if (element.holyPercent != -9999)
                {
                    monster.Holy = tfstoGenericElementalPercent(element.holyPercent);
                }
                else if (element.deathPercent != -9999)
                {
                    monster.Death = tfstoGenericElementalPercent(element.deathPercent);
                }
                else if (element.drownPercent != -9999)
                {
                    monster.Drown = tfstoGenericElementalPercent(element.drownPercent);
                }
                else if (element.lifedrainPercent != -9999)
                {
                    monster.LifeDrain = tfstoGenericElementalPercent(element.lifedrainPercent);
                }
                else if (element.manadrainPercent != -9999)
                {
                    monster.ManaDrain = tfstoGenericElementalPercent(element.manadrainPercent);
                }
            }
            #endregion
            #region paraseImmunities
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
                            Console.WriteLine("Unknown Immunity Bleed");
                            break;
                    }
                }
                else if (immunity.physical != -9999)
                {
                    monster.Physical = 0;
                }
                else if (immunity.energy != -9999)
                {
                    monster.Energy = 0;
                }
                else if (immunity.fire != -9999)
                {
                    monster.Fire = 0;
                }
                else if (immunity.poison != -9999) //poison and earth are the same
                {
                    monster.Earth = 0;
                }
                else if (immunity.earth != -9999) //poison and earth are the same
                {
                    monster.Earth = 0;
                }
                else if (immunity.drown != -9999)
                {
                    monster.Drown = 0;
                }
                else if (immunity.ice != -9999)
                {
                    monster.Ice = 0;
                }
                else if (immunity.holy != -9999)
                {
                    monster.Holy = 0;
                }
                else if (immunity.death != -9999)
                {
                    monster.Death = 0;
                }
                else if (immunity.lifedrain != -9999)
                {
                    monster.LifeDrain = 0;
                }
                else if (immunity.manadrain != -9999)
                {
                    monster.ManaDrain = 0;
                }
                else if (immunity.paralyze != -9999)
                {
                    monster.IgnoreParalyze = true;
                }
                else if (immunity.outfit != -9999)
                {
                    monster.IgnoreOutfit = true;
                }
                else if (immunity.bleed != -9999)
                {
                    Console.WriteLine("Unknown Immunity Bleed");
                }
                else if (immunity.drunk != -9999)
                {
                    monster.IgnoreDrunk = true;
                }
                else if (immunity.invisible != -9999) //invisible and invisibility are the same
                {
                    monster.IgnoreInvisible = true;
                }
                else if (immunity.invisibility != -9999) //invisible and invisibility are the same
                {
                    monster.IgnoreInvisible = true;
                }
            }
            #endregion
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
            System.Xml.XmlAttribute attr = e.Attr;
            Console.WriteLine("Unknown attribute " + attr.Name + "='" + attr.Value + "'");
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
        public string race; //todo: this could be number or string
        [XmlAttribute]
        public int experience;
        [XmlAttribute]
        public int speed;
        [XmlAttribute]
        public int manacost;
        [XmlAttribute]
        public string skull; //uses strings, "none", "yellow", "green", "white", "red", "black", "orange"
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
        public Loot loot;
        public Elements elements;
        public TFSXmlSummons summons;
    }

    [XmlRoot(ElementName = "health")]
    public class TFSXmlHealth
    {
        [XmlAttribute]
        public int now;
        [XmlAttribute]
        public int max;
    }


    [XmlRoot(ElementName = "flags")]
    public class Flags
    {
        [XmlElementAttribute]
        public Flag[] flag;
    }

    public class Flag
    {
        [XmlAttribute]
        public int summonable = -99;
        [XmlAttribute]
        public int attackable = -99;
        [XmlAttribute]
        public int hostile = -99;
        [XmlAttribute]
        public int illusionable = -99;
        [XmlAttribute]
        public int convinceable = -99;
        [XmlAttribute]
        public int pushable = -99;
        [XmlAttribute]
        public int canpushitems = -99;
        [XmlAttribute]
        public int canpushcreatures = -99;
        [XmlAttribute]
        public int targetdistance = -99;
        [XmlAttribute]
        public int staticattack = -99;
        [XmlAttribute]
        public int lightlevel = -99;
        [XmlAttribute]
        public int lightcolor = -99;
        [XmlAttribute]
        public int runonhealth = -99;
        [XmlAttribute]
        public int hidehealth = -99;
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
        public int type = -99;
        [XmlAttribute]
        public int head = -99; //only can exist if type exists
        [XmlAttribute]
        public int body = -99; //only can exist if type exists
        [XmlAttribute]
        public int legs = -99; //only can exist if type exists
        [XmlAttribute]
        public int feet = -99; //only can exist if type exists
        [XmlAttribute]
        public int addons = -99; //only can exist if type exists
        [XmlAttribute]
        public int typeex;
        [XmlAttribute]
        public int mount;
        [XmlAttribute]
        public int corpse;
    }

    public class Attacks
    {
        [XmlElementAttribute]
        public Attack[] attack;
    }

    public class Attack
    {
        //[XmlAttribute]
        //public int script; //todo: how to handle
        [XmlAttribute]
        public string name;
        public int interval = 2000; //interval and speed are the same //defaults to 2000 if missing
        [XmlAttribute]
        public int speed = 2000; //interval and speed are the same //defaults to 2000 if missing

        //[XmlAttribute]
        //public int chance = 100; //defaults to 100 if missing
        //[XmlAttribute]
        //public int range = 0; //defaults to 0 if missing
        //[XmlAttribute]
        //public int min = 0; //defaults to 0 if missing
        //[XmlAttribute]
        //public int max = 0; //defaults to 0 if missing
        //[XmlAttribute]
        //public int length = 0; //if length exists spread defaults to 3
        //[XmlAttribute]
        //public int radius = 0;

        // the following only exist if name is melee
        // when melee exists minMax and Max are set to 0
        // interval is set to 200
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
        public int tick; //only used if a condition is set
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
        public int physical = -9999;
        [XmlAttribute]
        public int energy = -9999;
        [XmlAttribute]
        public int fire = -9999;
        [XmlAttribute]
        public int poison = -9999; //poison and earth are the same
        [XmlAttribute]
        public int earth = -9999; //poison and earth are the same
        [XmlAttribute]
        public int drown = -9999;
        [XmlAttribute]
        public int ice = -9999;
        [XmlAttribute]
        public int holy = -9999;
        [XmlAttribute]
        public int death = -9999;
        [XmlAttribute]
        public int lifedrain = -9999;
        [XmlAttribute]
        public int manadrain = -9999;
        [XmlAttribute]
        public int paralyze = -9999;
        [XmlAttribute]
        public int outfit = -9999;
        [XmlAttribute]
        public int bleed = -9999;
        [XmlAttribute]
        public int drunk = -9999;
        [XmlAttribute]
        public int invisible = -9999; //invisible and invisibility are the same
        [XmlAttribute]
        public int invisibility = -9999; //invisible and invisibility are the same
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
        [XmlAttribute]
        public string yell; //can be 1 or true? //if it doesnt exist the value is false
    }

    public class Loot
    {
        [XmlElementAttribute]
        public Item[] item;
    }

    public class Item
    {
        [XmlAttribute]
        public string id;
        [XmlAttribute]
        public int countmax = 1; //default value is 1
        [XmlAttribute]
        public int chance; //chance and chance1 are the same
        [XmlAttribute]
        public int chance1; //chance and chance1 are the same

        //optional
        [XmlAttribute]
        public int subtype; //used for charges?
        [XmlAttribute]
        public int actionId;
        [XmlAttribute]
        public string test; //used for? //Id guess to override the fault item name string?
    }

    public class Elements
    {
        [XmlElementAttribute]
        public Element[] element;
    }

    public class Element
    {
        [XmlAttribute]
        public int physicalPercent = -9999;
        [XmlAttribute]
        public int icePercent = -9999;
        [XmlAttribute]
        public int poisonPercent = -9999; //poisonPercent and earthPercent are the same
        [XmlAttribute]
        public int earthPercent = -9999; //poisonPercent and earthPercent are the same
        [XmlAttribute]
        public int firePercent = -9999;
        [XmlAttribute]
        public int energyPercent = -9999;
        [XmlAttribute]
        public int holyPercent = -9999;
        [XmlAttribute]
        public int deathPercent = -9999;
        [XmlAttribute]
        public int drownPercent = -9999;
        [XmlAttribute]
        public int lifedrainPercent = -9999;
        [XmlAttribute]
        public int manadrainPercent = -9999;
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
