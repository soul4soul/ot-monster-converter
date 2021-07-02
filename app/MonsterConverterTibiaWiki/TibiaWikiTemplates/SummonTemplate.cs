namespace MonsterConverterTibiaWiki
{
    [TemplateName("Summon", Url = "https://tibia.fandom.com/wiki/Template:Summon")]
    class SummonTemplate
    {
        [TemplateParameter(Index = 0, Required = ParameterRequired.No, Indicator = ParameterIndicator.Position)]
        public string Creature { get; set; }

        [TemplateParameter(Index = 1, Required = ParameterRequired.No, Indicator = ParameterIndicator.Position)]
        public string Amount { get; set; } = "1";

        [TemplateParameter(Index = 2, Required = ParameterRequired.No, Indicator = ParameterIndicator.Position)]
        public string[] Creatures { get; set; }
    }
}
