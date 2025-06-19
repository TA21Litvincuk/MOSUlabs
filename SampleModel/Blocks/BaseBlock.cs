using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleModel
{
    public abstract class BaseBlock
    {
        // Абстрактний метод Calc: кожен блок сам реалізує, як він обчислює результат
        public abstract double Calc(double x);

    }
}
