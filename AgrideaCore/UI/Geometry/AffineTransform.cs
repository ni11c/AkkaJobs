using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agridea.Geometry
{
    public class AffineTransform
    {
        public double Scale { get; set; }

        public double Gx { get; set; }

        public double Gy { get; set; }

        public double Cx { get; set; }

        public double Cy { get; set; }

        public AffineTransform(double scale, double gx, double gy, double cx, double cy)
        {
            Scale = scale;
            Gx = gx;
            Gy = gy;
            Cx = cx;
            Cy = cy;
        }

        public Point Transform(double x, double y)
        {
            double dx = Cx - Scale * Gx;
            double dy = Cy + Scale * Gy;
            var newx = x * Scale + dx;
            var newy = y * (-Scale) + dy;
            return new Point(newx, newy);
        }
    }

    public class Point
    {
        public int X { get; set; }

        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point(double x, double y)
        {
            X = (int)(Math.Floor(x));
            Y = (int)(Math.Floor(y));
        }

        public static Point UpperLeftCorner { get { return new Point(0, 0); } }
    }
}