using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Where1.stat.Regression;

namespace Where1.stat.Graph
{
    public enum Axis
    {
        x,
        y
    }

    class Plot
    {
        VectorSet Vectors;
        public Plot(VectorSet vset)
        {
            Vectors = vset;
            if (Vectors.Dimensions != 2)
            {
                throw new NotSupportedException("Arbitrarily dimensioned plots are not supported. Currently only 2D plots are supported.");
            }
        }

        private const int width = 600;
        private const int height = 400;

        private int Justify(double scale, double value, Axis axis)
        {
            switch (axis)
            {
                case Axis.x:
                    return (int)(value * scale) + width / 2;
                    break;
                case Axis.y:
                    return (int)(height - ((value * scale) + height / 2));
                    break;
            }

            throw new NotSupportedException("An attempt to justify an unsupported axis was made.");
        }

        public async Task<string> Draw(RegressionLines regline = RegressionLines.none)
        {
            bool lockedScale = false;

            Bitmap bmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bmp);


            TypeConverter converter = new TypeConverter();
            Color lightGrey = ColorTranslator.FromHtml("#E0E0E0");



            StringFormat centeredString = new StringFormat();
            centeredString.Alignment = StringAlignment.Center;
            centeredString.LineAlignment = StringAlignment.Center;


            g.FillRectangle(new SolidBrush(Color.White), 20, 20, width - 40, height - 40);

            g.DrawLine(new Pen(Color.Blue), width / 2, 20, width / 2, height - 20);
            g.DrawLine(new Pen(Color.Blue), 20, height / 2, width - 20, height / 2);

            double[] originalScale = {
                (width - 100) / (2 * Vectors.DataSets[0].Max),
                (height - 100) / (2 * Vectors.DataSets[1].Max)
            };


            double[] scale = new double[Vectors.Dimensions];


            double min = originalScale[0];
            double max = originalScale[0];
            foreach (var curr in originalScale)
            {
                min = curr < min ? curr : min;
                max = curr > max ? curr : max;
            }

            if (min / max > 0.5)
            {
                lockedScale = true;
                for (int i = 0; i < scale.Length; i++)
                {
                    scale[i] = min;
                }
            }
            else
            {
                scale = originalScale;
            }

            g.DrawString("0", new Font("Sans Serif", 12), new SolidBrush(Color.Gray), width / 2 + 10, height / 2 + 10, centeredString);

            g.DrawString($"{-Vectors.DataSets[0].Max:f2}", new Font("Sans Serif", 12), new SolidBrush(Color.Gray), Justify(scale[0], -Vectors.DataSets[0].Max, Axis.x), height / 2 + 10, centeredString);
            g.DrawString($"{Vectors.DataSets[0].Max:f2}", new Font("Sans Serif", 12), new SolidBrush(Color.Gray), Justify(scale[0], Vectors.DataSets[0].Max, Axis.x), height / 2 + 10, centeredString);

            g.DrawString($"{-Vectors.DataSets[1].Max:f2}", new Font("Sans Serif", 12), new SolidBrush(Color.Gray), width / 2 + 30, Justify(scale[1], -Vectors.DataSets[1].Max, Axis.y), centeredString);
            g.DrawString($"{-Vectors.DataSets[1].Max:f2}", new Font("Sans Serif", 12), new SolidBrush(Color.Gray), width / 2 + 30, Justify(scale[1], Vectors.DataSets[1].Max, Axis.y), centeredString);

            if (lockedScale)
            {
                for (int i = 0; i < Vectors.Dimensions; i++)
                {
                    Axis axis = i == 0 ? Axis.x : Axis.y;

                    if (scale[i] != originalScale[i])
                    {
                        if (axis == Axis.x)
                        {
                            g.DrawString($"{Vectors.DataSets[i].Max * (originalScale[i] / scale[i]):f2}", new Font("Sans Serif", 12), new SolidBrush(Color.Gray), Justify(originalScale[i], Vectors.DataSets[i].Max, axis), height / 2 + 10, centeredString);
                            g.DrawString($"{-Vectors.DataSets[i].Max * (originalScale[i] / scale[i]):f2}", new Font("Sans Serif", 12), new SolidBrush(Color.Gray), Justify(originalScale[i], -Vectors.DataSets[i].Max, axis), height / 2 + 10, centeredString);
                        }
                        else
                        {
                            g.DrawString($"{Vectors.DataSets[i].Max * (originalScale[i] / scale[i]):f2}", new Font("Sans Serif", 12), new SolidBrush(Color.Gray), width / 2 + 30, Justify(originalScale[i], Vectors.DataSets[i].Max, axis), centeredString);
                            g.DrawString($"{-Vectors.DataSets[i].Max * (originalScale[i] / scale[i]):f2}", new Font("Sans Serif", 12), new SolidBrush(Color.Gray), width / 2 + 30, Justify(originalScale[i], -Vectors.DataSets[i].Max, axis), centeredString);
                        }
                    }
                }
            }


            foreach (var curr in Vectors.Vectors)
            {
                int x = Justify(scale[0], curr[0], Axis.x);
                int y = Justify(scale[1], curr[1], Axis.y);

                int r = 4;
                g.FillEllipse(new SolidBrush(Color.Black), x - r / 2, y - r / 2, r, r);

                //Console.WriteLine($"{x},{y}");
            }

            if (regline == RegressionLines.linear)
            {
                double[] lineCoefficients = new LinearRegressionLine().Calculate(Vectors);

                int xMagnitude = (int)(10 * Vectors.DataSets[0].Max);
                Point p1 = new Point((int)Justify(scale[0], -xMagnitude, Axis.x), (int)Justify(scale[1], ((-xMagnitude * lineCoefficients[1]) + lineCoefficients[0]), Axis.y));
                Point p2 = new Point((int)Justify(scale[0], xMagnitude, Axis.x), (int)Justify(scale[1], ((xMagnitude * lineCoefficients[1]) + lineCoefficients[0]), Axis.y));
                Pen reglinePen = new Pen(Brushes.Gray);
                reglinePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
                reglinePen.DashPattern = new float[] { 10, 15 };
                g.DrawLine(reglinePen, p1, p2);
            }

            g.FillRectangle(new SolidBrush(lightGrey), 0, 0, width, 20);
            g.FillRectangle(new SolidBrush(lightGrey), 0, height - 20, width, 20);

            g.FillRectangle(new SolidBrush(lightGrey), 0, 0, 20, height);
            g.FillRectangle(new SolidBrush(lightGrey), width - 20, 0, 20, height);

            g.DrawRectangle(new Pen(Color.Red), new Rectangle(19, 19, width - 38, height - 38));

            if (!Directory.Exists(Directory.GetCurrentDirectory() + "/plots"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/plots");
            }
            string filename = $"{Directory.GetCurrentDirectory()}/plots/plot_{ DateTime.Now.ToShortDateString() }___{ DateTime.Now.ToLongTimeString().Replace(':', '-').Replace(' ', '_')}.bmp";
            FileStream stream = new FileStream(filename, FileMode.Create);
            bmp.Save(stream, ImageFormat.Bmp);
            await stream.FlushAsync();
            await stream.DisposeAsync();
            
            return filename;
        }
    }
}
