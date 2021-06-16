namespace MonsterConverterTibiaWiki
{
    [TemplateName("Haste", Url = "https://tibia.fandom.com/wiki/Template:Haste")]
    class HasteTemplate
    {
        [TemplateParameter("name", Index = 0, Required = ParameterRequired.Partical)]
        public string Name { get; set; }
    }
}
