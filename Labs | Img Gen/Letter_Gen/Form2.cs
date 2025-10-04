using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _1лаба
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }
   


        private void button1_Click(object sender, EventArgs e)
        {
            Graphics g = pictureBox1.CreateGraphics();
          
            //буква В
            Bresenham4Line(g, Color.Black, Width / 2 + 50, Height / 2 - 40, Width / 2 + 95, Height / 2 - 40);
            Bresenham4Line(g, Color.Black, Width / 2 + 65, Height / 2 + 40, Width / 2 + 95, Height / 2 + 40);
            Bresenham4Line(g, Color.Black, Width / 2 + 95, Height / 2 - 40, Width / 2 + 110, Height / 2 - 20);
            Bresenham4Line(g, Color.Black, Width / 2 + 50, Height / 2 - 38, Width / 2 + 50, Height / 2 - 95);
            Bresenham4Line(g, Color.Black, Width / 2 + 110, Height / 2 - 20, Width / 2 + 110, Height / 2 - 60);
            Bresenham4Line(g, Color.Black, Width / 2 + 50, Height / 2 + 18, Width / 2 + 65, Height / 2 + 40);
            Bresenham4Line(g, Color.Black, Width / 2 + 110, Height / 2 + 18, Width / 2 + 95, Height / 2 + 40);
            Bresenham4Line(g, Color.Black, Width / 2 + 65, Height / 3 - 40, Width / 2 + 95, Height / 3 - 40);
            Bresenham4Line(g, Color.Black, Width / 2 + 95, Height / 3 - 40, Width / 2 + 110, Height / 3 - 20);
            Bresenham4Line(g, Color.Black, Width / 2 + 65, Height / 3 - 40, Width / 2 + 50, Height / 3 - 20);
            Bresenham4Line(g, Color.Black, Width / 2 + 50, Height / 3 - 20, Width / 2 + 50, Height / 3 - 100);
            Bresenham4Line(g, Color.Black, Width / 2 + 110, Height / 3 - 20, Width / 2 + 110, Height / 3 - 60);
            Bresenham4Line(g, Color.Black, Width / 2 + 110, Height / 3 + 18, Width / 2 + 95, Height / 3 + 40);

            //
            RasterAlgorithm.BresenhamCircle(g, Color.Green, 150, 150, 3 * (2 + 9));
            RasterAlgorithm.BresenhamCircle(g, Color.Yellow, 150, 150, 5 * (2 + 9));
            RasterAlgorithm.BresenhamCircle(g, Color.Red, 150, 150, 10 * (2 + 9));



            //Буква Ц

            Bresenham4Line(g, Color.Black, Width / 2 - 90, Height / 2 - 120, Width / 2 - 90, Height / 2 + 25);
            Bresenham4Line(g, Color.Black, Width / 2 - 10, Height / 2 - 120, Width / 2 - 10, Height / 2 + 25);
            Bresenham4Line(g, Color.Black, Width / 2 - 90, Height / 2 + 25, Width / 2 + 10, Height / 2 + 25);
            Bresenham4Line(g, Color.Black, Width / 2 + 10, Height / 2 + 25, Width / 2 + 10, Height / 2);



        }
        private static void PutPixel(Graphics g, Color col, int x, int y, int alpha)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(alpha, col)), x, y, 1, 1);
        }

        //Статический метод, реализующий отрисовку 4-связной линии по алгоритму Брезенхема
        static public void Bresenham4Line(Graphics g, Color clr, int x0, int y0, int x1, int y1)
            {
            //Изменения координат
            int dx = (x1 > x0) ? (x1 - x0) : (x0 - x1);
            int dy = (y1 > y0) ? (y1 - y0) : (y0 - y1);
            //Направление приращения
            int sx = (x1 >= x0) ? (1) : (-1);
            int sy = (y1 >= y0) ? (1) : (-1);

            if (dy < dx)
            {
                int d = (dy << 1) - dx;
                int d1 = dy << 1;
                int d2 = (dy - dx) << 1;
                PutPixel(g, clr, x0, y0, 255);
                int x = x0 + sx;
                int y = y0;
                for (int i = 1; i <= dx; i++)
                {
                    if (d > 0)
                    {
                        d += d2;
                        y += sy;
                    }
                    else
                        d += d1;
                    PutPixel(g, clr, x, y, 255);
                    x++;
                }
            }
            else
            {
                int d = (dx << 1) - dy;
                int d1 = dx << 1;
                int d2 = (dx - dy) << 1;
                PutPixel(g, clr, x0, y0, 255);
                int x = x0;
                int y = y0 + sy;
                for (int i = 1; i <= dy; i++)
                {
                    if (d > 0)
                    {
                        d += d2;
                        x += sx;
                    }
                    else
                        d += d1;
                    PutPixel(g, clr, x, y, 255);
                    y++;
                }
            }
        }
        static public void LukLine(Graphics g, Color clr, int Xd, int Xf, int Yd, int Yf)
        {
            int Dx, Dy, Cumul;
            int Xinc, Yinc, X, Y;
            int col;
            int i;

            X = Xd; Y = Yd; col = 4;
            PutPixel(g, clr, X, Y, col);
            if (Xd < Xf) Xinc = 1; else Xinc = -1;
            if (Yd < Yf) Yinc = 1; else Yinc = -1;
            Dx = Math.Abs(Xd - Xf);
            Dy = Math.Abs(Yd - Yf);
            if (Dx > Dy)
            {
                Cumul = Dx / 2;
                for (i = 0; i < Dx; i++)
                {
                    X = X + Xinc;
                    Cumul = Cumul + Dy;
                    if (Cumul >= Dx)
                    {
                        Cumul = Cumul - Dx;
                        Y = Y + Yinc;
                    }
                    PutPixel(g, clr, X, Y, col);
                }
            }
            else
            {
                Cumul = Dy / 2;
                for (i = 0; i < Dy; i++)
                {
                    Y = Y + Yinc;
                    Cumul = Cumul + Dx;
                    if (Cumul >= Dy)
                    {
                        Cumul = Cumul - Dy;
                        X = X + Xinc;
                        PutPixel(g, clr, X, Y, col);
                    }
                }
            }
        }
        static class RasterAlgorithm
        {

            public static void BresenhamCircle(Graphics g, Color clr, int _x, int _y, int radius)
            {
                int x = 0, y = radius, gap = 0, delta = (2 - 2 * radius);
                while (y >= 0)
                {
                    PutPixel(g, clr, _x + x, _y + y, 255);
                    PutPixel(g, clr, _x + x, _y - y, 255);
                    PutPixel(g, clr, _x - x, _y - y, 255);
                    PutPixel(g, clr, _x - x, _y + y, 255);
                    gap = 2 * (delta + y) - 1;
                    if (delta < 0 && gap <= 0)
                    {
                        x++;
                        delta += 2 * x + 1;
                        continue;
                    }
                    if (delta > 0 && gap > 0)
                    {
                        y--;
                        delta -= 2 * y + 1;
                        continue;
                    }
                    x++;
                    delta += 2 * (x - y);
                    y--;
                }
            }
            //Метод, устанавливающий пиксел на форме с заданными цветом и прозрачностью
            private static void PutPixel(Graphics g, Color col, int x, int y, int alpha)
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(alpha, col)), x, y, 1, 1);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
