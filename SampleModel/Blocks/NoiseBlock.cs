using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleModel.Blocks
{
    // Блок шуму — додає випадковий шум до вхідного сигналу
    public class NoiseBlock : BaseBlock
    {
        private double noise { get; set; }  // Відсоток шуму від вхідного сигналу
        private Random rand;                // Генератор випадкових чисел

        public NoiseBlock(double noise)
        {
            this.noise = noise;             // Ініціалізація рівня шуму
            rand = new Random();            // Створення генератора випадкових чисел
        }

        public override double Calc(double x)
        {
            var nn = x * noise / 100;       // Визначаємо амплітуду шуму (в процентах від x)
            return x + 2 * nn * rand.NextDouble() - nn;  // Додаємо шум у діапазоні [-nn, +nn]
        }
    }
}
