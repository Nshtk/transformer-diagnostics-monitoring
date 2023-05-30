using MainApp.ServerMediator.Models.Diagnostics;
using MainApp.ServerMediator.Models.Expert;
using MainApp.ServerMediator.Models.Monitoring;
using MainApp.ServerMediator.Services;

internal class Program
{
	private static void Main(string[] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
		WebApplication app;

		builder.Services.AddGrpc();
		builder.Services.AddDbContext<DbContextMonitoring>();
		builder.Services.AddDbContext<DbContextExpert>();
		builder.Services.AddDbContext<DbContextDiagnostics>();
		builder.Services.AddControllersWithViews();

		app = builder.Build();
		app.MapGrpcService<ServiceDbDiagnostics>();
		app.MapGrpcService<ServiceDbExpert>();
		app.MapGrpcService<ServiceDbMonitoring>();
		app.MapGet("/", () => "Blah.");
		app.Run();
	}
}