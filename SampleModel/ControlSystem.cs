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
        private PIDBlock PID;
        private Tank Tank;
        private LimitBlock xLimit = new LimitBlock(0, 100);
        private LimitBlock levelLimit = new LimitBlock(0, 10);
        public double Time { get; set; } = 0;
        //
        private double input1;
        private double input2;
        public double Input1 { get { return input1; } set { input1 = xLimit.Calc(value); } }
        public double Input2 { get { return input2; } set { input2 = xLimit.Calc(value); } }

        private double setPoint;
        public double SetPoint { get { return setPoint; } set { setPoint = levelLimit.Calc(value); } }

        public double K { get { return PID.K; } set { PID.K = value; } }
        public double Ti { get { return PID.Ti; } set { PID.Ti = value; } }
        public double Td { get { return PID.Td; } set { PID.Td = value; } }

        public double Output { get; set; }


        public ControlSystem(double dt)
        {
            this.dt = dt;
            PID = new PIDBlock(dt);
            Tank = new Tank(dt);
        }


        public void Calc()
        {
            Output = Tank.Calc(Input1, Input2);
            var e = SetPoint - Output;
            var u = PID.Calc(e);
            Input1 = u;
            Time += dt;
        }
    }
}
