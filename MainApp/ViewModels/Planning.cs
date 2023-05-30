using MainApp.Client.Clients.ClientServer;
using MainApp.Context.Utility;
using MainApp.Models;
using MainApp.Views;
using MainApp.WPF.MVVM.ViewModel;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using OxyPlot.Wpf;
using Proto.Monitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MainApp.ViewModels;

class ViewModelPlanning : ViewModelBase, IListen<ViewModelMainWindow.Message_Transformer>, IListen<ViewModelServer.Message_User>
{
	#region Fields
	private ClientMonitoring _client_monitoring = new ClientMonitoring();
	private getParameterValuesPastReply _reply;
	private ModelTransformer _transformer;
	private uint _instance_id;
	#region WPF
	private Visibility _tab_visibility = Visibility.Visible;
	private DateTime _date_selected;
	private HashSet<DateTime> _dates_reserved = new HashSet<DateTime>();
	private DateTime _display_date = DateTime.Now;
	private PlotModel _plot_model =new PlotModel();
	private RelayCommandAsync _command_date_evaluate_state;
	private RelayCommandAsync _command_plan_maintenance;
	private bool _command_date_evaluate_state_can_execute=false;
	private bool _command_date_plan_maintenance_can_execute=false;
	#endregion
	#endregion

	#region Properties
	#region WPF
	public Visibility Tab_Visibility
	{
		get { return _tab_visibility; }
		set { _tab_visibility=value; invokePropertyChanged("Tab_Visibility"); }
	}
	public DateTime DateSelected
	{
		get { return _date_selected; }
		set 
		{
			if(value!=_date_selected)
			{
				_date_selected=value;
				CommandDateEvaluateState_CanExecute=true;
				if(!_dates_reserved.Contains(_date_selected))
					CommandDatePlanMaintenance_CanExecute=true;
			}
		}
	}
	public HashSet<DateTime> DatesReserved
	{
		get { return _dates_reserved; }
	}
	public DateTime DisplayDate
	{
		set { _display_date=value; invokePropertyChanged("DisplayDate"); }
		get { return _display_date; }
	}
	public PlotModel PlotModel
	{
		get { return _plot_model; }
		set { _plot_model=value; invokePropertyChanged("PlotModel"); }
	}
	public RelayCommandAsync CommandDateEvaluateState
	{
		get { return _command_date_evaluate_state ??= new RelayCommandAsync(dateEvaluateState_execute, dateEvaluateState_canExecute, (ex) => { return; }); }
	}
	public RelayCommandAsync CommandDatePlanMaintenance
	{
		get { return _command_plan_maintenance ??= new RelayCommandAsync(datePlanMaintenance_execute, datePlanMaintenance_canExecute, (ex) => { return; }); }
	}
	public bool CommandDateEvaluateState_CanExecute
	{
		get { return _command_date_evaluate_state_can_execute; }
		set { _command_date_evaluate_state_can_execute=value; invokePropertyChanged("CommandDateEvaluateState_CanExecute"); }
	}
	public bool CommandDatePlanMaintenance_CanExecute
	{
		get { return _command_date_plan_maintenance_can_execute; }
		set { _command_date_plan_maintenance_can_execute=value; invokePropertyChanged("CommandDatePlanMaintenance_CanExecute"); }
	}
	#endregion
	#endregion


	public ViewModelPlanning()
	{
		Title = "Планирование";
		DateSelected= DateTime.Today;
	}

