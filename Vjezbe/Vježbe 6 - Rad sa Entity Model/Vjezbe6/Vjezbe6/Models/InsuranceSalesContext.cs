using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Vjezbe6.Models
{
    public partial class InsuranceSalesContext : DbContext
    {
        public virtual DbSet<Police> Police { get; set; }
        public virtual DbSet<Pribavitelj> Pribavitelj { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.

            var config = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json") .Build();

            string connString = config["Data:DefaultConnection:lokalnaBaza"];
            connString = connString.Replace("lozinka", config["lozinke:lokalnaLozinka"]);

            optionsBuilder.UseSqlServer(connString);

            //  optionsBuilder.UseSqlServer(@"Server=DESKTOP-CK9NDC0\SQLEXPRESS;Database=InsuranceSales;User ID=konekcija;Password=konekcija10");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Police>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DatumIzdavanja).HasColumnType("datetime");

                entity.Property(e => e.DatumKraja).HasColumnType("date");

                entity.Property(e => e.DatumPocetka).HasColumnType("date");

                entity.Property(e => e.Porez).HasColumnType("money");

                entity.Property(e => e.Posrednik).HasColumnType("varchar(50)");

                entity.Property(e => e.Premija).HasColumnType("money");

                entity.Property(e => e.VrstaPolice)
                    .IsRequired()
                    .HasMaxLength(6);

                entity.HasOne(d => d.PribaviteljNavigation)
                    .WithMany(p => p.Police)
                    .HasForeignKey(d => d.Pribavitelj)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Pribavitelj");
            });

            modelBuilder.Entity<Pribavitelj>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Ime)
                    .IsRequired()
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Prezime).HasColumnType("varchar(255)");
            });
        }
    }
}