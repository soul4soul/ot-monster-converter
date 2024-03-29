﻿namespace MonsterConverterTibiaWiki
{
    [TemplateName("Outfit", Url = "https://tibia.fandom.com/wiki/Template:Outfit")]
    class OutfitTemplate
    {
        [TemplateParameter(Index = 1, Required = ParameterRequired.Yes, Indicator = ParameterIndicator.Position | ParameterIndicator.Name)]
        public string victim { get; set; }

        [TemplateParameter(Index = 2, Required = ParameterRequired.No, Indicator = ParameterIndicator.Position | ParameterIndicator.Name)]
        public string thing { get; set; }

        [TemplateParameter(Index = 3, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string scene { get; set; } = "1";

        [TemplateParameter(Index = 4, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string thing2 { get; set; }

        [TemplateParameter(Index = 5, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string thing3 { get; set; }

        [TemplateParameter(Index = 6, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string thing4 { get; set; }
    }
}
