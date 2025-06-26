using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleModel.Blocks
{
    // LimitedIntBlock — інтегруючий блок із обмеженням (anti-windup)
    // Накопичує інтеграл сигналу, але не дозволяє значенню sum виходити за межі (UpLimit, DownLimit)
    internal class LimitedIntBlock : BaseBlock
    {
        private double prev = 0;
        private double dt;
        private double sum = 0;
        public double UpLimit { get; set; }
        public double DownLimit { get; set; }
        public LimitedIntBlock(double dt)
        {
            this.dt = dt;
        }
        public LimitedIntBlock(double dt, double downLimit, double upLimit)
        {
            this.dt = dt;
            UpLimit = upLimit;
            DownLimit = downLimit;
        }
        public override double Calc(double x)
        {
            sum += (prev + x) * dt / 2;
            sum = (sum > UpLimit) ? UpLimit : sum;
            sum = (sum < DownLimit) ? DownLimit : sum;
            prev = x;
            return sum;
        }

       
        public (double sum, double prev) GetState()
        {
            return (sum, prev);
        }

        public void SetState(double sum, double prev)
        {
            this.sum = sum;
            this.prev = prev;
        }
    }


}

