using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace MainApp.ServerMediator.Models.Diagnostics;

public class DbContextDiagnostics : DbContext
{
	public DbSet<Transformer> Transformer { get; set; }
	public DbSet<Element> Element { get; set; }
	public DbSet<Defect> Defect { get; set; }
	public DbSet<Transformer_Element> Transformer_Element { get; set; }
	public DbSet<Transformer_Element_Defect> Transformer_Element_Defect { get; set; }

	public DbContextDiagnostics(DbContextOptions<DbContextDiagnostics> options) : base(options)
	{
		Database.EnsureCreated();
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=diagnostics;Username=postgres;Password=4444555556");
	}
}