using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleModel
{
    internal class Criteria
    {
        // Розрахунок інтегралу квадрата помилки для заданих K, Ti (Td можна залишити фіксованим)
        public static double ISE(ControlSystem sys, double K, double Ti, double Td, double dt = 0.1, double maxTime = 10)
        {
            sys.PID.K = K;
            sys.PID.Ti = Ti;
            sys.PID.Td = Td;
            sys.Output = 0; // Початковий рівень
            double ise = 0;
            int steps = (int)(maxTime / dt);
            sys.SetPoint = 5;

            for (int i = 0; i < steps; i++)
            {
                sys.Calc();
                ise += sys.E * sys.E * dt;  // Використай sys.E (або Error, якщо так називаєш)
            }
            return ise;
        }
    }
}
