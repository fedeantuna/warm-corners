namespace WarmCorners.Application.Common.Services;

public interface IShellService
{
    Task Run(string shellCommand);
}
