using SampleModel.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleModel
{
    public class ControlSystem
    {
        private double dt;
        public PIDBlock PID { get; private set; } // PID-регулятор
        private Tank Tank;                        // Модель бака (об'єкта)
        public double E => SetPoint - Output;     // Поточна помилка регулювання

        private LimitBlock xLimit = new LimitBlock(0, 100); // Обмежувач входу
        private LimitBlock levelLimit = new LimitBlock(0, 10); // Обмеження для setpoint (рівень)

        public double Time { get; set; } = 0;     // Час моделювання

        private double input1;                    // Вхід 1 (керування)
        private double input2;                    // Вхід 2 (злив)
        public double Input1 { get { return input1; } set { input1 = xLimit.Calc(value); } }
        public double Input2 { get { return input2; } set { input2 = xLimit.Calc(value); } }

        private double setPoint;
        public double SetPoint { get { return setPoint; } set { setPoint = levelLimit.Calc(value); } }

        // Доступ до параметрів регулятора (для оптимізації)
        public double K { get { return PID.K; } set { PID.K = value; } }
        public double Ti { get { return PID.Ti; } set { PID.Ti = value; } }
        public double Td { get { return PID.Td; } set { PID.Td = value; } }

        public double Output { get; set; }        // Вихід (рівень у баці)

        public ControlSystem(double dt)
        {
            this.dt = dt;
            PID = new PIDBlock(dt);
            Tank = new Tank(dt);
        }

        // Основний розрахунок системи на один крок
        public void Calc()
        {
            Output = Tank.Calc(Input1, Input2);   // Симуляція бака
            var e = SetPoint - Output;            // Помилка
            var u = PID.Calc(e);                  // Новий керуючий сигнал
            Input1 = u;                           // Подаємо керування на об'єкт
            Time += dt;
        }
    }
}
