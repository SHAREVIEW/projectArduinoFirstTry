using System;
using System.Collections.Generic;
using System.IO;
using System.Speech.Recognition;
using System.Windows;
using System.Windows.Controls;
using System.Web.Script.Serialization;
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

        private readonly Dictionary<long, MedicineInfo> _medicineInfoDict = new Dictionary<long, MedicineInfo>(); //Dictionary of medicine and it's angles
        private readonly SpeechRecognitionEngine _speechRecognizer = new SpeechRecognitionEngine();
        private static MedicineList _medicineList; //Medicine list from the cloud
        private bool _isMicOn = false;
        private static int _countClicks;

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

        internal static void UpdateCounter(Medicine medicine, MainWindow mainWindow)
        {
            var textBlockCounter = (TextBlock)mainWindow.DrugsGrid.FindName($"count_{medicine.Code}");

            int count;
            if (textBlockCounter == null || !int.TryParse(textBlockCounter.Text, out count))
            {
                return;
            }

            count += 1;
            textBlockCounter.Text = count.ToString();
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

            if (_medicineInfoDict.ContainsKey(medicine.Code) && BluetoothHandler.IsConnected())
            {
                BluetoothHandler.SendAnglesToLaser(_medicineInfoDict[medicine.Code].DeltaAngle);
                Console.WriteLine("Got this medicine: {0}", medicine.Name);
            }

            _speechRecognizer.SpeechRecognized += speechRecognizer_SpeechRecognized;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            PutMedicinesInTable();
        }

        private void PutMedicinesInTable()
        {
            if (_medicineList == null)
            {
                return;
            }

            var medicines = _medicineList.MedicineVal;
            
            for (var index = 0; index < medicines.Count; index++)
            {
                var medicine = medicines[index];

                var checkBox = DrugsGrid.FindName($"check_{medicine.Code}");
                var isChecked = ((CheckBox) checkBox)?.IsChecked;
                if (isChecked != null && isChecked.Value)
                {
                    index += 1; //Go to the next medicine
                    medicines.Remove(medicine);
                    continue;
                }

                RowAdder.AddRow(medicine, this, index + 1, OnClickCheckBox);
            }

            PrepareTable(medicines.Count);
        }

        private void PrepareTable(int count)
        {
            const int colCount = 8;
            int rowSpan = count > 28 ? 28 : count;

            for (int col = 0; col <= colCount; col += 2)
            {
                var verticalBorder = new Border();
                AddBorderLine(verticalBorder, col, 0, rowSpan, 1);
            }

            const int colSpan = 13;
            var horizontalBorder = new Border();
            AddBorderLine(horizontalBorder, 0, 0, 1, colSpan);
           
            PutTableHeader();
        }

        private void PutTableHeader()
        {
            RowAdder.AddTextBlockExternal(0, 0, "Item", DrugsGrid, HorizontalAlignment.Center, VerticalAlignment.Center);
            RowAdder.AddTextBlockExternal(1, 0, "Name", DrugsGrid, HorizontalAlignment.Center, VerticalAlignment.Center);
            RowAdder.AddTextBlockExternal(2, 0, "Date", DrugsGrid, HorizontalAlignment.Center, VerticalAlignment.Center);
            RowAdder.AddTextBlockExternal(3, 0, "Amount", DrugsGrid, HorizontalAlignment.Center, VerticalAlignment.Center);
            RowAdder.AddTextBlockExternal(4, 0, "Price", DrugsGrid, HorizontalAlignment.Center, VerticalAlignment.Center);
            RowAdder.AddTextBlockExternal(5, 0, "Picture", DrugsGrid, HorizontalAlignment.Center, VerticalAlignment.Center);
            RowAdder.AddTextBlockExternal(6, 0, "Item_Code", DrugsGrid, HorizontalAlignment.Center, VerticalAlignment.Center);
            RowAdder.AddTextBlockExternal(7, 0, "", DrugsGrid, HorizontalAlignment.Center, VerticalAlignment.Center);
        }

        private void AddBorderLine(Border border, int col, int row, int rowSpan, int colSpan)
        {
            Grid.SetRowSpan(border, rowSpan);
            Grid.SetColumnSpan(border, colSpan);
            Grid.SetRow(border, row);
            Grid.SetColumn(border, col);
            border.BorderThickness = rowSpan > 1 ? new Thickness(1, 0, 1, 0) : new Thickness(0, 1, 0, 1); //different thickness for vertical and horizontal lines
            border.BorderBrush = new SolidColorBrush(Colors.Gray);
            DrugsGrid.Children.Add(border);
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

        private void OnClickAddOrRemove(object sender, RoutedEventArgs e)
        {
            if (_countClicks == 0)
            {
                 Add popup = new Add(this);
                 popup.ShowDialog();
                 return;
            }

            _countClicks = 0;

            DrugsGrid.Children.Clear();

            PutMedicinesInTable();
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

                RowAdder.AddRow(medicine, this, medicines.Count, OnClickCheckBox);
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

        private void OnNfc(object sender, RoutedEventArgs e)
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

        internal static void OnClickCheckBox(object sender, RoutedEventArgs e)
        {
            var isChecked = ((CheckBox)sender).IsChecked;
            if (isChecked != null && isChecked.Value)
            {
                _countClicks += 1;
            }
            else if(_countClicks > 0)
            {
                _countClicks -= 1;
            }
        }

        private void InitializeDeltaAngles()
        {
            var medicineInfo = new MedicineInfo(new DeltaAngle(50, 33));
            _medicineInfoDict.Add(7290008086363, medicineInfo);

            medicineInfo = new MedicineInfo(new DeltaAngle(5, 39));
            _medicineInfoDict.Add(7290008004664, medicineInfo);

            medicineInfo = new MedicineInfo(new DeltaAngle(44, 37));
            _medicineInfoDict.Add(7290008546287, medicineInfo);

            medicineInfo = new MedicineInfo(new DeltaAngle(39, 38));
            _medicineInfoDict.Add(729000002988, medicineInfo);

            medicineInfo = new MedicineInfo(new DeltaAngle(36, 39));
            _medicineInfoDict.Add(729008872317, medicineInfo);

            medicineInfo = new MedicineInfo(new DeltaAngle(30, 39));
            _medicineInfoDict.Add(729000801650, medicineInfo);

            medicineInfo = new MedicineInfo(new DeltaAngle(23, 41));
            _medicineInfoDict.Add(729008546126, medicineInfo);

            medicineInfo = new MedicineInfo(new DeltaAngle(16, 40));
            _medicineInfoDict.Add(729008546003, medicineInfo);

            medicineInfo = new MedicineInfo(new DeltaAngle(13, 41));
            _medicineInfoDict.Add(729000806198, medicineInfo);

            medicineInfo = new MedicineInfo(new DeltaAngle(9, 40));
            _medicineInfoDict.Add(7290102062218, medicineInfo);

            medicineInfo = new MedicineInfo(new DeltaAngle(0, 39));
            _medicineInfoDict.Add(7290000810027, medicineInfo);
        }

        private static void JsonHandler()
        {
            string jsonFile = "C:\\Users\\admin\\Documents\\Visual Studio 2015\\Projects\\projectArduinoFirstTry\\medicine.json";
            var readAllText = File.ReadAllText(jsonFile);
            JavaScriptSerializer ser = new JavaScriptSerializer();
            _medicineList = ser.Deserialize<MedicineList>(readAllText);
        }
    }
}
