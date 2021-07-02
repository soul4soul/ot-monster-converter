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
        private const decimal DEFAULT_LOOT_CHANCE = 0.2M;
        private const int DEFAULT_LOOT_COUNT = 1;

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
        // For each effect tibiawiki includes effect id in the individual effect templates
        // We could generate this via code if we want to, the advance is that it's always up to date and accurate
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
            { "electrical spark effect", Effect.None }, // 12.70
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

        private static IDictionary<string, CombatDamage> WikiToElements = new Dictionary<string, CombatDamage>
        {
            {"physical", CombatDamage.Physical},
            {"energy", CombatDamage.Energy},
            {"earth", CombatDamage.Earth},
            {"fire", CombatDamage.Fire},
            {"life draing", CombatDamage.LifeDrain},
            {"mana drain", CombatDamage.ManaDrain},
            {"healing", CombatDamage.Healing},
            {"drown", CombatDamage.Drown},
            {"ice", CombatDamage.Ice},
            {"holy", CombatDamage.Holy},
            {"death", CombatDamage.Death}
        };

        public override string ConverterName { get => "TibiaWiki"; }

        public override FileSource FileSource { get => FileSource.Web; }

        public override string FileExt { get => "html"; }

        public override bool IsReadSupported { get => true; }

        public override bool IsWriteSupported { get => true; }

        /// <summary>
        /// Boss monsters and single appear monsters don't use article
        /// </summary>
        /// <param name="mon"></param>
        /// <param name="m"></param>
        private static void ParseArticle(Monster mon, string article)
        {
            if (string.IsNullOrWhiteSpace(article))
            {
                mon.Description = mon.Name;
            }
            else
            {
                mon.Description = string.Format("{0} {1}", article.ToLower(), mon.Name).Trim();
            }
        }

        private static void ParseWalksAround(Monster mon, string s)
        {
            foreach (string field in s.ToLower().Split(","))
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

        private static void ParseWalksThrough(Monster mon, string s)
        {
            foreach (string field in s.ToLower().Split(","))
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

        private static void ParseSoundList(Monster mon, string sounds)
        {
            if (TemplateParser.IsTemplateMatch<SoundListTemplate>(sounds))
            {
                var soundTemplated = TemplateParser.Deseralize<SoundListTemplate>(sounds);
                if (soundTemplated.Sounds != null)
                {
                    foreach (string sound in soundTemplated.Sounds)
                    {
                        // Sometimes unknow sound templates include a single empty sound {{SoundList|}}
                        if (!string.IsNullOrWhiteSpace(sound))
                            mon.Voices.Add(new Voice() { Sound = sound, SoundLevel = SoundLevel.Say });
                    }
                }
            }
        }

        private static void ParseBehavior(Monster monster, string behavior)
        {
            // TibiaWiki generally doesn't provide distance so we default to 4. In TFS monster pack 70 of 77 monsters which use distance attack stand at a range of 4.
            behavior = behavior.ToLower();
            if (behavior.Contains("distance") || behavior.Contains("range"))
                monster.TargetDistance = 4;
            else
                monster.TargetDistance = 1;
        }

        /// <summary>
        /// Parsing abilties is implemented like this instead of a using the RegexPatternKeys so we can easily print a list of abilties which fail to be parsed
        /// Abilities from tibiawiki include melee, attacks, and defenses
        /// </summary>
        /// <param name="mon"></param>
        /// <param name="m"></param>
        private static void ParseAbilities(Monster mon, string abilities)
        {
            abilities = abilities.ToLower().Replace("\r", "").Replace("\n", "").Trim();

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
                            if (TryParseRange(match.Groups["damage"].Value, out int min, out int max))
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
                            if (TryParseRange(match.Groups["damage"].Value, out int min, out int max))
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
                            if (TryParseRange(match.Groups["damage"].Value, out int min, out int max))
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
            var abilityList = TemplateParser.Deseralize<AbilityListTemplate>(abilities);

            if (abilityList.Ability != null)
            {
                foreach (string ability in abilityList.Ability)
                {
                    if (TemplateParser.IsTemplateMatch<MeleeTemplate>(ability))
                    {
                        var melee = TemplateParser.Deseralize<MeleeTemplate>(ability);
                        var spell = new Spell() { Name = "melee", SpellCategory = SpellCategory.Offensive, Interval = 2000, Chance = 1 };
                        if (TryParseRange(melee.Damage, out int min, out int max))
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
                    if (TemplateParser.IsTemplateMatch<HealingTemplate>(ability))
                    {
                        var healing = TemplateParser.Deseralize<HealingTemplate>(ability);
                        var spell = new Spell() { Name = "combat", SpellCategory = SpellCategory.Defensive, DamageElement = CombatDamage.Healing, Interval = 2000, Chance = 0.2 };
                        if (TryParseRange(healing.Damage, out int min, out int max))
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
                    if (TemplateParser.IsTemplateMatch<SummonTemplate>(ability))
                    {
                        var summon = TemplateParser.Deseralize<SummonTemplate>(ability);
                        int maxSummons = 1;
                        TryParseRange(summon.Amount, out int min, out maxSummons);
                        mon.MaxSummons += (uint)maxSummons;
                        string firstSummonName = summon.Creature;
                        mon.Summons.Add(new Summon() { Name = firstSummonName });

                        if (summon.Creatures != null)
                        {
                            foreach (var name in summon.Creatures)
                            {
                                mon.Summons.Add(new Summon() { Name = name });
                            }
                        }
                    }
                    if (TemplateParser.IsTemplateMatch<HasteTemplate>(ability))
                    {
                        var haste = TemplateParser.Deseralize<HasteTemplate>(ability);
                        int MinSpeedChange = 300;
                        int MaxSpeedChange = 300;
                        int Duration = 7000;
                        if ((!string.IsNullOrWhiteSpace(haste.Name) && haste.Name.Contains("strong")))
                        {
                            MinSpeedChange = 450;
                            MaxSpeedChange = 450;
                            Duration = 4000;
                        }
                        var spell = new Spell() { Name = "speed", SpellCategory = SpellCategory.Defensive, Interval = 2000, Chance = 0.15, MinSpeedChange = MinSpeedChange, MaxSpeedChange = MaxSpeedChange, AreaEffect = Effect.MagicRed, Duration = Duration };
                        mon.Attacks.Add(spell);
                    }
                    if (TemplateParser.IsTemplateMatch<AbilityTemplate>(ability))
                    {
                        var abilityObj = TemplateParser.Deseralize<AbilityTemplate>(ability);
                        if (TryParseScene(abilityObj.Scene, out Spell spell))
                        {
                            spell.Name = "combat";
                            spell.SpellCategory = SpellCategory.Offensive;
                            spell.DamageElement = CombatDamage.Physical;
                            if (!string.IsNullOrWhiteSpace(abilityObj.Element) && WikiToElements.ContainsKey(abilityObj.Element.ToLower()))
                                spell.DamageElement = WikiToElements[abilityObj.Element.ToLower()];

                            if (TryParseRange(abilityObj.Damage, out int min, out int max))
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
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"{mon.FileName} couldn't parse scene for ability \"{ability}\", likely scene is missing");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"{mon.FileName} ability not parsed \"{ability}\"");
                    }
                }
            }
        }

        /// <summary>
        /// Converts from a TibiaWiki Scene to a generic spell
        /// Information about the spell shape is located at https://tibia.fandom.com/wiki/Module:SceneBuilder/data
        /// </summary>
        /// <param name="input">scene in string form</param>
        /// <param name="spell">generic spell</param>
        /// <returns>True for success</returns>
        private static bool TryParseScene(string input, out Spell spell)
        {
            spell = new Spell() { AreaEffect = Effect.None, ShootEffect = Animation.None, Interval = 2000, Chance = 0.15 };

            if (!TemplateParser.IsTemplateMatch<SceneTemplate>(input))
                return false;

            SceneTemplate scene = TemplateParser.Deseralize<SceneTemplate>(input);
            if ((scene.Missile != null) && (WikiMissilesToAnimations.ContainsKey(scene.Missile)))
            {
                spell.ShootEffect = WikiMissilesToAnimations[scene.Missile];
                spell.OnTarget = true;
            }
            if ((scene.Effect != null) && (WikiToEffects.ContainsKey(scene.Effect)))
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
                    spell.Radius = 1;
                    spell.OnTarget = true;
                    break;
                case "2sqmballtarget":
                    spell.Radius = 2;
                    spell.OnTarget = true;
                    break;
                case "3sqmballtarget":
                    spell.Radius = 3;
                    spell.OnTarget = true;
                    break;
                case "4sqmballtarget":
                    spell.Radius = 4;
                    spell.OnTarget = true;
                    break;
                case "5sqmballtarget":
                    spell.Radius = 5;
                    spell.OnTarget = true;
                    break;
                case "8sqmwave":
                    spell.IsDirectional = true;
                    spell.Spread = 5;
                    spell.Length = 8;
                    break;
                case "10sqmwave":
                    spell.IsDirectional = true;
                    spell.Spread = 5;
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
                    spell.Radius = 5;
                    spell.OnTarget = false;
                    break;
                case "6sqmballself":
                    spell.Radius = 6;
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

            return true;
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

        private void ParseLoot(Monster monster, string lootTable, string filename)
        {
            var lootTableTemplate = TemplateParser.Deseralize<LootTableTemplate>(lootTable);
            if ((lootTableTemplate.Loot != null) && (lootTableTemplate.Loot.Length >= 1) && (!string.IsNullOrWhiteSpace(lootTableTemplate.Loot[0])))
            {
                // Request for full loot stats now that we are sure monster has loot
                string looturl = $"https://tibia.fandom.com/api.php?action=parse&format=json&page=Loot_Statistics:{filename}&prop=wikitext";
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

                                if (!int.TryParse(amount, out int count))
                                {
                                    var amounts = amount.Split("-");
                                    if (amounts.Length >= 2)
                                    {
                                        int.TryParse(amounts[1], out count);
                                    }
                                }
                                count = (count > 0) ? count : 1;

                                monster.Items.Add(new Loot()
                                {
                                    Item = item,
                                    Chance = (decimal)percent,
                                    Count = count
                                });
                            }
                        }
                    }
                }
                else
                {
                    // Creature has loot but no loot statistics. Use information from loot table to generate the loot
                    // Could be loot item template or just a list of items....
                    foreach (string loot in lootTableTemplate.Loot)
                    {
                        if (TemplateParser.IsTemplateMatch<LootItemTemplate>(loot))
                        {
                            LootItemTemplate lootItem = TemplateParser.Deseralize<LootItemTemplate>(loot);
                            if (lootItem.Parts != null)
                            {
                                if (lootItem.Parts.Length == 1)
                                {
                                    // template name only
                                    monster.Items.Add(new Loot() { Item = lootItem.Parts[0], Chance = DEFAULT_LOOT_CHANCE, Count = DEFAULT_LOOT_COUNT });
                                }
                                else if (lootItem.Parts.Length == 2)
                                {
                                    // template name + rarity OR count + name
                                    // Assumes first combination if parts[1] matches a rarity description
                                    if (TryParseTibiaWikiRarity(lootItem.Parts[1], out decimal chance))
                                    {
                                        monster.Items.Add(new Loot() { Item = lootItem.Parts[0], Chance = chance, Count = DEFAULT_LOOT_COUNT });
                                    }
                                    else
                                    {
                                        if (!TryParseRange(lootItem.Parts[0], out int min, out int max))
                                            max = DEFAULT_LOOT_COUNT;
                                        monster.Items.Add(new Loot() { Item = lootItem.Parts[1], Chance = DEFAULT_LOOT_CHANCE, Count = max });
                                    }
                                }
                                else if (lootItem.Parts.Length == 3)
                                {
                                    // template name + rarity + count
                                    if (!TryParseRange(lootItem.Parts[0], out int min, out int max))
                                        max = DEFAULT_LOOT_COUNT;
                                    TryParseTibiaWikiRarity(lootItem.Parts[2], out decimal chance);
                                    monster.Items.Add(new Loot() { Item = lootItem.Parts[1], Chance = chance, Count = max });
                                }
                            }

                        }
                    }
                }
            }
        }

        private static bool TryParseTibiaWikiRarity(string input, out decimal chance)
        {
            chance = DEFAULT_LOOT_CHANCE;
            if (input == null)
                return false;

            input = input.ToLower();
            if (input == "always")
            {
                chance = 1.0M;
                return true;
            }
            else if (input == "common")
            {
                chance = 0.35M;
                return true;
            }
            else if (input == "uncommon")
            {
                chance = 0.15M;
                return true;
            }
            else if (input == "semi-rare")
            {
                chance = 0.025M;
                return true;
            }
            else if (input == "rare")
            {
                chance = 0.075M;
                return true;
            }
            else if (input == "very rare")
            {
                chance = 0.04M;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Creature run at can be number or color based
        /// </summary>
        /// <param name="monster"></param>
        /// <param name="runsAt"></param>
        private void ParseRunAt(Monster monster, string runsAt)
        {
            if (TryParseRange(runsAt, out int min, out int max))
            {
            }
            else if (runsAt == "deep red")
            {
                max = (int)((monster.Health * 0.04) - 1);
                min = 1;
            }
            else if (runsAt == "red")
            {
                max = (int)((monster.Health * 0.3) - 1);
                min = (int)(monster.Health * 0.04);
            }
            else if (runsAt == "yellow")
            {
                max = (int)((monster.Health * 0.6) - 1);
                min = (int)(monster.Health * 0.3);
            }
            else if (runsAt == "light green")
            {
                max = (int)((monster.Health * 0.95) - 1);
                min = (int)(monster.Health * 0.6);
            }
            else if (runsAt == "green")
            {
                max = (int)(monster.Health - 1);
                min = (int)(monster.Health * 0.95);
            }

            if (min == 0)
                monster.RunOnHealth = (uint)max;
            else
                monster.RunOnHealth = (uint)((max + min) / 2);
        }

        /// <summary>
        /// Converts a string representing a numeric range to two intergers
        /// Example numeric ranges which can be parsed are "500", "0-500", and "0-500?"
        /// </summary>
        /// <param name="range">String to parse</param>
        /// <param name="min">lower bound value in range, defaults to 0</param>
        /// <param name="max">high bonund value in the range, will be set when the range only has a single number</param>
        /// <returns>returns false when no numeric values can be parsed</returns>
        private static bool TryParseRange(string range, out int min, out int max)
        {
            min = 0;
            max = 0;

            if (range == null)
                return false;

            Regex rgx = new Regex(@"(?<first>\d+)(([ -]?)(?<second>\d+))?");
            var match = rgx.Match(range);
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

        private static bool RobustTryParse(string input, out uint value)
        {
            value = 0;
            if (input == null)
                return false;

            Regex rgx = new Regex(@"(?<value>\d+)");
            var match = rgx.Match(input);

            return uint.TryParse(match.Groups["value"].Value, out value);
        }


        private static bool RobustTryParse(string input, out bool value)
        {
            value = false;
            if (input == null)
            {
                return false;
            }
            input = input.Trim().ToLower();

            if ((input == "yes") || (input == "y") || (input == "true") || (input == "t") || (input == "1"))
            {
                value = true;
                return true;
            }
            else if ((input == "no") || (input == "n") || (input == "false") || (input == "f") || (input == "0"))
            {
                value = false;
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
            var namematches = new Regex("/wiki/(?<name>[[a-zA-Z0-9.()_%27%C3%B1-]+)").Matches(monsterTable);
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

            var streamTask = client.GetStreamAsync(endpoint);
            var repositories = await JsonSerializer.DeserializeAsync<Root>(await streamTask);
            return repositories.Parse;
        }

        public override ConvertResult ReadMonster(string filename, out Monster monster)
        {
            string resultMessage = "Blood type, look type data, and abilities are not parsed.";

            string monsterurl = $" https://tibia.fandom.com/api.php?action=parse&format=json&page={filename}&prop=wikitext";

            uint uintVal;
            bool boolVal;
            var monsterPage = RequestData(monsterurl).Result;
            InfoboxCreatureTemplate creature = TemplateParser.Deseralize<InfoboxCreatureTemplate>(monsterPage.Wikitext.Empty);
            monster = new Monster();
            if (!string.IsNullOrWhiteSpace(creature.Name)) { monster.RegisteredName = monster.FileName = creature.Name; }
            if (!string.IsNullOrWhiteSpace(creature.ActualName)) { monster.Name = creature.ActualName; }
            if (!string.IsNullOrWhiteSpace(creature.Article)) { ParseArticle(monster, creature.Article); }
            if (RobustTryParse(creature.Hp, out uintVal)) { monster.Health = uintVal; }
            if (RobustTryParse(creature.Exp, out uintVal)) { monster.Experience = uintVal; }
            if (RobustTryParse(creature.Armor, out uintVal)) { monster.TotalArmor = monster.Shielding = uintVal; }
            if (RobustTryParse(creature.Speed, out uintVal)) { monster.Speed = uintVal * 2; }
            if (!string.IsNullOrWhiteSpace(creature.RunsAt)) { ParseRunAt(monster, creature.RunsAt); }
            if (RobustTryParse(creature.Summon, out uintVal)) { monster.SummonCost = uintVal; }
            if (RobustTryParse(creature.Convince, out uintVal)) { monster.ConvinceCost = uintVal; }
            if (RobustTryParse(creature.Illusionable, out boolVal)) { monster.Illusionable = boolVal; }
            if (RobustTryParse(creature.IsBoss, out boolVal)) { monster.IsBoss = boolVal; }
            if (!string.IsNullOrWhiteSpace(creature.PrimaryType)) { monster.HideHealth = creature.PrimaryType.ToLower().Contains("trap"); }
            if (RobustTryParse(creature.Pushable, out boolVal)) { monster.Pushable = boolVal; }
            // In cipbia ability to push objects means ability to push creatures too
            if (RobustTryParse(creature.PushObjects, out boolVal)) { monster.PushItems = monster.PushCreatures = boolVal; }
            if (RobustTryParse(creature.SenseInvis, out boolVal)) { monster.IgnoreInvisible = boolVal; }
            if (RobustTryParse(creature.ParaImmune, out boolVal)) { monster.IgnoreParalyze = boolVal; }
            if (!string.IsNullOrWhiteSpace(creature.WalksAround)) { ParseWalksAround(monster, creature.WalksAround); }
            if (!string.IsNullOrWhiteSpace(creature.WalksThrough)) { ParseWalksThrough(monster, creature.WalksThrough); }
            if (RobustTryParse(creature.PhysicalDmgMod, out uintVal)) { monster.Physical = uintVal / 100.0; }
            if (RobustTryParse(creature.EarthDmgMod, out uintVal)) { monster.Earth = uintVal / 100.0; }
            if (RobustTryParse(creature.FireDmgMod, out uintVal)) { monster.Fire = uintVal / 100.0; }
            if (RobustTryParse(creature.DeathDmgMod, out uintVal)) { monster.Death = uintVal / 100.0; }
            if (RobustTryParse(creature.EnergyDmgMod, out uintVal)) { monster.Energy = uintVal / 100.0; }
            if (RobustTryParse(creature.HolyDmgMod, out uintVal)) { monster.Holy = uintVal / 100.0; }
            if (RobustTryParse(creature.IceDmgMod, out uintVal)) { monster.Ice = uintVal / 100.0; }
            //if (RobustTryParse(creature.HealMod, out uintVal)) { monster. = uintVal / 100.0; }
            if (RobustTryParse(creature.LifeDrainDmgMod, out uintVal)) { monster.LifeDrain = uintVal / 100.0; }
            if (RobustTryParse(creature.DrownDmgMod, out uintVal)) { monster.Drown = uintVal / 100.0; }
            if (!string.IsNullOrWhiteSpace(creature.Sounds)) { ParseSoundList(monster, creature.Sounds); }
            if (!string.IsNullOrWhiteSpace(creature.Behavior)) { ParseBehavior(monster, creature.Behavior); }
            if (!string.IsNullOrWhiteSpace(creature.Abilities)) { ParseAbilities(monster, creature.Abilities); }
            if (!string.IsNullOrWhiteSpace(creature.Loot)) { ParseLoot(monster, creature.Loot, filename); }

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            if (string.IsNullOrWhiteSpace(monster.Name) && !string.IsNullOrWhiteSpace(monster.FileName))
            {
                // Better then nothing guess
                resultMessage += "Guessed creature name";
                monster.Name = monster.FileName;
            }
            monster.Name = textInfo.ToTitleCase(monster.Name);

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
                string.Format("| convince       = {0}", monster.ConvinceCost > 0 ? monster.ConvinceCost.ToString() : "--"),
                string.Format("| illusionable   = {0}", monster.Illusionable ? "yes" : "no"),
                string.Format("| primarytype    = {0}", monster.HideHealth ? "trap" : ""),
                string.Format("| isboss         = {0}", monster.IsBoss ? "yes" : "no"),
                string.Format("| pushable       = {0}", monster.Pushable ? "yes" : "no"),
                string.Format("| pushobjects    = {0}", monster.PushItems ? "yes" : "no"),
                $"| walksaround    = {GenericToTibiaWikiWalkAround(ref monster)}",
                $"| walksthrough   = {GenericToTibiaWikiWalkThrough(ref monster)}",
                string.Format("| paraimmune     = {0}", monster.IgnoreParalyze ? "yes" : "no"),
                string.Format("| senseinvis     = {0}", monster.IgnoreInvisible ? "yes" : "no"),
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
