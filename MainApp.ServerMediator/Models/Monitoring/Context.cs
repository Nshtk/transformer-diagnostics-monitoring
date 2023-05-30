using Microsoft.EntityFrameworkCore;

namespace MainApp.ServerMediator.Models.Monitoring;
public class DbContextMonitoring : DbContext
{
    public DbSet<Building> Building { get; set; }
    public DbSet<Room> Room { get; set; }
    public DbSet<Transformer> Transformer { get; set; }
    public DbSet<Sensor> Sensor { get; set; }
    public DbSet<Sensor_Transformer> Sensor_Transformer { get; set; }

    public DbContextMonitoring()
    {
        Database.EnsureCreated();
    }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=monitoring;Username=postgres;Password=4444555556");
	}
}