﻿using System;
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
        public override bool WriteMonster(string directory, ref ICustomMonster monster)
        {
            string lowerName = monster.Name.ToLower();

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            string titleName = textInfo.ToTitleCase(lowerName);
            string[] lines =
            {
                $"local mType = MonsterType({titleName})",
                "local monster = {}",
                "",
                //"monster.eventFile = true -- will try to load the file example.lua in data/scripts/monsters/events",
                //"monster.eventFile = "test" -- will try to load the file test.lua in data/scripts/monsters/events",
                $"monster.description = \"{monster.Description}\"",
                $"monster.experience = {monster.Experience}",
                "monster.outfit = {",
                "	lookType = 37",
                "}",
                "",
                $"monster.health = {monster.Health}",
                $"monster.maxHealth = {monster.Health}",
                $"monster.race = \"{monster.Race}\"", // TODO check if mapping is neeeded
                $"monster.corpse = {monster.CorpseId}",
                $"monster.speed = {monster.Speed}",
                $"monster.maxSummons = {monster.Speed}",
                "",
                //"monster.changeTarget = {",
                //"	interval = 4*1000,",
                //"	chance = 20",
                //"}",
                "",
                "monster.flags = {",
                //"	summonable = {monster.SummonCost > 0 ? "true" : "false"},",
                $"	attackable = true,",
                $"	hostile = true,",
                $"	convinceable = false,",
                $"	illusionable = {monster.Illusionable},",
                $"	canPushItems = {monster.PushItems},",
                $"	canPushCreatures = {monster.PushCreatures},",
                $"	targetDistance = {monster.TargetDistance},",
                $"	staticAttackChance = 70",
                "}",
                "",
                "monster.summons = {",
                //"	{name = "demon", chance = 10, interval = 2*1000}",
                "}",
                "",
                "monster.voices = {",
                "	interval = 5000,",
                "	chance = 10,",
                //"	{text = "I'm an example", yell = false},",
                //"	{text = "You shall bow", yell = false},",
                "}",
                "",
                "monster.loot = {",
                //"	{id = "gold coin", chance = 60000, maxCount = 100},",
                //"	{id = "bag", chance = 60000, ",
                //"		child = {",
                //"			{id = "platin coin", chance = 60000, maxCount = 100},",
                //"			{id = "crystal coin", chance = 60000, maxCount = 100}",
                //"		}",
                //"	}",
                "}",
                "",
                "monster.attacks = {",
                //"	{name = "melee", attack = 130, skill = 70, effect = CONST_ME_DRAWBLOOD, interval = 2*1000},",
                //"	{name = "energy strike", range = 1, chance = 10, interval = 2*1000, minDamage = -210, maxDamage = -300, target = ","true},",
                //"	{name = "combat", type = COMBAT_MANADRAIN, chance = 10, interval = 2*1000, minDamage = 0, maxDamage = -120, target ","= true, range = 7, effect = CONST_ME_MAGIC_BLUE},",
                //"	{name = "combat", type = COMBAT_FIREDAMAGE, chance = 20, interval = 2*1000, minDamage = -150, maxDamage = -250, ","radius = 1, target = true, effect = CONST_ME_FIREAREA, shootEffect = CONST_ANI_FIRE},",
                //"	{name = "speed", chance = 15, interval = 2*1000, speed = -700, radius = 1, target = true, duration = 30*1000, ","effect = CONST_ME_MAGIC_RED},",
                //"	{name = "firefield", chance = 10, interval = 2*1000, range = 7, radius = 1, target = true, shootEffect = ","CONST_ANI_FIRE},",
                //"	{name = "combat", type = COMBAT_LIFEDRAIN, chance = 10, interval = 2*1000, length = 8, spread = 0, minDamage = -300,"," maxDamage = -490, effect = CONST_ME_PURPLEENERGY}",
                "}",
                "",
                "monster.defenses = {",
                //"	defense = 55,",
                $"	armor = {monster.TotalArmor},",
                //"	{name = "combat", type = COMBAT_HEALING, chance = 15, interval = 2*1000, minDamage = 180, maxDamage = 250, effect = ","CONST_ME_MAGIC_BLUE},",
                //"	{name = "speed", chance = 15, interval = 2*1000, speed = 320, effect = CONST_ME_MAGIC_RED}",
                "}",
                "",
                "monster.elements = {",
                //"	{type = COMBAT_PHYSICALDAMAGE, percent = 30},",
                //"	{type = COMBAT_DEATHDAMAGE, percent = 30},",
                //"	{type = COMBAT_ENERGYDAMAGE, percent = 50},",
                //"	{type = COMBAT_EARTHDAMAGE, percent = 40},",
                //"	{type = COMBAT_ICEDAMAGE, percent = -10},",
                //"	{type = COMBAT_HOLYDAMAGE, percent = -10}",
                "}",
                "",
                "monster.immunities = {",
                //"	{type = "fire", combat = true, condition = true},",
                //"	{type = "drown", condition = true},",
                //"	{type = "lifedrain", combat = true},",
                //"	{type = "paralyze", condition = true},",
                //"	{type = "invisible", condition = true}",
                "}",
                "",
                "mType:register(monster)"
            };
            string fileName = Path.Combine(directory, titleName);
            File.WriteAllLines(fileName, lines);

            return true;
        }

        public override bool ReadMonster(string filename, out ICustomMonster monster)
        {
            throw new NotImplementedException();
        }
    }
}
