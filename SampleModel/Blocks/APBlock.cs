using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleModel.Blocks
{
    // APBlock — аперіодична ланка першого порядку (інерційна)
    // Реалізує фільтрацію або згладжування вхідного сигналу
    public class APBlock : BaseBlock
	{
        private double dt;      // Крок моделювання (дискретизації)
        private double T;       // Постійна часу ланки (інерційність)
        private double prev;    // Попереднє вихідне значення y[n-1]

        // Конструктор: задаємо крок дискретизації та сталу часу
        public APBlock(double dt, double T)
        {
            this.dt = dt;
            this.T = T;
        }
        // Основний метод обчислення виходу ланки
        public override double Calc(double x) {
            var y = (dt * x + T * prev) / (T + dt);  // Формула аперіодичної дискретної ланки
            prev = y;  // Зберігаємо поточний вихід для використання на наступному кроці
            return y;
        }
	}
}
