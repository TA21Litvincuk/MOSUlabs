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
        private double ki = 0.000001;
        public double Ti
        {
            get => 1 / ki;
            set => ki = (value == 0) ? double.MaxValue : 1 / value;
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
            // Попередній розрахунок керуючого сигналу
            double derivative = (x - prevX) / dt;
            double u = K * x + ki * intSum + Td * derivative;

            // Перевірка обмеження
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

            // Компенсація насичення (anti-windup)
            if (ki != 0 && limited)
            {
                intSum = (u - K * x - Td * derivative) / ki;
            }
            else
            {
                intSum += (prevX + x) * dt / 2;
            }

            prevX = x;
            return u;
        }

        // Стан інтегратора — для збереження при перемиканні режимів
        public (double sum, double prev) IntState
        {
            get => (intSum, prevX);
            set { intSum = value.sum; prevX = value.prev; }
        }
    }
}
