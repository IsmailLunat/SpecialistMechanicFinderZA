using MechanicFinder.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MechanicFinder.Infrastructure.Data;

public class MechanicFinderDbContext : DbContext
{
    public MechanicFinderDbContext(DbContextOptions<MechanicFinderDbContext> options) : base(options)
    {
    }

    public DbSet<Province> Provinces => Set<Province>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<VehicleMake> VehicleMakes => Set<VehicleMake>();
    public DbSet<SpecialtyCategory> SpecialtyCategories => Set<SpecialtyCategory>();
    public DbSet<Business> Businesses => Set<Business>();
    public DbSet<BusinessVehicleMake> BusinessVehicleMakes => Set<BusinessVehicleMake>();
    public DbSet<BusinessSpecialty> BusinessSpecialties => Set<BusinessSpecialty>();
    public DbSet<BusinessRating> BusinessRatings => Set<BusinessRating>();
    public DbSet<BusinessSource> BusinessSources => Set<BusinessSource>();
    public DbSet<BusinessEvidence> BusinessEvidences => Set<BusinessEvidence>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Province>()
            .HasIndex(p => p.Code)
            .IsUnique();

        modelBuilder.Entity<Province>()
            .HasIndex(p => p.Name)
            .IsUnique();

        modelBuilder.Entity<City>()
            .HasIndex(c => new { c.ProvinceId, c.Name })
            .IsUnique();

        modelBuilder.Entity<VehicleMake>()
            .HasIndex(v => v.NormalizedName)
            .IsUnique();

        modelBuilder.Entity<SpecialtyCategory>()
            .HasIndex(s => s.Slug)
            .IsUnique();

        modelBuilder.Entity<Business>()
            .HasIndex(b => b.NormalizedName);

        modelBuilder.Entity<Business>()
            .HasIndex(b => b.CityId);

        modelBuilder.Entity<BusinessVehicleMake>()
            .HasKey(x => new { x.BusinessId, x.VehicleMakeId });

        modelBuilder.Entity<BusinessSpecialty>()
            .HasKey(x => new { x.BusinessId, x.SpecialtyCategoryId });

        modelBuilder.Entity<BusinessSource>()
            .HasIndex(x => new { x.BusinessId, x.SourceName, x.ExternalBusinessId })
            .IsUnique();

        modelBuilder.Entity<BusinessRating>()
            .HasIndex(x => new { x.BusinessId, x.Source, x.ObservedAtUtc });

        modelBuilder.Entity<BusinessEvidence>()
            .HasIndex(x => x.BusinessId);

        modelBuilder.Entity<Business>()
            .HasOne(b => b.City)
            .WithMany()
            .HasForeignKey(b => b.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BusinessVehicleMake>()
            .HasOne(bvm => bvm.Business)
            .WithMany(b => b.BusinessVehicleMakes)
            .HasForeignKey(bvm => bvm.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BusinessSpecialty>()
            .HasOne(bsp => bsp.Business)
            .WithMany(b => b.BusinessSpecialties)
            .HasForeignKey(bsp => bsp.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BusinessSource>()
            .HasOne(bs => bs.Business)
            .WithMany(b => b.Sources)
            .HasForeignKey(bs => bs.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BusinessRating>()
            .HasOne(br => br.Business)
            .WithMany(b => b.Ratings)
            .HasForeignKey(br => br.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BusinessEvidence>()
            .HasOne(be => be.Business)
            .WithMany(b => b.Evidences)
            .HasForeignKey(be => be.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
