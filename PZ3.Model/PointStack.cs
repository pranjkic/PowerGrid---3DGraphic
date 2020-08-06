using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ3.Model
{
    public class PointStack
    {
        private double x;
        private double y;
        private int count = 0;

        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }
        public int Count { get => count; set => count = value; }

        public PointStack() { }

        public PointStack(double x, double y, int count)
        {
            X = x;
            Y = y;
            Count = count;
        }
    }
}
