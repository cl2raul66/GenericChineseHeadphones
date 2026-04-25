using System.Runtime.InteropServices;

namespace GenericChineseHeadphones.TestApp;

internal static class Tools
{
    public static string FormatMacAddress(ulong address)
    {
        byte[] bytes = BitConverter.GetBytes(address);
        return $"{bytes[5]:X2}:{bytes[4]:X2}:{bytes[3]:X2}:{bytes[2]:X2}:{bytes[1]:X2}:{bytes[0]:X2}";
    }

    public static bool IsAudioDevice(uint classOfDevice)
    {
        uint majorDeviceClass = classOfDevice & 0x1F00;
        return majorDeviceClass == 0x0400;
    }

    public static (string ShortName, string FullName) IdentifyBluetoothService(string instanceId)
    {
        int startIndex = instanceId.IndexOf('{');

        if (startIndex != -1 && instanceId.Length >= startIndex + 9)
        {
            ReadOnlySpan<char> hexSpan = instanceId.AsSpan(startIndex + 5, 4);

            if (ushort.TryParse(hexSpan, System.Globalization.NumberStyles.HexNumber, null, out ushort uuid))
            {
                return GetProfileInfo(uuid);
            }
        }

        return ("UNKNOWN", "Servicio Desconocido / Formato no estándar");
    }

    public static string[] ParseMultiSz(IntPtr buffer)
    {
        List<string> strings = [];
        IntPtr current = buffer;

        while (true)
        {
            string? s = Marshal.PtrToStringUni(current);
            if (string.IsNullOrEmpty(s)) break; 

            strings.Add(s);

            current = IntPtr.Add(current, (s.Length + 1) * 2);
        }
        return [..strings];
    }

    public static void ReadSiblings(uint devInst, string cleanMac, DEVPROPKEY devpropkey)
    {
        uint bufferSize = 0;

        _ = PnpNativeMethods.CM_Get_DevNode_Property(devInst, in devpropkey, out _, IntPtr.Zero, ref bufferSize, 0);

        if (bufferSize == 0)
        {
            Console.WriteLine("  |-- (No se encontraron servicios del mismo nivel)");
            return;
        }

        IntPtr buffer = Marshal.AllocHGlobal((int)bufferSize);
        try
        {
            int result = PnpNativeMethods.CM_Get_DevNode_Property(devInst, in devpropkey, out _, buffer, ref bufferSize, 0);

            if (result == PnpNativeMethods.CR_SUCCESS)
            {
                Console.WriteLine("  |-- [SERVICIOS ENCONTRADOS (Del mismo nivel)]:");

                int count = 0;
                foreach (string siblingId in ParseMultiSz(buffer))
                {
                    if (siblingId.Contains(cleanMac) && siblingId.Contains('{'))
                    {
                        var (ShortName, FullName) = IdentifyBluetoothService(siblingId);
                        Console.WriteLine($"      |-- [{ShortName}] {FullName}");
                        Console.WriteLine($"          ID: {siblingId}");
                        count++;
                    }
                }

                if (count == 0)
                {
                    Console.WriteLine("      |-- (Los servicios están inactivos o el audífono está suspendido)");
                }
            }
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    private static (string ShortName, string FullName) GetProfileInfo(ushort uuid) => uuid switch
    {
        // === AUDIO CLASSIC (A2DP + AVRCP + HFP) ===
        0x110A => ("A2DP", "Advanced Audio Distribution Profile (Sink)"),
        0x110B => ("A2DP", "Advanced Audio Distribution Profile (Source)"),
        0x110E => ("AVRCP", "Audio/Video Remote Control Profile"),
        0x111E => ("HFP", "Hands-Free Profile (AG)"),
        0x111F => ("HFP", "Hands-Free Profile (HF)"),
        0x1108 => ("HSP", "Headset Profile"),

        // === CONTROL (BOTONES) ===
        0x1124 => ("HID", "Human Interface Device (Controles)"),

        // === LE AUDIO (Windows 11 24H2+) ===
        0x1847 => ("BAP", "Basic Audio Profile"),
        0x1848 => ("TMAP", "Telephony and Media Audio Profile"),
        0x1849 => ("MCP", "Media Control Profile"),
        0x184A => ("CCP", "Call Control Profile"),
        0x184B => ("MICP", "Microphone Control Profile"),
        0x184C => ("VCP", "Volume Control Profile"),
        0x184D => ("CSIP", "Coordinated Set Identification"),
        0x184E => ("ASC", "Audio Stream Control"),
        0x184F => ("BASS", "Broadcast Audio Scan Service"),
        0x1850 => ("PAC", "Published Audio Capabilities"),
        0x1854 => ("HAS", "Hearing Access Service"),

        // === SERVICIOS GATT UTILES ===
        0x180F => ("BAS", "Battery Service"),
        0x180A => ("DIS", "Device Information Service"),
        0x1811 => ("ANS", "Alert Notification Service"),
        0x181C => ("OTS", "Object Transfer Service"),

        _ => ("UNKNOWN", $"Perfil desconocido (0x{uuid:X4})")
    };
}
