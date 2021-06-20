namespace MonsterConverterTibiaWiki
{
    [TemplateName("Healing", Url = "https://tibia.fandom.com/wiki/Template:Healing")]
    class HealingTemplate
    {
        [TemplateParameter(0, Required = ParameterRequired.Partical)]
        public string Name { get; set; }
        [TemplateParameter(1, Required = ParameterRequired.No)]
        public string Range { get; set; }
        [TemplateParameter(2, Required = ParameterRequired.No)]
        public string Scene { get; set; }
    }
}
