using System.Windows;
using System.Windows.Input;
using MainApp.ViewModels;

namespace MainApp.Views;

public partial class ViewMainWindow : Window
{
    public ViewMainWindow()
    {
        InitializeComponent();
		DataContext = new ViewModelMainWindow();
	}
}
