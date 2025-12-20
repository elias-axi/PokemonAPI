namespace PokemonAPI.Models.DTOs.Pokemon
{
    public class PokeApiResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<AbilityInfo> Abilities { get; set; } = new();
        public List<StatInfo> Stats { get; set; } = new();
        public List<TypeInfo> Types { get; set; } = new();
        public SpritesInfo Sprites { get; set; } = new();
        public SpeciesInfo Species { get; set; } = new();
    }

    public class AbilityInfo
    {
        public AbilityDetail Ability { get; set; } = new();
    }

    public class AbilityDetail
    {
        public string Name { get; set; } = string.Empty;
    }

    public class StatInfo
    {
        public int Base_stat { get; set; }
        public StatDetail Stat { get; set; } = new();
    }

    public class StatDetail
    {
        public string Name { get; set; } = string.Empty;
    }

    public class TypeInfo
    {
        public TypeDetail Type { get; set; } = new();
    }

    public class TypeDetail
    {
        public string Name { get; set; } = string.Empty;
    }

    public class SpritesInfo
    {
        public string Front_default { get; set; } = string.Empty;
    }

    public class SpeciesInfo
    {
        public string Url { get; set; } = string.Empty;
    }

    public class SpeciesResponse
    {
        public List<FlavorTextEntry> Flavor_text_entries { get; set; } = new();
    }

    public class FlavorTextEntry
    {
        public string Flavor_text { get; set; } = string.Empty;
        public LanguageInfo Language { get; set; } = new();
    }

    public class LanguageInfo
    {
        public string Name { get; set; } = string.Empty;
    }

}
