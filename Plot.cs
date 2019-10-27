using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace Where1.stat.Graph
{
    class Plot
    {
        VectorSet Vectors;
        public Plot(VectorSet vset) {
            Vectors = vset;
            if (Vectors.Dimensions != 2) {
                throw new NotSupportedException("Arbitrarily dimensioned plots are not supported. Currently only 2D plots are supported.");
            }
        }

        public void Draw() {
            Bitmap bmp = new Bitmap(600, 400);
            Graphics g = Graphics.FromImage(bmp);


            Pen pen = new Pen(Color.Red);
            g.DrawRectangle(pen, new Rectangle(20, 20, 300, 300));

            FileStream stream = new FileStream($"plot_{ DateTime.UtcNow.Ticks }.bmp", FileMode.Create);
            bmp.Save(stream, ImageFormat.Bmp);
        }
    }
}
