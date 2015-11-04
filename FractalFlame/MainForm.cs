using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace FractalFlame
{
    public partial class MainForm : Form
    {
        private Random _random;
        private AffineTransformation _affine;

        public MainForm()
        {
            InitializeComponent();
            PrepareComboBox();
        }

        private void PrepareComboBox()
        {
            foreach (String functionName in Function.GetDictionaryKeys())
                comboBox1.Items.Add(functionName);
            comboBox1.SelectedItem = comboBox1.Items[0];
        }

        private void PrepareRandom()
        {
            if (textBox2.Text.Equals(""))
            {
                _random = new Random();
                return;
            }

            int number = 0;
            for (int i = 0; i < textBox2.Text.Length; i++)
            {
                number += (int)textBox2.Text[i] * (i + 1);
            }
            _random = new Random(number);
        }

        private void PrepareAffineTransformations()
        {
            _affine = new AffineTransformation(Convert.ToInt32(textBox1.Text), _random);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PrepareRandom();
            PrepareAffineTransformations();

            
            RenderImage();
            //(new Thread(ShowProgress)).Start();
        }

        private struct ImageElement
        {
            public int R;
            public int G;
            public int B;
            public int Counter;
            public double Normal;
        }

        private void ShowProgress()
        {

        }
        int step = -20;
        private void RenderImage()
        {
            Size imageSize = new System.Drawing.Size((int)(pictureBox.Width * numericUpDown1.Value), (int)(pictureBox.Height * numericUpDown1.Value));
            progressBar1.Maximum = imageSize.Width * imageSize.Height * (int)numericUpDown2.Value;
            
            ImageElement[,] pixels = new ImageElement[imageSize.Width, imageSize.Height];

            double _yMax = 1, _yMin = -_yMax;
            double _xMax = (double)imageSize.Height / imageSize.Width, _xMin = -_xMax;


            double newX = _random.NextDouble() * (_xMax - _xMin) + _xMin;
            double newY = _random.NextDouble() * (_yMax - _yMin) + _yMin;

            for (step = -20; step < (imageSize.Width * imageSize.Height * (int)numericUpDown2.Value); step++)
            {
                //Выбираем одно из аффинных преобразований
                int i = _random.Next(Convert.ToInt32(textBox1.Text));
                //и применяем его
                newX = _affine.GetCoeffecients(i).A * newX + _affine.GetCoeffecients(i).B * newY + _affine.GetCoeffecients(i).C;
                newY = _affine.GetCoeffecients(i).D * newX + _affine.GetCoeffecients(i).E * newY + _affine.GetCoeffecients(i).F;
                //Применяем нелинейное преобразование
                Vector p = Function.GetResult((String)comboBox1.SelectedItem, newX, newY);
                newX = p.X;
                newY = p.Y;

                if (step >= 0 && _xMin <= newX && newX <= _xMax && _yMin <= newY && newY <= _yMax)
                {
                    //Вычисляем координаты точки, а затем задаем цвет
                    int x1 = (int)(imageSize.Width * (1 - ((_xMax - newX) / (_xMax - _xMin))));
                    int y1 = (int)(imageSize.Height * (1 - ((_yMax - newY) / (_yMax - _yMin))));
                    //Если точка попала в область изображения
                    if (x1 < imageSize.Width && y1 < imageSize.Height)
                    {
                        //то проверяем, первый ли раз попали в нее
                        if (pixels[x1, y1].Counter == 0)
                        {
                            //Попали в первый раз, берем стартовый цвет у соответствующего аффинного преобразования
                            pixels[x1, y1].R = _affine.GetCoeffecients(i).Color.R;
                            pixels[x1, y1].G = _affine.GetCoeffecients(i).Color.G;
                            pixels[x1, y1].B = _affine.GetCoeffecients(i).Color.B;
                        }
                        else
                        {
                            //Попали не в первый раз, считаем так:
                            pixels[x1, y1].R = (pixels[x1, y1].R + _affine.GetCoeffecients(i).Color.R) / 2;
                            pixels[x1, y1].G = (pixels[x1, y1].G + _affine.GetCoeffecients(i).Color.G) / 2;
                            pixels[x1, y1].B = (pixels[x1, y1].B + _affine.GetCoeffecients(i).Color.B) / 2;
                        }
                        //Увеличиваем счетчик точки на единицу
                        pixels[x1, y1].Counter++;
                    }

                    //progressBar1.Value = step;
                }
            }

            //КОРРЕКЦИЯ
            double max = 0.0;
            double gamma = 2.2;
            for (int y = 0; y < imageSize.Height; y++)
            {
                for (int x = 0; x < imageSize.Width; x++)
                {
                    if (pixels[x, y].Counter != 0)
                    {
                        pixels[x, y].Normal = Math.Log10(pixels[x, y].Counter);
                        if (pixels[x, y].Normal > max)
                            max = pixels[x, y].Normal;
                    }
                }
            }
            for (int y = 0; y < imageSize.Height; y++)
            {
                for (int x = 0; x < imageSize.Width; x++)
                {
                    pixels[x, y].Normal /= max;
                    pixels[x, y].R = (int)(pixels[x, y].R * Math.Pow(pixels[x, y].Normal, (1.0 / gamma)));
                    pixels[x, y].G = (int)(pixels[x, y].G * Math.Pow(pixels[x, y].Normal, (1.0 / gamma)));
                    pixels[x, y].B = (int)(pixels[x, y].B * Math.Pow(pixels[x, y].Normal, (1.0 / gamma)));
                }
            }
            //

            Bitmap bmp = new Bitmap(imageSize.Width, imageSize.Height);
            for (int y = 0; y < imageSize.Height; y++)
            {
                for (int x = 0; x < imageSize.Width; x++)
                {
                    int r = (int)pixels[x, y].R;
                    int g = (int)pixels[x, y].G;
                    int b = (int)pixels[x, y].B;

                    bmp.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
            pictureBox.Image = bmp;
        }
    }
}
