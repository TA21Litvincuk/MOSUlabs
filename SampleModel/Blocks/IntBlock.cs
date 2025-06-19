using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleModel.Blocks
{
    // Інтегральний блок — чисельне інтегрування сигналу методом трапецій
    public class IntBlock : BaseBlock
    {
        private double prev = 0;   // Попереднє значення вхідного сигналу
        private double dt;         // Крок дискретизації
        private double sum = 0;    // Накопичена інтегральна сума

        public IntBlock(double dt)
        {
            this.dt = dt;          // Ініціалізація кроку дискретизації
        }

        public override double Calc(double x)
        {
            sum += (prev + x) * dt / 2;   // Метод трапецій: інтегруємо площу під кривою між prev і x
            prev = x;                     // Запам’ятовуємо поточне значення для наступного кроку
            return sum;                   // Повертаємо інтеграл
        }

        // Метод для отримання поточного стану інтегратора
        public (double sum, double prev) GetState()
        {
            return (sum, prev);
        }

        // Метод для встановлення стану інтегратора (для безударного переключення)
        public void SetState(double sum, double prev)
        {
            this.sum = sum;
            this.prev = prev;
        }
    }
}
