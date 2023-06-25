using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace WarmCorners.Service.Infrastructure.Wrapper;

public interface IUser32Wrapper
{
    (int Width, int Height) GetScreenResolution();
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class User32Wrapper : IUser32Wrapper
{
    public (int Width, int Height) GetScreenResolution()
    {
        const int ENUM_CURRENT_SETTINGS = -1;

        DEVMODE devMode = default;
        devMode.dmSize = (short)Marshal.SizeOf(devMode);
        EnumDisplaySettings(null!, ENUM_CURRENT_SETTINGS, ref devMode);

        return (devMode.dmPelsWidth, devMode.dmPelsHeight);
    }

    [DllImport("user32.dll")]
    [SuppressMessage("Globalization", "CA2101:Specify marshaling for P/Invoke string arguments")]
    [SuppressMessage("Interoperability",
        "SYSLIB1054:Use \'LibraryImportAttribute\' instead of \'DllImportAttribute\' to generate P/Invoke marshalling code at compile time")]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

    [StructLayout(LayoutKind.Sequential)]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
    private struct DEVMODE
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
        public readonly string dmDeviceName;

        public readonly short dmSpecVersion;
        public readonly short dmDriverVersion;
        public short dmSize;
        public readonly short dmDriverExtra;
        public readonly int dmFields;
        public readonly int dmPositionX;
        public readonly int dmPositionY;
        public readonly int dmDisplayOrientation;
        public readonly int dmDisplayFixedOutput;
        public readonly short dmColor;
        public readonly short dmDuplex;
        public readonly short dmYResolution;
        public readonly short dmTTOption;
        public readonly short dmCollate;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
        public readonly string dmFormName;

        public readonly short dmLogPixels;
        public readonly int dmBitsPerPel;
        public readonly int dmPelsWidth;
        public readonly int dmPelsHeight;
        public readonly int dmDisplayFlags;
        public readonly int dmDisplayFrequency;
        public readonly int dmICMMethod;
        public readonly int dmICMIntent;
        public readonly int dmMediaType;
        public readonly int dmDitherType;
        public readonly int dmReserved1;
        public readonly int dmReserved2;
        public readonly int dmPanningWidth;
        public readonly int dmPanningHeight;
    }
}
