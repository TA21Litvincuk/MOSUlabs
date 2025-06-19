using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleModel.Blocks
{
    // Обмежувальний блок — обмежує вхідне значення між мінімумом і максимумом
    public class LimitBlock : BaseBlock
    {
        private double min;   // Мінімальне допустиме значення
        private double max;   // Максимальне допустиме значення

        public LimitBlock(double min, double max)
        {
            this.min = min;   // Ініціалізація мінімального обмеження
            this.max = max;   // Ініціалізація максимального обмеження
        }

        public override double Calc(double x)
        {
            x = (x > max) ? max : x;   // Якщо x більше max — присвоїти max
            x = (x < min) ? min : x;   // Якщо x менше min — присвоїти min
            return x;                  // Повернути обмежене значення
        }
    }
}
