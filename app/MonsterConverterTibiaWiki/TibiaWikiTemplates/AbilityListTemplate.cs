namespace MonsterConverterTibiaWiki
{
    [TemplateName("Ability List", Url = "https://tibia.fandom.com/wiki/Template:Ability_List")]
    class AbilityListTemplate
    {
        [TemplateParameter(Index = 0, Required = ParameterRequired.No, Indicator = ParameterIndicator.Position)]
        public string[] ability { get; set; }
    }
}
