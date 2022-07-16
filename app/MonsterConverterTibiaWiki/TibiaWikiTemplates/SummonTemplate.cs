namespace MonsterConverterTibiaWiki
{
    [TemplateName("Summon", Url = "https://tibia.fandom.com/wiki/Template:Summon")]
    class SummonTemplate
    {
        [TemplateParameter(Index = 1, Required = ParameterRequired.No, Indicator = ParameterIndicator.Position | ParameterIndicator.Name)]
        public string creature { get; set; }

        [TemplateParameter(Index = 2, Required = ParameterRequired.No, Indicator = ParameterIndicator.Position | ParameterIndicator.Name)]
        public string amount { get; set; } = "1";

        [TemplateParameter(Index = 3, Required = ParameterRequired.No, Indicator = ParameterIndicator.Position)]
        public string[] creatures { get; set; }
    }
}
