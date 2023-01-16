using MonsterConverterInterface;
using MonsterConverterInterface.MonsterTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
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
    public partial class TibiaWikiConverter : MonsterConverter
    {
        private record TibiaWikiItemData(string Name, string ActualName, string Ids) { }

        private const int STRONG_HASTE_SPEED = 450;
        private const decimal DEFAULT_LOOT_CHANCE = 0.2M;
        private const int DEFAULT_LOOT_COUNT = 1;

        private static readonly HttpClient httpClient = new HttpClient();
        private const string TW_DOMAIN = "https://tibia.fandom.com/";
        private static readonly TimeSpan MAX_CACHE_AGE = TimeSpan.FromDays(1);

        // <damage type, wiki element name>
        private static Dictionary<CombatDamage, string> DamageTypeToWikiElement = new Dictionary<CombatDamage, string>
        {
            {CombatDamage.Physical, "physical"},
            {CombatDamage.Energy, "energy"},
            {CombatDamage.Earth, "earth"},
            {CombatDamage.Fire, "fire"},
            {CombatDamage.LifeDrain, "life drain"},
            {CombatDamage.ManaDrain, "mana drain"},
            {CombatDamage.Healing, "healing"},
            {CombatDamage.Drown, "drown"},
            {CombatDamage.Ice, "ice"},
            {CombatDamage.Holy, "holy"},
            {CombatDamage.Death, "death"}
        };

        // <condition type, wiki condition name>
        private static Dictionary<ConditionType, string> ConditionTypeToWikiElement = new Dictionary<ConditionType, string>
        {
            {ConditionType.Poison, "poisoned"},
            {ConditionType.Fire, "burning"},
            {ConditionType.Energy, "electrified"},
            {ConditionType.Bleeding, "bleeding"},
            {ConditionType.Drown, "drowning"},
            {ConditionType.Freezing, "freezing"},
            {ConditionType.Dazzled, "dazzled"},
            {ConditionType.Cursed, "cursed"},
            {ConditionType.Root, "rooted"},
        };

        // <item name, data>
        private static IDictionary<string, TibiaWikiItemData> itemsByName = new Dictionary<string, TibiaWikiItemData>();
        // <item id, data>
        private static IDictionary<int, TibiaWikiItemData> itemsById = new Dictionary<int, TibiaWikiItemData>();

        // <missile name, missile>
        private static BiDictionary<string, Missile> missileIds = new BiDictionary<string, Missile>();

        // <effect name, missile>
        private static BiDictionary<string, Effect> effectIds = new BiDictionary<string, Effect>();

        public override string ConverterName { get => "TibiaWiki"; }

        public override FileSource FileSource { get => FileSource.Web; }

        public override ItemIdType ItemIdType { get => ItemIdType.Client; }

        public override string FileExt { get => "html"; }

        public override bool IsReadSupported { get => true; }

        public override bool IsWriteSupported { get => true; }

        public override string[] GetFilesForConversion(string directory)
        {
            // directory parameter is ignored for this format...
            IList<string> names = new List<string>();
            var monsterTable = GetWikiPage("List_of_Creatures_(Ordered)", "text").Text.Empty;

            // Links are HTML encoded
            // %27 is HTML encode for ' character
            // %27%C3% is HTML encode for ñ character
            var namematches = new Regex("/wiki/(?<name>[[a-zA-Z0-9.()_%27%C3%B1-]+)").Matches(monsterTable);
            foreach (Match match in namematches)
            {
                names.Add(match.Groups["name"].Value.Replace("%27", "'").Replace("%C3%B1", "ñ"));
            }

            // Populate id lists, here is as good a place as any to fetch and prepare this information
            // Only need to get the list once per program execution, between exeuctions its reasonable to retry should the list be empty
            // It's not worth the overhead to keep trying to get the id lists should they fail during conversion which is the reason
            // the ids are fetched here and not in Parse when the data is needed for the first time.
            GetItemIds();
            GetMissileIds();
            GetEffectIds();

            return names.ToArray();
        }

        private static async Task<string> RequestData(string endpoint)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            return await httpClient.GetStringAsync(endpoint);
        }

        private static Parse GetWikiPage(string page, string prop)
        {
            string directoryToSearch = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            directoryToSearch = Path.Combine(directoryToSearch, "tw_cache");
            if (!Directory.Exists(directoryToSearch))
            {
                Directory.CreateDirectory(directoryToSearch);
            }

            string pageData = string.Empty;
            string cacheFile = Path.Combine(directoryToSearch, $"{CoerceValidFileName(page)}.json");
            if (File.Exists(cacheFile))
            {
                DateTime lastAccess = File.GetLastWriteTime(cacheFile);
                TimeSpan fileAge = System.DateTime.Now - lastAccess;
                if (fileAge < MAX_CACHE_AGE)
                {
                    pageData = File.ReadAllText(cacheFile);
                }
            }

            if (string.IsNullOrEmpty(pageData))
            {
                string endpoint = $"{TW_DOMAIN}/api.php?action=parse&format=json&page={page}&prop={prop}";
                pageData = Task.Run(async () => await RequestData(endpoint)).Result;
                File.WriteAllText(cacheFile, pageData);
            }
            return JsonSerializer.Deserialize<ParseActionRoot>(pageData).Parse;
        }

        private static void GetItemIds()
        {
            if (itemsByName.Count != 0)
            {
                // Ids were already fetched
                return;
            }

            GetItemIds("User:Soul4Soul/List_of_Pickupable_Items", "text");
            // Non-pickable items are needed for shapeshifting abilities using items such as snowball, football, and concooned victimed
            GetItemIds("User:Soul4Soul/List_of_Non-Pickupable_Objects", "text");
        }

        private static void GetItemIds(string page, string prop)
        {
            var itemTable = GetWikiPage(page, prop).Text.Empty;

            var itemMatches = new Regex("\">(?<name>.*?)<\\/a><\\/td>\n<td>(?<actualname>.*?)\n<\\/td>\n<td>(?<itemid>.*?)\n<\\/td>").Matches(itemTable);
            foreach (Match match in itemMatches)
            {
                string name = match.Groups["name"].Value.ToLower();
                string actualName = match.Groups["actualname"].Value.ToLower();
                string ids = match.Groups["itemid"].Value;
                TibiaWikiItemData data = new TibiaWikiItemData(name, actualName, ids);
                if (itemsByName.ContainsKey(name))
                {
                    // Bug with TibiaWiki Data
                    continue;
                }
                itemsByName.Add(name, data);

                if (ushort.TryParse(ids, out ushort id))
                {
                    if (itemsById.ContainsKey(id))
                    {
                        // Bug with TibiaWiki Data
                        continue;
                    }
                    itemsById.Add(id, data);
                }
                else
                {
                    foreach (var i in ids.Split(","))
                    {
                        if (ushort.TryParse(i, out id))
                        {
                            if (itemsById.ContainsKey(id))
                            {
                                // Bug with TibiaWiki Data
                                continue;
                            }
                            itemsById.Add(id, data);
                        }
                    }
                }
            }
        }

        private static void GetMissileIds()
        {
            if (missileIds.Count != 0)
            {
                // Ids were already fetched
                return;
            }

            var missileTable = GetWikiPage("User:Soul4Soul/List_Of_Missiles", "text").Text.Empty;

            var missileMatches = new Regex("\">(?<name>.*?)<\\/a><\\/td>\n<td>(?<id>.*?)\n<\\/td>\n<td>(?<implemented>.*?)\n<\\/td>").Matches(missileTable);
            foreach (Match match in missileMatches)
            {
                string name = match.Groups["name"].Value;
                string id = match.Groups["id"].Value;
                missileIds.Add(name, Enum.Parse<Missile>(id));
            }
        }

        private static void GetEffectIds()
        {
            if (effectIds.Count != 0)
            {
                // Ids were already fetched
                return;
            }

            var effectTable = GetWikiPage("User:Soul4Soul/List_Of_Effects", "text").Text.Empty;

            var effectMatches = new Regex("\">(?<name>.*?)<\\/a><\\/td>\n<td>(?<id>.*?)\n<\\/td>\n<td>(?<implemented>.*?)\n<\\/td>").Matches(effectTable);
            foreach (Match match in effectMatches)
            {
                string name = match.Groups["name"].Value;
                string id = match.Groups["id"].Value;
                if (Enum.TryParse(id, out Effect effect))
                {
                    effectIds.Add(name, effect);
                }
            }
        }

        /// <summary>
        /// Strip illegal chars and reserved words from a candidate filename (should not include the directory path)
        /// </summary>
        /// <remarks>
        /// http://stackoverflow.com/questions/309485/c-sharp-sanitize-file-name
        /// </remarks>
        private static string CoerceValidFileName(string filename)
        {
            var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            var invalidReStr = string.Format(@"[{0}]+", invalidChars);

            var reservedWords = new[]
            {
                "CON", "PRN", "AUX", "CLOCK$", "NUL", "COM0", "COM1", "COM2", "COM3", "COM4",
                "COM5", "COM6", "COM7", "COM8", "COM9", "LPT0", "LPT1", "LPT2", "LPT3", "LPT4",
                "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
            };

            var sanitisedNamePart = Regex.Replace(filename, invalidReStr, "_");
            foreach (var reservedWord in reservedWords)
            {
                var reservedWordPattern = string.Format("^{0}\\.", reservedWord);
                sanitisedNamePart = Regex.Replace(sanitisedNamePart, reservedWordPattern, "_reservedWord_.", RegexOptions.IgnoreCase);
            }

            return sanitisedNamePart;
        }
    }
}
