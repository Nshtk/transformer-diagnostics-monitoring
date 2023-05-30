using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;

namespace MainApp.WPF.MVVM.View;

public abstract class ValueConverterBase<TConverter> : MarkupExtension, IValueConverter where TConverter : class, new()
{
    private static TConverter _ValueToProvide;

    protected virtual bool Check(in object value, in object parameter)
    {
        if (value == DependencyProperty.UnsetValue)
			return false;
		return true;
    }
    public virtual object Convert(object value, Type target_type, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public virtual object ConvertBack(object value, Type target_type, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public override sealed object ProvideValue(IServiceProvider ServiceProvider)
    {
        return _ValueToProvide ??= new TConverter();
    }
}
public abstract class MultiValueConverterBase<TMultiConverter> : MarkupExtension, IMultiValueConverter where TMultiConverter : class, new()
{
    private static TMultiConverter _ValueToProvide;

    protected virtual bool Check(in object[] values, in object parameter)
    {
        for(int i=0; i<values.Length; i++)
            if(values[i] == DependencyProperty.UnsetValue)
                return false;
        return true;
    }
    public virtual object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
    public virtual object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return _ValueToProvide ??= new TMultiConverter();
    }
}
/*
public class EnumConverter : ValueConverterBase<EnumConverter>
{
    protected override bool Check(in object value, in object parameter)
    {
        if (!(value is Enum))
            throw new ArgumentException("value is not of type Enum", nameof(value));
        return true;
    }
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!Check(value, parameter))
            return DependencyProperty.UnsetValue;

        return (int)value == 0;
    }
}
public class ConverterEnumBool : ValueConverterBase<ConverterEnumBool>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.Equals(parameter);
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.Equals(true) == true ? parameter : Binding.DoNothing;
    }
}
public class StyleConverter : MultiValueConverterBase<StyleConverter>
{
    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        FrameworkElement target_element = values[0] as FrameworkElement;
        string style_name = values[1] as string;

        if (style_name == null)
            return null;

        Style new_style = (Style)target_element.TryFindResource(style_name);

        if (new_style == null)
            new_style = (Style)target_element.TryFindResource("MyDefaultStyleName");

        return new_style;
    }

    public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}*/
public class ConverterScoreBrush : ValueConverterBase<ConverterScoreBrush>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int score = (int)value;

        if (score > 84)
            return Brushes.Green;
        if (score > 59)
            return Brushes.LightGreen;
        if (score > 39)
            return Brushes.Gold;
        if (score > 24)
            return Brushes.Orange;
        return Brushes.Red;
    }
}
public class ConverterScoreString : ValueConverterBase<ConverterScoreString>
{
    protected override bool Check(in object value, in object parameter)
    {
        if (!(value is int))
            throw new ArgumentException("value is not of type int", nameof(value));
        return true;
    }
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int score = (int)value;

        if (score > 84)
            return "Отличное";
        if (score > 59)
            return "Хорошее";
        if (score > 39)
            return "Удовлетворительное";
        if (score > 24)
            return "Неудовлетворительное";
        return "Предельное";
    }
}
public class ConverterWidth : ValueConverterBase<ConverterWidth>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (double)value - 29;
    }
}
public class ConverterDefects : ValueConverterBase<ConverterDefects>
{
	public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if(!Check(value, parameter))
			return false;

		dynamic dictionary = value;
		foreach(dynamic item in dictionary)
			return (bool)item.Value.GetType().GetProperty("IsActive").GetValue(item.Value, null)==true ? value : DependencyProperty.UnsetValue;

		return DependencyProperty.UnsetValue;
	}
}

public class ConverterCalendar : MultiValueConverterBase<ConverterCalendar>
{
	protected override bool Check(in object[] values, in object parameter)
	{
		if(values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue || values[0] == null || values[1] == null)
			return false;

		return true;
	}
	public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		if(!Check(values, parameter))
			return false;
		DateTime date_selected = (DateTime)values[0];
		HashSet<DateTime> dates_reserved = (HashSet<DateTime>)values[1];

		return dates_reserved.Contains(date_selected);
	}
}
