using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MonsterConverterTfsXml
{
    public enum TfsXmlNamedImmunity
    {
        physical,
        energy,
        fire,
        poison,
        earth = TfsXmlNamedImmunity.poison,
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
        invisibility = TfsXmlNamedImmunity.invisible,
        bleed,
        NA
    }

    [XmlRoot("monster")]
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
        [XmlAttribute]
        public string script;

        public TfsXmlHealth health;
        public Flags flags;
        public Look look;
        public TargetChange targetchange;
        public Attacks attacks;
        public Defenses defenses;
        public Immunities immunities;
        public Voices voices;
        public TfsXmlLoot loot;
        public Elements elements;
        public TfsXmlSummons summons;
        [XmlElement(ElementName = "script")]
        public TfsXmlScripts scripts;
    }

    [XmlRoot(ElementName = "health")]
    public class TfsXmlHealth
    {
        [XmlAttribute]
        public int now = 100;
        [XmlAttribute]
        public int max = 100;
    }

    [XmlRoot(ElementName = "flags")]
    public class Flags
    {
        [XmlElement]
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
        public ushort corpse = 0;
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
        [XmlElement]
        public Attack[] attack;
    }

    public class Attack
    {
        // only script or name not both
        [XmlAttribute]
        public string script;
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
        public int spread = -1; //if length exists spread defaults to 3, default is handled in parsing
        [XmlAttribute]
        public int radius = 0;
        [XmlAttribute]
        public int ring = 0;
        [XmlAttribute]
        public int direction = -1; // 0/1, default is 0, set to 1 if length is set, only parsed if scripts attribute is set
        [XmlAttribute]
        public int target = -1; // 0/1, default is 0, parsed if script or radius attribute is set

        [XmlAttribute]
        public int speedchange = 0;
        [XmlAttribute]
        public int minspeedchange = 0;
        [XmlAttribute]
        public int maxspeedchange = 0;
        [XmlAttribute]
        public int duration = 10000;
        [XmlAttribute]
        public int drunkenness = 25;

        [XmlElement(ElementName = "attribute")]
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
        public ushort item;
    }

    public class Defenses
    {
        [XmlAttribute]
        public int defense;
        [XmlAttribute]
        public int armor;

        [XmlElement(ElementName = "defense")]
        public Attack[] defenses;
    }

    public class Immunities
    {
        [XmlElement]
        public Immunity[] immunity;
    }

    public class Immunity
    {
        [XmlAttribute]
        public TfsXmlNamedImmunity name = TfsXmlNamedImmunity.NA;
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
        public int outfit = 0;
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
        public double chance;
        [XmlElement]
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

    [XmlRoot("Loot")]
    public class TfsXmlLoot
    {
        [XmlElement]
        public Item[] item;
    }

    public class Item
    {
        // Only name or ID will be used not both
        [XmlAttribute]
        public string name;
        [XmlAttribute]
        public ushort id;
        [XmlAttribute]
        public int countmax = 1; //default value is 1
        [XmlAttribute]
        public int chance; //chance and chance1 are the same
        [XmlAttribute]
        public int chance1; //chance and chance1 are the same

        [XmlAttribute]
        public int subtype; //used for charges too
        [XmlAttribute]
        public int actionId;
        [XmlAttribute]
        public string text; //used for setting text to items like letters

        [XmlElement(ElementName = "item")]
        public Item[] NestedItems { get; set; }

        // Pre TFS 1.0 - Really is anybody using this anymore?
        [XmlElement(ElementName = "inside")]
        public Inside Inside { get; set; }
    }

    [XmlRoot(ElementName = "inside")]
    public class Inside
    {
        [XmlElement(ElementName = "item")]
        public Item[] NestedItems { get; set; }
    }

    public class Elements
    {
        [XmlElement]
        public MultiAttr[] element;
    }

    [XmlRoot(ElementName = "summons")]
    public class TfsXmlSummons
    {
        [XmlAttribute]
        public int maxSummons;
        [XmlElement]
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
        public double chance = 100; //defaults to 100 if missing
        [XmlAttribute]
        public int max;
        [XmlAttribute]
        public bool force;
    }

    [XmlRoot(ElementName = "event")]
    public class TfsXmlEvent
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "script")]
    public class TfsXmlScripts
    {
        [XmlElement(ElementName = "event")]
        public TfsXmlEvent[] Event { get; set; }
    }
}
