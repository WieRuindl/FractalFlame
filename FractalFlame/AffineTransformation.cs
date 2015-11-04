using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace FractalFlame
{
    public struct Coefficients
    {
        public double A, B, C, D, E, F;
        public Color Color;

        public Coefficients(double a, double b, double c, double d, double e, double f, Color color)
        {
            A = a;
            B = b;
            C = c;
            D = d;
            E = e;
            F = f;
            Color = color;
        }
    }

    class AffineTransformation
    {
        private Coefficients[] _coefficients;

        public AffineTransformation(int count, Random random)
        {
            _coefficients = new Coefficients[count];

            for (int i = 0; i < count; i++)
            {
                double 
                    a = random.NextDouble() * 2 - 1,
                    b = random.NextDouble() * 2 - 1,
                    c = random.NextDouble() * 2 - 1,
                    d = random.NextDouble() * 2 - 1,
                    e = random.NextDouble() * 2 - 1,
                    f = random.NextDouble() * 2 - 1;
                
                //a^2+b^2+d^2+e^2<1+(ae-bd)^2
                do
                {
                    //a^2+d^2<1
                    do
                    {
                        a = random.NextDouble() * 2 - 1;
                        d = random.NextDouble() * 2 - 1;
                    }
                    while (!((Math.Pow(a, 2) + Math.Pow(d, 2)) < 1));
                    //b^2+e^2<1
                    do
                    {
                        b = random.NextDouble() * 2 - 1;
                        e = random.NextDouble() * 2 - 1;
                    }
                    while (!((Math.Pow(b, 2) + Math.Pow(e, 2)) < 1));
                }
                while (!((Math.Pow(a, 2) + Math.Pow(b, 2) + Math.Pow(d, 2) + Math.Pow(e, 2)) < (1 + Math.Pow(a * e - b * d, 2))));
                
                c = random.NextDouble() * 2 - 1;
                f = random.NextDouble() * 2 - 1;

                int rc = 0, gc = 0, bc = 0, mid = 0;
                while (Math.Sqrt(Math.Pow(rc - mid, 2) + Math.Pow(gc - mid, 2) + Math.Pow(bc - mid, 2)) < 50)
                {
                    rc = random.Next(0, 255);
                    gc = random.Next(0, 255);
                    bc = random.Next(0, 255);
                    mid = (int)((rc + gc + bc) / 3);
                }
                
                _coefficients[i] = new Coefficients(a, b, c, d, e, f,  Color.FromArgb(rc, gc, bc));
            }
        }

        public Coefficients GetCoeffecients(int i)
        {
            if (i < 0 || i >= _coefficients.Length) throw new Exception("Error in AffineTransformation.GetCoeffecients()");

            return _coefficients[i];
        }
    }
}
