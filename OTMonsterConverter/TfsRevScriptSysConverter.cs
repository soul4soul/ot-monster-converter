using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTMonsterConverter
{
    class TfsRevScriptSysConverter : CommonConverter
    {
        const uint MAX_LOOTCHANCE = 100000;

        public override bool WriteMonster(string directory, ref ICustomMonster monster)
        {
            string lowerName = monster.Name.ToLower();

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            string titleName = textInfo.ToTitleCase(lowerName);

            Directory.CreateDirectory(directory);

            string fileName = Path.Combine(directory, titleName + ".lua");

            using (var dest = File.AppendText(fileName))
            {
                dest.WriteLine($"local mType = MonsterType(\"{titleName}\")");
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
                dest.WriteLine($"	isPushable = {monster.Pushable.ToString().ToLower()}");
                dest.WriteLine($"	illusionable = {monster.Illusionable.ToString().ToLower()},");
                dest.WriteLine($"	canPushItems = {monster.PushItems.ToString().ToLower()},");
                dest.WriteLine($"	canPushCreatures = {monster.PushCreatures.ToString().ToLower()},");
                dest.WriteLine($"	staticAttackChance = {monster.StaticAttack},");
                dest.WriteLine($"	lightlevel = {monster.LightLevel},");
                dest.WriteLine($"	lightcolor = {monster.LightColor},");
                dest.WriteLine($"	targetdistance = {monster.TargetDistance},");
                dest.WriteLine($"	runHealth = {monster.RunOnHealth},");
                dest.WriteLine($"	isHealthHidden = {monster.HideHealth.ToString().ToLower()},");
                dest.WriteLine($"	canwalkonenergy = {monster.AvoidEnergy.ToString().ToLower()},");
                dest.WriteLine($"	canwalkonfire = {monster.AvoidFire.ToString().ToLower()},");
                dest.WriteLine($"	canwalkonpoison = {monster.AvoidPoison.ToString().ToLower()}");
                dest.WriteLine("}");
                dest.WriteLine("");

                dest.WriteLine("monster.summons = {");
                string summon;
                for (int i = 0; i < monster.Summons.Count; i++)
                {
                    summon = $"	{{name = \"{monster.Summons[i].Name}\",chance = {monster.Summons[i].Chance}, interval = {monster.Summons[i].Rate}}},";
                    if (i == monster.Summons.Count - 1)
                    {
                        summon = summon.TrimEnd(',');
                    }
                    dest.WriteLine(summon);
                }
                dest.WriteLine("}");
                dest.WriteLine("");

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

                dest.WriteLine("monster.loot = {");
                string loot;
                for (int i = 0; i < monster.Items.Count; i++)
                {
                    decimal chance = monster.Items[i].Chance * MAX_LOOTCHANCE;
                    if (monster.Items[i].Count > 1)
                    {
                        loot = $"	{{id = \"{monster.Items[i].Item}\", chance = {chance:0}, maxCount = {monster.Items[i].Count}}},";
                    }
                    else
                    {
                        loot = $"	{{id = \"{monster.Items[i].Item}\", chance = {chance:0}}},";
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
                //"	{name = "melee", attack = 130, skill = 70, effect = CONST_ME_DRAWBLOOD, interval = 2*1000},",
                //"	{name = "energy strike", range = 1, chance = 10, interval = 2*1000, minDamage = -210, maxDamage = -300, target = ","true},",
                //"	{name = "combat", type = COMBAT_MANADRAIN, chance = 10, interval = 2*1000, minDamage = 0, maxDamage = -120, target ","= true, range = 7, effect = CONST_ME_MAGIC_BLUE},",
                //"	{name = "combat", type = COMBAT_FIREDAMAGE, chance = 20, interval = 2*1000, minDamage = -150, maxDamage = -250, ","radius = 1, target = true, effect = CONST_ME_FIREAREA, shootEffect = CONST_ANI_FIRE},",
                //"	{name = "speed", chance = 15, interval = 2*1000, speed = -700, radius = 1, target = true, duration = 30*1000, ","effect = CONST_ME_MAGIC_RED},",
                //"	{name = "firefield", chance = 10, interval = 2*1000, range = 7, radius = 1, target = true, shootEffect = ","CONST_ANI_FIRE},",
                //"	{name = "combat", type = COMBAT_LIFEDRAIN, chance = 10, interval = 2*1000, length = 8, spread = 0, minDamage = -300,"," maxDamage = -490, effect = CONST_ME_PURPLEENERGY}",
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
                dest.WriteLine($"	{{type = COMBAT_PHYSICALDAMAGE, percent = {GenericToTfsElemementPercent(monster.Physical)}}},");
                dest.WriteLine($"	{{type = COMBAT_ENERGYDAMAGE, percent = {GenericToTfsElemementPercent(monster.Energy)}}},");
                dest.WriteLine($"	{{type = COMBAT_EARTHDAMAGE, percent = {GenericToTfsElemementPercent(monster.Earth)}}},");
                dest.WriteLine($"	{{type = COMBAT_FIREDAMAGE, percent = {GenericToTfsElemementPercent(monster.Fire)}}},");
                dest.WriteLine($"	{{type = COMBAT_LIFEDRAIN, percent = {GenericToTfsElemementPercent(monster.LifeDrain)}}},");
                dest.WriteLine($"	{{type = COMBAT_MANADRAIN, percent = {GenericToTfsElemementPercent(monster.ManaDrain)}}},");
                //dest.WriteLine($"	{{type = COMBAT_HEALING, percent = {GenericToTfsElemementPercent(monster.XXXX)}}},");
                dest.WriteLine($"	{{type = COMBAT_DROWNDAMAGE, percent = {GenericToTfsElemementPercent(monster.Drown)}}},");
                dest.WriteLine($"	{{type = COMBAT_ICEDAMAGE, percent = {GenericToTfsElemementPercent(monster.Ice)}}},");
                dest.WriteLine($"	{{type = COMBAT_HOLYDAMAGE , percent = {GenericToTfsElemementPercent(monster.Holy)}}},");
                dest.WriteLine($"	{{type = COMBAT_DEATHDAMAGE , percent = {GenericToTfsElemementPercent(monster.Death)}}}");
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

        public override bool ReadMonster(string filename, out ICustomMonster monster)
        {
            throw new NotImplementedException();
        }

        double GenericToTfsElemementPercent(double percent)
        {
            return (1 - percent) * 100;
        }
    }
}
