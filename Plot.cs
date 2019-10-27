using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

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

        public void Draw()
        {
            Bitmap bmp = new Bitmap(600, 400);
            Graphics g = Graphics.FromImage(bmp);


            TypeConverter converter = new TypeConverter();
            Color lightGrey= ColorTranslator.FromHtml("#E0E0E0");

            g.FillRectangle(new SolidBrush(lightGrey), 0, 0, bmp.Width, bmp.Height);
            g.DrawRectangle(new Pen(Color.Red), new Rectangle(19, 19, bmp.Width - 38, bmp.Height - 38));
            g.FillRectangle(new SolidBrush(Color.White), 20, 20, bmp.Width - 40, bmp.Height - 40);


            FileStream stream = new FileStream($"{Directory.GetCurrentDirectory()}/plots/plot_{ DateTime.Now.ToShortDateString() } { DateTime.Now.ToLongTimeString().Replace(':', '_') }.bmp", FileMode.Create);
            bmp.Save(stream, ImageFormat.Bmp);
        }
    }
}
