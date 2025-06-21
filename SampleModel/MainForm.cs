using SampleModel.Blocks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SampleModel
{

    public partial class MainForm : Form
    {
        private Tank Block;                     // Модель об'єкта (бак)
        private PIDBlock pid = new PIDBlock(0.1); // ПІД-регулятор
        private LimitBlock xLimit = new LimitBlock(0, 100); // Обмеження входів

        private double x1;  // Вхід 1 (ручний або PID)
        private double x2;  // Вхід 2 (злив)
        private double y;   // Вихід (рівень)
        private double time = 0;
        private double dt = 0.1;

        private bool isAutoMode = false; // false = ручний режим, true = автомат
        private double setpoint = 50;    // бажаний рівень у режимі ПІД
        private double ise = 0;

        // Змінні для збереження стану інтегратора при переключенні режимів
        private (double sum, double prev) savedIntState;

        public MainForm() {
            InitializeComponent();
            Block = new Tank(dt);

            // Початкові параметри ПІД-регулятора
            pid.K = 1;
            pid.Ti = 5;
            pid.Td = 0.1;

            // Обмеження виходу ПІД-регулятора (для anti-windup)
            pid.UpLimit = 100;
            pid.DownLimit = 0;



            // Ініціалізація текстових полів
            tbX.Text = x1.ToString("F2");
            tbX2.Text = x2.ToString("F2");
            tbSetpoint.Text = setpoint.ToString("F2");
            tbK.Text = pid.K.ToString("F2");
            tbTi.Text = pid.Ti.ToString("F2");
            tbTd.Text = pid.Td.ToString("F2");
        }


        // Метод, що викликається кожен крок таймера (симулює час)
        private void tmModel_Tick(object sender, EventArgs e) 
        {
            double controlInput;

            if (isAutoMode)
            {
                // Автоматичне керування через ПІД-регулятор
                double error = setpoint - y;
                controlInput = pid.Calc(error);
                x1 = controlInput; // для графіка
            }
            else
            {
                // Ручне керування
                x1 = xLimit.Calc(x1);
            }

            x2 = xLimit.Calc(x2);

            y = Block.Calc(x1, x2);
            time += dt;

            // ---- ISE ----
            double errorForISE = setpoint - y;
            ise += errorForISE * errorForISE * dt; // Інтеграл квадрата помилки

            // ---- Додавання даних на графіки ----
            chMainPlot.Series["seriesY"].Points.AddXY(time, y);      // Рівень у баку (y)
            chMainPlot.Series["seriesX"].Points.AddXY(time, x1);        // Керуючий сигнал (u1)
            chMainPlot.Series["seriesX2"].Points.AddXY(time, x2);        // Злив (u2)
            //chMainPlot.Series["SeriesISE"].Points.AddXY(time, ise);      // ISE

            // ---- Оновлюємо текстові поля ----
            lbY.Text = y.ToString("F2");
            tbX.Text = x1.ToString("F2");
            tbX2.Text = x2.ToString("F2");
        }

        // Старт моделі (запуск таймера)
        private void btnStart_Click(object sender, EventArgs e) 
        {
            tmModel.Start();
        }

        // Стоп моделі (зупинка таймера)
        private void btnStop_Click(object sender, EventArgs e) {
            tmModel.Stop();
        }

        // Збільшення потоку x1 (вхідного) через кнопку
        private void btnUp_Click(object sender, EventArgs e)
        {
            if (!isAutoMode)
            {
                x1 += 1;
                x1 = xLimit.Calc(x1);
                tbX.Text = x1.ToString("F2");
            }
        }

        // Зменшення потоку x1
        private void btnDn_Click(object sender, EventArgs e) 
        {
            if (!isAutoMode)
            {
                x1 -= 1;
                x1 = xLimit.Calc(x1);
                tbX.Text = x1.ToString("F2");
            }
        }

        // Зміна швидкості таймера — повільно (1 раз/сек)
        private void btnX1_Click(object sender, EventArgs e) {
            tmModel.Interval = 1000;
        }

        // Зміна швидкості таймера — швидко (10 раз/сек)
        private void btnX10_Click(object sender, EventArgs e) {
            tmModel.Interval = 100;
        }

        private void lbYCaption_Click(object sender, EventArgs e) {

        }

        // Зменшення потоку x2 (вихідного)
        private void btnDn2_Click(object sender, EventArgs e) {
            x2 -= 1;
            x2 = xLimit.Calc(x2);
            tbX2.Text = x2.ToString("F2");
        }

        // Збільшення потоку x2
        private void btnUp2_Click(object sender, EventArgs e) {
            x2 += 1;
            x2 = xLimit.Calc(x2);
            tbX2.Text = x2.ToString("F2");
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        // Зміна SetPoint
        private void btnSPDn_Click(object sender, EventArgs e)
        {
            setpoint -= 1;  // Зменшуємо SetPoint при "Down"
            setpoint = new LimitBlock(0, 100).Calc(setpoint);
            tbSetpoint.Text = setpoint.ToString("F2");
        }

        private void btnSPUp_Click(object sender, EventArgs e)
        {
            setpoint += 1;  // Збільшуємо SetPoint при "Up"
            setpoint = new LimitBlock(0, 100).Calc(setpoint);
            tbSetpoint.Text = setpoint.ToString("F2");
        }

        // Зміна параметрів PID
        private void btnKDn_Click(object sender, EventArgs e)
        {
            pid.K -= 0.1;
            if (pid.K < 0) pid.K = 0;
            tbK.Text = pid.K.ToString("F2");
        }

        private void btnKUp_Click(object sender, EventArgs e)
        {
            pid.K += 0.1;
            tbK.Text = pid.K.ToString("F2");
        }

        private void btnTiDn_Click(object sender, EventArgs e)
        {
            pid.Ti -= 0.5;
            if (pid.Ti < 0) pid.Ti = 0;
            tbTi.Text = pid.Ti.ToString("F2");
        }

        private void btnTiUp_Click(object sender, EventArgs e)
        {
            pid.Ti += 0.5;
            tbTi.Text = pid.Ti.ToString("F2");

        }

        private void btnTdDn_Click(object sender, EventArgs e)
        {
            pid.Td -= 0.05;
            if (pid.Td < 0) pid.Td = 0;
            tbTd.Text = pid.Td.ToString("F2");
        }

        private void btnTdUp_Click(object sender, EventArgs e)
        {
            pid.Td += 0.05;
            tbTd.Text = pid.Td.ToString("F2");
        }

        private void chMainPlot_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (isAutoMode)
            {
                // Збережемо стан інтегратора для плавного переключення назад в автомат
                savedIntState = pid.IntState;
            }
            isAutoMode = false;
            MessageBox.Show("Ручний режим активовано");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!isAutoMode)
            {
                // Відновлюємо стан інтегратора, щоб уникнути стрибків
                pid.IntState = savedIntState;
            }
            isAutoMode = true;
            MessageBox.Show("Автоматичний режим (PID) активовано");
        }


        private void button3_Click(object sender, EventArgs e)
        {
            // Новий об'єкт системи для кожної оцінки (уникаємо накопичення стану)
            Func<double, double, double> iseFunc = (k, ti) =>
            {
                // Обмеження на параметри PID (запобігає дивним результатам)
                if (k < 0.1 || k > 10 || ti < 0.1 || ti > 50)
                    return double.MaxValue;

                var sys = new ControlSystem(0.1);
                sys.PID.K = k;
                sys.PID.Ti = ti;
                sys.PID.Td = pid.Td; // Td залишаємо сталим або оптимізуємо окремо
                sys.SetPoint = setpoint; // або 5, або твоє поле

                double ise = 0;
                int steps = 100; // 10 секунд
                for (int i = 0; i < steps; i++)
                {
                    sys.Calc();
                    ise += Math.Pow(sys.E, 2) * 0.1;
                }
                return ise;
            };

            // Стартова точка оптимізації
            var optimizer = new PIDOptimizer(iseFunc);
            var (bestK, bestTi, minISE, iters) = optimizer.Optimize(pid.K, pid.Ti);

            // Підставляємо знайдені параметри у твій робочий регулятор
            pid.K = bestK;
            pid.Ti = bestTi;
            tbK.Text = pid.K.ToString("F2");
            tbTi.Text = pid.Ti.ToString("F2");

            MessageBox.Show(
                $"Параметричний синтез завершено!\nK = {bestK:F2}, Ti = {bestTi:F2}\nISE = {minISE:F4}\nІтерацій = {iters}",
                "Параметричний синтез"
            );

        }

        private void button4_Click(object sender, EventArgs e)
        {
            var optimizer = new GaussSeidelOptimizer();
            // Початкові значення (можна взяти з полів або задати явно)
            double startU1 = 0, startU2 = 0;

            var (u1, u2, Imin, iter) = optimizer.Minimize(startU1, startU2);

            MessageBox.Show(
                $"Мінімум знайдено:\n" +
                $"u1 = {u1:F4}\nu2 = {u2:F4}\n" +
                $"Imin = {Imin:F4}\nІтерацій = {iter}",
                "Gauss-Seidel Optimization"
            );
        }
    }
}
