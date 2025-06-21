using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleModel
{
    public class OptimizationSolver
    {
        // Оптимальні значення змінних після обчислення
        public double U1 { get; private set; }
        public double U2 { get; private set; }

        // Значення функції у знайденій точці
        public double IValue { get; private set; }

        // Конструктор — початкові значення можна змінити
        public OptimizationSolver()
        {
            U1 = 0;
            U2 = 0;
        }

        // Основний метод реалізації методу Гауса-Зейделя
        public void Solve(double eps = 1e-5, int maxIter = 1000)
        {
            double u1 = U1;
            double u2 = U2;

            for (int iter = 0; iter < maxIter; iter++)
            {
                double prevU1 = u1;
                double prevU2 = u2;

                // Крок 1: обчислюємо нове u1 (розв'язання ∂I/∂u1 = 0)
                double temp = (u2 + 2) / 3.0;
                u1 = temp >= 0 ? Math.Sqrt(temp) : 0; // гарантія, що корінь з додатнього

                // Крок 2: обчислюємо нове u2 (розв'язання ∂I/∂u2 = 0)
                u2 = (u1 - 3) / 2.0;

                // Перевірка збіжності
                if (Math.Abs(u1 - prevU1) < eps && Math.Abs(u2 - prevU2) < eps)
                    break;
            }

            // Зберігаємо результат
            U1 = u1;
            U2 = u2;
            IValue = ComputeFunction(u1, u2);
        }

        // Обчислення значення функції I(u1, u2)
        private double ComputeFunction(double u1, double u2)
        {
            return Math.Pow(u1, 3) - u1 * u2 + Math.Pow(u2, 2) - 2 * u1 + 3 * u2 - 4;
        }
    }
}
