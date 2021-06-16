namespace MonsterConverterTibiaWiki
{
    [TemplateName("Ability", Url = "https://tibia.fandom.com/wiki/Template:Ability")]
    class AbilityTemplate
    {
        [TemplateParameter("name", Index = 0, Required = ParameterRequired.Partical)]
        public string Name { get; set; }
        [TemplateParameter("damage", Index = 1, Required = ParameterRequired.No)]
        public string Damage { get; set; }
        [TemplateParameter("element", Index = 2, Required = ParameterRequired.No)]
        public string Element { get; set; }
        [TemplateParameter("scene", Index = 3, Required = ParameterRequired.No)]
        public string Scene { get; set; }
        [TemplateParameter("parentheses", Index = 4, Required = ParameterRequired.No)]
        public string Parentheses { get; set; }
    }
}
