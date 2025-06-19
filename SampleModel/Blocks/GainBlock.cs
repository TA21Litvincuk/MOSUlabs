using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleModel.Blocks
{
    // Підсилювальний блок — просто множить вхідний сигнал на коефіцієнт Gain
    public class GainBlock : BaseBlock
    {
        public double Gain { get; set; }   // Коефіцієнт підсилення

        public GainBlock(double Gain)
        {
            this.Gain = Gain;              // Ініціалізація коефіцієнта підсилення
        }

        public override double Calc(double x)
        {
            return Gain * x;               // Помножує вхід на Gain і повертає результат
        }

    }
}
