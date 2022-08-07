namespace MonsterConverterTibiaWiki
{
    [TemplateName("Infobox Creature", Url = "https://tibia.fandom.com/wiki/Template:Infobox_Creature")]
    class InfoboxCreatureTemplate
    {
        // Unknown purpose parameter used by tibiawiki, default the value for compliant output
        [TemplateParameter(Index = 1, Required = ParameterRequired.Yes, Indicator = ParameterIndicator.Name)]
        public string list { get; set; } = "{{{1|}}}";

        // Unknown purpose parameter used by tibiawiki, default the value for compliant output
        [TemplateParameter(Index = 2, Required = ParameterRequired.Yes, Indicator = ParameterIndicator.Name)]
        public string getValue { get; set; } = "{{{GetValue|}}}";

        [TemplateParameter(Index = 3, Required = ParameterRequired.Yes, Indicator = ParameterIndicator.Name)]
        public string name { get; set; }

        [TemplateParameter(Index = 4, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string article { get; set; }

        [TemplateParameter(Index = 5, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string actualname { get; set; }

        [TemplateParameter(Index = 6, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string plural { get; set; }

        [TemplateParameter(Index = 7, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string hp { get; set; }

        [TemplateParameter(Index = 8, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string exp { get; set; }

        [TemplateParameter(Index = 9, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string armor { get; set; }

        [TemplateParameter(Index = 10, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string summon { get; set; }

        [TemplateParameter(Index = 11, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string convince { get; set; }

        [TemplateParameter(Index = 12, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string illusionable { get; set; }

        [TemplateParameter(Index = 13, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string primarytype { get; set; }

        [TemplateParameter(Index = 16, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string bestiaryclass { get; set; }

        [TemplateParameter(Index = 17, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string bestiarylevel { get; set; }

        [TemplateParameter(Index = 18, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string occurrence { get; set; }

        [TemplateParameter(Index = 19, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string attacktype { get; set; }

        [TemplateParameter(Index = 20, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string usespells { get; set; }

        [TemplateParameter(Index = 21, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string spawntype { get; set; }

        [TemplateParameter(Index = 22, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string isboss { get; set; }

        [TemplateParameter(Index = 23, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string bosstiaryclass { get; set; }

        [TemplateParameter(Index = 24, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string isarenaboss { get; set; }

        [TemplateParameter(Index = 25, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string abilities { get; set; }

        [TemplateParameter(Index = 28, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string lightcolor { get; set; }

        [TemplateParameter(Index = 29, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string lightradius { get; set; }

        [TemplateParameter(Index = 30, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string pushable { get; set; }

        [TemplateParameter(Index = 31, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string pushobjects { get; set; }

        [TemplateParameter(Index = 32, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string walksaround { get; set; }

        [TemplateParameter(Index = 33, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string walksthrough { get; set; }

        [TemplateParameter(Index = 34, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string paraimmune { get; set; }

        [TemplateParameter(Index = 35, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string senseinvis { get; set; }

        [TemplateParameter(Index = 36, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string physicaldmgmod { get; set; }

        [TemplateParameter(Index = 37, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string earthdmgmod { get; set; }

        [TemplateParameter(Index = 38, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string firedmgmod { get; set; }

        [TemplateParameter(Index = 39, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string deathdmgmod { get; set; }

        [TemplateParameter(Index = 40, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string energydmgmod { get; set; }

        [TemplateParameter(Index = 41, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string holydmgmod { get; set; }

        [TemplateParameter(Index = 42, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string icedmgmod { get; set; }

        [TemplateParameter(Index = 43, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string healmod { get; set; }

        [TemplateParameter(Index = 44, Name = "hpDrainDmgMod", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string lifedraindmgmod { get; set; }

        [TemplateParameter(Index = 45, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string drowndmgmod { get; set; }

        [TemplateParameter(Index = 48, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string sounds { get; set; }

        [TemplateParameter(Index = 50, Name = "race_id", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string raceid { get; set; }

        [TemplateParameter(Index = 48, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string notes { get; set; }

        [TemplateParameter(Index = 52, Name = "behaviour", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string behavior { get; set; }

        [TemplateParameter(Index = 53, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string runsat { get; set; }

        [TemplateParameter(Index = 54, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string speed { get; set; }

        [TemplateParameter(Index = 56, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string location { get; set; }

        [TemplateParameter(Index = 57, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string loot { get; set; }
    }
}
