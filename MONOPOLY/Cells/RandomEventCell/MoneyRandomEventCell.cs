using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MONOPOLY
{
    class MoneyRandomEventCell : RandomEventCell
    {
        public static int[] MoneyValues { get; } = { 50, 100, 150, 200, -50, -100, -150, -200 };
        public static string[] MoneyTexts
        {
            get => new string[] 
            {
            "Банк платит вам дивиденды в размере ",
            "Наступил срок выплаты вашей ссуды на строительство. Получите ",

            "Штраф за неоплаченную парковку ",
            "Время оплатить ипотеку в размере",
            "Оплатите услуги ЖКХ в размере "
            };
        }

        public override void Play(Player player)
        {
            player.MoneyRandomEventValue = MoneyValues[Player.Random.Next(MoneyValues.Length)];
            if (player.MoneyRandomEventValue > 0)
                Console.WriteLine($"{MoneyTexts[Player.Random.Next(2)]} {player.MoneyRandomEventValue}");
            else
                Console.WriteLine($"{MoneyTexts[Player.Random.Next(2, MoneyTexts.Length)]} {-player.MoneyRandomEventValue}");

            player.Money += player.MoneyRandomEventValue;
            Console.WriteLine($"Баланс игрока {player.Name} - {player.Money} $");
        }
    }
}
