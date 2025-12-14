using System;

namespace Lab_9
{
    [Serializable]
    public abstract class Element : IInvertible
    {
        // Закрытые поля
        private readonly string name;
        private int inputsCount;
        private int outputsCount;

        // Свойства
        public string Name => name; // Только чтение

        public int InputsCount
        {
            get => inputsCount;
            protected set => inputsCount = Math.Max(0, value);
        }

        public int OutputsCount
        {
            get => outputsCount;
            protected set => outputsCount = Math.Max(0, value);
        }

        // Конструктор по умолчанию
        protected Element() : this("Unknown", 1, 1) { }

        // Конструктор, задающий имя и устанавливающий равным 1 входы/выходы
        protected Element(string name) : this(name, 1, 1) { }

        // Конструктор, задающий значения всех полей
        protected Element(string name, int inputs, int outputs)
        {
            this.name = name ?? "Unknown";
            InputsCount = inputs;
            OutputsCount = outputs;
        }

        public virtual void Activate() { }

        // Абстрактный метод 
        public abstract void Invert();

        // Переопределение Equals
        public override bool Equals(object obj)
        {
            return obj is Element other &&
                   Name == other.Name &&
                   InputsCount == other.InputsCount &&
                   OutputsCount == other.OutputsCount;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + (Name?.GetHashCode() ?? 0);
            hash = hash * 23 + InputsCount.GetHashCode();
            hash = hash * 23 + OutputsCount.GetHashCode();
            return hash;
        }
    }
}