using Proto.Connection;
using System.Threading.Tasks;
using System.Windows;
using MainApp.WPF.MVVM.ViewModel;
using MainApp.Context.Utility;
using MainApp.Views;
using Proto.Common;
using Proto;
using MainApp.Client.Clients.ClientServer;
using MainApp.Models;
using System.Collections.ObjectModel;
using MainApp.Context;
using Proto.Monitoring;
using System.ComponentModel;
using System.Windows.Controls;
using System;
using System.Text.RegularExpressions;

namespace MainApp.ViewModels;

public class ViewModelServer : ViewModelBase, IListen<ViewModelMainWindow.Message_Transformer>, IDataErrorInfo
{
	public class Message_User
	{
		public uint id;
		public string email;

		public Message_User(uint id, string email)
		{
			this.id=id;
			this.email=email;
		}
	}
	public class Message_Transformers
	{
		public ObservableCollection<ModelTransformer> transformers;

		public Message_Transformers(ObservableCollection<ModelTransformer> transformers)
		{
			this.transformers=transformers;
		}
	}
	public class SliderData
	{
		private double _value;
		public string ElementName
		{
			get;
			set;
		}
		public string ParameterName
		{
			get;
			set;
		}
		public double Value
		{
			get { return _value; }
			set { _value=value; }
		}
	}

	#region Fields
	private ClientConnection _client_connection = new ClientConnection();
	private ClientMonitoring _client_monitoring = new ClientMonitoring();
	private ClientCommon _client_common = new ClientCommon();
	private uint _instance_id;
	private (string, InformationReply.Types.Message_Type) _server_reply_information_details;
	#region WPF
	private ModelTransformer _transformer;
	private bool _authorize_user_can_execute = true;
	private bool _enable_monitoring_can_execute = false;
	private bool _send_edited_parameters_can_execute = false;
	private ObservableCollection<SliderData> _sliders;
	private string _user_email="Введите e-mail адрес";
	private string _error=string.Empty;
	private Visibility _textbox_email_visibility = Visibility.Collapsed;
	private Visibility _tab_visibility = Visibility.Visible;
	private Visibility _stackpanel_authorize_user_visibility = Visibility.Visible;
	private Visibility _controls_server_visibility = Visibility.Collapsed;
	private Visibility _controls_edit_parameters_visibility = Visibility.Collapsed;
	private RelayCommand _command_enter_email;
	private RelayCommandAsync _command_authorize_user;
	private RelayCommandAsync _command_enable_monitoring;
	private RelayCommandAsync _command_send_edited_parameters;
	#endregion
	#endregion

	#region Properties
	public (string, InformationReply.Types.Message_Type) ServerReplyInformationDetails
	{
		get { return _server_reply_information_details; }
		set { _server_reply_information_details=value; Utility.Event_Aggregator.send(new ViewServer.Message_Log(value.Item1, value.Item2)); }
	}
	#region WPF
	public ModelTransformer Transformer
	{
		get { return _transformer; }
		set { _transformer=value; invokePropertyChanged("Transformer"); }
	}
	public ObservableCollection<SliderData> Sliders
	{
		get { return _sliders; }
		set { _sliders=value; invokePropertyChanged("Sliders"); }
	}
	public string UserEmail
	{
		get { return _user_email; }
		set { _user_email=value; invokePropertyChanged("UserEmail"); }
	}
	public Visibility TextBox_Email_Visibility
	{
		get { return _textbox_email_visibility; }
		set { _textbox_email_visibility=value; invokePropertyChanged("TextBox_Email_Visibility"); }
	}
	public Visibility StackPanel_AuthorizeUser_Visibility
	{
		get { return _stackpanel_authorize_user_visibility; }
		set { _stackpanel_authorize_user_visibility=value; invokePropertyChanged("StackPanel_AuthorizeUser_Visibility"); }
	}
	public Visibility Controls_Server_Visibility
	{
		get { return _controls_server_visibility; }
		set { _controls_server_visibility=value; invokePropertyChanged("Controls_Server_Visibility"); }
	}
	public Visibility Controls_Edit_Parametrs_Visibility
	{
		get { return _controls_edit_parameters_visibility; }
		set { _controls_edit_parameters_visibility=value; invokePropertyChanged("Controls_Edit_Parametrs_Visibility"); }
	}
	public Visibility Tab_Visibility
	{
		get { return _tab_visibility; }
		set { _tab_visibility=value; invokePropertyChanged("Tab_Visibility"); }
	}
	public RelayCommand CommandEnterEmail
	{
		get { return _command_enter_email ??= new RelayCommand(enterEmail_execute, enterEmail_canExecute); }
	}
	public RelayCommandAsync CommandAuthorizeUser
	{
		get { return _command_authorize_user ??= new RelayCommandAsync(authorizeUser_execute, authorizeUser_canExecute, (ex) => { return; }); }
	}
	public RelayCommandAsync CommandEnableMonitoring
	{
		get { return _command_enable_monitoring ??= new RelayCommandAsync(enableMonitoring_execute, enableMonitoring_canExecute, (ex) => { return; }); }
	}
	public RelayCommandAsync CommandSendEditedParameters
	{
		get { return _command_send_edited_parameters ??= new RelayCommandAsync(sendEditedParameters_execute, sendEditedParameters_canExecute, (ex) => { return; }); }
	}
	public bool AuthorizeUserCanExecute
	{
		get { return _authorize_user_can_execute; }
		set { _authorize_user_can_execute=value; invokePropertyChanged("AuthorizeUserCanExecute"); }
	}
	public bool EnableMonitoringCanExecute
	{
		get { return _enable_monitoring_can_execute; }
		set { _enable_monitoring_can_execute=value; invokePropertyChanged("EnableMonitoringCanExecute"); }
	}
	public bool SendEditedParametersCanExecute
	{
		get { return _send_edited_parameters_can_execute; }
		set { _send_edited_parameters_can_execute=value; invokePropertyChanged("SendEditedParametersCanExecute"); }
	}

