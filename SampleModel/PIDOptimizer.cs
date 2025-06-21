using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleModel
{
    // Клас для оптимізації параметрів PID-регулятора (K і Ti)
    public class PIDOptimizer
    {
        private const double Epsilon = 1e-6;
        private const double Step = 0.1;
        private const int MaxIterations = 1000;

        private readonly Func<double, double, double> targetFunction;

        public PIDOptimizer(Func<double, double, double> function)
        {
            targetFunction = function;
        }

        // Повертає кортеж: K, Ti, мінімальне ISE, кількість ітерацій
        public (double K, double Ti, double ISE, int iterations) Optimize(double K0, double Ti0)
        {
            double K = K0;
            double Ti = Ti0;
            int iter = 0;

            while (iter < MaxIterations)
            {
                double current = targetFunction(K, Ti);
                double best = current;
                double newK = K;
                double newTi = Ti;

                // Крок по K
                double forwardK = targetFunction(K + Step, Ti);
                double backwardK = (K - Step > 0) ? targetFunction(K - Step, Ti) : double.MaxValue;
                if (forwardK < best)
                {
                    best = forwardK;
                    newK = K + Step;
                }
                else if (backwardK < best)
                {
                    best = backwardK;
                    newK = K - Step;
                }

                // Крок по Ti
                double forwardTi = targetFunction(newK, Ti + Step);
                double backwardTi = (Ti - Step > 0) ? targetFunction(newK, Ti - Step) : double.MaxValue;
                if (forwardTi < best)
                {
                    best = forwardTi;
                    newTi = Ti + Step;
                }
                else if (backwardTi < best)
                {
                    best = backwardTi;
                    newTi = Ti - Step;
                }

                if (Math.Abs(newK - K) < Epsilon && Math.Abs(newTi - Ti) < Epsilon)
                    break;

                K = newK;
                Ti = newTi;
                iter++;
            }

            return (K, Ti, targetFunction(K, Ti), iter);
        }
    }
}
