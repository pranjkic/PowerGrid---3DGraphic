using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PZ3.Model
{
    [Serializable]
    [XmlRoot("NetworkModel")]
    public class LineEntity : Entity
    {
        private bool isUnderground;
        private double r;
        private string conductorMaterial;
        private string lineType;
        private int thermalConstantHeat;
        private UInt64 firstEnd;
        private UInt64 secondEnd;
        private List<Point> vertices = new List<Point>();

        public bool IsUnderground { get => isUnderground; set => isUnderground = value; }
        public double R { get => r; set => r = value; }
        public string ConductorMaterial { get => conductorMaterial; set => conductorMaterial = value; }
        public string LineType { get => lineType; set => lineType = value; }
        public int ThermalConstantHeat { get => thermalConstantHeat; set => thermalConstantHeat = value; }
        public UInt64 FirstEnd { get => firstEnd; set => firstEnd = value; }
        public UInt64 SecondEnd { get => secondEnd; set => secondEnd = value; }

        [XmlArray("Vertices"), XmlArrayItem(typeof(Point), ElementName = "Point")]
        public List<Point> Vertices { get => vertices; set => vertices = value; }

        public LineEntity()
        {
            this.vertices = new List<Point>();
        }

        public override string ToString()
        {
            return String.Format($"{Id}, {Name}, {ConductorMaterial}, {LineType}");
        }
    }
}
