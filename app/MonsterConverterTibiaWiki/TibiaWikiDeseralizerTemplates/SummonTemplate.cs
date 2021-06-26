namespace MonsterConverterTibiaWiki
{
    [TemplateName("Summon", Url = "https://tibia.fandom.com/wiki/Template:Summon")]
    class SummonTemplate
    {
        [TemplateParameter(0, Required = ParameterRequired.No, Indicator = ParameterIndicator.Position)]
        public string Creature { get; set; }

        [TemplateParameter(1, Required = ParameterRequired.No, Indicator = ParameterIndicator.Position)]
        public string Amount { get; set; } = "1";

        [TemplateParameter(2, Required = ParameterRequired.No, Indicator = ParameterIndicator.Position)]
        public string[] Creatures { get; set; }
    }
}
