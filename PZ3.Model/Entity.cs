using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PZ3.Model
{
    public class Entity
    {
        private UInt64 id;
        private string name;
        private double x;
        private double y;
        private double mapX;
        private double mapY;
        private int numConnctions = 0;

        public UInt64 Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }
        public double MapX { get => mapX; set => mapX = value; }
        public double MapY { get => mapY; set => mapY = value; }
        [XmlIgnore]
        public int NumConnctions { get => numConnctions; set => numConnctions = value; }

        public Entity() { }
    }
}
