using MainApp.Models;
using MainApp.WPF.MVVM.ViewModel;
using MainApp.Context;
using MainApp.Context.Utility;
using MainApp.Views;
using Proto;
using System.Threading.Tasks;
using System.Threading;
using Proto.Monitoring;
using MainApp.Client.Clients.ClientServer;
using System.Windows;
using MainApp.Client.Clients.ClientClient;
using System.Collections.Generic;

namespace MainApp.ViewModels;

public class ViewModelOverview : ViewModelBase, IListen<ViewModelMainWindow.Message_Transformer>, IListen<ViewModelServer.Message_User>, IListen<ViewModelOverview.Message_Monitoring>, IListen<ViewModelMainWindow.Message_Transformer_Initial>
{
	public class Message_Monitoring
	{
		public bool monitoring_enabled;

		public Message_Monitoring(bool monitoring_enabled)
		{
			this.monitoring_enabled=monitoring_enabled;
		}
	}

	#region Fields
	private ModelTransformer _transformer;
	private ClientMonitoring _client_monitoring= new ClientMonitoring();
	private ClientEmail _client_email = new ClientEmail();
	private uint _instance_id;
	private string _instance_email;
	private bool _monitoring_enabled = false;
	private (string, InformationReply.Types.Message_Type) _server_reply_information_details;
	#region WPF
	private Visibility _tab_visibility = Visibility.Visible;
	#endregion
	#endregion

	#region Properties
	public ModelTransformer Transformer 
	{ 
		get { return _transformer; }
		set { _transformer = value; invokePropertiesChanged("OilQuality", "Windings", "Insulation", "Bushings", "HVR", "MagneticCore", "Cooling", "GeneralState"); }
	}

	#region WPF
	public Visibility Tab_Visibility
	{
		get { return _tab_visibility; }
		set { _tab_visibility=value; invokePropertyChanged("Tab_Visibility"); }
	}
	public ModelElement OilQuality
	{
		get { return Transformer.Elements[0]; }
	}
	public ModelElement Windings
	{
		get { return Transformer.Elements[1]; }
	}
	public ModelElement Insulation
	{
		get { return Transformer.Elements[2]; }
	}
	public ModelElement Bushings
	{
		get { return Transformer.Elements[3]; }
	}
	public ModelElement HVR
	{
		get { return Transformer.Elements[4]; }
	}
	public ModelElement MagneticCore
	{
		get { return Transformer.Elements[5]; }
	}
	public ModelElement Cooling
	{
		get { return Transformer.Elements[6]; }
	}
	public ModelElement GeneralState
	{
		get { return Transformer.Elements[7]; }
	}
	public (string, InformationReply.Types.Message_Type) ServerReplyInformationDetails
	{
		get { return _server_reply_information_details; }
		set { _server_reply_information_details=value; Utility.Event_Aggregator.send(new ViewServer.Message_Log(value.Item1, value.Item2)); }
	}
	#endregion
	#endregion

	public ViewModelOverview() 
	{
		Title = "Мониторинг";
		Utility.Event_Aggregator.subscribe(this);
	}

	#region Methods
	public async void getParametersAsync()
	{
		while(_monitoring_enabled)
		{
			GetParametersReply reply = await _client_monitoring.getParametersAsync(_instance_id);

			((ModelParameter<double>)OilQuality.Parameters[0]).Value=reply.OilConcentrationH2;
			((ModelParameter<double>)OilQuality.Parameters[1]).Value=reply.OilConcentrationCh4;
			((ModelParameter<double>)OilQuality.Parameters[2]).Value=reply.OilConcentrationC2H2;
			((ModelParameter<double>)OilQuality.Parameters[3]).Value=reply.OilConcentrationC2H4;
			((ModelParameter<double>)OilQuality.Parameters[4]).Value=reply.OilConcentrationC2H6;
			((ModelParameter<double>)OilQuality.Parameters[5]).Value=reply.OilConcentrationCo;
			((ModelParameter<double>)OilQuality.Parameters[6]).Value=reply.OilConcentrationCo2;
			((ModelParameter<double>)OilQuality.Parameters[7]).Value=reply.OilAcidNumber;
			((ModelParameter<double>)Windings.Parameters[0]).Value=reply.WindingsHumidity;
			((ModelParameter<double>)Insulation.Parameters[0]).Value=reply.InsulationFuranConcentration;
			((ModelParameter<int>)Insulation.Parameters[1]).Value=reply.InsulationTemperature;
			((ModelParameter<int>)Bushings.Parameters[0]).Value=reply.BushingsElectricalLossTangent;
			((ModelParameter<int>)HVR.Parameters[0]).Value=reply.HvrBreakVoltage;
			((ModelParameter<int>)MagneticCore.Parameters[0]).Value=reply.MagneticCoreIdleLoss;
			((ModelParameter<double>)MagneticCore.Parameters[1]).Value=reply.MagneticCorePartialDischarges;
			((ModelParameter<int>)MagneticCore.Parameters[2]).Value=reply.MagneticCoreVibration;
			((ModelParameter<int>)Cooling.Parameters[0]).Value=reply.CoolingSystemEfficiency;
			((ModelParameter<double>)GeneralState.Parameters[0]).Value=reply.CommonLifeTime;
			((ModelParameter<int>)GeneralState.Parameters[1]).Value=reply.CommonTemperature;
			Transformer.invalidateParametersOutOfLimits();
			Transformer.calculateScore();
			if(_instance_email!="Введите e-mail адрес" && Transformer.Score<50)
			{
				string report = $"<h2>Текущее состояние трансформатора: {Transformer.Score}/100.</h2>";
				string defects="";

				report+="<h3>Состояние элементов трансформатора:</h3><p>";
				foreach(ModelElement element in Transformer.Elements)
				{
					report+=$"{element.Name}: { element.Score}/100.<br>";
					foreach(KeyValuePair<int, Documentation.Defect> defect in element.Defects)
						defects+=defect.Value.Name+", ";
				}
				report+="</p>";
				if(defects!="")
				{
					report+="<h2>Обнаружены следующие серьезные проблемы:</h2>";
					report+=defects;
				}
					
				await _client_email.SendEmailAsync(_instance_email, "Отчет о состоянии трансформатора "+Transformer.Name, report);
			}
				
			Thread.Sleep(5000);
		}
	}
	public void receive(ViewModelMainWindow.Message_Transformer message)
	{
		Transformer=message.transformer;
	}
	public void receive(ViewModelServer.Message_User message)
	{
		_instance_id=message.id;
		_instance_email=message.email;
	}
	public void receive(Message_Monitoring message)
	{
		_monitoring_enabled=message.monitoring_enabled;
		if(message.monitoring_enabled==true)
			Task.Run(()=>getParametersAsync());
	}

	public void receive(ViewModelMainWindow.Message_Transformer_Initial message)
	{
		Transformer=message.transformer;
	}
	#endregion
}
