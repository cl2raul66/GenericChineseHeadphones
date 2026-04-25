using System.Runtime.InteropServices;

namespace GenericChineseHeadphones.TestApp;

internal static partial class PnpNativeMethods
{
    public const uint DIGCF_PRESENT = 0x00000002;
    public const uint DIGCF_ALLCLASSES = 0x00000004;
    public const int CR_SUCCESS = 0x00000000;

    // --- SetupAPI ---
    [LibraryImport("setupapi.dll", EntryPoint = "SetupDiGetClassDevsW", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    public static partial IntPtr SetupDiGetClassDevs(IntPtr ClassGuid, string Enumerator, IntPtr hwndParent, uint Flags);

    [LibraryImport("setupapi.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, uint MemberIndex, ref SP_DEVINFO_DATA DeviceInfoData);

    [LibraryImport("setupapi.dll", EntryPoint = "SetupDiGetDeviceInstanceIdW", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetupDiGetDeviceInstanceId(IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, [Out] char[] DeviceInstanceId, uint DeviceInstanceIdSize, out uint RequiredSize);

    [LibraryImport("setupapi.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

    // --- CfgMgr32 ---
    [LibraryImport("cfgmgr32.dll")]
    public static partial int CM_Get_Child(out uint dnDevInst, uint dnDevInstParent, uint ulFlags);

    [LibraryImport("cfgmgr32.dll")]
    public static partial int CM_Get_Sibling(out uint dnDevInst, uint dnDevInstSibling, uint ulFlags);

    [LibraryImport("cfgmgr32.dll", EntryPoint = "CM_Get_Device_IDW", StringMarshalling = StringMarshalling.Utf16)]
    public static partial int CM_Get_Device_ID(uint dnDevInst, [Out] char[] Buffer, uint BufferLen, uint ulFlags);

    [LibraryImport("cfgmgr32.dll", EntryPoint = "CM_Get_DevNode_PropertyW")]
    public static partial int CM_Get_DevNode_Property(uint dnDevInst, in DEVPROPKEY PropertyKey, out uint PropertyType, IntPtr PropertyBuffer, ref uint PropertyBufferSize, uint ulFlags);
}
