using System;
using System.Collections.Generic;
using System.IO;
using System.Speech.Recognition;
using System.Windows;
using System.Windows.Controls;
using System.Web.Script.Serialization;
using BluetoothSample.Services;
using projectArduinoFirstTry.Sources;
using System.Windows.Media.Imaging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media;
using InTheHand.Net;

namespace projectArduinoFirstTry
{
    public partial class MainWindow : Window, IDisposable
    {
        internal static List<Medicine> Dict
        {
            get { return _medicineList.MedicineVal; }
            set { _medicineList.MedicineVal = value; }
        }

        public readonly Dictionary<long, MedicineInfo> MedicineInfoDict = new Dictionary<long, MedicineInfo>();

        private readonly SpeechRecognitionEngine _speechRecognizer = new SpeechRecognitionEngine();
        private static MedicineList _medicineList;
        private bool _isMicOn = false;
        
        public MainWindow()
        {
            InitializeComponent();

            InitializeSpeechRecognizer();

            InitializeDeltaAngles();

            try
            {
                AzureHandler.InitializeAzureStorage();
                _medicineList = AzureHandler.ReadFromTable();
            }
            catch (Exception)
            {
                JsonHandler();
            }
        }

        private void InitializeDeltaAngles()
        {
            var medicineInfo = new MedicineInfo(new DeltaAngle(45, 39));
            MedicineInfoDict.Add(7290008086363, medicineInfo);

            medicineInfo = new MedicineInfo(new DeltaAngle(4, 40));
            MedicineInfoDict.Add(7290008004664, medicineInfo);

            medicineInfo = new MedicineInfo(new DeltaAngle(42, 40));
            MedicineInfoDict.Add(7290008546287, medicineInfo);

            medicineInfo = new MedicineInfo(new DeltaAngle(37, 41));
            MedicineInfoDict.Add(729000002988, medicineInfo);

            medicineInfo = new MedicineInfo(new DeltaAngle(33, 42));
            MedicineInfoDict.Add(729008872317, medicineInfo);

            medicineInfo = new MedicineInfo(new DeltaAngle(28, 42));
            MedicineInfoDict.Add(729000801650, medicineInfo);

            medicineInfo = new MedicineInfo(new DeltaAngle(23, 43));
            MedicineInfoDict.Add(729008546126, medicineInfo);

            medicineInfo = new MedicineInfo(new DeltaAngle(18, 42));
            MedicineInfoDict.Add(729008546003, medicineInfo);

            medicineInfo = new MedicineInfo(new DeltaAngle(12, 42));
            MedicineInfoDict.Add(729000806198, medicineInfo);

            medicineInfo = new MedicineInfo(new DeltaAngle(9, 40));
            MedicineInfoDict.Add(7290102062218, medicineInfo);

            medicineInfo = new MedicineInfo(new DeltaAngle(0, 39));
            MedicineInfoDict.Add(7290000810027, medicineInfo);
        }

        private void JsonHandler()
        {
            string jsonFile = "C:\\Users\\admin\\Documents\\Visual Studio 2015\\Projects\\projectArduinoFirstTry\\medicine.json";
            var readAllText = File.ReadAllText(jsonFile);
            JavaScriptSerializer ser = new JavaScriptSerializer();
            _medicineList = ser.Deserialize<MedicineList>(readAllText);
        }

        internal void UpdateCounter(Medicine medicine, MainWindow mainWindow)
        {
            TextBlock textBlockCounter = (TextBlock)mainWindow.DrugsGrid.FindName($"count_{medicine.Code}");

            int count;
            if (textBlockCounter != null && int.TryParse(textBlockCounter.Text, out count))
            {
                count += 1;
                textBlockCounter.Text = count.ToString();
            }
        }
        private void InitializeSpeechRecognizer()
        {
            _speechRecognizer.UnloadAllGrammars();

            _speechRecognizer.SpeechRecognized += speechRecognizer_SpeechRecognized;

            GrammarBuilder grammarBuilder = new GrammarBuilder();

            Choices commandChoices = new Choices("medicine");

            grammarBuilder.Append(commandChoices);

            Choices valueChoices = new Choices();

            valueChoices.Add("Zinnat", "Lorivan", "Nocturno", "Zodorm", "Vader","Brotizolam", "Norvasc", "Spirnolactone","Zaldiar","Tribmin","Rispond","Spirnolactone","Ridazin","bonserin","Alloril","Amlodipine","Amlow","Muscol");

            grammarBuilder.Append(valueChoices);

            _speechRecognizer.LoadGrammar(new Grammar(grammarBuilder));
            _speechRecognizer.SetInputToDefaultAudioDevice();
        }

        private void speechRecognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            var firstWord = e.Result.Words[0].Text.ToLower();
            if (e.Result.Words.Count != 2 && firstWord != "medicine")
            {
                return;
            }

            var medicineName = e.Result.Words[1].Text.ToLower();

