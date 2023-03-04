using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DL;

public partial class AnimeContext : DbContext
{
    public AnimeContext()
    {
    }

    public AnimeContext(DbContextOptions<AnimeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Anime> Animes { get; set; }

    public virtual DbSet<Manga> Mangas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.; Database= Anime; Trusted_Connection=True; User ID=sa; Password=pass@word1;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Anime>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Anime");

            entity.Property(e => e.Gid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("gid");
            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Precision)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("precision");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("type");
            entity.Property(e => e.Vintage)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("vintage");
        });

        modelBuilder.Entity<Manga>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Manga");

            entity.Property(e => e.Gid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("gid");
            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Precision)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("precision");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("type");
            entity.Property(e => e.Vintage)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("vintage");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
