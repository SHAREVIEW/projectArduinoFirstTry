using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.Win32;
using projectArduinoFirstTry.Sources;

namespace projectArduinoFirstTry
{
    /// <summary>
    /// Interaction logic for Add.xaml
    /// </summary>
    public partial class Add : Window
    {
        private MainWindow _mainWindow;

        public Add(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            InitializeComponent();
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnOk(object sender, RoutedEventArgs e)
        {
            var medicineName = MedicineName.Text;
            var medicineCode = string.IsNullOrEmpty(MedicineCode.Text) ? 0 : long.Parse(MedicineCode.Text);
            var medicineDate = MedicineDate.SelectedDate ?? MedicineDate.DisplayDate;
            Medicine medicine = new Medicine(medicineName, medicineDate, medicineCode);

            medicine.ImagePath = txtEditor.Text;

            medicine.UserDesc = new TextRange(UsagesByUser.Document.ContentStart, UsagesByUser.Document.ContentEnd).Text;
            medicine.DangersDesc = new TextRange(DangersByUser.Document.ContentStart, DangersByUser.Document.ContentEnd).Text;

            List<Medicine> medicines = MainWindow.Dict;
            Predicate<Medicine> medicineFinder = (Medicine m) => { return m.Code == medicineCode; };
            if (medicines.Exists(medicineFinder))
            {
                _mainWindow.UpdateCounter(medicine, _mainWindow);

                return;
            }

            medicines.Add(medicine);

            RowAdder.AddRow(medicine, _mainWindow, medicines.Count);
        }
        
        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                txtEditor.Text = openFileDialog.FileName;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}