	#region Methods
	private async Task dateEvaluateState_execute(object parameter)
	{
		_reply=await _client_monitoring.getParameterValuesPastAsync(_instance_id);
		Utility.Event_Aggregator.send(new ViewServer.Message_Log(_reply.Information.Details, _reply.Information.Type));
		if(_reply.Information.Result==false)
			return;

		int i = 0;
		((ModelParameter<double>)_transformer.Elements[0].Parameters[0]).values_per_date=new (int, double)[_reply.ValuesPastOilConcentrationH2.Count];
		((ModelParameter<double>)_transformer.Elements[0].Parameters[1]).values_per_date=new (int, double)[_reply.ValuesPastOilConcentrationCh4.Count];
		((ModelParameter<double>)_transformer.Elements[0].Parameters[2]).values_per_date=new (int, double)[_reply.ValuesPastOilConcentrationC2H2.Count];
		((ModelParameter<double>)_transformer.Elements[0].Parameters[3]).values_per_date=new (int, double)[_reply.ValuesPastOilConcentrationC2H4.Count];
		((ModelParameter<double>)_transformer.Elements[0].Parameters[4]).values_per_date=new (int, double)[_reply.ValuesPastOilConcentrationC2H6.Count];
		((ModelParameter<double>)_transformer.Elements[0].Parameters[5]).values_per_date=new (int, double)[_reply.ValuesPastOilConcentrationCo.Count];
		((ModelParameter<double>)_transformer.Elements[0].Parameters[6]).values_per_date=new (int, double)[_reply.ValuesPastOilConcentrationCo2.Count];
		((ModelParameter<double>)_transformer.Elements[0].Parameters[7]).values_per_date=new (int, double)[_reply.ValuesPastOilAcidNumber.Count];
		((ModelParameter<double>)_transformer.Elements[1].Parameters[0]).values_per_date=new (int, double)[_reply.ValuesPastWindingsHumidity.Count];
		((ModelParameter<double>)_transformer.Elements[2].Parameters[0]).values_per_date=new (int, double)[_reply.ValuesPastInsulationFuranConcentration.Count];
		((ModelParameter<int>)_transformer.Elements[2].Parameters[1]).values_per_date=new (int, int)[_reply.ValuesPastInsulationTemperature.Count];
		((ModelParameter<int>)_transformer.Elements[3].Parameters[0]).values_per_date=new (int, int)[_reply.ValuesPastBushingsElectricalLossTangent.Count];
		((ModelParameter<int>)_transformer.Elements[4].Parameters[0]).values_per_date=new (int, int)[_reply.ValuesPastHvrBreakVoltage.Count];
		((ModelParameter<int>)_transformer.Elements[5].Parameters[0]).values_per_date=new (int, int)[_reply.ValuesPastMagneticCoreIdleLoss.Count];
		((ModelParameter<double>)_transformer.Elements[5].Parameters[1]).values_per_date=new (int, double)[_reply.ValuesPastMagneticCorePartialDischarges.Count];
		((ModelParameter<int>)_transformer.Elements[5].Parameters[2]).values_per_date=new (int, int)[_reply.ValuesPastMagneticCoreVibration.Count];
		((ModelParameter<int>)_transformer.Elements[6].Parameters[0]).values_per_date=new (int, int)[_reply.ValuesPastCoolingSystemEfficiency.Count];
		((ModelParameter<double>)_transformer.Elements[7].Parameters[0]).values_per_date=new (int, double)[_reply.ValuesPastCommonLifeTime.Count];
		((ModelParameter<int>)_transformer.Elements[7].Parameters[1]).values_per_date=new (int, int)[_reply.ValuesPastCommonTemperature.Count];
		foreach(var parameter_value_data in _reply.ValuesPastOilConcentrationH2)
			((ModelParameter<double>)_transformer.Elements[0].Parameters[0]).values_per_date[i++] = (parameter_value_data.Date, parameter_value_data.Value);
		i = 0;
		foreach(var parameter_value_data in _reply.ValuesPastOilConcentrationCh4)
			((ModelParameter<double>)_transformer.Elements[0].Parameters[1]).values_per_date[i++] = (parameter_value_data.Date, parameter_value_data.Value);
		i = 0;
		foreach(var parameter_value_data in _reply.ValuesPastOilConcentrationC2H2)
			((ModelParameter<double>)_transformer.Elements[0].Parameters[2]).values_per_date[i++] = (parameter_value_data.Date, parameter_value_data.Value);
		i = 0;
		foreach(var parameter_value_data in _reply.ValuesPastOilConcentrationC2H4)
			((ModelParameter<double>)_transformer.Elements[0].Parameters[3]).values_per_date[i++] = (parameter_value_data.Date, parameter_value_data.Value);
		i = 0;
		foreach(var parameter_value_data in _reply.ValuesPastOilConcentrationC2H6)
			((ModelParameter<double>)_transformer.Elements[0].Parameters[4]).values_per_date[i++] = (parameter_value_data.Date, parameter_value_data.Value);
		i = 0;
		foreach(var parameter_value_data in _reply.ValuesPastOilConcentrationCo)
			((ModelParameter<double>)_transformer.Elements[0].Parameters[5]).values_per_date[i++] = (parameter_value_data.Date, parameter_value_data.Value);
		i = 0;
		foreach(var parameter_value_data in _reply.ValuesPastOilConcentrationCo2)
			((ModelParameter<double>)_transformer.Elements[0].Parameters[6]).values_per_date[i++] = (parameter_value_data.Date, parameter_value_data.Value);
		i = 0;
		foreach(var parameter_value_data in _reply.ValuesPastOilAcidNumber)
			((ModelParameter<double>)_transformer.Elements[0].Parameters[7]).values_per_date[i++] = (parameter_value_data.Date, parameter_value_data.Value);
		i = 0;
		foreach(var parameter_value_data in _reply.ValuesPastWindingsHumidity)
			((ModelParameter<double>)_transformer.Elements[1].Parameters[0]).values_per_date[i++] = (parameter_value_data.Date, parameter_value_data.Value);
		i = 0;
		foreach(var parameter_value_data in _reply.ValuesPastInsulationFuranConcentration)
			((ModelParameter<double>)_transformer.Elements[2].Parameters[0]).values_per_date[i++] = (parameter_value_data.Date, parameter_value_data.Value);
		i = 0;
		foreach(var parameter_value_data in _reply.ValuesPastInsulationTemperature)
			((ModelParameter<int>)_transformer.Elements[2].Parameters[1]).values_per_date[i++] = (parameter_value_data.Date, (int)parameter_value_data.Value);
		i = 0;
		foreach(var parameter_value_data in _reply.ValuesPastBushingsElectricalLossTangent)
			((ModelParameter<int>)_transformer.Elements[3].Parameters[0]).values_per_date[i++] = (parameter_value_data.Date, (int)parameter_value_data.Value);
		i = 0;
		foreach(var parameter_value_data in _reply.ValuesPastHvrBreakVoltage)
			((ModelParameter<int>)_transformer.Elements[4].Parameters[0]).values_per_date[i++] = (parameter_value_data.Date, (int)parameter_value_data.Value);
		i = 0;
		foreach(var parameter_value_data in _reply.ValuesPastMagneticCoreIdleLoss)
			((ModelParameter<int>)_transformer.Elements[5].Parameters[0]).values_per_date[i++] = (parameter_value_data.Date, (int)parameter_value_data.Value);
		i = 0;
		foreach(var parameter_value_data in _reply.ValuesPastMagneticCorePartialDischarges)
			((ModelParameter<double>)_transformer.Elements[5].Parameters[1]).values_per_date[i++] = (parameter_value_data.Date, parameter_value_data.Value);
		i = 0;
		foreach(var parameter_value_data in _reply.ValuesPastMagneticCoreVibration)
			((ModelParameter<int>)_transformer.Elements[5].Parameters[2]).values_per_date[i++] = (parameter_value_data.Date, (int)parameter_value_data.Value);
		i = 0;
		foreach(var parameter_value_data in _reply.ValuesPastCoolingSystemEfficiency)
			((ModelParameter<int>)_transformer.Elements[6].Parameters[0]).values_per_date[i++] = (parameter_value_data.Date, (int)parameter_value_data.Value);
		i = 0;
		foreach(var parameter_value_data in _reply.ValuesPastCommonLifeTime)
			((ModelParameter<double>)_transformer.Elements[7].Parameters[0]).values_per_date[i++] = (parameter_value_data.Date, parameter_value_data.Value);
		i = 0;
		foreach(var parameter_value_data in _reply.ValuesPastCommonTemperature)
			((ModelParameter<int>)_transformer.Elements[7].Parameters[1]).values_per_date[i++] = (parameter_value_data.Date, (int)parameter_value_data.Value);

		PlotModel plot=new PlotModel();
		List<IUnit> parameters_minimal_score = new List<IUnit>(5);
		//List<IUnit> parameters_minimal_score = new List<IUnit>(5) { new ModelParameter<double>() };  // Раскомментировать для возможности выбора других параметров
		//parameters_minimal_score[0].Score=4;

		foreach(var element in _transformer.Elements)
		{
			//if(element.Score>0)
			//{
				foreach(var parameter_element in element.Parameters)
				{
					if(parameter_element.Name=="Концентрация H2" || parameter_element.Name=="Концентрация CH4" || parameter_element.Name=="Концентрация C2H4") // Закомментировать для возможности выбора других параметров
						parameters_minimal_score.Add(parameter_element);
					continue;
					for(int j = 0; j<parameters_minimal_score.Count; j++)
					{
						if(parameter_element.Score<(parameters_minimal_score[j].Score+Utility.Random.Next(Math.Abs(3-parameters_minimal_score[j].Score))) && parameter_element.Score>0)
						{
							if(parameters_minimal_score.Count==parameters_minimal_score.Capacity || parameters_minimal_score[0].Name==null)
								parameters_minimal_score[j]=parameter_element;
							else
							{
								if(parameters_minimal_score.ElementAtOrDefault(j+1)==null)
									parameters_minimal_score.Add(parameter_element);
								else
									parameters_minimal_score[j+1]=parameter_element;
							}
							break;
						}
					}
				}
			//}
		}

		DateTime date_start = DateTime.Now.AddDays(-(_reply.ValuesPastOilConcentrationH2.Count));
		var fields = typeof(OxyColors).GetFields();
		foreach(var parameter_element in parameters_minimal_score)
		{
			LineSeries series = new LineSeries() { Color=(OxyColor)fields[Utility.Random.Next(0, fields.Length-1)].GetValue(null) };
			LineSeries series_end = new LineSeries() { Color=series.Color, MarkerType=MarkerType.Circle, MarkerFill=OxyColors.Red };

			series.Title=parameter_element.Name;
			series.RenderInLegend=true;
			series.InterpolationAlgorithm=InterpolationAlgorithms.CanonicalSpline;

			dynamic parameter_element_casted;
			if(parameter_element is ModelParameter<double>)
				parameter_element_casted = (ModelParameter<double>)parameter_element;
			else
				parameter_element_casted = (ModelParameter<int>)parameter_element;
			var prediction = parameter_element_casted.predictState();
			for(i = 0; i<parameter_element_casted.values_per_date.Length; i++)
				series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(date_start.AddDays(parameter_element_casted.values_per_date[i].Item1)), parameter_element_casted.values_per_date[i].Item2));

