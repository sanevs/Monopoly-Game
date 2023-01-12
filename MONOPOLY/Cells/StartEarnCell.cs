using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MONOPOLY
{
    class StartEarnCell : ICell
    {
        public string Name => "Старт (получите 200 $)";

        public void Play(Player player)
        {
            Console.WriteLine($"{player.Name} находится на клетке {Name} !");
            player.Money += 200;
            Console.WriteLine($"Баланс игрока {player.Name} - {player.Money} $");
        }
    }
}
