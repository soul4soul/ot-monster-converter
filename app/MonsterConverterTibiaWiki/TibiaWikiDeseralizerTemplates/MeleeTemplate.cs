namespace MonsterConverterTibiaWiki
{
    [TemplateName("Melee", Url = "https://tibia.fandom.com/wiki/Template:Melee")]
    class MeleeTemplate
    {
        [TemplateParameter(0, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name | ParameterIndicator.Position)]
        public string Damage { get; set; }
        [TemplateParameter(1, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name | ParameterIndicator.Position)]
        public string Element { get; set; }
        [TemplateParameter(2, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Name { get; set; }
        [TemplateParameter(3, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Scene { get; set; }
    }
}
