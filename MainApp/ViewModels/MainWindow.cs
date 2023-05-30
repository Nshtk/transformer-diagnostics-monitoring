using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using MainApp.Models;
using MainApp.WPF.MVVM.ViewModel;
using MainApp.Context;
using MainApp.Context.Utility;
using MainApp.ViewModels;

namespace MainApp.ViewModels;

public class ViewModelMainWindow : ViewModelBase, IListen<ViewModelServer.Message_Transformers>, IListen<ViewModelMainWindow.Message_Transformer_Initial>
{
	public class Message_Transformer_Initial
	{
		public ModelTransformer transformer;
	}
	public class Message_Transformer
	{
		public ModelTransformer transformer;
		// TODO constructor
	}

	#region Fields
	private ModelTransformer _transformer_selected;

	#region WPF
	private ObservableCollection<ViewModelBase> _tabs;
	private ObservableCollection<ModelTransformer> _transformers = new ObservableCollection<ModelTransformer>();

	private RelayCommand _command_load_transformer;
	private RelayCommand _command_open_document;
	private RelayCommand _command_diagnose_offline;
	/*private RelayCommand _command_open_tab;
	private RelayCommand _command_close_tab;*/
	private bool _command_diagnose_offline_can_execute=false;
		#endregion
	#endregion

	#region Properties
	public ObservableCollection<ModelTransformer> Transformers
	{ 
		get { return _transformers; } 
		set { _transformers = value; invokePropertyChanged("Transformers"); }
	}

		#region WPF
	public ObservableCollection<ViewModelBase> Tabs
	{
		get { return _tabs; }
		set { _tabs = value; invokePropertyChanged("Tabs"); }
	}

	public RelayCommand CommandOpenDocument
	{
		get { return _command_open_document ??= new RelayCommand(openDocument_execute, openDocument_canExecute); }
	}
	public RelayCommand CommandLoadTransformer
	{
		get { return _command_load_transformer ??= new RelayCommand(loadTransformer_execute, loadTransformer_canExecute); }
	}
	public RelayCommand CommandDiagnoseOffline
	{
		get { return _command_diagnose_offline??= new RelayCommand(diagnoseOffline_execute, diagnoseOffline_canExecute); }
	}
	/*public RelayCommand CommandOpenTab
	{
		get { return _command_open_tab ??= new RelayCommand(openTab_execute, openTab_canExecute); }
	}
	public RelayCommand CommandCloseTab
	{
		get { return _command_close_tab ??= new RelayCommand(closeTab_execute, closeTab_canExecute); }
	}*/
		#endregion
	#endregion

	public ViewModelMainWindow() 
	{
		Utility.Event_Aggregator.subscribe(this);
		_tabs= new ObservableCollection<ViewModelBase> { new ViewModelOverview(), new ViewModelTechnicalCondition(), new ViewModelPlanning(), new ViewModelServer(), new ViewModelSTD() };
		//Utility.Event_Aggregator.subscribe((IListen)Tabs[0]);
		Utility.Event_Aggregator.subscribe((IListen)Tabs[1]);
		Utility.Event_Aggregator.subscribe((IListen)Tabs[2]);
		Utility.Event_Aggregator.subscribe((IListen)Tabs[3]);
		Utility.Event_Aggregator.subscribe((IListen)Tabs[4]);
	}

	#region Methods


	#region WPF
	private void loadTransformer_execute(object parameter)
	{
		// TODO load transformer documentation data from server
		if (_transformer_selected!=(ModelTransformer)parameter)
		{
			if(_transformer_selected!=null)
				_transformer_selected.IsLoaded = false;
			_transformer_selected = (ModelTransformer)parameter;
			Utility.Event_Aggregator.send(new ViewModelMainWindow.Message_Transformer { transformer = _transformer_selected });
		}
		_transformer_selected.IsLoaded = true;
		_command_diagnose_offline_can_execute=false;
	}
	private void diagnoseOffline_execute(object parameter) 
	{
		_transformer_selected.invalidateParametersOutOfLimits();
		_transformer_selected.calculateScore();
	}
	private bool diagnoseOffline_canExecute(object parameter)
	{
		return _command_diagnose_offline_can_execute;
	}
	private bool openDocument_canExecute(object parameter)
	{
		return true;
	}
    private void openDocument_execute(object parameter)
    {
        OpenFileDialog open_file_dialog = new OpenFileDialog();
		open_file_dialog.Filter = "PDF Files (PDF)|*.PDF";
		if (open_file_dialog.ShowDialog() == true && Path.GetExtension(open_file_dialog.FileName) == ".pdf")
			Utility.Event_Aggregator.send(new Message_ViewModelSTD { document_path = open_file_dialog.FileName });
		else
			MessageBox.Show("Ошибка при открытии pdf файла");
    }
    private bool loadTransformer_canExecute(object parameter)
    {
        return true;
    }

	public void receive(ViewModelMainWindow.Message_Transformer_Initial message)
	{
		_transformer_selected = message.transformer;
		_command_diagnose_offline_can_execute=true;
	}
	public void receive(ViewModelServer.Message_Transformers message)
	{
		Transformers=message.transformers;
	}
	/*private void openTab_execute(object parameter)
{

}
private bool openTab_canExecute(object parameter)
{
return true;
}
private void closeTab_execute(object parameter)
{
Сделать с помощью Tab Visibility (см. KP)
}
private bool closeTab_canExecute(object parameter)
{
return true;
}*/
	#endregion
	#endregion
}
