using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleModel.Blocks
{
    // Диференціальний блок — чисельне диференціювання сигналу
    public class DiffBlock : BaseBlock
    {
        private double dt;      // Крок дискретизації
        private double prev;    // Попереднє вхідне значення

        public DiffBlock(double dt)
        {
            this.dt = dt;       // Ініціалізуємо крок дискретизації
        }

        public override double Calc(double x)
        {
            var y = (x - prev) / dt;  // Обчислюємо чисельне похідне (різницю по часу)
            prev = x;                 // Запам’ятовуємо поточне значення для наступного кроку
            return y;                 // Повертаємо похідне
        }
    }
}
