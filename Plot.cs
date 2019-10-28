using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace Where1.stat.Graph
{
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

        public async Task<string> Draw()
        {
            Bitmap bmp = new Bitmap(600, 400);
            Graphics g = Graphics.FromImage(bmp);


            TypeConverter converter = new TypeConverter();
            Color lightGrey = ColorTranslator.FromHtml("#E0E0E0");




            g.FillRectangle(new SolidBrush(lightGrey), 0, 0, bmp.Width, bmp.Height);
            g.DrawRectangle(new Pen(Color.Red), new Rectangle(19, 19, bmp.Width - 38, bmp.Height - 38));
            g.FillRectangle(new SolidBrush(Color.White), 20, 20, bmp.Width - 40, bmp.Height - 40);

            g.DrawLine(new Pen(Color.Blue), bmp.Width / 2, 20, bmp.Width / 2, bmp.Height - 20);
            g.DrawLine(new Pen(Color.Blue), 20, bmp.Height / 2, bmp.Width - 20, bmp.Height / 2);

            double xScale = (bmp.Width - 100) / (4 * Vectors.DataSets[0].Max);
            double yScale = (bmp.Height - 100) / (4 * Vectors.DataSets[1].Max);

            foreach (var curr in Vectors.Vectors)
            {
                int x = (int)(curr[0] * xScale) + bmp.Width / 2;
                int y = (int)(bmp.Height - ((curr[1] * yScale) + bmp.Height / 2));
                g.FillEllipse(new SolidBrush(Color.Black), x, y, 5, 5);

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
