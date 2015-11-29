using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace projectArduinoFirstTry.Sources
{
    internal static class RowAdder
    {
        public static void AddRow(string medicineName, int code, DateTime dateTime, Grid grid)
        {
            _grid = grid;

            int col = 0;

            AddTextBlock(col, _rowCount, code.ToString());
            AddTextBlock(++col, _rowCount,medicineName);
            AddTextBlock(++col, _rowCount,dateTime.ToShortDateString());

            _rowCount += 1;
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
            textBlock.Padding = new Thickness(10,0,10,0);

            _grid.Children.Add(textBlock);
        }

        private static void AddPic(int col, int row, string imagePath)
        {
            
        }

        private static Grid _grid;
        static private int _rowCount = 1;
    }
}
