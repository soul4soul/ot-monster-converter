using MonsterConverterInterface;
using MonsterConverterInterface.MonsterTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MonsterConverterTibiaWiki
{
    [Export(typeof(IMonsterConverter))]
    public class TibiaWikiConverter : MonsterConverter
    {
        // https://tibia.fandom.com/wiki/Missiles
        private static IDictionary<string, Animation> WikiMissilesToAnimations = new Dictionary<string, Animation>
        {
            { "death missile (large)", Animation.SuddenDeath },
            { "death missile", Animation.Death },
            { "earth missile effect", Animation.Earth },
            { "energy ball missile", Animation.EnergyBall },
            { "energy missile", Animation.Energy },
            { "ethereal spear missile", Animation.EtherealSpear },
            { "fire missile", Animation.Fire },
            { "holy missile (appearance)", Animation.Holy },
            { "holy missile (small)", Animation.SmallHoly },
            { "ice missile", Animation.Ice },
            { "ice shard missile", Animation.SmallIce },
            { "poison missile", Animation.Poison },
            { "rocks missile", Animation.SmallEarth },
            { "small rock missile", Animation.Explosion },
            { "throwing axe missile", Animation.WhirlwindAxe },
            { "throwing club missile", Animation.WhirlwindClub },
            { "throwing sword missile", Animation.WhirlwindSword },
            { "arrow missile", Animation.Arrow },
            { "bolt missile", Animation.Bolt },
            { "burst arrow missile", Animation.BurstArrow },
            { "crystalline arrow effect", Animation.CrystallineArrow },
            { "diamond arrow missile", Animation.None }, // 11.4
            { "drill bolt effect", Animation.DrillBolt },
            { "earth arrow missile", Animation.EarthArrow },
            { "envenomed arrow missile", Animation.EnvenomedArrow },
            { "flaming arrow missile", Animation.FlammingArrow },
            { "flash arrow missile", Animation.FlashArrow },
            { "infernal bolt missile", Animation.InfernalBolt },
            { "onyx arrow missile", Animation.OnyxArrow },
            { "piercing bolt missile", Animation.PiercingBolt },
            { "poison arrow missile", Animation.PoisonArrow },
            { "power bolt missile", Animation.PowerBolt },
            { "prismatic bolt missile", Animation.PrismaticBolt },
            { "shiver arrow missile", Animation.ShiverArrow },
            { "simple arrow missile", Animation.SimpleArrow },
            { "sniper arrow missile", Animation.SniperArrow },
            { "spectral bolt missile", Animation.None }, // 11.4
            { "tarsal arrow missile", Animation.TarsalArrow },
            { "vortex bolt missile", Animation.VortexBolt },
            { "assassin star missile", Animation.RedStar },
            { "enchanted spear missile", Animation.EnchantedSpear },
            { "glooth spear missile", Animation.GloothSpear },
            { "hunting spear missile", Animation.HuntingSpear },
            { "leaf star missile", Animation.None }, // 11.4
            { "royal spear missile", Animation.RoyalSpear },
            { "royal star missile", Animation.None }, // 11.4
            { "small stone missile", Animation.SmallStone },
            { "snowball missile", Animation.Snowball },
            { "spear missile", Animation.Spear },
            { "throwing cake missile", Animation.Cake },
            { "throwing knife missile", Animation.ThrowingKnife },
            { "throwing star missile", Animation.ThrowingStar },
            { "viper star missile", Animation.GreenStar },
            { "stone missile", Animation.LargeRock }
        };

        // https://tibia.fandom.com/wiki/Effects
        private static IDictionary<string, Effect> WikiToEffects = new Dictionary<string, Effect>
        {
            { "black smoke effect", Effect.BlackSmoke },
            { "block effect", Effect.None }, // 12.31
            { "flitter effect", Effect.None }, // should be in by 9.40
            { "floating block effect", Effect.None }, // 12.31
            { "green smoke effect", Effect.GreenSmoke },
            { "purple smoke effect", Effect.PurpleSmoke },
            { "red smoke effect", Effect.RedSmoke },
            { "steam effect", Effect.Smoke },
            { "sun priest effect", Effect.None }, // 12.4
            { "the cube effect", Effect.None }, // 12.2
            { "werelion effect", Effect.None }, // 2.4
            { "yellow smoke effect", Effect.YellowSmoke },
            { "assassin effect", Effect.Assassin },
            { "big scratching effect", Effect.None }, // 12.4
            { "bite effect", Effect.None }, // 12.4
            { "blue ghost effect", Effect.None }, // 11.5
            { "demon mirror effect", Effect.MirrorHorizontal }, // Could be vertical too
            { "exploding kraknaknork effect", Effect.OrcShamanFire },//
            { "fae effect 1", Effect.None }, // 11.4
            { "fae effect 2", Effect.None }, // 11.4
            { "ferumbras effect", Effect.Ferumbras },
            { "ghost smoke effect", Effect.None }, // 12.3
            { "ghostly bite effect", Effect.None }, // 12.4
            { "ghostly scratch effect", Effect.None }, // 12.4
            { "green ghost effect", Effect.YalahariGhost },
            { "kraknaknork effect", Effect.OrcShaman },
            { "scratching effect", Effect.StepsHorizontal },
            { "sea serpent effect", Effect.WaterCreature },
            { "slash effect", Effect.None }, //12.4
            { "thaian effect 1", Effect.None }, //12.3
            { "thaian effect 2", Effect.None }, //12.3
            { "vanishing fae effect", Effect.None }, //11.4
            { "black blood effect", Effect.None }, // 12.20
            { "blood effect", Effect.DrawBlood },
            { "blue chain effect", Effect.None }, // 12.00?
            { "blue electricity effect", Effect.EnergyHit },
            { "carnivorous plant effect", Effect.Carniphila },
            { "challenge effect", Effect.None }, // 12.55
            { "cloud effect", Effect.SmallClouds },
            { "critical hit effect", Effect.CriticalDamage },
            { "death effect", Effect.MortArea },
            { "divine dazzle effect", Effect.None }, // 12.60
            { "dust effect", Effect.GroundShaker },
            { "electrical spark effect", Effect.None }, // ?
            { "energy effect", Effect.EnergyArea },
            { "explosion effect", Effect.ExplosionArea },
            { "fire effect", Effect.HitByFire },
            { "fireball effect", Effect.FireArea },
            { "flame effect", Effect.FireAttack },
            { "green chain effect", Effect.None }, // 12.00?
            { "grey chain effect", Effect.None }, // 12.00?
            { "holy cross effect", Effect.HolyArea },
            { "holy effect", Effect.HolyDamage },
            { "ice crystal effect", Effect.GiantIce },
            { "ice explosion effect", Effect.IceAttack },
            { "ice tornado effect", Effect.IceTornado },
            { "icicle effect", Effect.IceArea },
            { "large plant effect", Effect.BigPlants },
            { "orange chain effect", Effect.None }, // 12.00?
            { "plant effect", Effect.SmallPlants },
            { "poison effect 1", Effect.GreenRings },
            { "poison effect 2", Effect.HitByPoison },
            { "poison effect 3", Effect.PoisonArea },
            { "purple chain effect", Effect.None }, // 12.00?
            { "purple electricity effect", Effect.PurpleEnergy },
            { "ripple effect", Effect.LoseEnergy },
            { "rooting effect", Effect.None }, // 12.40
            { "slicing plant effect", Effect.PlantAttack },
            { "stars effect", Effect.Stun },
            { "stone shower effect", Effect.Stones },
            { "thunderstorm effect", Effect.BigClouds },
            { "yellow chain effect", Effect.None }, // 12.00?
            { "yellow electricity effect", Effect.YellowEnergy },
            { "yellow poison effect", Effect.YellowRings },
            { "map effect", Effect.None }, // 11.80
            { "point of interest effect", Effect.None }, // 11.80
            { "point of interest found effect", Effect.None }, // 11.80
            { "tile highlight effect", Effect.TutorialSquare },
            { "tutorial arrow effect", Effect.TutorialArrow },
            { "bat swarm effect", Effect.Bats },
            { "blast effect", Effect.ExplosionHit },
            { "blue confetti effect", Effect.FireworkBlue },
            { "blue notes effect", Effect.SoundBlue },
            { "blue sparkles effect", Effect.MagicBlue },
            { "bubbles effect", Effect.Bubbles },
            { "confetti effect", Effect.GiftWraps },
            { "cream cake effect", Effect.Cake },//
            { "die effect", Effect.Craps },
            { "green confetti effect", Effect.None }, // 12.02
            { "green notes effect", Effect.SoundGreen },
            { "green sparkles effect", Effect.MagicGreen },
            { "grey teleport effect", Effect.None }, // 12.70
            { "hearts effect", Effect.Hearts },
            { "jumping fish effect", Effect.PlungingFish },
            { "light blue teleport effect", Effect.None }, // 12.70
            { "light effect", Effect.Thunder },
            { "moonlight effect", Effect.EarlyThunder },
            { "orange confetti effect", Effect.None }, // 12.02
            { "orange teleport effect", Effect.None }, // 12.70
            { "poof effect", Effect.Poff },//
            { "prismatic sparkles effect", Effect.None }, // 12.15
            { "purple confetti effect", Effect.None }, // 12.02
            { "purple notes effect", Effect.SoundPurple },
            { "purple teleport effect", Effect.None }, // 12.70
            { "red confetti effect", Effect.FireworkRed },
            { "red notes effect", Effect.SoundRed },//
            { "red sparkles effect", Effect.MagicRed },
            { "red teleport effect", Effect.None }, // 12.70
            { "snoring effect", Effect.Sleep },
            { "sparks effect", Effect.BlockHit },
            { "spooky face effect", Effect.SkullHorizontal }, // Could be vertical too
            { "teleport effect", Effect.Teleport },
            { "trembling effect", Effect.HitArea },
            { "turquoise confetti effect", Effect.None }, // 12.02
            { "water splash effect", Effect.WaterSplash },
            { "white notes effect", Effect.SoundWhite },//
            { "yellow confetti effect", Effect.FireworkYellow },
            { "yellow notes effect", Effect.SoundYellow },
            { "yellow sparkles effect", Effect.None } // 12.60?
        };

        public override string ConverterName { get => "TibiaWiki"; }

        public override FileSource FileSource { get => FileSource.Web; }

        public override string FileExt { get => "html"; }

        public override bool IsReadSupported { get => true; }

        public override bool IsWriteSupported { get => true; }

        private class RegexPatternKeys
        {
            public RegexPatternKeys(string name, string pattern, Action<Monster, Match> action)
            {
                Name = name;
                Pattern = @$"\|\s+{name}\s+=\s+{pattern}\s*";
                Action = action;
            }

            public string Name { get; }
            public string Pattern { get; }
            public Action<Monster, Match> Action { get; }

            public override string ToString()
            {
                return $"{Name} {Pattern}";
            }
        }

        private RegexPatternKeys[] monparams = new RegexPatternKeys[] {
            new RegexPatternKeys("name", "(?<name>[A-Za-z'ñ.() -]*)", (mon, m) => mon.RegisteredName = mon.FileName = m.Groups["name"].Value),
            new RegexPatternKeys("actualname", "(?<actualname>[A-Za-z'ñ. -]*)", (mon, m) => mon.Name = m.Groups["actualname"].Value),
            new RegexPatternKeys("article", "(?<article>[A-Za-z ]*)", ParseArticle),
            new RegexPatternKeys("hp", @"(?<hp>\d+)", (mon, m) => { if (m.Groups["hp"].Success) mon.Health = uint.Parse(m.Groups["hp"].Value); }),
            new RegexPatternKeys("exp", @"(?<exp>\d+)", (mon, m) => { if (m.Groups["exp"].Success) mon.Experience = uint.Parse(m.Groups["exp"].Value); }),
            new RegexPatternKeys("armor", @"(?<armor>\d+)", (mon, m) => { if (m.Groups["armor"].Success) mon.TotalArmor = mon.Shielding = uint.Parse(m.Groups["armor"].Value); }),
            new RegexPatternKeys("speed", @"(?<speed>\d+)", (mon, m) => { if (m.Groups["speed"].Success) mon.Speed = uint.Parse(m.Groups["speed"].Value) * 2; }),
            new RegexPatternKeys("runsat", @"(?<runsat>\d+)", (mon, m) => { if (m.Groups["runsat"].Success) mon.RunOnHealth = uint.Parse(m.Groups["runsat"].Value); }),
            new RegexPatternKeys("summon", @"(?<summon>\d+)", (mon, m) => { if (m.Groups["summon"].Success) mon.SummonCost = uint.Parse(m.Groups["summon"].Value); }),
            new RegexPatternKeys("convince", @"(?<convince>\d+)", (mon, m) => { if (m.Groups["convince"].Success) mon.ConvinceCost = uint.Parse(m.Groups["convince"].Value); }),
            new RegexPatternKeys("illusionable", @"(?<illusionable>((Y|y)(E|e)(S|s)))", (mon, m) => mon.Illusionable = m.Groups["illusionable"].Success),
            new RegexPatternKeys("isboss", @"(?<isboss>((Y|y)(E|e)(S|s)))", (mon, m) => mon.IsBoss = m.Groups["isboss"].Success),
            new RegexPatternKeys("priamrtype", @"(?<hidehealth>trap)", (mon, m) => mon.HideHealth = m.Groups["hidehealth"].Success),
            new RegexPatternKeys("pushable", @"(?<pushable>((Y|y)(E|e)(S|s)))", (mon, m) => mon.Pushable = m.Groups["pushable"].Success),
            // In cipbia ability to push objects means ability to push creatures too
            new RegexPatternKeys("pushobjects", @"(?<pushobjects>((Y|y)(E|e)(S|s)))", (mon, m) => mon.PushItems = mon.PushCreatures = m.Groups["pushobjects"].Success),
            new RegexPatternKeys("senseinvis", @"(?<senseinvis>((Y|y)(E|e)(S|s)))", (mon, m) => mon.IgnoreInvisible = m.Groups["senseinvis"].Success),
            new RegexPatternKeys("paraimmune", @"(?<paraimmune>((Y|y)(E|e)(S|s)))", (mon, m) => mon.IgnoreParalyze = m.Groups["paraimmune"].Success),
            new RegexPatternKeys("walksaround", @"(?<walksaround>\w+(, \w+)*)", ParseWalksAround),
            new RegexPatternKeys("walksthrough", @"(?<walksthrough>\w+(, \w+)*)", ParseWalksThrough),
            new RegexPatternKeys("physicalDmgMod", @"(?<physicaldmgmod>\d+)%", (mon, m) => { if (m.Groups["physicaldmgmod"].Success) mon.Physical = double.Parse(m.Groups["physicaldmgmod"].Value) / 100.0; }),
            new RegexPatternKeys("earthDmgMod", @"(?<earthdmgmod>\d+)%", (mon, m) => { if (m.Groups["earthdmgmod"].Success) mon.Earth = double.Parse(m.Groups["earthdmgmod"].Value) / 100.0; }),
            new RegexPatternKeys("fireDmgMod", @"(?<firedmgmod>\d*)%", (mon, m) => { if (m.Groups["firedmgmod"].Success) mon.Fire = double.Parse(m.Groups["firedmgmod"].Value) / 100.0; }),
            new RegexPatternKeys("deathDmgMod", @"(?<deathdmgmod>\d*)%", (mon, m) => { if (m.Groups["deathdmgmod"].Success) mon.Death = double.Parse(m.Groups["deathdmgmod"].Value) / 100.0; }),
            new RegexPatternKeys("energyDmgMod", @"(?<energydmgmod>\d+)%", (mon, m) => { if (m.Groups["energydmgmod"].Success) mon.Energy = double.Parse(m.Groups["energydmgmod"].Value) / 100.0; }),
            new RegexPatternKeys("holyDmgMod", @"(?<holydmgmod>\d+)%", (mon, m) => { if (m.Groups["holydmgmod"].Success) mon.Holy = double.Parse(m.Groups["holydmgmod"].Value) / 100.0; }),
            new RegexPatternKeys("iceDmgMod", @"(?<icedmgmod>\d+)%", (mon, m) => { if (m.Groups["icedmgmod"].Success) mon.Ice = double.Parse(m.Groups["icedmgmod"].Value) / 100.0; }),
            new RegexPatternKeys("hpdrainDmgMod", @"(?<hpdraindmgmod>\d+)%", (mon, m) => { if (m.Groups["hpdraindmgmod"].Success) mon.LifeDrain = double.Parse(m.Groups["hpdraindmgmod"].Value) / 100.0; }),
            new RegexPatternKeys("drownDmgMod", @"(?<drowndmgmod>\d+)%", (mon, m) => { if (m.Groups["drowndmgmod"].Success) mon.Drown = double.Parse(m.Groups["drowndmgmod"].Value) / 100.0; }),
            new RegexPatternKeys("sounds", @"{{Sound List\|(?<sounds>[a-zA-Z !?.'*]+(\|[a-zA-Z !?.'*]+)*)", ParseSoundList),
            // TibiaWiki generally doesn't provide distance so we default to 4. In TFS monster pack 70 of 77 monsters which use distance attack stand at a range of 4.
            new RegexPatternKeys("behavior", @"(?<behavior>((D|d)(I|i)(S|s)(A|a)(N|n)(C|c)(|s)(E|e)))", (mon, m) => { mon.TargetDistance = m.Groups["behavior"].Success ? (uint)4 : (uint)1; }),
            new RegexPatternKeys("abilities", @"(?<abilities>.*)", ParseAbilities)
        };

        /// <summary>
        /// Boss monsters and single appear monsters don't use article
        /// </summary>
        /// <param name="mon"></param>
        /// <param name="m"></param>
        private static void ParseArticle(Monster mon, Match m)
        {
            if (m.Groups["article"].Success)
            {
                mon.Description = string.Format("{0} {1}", m.Groups["article"].Value, mon.Name).Trim();
            }
            else
            {
                mon.Description = mon.Name;
            }
        }

        private static void ParseWalksAround(Monster mon, Match m)
        {
            foreach (string field in m.Groups["walksaround"].Value.ToLower().Split(","))
            {
                string fieldtrim = field.Trim();
                if (fieldtrim == "fire")
                {
                    mon.AvoidFire = true;
                }
                else if (fieldtrim == "energy")
                {
                    mon.AvoidEnergy = true;
                }
                else if (fieldtrim == "poison")
                {
                    mon.AvoidPoison = true;
                }
            }
        }

        private static void ParseWalksThrough(Monster mon, Match m)
        {
            foreach (string field in m.Groups["walksthrough"].Value.ToLower().Split(","))
            {
                string fieldtrim = field.Trim();
                if (fieldtrim == "fire")
                {
                    mon.AvoidFire = false;
                }
                else if (fieldtrim == "energy")
                {
                    mon.AvoidEnergy = false;
                }
                else if (fieldtrim == "poison")
                {
                    mon.AvoidPoison = false;
                }
            }
        }

        private static void ParseSoundList(Monster mon, Match m)
        {
            foreach (string sound in m.Groups["sounds"].Value.Split("|"))
            {
                if (!string.IsNullOrWhiteSpace(sound))
                {
                    mon.Voices.Add(new Voice() { Sound = sound, SoundLevel = SoundLevel.Say });
                }
            }
        }

        /// <summary>
        /// Parsing abilties is implemented like this instead of a using the RegexPatternKeys so we can easily print a list of abilties which fail to be parsed
        /// Abilities from tibiawiki include melee, attacks, and defenses
        /// </summary>
        /// <param name="mon"></param>
        /// <param name="m"></param>
        private static void ParseAbilities(Monster mon, Match m)
        {
            string abilities = m.Groups["abilities"].Value.ToLower().Replace("\r", "").Replace("\n", "");

            if (abilities.Contains("ability list"))
                ParseAbilityList(mon, abilities);
            else
                ParseLegacyAbilities(mon, abilities);
        }

        private static void ParseLegacyAbilities(Monster mon, string abilities)
        {
            if (string.IsNullOrWhiteSpace(abilities) || abilities.Contains("none") || abilities.Contains("unknown") || abilities == "?")
                return;

            // Generally we find each ability is seperated by a comma expect those inside ()'s
            var splitAbilities = abilities.SplitTopLevel(',');

            // Due our best to parse the none standard ability information
            foreach (string ability in splitAbilities)
            {
                string cleanedAbility = ability.Trim().TrimEnd('.');
                switch (cleanedAbility)
                {
                    case var _ when new Regex(@"\[\[melee\]\](\s*\((?<damage>[0-9- ]+))?").IsMatch(cleanedAbility):
                        {
                            var match = new Regex(@"\[\[melee\]\](\s*\((?<damage>[0-9- ]+))?").Match(cleanedAbility);
                            var spell = new Spell() { Name = "melee", SpellCategory = SpellCategory.Offensive, Interval = 2000, Chance = 1 };
                            if (ParseNumericRange(match.Groups["damage"].Value, out int min, out int max))
                            {
                                spell.MinDamage = -min;
                                spell.MaxDamage = -max;
                            }
                            else
                            {
                                // Could guess defaults based on creature HP, EXP, and bestiary difficulty
                            }
                            mon.Attacks.Add(spell);
                            break;
                        }

                    // Effect might need to be optional
                    case var _ when new Regex(@"\[\[distance fighting\|(?<effect>[a-z ]+)\]\]s?\s*\((?<damage>[0-9- ]+)(\+?~)?\)").IsMatch(cleanedAbility):
                        {
                            var match = new Regex(@"\[\[distance fighting\|(?<effect>[a-z ]+)\]\]s?\s*\((?<damage>[0-9- ]+)(\+?~)?\)").Match(cleanedAbility);
                            var spell = new Spell() { Name = "combat", SpellCategory = SpellCategory.Offensive, DamageElement = CombatDamage.Physical, Interval = 2000, Chance = 1, Range = 7, ShootEffect = TibiaWikiToAnimation(match.Groups["effect"].Value) };
                            if (ParseNumericRange(match.Groups["damage"].Value, out int min, out int max))
                            {
                                spell.MinDamage = -min;
                                spell.MaxDamage = -max;
                            }
                            else
                            {
                                // Could guess defaults based on creature HP, EXP, and bestiary difficulty
                            }
                            mon.Attacks.Add(spell);
                            break;
                        }

                    case var _ when new Regex(@"\[\[haste\]\]").IsMatch(cleanedAbility):
                        {
                            var spell = new Spell() { Name = "speed", SpellCategory = SpellCategory.Defensive, Interval = 2000, Chance = 0.15, MinSpeedChange = 300, MaxSpeedChange = 300, AreaEffect = Effect.MagicRed, Duration = 7000 };
                            mon.Attacks.Add(spell);
                            break;
                        }

                    case var _ when new Regex(@"\[\[strong haste\]\]").IsMatch(cleanedAbility):
                        {
                            var spell = new Spell() { Name = "speed", SpellCategory = SpellCategory.Defensive, Interval = 2000, Chance = 0.15, MinSpeedChange = 450, MaxSpeedChange = 450, AreaEffect = Effect.MagicRed, Duration = 4000 };
                            mon.Attacks.Add(spell);
                            break;
                        }

                    case var _ when new Regex(@"\[\[(self-? ?healing)\]\](\s*\((?<damage>[0-9- ]+))?").IsMatch(cleanedAbility):
                        {
                            var match = new Regex(@"\[\[(self-? ?healing)\]\](\s*\((?<damage>[0-9- ]+))?").Match(cleanedAbility);
                            var spell = new Spell() { Name = "combat", SpellCategory = SpellCategory.Defensive, DamageElement = CombatDamage.Healing, Interval = 2000, Chance = 0.2 };
                            if (ParseNumericRange(match.Groups["damage"].Value, out int min, out int max))
                            {
                                spell.MinDamage = min;
                                spell.MaxDamage = max;
                            }
                            else
                            {
                                // Guess defaults based on creature HP
                                spell.MinDamage = (int?)(mon.Health * 0.1);
                                spell.MaxDamage = (int?)(mon.Health * 0.25);
                            }
                            mon.Attacks.Add(spell);
                            break;
                        }

                    default:
                        System.Diagnostics.Debug.WriteLine($"{mon.FileName} legacy ability not parsed \"{cleanedAbility}\"");
                        break;
                }
            }
        }

        /// <summary>
        /// To parse abilities from the ability list template we the string needs to be split by top level |
        /// </summary>
        /// <param name="mon"></param>
        /// <param name="abilities"></param>
        private static void ParseAbilityList(Monster mon, string abilities)
        {
            abilities = @"{{Ability List|{{Melee|0-500}}|{{Ability|Great Fireball|150-250|fire|scene={{Scene|spell=5sqmballtarget|effect=Fireball Effect|caster=Demon|look_direction=|effect_on_target=Fireball Effect|missile=Fire Missile|missile_direction=south-east|missile_distance=5/5|edge_size=32}}
}}|{{Ability|[[Great Energy Beam]]|300-480|lifedrain|scene={{Scene|spell=8sqmbeam|effect=Blue Electricity Effect|caster=Demon|look_direction=east}}}}|{{Ability|Close-range Energy Strike|210-300|energy}}|{{Ability|Mana Drain|30-120|manadrain}}|{{Healing|range=80-250}}|{{Ability|Shoots [[Fire Field]]s||fire}}|{{Ability|Distance Paralyze||paralyze}}|{{Summon|Fire Elemental|1}}}}".ToLower().Replace("\r", "").Replace("\n", "");

            var abilityList = TemplateParser.Deseralize<AbilityListTemplate>(abilities);

            foreach (string ability in abilityList.Ability)
            {
                if (Regex.IsMatch(ability, @"{{melee\|.*}}"))
                {
                    var melee = TemplateParser.Deseralize<MeleeTemplate>(ability);
                    var spell = new Spell() { Name = "melee", SpellCategory = SpellCategory.Offensive, Interval = 2000, Chance = 1 };
                    if (ParseNumericRange(melee.Damage, out int min, out int max))
                    {
                        spell.MinDamage = -min;
                        spell.MaxDamage = -max;
                    }
                    else
                    {
                        // Could guess defaults based on creature HP, EXP, and bestiary difficulty
                    }
                    mon.Attacks.Add(spell);
                }
                else if (Regex.IsMatch(ability, @"{{healing\|.*}}"))
                {
                    var healing = TemplateParser.Deseralize<HealingTemplate>(ability);
                    var spell = new Spell() { Name = "combat", SpellCategory = SpellCategory.Defensive, DamageElement = CombatDamage.Healing, Interval = 2000, Chance = 0.2 };
                    if (ParseNumericRange(healing.Damage, out int min, out int max))
                    {
                        spell.MinDamage = min;
                        spell.MaxDamage = max;
                    }
                    else
                    {
                        // Guess defaults based on creature HP
                        spell.MinDamage = (int?)(mon.Health * 0.1);
                        spell.MaxDamage = (int?)(mon.Health * 0.25);
                    }
                    mon.Attacks.Add(spell);
                }
                else if (Regex.IsMatch(ability, @"{{summon\|.*}}"))
                {
                    var summon = TemplateParser.Deseralize<SummonTemplate>(ability);
                    int maxSummons = 1;
                    ParseNumericRange(summon.Amount, out int min, out maxSummons);
                    mon.MaxSummons += (uint)maxSummons;
                    string firstSummonName = summon.Creature;
                    mon.Summons.Add(new Summon() { Name = firstSummonName });

                    foreach (var name in summon.Creatures)
                    {
                        mon.Summons.Add(new Summon() { Name = name });
                    }
                }
                else if (Regex.IsMatch(ability, @"{{haste\|.*}}"))
                {
                    var haste = TemplateParser.Deseralize<HasteTemplate>(ability);
                    int MinSpeedChange = 300;
                    int MaxSpeedChange = 300;
                    int Duration = 7000;
                    if (haste.Name .Contains("strong"))
                    {
                        MinSpeedChange = 450;
                        MaxSpeedChange = 450;
                        Duration = 4000;
                    }
                    var spell = new Spell() { Name = "speed", SpellCategory = SpellCategory.Defensive, Interval = 2000, Chance = 0.15, MinSpeedChange = MinSpeedChange, MaxSpeedChange = MaxSpeedChange, AreaEffect = Effect.MagicRed, Duration = Duration };
                    mon.Attacks.Add(spell);
                }
                else if (Regex.IsMatch(ability, @"{{ability\|.*}}"))
                {
                    var abilityObj = TemplateParser.Deseralize<AbilityTemplate>(ability);
                    if (!string.IsNullOrWhiteSpace(abilityObj.Scene))
                    {
                        SceneTemplate scene = TemplateParser.Deseralize<SceneTemplate>(abilityObj.Scene);
                        Spell spell = WikiSceneToSpell(scene);
                        if (ParseNumericRange(abilityObj.Damage, out int min, out int max))
                        {
                            spell.MinDamage = -min;
                            spell.MaxDamage = -max;
                        }
                        else
                        {
                            // Could guess defaults based on creature HP, EXP, and bestiary difficulty
                        }
                        spell.Name = "combat";
                        spell.SpellCategory = SpellCategory.Offensive;
                        mon.Attacks.Add(spell);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"{mon.FileName} ability not parsed \"{ability}\"");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"{mon.FileName} ability not parsed \"{ability}\"");
                }
            }
        }

        // TODO create a mapping from scenebuilder data to our spell shape https://tibia.fandom.com/wiki/Module:SceneBuilder/data
        private static Spell WikiSceneToSpell(SceneTemplate scene)
        {
            Spell spell = new Spell() { AreaEffect = Effect.None, ShootEffect = Animation.None, Interval = 2000, Chance = 0.15 };
            if (WikiMissilesToAnimations.ContainsKey(scene.Missile))
            {
                spell.ShootEffect = WikiMissilesToAnimations[scene.Missile];
                spell.OnTarget = true;
            }
            if (WikiToEffects.ContainsKey(scene.Effect))
                spell.AreaEffect = WikiToEffects[scene.Effect];
            switch (scene.Spell)
            {
                case "front_sweep":
                    spell.IsDirectional = true;
                    spell.Length = 1;
                    spell.Spread = 3;
                    break;
                case "1sqmstrike":
                    spell.OnTarget = true;
                    spell.Range = 1;
                    break;
                case "2sqmstrike":
                    spell.OnTarget = true;
                    spell.Range = 2;
                    break;
                case "3sqmstrike":
                    spell.OnTarget = true;
                    spell.Range = 3;
                    break;
                case "5sqmstrike":
                    spell.OnTarget = true;
                    spell.Range = 5;
                    break;
                case "great_explosion":
                    spell.Radius = 4;
                    break;
                case "3x3spell":
                    spell.Radius = 3;
                    break;
                case "xspell":
                    break;
                case "plusspell":
                    break;
                case "plusspelltarget":
                    break;
                case "3sqmwave":
                    spell.IsDirectional = true;
                    spell.Length = 3;
                    break;
                case "5sqmwavenarrow":
                    spell.IsDirectional = true;
                    spell.Length = 5;
                    spell.Spread = 3;
                    break;
                case "5sqmwavewide":
                    spell.IsDirectional = true;
                    spell.Length = 5;
                    spell.Spread = 5;
                    break;
                case "1sqmballtarget":
                    spell.OnTarget = true;
                    break;
                case "2sqmballtarget":
                    spell.OnTarget = true;
                    break;
                case "3sqmballtarget":
                    spell.OnTarget = true;
                    break;
                case "4sqmballtarget":
                    spell.OnTarget = true;
                    break;
                case "5sqmballtarget":
                    spell.OnTarget = true;
                    break;
                case "8sqmwave":
                    spell.Length = 8;
                    break;
                case "10sqmwave":
                    spell.Length = 10;
                    break;
                case "2sqmring":
                    break;
                case "3sqmring":
                    break;
                case "4sqmring":
                    break;
                case "4sqmballself":
                    spell.OnTarget = false;
                    break;
                case "5sqmballself":
                    spell.OnTarget = false;
                    break;
                case "6sqmballself":
                    spell.OnTarget = false;
                    break;
                case "4sqmbeam":
                    spell.IsDirectional = true;
                    spell.Length = 4;
                    spell.Spread = 1;
                    break;
                case "5sqmbeam":
                    spell.IsDirectional = true;
                    spell.Length = 5;
                    spell.Spread = 1;
                    break;
                case "6sqmbeam":
                    spell.IsDirectional = true;
                    spell.Length = 6;
                    spell.Spread = 1;
                    break;
                case "7sqmbeam":
                    spell.IsDirectional = true;
                    spell.Length = 7;
                    spell.Spread = 1;
                    break;
                case "8sqmbeam":
                    spell.IsDirectional = true;
                    spell.Length = 8;
                    spell.Spread = 1;
                    break;
                case "energy_wall_north_diag_area":
                    break;
                case "energy_wall_south_diag_area":
                    break;
                case "energy_wall_north_south_area":
                    break;
                case "chivalrous_challenge":
                    break;
            }

            return spell;
        }

        private static Animation TibiaWikiToAnimation(string missile)
        {
            if ((missile == "spear") || (missile == "spears"))
            {
                return Animation.Spear;
            }
            else if ((missile == "throwing knives") || (missile == "throwing knife"))
            {
                return Animation.ThrowingKnife;
            }
            else if ((missile == "bolt") || (missile == "bolts"))
            {
                return Animation.Bolt;
            }
            else if ((missile == "arrow") || (missile == "arrows"))
            {
                return Animation.Arrow;
            }
            else if (missile.Contains("boulder"))
            {
                return Animation.LargeRock;
            }
            else if (missile.Contains("stone"))
            {
                return Animation.SmallStone;
            }
            else
            {
                return Animation.None;
            }
        }

        /// <summary>
        /// Converts a string representing a numeric range to two intergers
        /// Example numeric ranges which can be parsed are "500", "0-500", and "0-500?"
        /// </summary>
        /// <param name="range">String to parse</param>
        /// <param name="min">lower bound value in range, defaults to 0</param>
        /// <param name="max">high bonund value in the range, will be set when the range only has a single number</param>
        /// <returns>returns false when no numeric values can be parsed</returns>
        private static bool ParseNumericRange(string range, out int min, out int max)
        {
            Regex rgx = new Regex(@"(?<first>\d+)(([ -]?)(?<second>\d+))?");
            var match = rgx.Match(range);

            min = 0;
            if (int.TryParse(match.Groups["second"].Value, out max))
            {
                int.TryParse(match.Groups["first"].Value, out min);
                return true;
            }
            else if (int.TryParse(match.Groups["first"].Value, out max))
            {
                return true;
            }
            return false;
        }

        public override string[] GetFilesForConversion(string directory)
        {
            // directory parameter is ignored for this format...
            string monsterlisturl = $"https://tibia.fandom.com/api.php?action=parse&format=json&page=List_of_Creatures_(Ordered)&prop=text";
            IList<string> names = new List<string>();
            var monsterTable = RequestData(monsterlisturl).Result.Text.Empty;

            // Links are HTML encoded
            // %27 is HTML encode for ' character
            // %27%C3% is HTML encode for ñ character
            var namematches = new Regex("/wiki/(?<name>[[a-zA-Z.()_%27%C3%B1-]+)").Matches(monsterTable);
            foreach (Match match in namematches)
            {
                names.Add(match.Groups["name"].Value.Replace("%27", "'").Replace("%C3%B1", "ñ"));
            }

            return names.ToArray();
        }

        private static readonly HttpClient client = new HttpClient();

        private async Task<Parse> RequestData(string endpoint)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            //client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            try
            {
                var streamTask = client.GetStreamAsync(endpoint);
                var repositories = await JsonSerializer.DeserializeAsync<Root>(await streamTask);
                return repositories.Parse;
            }
            catch
            {
                return null;
            }
        }

        public override ConvertResult ReadMonster(string filename, out Monster monster)
        {
            string resultMessage = "Blood type, look type data, and abilities are not parsed.";

            string monsterurl = $" https://tibia.fandom.com/api.php?action=parse&format=json&page={filename}&prop=wikitext";
            string looturl = $"https://tibia.fandom.com/api.php?action=parse&format=json&page=Loot_Statistics:{filename}&prop=wikitext";

            monster = new Monster() { Name = "" };

            var monsterPage = RequestData(monsterurl).Result;
            if (monsterPage != null)
            {
                var wikiText = monsterPage.Wikitext.Empty;
                foreach (var x in monparams)
                {
                    var match = new Regex(x.Pattern).Match(wikiText);
                    try
                    {
                        x.Action.Invoke(monster, match);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"{filename} Pattern \"{x.Pattern}\" failed with \"{ex.Message}\"");
                    }
                }
            }

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            if (string.IsNullOrWhiteSpace(monster.Name) && !string.IsNullOrWhiteSpace(monster.FileName))
            {
                // Better then nothing guess
                resultMessage += "Guessed creature name";
                monster.Name = monster.FileName;
            }
            monster.Name = textInfo.ToTitleCase(monster.Name);

            var lootPage = RequestData(looturl).Result;
            if (lootPage != null)
            {
                string elements = lootPage.Wikitext.Empty.ToLower();
                var lootsectionsRegEx = new Regex("{{loot2(?<loots>.*)}}", RegexOptions.Singleline);
                if (lootsectionsRegEx.IsMatch(elements))
                {
                    var lootsection = lootsectionsRegEx.Match(elements);
                    string loots = lootsection.Captures[0].Value;

                    var killsmatch = new Regex(@"\|kills=(?<kills>\d+)").Match(loots);
                    double.TryParse(killsmatch.Groups["kills"].Value, out double kills);
                    // sometimes TibiaWiki doesn't show the amount field
                    var lootregex = new Regex(@"\|\s*(?<itemname>[a-z'.() ]*),\s*times:\s*(?<times>\d+)(, amount:\s*(?<amount>[0-9-]+))?");
                    var matches = lootregex.Matches(loots);
                    foreach (Match loot in matches)
                    {
                        string item = loot.Groups["itemname"].Value;
                        double.TryParse(loot.Groups["times"].Value, out double times);
                        string amount = loot.Groups["amount"].Value;

                        if (item != "empty")
                        {
                            double percent = times / kills;

                            if (!double.TryParse(amount, out double count))
                            {
                                var amounts = amount.Split("-");
                                if (amounts.Length >= 2)
                                {
                                    double.TryParse(amounts[1], out count);
                                }
                            }
                            count = (count > 0) ? count : 1;

                            monster.Items.Add(new Loot()
                            {
                                Item = item,
                                Chance = (decimal)percent,
                                Count = (decimal)count
                            });
                        }
                    }
                }
            }

            return new ConvertResult(filename, ConvertError.Warning, resultMessage);
        }

        public override ConvertResult WriteMonster(string directory, ref Monster monster)
        {
            string[] lines =
            {
                "{{Infobox Creature|List={{{1|}}}|GetValue={{{GetValue|}}}",
                $"| name           = {monster.RegisteredName}",
                $"| hp             = {monster.Health}",
                $"| actualname     = {monster.Name}",
                $"| exp            = {monster.Experience}",
                $"| armor          = {monster.TotalArmor}",
                string.Format("| summon         = {0}", monster.SummonCost > 0 ? monster.SummonCost.ToString() : "--"),
                string.Format("| convince         = {0}", monster.ConvinceCost > 0 ? monster.ConvinceCost.ToString() : "--"),
                string.Format("| convince         = {0}", monster.Illusionable ? "yes" : "no"),
                string.Format("| isboss         = {0}", monster.IsBoss ? "yes" : "no"),
                string.Format("| pushable         = {0}", monster.Pushable ? "yes" : "no"),
                string.Format("| pushobjects         = {0}", monster.PushItems ? "yes" : "no"),
                $"| walksaround    = {GenericToTibiaWikiWalkAround(ref monster)}",
                $"| walksthrough   = {GenericToTibiaWikiWalkThrough(ref monster)}",
                string.Format("| paraimmune         = {0}", monster.IgnoreParalyze ? "yes" : "no"),
                string.Format("| senseinvis         = {0}", monster.IgnoreInvisible ? "yes" : "no"),
                $"| physicalDmgMod = {monster.Physical * 100:0}%",
                $"| earthDmgMod    = {monster.Earth * 100:0}%",
                $"| fireDmgMod     = {monster.Fire * 100:0}%",
                $"| deathDmgMod    = {monster.Death * 100:0}%",
                $"| energyDmgMod   = {monster.Energy * 100:0}%",
                $"| holyDmgMod     = {monster.Holy * 100:0}%",
                $"| iceDmgMod      = {monster.Ice * 100:0}%",
                $"| hpDrainDmgMod  = {monster.LifeDrain * 100:0}%",
                $"| drownDmgMod    = {monster.Drown * 100:0}%",
                $"| sounds         = {GenericToTibiaWikiVoice(ref monster)}",
                $"| runsat         = {monster.RunOnHealth}",
                $"| speed          = {monster.Speed}"
            };
            string fileName = Path.Combine(directory, monster.FileName);
            File.WriteAllLines(fileName, lines);

            return new ConvertResult(fileName, ConvertError.Warning, "Loot information is not written.");
        }

        private string GenericToTibiaWikiWalkAround(ref Monster monster)
        {
            string walks = "";
            if (monster.AvoidFire)
            {
                walks += "Fire, ";
            }
            if (monster.AvoidEnergy)
            {
                walks += "Energy, ";
            }
            if (monster.AvoidPoison)
            {
                walks += "Poison, ";
            }
            if (string.IsNullOrWhiteSpace(walks))
            {
                return "None";
            }
            else
            {
                return walks.Substring(0, walks.Length - 2); // Chop off trailing ", "
            }
        }

        private string GenericToTibiaWikiWalkThrough(ref Monster monster)
        {
            string walks = "";
            if (!monster.AvoidFire)
            {
                walks += "Fire, ";
            }
            if (!monster.AvoidEnergy)
            {
                walks += "Energy, ";
            }
            if (!monster.AvoidPoison)
            {
                walks += "Poison, ";
            }
            if (string.IsNullOrWhiteSpace(walks))
            {
                return "None";
            }
            else
            {
                return walks.Substring(0, walks.Length - 2); // Chop off trailing ", "
            }
        }

        private string GenericToTibiaWikiVoice(ref Monster monster)
        {
            string voice = "";
            foreach (var v in monster.Voices)
            {
                if (string.IsNullOrWhiteSpace(voice))
                {
                    voice = v.Sound;
                }
                else
                {
                    voice = $"{voice}|{v.Sound}";
                }
            }
            return $"{{{{Sound List|{voice}}}}}";
        }
    }
}
