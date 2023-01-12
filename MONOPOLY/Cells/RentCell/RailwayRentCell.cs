using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MONOPOLY
{
    class RailwayRentCell : RentCell
    {
        public RailwayRentCell(string name) :
            base(name, 200, 25, StreetType.Railway) { }

        public XElement RailwayXElement() =>
            new XElement("RailwayRentCell", new XElement("name", Name));
    }
}
