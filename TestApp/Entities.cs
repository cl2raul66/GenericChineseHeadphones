using System.Runtime.InteropServices;

namespace GenericChineseHeadphones.TestApp;

[StructLayout(LayoutKind.Sequential)]
public struct SYSTEMTIME
{
    public ushort wYear;
    public ushort wMonth;
    public ushort wDayOfWeek;
    public ushort wDay;
    public ushort wHour;
    public ushort wMinute;
    public ushort wSecond;
    public ushort wMilliseconds;
}

[StructLayout(LayoutKind.Sequential)]
public struct BLUETOOTH_DEVICE_SEARCH_PARAMS
{
    public uint dwSize;               // Tamaño de esta estructura
    public int fReturnAuthenticated;  // Dispositivos emparejados con clave
    public int fReturnRemembered;     // Dispositivos recordados (emparejados)
    public int fReturnUnknown;        // Dispositivos desconocidos
    public int fReturnConnected;      // Dispositivos actualmente conectados
    public int fIssueInquiry;         // ¿Hacer un escaneo nuevo de radio? (Lento, pondremos 0)
    public byte cTimeoutMultiplier;   // Tiempo de espera
    public IntPtr hRadio;             // Handle de la radio (IntPtr.Zero para usar todas)
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public unsafe struct BLUETOOTH_DEVICE_INFO
{
    public uint dwSize;               // Tamaño de esta estructura
    public ulong Address;             // La MAC en formato numérico puro (64 bits)
    public uint ulClassofDevice;      // Tipo de dispositivo (Audio, PC, Teléfono)
    public int fConnected;            // 1 si está conectado, 0 si no
    public int fRemembered;           // 1 si está emparejado
    public int fAuthenticated;        // 1 si está autenticado
    public SYSTEMTIME stLastSeen;     // Última vez visto
    public SYSTEMTIME stLastUsed;     // Última vez usado
    public fixed char szName[248];
}

[StructLayout(LayoutKind.Sequential)]
public struct SP_DEVINFO_DATA
{
    public uint cbSize;
    public Guid ClassGuid;
    public uint DevInst; 
    public IntPtr Reserved;
}

[StructLayout(LayoutKind.Sequential)]
public struct DEVPROPKEY
{
    public Guid fmtid;
    public uint pid;
}
