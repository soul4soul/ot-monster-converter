﻿namespace MonsterConverterTibiaWiki
{
    [TemplateName("Scene", Url = "https://tibia.fandom.com/wiki/Template:Scene")]
    class SceneTemplate
    {
        [TemplateParameter(Index = 0, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Spell { get; set; }
        [TemplateParameter(Index = 1, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Effect { get; set; }
        [TemplateParameter(Index = 2, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Background { get; set; }
        [TemplateParameter(Index = 3, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Caster { get; set; }
        [TemplateParameter(Index = 4, Name = "casting_effect", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string CastingEffect { get; set; }
        [TemplateParameter(Index = 5, Name = "look_direction", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string LookDirection { get; set; }
        [TemplateParameter(Index = 6, Name = "effect_on_caster", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string EffectOnCaster { get; set; }
        [TemplateParameter(Index = 7, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Missile { get; set; }
        [TemplateParameter(Index = 8, Name = "missile_direction", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string MissileDirection { get; set; }
        [TemplateParameter(Index = 9, Name = "missile_distance", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string MissileDistance { get; set; }
        [TemplateParameter(Index = 10, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Target { get; set; }
        [TemplateParameter(Index = 11, Name = "effect_on_target", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string EffectOnTarget { get; set; }
        [TemplateParameter(Index = 12, Name = "sprite_1", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Spirt1 { get; set; }
        [TemplateParameter(Index = 13, Name = "sprite_2", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Spirt2 { get; set; }
        [TemplateParameter(Index = 14, Name = "sprite_3", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Spirt3 { get; set; }
        [TemplateParameter(Index = 15, Name = "edge_size", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string EdgeSize { get; set; }
    }
}
