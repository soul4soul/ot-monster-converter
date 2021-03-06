﻿namespace MonsterConverterTibiaWiki
{
    [TemplateName("Haste", Url = "https://tibia.fandom.com/wiki/Template:Haste", BeforeCaptureProperty = "BeforeText", AfterCaptureProperty = "AfterText")]
    class HasteTemplate
    {
        [TemplateParameter(Index = 0, Required = ParameterRequired.Partical, Indicator = ParameterIndicator.Name | ParameterIndicator.Position)]
        public string Name { get; set; }

        [TemplateParameter(Index = 1, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Scene { get; set; }

        [TemplateParameter(Indicator = ParameterIndicator.Name)]
        public string BeforeText { get; set; }

        [TemplateParameter(Indicator = ParameterIndicator.Name)]
        public string AfterText { get; set; }
    }
}
