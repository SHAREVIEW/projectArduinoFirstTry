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
using System.Windows.Shapes;
using projectArduinoFirstTry.Sources;

namespace projectArduinoFirstTry
{
    /// <summary>
    /// Interaction logic for Add.xaml
    /// </summary>
    public partial class Add : Window
    {
        public Add()
        {
            InitializeComponent();
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnOk(object sender, RoutedEventArgs e)
        {
            var flowDocument = UsagesByUser.Document;
            var medicine = new Medicine("Acamol", DateTime.Now, 10);
           
            Medicine newMedicine;
            if (!MainWindow.Dict.TryGetValue(130, out newMedicine))
            {
                newMedicine = medicine;
                MainWindow.Dict.Add(130, newMedicine);
            }

            newMedicine.UserDesc = flowDocument;
        }
    }
}
