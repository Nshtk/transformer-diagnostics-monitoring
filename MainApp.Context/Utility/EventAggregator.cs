using System.Collections.Generic;
using System.Linq;

namespace MainApp.Context.Utility;

#region Fields
#region WPF
#endregion
#endregion

#region Properties
#endregion

#region Methods
#endregion

public interface IListen
{
}
public interface IListen<T> : IListen
{
    void receive(T message);
}

public class EventAggregator
{
    private List<IListen> _subscribers = new List<IListen>();

    public void subscribe(IListen subscriber)
    {
        _subscribers.Add(subscriber);
    }

    public void unsubscribe(IListen subscriber)
    {
        _subscribers.Remove(subscriber);
    }

    public void send<T>(T message)
    {
        foreach (var subscriber in _subscribers.OfType<IListen<T>>())
            subscriber.receive(message);
    }
}
