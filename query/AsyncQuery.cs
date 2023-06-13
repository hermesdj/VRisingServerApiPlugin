#nullable enable
using System;
using VRisingServerApiPlugin.http;

namespace VRisingServerApiPlugin.query;

public interface Query
{
    public void Invoke();
}

public class AsyncQuery<T> : Query
{
    private readonly Func<T> _action;

    public AsyncQuery(Func<T> action)
    {
        Status = Status.PENDING;
        _action = action;
    }
    
    public T? Data { get; private set; }
    public Exception? Exception { get; private set; }
    public Status Status { get; private set; }


    public void Invoke()
    {
        try
        {
            Data = _action();
            Status = Status.SUCCESS;
        }
        catch (Exception e)
        {
            Exception = e;
            Status = Status.FAILURE;
        }
    }
}