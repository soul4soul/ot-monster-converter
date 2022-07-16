namespace MonsterConverterTibiaWiki
{
    [TemplateName("Loot Item", Url = "https://tibia.fandom.com/wiki/Template:Loot_Item")]
    class LootItemTemplate
    {
        [TemplateParameter(Index = 1, Required = ParameterRequired.No, Indicator = ParameterIndicator.Position)]
        public string[] parts { get; set; }
    }
}
