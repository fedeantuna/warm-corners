using System.Diagnostics;

namespace WarmCorners.Core.Wrappers;

public interface IProcessWrapper
{
    ProcessStartInfo StartInfo { get; }
    void SetStartInfo(ProcessStartInfo startInfo);
    bool Start();
}

public class ProcessWrapper : IProcessWrapper
{
    private readonly Process _process = new();

    public ProcessStartInfo StartInfo => this._process.StartInfo;

    public void SetStartInfo(ProcessStartInfo startInfo)
        => this._process.StartInfo = startInfo;

    public bool Start() =>
        this._process.Start();
}
