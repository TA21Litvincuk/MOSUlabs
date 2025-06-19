using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleModel
{
    // Комплексний блок — контейнер для послідовного обчислення декількох блоків
    public class ComplexBlock : BaseBlock
    {
        private List<BaseBlock> blocks;   // Список вкладених блоків (будь-яких, що наслідують BaseBlock)

        public ComplexBlock()
        {
            blocks = new List<BaseBlock>();  // Ініціалізація списку блоків
        }

        // Метод для додавання блоку до комплексного ланцюга
        public void Add(BaseBlock block)
        {
            blocks.Add(block);
        }

        // Обчислення — послідовно викликає Calc для кожного блоку, передаючи результат далі
        public override double Calc(double x)
        {
            double res = x;
            foreach (var b in blocks)
            {
                res = b.Calc(res);   // Вхід у наступний блок — це вихід попереднього
            }
            return res;  // Повертаємо результат після проходження усіх блоків
        }
    }
}
