using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PZ3.Model
{
    [Serializable]
    public class NetworkModel
    {
        private List<SubstationEntity> substations = new List<SubstationEntity>();
        private List<NodeEntity> nodes = new List<NodeEntity>();
        private List<SwitchEntity> switches = new List<SwitchEntity>();
        private List<LineEntity> lines = new List<LineEntity>();

        [XmlArray("Substations"), XmlArrayItem(typeof(SubstationEntity), ElementName = "SubstationEntity")]
        public List<SubstationEntity> Substations { get => substations; set => substations = value; }

        [XmlArray("Nodes"), XmlArrayItem(typeof(NodeEntity), ElementName = "NodeEntity")]
        public List<NodeEntity> Nodes { get => nodes; set => nodes = value; }

        [XmlArray("Switches"), XmlArrayItem(typeof(SwitchEntity), ElementName = "SwitchEntity")]
        public List<SwitchEntity> Switches { get => switches; set => switches = value; }

        [XmlArray("Lines"), XmlArrayItem(typeof(LineEntity), ElementName = "LineEntity")]
        public List<LineEntity> Lines { get => lines; set => lines = value; }

        public NetworkModel()
        {
            Substations = new List<SubstationEntity>();
            Nodes = new List<NodeEntity>();
            Switches = new List<SwitchEntity>();
            Lines = new List<LineEntity>();
        }
    }
}
