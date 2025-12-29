using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PokemonAPI.Models.Entities;

namespace PokemonAPI.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Pokemon> Pokemons { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Pokemon>(entity =>
        {
            entity.HasIndex(p => p.Name)
                .IsUnique()
                .HasDatabaseName("IX_Pokemons_Name_Unique");
        });
    }

}
