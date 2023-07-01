[![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/fedeantuna/warm-corners/build.yml?style=flat-square)](https://github.com/fedeantuna/warm-corners/blob/main/.github/workflows/build.yml)
[![Codacy Code Analysis](https://img.shields.io/codacy/grade/cbd8b1b81d7a4dfaabb4cff12ea7ab1b?style=flat-square)](https://www.codacy.com/gh/fedeantuna/warm-corners/dashboard?utm_source=github.com\&utm_medium=referral\&utm_content=fedeantuna/warm-corners\&utm_campaign=Badge_Grade)
[![Codacy Code Coverage](https://img.shields.io/codacy/coverage/cbd8b1b81d7a4dfaabb4cff12ea7ab1b?style=flat-square)](https://www.codacy.com/gh/fedeantuna/warm-corners/dashboard?utm_source=github.com\&utm_medium=referral\&utm_content=fedeantuna/warm-corners\&utm_campaign=Badge_Coverage)
[![GitHub](https://img.shields.io/github/license/fedeantuna/warm-corners?style=flat-square)](https://github.com/fedeantuna/warm-corners/blob/main/LICENSE)

# WarmCorners

WarmCorners is a solution compatible with Windows that provides a feature similar to `Hot Corners` on GNOME. Basically you can setup triggers for when you move the mouse over the corners of main screen.

## Usage

WarmCorners detects where your mouse cursor is in real time. If it's over a corner where you have a trigger configured, it will execute the trigger.

The way to configure triggers is using the `appsettings.json` file. If you installed the software using the installation script, then the file should be located under `%APPDATA%\WarmCorners\`.

You can configure `Command Triggers` and `Key Combination Triggers`, even both at the same time. By default there will be a single `Key Combination Trigger`.

### Command Triggers

Command Triggers are triggers that when activated will execute a command. These triggers are configured as items in a list called `CommandTriggers` inside the `Triggers` key. Lets say you want to execute the notepad command when you move your mouse cursor over the top right corner, you will need to have the following in your `appsettings.json`:

```json
{
  "Triggers": {
    "CommandTriggers": [
      {
        "ScreenCorner": "TopRight",
        "Command": "notepad"
      }
    ]
  }
}
```

The possible values for the `ScreenCorner` are:

*   TopLeft
*   TopRight
*   BottomRight
*   BottomLeft

These are all case insensitive, so you can use whatever casing you like the most.

The possible values for the `Command` are any command that you can run using `cmd`.

If you want to add more tha one Command Trigger, just add another entry to that list following the same rules.

### Key Combination Triggers

Key Combination Triggers are triggers that when activated will execute a key combination. These triggers are configured as items in a list called `KeyCombinationTriggers` inside the `Triggers` key. Lets say you want to execute the `WindowsKey + Tab` key combination when you move your mouse cursor over the top left corner in order to have a similar behavior to the `Activities` on `GNOME`. You will need to have the following in your `appsettings.json`:

```json
{
  "Triggers": {
    "KeyCombinationTriggers": [
      {
        "ScreenCorner": "TopLeft",
        "KeyCombination": "LeftMeta+Tab"
      }
    ]
  }
}
```

The possible values for the `ScreenCorner` are:

*   TopLeft
*   TopRight
*   BottomRight
*   BottomLeft

These are all case insensitive, so you can use whatever casing you like the most.

The possible values for the `KeyCombination` are any valid keys (see below) joined together with a `+` sign.

If you want to add more tha one Key Combination Trigger, just add another entry to that list following the same rules.

Valid Keys for `KeyCombination`:

*   Escape
*   F1
*   F2
*   F3
*   F4
*   F5
*   F6
*   F7
*   F8
*   F9
*   F10
*   F11
*   F12
*   F13
*   F14
*   F15
*   F16
*   F17
*   F18
*   F19
*   F20
*   F21
*   F22
*   F23
*   F24
*   Backquote
*   1
*   2
*   3
*   4
*   5
*   6
*   7
*   8
*   9
*   0
*   Minus
*   Equals
*   Backspace
*   Tab
*   CapsLock
*   A
*   B
*   C
*   D
*   E
*   F
*   G
*   H
*   I
*   J
*   K
*   L
*   M
*   N
*   O
*   P
*   Q
*   R
*   S
*   T
*   U
*   V
*   W
*   X
*   Y
*   Z
*   OpenBracket
*   CloseBracket
*   BackSlash
*   Semicolon
*   Quote
*   Enter
*   Comma
*   Period
*   Slash
*   Space
*   PrintScreen
*   ScrollLock
*   Pause
*   LesserGreater
*   Insert
*   Delete
*   Home
*   End
*   PageUp
*   PageDown
*   Up
*   Left
*   Clear
*   Right
*   Down
*   NumLock
*   NumPadDivide
*   NumPadMultiply
*   NumPadSubtract
*   NumPadEquals
*   NumPadAdd
*   NumPadEnter
*   NumPadSeparator
*   NumPad1
*   NumPad2
*   NumPad3
*   NumPad4
*   NumPad5
*   NumPad6
*   NumPad7
*   NumPad8
*   NumPad9
*   NumPad0
*   NumPadEnd
*   NumPadDown
*   NumPadPageDown
*   NumPadLeft
*   NumPadClear
*   NumPadRight
*   NumPadHome
*   NumPadUp
*   NumPadPageUp
*   NumPadInsert
*   NumPadDelete
*   LeftShift
*   RightShift
*   LeftControl
*   RightControl
*   LeftAlt
*   RightAlt
*   LeftMeta
*   RightMeta
*   ContextMenu
*   Power
*   Sleep
*   Wake
*   MediaPlay
*   MediaStop
*   MediaPrevious
*   MediaNext
*   MediaSelect
*   MediaEject
*   VolumeMute
*   VolumeUp
*   VolumeDown
*   AppMail
*   AppCalculator
*   AppMusic
*   AppPictures
*   BrowserSearch
*   BrowserHome
*   BrowserBack
*   BrowserForward
*   BrowserStop
*   BrowserRefresh
*   BrowserFavorites
*   Katakana
*   Underscore
*   Furigana
*   Kanji
*   Hiragana
*   Yen
*   NumPadComma
*   SunHelp
*   SunStop
*   SunProps
*   SunFront
*   SunOpen
*   SunFind
*   SunAgain
*   SunUndo
*   SunCopy
*   SunInsert
*   SunCut

## Build from source

In order to build from source you need the `dotnet` SDK and the source code. This project is using version 7.0, you can download it from the [official website](https://dotnet.microsoft.com/en-us/).

Once `dotnet` is installed, you can use the following commands in your shell while in the solution root directory (directory where the `WarmCorners.sln` file is located):

*   Build the solution: `dotnet build`
*   Run the solution: `dotnet run --project src/WarmCorners.Service`
*   Run tests: `dotnet test`

To get the coverage results, you can alternatively run the tests using the scripts `run_test_coverage.sh` and `run_test_coverage.ps1` that are in the `scripts` directory.

If you prefer using an IDE, you can do so with Visual Studio, Visual Studio Code, JetBrains Rider and vi/vim/neovim. Others, like MonoDevelop, should also work fine as long as you have the `dotnet` SDK installed. If your IDE has the option, select `WarmCorners.Service` as the `Main Project`, `Startup Project` or `Entry Point`.

## Create an issue

Do you have ideas on how to improve WarmCorners? Are there things that bother you? Did you find any errors? Then please, create an issue!

If you found a bug, create an issue from the `bug` template. If you want a new feature to be available, create an issue from the `feature` template. If you want to ask for a change in the code to improve quality, readability, etc. then create an issue from the `feature` template but update the label to the proper one:

*   **build**: Changes to the build system, like updating or adding a publish profile
*   **chore**: General changes, like updating or adding a script
*   **ci**: Changes to CI configuration and scripts (that are CI related), like updating a GitHub Workflow
*   **docs**: Improvements or additions to documentation
*   **feat**: New feature or request (default label for the `feature` template)
*   **fix**: Something isn't working, like triggers are not triggering
*   **perf**: Improvements on performance
*   **refactor**: Improvements on code quality
*   **test**: Adding or correcting tests

The labels `dependencies` and `.NET` are only for `dependabot` to use. `duplicate`, `good first issue` and `wontfix` are only for maintainers to use.

If you are not sure which label to use, you can just leave it blank or use the one you think fits better, don't be afraid to make a mistake!
