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
        private double dt;                       // Крок дискретизації
        private double prevX = 0;                // Попереднє значення помилки (для похідної)
        private LimitedIntBlock integrator;      // Інтегратор з обмеженням

        public double K { get; set; } = 1;       // Пропорційний коефіцієнт

        private double ki = 1;                   // Інтегральний коефіцієнт (1/Ti)
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

        public double Td { get; set; } = 0;      // Диференційний коефіцієнт

        // Межі вихідного сигналу
        public double UpLimit
        {
            get => integrator.UpLimit;
            set => integrator.UpLimit = value;
        }
        public double DownLimit
        {
            get => integrator.DownLimit;
            set => integrator.DownLimit = value;
        }

        public PIDBlock(double dt)
        {
            this.dt = dt;
            // Початкові межі інтегратора
            integrator = new LimitedIntBlock(dt, 0, 100);
        }

        // Основний метод — обчислення керуючого сигналу регулятора
        public override double Calc(double x)
        {
            double derivative = (x - prevX) / dt;          // Диференційна складова (швидкість зміни помилки)
            double integral = integrator.Calc(x);          // Інтегральна складова з обмеженням

            double u = K * x + ki * integral + Td * derivative; // ПІД вираз

            // Обмеження вихідного сигналу (saturation)
            if (u > UpLimit) u = UpLimit;
            else if (u < DownLimit) u = DownLimit;

            prevX = x;  // Оновлюємо попередню помилку
            return u;
        }

        // Для скидання інтегратора (наприклад, при зміні режиму)
        public void ResetIntegrator()
        {
            integrator = new LimitedIntBlock(dt, DownLimit, UpLimit);
        }

        // Збереження/відновлення стану інтегратора (для плавного перемикання режимів)
        public (double sum, double prev) IntState
        {
            get => (integrator == null ? (0, 0) : (integrator.GetState().sum, integrator.GetState().prev));
            set
            {
                if (integrator != null)
                    integrator.SetState(value.sum, value.prev);
            }
        }
    }


}
