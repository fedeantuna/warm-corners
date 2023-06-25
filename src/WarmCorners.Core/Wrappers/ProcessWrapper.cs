using System.Diagnostics;

namespace WarmCorners.Core.Wrappers;

public interface IProcessWrapper
{
    bool Start(string fileName, string arguments);
}

public class ProcessWrapper : IProcessWrapper
{
    public bool Start(string fileName, string arguments)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo(fileName, arguments)
            {
                CreateNoWindow = true
            }
        };

        return process.Start();
    }
}
