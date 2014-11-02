using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTMonsterConverter
{
    interface IGenericMonster
    {
        void ReadMonster(string filename);
        void WriteMonster(string filename);
    }

    public class DetailedLookType
    {
        public uint head;
        public uint body;
        public uint legs;
        public uint feet;
    }

    //public class Summon
    //{
    //    public string name;
    //    public uint rate;
    //    public uint chance;
    //}

    //public class Loot
    //{
    //    //todo: name count be ID or string
    //    public uint max;
    //    public uint chance;
    //}

    abstract class GenericMonster : IGenericMonster
    {
        // General
        public string name;
        public string description;
        public uint health;
        public List<string> voices;
        public uint experience;
        public uint speed;
        //public uint maxSummons;
        //race or blood?
        //public List<Summon> summons;

        // Look
        public uint corpseId;
        public uint lookId;
        public DetailedLookType lookTypeDetails;

        // Behavior
        public uint summonable;
        public bool hostile;
        public bool illusionable;
        public uint convinceable;
        public bool pushable;
        public bool pushItems;
        public bool pushCreatures;
        public bool targetDistance;
        public uint runOnHealth;

        // Defeneses
        public uint armor;
        public uint fire;
        public uint earth;
        public uint energy;
        public uint ice;
        public uint holy;
        public uint death;
        public uint physical;
        public uint drown;

        // Immunities
        public bool paralyze;
        public bool invisible;
        public bool lifeDrain;
        public bool drunk;
        public bool manaDrain;



        public GenericMonster()
        {
            voices = new List<string>();
        }

        public abstract void ReadMonster(string filename);

        public abstract void WriteMonster(string filename);
    }
}
