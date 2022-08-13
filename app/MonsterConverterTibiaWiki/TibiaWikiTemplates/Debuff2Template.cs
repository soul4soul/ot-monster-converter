namespace MonsterConverterTibiaWiki
{
    [TemplateName("Debuff2", Url = "https://tibia.fandom.com/wiki/Template:Debuff2", BeforeCaptureProperty = "BeforeText", AfterCaptureProperty = "AfterText")]
    class Debuff2Template
    {
        [TemplateParameter(Index = 1, Required = ParameterRequired.No, Indicator = ParameterIndicator.Position | ParameterIndicator.Name)]
        public string name { get; set; }

        [TemplateParameter(Index = 2, Required = ParameterRequired.No, Indicator = ParameterIndicator.Position | ParameterIndicator.Name)]
        public string melee { get; set; }

        [TemplateParameter(Index = 3, Required = ParameterRequired.No, Indicator = ParameterIndicator.Position | ParameterIndicator.Name)]
        public string distance { get; set; }

        [TemplateParameter(Index = 4, Required = ParameterRequired.No, Indicator = ParameterIndicator.Position | ParameterIndicator.Name)]
        public string shielding { get; set; }

        [TemplateParameter(Index = 5, Required = ParameterRequired.No, Indicator = ParameterIndicator.Position | ParameterIndicator.Name)]
        public string magic { get; set; }

        [TemplateParameter(Index = 6, Required = ParameterRequired.No, Indicator = ParameterIndicator.Position | ParameterIndicator.Name)]
        public string value { get; set; }

        [TemplateParameter(Index = 7, Required = ParameterRequired.No, Indicator = ParameterIndicator.Position | ParameterIndicator.Name)]
        public string duration { get; set; }

        [TemplateParameter(Index = 8, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string scene { get; set; }

        [TemplateParameter(Indicator = ParameterIndicator.Name)]
        public string BeforeText { get; set; }

        [TemplateParameter(Indicator = ParameterIndicator.Name)]
        public string AfterText { get; set; }
    }
}
