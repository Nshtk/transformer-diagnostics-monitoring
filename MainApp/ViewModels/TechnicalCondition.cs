using System.Collections.ObjectModel;
using MainApp.Models;
using MainApp.WPF.MVVM.ViewModel;
using MainApp.Context;
using MainApp.Context.Utility;
using System.Windows;

namespace MainApp.ViewModels;

public class ViewModelTechnicalCondition : ViewModelBase, IListen<ViewModelMainWindow.Message_Transformer>
{
	#region Fields
	private ModelTransformer _transformer = new ModelTransformer(new Documentation("Трансформатор не выбран", ""));
	#region WPF
	private RelayCommand _command_show_recommendations;
	private Visibility _tab_visibility = Visibility.Visible;
	#endregion
	#endregion

	#region Properties
	public ModelTransformer Transformer
	{
		get { return _transformer; }
		set { _transformer = value; invokePropertiesChanged("Transformer", "Transformer.Score", "Elements"); }
	}
	public ObservableCollection<ModelElement> Elements
	{
		get { return Transformer.Elements; }
	}

	#region WPF
	public RelayCommand CommandShowRecommendations
	{
		get { return _command_show_recommendations ??= new RelayCommand(showRecommendations_execute, showRecommendations_canExecute); }
	}
	public Visibility Tab_Visibility
	{
		get { return _tab_visibility; }
		set { _tab_visibility=value; invokePropertyChanged("Tab_Visibility"); }
	}
	#endregion
	#endregion

	public ViewModelTechnicalCondition()
	{
		Title = "Диагностика";
		Utility.Event_Aggregator.send(new ViewModelMainWindow.Message_Transformer_Initial {transformer=_transformer });
	}

	#region Methods
	private void showRecommendations_execute(object parameter)
	{
		string recommendation;

		if(_transformer.Score>84)
			recommendation="Плановое диагностирование.";
		else if(_transformer.Score>59)
			recommendation="Ремонт по результатам планового диагностирования.";
		else if(_transformer.Score>39)
			recommendation="Усиленный контроль технического состояния, дополнительное техническое обслуживание и ремонт.";
		else if(_transformer.Score>24)
			recommendation = "Усиленный контроль технического состояния капитальный ремонт, реконструкция.";
		else
			recommendation="Вывод из эксплуатации.";

		MessageBox.Show(recommendation, "Рекомендация", MessageBoxButton.OK, MessageBoxImage.Information);
	}
	private bool showRecommendations_canExecute(object parameter)
	{
		return Transformer.HasActiveDefects;
	}
	public void receive(ViewModelMainWindow.Message_Transformer message)
	{
		Transformer = message.transformer;
	}
	#endregion
}
