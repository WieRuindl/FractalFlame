using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace FractalFlame
{
    static class Function
    {
        public static Vector GetResult(String functionName, double x, double y)
        {
            if (!_dictionary.ContainsKey(functionName))
                throw new ArgumentException(string.Format("Function {0} is invalid", functionName), "functionName");

            return _dictionary[functionName](x, y);
        }

        private static Dictionary<string, Func<double, double, Vector>> _dictionary =
    new Dictionary<string, Func<double, double, Vector>>
    {
        { "Sin", (x, y) => Functions.Sin(x, y) },
        { "Sphere", (x, y) => Functions.Sphere(x, y) },
        { "Disk", (x, y) => Functions.Disk(x, y) },
        { "EyeFish", (x, y) => Functions.EyeFish(x, y)},
        { "Spiral", (x, y) => Functions.Spiral(x, y)},
        { "Cosine", (x, y) => Functions.Cosine(x, y) },
    };

        public static String[] GetDictionaryKeys()
        {
            return _dictionary.Keys.ToArray();
        }

        private static class Functions
        {
            public static Vector Sin(double x, double y)
            {
                double xNew = Math.Sin(x);
                double yNew = Math.Sin(y);
                return new Vector(xNew, yNew);
            }

            public static Vector Sphere(double x, double y)
            {
                double xNew = x / (Math.Pow(x, 2) + Math.Pow(y, 2));
                double yNew = y / (Math.Pow(x, 2) + Math.Pow(y, 2));
                return new Vector(xNew, yNew);
            }

            public static Vector Disk(double x, double y)
            {
                double xNew = 1 / Math.PI * Math.Atan((double)y / x) * Math.Sin(Math.PI * Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)));
                double yNew = 1 / Math.PI * Math.Atan((double)y / x) * Math.Cos(Math.PI * Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)));
                return new Vector(xNew, yNew);
            }

            public static Vector EyeFish(double x, double y)
            {
                double r = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
                double xNew = 2 / (r + 1) * x;
                double yNew = 2 / (r + 1) * y;
                return new Vector(xNew, yNew);
            }

            public static Vector Spiral(double x, double y)
            {
                double r = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
                double theta = Math.Atan(x / y);
                double xNew = 1 / r * Math.Cos(theta) + Math.Sin(r);
                double yNew = 1 / r * Math.Sin(theta) - Math.Cos(r);
                return new Vector(xNew, yNew);
            }

            public static Vector Cosine(double x, double y)
            {
                double xNew = Math.Cos(Math.PI * x) * Math.Cosh(y);
                double yNew = -Math.Sin(Math.PI * x) * Math.Sinh(y);
                return new Vector(xNew, yNew);
            }
        }
    }
}
