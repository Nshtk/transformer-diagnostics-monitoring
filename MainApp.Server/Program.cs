using MainApp.Server.Services;


internal class Program
{
	private static void Main(string[] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
		WebApplication app;

		builder.Services.AddGrpc();
		builder.Services.AddSingleton(new ServiceConnection());
		builder.Services.AddSingleton(new ServiceCommon());
		builder.Services.AddSingleton(new ServiceMonitoring());

		app = builder.Build();
		app.MapGrpcService<ServiceConnection>();
		app.MapGrpcService<ServiceCommon>();
		app.MapGrpcService<ServiceMonitoring>();
		app.MapGet("/", () => "Blah.");
		app.Run();
	}
}