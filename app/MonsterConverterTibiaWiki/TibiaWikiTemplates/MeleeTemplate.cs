﻿namespace MonsterConverterTibiaWiki
{
    [TemplateName("Melee", Url = "https://tibia.fandom.com/wiki/Template:Melee", BeforeCaptureProperty = "BeforeText", AfterCaptureProperty = "AfterText")]
    class MeleeTemplate
    {
        [TemplateParameter(Index = 1, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name | ParameterIndicator.Position)]
        public string damage { get; set; }
        [TemplateParameter(Index = 2, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name | ParameterIndicator.Position)]
        public string element { get; set; }
        [TemplateParameter(Index = 3, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string name { get; set; }
        [TemplateParameter(Index = 4, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string scene { get; set; }
        [TemplateParameter(Index = 5, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string poison { get; set; }
        [TemplateParameter(Indicator = ParameterIndicator.Name)]
        public string BeforeText { get; set; }
        [TemplateParameter(Indicator = ParameterIndicator.Name)]
        public string AfterText { get; set; }
    }
}
