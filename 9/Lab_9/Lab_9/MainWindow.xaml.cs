using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lab_9
{
    public partial class MainWindow : Window
    {
        private Combinational comb;
        private Register reg;

        private Element[] elements;

        public MainWindow()
        {
            InitializeComponent();

            comb = new Combinational();
            reg = new Register();

            // Демонстрация массива элементов базового класса
            elements = new Element[] { comb, reg };

            // Инициализация отображения состояния регистра
            RegisterStateTextBlock.Text = string.Join("", reg.GetOutputs().Select(b => b ? "1" : "0"));
        }

        private void CalcCombElementButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var parts = CombInputsTextBox.Text.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != comb.InputsCount)
                {
                    MessageBox.Show($"Введите {comb.InputsCount} значений (0/1) через пробел.");
                    return;
                }

                bool[] vals = parts.Select(x => x == "1").ToArray();

                comb.SetInputs(vals);
                CombResultTextBlock.Text = comb.CalculateOutput() ? "1" : "0";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void InvertCombElementButton_Click(object sender, RoutedEventArgs e)
        {
            elements[0].Invert();
            CombInputsTextBox.Text = string.Join(" ", Enumerable.Range(0, comb.InputsCount).Select(i => comb.GetInput(i) ? "1" : "0"));
            CombResultTextBlock.Text = comb.CalculateOutput() ? "1" : "0";
        }

        private void UpdateRegisterButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyRegisterInputs(TInputsTextBox, SInputsTextBox, RInputsTextBox, reg, false); // Передаем false, чтобы не вызывать UpdateState

            elements[1].Activate();

            RegisterStateTextBlock.Text = string.Join("", reg.GetOutputs().Select(b => b ? "1" : "0"));
        }

        private void ResetRegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Сброс через R=1 для всех триггеров
            bool[] zeros = new bool[10];
            bool[] ones = Enumerable.Repeat(true, 10).ToArray();

            // R=1, T=0, S=0
            reg.SetInputs(zeros, zeros, ones);
            reg.UpdateState();

            reg.SetInputs(zeros, zeros, zeros);

            RegisterStateTextBlock.Text = string.Join("", reg.GetOutputs().Select(b => b ? "1" : "0"));
        }

        private void InvertRegisterButton_Click(object sender, RoutedEventArgs e)
        {
            elements[1].Invert();
            RegisterStateTextBlock.Text = string.Join("", reg.GetOutputs().Select(x => x ? "1" : "0"));
        }

        private void ShiftRegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(ShiftValueTextBox.Text, out int shift))
            {
                reg.Shift(shift);
                RegisterStateTextBlock.Text = string.Join("", reg.GetOutputs().Select(x => x ? "1" : "0"));
            }
            else
            {
                MessageBox.Show("Введите корректное число для сдвига.");
            }
        }

        private void SaveRegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog { Filter = "Binary File|*.bin" };

            if (dlg.ShowDialog() == true)
            {
                Serializer.Save(dlg.FileName, reg);
                MessageBox.Show("Сохранено.");
            }
        }

        private void LoadRegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog { Filter = "Binary File|*.bin" };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    reg = (Register)Serializer.Load(dlg.FileName);
                    elements[1] = reg;
                    RegisterStateTextBlock.Text = string.Join("", reg.GetOutputs().Select(b => b ? "1" : "0"));
                    MessageBox.Show("Загружено.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки: {ex.Message}");
                }
            }
        }

        // Добавлен флаг executeUpdate для управления вызовом UpdateState
        private void ApplyRegisterInputs(TextBox tBox, TextBox sBox, TextBox rBox, Register targetReg, bool executeUpdate = true)
        {
            // Корректное применение 10-битных S и R
            if (tBox.Text.Length != 10 || sBox.Text.Length != 10 || rBox.Text.Length != 10)
            {
                MessageBox.Show("Введите 10 символов для T, S, R");
                return;
            }

            try
            {
                bool[] T = tBox.Text.Select(c => c == '1').ToArray();
                bool[] S = sBox.Text.Select(c => c == '1').ToArray();
                bool[] R = rBox.Text.Select(c => c == '1').ToArray();

                // Устанавливаем входы S, R, T
                targetReg.SetInputs(T, S, R);

                if (executeUpdate)
                {
                    targetReg.UpdateState();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка ввода: {ex.Message}");
            }
        }

        private void CompareRegistersButton_Click(object sender, RoutedEventArgs e)
        {
            Register reg1 = new Register();
            Register reg2 = new Register();

            // ввод, который обновляет состояние регистров (executeUpdate = true по умолчанию)
            ApplyRegisterInputs(Comp1TInput, Comp1SInput, Comp1RInput, reg1);
            ApplyRegisterInputs(Comp2TInput, Comp2SInput, Comp2RInput, reg2);

            // Сравнение теперь использует корректно переопределенный Register.Equals.
            CompareResultTextBlock.Text = reg1.Equals(reg2) ? "Регистры равны" +
                "" : "Регистры не равны";
        }

        private void RegisterInputsTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешены только '0' и '1'
            e.Handled = !(e.Text == "0" || e.Text == "1");
        }

        private void CombInputsTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешены '0', '1' и ' '
            e.Handled = !(e.Text == "0" || e.Text == "1" || e.Text == " ");
        }

        private void ShiftInputTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешены только цифры
            e.Handled = !int.TryParse(e.Text, out _);
        }
    }
}