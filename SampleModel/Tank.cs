using SampleModel.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleModel
{
    public class Tank
    {
        // Tank моделює інтегруючу ланку(сумує різницю між подачею та зливом).
        private ComplexBlock Block;                 // Комплексний блок (може містити кілька обчислювальних блоків)
        private GainBlock kx1;                      // Підсилювач для x1 з коефіцієнтом 1
        private GainBlock kx2;                      // Підсилювач для x2 з коефіцієнтом -1
        private LimitBlock xLimit = new LimitBlock(0, 100); // Обмежувач значення від 0 до 100
        private double dt = 0.1;                    // Крок моделювання (дискретизації)
        public Tank(double dt)
        {
            this.dt = dt;                           // Ініціалізація кроку дискретизації
            kx1 = new GainBlock(1);                 // Створення підсилювача з коеф. 1
            kx2 = new GainBlock(-1);                // Створення підсилювача з коеф. -1
            Block = new ComplexBlock();             // Створення комплексного блоку
            Block.Add(new IntBlock(dt));            // Додаємо інтегруючий блок до комплексного
            Block.Add(new LimitBlock(0, 100));      // обмежуємо рівень у межах 0–100
        }

        public double Calc(double x1, double x2) {
            x1 = xLimit.Calc(x1);                   // Обмеження значення x1 в межах [0;100]
            x2 = xLimit.Calc(x2);                   // Те саме для x2
            var x = kx1.Calc(x1) + kx2.Calc(x2);    // Обчислення результату підсилення
            return Block.Calc(x);                   // Подаємо результат у комплексний блок
        }
    }
}
