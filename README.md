[![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/fedeantuna/warm-corners/build.yml?style=flat-square)](https://github.com/fedeantuna/warm-corners/blob/main/.github/workflows/build.yml)
[![Codacy Code Analysis](https://img.shields.io/codacy/grade/cbd8b1b81d7a4dfaabb4cff12ea7ab1b?style=flat-square)](https://www.codacy.com/gh/fedeantuna/warm-corners/dashboard?utm_source=github.com\&utm_medium=referral\&utm_content=fedeantuna/warm-corners\&utm_campaign=Badge_Grade)
[![Codacy Code Coverage](https://img.shields.io/codacy/coverage/cbd8b1b81d7a4dfaabb4cff12ea7ab1b?style=flat-square)](https://www.codacy.com/gh/fedeantuna/warm-corners/dashboard?utm_source=github.com\&utm_medium=referral\&utm_content=fedeantuna/warm-corners\&utm_campaign=Badge_Coverage)
[![GitHub](https://img.shields.io/github/license/fedeantuna/warm-corners?style=flat-square)](https://github.com/fedeantuna/warm-corners/blob/main/LICENSE)

# WarmCorners

WarmCorners is a solution compatible with Windows that provides a feature similar to `Hot Corners` on GNOME. Basically you can setup triggers for when you move the mouse over the corners of main screen.

## Build from source

In order to build from source you need the `dotnet` SDK and the source code. This project is using version 7.0, you can download it from the [official website](https://dotnet.microsoft.com/en-us/).

Once `dotnet` is installed, you can use the following commands in your shell while in the solution root directory (directory where the `WarmCorners.sln` file is located):

- Build the solution: `dotnet build`
- Run the solution: `dotnet run --project src/WarmCorners.Service`
- Run tests: `dotnet test`

To get the coverage results, you can alternatively run the tests using the scripts `run_test_coverage.sh` and `run_test_coverage.ps1` that are in the `scripts` directory.

If you prefer using an IDE, you can do so with Visual Studio, Visual Studio Code, JetBrains Rider and vi/vim/neovim. Others, like MonoDevelop, should also work fine as long as you have the `dotnet` SDK installed. If your IDE has the option, select `WarmCorners.Service` as the `Main Project`, `Startup Project` or `Entry Point`.