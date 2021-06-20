namespace MonsterConverterTibiaWiki
{
    [TemplateName("Scene", Url = "https://tibia.fandom.com/wiki/Template:Scene")]
    class SceneTemplate
    {
        [TemplateParameter(0, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Spell { get; set; }
        [TemplateParameter(1, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Effect { get; set; }
        [TemplateParameter(2, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Background { get; set; }
        [TemplateParameter(3, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Caster { get; set; }
        [TemplateParameter(4, Name = "casting_effect", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string CastingEffect { get; set; }
        [TemplateParameter(5, Name = "look_direction", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string LookDirection { get; set; }
        [TemplateParameter(6, Name = "effect_on_caster", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string EffectOnCaster { get; set; }
        [TemplateParameter(7, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Missile { get; set; }
        [TemplateParameter(8, Name = "missile_direction", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string MissileDirection { get; set; }
        [TemplateParameter(9, Name = "missile_distance", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string MissileDistance { get; set; }
        [TemplateParameter(10, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Target { get; set; }
        [TemplateParameter(11, Name = "effect_on_target", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string EffectOnTarget { get; set; }
        [TemplateParameter(12, Name = "sprite_1", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Spirt1 { get; set; }
        [TemplateParameter(13, Name = "sprite_2", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Spirt2 { get; set; }
        [TemplateParameter(14, Name = "sprite_3", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Spirt3 { get; set; }
        [TemplateParameter(15, Name = "edge_size", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string EdgeSize { get; set; }
    }
}
