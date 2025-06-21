using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleModel
{
    public class PIDOptimizer
    {
        private const double Epsilon = 1e-6;
        private const double Step = 0.1;
        private const int MaxIterations = 1000;

        private readonly Func<double, double, double> costFunction;

        public PIDOptimizer(Func<double, double, double> function)
        {
            costFunction = function;
        }

        public (double K, double Ti, double ISE, int iterations) Optimize(double K0, double Ti0)
        {
            double K = K0;
            double Ti = Ti0;
            int iter = 0;

            while (iter < MaxIterations)
            {
                double current = costFunction(K, Ti);
                double best = current;
                double newK = K;
                double newTi = Ti;

                // Пошук по K
                double forwardK = costFunction(K + Step, Ti);
                double backwardK = costFunction(K - Step, Ti);
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

                // Пошук по Ti
                double forwardTi = costFunction(newK, Ti + Step);
                double backwardTi = costFunction(newK, Ti - Step);
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

            return (K, Ti, costFunction(K, Ti), iter);
        }
    }
}
