namespace PokemonAPI.Models.DTOs.Pokemon
{
    public class PokemonResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Power { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Nature { get; set; } = string.Empty;
    }

}
