using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace z1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.BackColor = Color.White;
            this.KeyPreview = true;
            // Подписка на обработчик события рисования в drawingPanel:
            
            panel1.Paint += new PaintEventHandler(panel1Paint);
        }
        private void panel1Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            DrawAxis(g);
            ApplyTransformation(g);
        }
        private void ApplyTransformation(Graphics g)
        {
            // Новая матрица преобразования
            Matrix m = new Matrix();
            // Определяем центр
            m.Translate(panel1.Width / 2, panel1.Height / 2);
            // Перемещение  
            if (rbTranslation.Checked)
            {
                
                int dx = Convert.ToInt16(tbTranslationX.Text);
                int dy = -Convert.ToInt16(tbTranslationY.Text);
                m.Translate(dx, dy);
            }

            // Поворот 
            else if (rbRotation.Checked)
            {
                
                float angle = Convert.ToSingle(tbRotaionAngle.Text);
                float x = Convert.ToSingle(tbRotateAtX.Text);
                float y = -Convert.ToSingle(tbRotateAtY.Text);
                g.FillEllipse(Brushes.Black, x - 4, y - 4, 8, 8);
                m.RotateAt(angle, new PointF(x, y));
            }

            g.Transform = m;
            DrawFigure(g, Color.Black);
        }
        private void DrawFigure(Graphics g, Color color)
        {
            Pen p = new Pen(Color.Navy, 3);
            // Буква В
            g.DrawRectangle(p, 46, -72, 70, 70);
            g.DrawRectangle(p, 46, -136, 1, 132);
            g.DrawRectangle(p, 99, -136, 1, 61);
            g.DrawRectangle(p, 46, -136, 50, 1);

            // Буква Ц
            g.DrawLine(p, -100, 1, 1, 1);
            g.DrawLine(p, -100, 1, -100, -132);
            g.DrawLine(p, -20, 1, -20, -132);
            g.DrawLine(p, 1, 1, 1, 20);

        }
        private void DrawAxis(Graphics g)
        { }

        

       
        private void button1_Click_1(object sender, EventArgs e)
        {
            panel1.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Сбрасываем параметры к исходным
                tbTranslationX.Text = "0";
            tbTranslationY.Text = "0";
            tbRotaionAngle.Text = "0";
            tbRotateAtX.Text = "0";
            tbRotateAtY.Text = "0";

            panelbm.Invalidate();
        }

        private void button1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F3 && e.Modifiers == Keys.Alt)
            {
                button1.PerformClick();// имитируем нажатие button1
            }
        }

        private void button2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2 && e.Modifiers == Keys.Alt)
            {
                button2.PerformClick();// имитируем нажатие button2
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
    }
