namespace GenericChineseHeadphones.TestApp;

internal class Program
{
    static unsafe void Main(string[] args)
    {
        Console.WriteLine("Buscando dispositivos Bluetooth (Modo Nativo)...\n");

        var bdi = DeviceServices.Find();
        Int16 count = 1;

        if (bdi.Length != 0)
        {
            foreach (var item in bdi)
            {
                if (!Tools.IsAudioDevice(item.ulClassofDevice)) continue;

                string deviceName = new string(item.szName).TrimEnd('\0');
                if (string.IsNullOrWhiteSpace(deviceName)) continue;

                string macAddress = Tools.FormatMacAddress(item.Address);
                bool isConnected = item.fConnected != 0;

                Console.WriteLine($"[{count}] {deviceName} (Dispositivo de Audio)");
                Console.WriteLine($"    MAC: {macAddress}");
                if (isConnected)
                {
                    Console.WriteLine("    Estado: Conectado");
                    PnpScanner.FindBluetoothChildren(macAddress);
                }
                else
                {
                    Console.WriteLine("    Estado: Desconectado");
                }

                Console.WriteLine("--------------------------------------------------");
                count++;
            }
        }
        else
        {
            Console.WriteLine("No se encontraron dispositivos.");
        }

        Console.ReadLine();
    }
}
