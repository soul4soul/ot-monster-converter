namespace MonsterConverterTibiaWiki
{
    [TemplateName("Healing", Url = "https://tibia.fandom.com/wiki/Template:Healing")]
    class HealingTemplate
    {
        [TemplateParameter(0, Required = ParameterRequired.Partical, Indicator = ParameterIndicator.Name | ParameterIndicator.Position)]
        public string Name { get; set; }
        [TemplateParameter(1, Name = "range", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name | ParameterIndicator.Position)]
        public string Damage { get; set; }
        [TemplateParameter(2, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Scene { get; set; }
    }
}
