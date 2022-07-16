namespace MonsterConverterTibiaWiki
{
    [TemplateName("Loot Table", Url = "https://tibia.fandom.com/wiki/Template:Loot_Table")]
    class LootTableTemplate
    {
        [TemplateParameter(Index = 1, Required = ParameterRequired.No, Indicator = ParameterIndicator.Position)]
        public string[] loot { get; set; }
    }
}
