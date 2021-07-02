namespace MonsterConverterTibiaWiki
{
    [TemplateName("Ability", Url = "https://tibia.fandom.com/wiki/Template:Ability", BeforeCaptureProperty = "BeforeText", AfterCaptureProperty = "AfterText")]
    class AbilityTemplate
    {
        [TemplateParameter(Index = 0, Required = ParameterRequired.Partical, Indicator = ParameterIndicator.Name | ParameterIndicator.Position)]
        public string Name { get; set; }
        [TemplateParameter(Index = 1, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name | ParameterIndicator.Position)]
        public string Damage { get; set; }
        [TemplateParameter(Index = 2, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name | ParameterIndicator.Position)]
        // Possible Element values are the list of damages and conditions https://tibia.fandom.com/wiki/Template:Icon
        public string Element { get; set; }
        [TemplateParameter(Index = 3, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Scene { get; set; }
        [TemplateParameter(Index = 4, Name = "parentheses", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Parentheses { get; set; }
        [TemplateParameter(Indicator = ParameterIndicator.Name)]
        public string BeforeText { get; set; }
        [TemplateParameter(Indicator = ParameterIndicator.Name)]
        public string AfterText { get; set; }
    }
}
