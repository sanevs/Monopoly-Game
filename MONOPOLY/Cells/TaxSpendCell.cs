using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MONOPOLY
{
    class TaxSpendCell : ICell
    {
        public string Name => "Налог (10 % от баланса игрока)";
        public void Play(Player player)
        {
            Console.WriteLine($"{player.Name} находится на клетке {Name} !");
            player.Money -= player.Money / 10;
            Console.WriteLine($"Баланс игрока {player.Name} - {player.Money} $");
        }
    }
}
