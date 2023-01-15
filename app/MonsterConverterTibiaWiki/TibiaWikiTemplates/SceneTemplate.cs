namespace MonsterConverterTibiaWiki
{
    [TemplateName("Scene", Url = "https://tibia.fandom.com/wiki/Template:Scene")]
    class SceneTemplate
    {
        [TemplateParameter(Index = 1, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string spell { get; set; }
        [TemplateParameter(Index = 2, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string effect { get; set; }
        // effect_2
        // background
        [TemplateParameter(Index = 5, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string caster { get; set; }
        [TemplateParameter(Index = 6, Name = "casting_effect", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string CastingEffect { get; set; }
        [TemplateParameter(Index = 7, Name = "look_direction", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string LookDirection { get; set; }
        [TemplateParameter(Index = 8, Name = "effect_on_caster", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string EffectOnCaster { get; set; }
        [TemplateParameter(Index = 9, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string missile { get; set; }
        [TemplateParameter(Index = 10, Name = "missile_direction", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string MissileDirection { get; set; }
        [TemplateParameter(Index = 11, Name = "missile_distance", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string MissileDistance { get; set; }
        // target
        [TemplateParameter(Index = 13, Name = "effect_on_target", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string EffectOnTarget { get; set; }
        [TemplateParameter(Index = 14, Name = "damage_effect", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string DamageEffect { get; set; }
        // sprite_1
        // sprite_2
        // sprite_3
        // edge_size
    }
}
