using Microsoft.EntityFrameworkCore;
using WebApi.API.Database.Tables;

namespace WebApi.API.Database;

public partial class PlantsContext : DbContext
{
	public PlantsContext()
	{
	}

	public PlantsContext(DbContextOptions<PlantsContext> options)
		: base(options)
	{
	}

	public virtual DbSet<Country> Countries { get; set; }

	public virtual DbSet<Plant> Plants { get; set; }

	public virtual DbSet<PlantRoot> PlantsRoots { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Country>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PK_Страны");

			entity.Property(e => e.Capital)
				.HasMaxLength(50)
				.IsFixedLength();
			entity.Property(e => e.Mainland)
				.HasMaxLength(50)
				.IsFixedLength();
			entity.Property(e => e.Name)
				.HasMaxLength(50)
				.IsFixedLength();
		});

		modelBuilder.Entity<Plant>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PK_Растения");

			entity.Property(e => e.Family).HasMaxLength(50);
			entity.Property(e => e.Name).HasMaxLength(50);
			entity.Property(e => e.Section).HasMaxLength(50);
		});

		modelBuilder.Entity<PlantRoot>(entity =>
		{
			entity.HasKey(e => new { e.CountryId, e.PlantId }).HasName("PK_РастенияВСтране");

			entity.HasOne(d => d.Country).WithMany(p => p.PlantsRoots)
				.HasForeignKey(d => d.CountryId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_РастенияВСтране_Страны");

			entity.HasOne(d => d.Plant).WithMany(p => p.PlantsRoots)
				.HasForeignKey(d => d.PlantId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_РастенияВСтране_Растения");
		});

		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
