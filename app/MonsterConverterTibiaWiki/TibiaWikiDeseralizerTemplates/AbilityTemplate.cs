namespace MonsterConverterTibiaWiki
{
    [TemplateName("Ability", Url = "https://tibia.fandom.com/wiki/Template:Ability")]
    class AbilityTemplate
    {
        [TemplateParameter(0, Required = ParameterRequired.Partical)]
        public string Name { get; set; }
        [TemplateParameter(1, Required = ParameterRequired.No)]
        public string Damage { get; set; }
        [TemplateParameter(2, Required = ParameterRequired.No)]
        public string Element { get; set; }
        [TemplateParameter(3, Required = ParameterRequired.No)]
        public string Scene { get; set; }
        [TemplateParameter(4, Name = "parentheses", Required = ParameterRequired.No)]
        public string Parentheses { get; set; }
    }
}
