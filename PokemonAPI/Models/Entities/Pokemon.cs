namespace PokemonAPI.Models.Entities
{
    public class Pokemon
    {
        public int Id { get; set; } = default!;
        public string Name { get; set; } = string.Empty;
        public int Power { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Nature { get; set; } = string.Empty;
    }

}
