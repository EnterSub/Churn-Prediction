using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace C
{
    public partial class Visualization_version1 : Form
    {
        public Visualization_version1()
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
                    file_log.Text = filename.Text;
                    open_file.Enabled = false;
                    true_predict.Visible = true;
                    false_predict.Visible = true;
                    equal.Visible = true;
                    unequal.Visible = true;
                    gini_index_caption.Visible = true;
                    gini_index.Visible = true;
                    start.Enabled = true;
                    start.Visible = true;
                    progress.Visible = true;
                }
            }
        }

        private void Start_Click(object sender, EventArgs e) //Press "Calculate" button
        {
            int string_count = 1; //String number
            int P = 0; //Number of coincidences
            int N = 0; //Number of differences
            string line; //String

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
                        if (stringNumbers[0] == stringNumbers[1]) //If matched
                    {
                        string_count += 1; P++; progress.Value += progress.Step;
                        progress.Maximum += progress.Step;
                        file_log.Text += Environment.NewLine + expression + "   +"; //String visualization
                    }
                        else //Else
                            {
                        string_count += 1; N++; progress.Value += progress.Step;
                        progress.Maximum += progress.Step;
                        file_log.Text += Environment.NewLine + expression + "   -"; //String visualization
                    }
                }
                
                progress.Maximum = P + N; //Dynamic change of the maximum of the progress bar

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

                float NP = P + N; //Overall probability
                float P1 = 1 / NP * P; //True prediction
                float N1 = 1 / NP * N; //False prediction

                equal.Text = Convert.ToString(P1);
                unequal.Text = Convert.ToString(N1);
                gini_index.Text = Convert.ToString(2 * P1 - 1); //Gini index

                //Color change conditions for true prediction
                if (Convert.ToDouble(equal.Text) >= 0.5 && Convert.ToDouble(equal.Text) < 0.6) { equal.BackColor = Color.Red; }
                        else if (Convert.ToDouble(equal.Text) >= 0.6 && Convert.ToDouble(equal.Text) < 0.7) { equal.BackColor = Color.Yellow; }
                        else if (Convert.ToDouble(equal.Text) >= 0.7 && Convert.ToDouble(equal.Text) < 0.8) { equal.BackColor = Color.GreenYellow; }
                        else if (Convert.ToDouble(equal.Text) >= 0.8 && Convert.ToDouble(equal.Text) < 0.9) { equal.BackColor = Color.LightGreen; }
                        else if (Convert.ToDouble(equal.Text) >= 0.9 && Convert.ToDouble(equal.Text) <= 1) { equal.BackColor = Color.Green; }

                //Color change conditions for false prediction
                if (Convert.ToDouble(unequal.Text) >= 0 && Convert.ToDouble(unequal.Text) < 0.1) { unequal.BackColor = Color.Green; }
                        else if (Convert.ToDouble(unequal.Text) >= 0.1 && Convert.ToDouble(unequal.Text) < 0.2) { unequal.BackColor = Color.LightGreen; }
                        else if (Convert.ToDouble(unequal.Text) >= 0.2 && Convert.ToDouble(unequal.Text) < 0.3) { unequal.BackColor = Color.GreenYellow; }
                        else if (Convert.ToDouble(unequal.Text) >= 0.3 && Convert.ToDouble(unequal.Text) < 0.4) { unequal.BackColor = Color.Yellow; }
                        else if (Convert.ToDouble(unequal.Text) >= 0.4 && Convert.ToDouble(unequal.Text) <= 0.5) { unequal.BackColor = Color.Red; }

                //Color change conditions for Gini index
                if (Convert.ToDouble(gini_index.Text) >= 0 && Convert.ToDouble(gini_index.Text) < 0.2) { gini_index.BackColor = Color.Red; }
                        else if (Convert.ToDouble(gini_index.Text) >= 0.2 && Convert.ToDouble(gini_index.Text) < 0.4) { gini_index.BackColor = Color.Yellow; }
                        else if (Convert.ToDouble(gini_index.Text) >= 0.4 && Convert.ToDouble(gini_index.Text) < 0.6) { gini_index.BackColor = Color.GreenYellow; }
                        else if (Convert.ToDouble(gini_index.Text) >= 0.6 && Convert.ToDouble(gini_index.Text) < 0.8) { gini_index.BackColor = Color.LightGreen; }
                        else if (Convert.ToDouble(gini_index.Text) >= 0.8 && Convert.ToDouble(gini_index.Text) <= 1) { gini_index.BackColor = Color.Green; }
                    }

            //Chart visualization
            string[] seriesArray = { true_predict.Text, false_predict.Text, gini_index_caption.Text }; //Rows
            double[] pointsArray = { Convert.ToDouble(equal.Text), Convert.ToDouble(unequal.Text), Convert.ToDouble(gini_index.Text) }; //Columns
            diagram.BorderlineDashStyle = ChartDashStyle.Solid;
            diagram.ChartAreas[0].AxisX.Enabled = AxisEnabled.False;
            diagram.ChartAreas[0].AxisY.MajorGrid.LineColor = SystemColors.ControlLight;
            diagram.ChartAreas[0].AxisY.Minimum = 0;
            diagram.ChartAreas[0].AxisY.Maximum = 1;
            diagram.ChartAreas[0].AxisY.MajorGrid.Interval = 0.1;
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

            diagram.Visible = true;
            file_log.Visible = true;
            start.Enabled = false;
            save_diagram.Visible = true;
            close.Visible = true;
        }

        private void Visualization_version1_Load(object sender, EventArgs e)
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
