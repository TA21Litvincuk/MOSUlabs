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
        private GainBlock Gain;    // Пропорційна складова
        private IntBlock Int;      // Інтегральна складова
        private DiffBlock Diff;    // Диференційна складова
        private LimitBlock outputLimit = new LimitBlock(0, 100); // Ліміт виходу PID

        private double ki = 0.000001; // Коефіцієнт інтегрування (щоб уникнути ділення на 0)

        public double K
        {
            get => Gain.Gain;
            set => Gain.Gain = value;
        }

        public double Ti
        {
            get => 1 / ki;
            set => ki = value == 0 ? double.MaxValue : 1 / value;
        }

        public double Ki
        {
            get => ki;
            set => ki = value;
        }

        public double Td { get; set; }

        public LimitBlock OutputLimit
        {
            get => outputLimit;
            set => outputLimit = value ?? new LimitBlock(0, 100);
        }

        public PIDBlock(double dt)
        {
            Gain = new GainBlock(1);
            Int = new IntBlock(dt);
            Diff = new DiffBlock(dt);
        }

        public override double Calc(double x)
        {
            double p = Gain.Calc(x);
            double i = ki * Int.Calc(x);
            double d = Td * Diff.Calc(x);

            double rawU = p + i + d;
            double limitedU = outputLimit.Calc(rawU);

            // Anti-windup: якщо насичення — інтегратор не накопичує
            if (rawU != limitedU)
            {
                var currentState = Int.GetState();
                Int.SetState(currentState.sum, 0); // Зупинити інтегрування
            }

            return limitedU;
        }

        // Методи для збереження/відновлення стану інтегратора
        public (double sum, double prev) IntState
        {
            get => Int.GetState();
            set => Int.SetState(value.sum, value.prev);
        }
    }
}
