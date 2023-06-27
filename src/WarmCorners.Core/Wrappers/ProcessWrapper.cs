using System.Diagnostics;

namespace WarmCorners.Core.Wrappers;

public interface IProcessWrapper
{
    void SetStartInfo(ProcessStartInfo startInfo);
    bool Start();
}

public class ProcessWrapper : IProcessWrapper
{
    private readonly Process _process = new();

    public void SetStartInfo(ProcessStartInfo startInfo)
        => this._process.StartInfo = startInfo;

    public bool Start() =>
        this._process.Start();
}
