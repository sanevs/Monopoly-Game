using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MONOPOLY
{
    class RandomEventCell : ICell
    {
        public string Name => "???|Случайное событие|???";

        public static RandomEventCell[] RandomEventCells { get; set; } =
            new RandomEventCell[]
            {
                new MoneyRandomEventCell(),
                new TransferRandomEventCell()
            };

        public virtual void Play(Player player)
        {
            Console.WriteLine($"{player.Name} находится на клетке {Name} !");
            if (Player.Random.Next(5) < 4)// 80% вероятность
                RandomEventCells[Player.Random.Next(RandomEventCells.Length)].Play(player);
            else
            {
                player.PrisonBreakCard = true;
                Console.WriteLine($"{player.Name}, получите карточку освобождения из тюрьмы.");
                Console.WriteLine("Она будет автоматически использована, окажись вы в тюрьме.");
            }
        }
    }
}
