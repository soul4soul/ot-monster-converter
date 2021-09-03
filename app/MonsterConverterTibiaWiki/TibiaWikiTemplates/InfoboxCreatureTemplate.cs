namespace MonsterConverterTibiaWiki
{
    [TemplateName("Infobox Creature", Url = "https://tibia.fandom.com/wiki/Template:Infobox_Creature")]
    class InfoboxCreatureTemplate
    {
        [TemplateParameter(Index = 2, Required = ParameterRequired.Yes, Indicator = ParameterIndicator.Name)]
        public string Name { get; set; }

        [TemplateParameter(Index = 3, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Article { get; set; }

        [TemplateParameter(Index = 4, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string ActualName { get; set; }

        [TemplateParameter(Index = 5, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Plural { get; set; }

        [TemplateParameter(Index = 6, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Hp { get; set; }

        [TemplateParameter(Index = 7, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Exp { get; set; }

        [TemplateParameter(Index = 8, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Armor { get; set; }

        [TemplateParameter(Index = 9, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Summon { get; set; }

        [TemplateParameter(Index = 10, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Convince { get; set; }

        [TemplateParameter(Index = 11, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Illusionable { get; set; }

        [TemplateParameter(Index = 13, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string PrimaryType { get; set; }

        [TemplateParameter(Index = 15, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string BestiaryClass { get; set; }

        [TemplateParameter(Index = 16, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string BestiaryLevel { get; set; }

        [TemplateParameter(Index = 17, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Occurrence { get; set; }

        [TemplateParameter(Index = 18, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string SpawnType { get; set; }

        [TemplateParameter(Index = 19, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string IsBoss { get; set; }

        [TemplateParameter(Index = 21, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Abilities { get; set; }

        [TemplateParameter(Index = 24, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Pushable { get; set; }

        [TemplateParameter(Index = 25, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string PushObjects { get; set; }

        [TemplateParameter(Index = 16, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string WalksAround { get; set; }

        [TemplateParameter(Index = 27, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string WalksThrough { get; set; }

        [TemplateParameter(Index = 28, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string ParaImmune { get; set; }

        [TemplateParameter(Index = 29, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string SenseInvis { get; set; }

        [TemplateParameter(Index = 30, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string PhysicalDmgMod { get; set; }

        [TemplateParameter(Index = 31, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string EarthDmgMod { get; set; }

        [TemplateParameter(Index = 32, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string FireDmgMod { get; set; }

        [TemplateParameter(Index = 33, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string DeathDmgMod { get; set; }

        [TemplateParameter(Index = 34, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string EnergyDmgMod { get; set; }

        [TemplateParameter(Index = 35, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string HolyDmgMod { get; set; }

        [TemplateParameter(Index = 36, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string IceDmgMod { get; set; }

        [TemplateParameter(Index = 37, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string HealMod { get; set; }

        [TemplateParameter(Index = 38, Name = "hpDrainDmgMod", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string LifeDrainDmgMod { get; set; }

        [TemplateParameter(Index = 39, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string DrownDmgMod { get; set; }

        [TemplateParameter(Index = 42, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Sounds { get; set; }

        [TemplateParameter(Index = 44, Name = "race_id", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string RaceId { get; set; }

        [TemplateParameter(Index = 45, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Notes { get; set; }

        [TemplateParameter(Index = 46, Name = "behaviour", Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Behavior { get; set; }

        [TemplateParameter(Index = 47, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string RunsAt { get; set; }

        [TemplateParameter(Index = 48, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Speed { get; set; }

        [TemplateParameter(Index = 51, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Location { get; set; }

        [TemplateParameter(Index = 51, Required = ParameterRequired.No, Indicator = ParameterIndicator.Name)]
        public string Loot { get; set; }
    }
}
