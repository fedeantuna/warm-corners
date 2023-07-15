using System.Diagnostics;

namespace WarmCorners.Application.Common.Wrappers;

public interface IProcessWrapper
{
    void SetStartInfo(ProcessStartInfo startInfo);
    bool Start();
}
