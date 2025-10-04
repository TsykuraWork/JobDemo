using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Laba7
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Image Source = null;
        // количество элементов в палитре
        int NNewColors = 16;
        int NOldColors = 256 * 256 * 256;
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            Source = Image.FromFile(openFileDialog1.FileName);
            Run();
        }

        void Run()
        {
            pictureBox1.Image = Source;
         pictureBox2.Image= static_palitr(Source);
            pictureBox3.Image =filtr(static_palitr(Image.FromFile(openFileDialog1.FileName)));

            pictureBox4.Image = optimpalitr(Image.FromFile(openFileDialog1.FileName));

            pictureBox5.Image = filtr(optimpalitr(Image.FromFile(openFileDialog1.FileName)));

        }

        public Image optimpalitr(Image image)
        {
            Bitmap img = new Bitmap(image);
            Color[] palitra = new Color[NNewColors];
            int value = 0;
            Dictionary<int, int> keyValuePairs = new Dictionary<int, int>();
            int px = 0;
            for (var i = 0; i < img.Width; i++)
                for (var j = 0; j < img.Height; j++) // для каждого пикселя
                {
                    px = ColorTranslator.ToWin32(img.GetPixel(i, j));
                    if (keyValuePairs.TryGetValue(px,out value))
                    {
                        keyValuePairs[px] = value + 1;
                    }
                    else
                    {
                        keyValuePairs[px] = 1;
                    }

                }
            List<KeyValuePair<int, int>> myList = keyValuePairs.ToList();

            myList.Sort(
                delegate (KeyValuePair<int, int> pair1,
                KeyValuePair<int, int> pair2)
                {
                    return pair1.Value.CompareTo(pair2.Value);
                }
            );
            
            for (var i = 0; i < img.Width; i++)
                for (var j = 0; j < img.Height; j++) // для каждого пикселя
                {
                    var pixel = img.GetPixel(i, j);
                     px = ColorTranslator.ToWin32(pixel);
                    // округляем, отбрасывая дробную часть
                    var newpixel = (int)(px / (NOldColors / NNewColors));
                    img.SetPixel(i, j, ColorTranslator.FromWin32(myList[newpixel].Key));

                }
            return img;
        }


        public Image static_palitr(Image image)
        {
            Bitmap img = new Bitmap( image);
            Color[] palitra = new Color[NNewColors];
            for (int i = 0; i < NNewColors; i++)
            {
                palitra[i] = ColorTranslator.FromWin32(i * (NOldColors / NNewColors)-(NOldColors / NNewColors/2)); 
            }
            for (var i = 0; i < img.Width; i++)
                for (var j = 0;j<img.Height;j++) // для каждого пикселя
                {
                    var pixel = img.GetPixel(i, j);
                    var px = ColorTranslator.ToWin32(pixel);
                    // округляем, отбрасывая дробную часть
                    var newpixel = (int)(px / (NOldColors / NNewColors));
                    img.SetPixel(i, j, palitra[newpixel]);

                }
            return img;
        }
        public Image filtr(Image image)
        {
            Bitmap img = new Bitmap(image);
            
            for (var i = 1; i < img.Width-1; i++)
                for (var j = 1; j < img.Height-1; j++) // для каждого пикселя
                {
                    var pixel = img.GetPixel(i, j);
                    var px = ColorTranslator.ToWin32(pixel);

                    int P = (int)Math.Floor(px + 0.5);
                    double e = px - P;
                    img.SetPixel(i, j, ColorTranslator.FromWin32(P));

                    //a(i, j + 1) = a(i, j + 1) + (e * 7 / 16);
                    img.SetPixel(i, j+1, ColorTranslator.FromWin32((int)(ColorTranslator.ToWin32(img.GetPixel(i, j+1))+ (e * 7 / 16))));
                    //a(i + 1, j - 1) = a(i + 1, j - 1) + (e * 3 / 16);
                    img.SetPixel(i+1, j - 1, ColorTranslator.FromWin32((int)(ColorTranslator.ToWin32(img.GetPixel(i + 1, j - 1)) + (e * 3 / 16))));

                    //a(i + 1, j) = a(i + 1, j) + (e * 5 / 16);
                    img.SetPixel(i + 1, j, ColorTranslator.FromWin32((int)(ColorTranslator.ToWin32(img.GetPixel(i + 1, j)) + (e * 5 / 16))));

                    //a(i + 1, j + 1) = a(i + 1, j + 1) + (e * 1 / 16);
                    img.SetPixel(i + 1, j+1, ColorTranslator.FromWin32((int)(ColorTranslator.ToWin32(img.GetPixel(i + 1, j+1)) + (e * 1 / 16))));


                }
            return img;
        }
    }
}
