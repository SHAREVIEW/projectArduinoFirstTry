using System;
using System.Collections.Generic;
using System.Linq;
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

namespace projectArduinoFirstTry
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            _dict = new Dictionary<long, Medicine>
            {
                {1, new Medicine("Acamol", new DateTime(10, 10, 10), 1)},
                {2, new Medicine("Aspirine", new DateTime(10, 10, 10), 2)},
                {3, new Medicine("Zodorom", new DateTime(10, 10, 10), 3)},
                {4, new Medicine("Ritalin", new DateTime(10, 10, 10), 4)},
                {5, new Medicine("Omega3", new DateTime(10, 10, 10), 5)}
            };
        }

        private Dictionary<long, Medicine> _dict;

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
        }

        private void OnClickMice(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
