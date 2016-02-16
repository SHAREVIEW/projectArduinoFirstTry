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
        public static void AddRow(Medicine medicine, MainWindow mainWindow, int index, RoutedEventHandler onClickCheckBox)
        {
            _mainWindow = mainWindow;
            _grid = mainWindow.DrugsGrid;

            int col = 0;

            AddTextBlock(col, _rowCount, index.ToString());
            AddTextBlock(++col, _rowCount, medicine.Name);
            AddTextBlock(++col, _rowCount, medicine.Date.ToShortDateString());
            AddCountTextBlock(++col, _rowCount, "1", $"count_{medicine.Code}");
            AddTextBlock(++col, _rowCount, $"{medicine.Price}$");

            if (File.Exists(medicine.ImagePath))
            {
                AddImage(++col, _rowCount, medicine.ImagePath);
            }

            AddTextBlock(++col, _rowCount, medicine.Code.ToString());
            AddCheckBox(++col, _rowCount, $"check_{medicine.Code}", onClickCheckBox);

            _rowSpan += 1;

            _rowCount += 1;

            AddLineInTable();

            ExpandColums();
        }

        private static void AddLineInTable()
        {
            //Add border
            var border = new Border();
            Grid.SetRow(border, _rowSpan);
            Grid.SetColumn(border, 0);
            Grid.SetColumnSpan(border, 13);

            //Add row definition
            var row = new RowDefinition {Height = GridLength.Auto};
            _grid.RowDefinitions.Add(row);

            border.BorderThickness = new Thickness(0,0,0,1);

            for (int col = 0; col <= 8; col += 2)
            {
                var verticalBorder = new Border();
                AddBorderLine(verticalBorder, col, _rowSpan);
            }

            border.BorderBrush = new SolidColorBrush(Color.FromRgb(128, 128, 128));

            _grid.Children.Add(border);
        }

        private static void AddBorderLine(Border border, int col, int row)
        {
            Grid.SetRow(border, row);
            Grid.SetColumn(border, col);
            border.BorderThickness = new Thickness(1, 0, 1, 0); //different thickness for vertical and horizontal lines
            border.BorderBrush = new SolidColorBrush(Colors.Gray);
            _grid.Children.Add(border);
        }
        private static void ExpandColums()
        {
            Grid.SetRowSpan(_mainWindow.Col1, _rowSpan);
            Grid.SetRowSpan(_mainWindow.Col2, _rowSpan);
            Grid.SetRowSpan(_mainWindow.Col3, _rowSpan);
            Grid.SetRowSpan(_mainWindow.Col4, _rowSpan);
        }

        private static void AddTextBlock(int col, int row, string text)
        {
            var textBlock = new TextBlock();

            AddTextBlockAux(col, row, text, textBlock, HorizontalAlignment.Left, VerticalAlignment.Center);

            _grid.Children.Add(textBlock);
        }

        internal static void AddTextBlockExternal(int col, int row, string text, Grid grid, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
        {
            var textBlock = new TextBlock();

            AddTextBlockAux(col, row, text, textBlock, horizontalAlignment, verticalAlignment);

            grid.Children.Add(textBlock);
        }

        private static void AddCheckBox(int col, int row, string name, RoutedEventHandler onClickCheckBox)
        {
            var checkBox = new CheckBox();

            if (!string.IsNullOrEmpty(name))
            {
                checkBox.Name = name;
            }

            checkBox.Click += onClickCheckBox;
            checkBox.HorizontalAlignment = HorizontalAlignment.Center;
            checkBox.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(checkBox, row);
            Grid.SetColumn(checkBox, col);
            _grid.Children.Add(checkBox);

            try
            {
                _grid.RegisterName(checkBox.Name, checkBox);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private static void AddTextBlockAux(int col, int row, string text, TextBlock textBlock, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
        {
            textBlock.Text = text;
            Grid.SetRow(textBlock, row);
            Grid.SetColumn(textBlock, col);
            textBlock.HorizontalAlignment = horizontalAlignment;
            textBlock.VerticalAlignment = verticalAlignment;
            textBlock.FontSize = 16;
            textBlock.Foreground = Brushes.White;
            textBlock.Padding = new Thickness(10, 0, 10, 0);
        }

        private static void AddCountTextBlock(int col, int row, string text, string name)
        {
            var textBlock = new TextBlock();

            AddTextBlockAux(col, row, text, textBlock, HorizontalAlignment.Left, VerticalAlignment.Center);

            if (!string.IsNullOrEmpty(name))
            {
                textBlock.Name = name;
                textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            }

            _grid.Children.Add(textBlock);

            try
            {
                if (name != null) _grid.RegisterName(name, textBlock);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private static void AddImage(int col, int row, string imagePath)
        {
            //Set Image Source
            var image = new Image
            {
                Height = 40,
                Width = 80
            };

            var bitImage = new BitmapImage();
            bitImage.BeginInit();
            bitImage.UriSource = new Uri(imagePath);
            bitImage.EndInit();

            image.Source = bitImage;

            //Put image inside a container
            var inlineUiContainer = new InlineUIContainer {Child = image};

            //New Text Block
            var textBlock = new TextBlock();
            textBlock.Inlines.Add(inlineUiContainer);
            Grid.SetRow(textBlock, row);
            Grid.SetColumn(textBlock, col);
            textBlock.Padding = new Thickness(10, 10, 10, 10);
            textBlock.TextAlignment = TextAlignment.Center;
            _grid.Children.Add(textBlock);
        }
        
        private static int _rowSpan;
        private static Grid _grid;
        private static int _rowCount = 1;
        private static MainWindow _mainWindow;
    }
}
