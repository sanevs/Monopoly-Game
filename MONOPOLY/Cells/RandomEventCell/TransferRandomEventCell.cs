using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MONOPOLY
{
    class TransferRandomEventCell : RandomEventCell
    {
        public int[] TransferIndexes { get; } = {
            0, 1, 3, 4, 5, 6, 7, 9, 11, 12, 13, 14, 15, 16, 18, 20,
            21, 22, 24, 25, 26, 27, 28, 29, 30, 32, 33, 34, 36 };

        public override void Play(Player player)
        {
            for (int i = 0; i < GameField.Cells.Length; i++)
                if (GameField.Cells[i] == player.CurrentCell)
                {
                    int cellIndex = Player.Random.Next(TransferIndexes.Length);
                    if (i != cellIndex)
                    {
                        player.CurrentCell = GameField.Cells[TransferIndexes[cellIndex]];
                        break;
                    }
                    else
                        i = -1;
                }
            Console.WriteLine($"Вы перемещаетесь на {player.CurrentCell.Name}.");
            player.CurrentCell.Play(player);
        }
    }
}
