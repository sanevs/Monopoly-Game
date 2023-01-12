using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MONOPOLY
{
    enum CellName
    {
        Start, Yellow1, Yellow2, Yellow3, YellowPenalty, South,
        Green1, Green2, Green3, GreenEvent, Prison,
        Coral1, Coral2, Coral3, West,
        White1, White2, White3, WhiteEvent, Parking,
        Red1, Red2, Red3, RedEvent, North, 
        Purple1, Purple2, Purple3, GoToPrison,
        Blue1, Blue2, Blue3, BlueEvent, East, 
        Black1, Black2, BlackTax
    }
    class GameField
    {
        public static ICell[] Cells { get; set; } = new ICell[37]
        {
            //Cells[(int)CellName.Start] = 
            new StartEarnCell(),
            //Cells[(int)CellName.Yellow1] = 
            new StreetRentCell("Житная ул.", 60, 2, 50, StreetType.Yellow),
            //Cells[(int)CellName.Yellow2] = 
            new StreetRentCell("Нагатинская ул.", 60, 4, 50, StreetType.Yellow),
            //Cells[(int)CellName.Yellow3] = 
            new StreetRentCell("Нагорная ул.", 80, 4, 50, StreetType.Yellow),
            //Cells[(int)CellName.YellowPenalty] = 
            new PenaltySpendCell(),
            //Cells[(int)CellName.South] = 
            new RailwayRentCell("Южная Ж/Д"),

            //Cells[(int)CellName.Green1] = 
            new StreetRentCell("Варшавское шоссе", 100, 6, 50, StreetType.Green),
            //Cells[(int)CellName.Green2] = 
            new StreetRentCell("Огарева ул.", 100, 6, 50, StreetType.Green),
            //Cells[(int)CellName.Green3] = 
            new StreetRentCell("Первая парковая ул.", 120, 8, 50, StreetType.Green),
            //Cells[(int)CellName.GreenEvent] = 
            new RandomEventCell(),
            //Cells[(int)CellName.Prison] = 
            new PrisonCell(),

            //Cells[(int)CellName.Coral1] = 
            new StreetRentCell("Полянка ул.", 140, 10, 100, StreetType.Coral),
            //Cells[(int)CellName.Coral2] = 
            new StreetRentCell("Сретенка ул.", 140, 10, 100, StreetType.Coral),
            //Cells[(int)CellName.Coral3] = 
            new StreetRentCell("Ростовская наб.", 160, 12, 100, StreetType.Coral),
            //Cells[(int)CellName.West] = 
            new RailwayRentCell("Западная Ж/Д"),

            //Cells[(int)CellName.White1] = 
            new StreetRentCell("Рязанский пр-т", 180, 14, 100, StreetType.White),
            //Cells[(int)CellName.White2] = 
            new StreetRentCell("Вавилова ул.", 180, 14, 100, StreetType.White),
            //Cells[(int)CellName.White3] = 
            new StreetRentCell("Рублевское шоссе", 200, 16, 100, StreetType.White),
            //Cells[(int)CellName.WhiteEvent] = 
            new RandomEventCell(),
            //Cells[(int)CellName.Parking] = 
            new ParkingCell(),

            //Cells[(int)CellName.Red1] = 
            new StreetRentCell("Тверская ул.", 220, 18, 150, StreetType.Red),
            //Cells[(int)CellName.Red2] = 
            new StreetRentCell("Пушкинская ул.", 220, 18, 150, StreetType.Red),
            //Cells[(int)CellName.Red3] = 
            new StreetRentCell("Площадь Маяковского", 240, 20, 150, StreetType.Red),
            //Cells[(int)CellName.RedEvent] = 
            new RandomEventCell(),
            //Cells[(int)CellName.North] = 
            new RailwayRentCell("Северная Ж/Д"),

            //Cells[(int)CellName.Purple1] = 
            new StreetRentCell("Грузинский вал ул.", 260, 22, 150, StreetType.Purple),
            //Cells[(int)CellName.Purple2] = 
            new StreetRentCell("Чайковского ул.", 260, 22, 150, StreetType.Purple),
            //Cells[(int)CellName.Purple3] = 
            new StreetRentCell("Смоленская пл.", 280, 24, 150, StreetType.Purple),
            //Cells[(int)CellName.GoToPrison] = 
            new GoToPrisonCell(),

            //Cells[(int)CellName.Blue1] = 
            new StreetRentCell("Щусева ул.", 300, 26, 200, StreetType.Blue),
            //Cells[(int)CellName.Blue2] = 
            new StreetRentCell("Гоголевский бульвар", 300, 26, 200, StreetType.Blue),
            //Cells[(int)CellName.Blue3] =
            new StreetRentCell("Кутузовский пр-т", 320, 28, 200, StreetType.Blue),
            //Cells[(int)CellName.BlueEvent] = 
            new RandomEventCell(),
            //Cells[(int)CellName.East] = 
            new RailwayRentCell("Восточная Ж/Д"),

            //Cells[(int)CellName.Black1] = 
            new StreetRentCell("Малая бронная ул.", 350, 35, 200, StreetType.Black),
            //Cells[(int)CellName.Black2] = 
            new StreetRentCell("Арбат ул.", 400, 50, 200, StreetType.Black),
            //Cells[(int)CellName.BlackTax] = 
            new TaxSpendCell(),
        };

        public static void Print (Player player)
        {
            Console.Clear();
            int startPosition = 0;
            foreach (ICell cell in Cells)
            {
                Console.SetCursorPosition(70, startPosition);
                if (cell is StartEarnCell)
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                if (cell is PrisonCell || cell is GoToPrisonCell)
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    
                if (cell is StreetRentCell)
                    Console.ForegroundColor = ConsoleColor.Green;
                if(cell is RailwayRentCell)
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                if(cell is RandomEventCell)
                    Console.ForegroundColor = ConsoleColor.Cyan;

                Console.Write(cell.Name);
                if(player.CurrentCell == cell)
                    Console.Write(" <=======");
                Console.ResetColor();
                startPosition++;
                Console.WriteLine();
            }
            Console.ResetColor();
        }

        public static void CellsToXML(string fileName)
        {
            XElement xml = new XElement("Cells",
                new XElement("StartEarnCell"),
                GetStreetCells(StreetType.Yellow, "Yellow"),
                new XElement("PenaltySpendCell"),
                GetRailwayCells(1),

                GetStreetCells(StreetType.Green, "Green"),
                new XElement("RandomEventCell"),
                new XElement("PrisonCell"),
                GetStreetCells(StreetType.Coral, "Coral"),
                GetRailwayCells(2),

                GetStreetCells(StreetType.White, "White"),
                new XElement("RandomEventCell"),
                new XElement("ParkingCell"),
                GetStreetCells(StreetType.Red, "Red"),
                new XElement("RandomEventCell"),
                GetRailwayCells(3),

                GetStreetCells(StreetType.Purple, "Purple"),
                new XElement("GoToPrisonCell"),
                GetStreetCells(StreetType.Blue, "Blue"),
                new XElement("RandomEventCell"),
                GetRailwayCells(4),

                GetStreetCells(StreetType.Black, "Black"),
                new XElement("TaxSpendCell")
                );
            xml.Save(fileName);
        }
        public static IEnumerable<XElement> GetStreetCells(StreetType streetType, string type) => 
            Cells
                .Where(c => c.GetType() == typeof(StreetRentCell))
                .Cast<StreetRentCell>()
                .Where(s => s.StreetType == streetType)
                .Select(s => s.StreetXElement(type));
        public static IEnumerable<XElement> GetRailwayCells(int index) => 
            Cells
                .Where(c => c.GetType() == typeof(RailwayRentCell))
                .Cast<RailwayRentCell>()
                .Select(r => r.RailwayXElement())
                .Take(index);

        public static StreetRentCell StreetRentCellsFromXML(XElement node) =>
            new StreetRentCell(
                (string)node.Element("name"),
                (int)node.Element("cost"),
                (int)node.Element("rent"),
                (int)node.Element("houseCost"),
                (StreetType)(int)node.Element("streetType")
            );

        public static RailwayRentCell RailwayRentCellsFromXML(XElement node) =>
            new RailwayRentCell((string)node.Element("name"));

        public static IList< ICell > GetAllCells(string fileName)
        { 
            throw new NotImplementedException();
        }
    }
}
