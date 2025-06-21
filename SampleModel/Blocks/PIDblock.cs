using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// PID-регулятор: формує керуючий сигнал на основі пропорційної, інтегральної та диференційної частин
namespace SampleModel.Blocks
{
public class PIDBlock : BaseBlock
    {

        private double dt;                  // Крок дискретизації
        private double prevX = 0;           // Попереднє значення вхідного сигналу (для диференціалу)
        private double intSum = 0;          // Накопичена інтегральна складова

        public double K { get; set; } = 1;  // Пропорційний коефіцієнт

        private double ki = 1;              // Інтегральний коефіцієнт (1/Ti)
        public double Ti
        {
            get => (ki == 0) ? 0 : 1 / ki;
            set => ki = (value <= 0) ? 0 : 1 / value;
        }
        public double Ki
        {
            get => ki;
            set => ki = value;
        }

        public double Td { get; set; } = 0; // Диференціальний коефіцієнт

        public double UpLimit { get; set; } = 100; // Верхня межа вихідного сигналу
        public double DownLimit { get; set; } = 0; // Нижня межа

        public PIDBlock(double dt)
        {
            this.dt = dt;
        }

        // Обчислення керуючого сигналу за поточним значенням помилки (x)
        public override double Calc(double x)
        {
            double derivative = (x - prevX) / dt; // Похідна (швидкість зміни помилки)

            // Основна формула ПІД-регулятора
            double u = K * x + ki * intSum + Td * derivative;

            // Обмеження сигналу (saturation)
            bool limited = false;
            if (u > UpLimit)
            {
                u = UpLimit;
                limited = true;
            }
            else if (u < DownLimit)
            {
                u = DownLimit;
                limited = true;
            }

            // Anti-windup: інтегруємо тільки якщо не насичено та інтеграл активний
            if (!limited && ki != 0)
            {
                intSum += (prevX + x) * dt / 2.0; // Інтегруємо методом трапецій
            }

            prevX = x;
            return u;
        }

        // Збереження/відновлення стану інтегратора (для плавного перемикання режимів)
        public (double sum, double prev) IntState
        {
            get => (intSum, prevX);
            set
            {
                intSum = value.sum;
                prevX = value.prev;
            }
        }
    }
}
