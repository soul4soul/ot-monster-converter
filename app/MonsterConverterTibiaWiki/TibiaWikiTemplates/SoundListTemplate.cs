namespace MonsterConverterTibiaWiki
{
    [TemplateName("Sound List", Url = "https://tibia.fandom.com/wiki/Template:Sound_List")]
    class SoundListTemplate
    {
        [TemplateParameter(Index = 0, Required = ParameterRequired.No, Indicator = ParameterIndicator.Position)]
        public string[] Sounds { get; set; }
    }
}
