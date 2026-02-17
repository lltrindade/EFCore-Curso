using System;
using FuscaFilmes.Entities;
using Microsoft.EntityFrameworkCore;


namespace FuscaFilmes.DbContexts;

public class Context(DbContextOptions<Context> options) : DbContext(options)
{
    public DbSet<Filme> Filmes { get; set; }
    public DbSet<Diretor> Diretores { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Diretor>()
            .HasMany(e => e.Filmes)
            .WithOne(e => e.Diretor)
            .HasForeignKey(e => e.DiretorId)
            .IsRequired();
    }

}
