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

        // Змінні для збереження стану інтегратора при переключенні режимів
        private (double sum, double prev) savedIntState;

        public MainForm() {
            InitializeComponent();
            Block = new Tank(dt);

            // Початкові параметри ПІД-регулятора
            pid.K = 1;
            pid.Ti = 5;
            pid.Td = 0.1;

            pid.OutputLimit = xLimit; // Ліміт для компенсації насичення інтегратора

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
                controlInput = xLimit.Calc(controlInput); // обмеження
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

            // Виведення на форму
            lbY.Text = y.ToString("F2");
            chMainPlot.Series[0].Points.AddXY(time, y);
            chMainPlot.Series[1].Points.AddXY(time, x1);
            chMainPlot.Series[2].Points.AddXY(time, x2);

            // Оновлюємо текстові поля для обох режимів
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

        private void tbX_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbX2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
