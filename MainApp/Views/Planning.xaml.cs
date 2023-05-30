using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MainApp.Views;

public partial class ViewPlanning : UserControl
{
    public ViewPlanning()
    {
        InitializeComponent();
		calendar.GotMouseCapture+=Calendar_GotMouseCapture;
	}

	private void Calendar_GotMouseCapture(object sender, MouseEventArgs e)
	{
		UIElement original_element = e.OriginalSource as UIElement;
		if(original_element is CalendarDayButton || original_element is CalendarItem)
			original_element.ReleaseMouseCapture();
	}
}
