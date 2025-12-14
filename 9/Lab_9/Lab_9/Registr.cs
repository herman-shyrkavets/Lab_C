using System;
using System.Linq;

namespace Lab_9
{
    [Serializable]
    public class Register : Element, IInvertible, IShiftable
    {
        private const int Size = 10;

        // Вложенный класс
        [Serializable]
        public class Memory : Element, IInvertible
        {
            // Входы: [0] = T, [1] = Set, [2] = Reset
            private bool[] inputs;
            private bool q; // Состояние на выходе триггера

            public Memory() : base("T-Trigger", 3, 2)
            {
                inputs = new bool[3];
                q = false; // По умолчанию сбрасывает экземпляр
            }

            public Memory(Memory other) : base(other.Name, other.InputsCount, other.OutputsCount)
            {
                inputs = new bool[3];
                Array.Copy(other.inputs, inputs, 3);
                q = other.q;
            }

            // Метод, задающий значение на входах
            public void SetInputs(bool T, bool Set, bool Reset)
            {
                inputs[0] = T;
                inputs[1] = Set;
                inputs[2] = Reset;
            }

            // Опрашивание состояния отдельного входа
            public bool GetInput(int index)
            {
                if (index < 0 || index >= 3) throw new IndexOutOfRangeException("Некорректный индекс входа (0:T, 1:Set, 2:Reset).");
                return inputs[index];
            }

            // Метод, вычисляющий состояние (синхронный T-триггер с асинхронными Set/Reset)
            public void CalculateState()
            {
                // Приоритет Set/Reset
                if (inputs[1] && inputs[2])
                {
                    // Состояние неопределенности (Set/Reset = 1), оставим текущее состояние
                }
                else if (inputs[1]) // Set
                {
                    q = true;
                }
                else if (inputs[2]) // Reset
                {
                    q = false;
                }
                else if (inputs[0]) // T=1 -> инверсия
                {
                    q = !q;
                }
                // Если T=0, S=0, R=0, состояние сохраняется
            }

            //доступ к текущему состоянию только для чтения
            public bool QOutput => q;
            public bool QnotOutput => !q;


            // Переопределение Invert()
            public override void Invert()
            {
                q = !q;
                for (int i = 0; i < inputs.Length; i++)
                    inputs[i] = !inputs[i];
            }

            // Переопределение Equals для сравнения значений
            public override bool Equals(object obj)
            {
                if (!(obj is Memory other)) return false;
                if (!base.Equals(other)) return false;
                if (q != other.q) return false;

                for (int i = 0; i < inputs.Length; i++)
                    if (inputs[i] != other.inputs[i])
                        return false;

                return true;
            }

            public override int GetHashCode()
            {
                int hash = base.GetHashCode() * 23 + q.GetHashCode();
                foreach (var v in inputs) hash = hash * 23 + v.GetHashCode();
                return hash;
            }
        }

        // Поля класса Register
        private Memory[] memoryArray;

        public Register() : base("Register", Size, Size)
        {
            memoryArray = new Memory[Size];
            for (int i = 0; i < Size; i++)
                memoryArray[i] = new Memory();
        }

        public Register(Register other) : base(other.Name, other.InputsCount, other.OutputsCount)
        {
            memoryArray = new Memory[Size];
            for (int i = 0; i < Size; i++)
                memoryArray[i] = new Memory(other.memoryArray[i]);
        }

        // Метод, задающий значение на входах экземпляра класса (T, S, R побитно)
        public void SetInputs(bool[] TInputs, bool[] SInputs, bool[] RInputs)
        {
            if (TInputs.Length != Size || SInputs.Length != Size || RInputs.Length != Size)
                throw new ArgumentException($"Требуется {Size} значений для T, S и R.");

            for (int i = 0; i < Size; i++)
            {
                memoryArray[i].SetInputs(TInputs[i], SInputs[i], RInputs[i]);
            }
        }

        public void UpdateState()
        {
            foreach (var mem in memoryArray)
            {
                mem.CalculateState();
            }
        }

        public bool[] GetOutputs()
        {
            bool[] outputs = new bool[Size];
            for (int i = 0; i < Size; i++)
                outputs[i] = memoryArray[i].QOutput;
            return outputs;
        }

        // Переопределение Invert()
        public override void Invert()
        {
            foreach (var mem in memoryArray)
                mem.Invert();
        }

        // сдвиг
        public void Shift(int bits)
        {
            bits %= Size;
            if (bits == 0) return;

            bool[] outputs = GetOutputs();
            bool[] shifted = new bool[Size];

            // Расчет циклического сдвига вправо (если bits > 0) или влево (если bits < 0)
            for (int i = 0; i < Size; i++)
                shifted[i] = outputs[(i - bits + Size) % Size];

            // Обновление состояния регистров в соответствии со сдвигом
            for (int i = 0; i < Size; i++)
            {
                bool set = shifted[i];
                bool reset = !shifted[i];

                // Устанавливаем S/R в Memory, T = false
                memoryArray[i].SetInputs(false, set, reset);
                memoryArray[i].CalculateState();

                // Сбрасываем входы S/R обратно на '0'
                memoryArray[i].SetInputs(memoryArray[i].GetInput(0), false, false);
            }
        }

        public override void Activate() => UpdateState();


        // Переопределение Equals в Register
        public override bool Equals(object obj)
        {
            if (!(obj is Register other)) return false;
            if (!base.Equals(other)) return false;

            // Сравнение состояний триггеров
            for (int i = 0; i < Size; i++)
            {
                if (!memoryArray[i].Equals(other.memoryArray[i]))
                    return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            foreach (var mem in memoryArray)
                hash = hash * 23 + mem.GetHashCode();
            return hash;
        }
    }
}