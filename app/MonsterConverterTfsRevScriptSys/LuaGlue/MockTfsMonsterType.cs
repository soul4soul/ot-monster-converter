using MonsterConverterInterface.MonsterTypes;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonsterConverterTfsRevScriptSys
{
    [MoonSharpUserData]
    internal class MockTfsMonsterType
    {
        public string Name { get; set; }

        public MockTfsMonsterType(string name)
        {
            name = Name;
        }

        public void register(Table t)
        {
            Monster mon = new Monster();
            DynValue dv;

            dv = t.Get("description");
            if (dv.Type == DataType.String)
            {
                mon.Description = dv.String;
            }

            dv = t.Get("experience");
            if (dv.Type == DataType.Number)
            {
                mon.Experience = (int)dv.Number;
            }

            /*
            dv = t.Get("maxHealth");
            if (dv.Type == DataType.Number)
            {
                mon.Health = (int)dv.Number;
            }
            */

            dv = t.Get("health");
            if (dv.Type == DataType.Number)
            {
                mon.Health = (int)dv.Number;
            }

            dv = t.Get("runHealth");
            if (dv.Type == DataType.Number)
            {
                mon.RunOnHealth = (int)dv.Number;
            }

            dv = t.Get("speed");
            if (dv.Type == DataType.Number)
            {
                mon.Speed = (int)dv.Number;
            }

            dv = t.Get("light");
            if (dv.Type == DataType.Table)
            {
                Table light = dv.Table;
                dv = light.Get("color");
                if (dv.Type == DataType.Number)
                {
                    mon.LightColor = (int)dv.Number;
                }

                dv = light.Get("level");
                if (dv.Type == DataType.Number)
                {
                    mon.LightLevel = (int)dv.Number;
                }
            }

            dv = t.Get("changeTarget");
            if (dv.Type == DataType.Table)
            {
                Table light = dv.Table;
                dv = light.Get("chance");
                if (dv.Type == DataType.Number)
                {
                    mon.RetargetChance = (int)dv.Number;
                }

                dv = light.Get("interval");
                if (dv.Type == DataType.Number)
                {
                    mon.RetargetInterval = (int)dv.Number;
                }
            }

            int summonCost = 0;
            dv = t.Get("manaCost");
            if (dv.Type == DataType.Number)
            {
                summonCost = (int)dv.Number;
            }

            dv = t.Get("flags");
            if (dv.Type == DataType.Table)
            {
                Table flags = dv.Table;
                dv = flags.Get("attackable");
                if (dv.Type == DataType.Boolean)
                {
                    mon.Attackable = dv.Boolean;
                }

                dv = flags.Get("healthHidden");
                if (dv.Type == DataType.Boolean)
                {
                    mon.HideHealth = dv.Boolean;
                }

                dv = flags.Get("convinceable");
                if (dv.Type == DataType.Boolean)
                {
                    mon.ConvinceCost = summonCost;
                }

                dv = flags.Get("summonable");
                if (dv.Type == DataType.Boolean)
                {
                    mon.SummonCost = summonCost;
                }

                dv = flags.Get("illusionable");
                if (dv.Type == DataType.Boolean)
                {
                    mon.IsIllusionable = dv.Boolean;
                }

                dv = flags.Get("hostile");
                if (dv.Type == DataType.Boolean)
                {
                    mon.IsHostile = dv.Boolean;
                }

                dv = flags.Get("pushable");
                if (dv.Type == DataType.Boolean)
                {
                    mon.IsPushable = dv.Boolean;
                }

                dv = flags.Get("canPushItems");
                if (dv.Type == DataType.Boolean)
                {
                    mon.PushItems = dv.Boolean;
                }

                dv = flags.Get("canPushCreatures");
                if (dv.Type == DataType.Boolean)
                {
                    mon.PushCreatures = dv.Boolean;
                }

                dv = flags.Get("targetDistance");
                if (dv.Type == DataType.Number)
                {
                    mon.TargetDistance = (int)dv.Number;
                }

                dv = flags.Get("staticAttackChance");
                if (dv.Type == DataType.Number)
                {
                    mon.StaticAttackChance = (int)dv.Number;
                }
            }

            dv = t.Get("corpse");
            if (dv.Type == DataType.Number)
            {
                mon.Look.CorpseId = (ushort)dv.Number;
            }
            dv = t.Get("outfit");
            if (dv.Type == DataType.Table)
            {
                Table outfit = dv.Table;

                dv = outfit.Get("lookTypeEx");
                if (dv.Type == DataType.Number)
                {
                    mon.Look.LookType = LookType.Item;
                    mon.Look.LookId = (int)dv.Number;
                }

                dv = outfit.Get("lookType");
                if (dv.Type == DataType.Number)
                {
                    mon.Look.LookType = LookType.Outfit;
                    mon.Look.LookId = (int)dv.Number;
                }
                dv = outfit.Get("lookHead");
                if (dv.Type == DataType.Number)
                {
                    mon.Look.Head = (int)dv.Number;
                }
                dv = outfit.Get("lookBody");
                if (dv.Type == DataType.Number)
                {
                    mon.Look.Body = (int)dv.Number;
                }
                dv = outfit.Get("lookLegs");
                if (dv.Type == DataType.Number)
                {
                    mon.Look.Legs = (int)dv.Number;
                }
                dv = outfit.Get("lookFeet");
                if (dv.Type == DataType.Number)
                {
                    mon.Look.Feet = (int)dv.Number;
                }
                dv = outfit.Get("lookAddons");
                if (dv.Type == DataType.Number)
                {
                    mon.Look.Addons = (int)dv.Number;
                }
                dv = outfit.Get("lookMount");
                if (dv.Type == DataType.Number)
                {
                    mon.Look.Mount = (int)dv.Number;
                }
            }

            dv = t.Get("maxSummons");
            if (dv.Type == DataType.Number)
            {
                mon.MaxSummons = (ushort)dv.Number;
            }
            dv = t.Get("summons");
            if (dv.Type == DataType.Table)
            {
                Table summons = dv.Table;
                for (int i = 1; i <= summons.Length; i++)
                {
                    dv = summons.Get(i);
                    if (dv.Type == DataType.Table)
                    {
                        mon.Summons.Add(GetSummon(dv.Table));
                    }
                }
            }

            mon.Race = TfsRevScriptSysRaceToGenericBlood(t.Get("race"));

            dv = t.Get("voices");
            if (dv.Type == DataType.Table)
            {
                Table voices = dv.Table;

                dv = voices.Get("interval");
                if (dv.Type == DataType.Number)
                {
                    mon.VoiceInterval = (int)dv.Number;
                }

                dv = voices.Get("chance");
                if (dv.Type == DataType.Number)
                {
                    mon.VoiceChance = (int)dv.Number;
                }

                for (int i = 1; i <= voices.Length; i++)
                {
                    dv = voices.Get(i);
                    if (dv.Type == DataType.Table)
                    {
                        mon.Voices.Add(GetVoice(dv.Table));
                    }
                }
            }

            // use signal/event to get data out of this function?

            return;
        }

        private Voice GetVoice(Table t)
        {
            DynValue dv;
            Voice voice = null;

            dv = t.Get("text");
            if (dv.Type == DataType.String)
            {
                voice = new Voice(dv.String);
            }
            dv = t.Get("yell");
            if (dv.Type == DataType.String)
            {
                voice.SoundLevel = dv.Boolean ? SoundLevel.Yell : SoundLevel.Say;
            }

            return voice;
        }

        private Summon GetSummon(Table t)
        {
            DynValue dv;
            Summon summon = new Summon();

            dv = t.Get("name");
            if (dv.Type == DataType.String)
            {
                summon.Name = dv.String;
            }
            dv = t.Get("interval");
            if (dv.Type == DataType.Number)
            {
                summon.Interval = (int)dv.Number;
            }
            dv = t.Get("chance");
            if (dv.Type == DataType.Number)
            {
                summon.Chance = dv.Number;
            }

            return summon;
        }


        private Blood TfsRevScriptSysRaceToGenericBlood(DynValue blood)
        {
            Blood race = Blood.blood; //default

            if (blood.Type == DataType.String)
            {
                switch (blood.String)
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
            else if (blood.Type == DataType.Number)
            {
                switch (blood.Number)
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

    }
}
