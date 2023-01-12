using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MONOPOLY
{
    enum StreetType { Railway, Black, Blue, Coral, Green, Purple, Red, White, Yellow }
    class RentCell : ICell
    {
        public string Name { get; }
        public int Cost { get; }
        public int SaleCost { get; set; }
        public int Rent { get; set; }
        public StreetType StreetType { get; }
        public bool IsPawn { get; set; } = false;

        public RentCell(string name, int cost, int rent, StreetType streetType)
        {
            Name = name;
            Cost = SaleCost = cost;
            Rent = rent;
            StreetType = streetType;
        }

        public void Play(Player player)
        {
            Console.WriteLine($"{player.Name} находится на клетке {Name} !");
            try
            {
                Player cellOwner = null;
                foreach (Player owner in Player.Players)
                {
                    foreach (RentCell rentCell in owner.OwnRentCells)
                        if (rentCell == this)
                        {
                            cellOwner = owner;
                            break;
                        }
                    if (cellOwner != null)
                        break;
                }
                if (cellOwner is null)
                    throw new ArgumentNullException();
                if (cellOwner != player && !IsPawn)
                {
                    Console.WriteLine($"{player.Name} платит {Rent} $ владельцу. Владелец - {cellOwner.Name}");
                    player.Money -= Rent;
                    cellOwner.Money += Rent;
                }
                else
                    Console.WriteLine($"{player.Name} находится на своей улице / ж/д");
            }
            catch (Exception ex) when (
                ex is ArgumentNullException ||
                ex is InvalidOperationException)
            {
                Buy(player);/*покупка player-ом,иначе аукцион()*/
            }
        }
        private void Buy(Player player)
        {
            try
            {
                Console.WriteLine($"{Name} свободна. Стоимость: {Cost}.\nХотите её приобрести? (введите да/нет)");
                Console.Write("> ");
                string command = Console.ReadLine().ToLower();
                if (!Activity.Yes.Pattern().IsMatch(command) &&
                    !Activity.No.Pattern().IsMatch(command))
                    throw new ArgumentOutOfRangeException("Введена неизвестная команда. Повторите ввод.");
                if (Activity.Yes.Pattern().IsMatch(command))
                    player.BuyFreeRentCell(this);
                if (Activity.No.Pattern().IsMatch(command))
                    SellAtAuction(player);
            }
            catch (Exception ex) when (ex is ArgumentOutOfRangeException)
            {
                Player.Error(ex);
                Buy(player);
            }
        }
        private void SellAtAuction(Player player)
        {
            Console.WriteLine($"{player.Name} отказался покупать клетку {Name}.\nОна выставлена на аукцион!");
            Console.WriteLine($"Стартовая цена: {Cost}");
            while (true)
            {
                try
                {

                    foreach (Player auctionPlayer in Player.Players
                        .Where(p => p.Name != player.Name))
                    {
                        Console.Write($"{auctionPlayer.Name} > ");
                        string command = Console.ReadLine().ToLower();
                        if (Activity.Pass.Pattern().IsMatch(command))
                            auctionPlayer.AuctionBid = 0;
                        else
                        {
                            auctionPlayer.AuctionBid = int.Parse(command);
                            if (auctionPlayer.AuctionBid > SaleCost)
                                SaleCost = auctionPlayer.AuctionBid;
                        }
                    }
                    Player auctionWinner =
                        Player.Players.OrderByDescending(p => p.AuctionBid).First();
                    if(auctionWinner.AuctionBid == SaleCost)
                        auctionWinner.BuyFreeRentCell(this);
                    else
                        Console.WriteLine("Аукцион окончен, покупатель не выявлен.\nКлетка остается в банке.");
                    break;
                }
                catch (Exception ex) when (
                    ex is ArgumentNullException ||
                    ex is FormatException ||
                    ex is OverflowException)
                {
                    Player.Error(ex);
                }
            }
        }
    }
}
