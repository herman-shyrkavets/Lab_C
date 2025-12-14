using System;

namespace Lab_9
{
    public interface IInvertible
    {
        void Invert();
    }

    /// Интерфейс сдвига
    public interface IShiftable
    {
        void Shift(int bits);
    }
}