			series_end.Points.Add(new DataPoint(DateTimeAxis.ToDouble(date_start.AddDays(parameter_element_casted.values_per_date[parameter_element_casted.values_per_date.Length-1].Item1)), parameter_element_casted.values_per_date[parameter_element_casted.values_per_date.Length-1].Item2));
			series_end.Points.Add(new DataPoint(DateTimeAxis.ToDouble(date_start.AddDays((double)prediction.Item1)), (double)prediction.Item2));
			plot.Series.Add(series);
			plot.Series.Add(series_end);
		}
		plot.Axes.Add(new DateTimeAxis {
			Title="Дата",
			TitleFontWeight = OxyPlot.FontWeights.Bold,
			Position = AxisPosition.Bottom,
			MajorGridlineStyle = LineStyle.Dot,
			ExtraGridlineColor = OxyColors.Black,
			AxislineColor = OxyColors.Transparent,
			TicklineColor = OxyColors.Transparent,
			StringFormat = "dd.MM.yyyy",
			IntervalLength = 75,
			IntervalType = DateTimeIntervalType.Days
		});
		plot.Axes.Add(new LinearAxis {
			Title="Значение",
			TitleFontWeight = OxyPlot.FontWeights.Bold,
			Position = AxisPosition.Left,
			MajorGridlineStyle = LineStyle.Automatic,
			ExtraGridlineColor = OxyColors.Black,
			AxislineColor =OxyColors.Transparent,
			TicklineColor = OxyColors.Transparent
		});
		plot.Title="Состояние параметров";
		plot.Legends.Add(new Legend() {
			 LegendTitle = "Параметры",
			 LegendPosition = LegendPosition.RightTop,
			 LegendBackground=OxyColor.FromArgb(150, 229, 250, 255),
		});
		PlotModel=plot;
		CommandDateEvaluateState_CanExecute=false;
	}
	private bool dateEvaluateState_canExecute(object parameter)
	{
		return _transformer!=null;
	}
	private async Task datePlanMaintenance_execute(object parameter)
	{
		if(DatesReserved.Contains(_date_selected))
			DatesReserved.Remove(_date_selected);
		else
			DatesReserved.Add(_date_selected);
		DisplayDate=DateTime.MinValue;
		DisplayDate=DateSelected;
		CommandDatePlanMaintenance_CanExecute=false;
	}
	private bool datePlanMaintenance_canExecute(object parameter)
	{
		return _transformer!=null;
	}

	void IListen<ViewModelMainWindow.Message_Transformer>.receive(ViewModelMainWindow.Message_Transformer message)
	{
		_transformer=message.transformer;
	}

	public void receive(ViewModelServer.Message_User message)
	{
		_instance_id=message.id;
	}
	#endregion
}