	public string this[string columnName]
	{
		get
		{
			string error = string.Empty;
			switch(columnName)
			{
				case "UserEmail":
					Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
					Match match = regex.Match(UserEmail);
					if(!match.Success)
						error="E-mail адрес не распознан.";
					break;
			}
			Error=error;
			return error;
		}
	}
	public string Error
	{
		get { return _error; }
		set { _error=value; invokePropertyChanged("Error");}
	}
	#endregion
	#endregion

	public ViewModelServer()
	{
		Title = "Сервер";
	}

	#region Methods
	private async void getTransformersData()
	{
		TransformersDataReply reply = await _client_common.getTransformersDataAsync(_instance_id);
		ObservableCollection<ModelTransformer> transformers = new ObservableCollection<ModelTransformer>();
		
		foreach(TransformerData transformer_data in reply.TransformersData)
			transformers.Add(new ModelTransformer(new Documentation(transformer_data.Location+transformer_data.Id.ToString(), "")));

		EnableMonitoringCanExecute=true;
		ServerReplyInformationDetails=(reply.Information.Details, reply.Information.Type);
		Utility.Event_Aggregator.send(new Message_Transformers(transformers));
	}

	private void enterEmail_execute(object parameter)
	{
		if((bool)parameter)
			TextBox_Email_Visibility=Visibility.Visible;
		else
			TextBox_Email_Visibility=Visibility.Collapsed;
	}
	private bool enterEmail_canExecute(object parameter)
	{
		return true;
	}
	private async Task authorizeUser_execute(object parameter)
	{
		AuthorizeUserReply reply=await _client_connection.authorizeUserAsync((string)parameter);

		ServerReplyInformationDetails=(reply.Information.Details, reply.Information.Type);
		if(reply.Information.Result==false)
			return;

		_instance_id=reply.Id;
		AuthorizeUserCanExecute=false;
		StackPanel_AuthorizeUser_Visibility=Visibility.Collapsed;
		Controls_Server_Visibility=Visibility.Visible;
		Utility.Event_Aggregator.send(new Message_User(reply.Id, UserEmail));
		getTransformersData();
	}
	private bool authorizeUser_canExecute(object parameter)
	{
		return _authorize_user_can_execute;
	}
	private async Task enableMonitoring_execute(object parameter)
	{
		if(_transformer==null)
		{
			MessageBox.Show("Выберите доступный трансформатор через меню Файл.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			return;
		}
			
		EnableMonitoringReply reply = await _client_common.enableMonitoringAsync(_instance_id);

		ServerReplyInformationDetails=(reply.Information.Details, reply.Information.Type);
		if(reply.Information.Result==false)
			return;

		SendEditedParametersCanExecute=!reply.IsEnabled;
		Utility.Event_Aggregator.send(new ViewModelOverview.Message_Monitoring(reply.IsEnabled));
	}
	private bool enableMonitoring_canExecute(object parameter)
	{
		return _enable_monitoring_can_execute;
	}
	private async Task sendEditedParameters_execute(object parameter)
	{
		SendEditedParametersReply reply = await _client_monitoring.sendEditedParametersAsync(_instance_id, new SendEditedParametersRequest());

		ServerReplyInformationDetails=(reply.Information.Details, reply.Information.Type);
		if(reply.Information.Result==false)
			return;

		
	}
	private bool sendEditedParameters_canExecute(object parameter)
	{
		return _send_edited_parameters_can_execute;
	}
	public void receive(ViewModelMainWindow.Message_Transformer message)
	{
		ObservableCollection<SliderData> sliders=new ObservableCollection<SliderData>();

		Transformer=message.transformer;
		Controls_Edit_Parametrs_Visibility=Visibility.Visible;

		foreach(var element in _transformer.Elements)
		{
			foreach(var parameter in  element.Parameters)
			{
				if((bool)parameter.GetType().GetProperty("IsEditable").GetValue(parameter, null)==true)
					sliders.Add(new SliderData { ElementName=element.Name+":", ParameterName=parameter.Name, Value=0 });
			}
		}
		Sliders=sliders;
	}
	#endregion
}
