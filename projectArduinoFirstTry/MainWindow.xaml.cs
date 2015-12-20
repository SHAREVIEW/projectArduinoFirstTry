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

namespace projectArduinoFirstTry
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal static List<Medicine> Dict
        {
            get { return _medicineForDemo.MedicineVal; }
            set { _medicineForDemo.MedicineVal = value; }
        }
        private readonly SpeechRecognitionEngine _speechRecognizer = new SpeechRecognitionEngine();
        private bool _isMicEnabled = true;
        private IReceiverBluetoothService receiver = new ReceiverBluetoothService();
        

        public MainWindow()
        {
            InitializeComponent();
            
            JasonHandler();

            InitializeSpeechRecognizer();

            //BluetoothHandler.MakeConnection();
        }

        private void JasonHandler()
        {
            string jsonFile = "C:\\Users\\admin\\Documents\\Visual Studio 2015\\Projects\\projectArduinoFirstTry\\medicine.json";
            var readAllText = File.ReadAllText(jsonFile);
            JavaScriptSerializer ser = new JavaScriptSerializer();
            _medicineForDemo = ser.Deserialize<MedicineList>(readAllText);
        }

        private void InitializeSpeechRecognizer()
        {
            _speechRecognizer.SpeechRecognized += speechRecognizer_SpeechRecognized;

            GrammarBuilder grammarBuilder = new GrammarBuilder();

            Choices commandChoices = new Choices("medicine");

            grammarBuilder.Append(commandChoices);

            Choices valueChoices = new Choices();

            valueChoices.Add("Aspirine", "Akamol", "Omega3", "Zodorm");

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

            foreach (var medicine in _medicineForDemo.MedicineVal)
            {
                if (medicine.Name.ToLower() != medicineName)
                {
                    continue;
                }

                MedicineLbl.Content = medicineName.ToUpper();

                UsagesText.Text = medicine.UserDesc;

                DangersText.Text = medicine.DangersDesc;

                BitmapImage bitImage = new BitmapImage();
                bitImage.BeginInit();
                bitImage.UriSource = new Uri(medicine.ImagePath);
                bitImage.EndInit();

                ImageContent.Source = bitImage;
                break;
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

        private static Dictionary<long, Medicine> _dict;
        private static MedicineList _medicineForDemo;

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            PutMedicine();
        }

        private void PutMedicine()
        {
            foreach (var medicine in _medicineForDemo.MedicineVal)
            {
                var medicineVal = medicine;
                RowAdder.AddRow(medicineVal, DrugsGrid);
            }

            ExpandColums();
        }

        internal  void ExpandColums()
        {
            Grid.SetRowSpan(Col1, RowAdder.RowSpan);
            Grid.SetRowSpan(Col2, RowAdder.RowSpan);
            Grid.SetRowSpan(Col3, RowAdder.RowSpan);
            Grid.SetRowSpan(Col4, RowAdder.RowSpan);
        }

        private void OnClickMice(object sender, RoutedEventArgs e)
        {
//            SpeechRecognizer speechRecognizer = new SpeechRecognizer();
            if (_isMicEnabled)
            {
                _speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
                _isMicEnabled = false;
                return;
            }

            _isMicEnabled = true;
            _speechRecognizer.RecognizeAsyncStop();
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
    }
}
