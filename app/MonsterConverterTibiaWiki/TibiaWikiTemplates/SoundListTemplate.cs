namespace MonsterConverterTibiaWiki
{
    [TemplateName("Sound List", Url = "https://tibia.fandom.com/wiki/Template:Sound_List")]
    class SoundListTemplate
    {
        [TemplateParameter(Index = 1, Required = ParameterRequired.No, Indicator = ParameterIndicator.Position)]
        public string[] sounds { get; set; }
    }
}
