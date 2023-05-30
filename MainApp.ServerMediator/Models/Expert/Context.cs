using Microsoft.EntityFrameworkCore;

namespace MainApp.ServerMediator.Models.Expert;

public class DbContextExpert : DbContext
{
	public DbSet<Transformer> Transformer { get; set; }
	public DbSet<Documentation> Documentation { get; set; }
	public DbSet<Element> Element { get; set; }
	public DbSet<Parameter> Parameter { get; set; }
	public DbSet<Defect> Defect { get; set; }
	public DbSet<Element_Documentation> Element_Documentation { get; set; }
	public DbSet<Defect_Documentation> Defect_Documentation { get; set; }

	public DbContextExpert(DbContextOptions<DbContextExpert> options) : base(options)
	{
		Database.EnsureCreated();
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=expert;Username=postgres;Password=4444555556");
	}
}