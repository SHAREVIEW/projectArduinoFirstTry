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
using System.Threading;
using System.Windows.Media;

namespace projectArduinoFirstTry
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        internal static List<Medicine> Dict
        {
            get { return _medicineForDemo.MedicineVal; }
            set { _medicineForDemo.MedicineVal = value; }
        }
        private readonly SpeechRecognitionEngine _speechRecognizer = new SpeechRecognitionEngine();
        private bool _isMicEnabled = true;
        private CloudStorageAccount _storageAccount = null;
        private CloudTableClient tableClient;
        private CloudTable table;
        private static MedicineList _medicineForDemo;
        
        public MainWindow()
        {
            InitializeComponent();
            
            JasonHandler();

            InitializeSpeechRecognizer();

            //BluetoothHandler.MakeConnection("NFC Scanner");
            //BluetoothHandler.MakeConnection("Galaxy Note5");

            //InitializeAzureStorage();

            //InsertToTable();

            //ReadFromTable();

            //DeleteEntryFromTable();
        }

        private void InitializeAzureStorage()
        {
            _storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            // Create the table client.
            tableClient = _storageAccount.CreateCloudTableClient();

            // Create the table if it doesn't exist.
            table = tableClient.GetTableReference("medicine");
            table.CreateIfNotExists();
        }

        public void InsertToTable()
        {
            MedicineEntity medicine1 = new MedicineEntity("9721356", "Omega3");
            medicine1.Date = "12/4/3";
            medicine1.DangersDesc = "Sivan U R the best. ";
            medicine1.UserDesc = "Sivan U gonna be a star <3";

            // Create the TableOperation object that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(medicine1);

            // Execute the insert operation.
            table.Execute(insertOperation);
        }

        public void ReadFromTable()
        {
            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<MedicineEntity> query = new TableQuery<MedicineEntity>();//.Where(/*TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Smith")*/);

            // Print the fields for each customer.
            foreach (MedicineEntity entity in table.ExecuteQuery(query))
            {
                Console.WriteLine("{0}, {1}\t{2}\t{3}\t{4}", entity.PartitionKey, entity.RowKey,
                    entity.Date, entity.UserDesc, entity.DangersDesc);
            }

        }

        private void DeleteEntryFromTable()
        {
            // Create a retrieve operation that expects a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<MedicineEntity>("Omega3", "9721356");

            // Execute the operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            // Assign the result to a CustomerEntity.
            MedicineEntity deleteEntity = (MedicineEntity) retrievedResult.Result;

            // Create the Delete TableOperation.
            if (deleteEntity != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);
                // Execute the operation.
                table.Execute(deleteOperation);
                Console.WriteLine("Entity deleted.");
            }
            else
                Console.WriteLine("Could not retrieve the entity.");
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

        public void Dispose()
        {
            BluetoothHandler.Close();
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            try
            {
                while (Laser.IsChecked != null && !Laser.IsChecked.Value)
                {
                    BluetoothHandler.GetStrFromBluetooth();
                }
            }
            catch (System.Security.Cryptography.CryptographicException cryptographicException)
            {
                Console.WriteLine(cryptographicException);
                Thread.Sleep(1000);
            }

            MessageBox.Show("Told ya");
        }

        private void onNFC(object sender, RoutedEventArgs e)
        {
            BluetoothHandler.Close();
            BluetoothHandler.MakeConnection("NFC Scanner");
            bluetoothIndicator.Fill = !BluetoothHandler.IsConnected() ? new SolidColorBrush( Colors.Red) : new SolidColorBrush(Colors.Green);
        }

        private void OnLaser(object sender, RoutedEventArgs e)
        {
            BluetoothHandler.Close();
            BluetoothHandler.MakeConnection("Galaxy Note5");
            if (!BluetoothHandler.IsConnected())
            {
                bluetoothIndicator.Fill = new SolidColorBrush(Colors.Red);
            }
            else
            {
                bluetoothIndicator.Fill = new SolidColorBrush(Colors.Green);
            }
        }
    }
}
