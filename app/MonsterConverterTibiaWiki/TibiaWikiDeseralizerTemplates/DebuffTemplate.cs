namespace MonsterConverterTibiaWiki
{
    [TemplateName("Debuff", Url = "https://tibia.fandom.com/wiki/Template:Debuff")]
    class DebuffTemplate
    {
        [TemplateParameter(0, Required = ParameterRequired.No, Indicator = ParameterIndicator.Position | ParameterIndicator.Named)]
        public string Name { get; set; }

        [TemplateParameter(1, Required = ParameterRequired.No, Indicator = ParameterIndicator.Position | ParameterIndicator.Named)]
        public string Effect { get; set; }

        [TemplateParameter(1, Required = ParameterRequired.No, Indicator = ParameterIndicator.Named)]
        public string Scene { get; set; }
    }
}
