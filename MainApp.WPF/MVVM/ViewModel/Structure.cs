using System.ComponentModel;

namespace MainApp.WPF.MVVM.ViewModel;
public abstract class ModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected void invokePropertyChanged(string property)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }

    protected void invokePropertiesChanged(params string[] properties)
    {
        foreach (var property in properties)
            invokePropertyChanged(property);
    }
}

public abstract class ViewModelBase : ModelBase
{
    public string Title
    {
        get; protected set;
    }
}

