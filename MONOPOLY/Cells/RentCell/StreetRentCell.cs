using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MONOPOLY
{
    enum HouseCount { Empty, One, Two, Three, Four, Five }
    class StreetRentCell : RentCell
    {
        public int HouseCost { get; }
        public bool CanBuildHouses { get; set; } = false;
        public HouseCount HouseCount { get; set; } = HouseCount.Empty;

        public StreetRentCell(string name, int cost, int rent, int houseCost, StreetType streetType) :
            base(name, cost, rent, streetType) => HouseCost = houseCost;

        public XElement StreetXElement(string type) =>
            new XElement(type,
                new XElement("name", Name),
                new XElement("cost", Cost),
                new XElement("rent", Rent),
                new XElement("houseCost", HouseCost),
                new XElement("streetType", StreetType)
            );
    }
}
