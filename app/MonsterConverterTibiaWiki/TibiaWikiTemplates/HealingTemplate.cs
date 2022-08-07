namespace MonsterConverterTibiaWiki
{
    [TemplateName("Healing", Url = "https://tibia.fandom.com/wiki/Template:Healing", BeforeCaptureProperty = "BeforeText", AfterCaptureProperty = "AfterText")]
    class HealingTemplate
    {
        [TemplateParameter(Index = 1, Required = ParameterRequired.Partical, Indicator = ParameterIndicator.Name | ParameterIndicator.Position)]
        public string name { get; set; }
        [TemplateParameter(Index = 2, Name = "range", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name | ParameterIndicator.Position)]
        public string damage { get; set; }
        [TemplateParameter(Index = 3, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string scene { get; set; }
        [TemplateParameter(Indicator = ParameterIndicator.Name)]
        public string BeforeText { get; set; }
        [TemplateParameter(Indicator = ParameterIndicator.Name)]
        public string AfterText { get; set; }
    }
}
