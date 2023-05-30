using MainApp.Context.Utility;
using MainApp.WPF.MVVM.ViewModel;
using System.Windows;

namespace MainApp.ViewModels;

public class Message_ViewModelSTD
{
	public string document_path= "about:blank";
}

class ViewModelSTD : ViewModelBase, IListen<Message_ViewModelSTD>
{
	private string _document_path;
	private Visibility _tab_visibility = Visibility.Collapsed;
	public string DocumentPath
	{
		get { return _document_path; }
		set { _document_path = "file:///"+value; invokePropertiesChanged("BindableSource"); }
	}
	public Visibility Tab_Visibility
	{
		get { return _tab_visibility; }
		set { _tab_visibility=value; invokePropertyChanged("Tab_Visibility"); }
	}

	public ViewModelSTD()
	{
		Title = "Документация";
	}

	public void receive(Message_ViewModelSTD message)
	{
		DocumentPath = message.document_path;
		Tab_Visibility=Visibility.Visible;
	}
}
