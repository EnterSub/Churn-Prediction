using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Diagnostics;

namespace Visualization
{
    public partial class Visualization_version2 : Form
    {
        public Visualization_version2()
        {
            InitializeComponent();
        }

        private void Open_file_Click(object sender, EventArgs e) //Press "Open" button
        {
            using (OpenFileDialog openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "Text files|*.txt"; //Import from text files only
                if (openDialog.ShowDialog(this) == DialogResult.OK)
                {
                    //Dynamic form reshaping for user convenience
                    file.Text = openDialog.FileName;
                    filename.Text += Path.GetFileNameWithoutExtension(openDialog.FileName);
                    file_log.Text = filename.Text; //?
                    open_file.Enabled = false;
                    true_predict.Visible = true;
                    false_predict.Visible = true;
                    equal.Visible = true;
                    unequal.Visible = true;
                    time_visualization.Visible = true;
                    visualization_time.Visible = true;
                    matrix.Visible = true;
                    p1.Visible = true;
                    p2.Visible = true;
                    n1.Visible = true;
                    n2.Visible = true;
                    matrix_TP.Visible = true;
                    matrix_FP.Visible = true;
                    matrix_FN.Visible = true;
                    matrix_TN.Visible = true;
                    start.Enabled = true;
                    start.Visible = true;
                    progress.Visible = true;
                }
            }
        }

