using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MONOPOLY
{
    class GoToPrisonCell : ICell
    {
        public string Name => "Марш в тюрьму!";
        public void Play(Player player)
        {
            Console.WriteLine($"{player.Name} находится на клетке {Name} !");
            player.CurrentCell = GameField.Cells[(int)CellName.Prison];
            player.InPrison = true;
        }
    }
}
