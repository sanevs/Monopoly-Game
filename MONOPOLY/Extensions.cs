using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MONOPOLY
{
    enum Activity { BuyHouse, SellHouses, Trade, SellRentCell, RepayRentCell, Lose,
                    RollDice, Yes, No, F1, OwnRentCells, PlayerRentCells,
                    Pass, Money, RentCell, Back }

    static class Extensions
    {
        public static IReadOnlyList<Regex> Activities { get; } = new List<Regex> {
            new Regex("купить дом|купить"),
            new Regex("продать дом|прод"),
            new Regex("торг"),
            new Regex("заложить"),
            new Regex("выкуп"),
            new Regex("сдаться"),

            new Regex("брос|кубик|куб"),
            new Regex("да|д"),
            new Regex("не|н"),
            new Regex("справка|все"),
            new Regex("свои|собств"),
            new Regex("игрок|друг"),

            new Regex("пас"),
            new Regex("деньги"),
            new Regex("улиц|ж/д|дорог|железн"),
            new Regex("назад|вернуть"),
        };

        public static Regex Pattern(this Activity action) =>
            Activities[(int)action];
    }
}
