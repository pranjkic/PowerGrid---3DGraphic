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
    public class SwitchEntity : Entity
    {
        private string status;

        public string Status { get => status; set => status = value; }

        public SwitchEntity() : base() { }

        public override string ToString()
        {
            return String.Format($"SWITCH\nId: {Id}\nName: {Name}\nConnections={NumConnctions}");
        }
    }
}
