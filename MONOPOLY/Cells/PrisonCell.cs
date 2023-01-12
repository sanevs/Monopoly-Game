using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MONOPOLY
{
    class PrisonCell : ICell
    {
        public string Name => "Тюрьма";
        public void Play(Player player)
        {
            Console.WriteLine($"{player.Name} находится на клетке {Name} !");
            if (!player.InPrison)
                return;
            if (player.PrisonTime == 3)
            {
                Console.WriteLine("Вы отсидели 3 года... хм... хода, откинулись. Можно сделать ход.");
                PrisonBreak(player);
            }
            else if (player.PrisonBreakCard)
            {
                Console.WriteLine("Использована карточка освобождения из тюрьмы. Можно сделать ход.");
                PrisonBreak(player);
                player.PrisonBreakCard = false;
            }
            else
            {
                player.PrisonTime++;
                Console.WriteLine("Чтобы выйти из тюрьмы досрочно, попытайте удачу, выбросив дубль.");
                Console.WriteLine("После трех ходов \"отсидки\" вы можете выйти из тюрьмы");
                player.Points = player.Roll();
                if (player.DoubleCount != 0)
                {
                    Console.WriteLine("Выпал дубль! Вы свободны. Можно сделать ход.");
                    PrisonBreak(player);
                }
                else
                    Console.WriteLine("В следующий раз повезет!");
            }
        }

        private static void PrisonBreak(Player player)
        {
            player.NewCurrentCell();
            player.CurrentCell.Play(player);
            player.DoubleCount = 0;
            player.PrisonTime = 1;
            player.InPrison = false;
        }
    }
}
