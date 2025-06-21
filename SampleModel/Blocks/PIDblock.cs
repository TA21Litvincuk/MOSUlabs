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
        private double dt;
        private double prevX = 0;
        private double intSum = 0;

        public double K { get; set; } = 1;

        private double ki = 1; // Замість 0.000001
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

        public double Td { get; set; } = 0;

        public double UpLimit { get; set; } = 100;
        public double DownLimit { get; set; } = 0;

        public PIDBlock(double dt)
        {
            this.dt = dt;
        }

        public override double Calc(double x)
        {
            double derivative = (x - prevX) / dt;

            // ПІД вираз
            double u = K * x + ki * intSum + Td * derivative;

            // Перевірка насичення
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

            // Anti-windup: інтегруємо лише якщо не насичено та інтеграл активний
            if (!limited && ki != 0)
            {
                intSum += (prevX + x) * dt / 2.0;
            }

            prevX = x;
            return u;
        }

        // Для збереження/відновлення стану інтегратора (при зміні режиму)
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