            Medicine medicine = new Medicine();
            for (int index = 0; index < _medicineList.MedicineVal.Count; index++)
            {
                medicine = _medicineList.MedicineVal[index];

                if (medicine.Name.ToLower() != medicineName)
                {
                    continue;
                }

                MedicineLbl.Content = medicineName.ToUpper();

                UsagesText.Text = medicine.UserDesc;

                DangersText.Text = medicine.DangersDesc;

                if (medicine.ImagePath == string.Empty)
                {
                    return;
                }
                
                BitmapImage bitImage = new BitmapImage();
                bitImage.BeginInit();
                bitImage.UriSource = new Uri(medicine.ImagePath);
                bitImage.EndInit();

                ImageContent.Source = bitImage;
                break;
            }

            if (MedicineInfoDict.ContainsKey(medicine.Code) && BluetoothHandler.IsConnected())
            {
                BluetoothHandler.SendAnglesToLaser(MedicineInfoDict[medicine.Code].DeltaAngle);
                Console.WriteLine("Got this medicine: {0}", medicine.Name);
            }

            _speechRecognizer.SpeechRecognized += speechRecognizer_SpeechRecognized;
            
            #region voice recognition with cases sample

            //            Medicine.Content = e.Result.Words;

            /*
                if(e.Result.Words.Count == 2)
                        {
                                string command = e.Result.Words[0].Text.ToLower();
                                string value = e.Result.Words[1].Text.ToLower();
                                switch(command)
                                {
                                        case "weight":
                                                FontWeightConverter weightConverter = new FontWeightConverter();
                                                lblDemo.FontWeight = (FontWeight)weightConverter.ConvertFromString(value);
                                                break;
                                        case "color":
                                                lblDemo.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(value));
                                                break;
                                        case "size":
                                                switch(value)
                                                {
                                                        case "small":
                                                                lblDemo.FontSize = 12;
                                                                break;
                                                        case "medium":
                                                                lblDemo.FontSize = 24;
                                                                break;
                                                        case "large":
                                                                lblDemo.FontSize = 48;
                                                                break;
                                                }
                                                break;
                                }
                        }
            */

            #endregion
        }
        private void OnLoad(object sender, RoutedEventArgs e)
        {
            PutMedicine();
        }
        private void PutMedicine()
        {
            if (_medicineList == null)
            {
                return;
            }

            var medicines = _medicineList.MedicineVal;
            for (int index = 0; index < medicines.Count; index++)
            {
                var medicine = medicines[index];
                var medicineVal = medicine;
                RowAdder.AddRow(medicineVal, this, index + 1);
            }
        }

        private void OnClickMice(object sender, RoutedEventArgs e)
        {
            if(!_isMicOn)
            {
                _isMicOn = true;
                _speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
                MicBorder.BorderBrush = new SolidColorBrush(Colors.Red);
                return;
            }

            _speechRecognizer.RecognizeAsyncStop();

            MicBorder.BorderBrush = new SolidColorBrush(Colors.Transparent);

            _isMicOn = false;
        }

        private void OnClickAdd(object sender, RoutedEventArgs e)
        {
            Add popup = new Add(this);
            popup.ShowDialog();
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnBarcodeClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var str = BluetoothHandler.GetStrFromBluetooth().Trim('\0').Split(',');
                long medicineCode = long.Parse(str[0]);
                string medicineName = str[1];
                int month = int.Parse(str[2].Substring(0, 2));
                int year = int.Parse(str[2].Substring(2, 4));
                var medicine = new Medicine(medicineName, new DateTime(year, month, 1), medicineCode);

                List<Medicine> medicines = Dict;
                Predicate<Medicine> medicineFinder = m => { return m.Code == medicineCode; };
                if (medicines.Exists(medicineFinder))
                {
                    UpdateCounter(medicine, this);

                    return;
                }

                medicines.Add(medicine);

                RowAdder.AddRow(medicine, this, medicines.Count);
            }
            catch (Exception exception)
            {
                bluetoothIndicator.Fill = new SolidColorBrush(Colors.Red);
                Console.WriteLine(exception);
                BluetoothHandler.Close();
            }
        }

        public void Dispose()
        {
            BluetoothHandler.Close();
        }

        private void onNFC(object sender, RoutedEventArgs e)
        {
            BluetoothAddress btAddress = new BluetoothAddress(new byte[]{252, 27, 32, 49, 211, 152, 0, 0});
            BluetoothHandler.MakeConnection(btAddress);
            bluetoothIndicator.Fill = !BluetoothHandler.IsConnected() ? new SolidColorBrush( Colors.Red) : new SolidColorBrush(Colors.Green);
        }

        private void OnLaser(object sender, RoutedEventArgs e)
        {
            BluetoothAddress btAddress = new BluetoothAddress(new byte[] { 17, 71, 48, 49, 211, 152, 0, 0 });
            BluetoothHandler.MakeConnection(btAddress);

            if (!BluetoothHandler.IsConnected())
            {
                bluetoothIndicator.Fill = new SolidColorBrush(Colors.Red);
            }
            else
            {
                bluetoothIndicator.Fill = new SolidColorBrush(Colors.Green);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}
