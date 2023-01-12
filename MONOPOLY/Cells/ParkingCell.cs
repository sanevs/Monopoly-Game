using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MONOPOLY
{
    class ParkingCell : ICell
    {
        public string Name => "Бесплатная парковка";
        public void Play(Player player) =>
            Console.WriteLine($"{player.Name} находится на клетке {Name} !");
    }
}
