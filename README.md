https://csharp.hotexamples.com/examples/InTheHand.Net.Sockets/BluetoothClient/Connect/php-bluetoothclient-connect-method-examples.html


EXAMPLE #1



# projectArduinoFirstTry
first attempt building gui


File: BluetoothHandler.cs  Project: idan573/projectArduinoFirstTry


        public static void MakeConnection(BluetoothAddress btAddress)
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
