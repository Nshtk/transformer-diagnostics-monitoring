using MainApp.Context.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Proto;
using System.Windows.Shapes;

namespace MainApp.Views;

public partial class ViewServer : UserControl, IListen<ViewServer.Message_Log>
{
	public class Message_Log
	{
		public string text;
		public int font_size;
		public FontStyle font_style;
		public SolidColorBrush color_foreground;
		public SolidColorBrush color_background;

		public Message_Log(string text, InformationReply.Types.Message_Type type)
		{
			this.text=text;
			switch(type)
			{
				case InformationReply.Types.Message_Type.Ordinary:
					font_size=15;
					font_style=FontStyles.Normal;
					color_foreground =Brushes.Black;
					color_background=Brushes.Transparent;
					break;
				case InformationReply.Types.Message_Type.Warning:
					font_size=17;
					font_style=FontStyles.Italic;
					color_foreground=Brushes.DarkOrange;
					color_background=Brushes.Transparent;
					break;
				case InformationReply.Types.Message_Type.Critical:
					font_size=20;
					font_style=FontStyles.Oblique;
					color_foreground=Brushes.White;
					color_background=Brushes.DarkRed;
					break;
			}
		}
	}

	private int _lists_runs_pointer=0;
	private int _runs_pointer=0;
	private List<List<Run>> _lists_runs = new List<List<Run>>();
	private Paragraph _paragraph;
	public ViewServer()
	{
		InitializeComponent();
		Utility.Event_Aggregator.subscribe(this);
		_paragraph = FlowDocument_Log.Document.Blocks.OfType<Paragraph>().First();
		listsRuns_add();
		TextBox_Email.GotFocus+=TextBox_Email_GotFocus;
		TextBox_Email.LostFocus+=TextBox_Email_LostFocus;
	}
	private void TextBox_Email_GotFocus(object sender, RoutedEventArgs e)
	{
		TextBox_Email.Text=string.Empty;
		TextBox_Email.Foreground=Brushes.Black;
	}
	private void TextBox_Email_LostFocus(object sender, RoutedEventArgs e)
	{
		if(TextBox_Email.Text==string.Empty)
		{
			TextBox_Email.Foreground=Brushes.Gray;
			TextBox_Email.Text="Введите e-mail адрес";
		}
	}

	private void listsRuns_add()
	{
		_lists_runs.Add(new List<Run>(new Run[255]));
		for(int i = 0; i<255; i++)
		{
			_lists_runs[_lists_runs_pointer][i]=new Run();
			_paragraph.Inlines.Add(_lists_runs[_lists_runs_pointer][i]);
		}
		_lists_runs_pointer++;
	}
	public void receive(Message_Log message)
	{
		List<Run> runs = _lists_runs.Last();
		runs[_runs_pointer].Text = message.text+"\u2028";
		runs[_runs_pointer].FontSize=message.font_size;
		runs[_runs_pointer].FontStyle=message.font_style;
		runs[_runs_pointer].Foreground=message.color_foreground;
		runs[_runs_pointer].Background=message.color_background;
		_runs_pointer++;
		if(_runs_pointer==255)
			listsRuns_add();
	}
}
