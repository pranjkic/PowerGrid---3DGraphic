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
    public class SubstationEntity : Entity
    {
        public SubstationEntity() : base() { }

        public override string ToString()
        {
            return String.Format($"SUBSTATION\nId: {Id}\nName: {Name}\nConnections={NumConnctions}");
        }
    }
}
