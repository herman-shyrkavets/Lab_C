using System;
using System.Linq;

namespace Lab_9
{
    /// Комбинационный элемент AND
    [Serializable]
    public class Combinational : Element, IInvertible
    {
        private bool[] inputs;

        // конструктор
        public Combinational(string name = "AND", int inputsCount = 4, int outputsCount = 1)
            : base(name, inputsCount, outputsCount)
        {
            inputs = new bool[InputsCount];
        }

        // Конструктор копирования
        public Combinational(Combinational other)
            : base(other.Name, other.InputsCount, other.OutputsCount)
        {
            inputs = new bool[InputsCount];
            Array.Copy(other.inputs, inputs, InputsCount);
        }

        // Установка входов
        public void SetInputs(bool[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (values.Length != InputsCount)
                throw new ArgumentException($"Требуется {InputsCount} входов.");

            Array.Copy(values, inputs, InputsCount);
        }

        // Получение значения конкретного входа
        public bool GetInput(int index)
        {
            if (index < 0 || index >= InputsCount)
                throw new IndexOutOfRangeException("Некорректный индекс входа.");

            return inputs[index];
        }

        // Вычисление выхода (логическое И всех входов)
        public bool CalculateOutput()
        {
            bool result = true;
            foreach (var val in inputs)
                result &= val;

            return result;
        }

        public override void Activate()
        {
            _ = CalculateOutput();
        }

        // Переопределение Invert()
        public override void Invert()
        {
            for (int i = 0; i < inputs.Length; i++)
                inputs[i] = !inputs[i];
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Combinational other)) return false;
            if (!base.Equals(other)) return false;

            // Сравнение входов
            for (int i = 0; i < InputsCount; i++)
                if (inputs[i] != other.inputs[i])
                    return false;

            return true;
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            foreach (var v in inputs)
                hash = hash * 23 + v.GetHashCode();
            return hash;
        }
    }
}