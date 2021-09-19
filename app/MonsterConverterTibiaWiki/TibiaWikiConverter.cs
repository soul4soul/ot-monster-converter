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

        private static IDictionary<string, TibiaWikiItemData> itemids = new Dictionary<string, TibiaWikiItemData>();

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

            // Populate item id list, here is as good a place as any to fetch and prepare this information
            // Only need to get the list once per program execution, between exeuctions its reasonable to retry should the list be empty
            // It's not worth the overhead to keep trying to get the itemid list should it fail during conversion which is the reason
            // the itemids are fetched here and not in ParseLoot when the data is needed.
            if (itemids.Count == 0)
            {
                GetItemIds();
            }
            

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
            string itemlisturl = $"https://tibia.fandom.com/api.php?action=parse&format=json&page=User:Soul4Soul/List_of_Pickupable_Items&prop=text";
            var itemTable = RequestData(itemlisturl).Result.Text.Empty;

            var itemMatches = new Regex("\">(?<name>.*?)<\\/a><\\/td>\n<td>(?<actualname>.*?)\n<\\/td>\n<td>(?<itemid>.*?)\n<\\/td>").Matches(itemTable);
            foreach (Match match in itemMatches)
            {
                string name = match.Groups["name"].Value.ToLower();
                string actualName = match.Groups["actualname"].Value.ToLower();
                string ids = match.Groups["itemid"].Value;
                itemids.Add(name, new TibiaWikiItemData(name, actualName, ids));
            }
        }
    }
}
