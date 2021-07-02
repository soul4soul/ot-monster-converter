using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MonsterConverterTibiaWiki
{
    public partial class Root
    {
        [JsonPropertyName("parse")]
        public Parse Parse { get; set; }
    }

    public partial class Parse
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("pageid")]
        public long Pageid { get; set; }

        [JsonPropertyName("text")]
        public Text Text { get; set; }

        [JsonPropertyName("wikitext")]
        public Wikitext Wikitext { get; set; }
    }

    public partial class Wikitext
    {
        [JsonPropertyName("*")]
        public string Empty { get; set; }
    }

    public partial class Text
    {
        [JsonPropertyName("*")]
        public string Empty { get; set; }
    }
}
