using SharpHook;
using SharpHook.Native;

namespace WarmCorners.Core.Wrappers;

public interface IEventSimulatorWrapper
{
    UioHookResult SimulateKeyPress(KeyCode keyCode);
    UioHookResult SimulateKeyRelease(KeyCode keyCode);
}

public class EventSimulatorWrapper : IEventSimulatorWrapper
{
    private readonly EventSimulator _eventSimulator = new();

    public UioHookResult SimulateKeyPress(KeyCode keyCode) => this._eventSimulator.SimulateKeyPress(keyCode);

    public UioHookResult SimulateKeyRelease(KeyCode keyCode) => this._eventSimulator.SimulateKeyRelease(keyCode);
}
