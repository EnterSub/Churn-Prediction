using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Diagnostics;
namespace Visualization
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Open_file_Click(object sender, EventArgs e) // Нажатие кнопки "Открыть"
        {
            using (OpenFileDialog openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "Текстовые файлы|*.txt"; // Импорт текстового файла
                if (openDialog.ShowDialog(this) == DialogResult.OK)
                {
                    // Динамическое изменение формы для удобства пользователя
                    file.Text = openDialog.FileName;
                    filename.Text += Path.GetFileNameWithoutExtension(openDialog.FileName);
                    file_log.Text = filename.Text;
                    open_file.Enabled = false;
                    true_predict.Visible = true;
                    false_predict.Visible = true;
                    time_visualization.Visible = true;
                    equal.Visible = true;
                    unequal.Visible = true;
                    visualization_time.Visible = true;
                    matrix.Visible = true;
                    p2.Visible = true;
                    p1.Visible = true;
                    n1.Visible = true;
                    n2.Visible = true;
                    matrix_TP.Visible = true;
                    matrix_FN.Visible = true;
                    matrix_FP.Visible = true;
                    matrix_TN.Visible = true;
                    start.Enabled = true;
                    start.Visible = true;
                    progress.Visible = true;
                }
            }
        }
        private void Start_Click(object sender, EventArgs e) // Нажатие кнопки "Посчитать"
        {
            int o1 = 1; // Номер строки
            int TP = 0;
            int TN = 0;
            int FP = 0;
            int FN = 0;
            string line; // Строка
            Stopwatch findTime = new Stopwatch();
            findTime.Start(); // Начало счета визуализации
            using (var streamReader = new StreamReader(file.Text, System.Text.Encoding.Default))
            {
                while ((line = streamReader.ReadLine()) != null) // Пока есть строки в файле
                {
                    var stringNumbers = line.Split(' '); // Разделитель цифр
                    string expression = o1 + ".  " + line;
                    // Проверка на корректные значения строки
                    if (stringNumbers[0].Length > 1 || stringNumbers[1].Length > 1 || Convert.ToInt32(stringNumbers[0]) > 1 || Convert.ToInt32(stringNumbers[1]) > 1)
                    {
                        MessageBox.Show("Неверные данные");
                        Application.Exit();
                    }
                    if (stringNumbers[0] == stringNumbers[1]) // Если совпало (TP, TN)
                        if (Convert.ToInt32(stringNumbers[0]) > 0) // TP
                        {
                            TP++;
                            // Визуализация строки
                            file_log.Text += Environment.NewLine + expression + "   +";
                        }
                        else // TN
                        {
                            TN++;
                            // Визуализация строки
                            file_log.Text += Environment.NewLine + expression + "   +";
                        }
                    else // Иначе (FP, FN)
                        if (Convert.ToInt32(stringNumbers[0]) > 0) // FN
                    {
                        FN++;
                        // Визуализация строки
                        file_log.Text += Environment.NewLine + expression + "   -";
                    }
                    else // FP
                    {
                        FP++;
                        // Визуализация строки
                        file_log.Text += Environment.NewLine + expression + "   -";
                    }
                    o1 += 1;
                    // Динамическое изменение максимума прогресс-бара
                    progress.Maximum += progress.Step;
                    progress.Value += progress.Step;
                }
                streamReader.Close(); // Закрыть файл
                for (int i = 1; i < file_log.Lines.Length; i++) // Закрашиваем строки
                {
                    string text = file_log.Lines[i];
                    file_log.Select(file_log.GetFirstCharIndexFromLine(i), text.Length);
                    if (text.Contains("-")) // Если не совпали
                    {
                        file_log.SelectionColor = Color.Red;
                    }
                }
                float P = TP + FN; // Количество полож. объектов
                float N = TN + FP; // Количество отриц. объектов
                // Верный прогноз
                equal.Text = Convert.ToString(String.Format("{0:0.0}", ((TP + TN) / (P + N)) * 100));
                // Неверный прогноз
                unequal.Text = Convert.ToString(String.Format("{0:0.0}", (FN / (N + P) + FP / (N + P)) * 100));
                // Заполняем матрицу ошибок (confusion matrix)
                matrix_TP.Text = Convert.ToString(TP); // TP (1 1)
                matrix_FN.Text = Convert.ToString(FN); // FN (1 0)
                matrix_FP.Text = Convert.ToString(FP); // FP (0 1)
                matrix_TN.Text = Convert.ToString(TN); // TN (0 0)
                // Условия изменения цвета для верного прогнозирования
                if (Convert.ToDouble(equal.Text) <= 100 && Convert.ToDouble(equal.Text) > 90)
                    equal.BackColor = Color.Green;
                else if (Convert.ToDouble(equal.Text) <= 90 && Convert.ToDouble(equal.Text) > 80)
                    equal.BackColor = Color.LightGreen;
                else if (Convert.ToDouble(equal.Text) <= 80 && Convert.ToDouble(equal.Text) > 70)
                    equal.BackColor = Color.GreenYellow;
                else if (Convert.ToDouble(equal.Text) <= 70 && Convert.ToDouble(equal.Text) >= 60)
                    equal.BackColor = Color.Yellow;
                else equal.BackColor = Color.Red;
                // Условия изменения цвета для неверного прогнозирования
                if (Convert.ToDouble(unequal.Text) >= 0 && Convert.ToDouble(unequal.Text) < 10)
                    unequal.BackColor = Color.Green;
                else if (Convert.ToDouble(unequal.Text) >= 10 && Convert.ToDouble(unequal.Text) < 20)
                    unequal.BackColor = Color.LightGreen;
                else if (Convert.ToDouble(unequal.Text) >= 20 && Convert.ToDouble(unequal.Text) < 30)
                    unequal.BackColor = Color.GreenYellow;
                else if (Convert.ToDouble(unequal.Text) >= 30 && Convert.ToDouble(unequal.Text) <= 40)
                    unequal.BackColor = Color.Yellow;
                else unequal.BackColor = Color.Red;
            }
            // Построение диаграммы
            string[] seriesArray = { true_predict.Text, false_predict.Text }; // Строки диаграммы
            double[] pointsArray = { Convert.ToDouble(equal.Text), Convert.ToDouble(unequal.Text) }; // Столбцы
            diagram.BorderlineDashStyle = ChartDashStyle.Solid;
            diagram.ChartAreas[0].AxisX.Enabled = AxisEnabled.False;
            diagram.ChartAreas[0].AxisY.MajorGrid.LineColor = SystemColors.ControlLight;
            diagram.ChartAreas[0].AxisY.Minimum = 0;
            diagram.ChartAreas[0].AxisY.Maximum = 100;
            diagram.ChartAreas[0].AxisY.MajorGrid.Interval = 10;
            diagram.ChartAreas[0].AxisY.Title = "Соотношение прогнозов в банковской сфере";
            diagram.BackColor = Color.PaleTurquoise;
            diagram.BackSecondaryColor = Color.White;
            diagram.BackGradientStyle = GradientStyle.DiagonalRight;
            diagram.BorderlineColor = Color.White;
            diagram.ChartAreas[0].BackColor = Color.Orange;
            diagram.Palette = ChartColorPalette.Bright;
            diagram.Titles.Add("Accuracy of" + Path.GetFileNameWithoutExtension(filename.Text));
            for (int i = 0; i < seriesArray.Length; i++)
            {
                Series series = diagram.Series.Add(seriesArray[i]);
                series.Points.Add(pointsArray[i]);
                diagram.ChartAreas[0].Area3DStyle.Enable3D = true;
                diagram.ChartAreas[0].Area3DStyle.IsRightAngleAxes = false;
                diagram.ChartAreas[0].Area3DStyle.Inclination = 0;
                diagram.ChartAreas[0].Area3DStyle.Rotation = -90;
                diagram.ChartAreas[0].Area3DStyle.LightStyle = LightStyle.Realistic;
                diagram.ChartAreas[0].Area3DStyle.Perspective = 10;
            }
            findTime.Stop(); // Окончание счета визуализации
            equal.Text += " %";
            unequal.Text += " %";
            visualization_time.Text = (findTime.Elapsed.Ticks) / 10000000 + " секунд";
            // Динамическое изменение формы для удобства пользователя
            diagram.Visible = true;
            file_log.Visible = true;
            start.Enabled = false;
            save_diagram.Visible = true;
            close.Visible = true;
        }

        private void Form1_Load(object sender, EventArgs e) // Загрузка формы
        {
            this.AutoSize = true; // Динамическое изменение формы для удобства пользователя
        }
        private void Close_Click(object sender, EventArgs e) // Нажатие кнопки "Закрыть"
        {
            this.Close();
        }
        private void Save_Click(object sender, EventArgs e) // Нажатие кнопки "Сохранить"
        {
            // Экспорт в Png
            save_diagram.Enabled = false;
            var image = Path.ChangeExtension(file.Text, "png");
            diagram.SaveImage(image, System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}
