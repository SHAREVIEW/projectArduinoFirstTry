using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace projectArduinoFirstTry.Sources
{
    static public class BluetoothHandler
    {
        static public void MakeConnection()
        {
            Guid serviceClass;
            serviceClass = BluetoothService.SerialPort;

            var cli = new BluetoothClient();
            var bluetoothDeviceInfos = cli.DiscoverDevices();
            var deviceInfos = bluetoothDeviceInfos.ToList();
            BluetoothDeviceInfo device = null;
            foreach (var bluetoothDeviceInfo in deviceInfos)
            {
                var deviceName = bluetoothDeviceInfo.DeviceName;

                if (deviceName == "HC-06")
                {
                    device = bluetoothDeviceInfo;
                }
            }

            if (device == null)
            {
                return;
            }

            var ep = new BluetoothEndPoint(device.DeviceAddress, serviceClass);

            if (!device.Connected)
                cli.Connect(ep);

            Stream peerStream = cli.GetStream();
            char[] chars = new[] {'4', '5', ',', '4', '5'};
            char b = '4';
            byte bByte = (byte) b;
            byte[] buffer = Encoding.ASCII.GetBytes(chars);
            peerStream.WriteAsync(buffer, 0, buffer.Count());
        }
    }
}
