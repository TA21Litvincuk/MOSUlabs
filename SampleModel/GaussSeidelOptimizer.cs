using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleModel
{
    // Клас оптимізації методом Гауса-Зейделя 

    internal class GaussSeidelOptimizer
    {

        private const double Epsilon = 1e-6;       // Точність зупинки (збіжності)
        private const int MaxIterations = 1000;    // Максимальна кількість ітерацій

        // Функція двох змінних 
        // I(u1, u2) = u1^3 - u1*u2 + u2^2 - 2u1 + 3u2 - 4
        public double I(double u1, double u2)
        {
            return Math.Pow(u1, 3) - u1 * u2 + Math.Pow(u2, 2) - 2 * u1 + 3 * u2 - 4;
        }

        // Головний метод: пошук мінімуму функції методом Гауса-Зейделя
        public (double u1, double u2, double Imin, int iterations) Minimize(double u1Start, double u2Start)
        {
            double u1 = u1Start;
            double u2 = u2Start;
            int iter = 0;

            while (iter < MaxIterations)
            {
                double prevU1 = u1;
                double prevU2 = u2;

                // КРОК 1: мінімізуємо по u1 при фіксованому u2
                // ∂I/∂u1 = 3u1^2 - u2 - 2 = 0  →  u1 = ±sqrt((u2 + 2) / 3)
                double val = (u2 + 2) / 3.0;
                if (val >= 0)
                    u1 = Math.Sqrt(val);    // Беремо додатній корінь
                else
                    u1 = 0;                 // Якщо підкореневий від'ємний — ставимо 0

                // КРОК 2: мінімізуємо по u2 при новому u1
                // ∂I/∂u2 = -u1 + 2u2 + 3 = 0  →  u2 = (u1 - 3) / 2
                u2 = (u1 - 3.0) / 2.0;

                // Перевірка збіжності: якщо зміни дуже маленькі — зупиняємо цикл
                if (Math.Abs(u1 - prevU1) < Epsilon && Math.Abs(u2 - prevU2) < Epsilon)
                    break;

                iter++;
            }

            double Imin = I(u1, u2);   // Мінімальне знайдене значення функції
            return (u1, u2, Imin, iter);
        }
    }
}
