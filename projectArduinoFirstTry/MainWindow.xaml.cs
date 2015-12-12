using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using projectArduinoFirstTry.Sources;
using Microsoft.WindowsAzure.MobileServices;

namespace projectArduinoFirstTry
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal static Dictionary<long, Medicine> Dict
        {
            get { return _dict; }
            set { _dict = value; }
        }

        public MainWindow()
        {
            InitializeComponent();

            _dict = new Dictionary<long, Medicine>
            {
                {1, new Medicine("Acamol", new DateTime(10, 10, 10), 1)},
                {2, new Medicine("Aspirine", new DateTime(10, 10, 10), 2)},
                {3, new Medicine("Zodorom", new DateTime(10, 10, 10), 3)},
                {4, new Medicine("Ritalin", new DateTime(10, 10, 10), 4)},
                {5, new Medicine("Omega3", new DateTime(10, 10, 10), 5)},
                {6, new Medicine("Acamol", new DateTime(10, 10, 10), 1)},
                {7, new Medicine("Aspirine", new DateTime(10, 10, 10), 2)},
                {8, new Medicine("Zodorom", new DateTime(10, 10, 10), 3)},
                {9, new Medicine("Ritalin", new DateTime(10, 10, 10), 4)},
                {10, new Medicine("Omega3", new DateTime(10, 10, 10), 5)},
                {11, new Medicine("Acamol", new DateTime(10, 10, 10), 1)},
                {12, new Medicine("Aspirine", new DateTime(10, 10, 10), 2)},
                {13, new Medicine("Zodorom", new DateTime(10, 10, 10), 3)},
                {14, new Medicine("Ritalin", new DateTime(10, 10, 10), 4)},
                {15, new Medicine("Omega3", new DateTime(10, 10, 10), 5)},
                {16, new Medicine("Acamol", new DateTime(10, 10, 10), 1)},
                {17, new Medicine("Aspirine", new DateTime(10, 10, 10), 2)},
                {18, new Medicine("Zodorom", new DateTime(10, 10, 10), 3)},
                {19, new Medicine("Ritalin", new DateTime(10, 10, 10), 4)},
                {20, new Medicine("Omega3", new DateTime(10, 10, 10), 5)},
                {21, new Medicine("Omega3", new DateTime(10, 10, 10), 5)},
                {22, new Medicine("Omega3", new DateTime(10, 10, 10), 5)},
                {23, new Medicine("Omega3", new DateTime(10, 10, 10), 5)},
                {24, new Medicine("Omega3", new DateTime(10, 10, 10), 5)},
                {25, new Medicine("Omega3", new DateTime(10, 10, 10), 5)},
                {26, new Medicine("Omega3", new DateTime(10, 10, 10), 5)},
                {27, new Medicine("Omega3", new DateTime(10, 10, 10), 5)},
                {28, new Medicine("Omega3", new DateTime(10, 10, 10), 5)},
                {29, new Medicine("Omega3", new DateTime(10, 10, 10), 5)},
                {30, new Medicine("Omega3", new DateTime(10, 10, 10), 5)},
                {31, new Medicine("Omega3", new DateTime(10, 10, 10), 5)},
                {32, new Medicine("Omega3", new DateTime(10, 10, 10), 5)},
                {33, new Medicine("Omega3", new DateTime(10, 10, 10), 5)},
                {34, new Medicine("Omega3", new DateTime(10, 10, 10), 5)},
                {35, new Medicine("Omega3", new DateTime(10, 10, 10), 5)},
                {36, new Medicine("Omega3", new DateTime(10, 10, 10), 5)},
                {37, new Medicine("Omega3", new DateTime(10, 10, 10), 5)},
                {38, new Medicine("Omega3", new DateTime(10, 10, 10), 5)},
                {39, new Medicine("Omega3", new DateTime(10, 10, 10), 5)},
                {40, new Medicine("Omega3", new DateTime(10, 10, 10), 5)},
                {41, new Medicine("Omega3", new DateTime(10, 10, 10), 5)},
                {42, new Medicine("Omega3", new DateTime(10, 10, 10), 5)},
            };

            CloudInteract();
        }

        private async Task CloudInteract()
        {
            int count = 1;
            foreach (var medicine in _dict)
            {
                var medicineVal = medicine.Value;

                Console.WriteLine(count);
                
                await App.MobileService.GetTable<Medicine>().InsertAsync(medicineVal);

                count += 1;
            }
        }

        private static Dictionary<long, Medicine> _dict;

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            PutMedicine();
        }

        private void PutMedicine()
        {
            foreach (var medicine in _dict)
            {
                var medicineVal = medicine.Value;
                RowAdder.AddRow(medicineVal.Name, (int) medicineVal.Code, medicineVal.Date, DrugsGrid);
            }

            ExpandColums();
        }

        private void ExpandColums()
        {
            Grid.SetRowSpan(Col1, RowAdder.RowSpan);
            Grid.SetRowSpan(Col2, RowAdder.RowSpan);
            Grid.SetRowSpan(Col3, RowAdder.RowSpan);
            Grid.SetRowSpan(Col4, RowAdder.RowSpan);
        }

        private void OnClickMice(object sender, RoutedEventArgs e)
        {
            
        }

        private void OnClickAdd(object sender, RoutedEventArgs e)
        {
            Add popup = new Add();
            popup.ShowDialog();
        }
    }
}