        private void Start_Click(object sender, EventArgs e) //Press "Calculate" button
        {
            int string_count = 1; //String number
            int TP = 0;
            int TN = 0;
            int FP = 0;
            int FN = 0;
            string line; //String
            Stopwatch findTime = new Stopwatch();
            findTime.Start(); //Start calculation time

            using (var streamReader = new StreamReader(file.Text, System.Text.Encoding.Default))
            {
                while ((line = streamReader.ReadLine()) != null) //While there are lines in the file
                {
                    var stringNumbers = line.Split(' ');
                    string expression = string_count + ".  " + line;
                    //Checking for valid string values
                    if (stringNumbers[0].Length > 1 || stringNumbers[1].Length > 1 || Convert.ToInt32(stringNumbers[0]) > 1 || Convert.ToInt32(stringNumbers[1]) > 1)
                    {
                        MessageBox.Show("Incorrect data");
                        Application.Exit();
                    }
                    if (stringNumbers[0] == stringNumbers[1]) //If matched (TP, TN)
                        if (Convert.ToInt32(stringNumbers[0]) > 0) //TP
                        {
                            TP++;
                            file_log.Text += Environment.NewLine + expression + "   +"; //String visualization
                        }
                        else //TN
                        {
                            TN++;
                            file_log.Text += Environment.NewLine + expression + "   +"; //String visualization
                        }
                    else //Else (FP, FN)
                        if (Convert.ToInt32(stringNumbers[0]) > 0) // FN
                    {
                        FN++;
                        file_log.Text += Environment.NewLine + expression + "   -"; //String visualization
                    }
                    else //FP
                    {
                        FP++;
                        file_log.Text += Environment.NewLine + expression + "   -"; //String visualization
                    }
                    string_count += 1;
                    //Dynamic change of the progress bar
                    progress.Maximum += progress.Step;
                    progress.Value += progress.Step;
                }
                streamReader.Close(); //Close file

                for (int i = 1; i < file_log.Lines.Length; i++) //Coloring the lines
                {
                    string text = file_log.Lines[i];
                    file_log.Select(file_log.GetFirstCharIndexFromLine(i), text.Length);
                    if (text.Contains("-")) //If not matched
                    {
                        file_log.SelectionColor = Color.Red;
                    }
                }

                float P = TP + FN; //True prediction count
                float N = TN + FP; //False prediction count

                //True prediction
                equal.Text = Convert.ToString(String.Format("{0:0.0}", ((TP + TN) / (P + N)) * 100));
                //False prediction
                unequal.Text = Convert.ToString(String.Format("{0:0.0}", (FN / (N + P) + FP / (N + P)) * 100));
                //Confusion matrix
                matrix_TP.Text = Convert.ToString(TP); //TP
                matrix_FP.Text = Convert.ToString(FN); //FN
                matrix_FN.Text = Convert.ToString(FP); //FP
                matrix_TN.Text = Convert.ToString(TN); //TN

                //Color change conditions for true prediction
                if (Convert.ToDouble(equal.Text) >= 50 && Convert.ToDouble(equal.Text) < 60) { equal.BackColor = Color.Red; } //С 0,5 т.к. можно реверсировать при -> 0
                        else if (Convert.ToDouble(equal.Text) >= 60 && Convert.ToDouble(equal.Text) < 70) { equal.BackColor = Color.Yellow; }
                        else if (Convert.ToDouble(equal.Text) >= 70 && Convert.ToDouble(equal.Text) < 80) { equal.BackColor = Color.GreenYellow; }
                        else if (Convert.ToDouble(equal.Text) >= 80 && Convert.ToDouble(equal.Text) < 90) { equal.BackColor = Color.LightGreen; }
                        else if (Convert.ToDouble(equal.Text) >= 90 && Convert.ToDouble(equal.Text) <= 100) { equal.BackColor = Color.Green; }

                //Color change conditions for false prediction
                if (Convert.ToDouble(unequal.Text) >= 0 && Convert.ToDouble(unequal.Text) < 10) { unequal.BackColor = Color.Green; }
                        else if (Convert.ToDouble(unequal.Text) >= 10 && Convert.ToDouble(unequal.Text) < 20) { unequal.BackColor = Color.LightGreen; }
                        else if (Convert.ToDouble(unequal.Text) >= 20 && Convert.ToDouble(unequal.Text) < 30) { unequal.BackColor = Color.GreenYellow; }
                        else if (Convert.ToDouble(unequal.Text) >= 30 && Convert.ToDouble(unequal.Text) < 40) { unequal.BackColor = Color.Yellow; }
                        else if (Convert.ToDouble(unequal.Text) >= 40 && Convert.ToDouble(unequal.Text) <= 50) { unequal.BackColor = Color.Red; }
                    }

            //Chart visualization
            string[] seriesArray = { true_predict.Text, false_predict.Text }; //Rows
            double[] pointsArray = { Convert.ToDouble(equal.Text), Convert.ToDouble(unequal.Text) }; //Columns
            diagram.BorderlineDashStyle = ChartDashStyle.Solid;
            diagram.ChartAreas[0].AxisX.Enabled = AxisEnabled.False;
            diagram.ChartAreas[0].AxisY.MajorGrid.LineColor = SystemColors.ControlLight;
            diagram.ChartAreas[0].AxisY.Minimum = 0;
            diagram.ChartAreas[0].AxisY.Maximum = 100;
            diagram.ChartAreas[0].AxisY.MajorGrid.Interval = 10;
            diagram.ChartAreas[0].AxisY.Title = "Churn prediction in banking sphere";
            diagram.BackColor = Color.PaleTurquoise;
            diagram.BackSecondaryColor = Color.White;
            diagram.BackGradientStyle = GradientStyle.DiagonalRight;
            diagram.BorderlineColor = Color.White;
            diagram.ChartAreas[0].BackColor = Color.Orange;
            diagram.Palette = ChartColorPalette.Bright;
            diagram.Titles.Add("Accuracy of " + Path.GetFileNameWithoutExtension(filename.Text));
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
            
            findTime.Stop(); // Stop calculation time
            equal.Text += " %";
            unequal.Text += " %";
            visualization_time.Text = Convert.ToString(findTime.Elapsed.Ticks / 10000000);
            
            diagram.Visible = true;
            file_log.Visible = true;
            start.Enabled = false;
            save_diagram.Visible = true;
            close.Visible = true;
        }

        private void Visualization_version2_Load(object sender, EventArgs e)
        {
            this.AutoSize = true;
        }

        private void Close_Click(object sender, EventArgs e) //Press "Close" button
        {
            this.Close();
        }

        private void Save_Click(object sender, EventArgs e) //Press "Save output image" button
        {
            //Save as Png
            save_diagram.Enabled = false;
            var image = Path.ChangeExtension(file.Text, "png");
            diagram.SaveImage(image, System.Drawing.Imaging.ImageFormat.Png);
        }
    }
    }
