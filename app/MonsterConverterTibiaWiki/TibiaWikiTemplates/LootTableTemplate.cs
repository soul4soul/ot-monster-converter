namespace MonsterConverterTibiaWiki
{
    [TemplateName("Loot Table", Url = "https://tibia.fandom.com/wiki/Template:Loot_Table")]
    class LootTableTemplate
    {
        [TemplateParameter(Index = 0, Required = ParameterRequired.No, Indicator = ParameterIndicator.Position)]
        public string[] Loot { get; set; }
    }
}
