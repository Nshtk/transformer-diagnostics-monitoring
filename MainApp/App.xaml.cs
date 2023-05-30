using System.Windows;
using System.Windows.Navigation;
using CefSharp;
using CefSharp.Wpf;
using MainApp.Controls;

namespace MainApp;

public partial class App : Application
{
	protected override void OnLoadCompleted(NavigationEventArgs e)
	{
		var settings = new CefSettings();
		settings.RegisterScheme(new CefCustomScheme
		{
			SchemeName = CustomProtocolSchemeHandlerFactory.SchemeName,
			SchemeHandlerFactory = new CustomProtocolSchemeHandlerFactory(),
			IsCSPBypassing = true
		});

		settings.LogSeverity = LogSeverity.Error;
		Cef.Initialize(settings);
	}
}
