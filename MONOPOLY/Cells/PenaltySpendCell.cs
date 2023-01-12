using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MONOPOLY
{
    class PenaltySpendCell : ICell
    {
        public string Name => "Штраф - 75 $";
        public void Play(Player player)
        {
            Console.WriteLine($"{player.Name} находится на клетке {Name} !");
            player.Money -= 75;
            Console.WriteLine($"Баланс игрока {player.Name} - {player.Money} $");
        }
    }
}
