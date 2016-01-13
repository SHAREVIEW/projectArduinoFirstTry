using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace projectArduinoFirstTry.Sources
{
    static public class BluetoothHandler
    {
        private static BluetoothClient _cli;
        private static bool _isConnected = false;
        private static byte[] _aesKey = new byte[] {
            0x80, 0x59, 0x43, 0xFA, 0x9D, 0x2B, 0x3F, 0x01, 0x00, 0x45, 0x89, 0x7A, 0x0A, 0x3C, 0xE2, 0x54
        };

        private static byte[] _iv = new byte[] {
            0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F
        };

        static public void MakeConnection(string deviceName)
        {
            Guid serviceClass;
            serviceClass = BluetoothService.SerialPort;
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
                var scannedDeviceName = bluetoothDeviceInfo.DeviceName;

                if (scannedDeviceName == deviceName)
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

        public static void SendMessage(DeltaAngle deltaAngle)
        {
            if (_cli == null)
            {
                return;
            }

            Stream peerStream = _cli.GetStream();

            byte[] buffer = new byte[3];

            buffer[0] = (byte) ',';

            var value = new byte();
            peerStream.WriteByte(value);
        }

        public static void GetStrFromBluetooth()
        {
            Stream peerStream = _cli.GetStream();

            byte[] buffer = new byte[1000];
            string str = "";
            int byteRead = peerStream.ReadByte();
            int length = 0;
            while ((char)byteRead != '\r')
            {
                str += (char)byteRead;
                buffer[length] = (byte)byteRead;
                ++length;
                byteRead = peerStream.ReadByte();
            }
            byte[] encrypted = new byte[length];
            for (int i = 0; i < length; ++i)
            {
                encrypted[i] = buffer[i];
            }

            Console.WriteLine("Received {0} bytes", length);
            string decrypted = DecryptStringFromBytes(encrypted, _aesKey, _iv);
            Console.WriteLine("Decrypted string:" + decrypted);

            peerStream.Flush();
            Thread.Sleep(2000);
        }

        static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments. 
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold 
            // the decrypted text. 
            string plaintext = null;

            // Create an RijndaelManaged object 
            // with the specified key and IV. 
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;
                rijAlg.Padding = PaddingMode.Zeros;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption. 
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream 
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }
            return plaintext;
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
