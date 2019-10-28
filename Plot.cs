using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

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

        public async Task<string> Draw()
        {
            Bitmap bmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bmp);


            TypeConverter converter = new TypeConverter();
            Color lightGrey = ColorTranslator.FromHtml("#E0E0E0");




            g.FillRectangle(new SolidBrush(lightGrey), 0, 0, width, height);
            g.DrawRectangle(new Pen(Color.Red), new Rectangle(19, 19, width - 38, height - 38));
            g.FillRectangle(new SolidBrush(Color.White), 20, 20, width - 40, height - 40);

            g.DrawLine(new Pen(Color.Blue), width / 2, 20, width / 2, height - 20);
            g.DrawLine(new Pen(Color.Blue), 20, height / 2, width - 20, height / 2);

            double xScale = (width - 100) / (2 * Vectors.DataSets[0].Max);
            double yScale = (height - 100) / (2 * Vectors.DataSets[1].Max);


            g.DrawString("0", new Font("Sans Serif", 12), new SolidBrush(Color.Gray), width / 2 + 10, height / 2 + 10);

            g.DrawString(-Vectors.DataSets[0].Max + "", new Font("Sans Serif", 12), new SolidBrush(Color.Gray), Justify(xScale, -Vectors.DataSets[0].Max, Axis.x), height / 2 + 10);
            g.DrawString(Vectors.DataSets[0].Max + "", new Font("Sans Serif", 12), new SolidBrush(Color.Gray), Justify(xScale, Vectors.DataSets[0].Max, Axis.x), height / 2 + 10);

            g.DrawString(-Vectors.DataSets[1].Max + "", new Font("Sans Serif", 12), new SolidBrush(Color.Gray), width / 2 + 10, Justify(yScale, -Vectors.DataSets[1].Max, Axis.y));
            g.DrawString(Vectors.DataSets[1].Max + "", new Font("Sans Serif", 12), new SolidBrush(Color.Gray), width / 2 + 10, Justify(yScale, Vectors.DataSets[1].Max, Axis.y));


            foreach (var curr in Vectors.Vectors)
            {
                int x = Justify(xScale, curr[0], Axis.x);
                int y = Justify(yScale, curr[1], Axis.y);

                int r = 4;
                g.FillEllipse(new SolidBrush(Color.Black), x, y, r, r);

                //Console.WriteLine($"{x},{y}");
            }

            if (!Directory.Exists(Directory.GetCurrentDirectory() + "/plots"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/plots");
            }
            string filename = $"{Directory.GetCurrentDirectory()}/plots/plot_{ DateTime.Now.ToShortDateString() }___{ DateTime.Now.ToLongTimeString().Replace(':', '-').Replace(' ', '_')}.bmp";
            FileStream stream = new FileStream(filename, FileMode.Create);
            bmp.Save(stream, ImageFormat.Bmp);
            await stream.FlushAsync();

            return filename;
        }
    }
}
