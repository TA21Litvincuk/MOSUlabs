using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleModel
{
    // DelayBlock — блок затримки сигналу на певний час (tau)
    // Імітує, що вхідний сигнал виходить із запізненням на tau секунд
    public class DelayBlock : BaseBlock
    {
        private double dt;                          // Крок дискретизації
        private int cnt;                           // Кількість кроків затримки
        private Queue<double> delayQueue = new Queue<double>(); // Черга для зберігання значень затримки

        public DelayBlock(double dt, double tau)
        {
            if (tau >= dt)
            {
                cnt = (int)(tau / dt);             // Обчислюємо, скільки кроків моделювання дорівнює часу затримки
            }
            else
            {
                throw new Exception("tau should be greater than dt"); // Викидаємо помилку, якщо затримка менша за крок
            }
        }

        public override double Calc(double x)
        {
            delayQueue.Enqueue(x);                 // Додаємо нове значення в чергу
            if (delayQueue.Count > cnt)
            {          // Якщо накопичилось достатньо значень для затримки
                return delayQueue.Dequeue();       // Повертаємо найстаріше (затримане) значення
            }
            else
            {
                return 0;                          // Поки черга недостатньо довга, повертаємо 0
            }
        }
    }
}
