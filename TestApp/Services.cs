using System.Runtime.InteropServices;

namespace GenericChineseHeadphones.TestApp;

internal static class DeviceServices
{
    static BLUETOOTH_DEVICE_SEARCH_PARAMS searchParams = new()
    {
        dwSize = (uint)Marshal.SizeOf<BLUETOOTH_DEVICE_SEARCH_PARAMS>(),
        fReturnRemembered = 1, 
        fReturnConnected = 1, 
        fReturnUnknown = 0,
        fIssueInquiry = 0,    
        cTimeoutMultiplier = 1,
        hRadio = IntPtr.Zero
    };

    static BLUETOOTH_DEVICE_INFO deviceInfo = new()
    {
        dwSize = (uint)Marshal.SizeOf<BLUETOOTH_DEVICE_INFO>()
    };

    static readonly IntPtr hFind = NativeMethods.BluetoothFindFirstDevice(in searchParams, ref deviceInfo);

    public static BLUETOOTH_DEVICE_INFO[] Find()
    {
        if (hFind == IntPtr.Zero)
        {
            Console.WriteLine("No se encontraron dispositivos Bluetooth emparejados.");
            return [];
        }

        List<BLUETOOTH_DEVICE_INFO> bdi = [];

        try
        {
            do
            {
                bdi.Add(deviceInfo);
            }   
            while (NativeMethods.BluetoothFindNextDevice(hFind, ref deviceInfo) != 0);

            return [.. bdi];
        }
        finally
        {
            NativeMethods.BluetoothFindDeviceClose(hFind);
        }
    }
}

internal static class PnpScanner
{
    private static readonly DEVPROPKEY DEVPKEY_Device_BusRelations = new()
    {
        fmtid = new Guid("4340A6C5-93FA-4706-972C-7B648008A5A7"),
        pid = 10 
    };

    public static void FindBluetoothChildren(string macAddress)
    {
        string cleanMac = macAddress.Replace(":", "").ToUpper();
        string parentTarget = $"DEV_{cleanMac}";

        Console.WriteLine($"\nBuscando servicios (BusRelations) para la MAC: {cleanMac}...");

        IntPtr hDevInfo = PnpNativeMethods.SetupDiGetClassDevs(IntPtr.Zero, "BTHENUM", IntPtr.Zero,
            PnpNativeMethods.DIGCF_ALLCLASSES | PnpNativeMethods.DIGCF_PRESENT);

        if (hDevInfo == IntPtr.Zero) return;

        try
        {
            SP_DEVINFO_DATA devData = new()
            {
                cbSize = (uint)Marshal.SizeOf<SP_DEVINFO_DATA>()
            };
            uint i = 0;

            while (PnpNativeMethods.SetupDiEnumDeviceInfo(hDevInfo, i, ref devData))
            {
                char[] instanceIdBuffer = new char[256];
                if (PnpNativeMethods.SetupDiGetDeviceInstanceId(hDevInfo, ref devData, instanceIdBuffer, 256, out _))
                {
                    string instanceId = new string(instanceIdBuffer).TrimEnd('\0');

                    if (instanceId.Contains(parentTarget) && !instanceId.Contains('{'))
                    {
                        Console.WriteLine($"\n[PADRE ENCONTRADO]");
                        Console.WriteLine($"ID: {instanceId}");

                        Tools.ReadSiblings(devData.DevInst, cleanMac, DEVPKEY_Device_BusRelations);
                        break;
                    }
                }
                i++;
            }
        }
        finally
        {
            PnpNativeMethods.SetupDiDestroyDeviceInfoList(hDevInfo);
        }
    }
}
