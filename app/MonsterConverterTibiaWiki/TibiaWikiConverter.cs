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
    public partial class TibiaWikiConverter : MonsterConverter
    {
        private record TibiaWikiItemData(string Name, string ActualName, string Ids) { }

        private const decimal DEFAULT_LOOT_CHANCE = 0.2M;
        private const int DEFAULT_LOOT_COUNT = 1;

        private static readonly HttpClient httpClient = new HttpClient();

        // <element name, damage type>
        private static BiDictionary<string, CombatDamage> WikiToElements = new BiDictionary<string, CombatDamage>
        {
            {"physical", CombatDamage.Physical},
            {"energy", CombatDamage.Energy},
            {"earth", CombatDamage.Earth},
            {"fire", CombatDamage.Fire},
            {"life drain", CombatDamage.LifeDrain},
            {"mana drain", CombatDamage.ManaDrain},
            {"healing", CombatDamage.Healing},
            {"drown", CombatDamage.Drown},
            {"ice", CombatDamage.Ice},
            {"holy", CombatDamage.Holy},
            {"death", CombatDamage.Death}
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

            // Populate id lists, here is as good a place as any to fetch and prepare this information
            // Only need to get the list once per program execution, between exeuctions its reasonable to retry should the list be empty
            // It's not worth the overhead to keep trying to get the id lists should they fail during conversion which is the reason
            // the ids are fetched here and not in Parse when the data is needed for the first time.
            GetItemIds();
            GetMissileIds();
            GetEffectIds();

            return names.ToArray();
        }

        private static async Task<Parse> RequestData(string endpoint)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();

            var streamTask = httpClient.GetStreamAsync(endpoint);
            var repositories = await JsonSerializer.DeserializeAsync<Root>(await streamTask);
            return repositories.Parse;
        }

        private static void GetItemIds()
        {
            if (itemsByName.Count != 0)
            {
                // Ids were already fetched
                return;
            }

            string itemlisturl = $"https://tibia.fandom.com/api.php?action=parse&format=json&page=User:Soul4Soul/List_of_Pickupable_Items&prop=text";
            var itemTable = RequestData(itemlisturl).Result.Text.Empty;

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

            string missilelisturl = $"https://tibia.fandom.com/api.php?action=parse&format=json&page=User:Soul4Soul/List_Of_Missles&prop=text";
            var missileTable = RequestData(missilelisturl).Result.Text.Empty;

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

                string missilelisturl = $"https://tibia.fandom.com/api.php?action=parse&format=json&page=User:Soul4Soul/List_Of_Effects&prop=text";
            var missileTable = RequestData(missilelisturl).Result.Text.Empty;

            var missileMatches = new Regex("\">(?<name>.*?)<\\/a><\\/td>\n<td>(?<id>.*?)\n<\\/td>\n<td>(?<implemented>.*?)\n<\\/td>").Matches(missileTable);
            foreach (Match match in missileMatches)
            {
                string name = match.Groups["name"].Value;
                string id = match.Groups["id"].Value;
                if (Enum.TryParse(id, out Effect effect))
                {
                    effectIds.Add(name, effect);
                }
            }
        }
    }
}
