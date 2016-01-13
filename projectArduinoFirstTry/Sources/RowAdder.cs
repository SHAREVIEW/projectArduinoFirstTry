using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace projectArduinoFirstTry.Sources
{
    internal static class RowAdder
    {
        public static void AddRow(Medicine medicine, Grid grid)
        {
            _grid = grid;

            int col = 0;

            AddTextBlock(col, _rowCount, medicine.Code.ToString());
            AddTextBlock(++col, _rowCount, medicine.Name);
            AddTextBlock(++col, _rowCount, medicine.Date.ToShortDateString());

            if(File.Exists(medicine.ImagePath))
                AddImage(5, _rowCount, medicine.ImagePath);

            _rowCount += 1;

            if (_rowCount >= RowSpan)
            {
                RowSpan += 1;
                AddLineInTable();
            }
        }

        public static void AddRow(Medicine medicine)
        {
            if (_grid == null)
            {
                return;
            }

            AddRow(medicine, _grid);
        }

        private static void AddLineInTable()
        {
            //Add border
            var border = new Border();
            Grid.SetRow(border, RowSpan);
            Grid.SetColumn(border, 0);
            Grid.SetColumnSpan(border, 13);

            //Add row definition
            var row = new RowDefinition();
            row.Height = GridLength.Auto;
            _grid.RowDefinitions.Add(row);
            border.BorderThickness = new Thickness(1, 1, 1, 1);
            border.BorderBrush = new SolidColorBrush(Color.FromRgb(128, 128, 128));

            _grid.Children.Add(border);
        }

        private static void AddTextBlock(int col, int row, string text)
        {
            var textBlock = new TextBlock();
            textBlock.Text = text;
            Grid.SetRow(textBlock, row);
            Grid.SetColumn(textBlock, col);
            textBlock.HorizontalAlignment = HorizontalAlignment.Left;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.FontSize = 16;
            textBlock.Foreground = Brushes.White;
            textBlock.Padding = new Thickness(10, 0, 10, 0);

            _grid.Children.Add(textBlock);
        }

        private static void AddImage(int col, int row, string imagePath)
        {
            //Set Image Source
            Image image = new Image();
            image.Height = 40;
            image.Width = 80;
            
            BitmapImage bitImage = new BitmapImage();
            bitImage.BeginInit();
            bitImage.UriSource = new Uri(imagePath);
            bitImage.EndInit();

            image.Source = bitImage;

            //Put image inside a container
            InlineUIContainer inlineUiContainer = new InlineUIContainer();
            inlineUiContainer.Child = image;

            //New Text Block
            TextBlock textBlock = new TextBlock();
            textBlock.Inlines.Add(inlineUiContainer);
            Grid.SetRow(textBlock, row);
            Grid.SetColumn(textBlock, col);
            textBlock.Padding = new Thickness(10, 10, 10, 10);
            textBlock.TextAlignment = TextAlignment.Center;
            _grid.Children.Add(textBlock);
        }

        public static int RowSpan = 1;
        private static Grid _grid;
        static private int _rowCount = 1;
    }
}
