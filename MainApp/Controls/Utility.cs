using CefSharp;
using MainApp.Views;
using System.Windows.Controls;

namespace MainApp.Controls;

public class ViewLocator
{
	private UserControl[] _views=new UserControl[] { new ViewOverview(), new ViewTechnicalCondition(), new ViewPlanning(), new ViewServer(), new ViewSTD() };
	public ViewOverview ViewOverview
	{
		get { return (ViewOverview)_views[0]; }
	}
	public ViewTechnicalCondition ViewTechnicalCondition
	{
		get { return (ViewTechnicalCondition)_views[1]; }
	}
	public ViewPlanning ViewPlanning
	{
		get { return (ViewPlanning)_views[2]; }
	}
	public ViewServer ViewServer
	{
		get { return (ViewServer)_views[3]; }
	}
	public ViewSTD ViewSTD
	{
		get { return (ViewSTD)_views[4]; }
	}
}

public class CustomProtocolSchemeHandler : ResourceHandler
{
	public CustomProtocolSchemeHandler()
	{}

	public override CefReturnValue ProcessRequestAsync(IRequest request, ICallback callback)
	{
		return CefReturnValue.ContinueAsync;
	}
}

public class CustomProtocolSchemeHandlerFactory : ISchemeHandlerFactory
{
	public const string SchemeName = "customFileProtocol";

	public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
	{
		return new CustomProtocolSchemeHandler();
	}
}