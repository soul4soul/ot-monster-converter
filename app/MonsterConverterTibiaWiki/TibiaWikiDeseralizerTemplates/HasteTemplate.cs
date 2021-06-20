namespace MonsterConverterTibiaWiki
{
    [TemplateName("Haste", Url = "https://tibia.fandom.com/wiki/Template:Haste")]
    class HasteTemplate
    {
        [TemplateParameter(0, Required = ParameterRequired.Partical, Indicator = ParameterIndicator.Name | ParameterIndicator.Position)]
        public string Name { get; set; }

        [TemplateParameter(1, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Scene { get; set; }
    }
}
