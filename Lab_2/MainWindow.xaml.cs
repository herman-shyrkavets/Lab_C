using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;

namespace GraphFunctionApp
{
    public class PointData
    {
        public double X { get; set; }
        public string Y { get; set; } // теперь строка
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private double Func(double x)
        {
            if (x <= -7 || x >= 4)
                return 0;
            else if (x > -7 && x < -3)
                return x + 7;
            else if (x >= -3 && x <= -2)
                return 4;
            else if (x > -2 && x < 2)
                return x * x;
            else if (x >= 2 && x <= 4)
                return -2 * x + 8;
            return 0;
        }

        private void btnCalc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!double.TryParse(txtX1.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double x1) ||
                    !double.TryParse(txtX2.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double x2) ||
                    !double.TryParse(txtH.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double h))
                {
                    MessageBox.Show("Ошибка: введите числа!");
                    return;
                }

                if (h <= 0)
                {
                    MessageBox.Show("Шаг должен быть > 0");
                    return;
                }

                if (x1 >= x2)
                {
                    MessageBox.Show("X1 должен быть меньше X2");
                    return;
                }

                var table = new List<PointData>();
                for (double x = x1; x <= x2; x += h)
                {
                    string yValue;

                    if (x > 7)
                    {
                        yValue = "вне интервала";
                    }
                    else if (x < -9)
                    {
                        yValue = "вне интервала";
                    }
                    else
                    {
                        yValue = Func(x).ToString("0.##");
                    }

                    table.Add(new PointData { X = Math.Round(x, 2), Y = yValue });
                }

                dataGrid.ItemsSource = table;
            }
            catch
            {
                MessageBox.Show("Ошибка при вычислении!");
            }
        }
    }
}
