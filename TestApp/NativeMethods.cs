using System.Runtime.InteropServices;

namespace GenericChineseHeadphones.TestApp;

internal static partial class NativeMethods
{
    [LibraryImport("bthprops.cpl", SetLastError = true)]
    public static partial IntPtr BluetoothFindFirstDevice(in BLUETOOTH_DEVICE_SEARCH_PARAMS pbtsp, ref BLUETOOTH_DEVICE_INFO pbtdi);

    [LibraryImport("bthprops.cpl", SetLastError = true)]
    public static partial int BluetoothFindNextDevice(IntPtr hFind, ref BLUETOOTH_DEVICE_INFO pbtdi);

    [LibraryImport("bthprops.cpl", SetLastError = true)]
    public static partial int BluetoothFindDeviceClose(IntPtr hFind);
}
