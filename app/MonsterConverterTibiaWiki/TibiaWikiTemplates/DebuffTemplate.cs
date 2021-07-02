namespace MonsterConverterTibiaWiki
{
    [TemplateName("Debuff", Url = "https://tibia.fandom.com/wiki/Template:Debuff", BeforeCaptureProperty = "BeforeText", AfterCaptureProperty = "AfterText")]
    class DebuffTemplate
    {
        [TemplateParameter(Index = 0, Required = ParameterRequired.No, Indicator = ParameterIndicator.Position | ParameterIndicator.Name)]
        public string Name { get; set; }

        [TemplateParameter(Index = 1, Required = ParameterRequired.No, Indicator = ParameterIndicator.Position | ParameterIndicator.Name)]
        public string Effect { get; set; }

        [TemplateParameter(Index = 1, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Scene { get; set; }

        [TemplateParameter(Indicator = ParameterIndicator.Name)]
        public string BeforeText { get; set; }

        [TemplateParameter(Indicator = ParameterIndicator.Name)]
        public string AfterText { get; set; }
    }
}
