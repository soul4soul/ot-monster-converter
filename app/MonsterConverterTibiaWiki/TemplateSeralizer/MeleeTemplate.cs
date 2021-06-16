namespace MonsterConverterTibiaWiki
{
    [TemplateName("Melee", Url = "https://tibia.fandom.com/wiki/Template:Melee")]
    class MeleeTemplate
    {
        [TemplateParameter("damage", Index = 0, Required = ParameterRequired.No)]
        public string Damage { get; set; }
        [TemplateParameter("element", Index = 1, Required = ParameterRequired.No)]
        public string Element { get; set; }
        [TemplateParameter("name", Index = 2, Required = ParameterRequired.No)]
        public string Name { get; set; }
        [TemplateParameter("scene", Index = 3, Required = ParameterRequired.No)]
        public string Scene { get; set; }
    }
}
