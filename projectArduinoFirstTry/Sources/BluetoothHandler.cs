using System;
using System.IO;
using System.Linq;
using System.Threading;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace projectArduinoFirstTry.Sources
{
    static public class BluetoothHandler
    {
        private static BluetoothClient _cli;
        private static bool _isConnected = false;

        static public void MakeConnection(BluetoothAddress btAddress)
        {
            var serviceClass = BluetoothService.SerialPort;
            if (_cli != null)
            {
                _cli.Close();
            }

            _cli = new BluetoothClient();
            var bluetoothDeviceInfos = _cli.DiscoverDevices();
            var deviceInfos = bluetoothDeviceInfos.ToList();
            BluetoothDeviceInfo device = null;
            foreach (var bluetoothDeviceInfo in deviceInfos)
            {
                var scannedDeviceAddress = bluetoothDeviceInfo.DeviceAddress;

                if (scannedDeviceAddress == btAddress)
                {
                    device = bluetoothDeviceInfo;
                }
            }

            if (device == null)
            {
                return;
            }

            var ep = new BluetoothEndPoint(device.DeviceAddress, serviceClass);

            try
            {
                if (!device.Connected)
                {
                    _cli.Connect(ep);
                }
            }
            catch(System.Net.Sockets.SocketException e)
            {
                _cli.Close();
                _isConnected = false;
                return;
            }

            _isConnected = true;
        }

        public static void SendAnglesToLaser(DeltaAngle deltaAngle)
        {
            if (_cli == null)
            {
                return;
            }

            string firstAnglesStr = $"{deltaAngle.DeltaX:D3}";
            string secondAnglesStr = $"{deltaAngle.DeltaY:D3}";
            
            byte[] buffer = new byte[firstAnglesStr.Length  + secondAnglesStr.Length + 2];
            int index;
            for (index = 0; index < firstAnglesStr.Length; index++)
            {
                buffer[index] = (byte)firstAnglesStr[index];
            }

            buffer[index++] = (byte) ',';

            int secondIndex;
            for (secondIndex = 0; secondIndex < secondAnglesStr.Length; secondIndex++)
            {
                buffer[index + secondIndex] = (byte)secondAnglesStr[secondIndex];
            }

            buffer[index + secondIndex] = (byte) '$';

            Stream peerStream = _cli.GetStream();
            for (int i = 0; i < index + secondIndex + 1; i++)
            {
                peerStream.WriteByte(buffer[i]);
            }
        }

        public static string GetStrFromBluetooth()
        {
            Stream peerStream = _cli.GetStream();

            byte[] buffer = new byte[1000];
            string str = string.Empty;
            byte length = (byte) peerStream.ReadByte();
            int byteRead = peerStream.ReadByte();

            for (int i = 0; i < length; ++i)
            {
                str += (char)byteRead;
                buffer[i] = (byte)byteRead;
                byteRead = peerStream.ReadByte();
            }

            byte[] encrypted = new byte[length];
            for (int i = 0; i < length; ++i)
            {
                encrypted[i] = buffer[i];
            }

            Console.WriteLine("Received {0} bytes", length);
            string decrypted = AesHandler.DecryptStringFromBytes(encrypted);
            Console.WriteLine("Decrypted string:" + decrypted);
            peerStream.ReadByte();
            peerStream.Flush();
            Thread.Sleep(2000);

            return decrypted;
        }

        static public void Close()
        {
            if (_cli != null)
            {
                _cli.Close();
            }
        }

        static public bool IsConnected()
        {
            return _isConnected;
        }
    }
}
