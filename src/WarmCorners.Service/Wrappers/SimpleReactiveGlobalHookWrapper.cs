using System.Reactive;
using SharpHook;
using SharpHook.Reactive;

namespace WarmCorners.Service.Wrappers;

public interface ISimpleReactiveGlobalHookWrapper : IDisposable
{
    IObservable<MouseHookEventArgs> MouseMoved { get; }
    IObservable<Unit> RunAsync();
}

public class SimpleReactiveGlobalHookWrapper : ISimpleReactiveGlobalHookWrapper
{
    private readonly SimpleReactiveGlobalHook _simpleReactiveGlobalHook = new();

    public IObservable<MouseHookEventArgs> MouseMoved => this._simpleReactiveGlobalHook.MouseMoved;

    public IObservable<Unit> RunAsync() => this._simpleReactiveGlobalHook.RunAsync();

    public void Dispose()
    {
        this._simpleReactiveGlobalHook.Dispose();
        GC.SuppressFinalize(this);
    }
}
