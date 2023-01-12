using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MONOPOLY
{
    class Player 
    {
        public static Random Random = new Random();
        private static int Count { get; set; }
        public static IList<Player> Players { get; private set; }
        private const int HalfSecond = 500;
        private const byte MinPoint = 1;
        private const byte MaxPoint = 7;

        public string Name { get; }
        private int Order { get; }//порядок хода
        public int Money { get; set; }//денежки
        public IList<RentCell> OwnRentCells { get; set; }//жд и улицы во владении игрока
        public ICell CurrentCell { get; set; }//клетка, на которой стоит игрок
        public int? Points { get; set; } = 0;//очки кубиков
        public int DoubleCount { get; set; } = 0;//число дублей
        public bool InPrison { get; set; } = false;//в тюрьме?
        public int PrisonTime { get; set; } = 1;//срок отсидки
        public int MoneyRandomEventValue { get; set; } = 0;//сколько денег прибавит/отнимет случ. событие
        public bool PrisonBreakCard { get; set; } = false;//карточка освобождения из тюрьмы
        public int AuctionBid { get; set; } = 0;//ставка на аукционе

        public Player(string name, int order)
        {
            Name = name;
            Order = order;
            Money = 1500;
            OwnRentCells = new List<RentCell>();
            CurrentCell = GameField.Cells[(int)CellName.Start];//начинает со старта
        }

        //Создание игроков
        public static void CreatePlayers()
        {
            GameField gameField = new GameField ();
            SetPlayersCount();
            string[] names = CreateNames();
            int[] order = SetPlayersOrder(names);
            Players = new List<Player>(Count);
            for (int i = 0; i < Count; i++)
                Players.Add(new Player(names[i], order[i]));
            Players = Players
                .OrderByDescending(p => p.Order)
                .ToList();
            PrintOrder();
            Console.WriteLine();
            Console.WriteLine("Для начала игры нажмите ввод...");
            Console.ReadKey();
        }
        // Кол-во игроков
        private static void SetPlayersCount()
        {
            Console.WriteLine("Добро пожаловать в игру МОНОПОЛИЯ!!!");
            try
            {
                Console.WriteLine("Сколько монополистов будет играть?");
                Count = byte.Parse(Console.ReadLine());
                Console.Write("> ");
                if (Count < 2 || Count > 5)
                    throw new ArgumentOutOfRangeException(
                        "Число игроков может быть от 2 до 5.");
            }
            catch (Exception ex) when (
                ex is ArgumentOutOfRangeException ||
                ex is FormatException ||
                ex is OverflowException ||
                ex is ArgumentNullException)
            {
                Error(ex);
                SetPlayersCount();
            }
        }
        // Заполнение имен игроков
        private static string[] CreateNames()
        {
            string[] names = new string[Count];
            int i = 0;
            while (true)
            {
                try
                {
                    for (; i < Count; i++)
                    {
                        Console.Write("Введите имя {0} игрока: ", i + 1);
                        names[i] = Console.ReadLine().ToUpper();
                        if (names[i].Length == 0)
                            throw new ArgumentNullException("Имя игрока не может быть пустым.");
                    }
                    return names;
                }
                catch (Exception ex) when (
                    ex is ArgumentNullException)
                {
                    Error(ex);
                }
            }
        }
        // Определение очередности
        private static int[] SetPlayersOrder(string[] names)
        {
            int[] order = new int[names.Length];
            int dice1, dice2;
            Console.WriteLine("\nДля определения очередности ходов каждый игрок бросит 2 кубика.");
            for (int i = 0; i < names.Length; i++)
            {
                dice1 = Random.Next(MinPoint, MaxPoint);
                dice2 = Random.Next(MinPoint, MaxPoint);
                order[i] = dice1 + dice2;
                RollText(names[i], dice1, dice2);
                if (CheckOrder(order, i))
                {
                    i--;
                    Console.WriteLine("Выпало повторяющееся число очков!");
                }
            }
            return order;
        }
        private static bool CheckOrder(int[] order, int iCurrent)
        {
            for (int i = 0; i < iCurrent; i++)
                if (order[iCurrent] == order[i])
                    return true;
            return false;
        }

        // Вывод ошибок
        public static void Error(Exception ex) =>
            Console.WriteLine($"Error {Random.Next(1000):D4}: {ex.Message}\n");

        private static void PrintOrder ()
        {
            Console.WriteLine("Очередность игроков");
            foreach (Player player in Players)
                Console.Write($" -> {player.Name}");
        }
        /////////////////////////////////////////////////

        //ИГРАТЬ
        public static void PlayGame()
        {
            CreatePlayers();
            while (Players.Count() > 1)
            {
                for (int i = 0; i < Players.Count(); i++)
                {
                    GameField.Print(Players[i]);
                    if (Players[i].Play() || Players[i].IsBankrupt())
                    {
                        Players[i].LoseAsAction();
                        i--;
                        if (Players.Count() == 1)
                            break;
                    }
                    Console.WriteLine("Для передачи хода нажмите ввод...");
                    Console.ReadKey();
                }
            }
            Console.WriteLine($"{Players.Single().Name} побеждает!!!");
        }
        public bool Play()
        {
            Console.SetCursorPosition(0, 0);
            PrintOrder();
            if (InPrison)
            {
                CurrentCell.Play(this);
                return false;
            }
            while (true)
            {                
                Console.WriteLine();
                Console.WriteLine($"*************ХОД************* {Name} *************ХОД*************");
                Console.WriteLine($"Баланс: {Money} $\nТекущая клетка: {CurrentCell.Name}");
                Console.WriteLine($"*************ХОД************* {Name} *************ХОД*************");
                Console.WriteLine("Перед броском кубиков вы можете выполнять различные действия");
                Console.WriteLine("(в конце действий введите бросить кубики):");
                PrintActivities();
                string actionInput = Console.ReadLine().ToLower();
                Console.Write("> ");
                if (Activity.Lose.Pattern().IsMatch(actionInput))
                    return true;
                if (Activity.RollDice.Pattern().IsMatch(actionInput))
                    break;
                Do(actionInput);
            }
            Move();
            return false;
        }
        private bool IsBankrupt()
        {
            while (Money <= 0)
            {
                Console.WriteLine($"{Name}, у вас осталось {Money} $. Что будете делать?");
                Console.Write("> ");
                PrintActivities();
                string actionInput = Console.ReadLine().ToLower();
                if (Activity.Lose.Pattern().IsMatch(actionInput))
                    return true;
                Do(actionInput);
            }
            return false;
        }
        //действия игрока (перед броском кубиков)
        private static void PrintActivities()
        {
            Console.WriteLine("Сдаться, купить дом, продать дома, торговать,\nзаложить улицу или ж/д,");
            Console.WriteLine("выкуп заложенной улицы или ж/д,");
            Console.WriteLine("информация о своих улицах и ж/д,");
            Console.WriteLine("информация об улицах и ж/д другого игрока,");
            Console.WriteLine("справка обо всех улицах и ж/д.");
        }
        private void Do(string actionInput)
        {
            if (Activity.BuyHouse.Pattern().IsMatch(actionInput))
                BuyHouseAsAction();
            else if (Activity.SellHouses.Pattern().IsMatch(actionInput))
                SellHousesAsAction();
            else if (Activity.Trade.Pattern().IsMatch(actionInput))
                TradeAsAction();
            else if (Activity.SellRentCell.Pattern().IsMatch(actionInput))
                SellRentCellAsAction();
            else if (Activity.RepayRentCell.Pattern().IsMatch(actionInput))
                RepayRentCellAsAction();
            else if (Activity.F1.Pattern().IsMatch(actionInput))
                GetAllRentCellsInfoAsAction();
            else if (Activity.OwnRentCells.Pattern().IsMatch(actionInput))
                GetOwnRentCellsAsAction();
            else if (Activity.PlayerRentCells.Pattern().IsMatch(actionInput))
                GetPlayerRentCellsAsAction();
            else
                Console.WriteLine("Нет такого действия!");
        }
        private void BuyHouseAsAction ()
        {
            IList<StreetRentCell> streetRentCells = new List<StreetRentCell>();
            if (OwnRentCells
                .Cast<StreetRentCell>()
                .Where(c => c.CanBuildHouses && c.StreetType != StreetType.Railway)
                .ToList()
                .Count() == 0)
            {
                Console.WriteLine("Нет улиц, где вы можете строить дома");
                return;
            }
            Console.WriteLine("Список всех улиц, где вы можете построить дом/отель:");
            foreach (IGrouping<StreetType, StreetRentCell> monopolyStreetsGroupByType in
                OwnRentCells
                .Cast<StreetRentCell>()
                .Where(c => c.CanBuildHouses)
                .ToList()
                .GroupBy(c => c.StreetType))
            {
                streetRentCells = 
                    (List<StreetRentCell>)monopolyStreetsGroupByType.OrderByDescending(c => c.HouseCount);
                for (int i = 0; i < monopolyStreetsGroupByType.Count() - 1; i++)
                {
                    if (streetRentCells[i].HouseCount - streetRentCells[i + 1].HouseCount > 0)
                    {
                        streetRentCells = (List<StreetRentCell>)streetRentCells.Skip(i + 1);
                        break;
                    }
                }
                foreach (StreetRentCell street in streetRentCells)
                    Console.WriteLine($"{street.Name} {street.StreetType}, кол-во домов: {street.HouseCount}");
            }
            while(true)
            {
                try
                {
                    Console.WriteLine("Введите улицу, чтобы построить на ней дом");
                    string input = Console.ReadLine();
                    Console.Write("> ");
                    if (Activity.Back.Pattern().IsMatch(input))
                        break;
                    StreetRentCell street = streetRentCells
                        .Where(s => s.Name == input)
                        .Single();
                    Console.WriteLine($"Дом построен");
                    Money -= street.HouseCost;
                    Console.WriteLine($"Баланс игрока {Name} - {Money} $");
                    street.HouseCount++;
                    street.Rent += (int)Math.Pow(3, (int)street.HouseCount + 1);
                    break;
                }
                catch (Exception ex) when(
                    ex is ArgumentOutOfRangeException ||
                    ex is ArgumentNullException ||
                    ex is InvalidOperationException)
                {
                    Error(ex);
                }
            }
        }
        private void SellHousesAsAction ()
        {
            IEnumerable<StreetRentCell> streetRentCells = OwnRentCells
                .Where(c => c.StreetType != StreetType.Railway)
                .Cast<StreetRentCell>();
            streetRentCells = streetRentCells
                .Where(c => c.HouseCount > 0);
            if(streetRentCells.Count() == 0)
            {
                Console.WriteLine("У вас нет домов");
                return;
            }
            Console.WriteLine($"Список всех улиц игрока {Name}, где есть дома:");
            foreach (StreetRentCell streetRentCell in streetRentCells)
                Console.WriteLine($"{streetRentCell.Name}, кол-во домов: {streetRentCell.HouseCost}");
            while (true)
            {
                try
                {
                    Console.Write("Введите улицу, на которой хотите продать дома: ");
                    string input = Console.ReadLine();
                    StreetRentCell rentCell = streetRentCells
                        .Where(c => c.Name == input)
                        .First();
                    if (rentCell is null)
                        throw new ArgumentOutOfRangeException("Неправильно введено название улицы");
                    Console.Write("Введите кол-во домов для продажи: ");
                    int houses = int.Parse(Console.ReadLine());
                    if (houses > (int)rentCell.HouseCount)
                        throw new ArgumentOutOfRangeException("Введено слишком большое кол-во домов");
                    Money += houses * rentCell.HouseCost / 2;
                    Console.WriteLine($"Продано домов: {rentCell.HouseCount} шт.");
                    rentCell.HouseCount -= houses;
                    Console.WriteLine($"Баланс игрока {Name}: {Money} $");
                }
                catch (Exception ex) when(
                    ex is ArgumentOutOfRangeException ||
                    ex is ArgumentNullException ||
                    ex is FormatException ||
                    ex is OverflowException)
                {
                    Error(ex);
                }
            }
        }
        private void TradeAsAction ()
        {
            Console.WriteLine("Торговать с кем?");
            while (true)
            {
                try
                {
                    Console.Write("> ");
                    string input = Console.ReadLine().ToUpper();
                    Player tradeWith = Players
                        .Where(p => p.Name == input)
                        .First();
                    if (tradeWith == null)
                        throw new ArgumentNullException("Неправильно введено имя игрока");
                    int? offerToTradeWith = Trade(tradeWith);
                    int? offerReply = tradeWith.Trade(this);

                    RentCell rentCellOffer = null;
                    if (offerToTradeWith is null)
                    {
                        rentCellOffer = TradeRentCell();
                        if (rentCellOffer is null)
                        {
                            Console.WriteLine("Сделка сорвалась!");
                            break;
                        }
                    }
                    RentCell rentCellReplyOffer = null;
                    if(offerReply is null)
                        rentCellReplyOffer = tradeWith.TradeRentCell();
                    //оба согласны?
                    if(IsTradeComplete() && tradeWith.IsTradeComplete())
                    {
                        Console.WriteLine("Сделка состоялась!");
                        CompleteTrade(offerToTradeWith, rentCellOffer, tradeWith);
                        tradeWith.CompleteTrade(offerReply, rentCellReplyOffer, this);
                    }
                    break;
                }
                catch (Exception ex) when(
                    ex is InvalidOperationException || 
                    ex is ArgumentNullException) 
                {
                    Error(ex);
                }
            }
        }
        private void SellRentCellAsAction ()
        {
            RentCell rentCell = TradeRentCell();
            if (rentCell is null)
                return;
            Money += rentCell.Cost / 2;
            Console.WriteLine($"Баланс игрока {Name}: {Money} $");
            rentCell.SaleCost = (int)(rentCell.Cost * 0.6);
            rentCell.IsPawn = true;
        }
        private void RepayRentCellAsAction ()
        {
            while (true)
            {
                try
                {
                    IEnumerable<RentCell> rentCells = OwnRentCells
                        .Where(c => c.IsPawn);
                    if(rentCells.Count() == 0)
                    {
                        Console.WriteLine("У вас нет заложенных улиц");
                        return;
                    }
                    PrintRentCells(rentCells);
                    Console.WriteLine("Введите название улицы или ж/д для выкупа.");
                    Console.Write("> ");
                    string input = Console.ReadLine();
                    RentCell rentCell = rentCells
                        .Where(c => c.Name == input)
                        .First();
                    rentCell.IsPawn = false;
                    Money -= rentCell.SaleCost;
                    Console.WriteLine($"Баланс игрока {Name} - {Money} $");
                    rentCell.SaleCost = rentCell.Cost;
                    break;
                }
                catch (Exception ex) when (
                    ex is ArgumentNullException)
                {
                    Error(ex);
                }
            }
        }
        private void LoseAsAction()
        {
            Console.WriteLine($"{Name} выбывает из игры.");
            Players.Remove(this);
        }
        private void GetAllRentCellsInfoAsAction ()
        {
            IList<RentCell> rentCells = GameField.Cells
                .Where(c => c is RentCell)             
                .Cast<RentCell>()
                .ToList();
            foreach (RentCell rentCell in rentCells)
                Console.WriteLine($"{rentCell.Name} - {rentCell.SaleCost} $");
        }
        private void GetOwnRentCellsAsAction()
        {
            if(OwnRentCells.Count() == 0)
            {
                Console.WriteLine("У вас нет улиц");
                return;
            }
            foreach (RentCell rentCell in OwnRentCells)
            {
                Console.Write(rentCell.Name);
                if(rentCell.IsPawn)
                    Console.WriteLine(" - заложена");
                else
                    Console.WriteLine();
            }
        }
        private void GetPlayerRentCellsAsAction()
        {
            while (true)
            {
                try
                {
                    Console.Write("Введите имя игрока: ");
                    string playerName = Console.ReadLine().ToUpper();
                    if(Players
                        .Where(p => p.Name == playerName)
                        .Single().OwnRentCells.Count() == 0)
                    {
                        Console.WriteLine($"У игрока {playerName} нет улиц или ж/д");

                    }
                    foreach (var cell in Players
                        .Where(p => p.Name == playerName)
                        .Single().OwnRentCells)
                        Console.WriteLine(cell.Name);
                    break;
                }
                catch (Exception ex) when (
                    ex is ArgumentNullException ||
                    ex is InvalidOperationException)
                {
                    Console.WriteLine("Неверное имя игрока");
                    Error(ex);
                }
            }
        }
        //+
        private void CompleteTrade (int? offer, RentCell rentCellOffer, Player tradeWith)
        {
            if (offer is null)
            {
                Console.WriteLine($"{Name} ---> {rentCellOffer.Name} ---> {tradeWith.Name}");
                OwnRentCells.Remove(rentCellOffer);
                tradeWith.OwnRentCells.Add(rentCellOffer);
            }
            else
            {
                Money -= (int)offer;
                tradeWith.Money += (int)offer;
                Console.WriteLine($"{Name} (баланс - {Money} $) ---> {(int)offer} ---> {tradeWith.Name} (баланс - {tradeWith.Money} $)");
            }
        }
        private int? Trade(Player tradeWith)
        {
            while (true)
            {
                try
                {
                    Console.WriteLine($"Что игрок {Name} предлагает игроку {tradeWith.Name} ?");
                    Console.WriteLine("\t(деньги, улицу или ж/д)");
                    Console.Write("> ");
                    string offer = Console.ReadLine().ToLower();
                    if (Activity.Money.Pattern().IsMatch(offer))
                    {
                        int? offerSum = TradeMoney();
                        if (offerSum is null)
                            throw new ArgumentOutOfRangeException("Неправильно введена сумма");
                        return offerSum;
                    }
                    else if (Activity.RentCell.Pattern().IsMatch(offer))
                        return null;
                    else
                        throw new ArgumentOutOfRangeException("Неправильно введена команда");
                }
                catch (Exception ex) when (
                    ex is ArgumentOutOfRangeException)
                {
                    Error(ex);
                }
            }
        }
        private static int? TradeMoney()
        {
            Console.WriteLine("О какой сумме идет речь?");
            Console.Write("> ");
            if(int.TryParse(Console.ReadLine(), out int sumToTradeWith))
                return sumToTradeWith;
            return null;
        }
        private RentCell TradeRentCell()
        {
            IEnumerable<RentCell> rentCells = GetNoMonopolyRentCells();
            if(rentCells.Count() == 0)
            {
                Console.WriteLine("У вас нет улиц");
                return null;
            }
            PrintRentCells(rentCells);
            while (true)
            {
                try
                {
                    Console.WriteLine($"Напишите название улицы или ж/д, которую вы, {Name}, хотите продать.");
                    Console.Write("> ");
                    string input = Console.ReadLine();
                    RentCell rentCellOffer = rentCells
                        .Where(c => c.Name == input)
                        .First();
                    if (rentCellOffer is null)
                        throw new ArgumentOutOfRangeException("Неправильно введено название улицы или ж/д");
                    return rentCellOffer;
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    Error(ex);
                }
            }
        }
        private bool IsTradeComplete ()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine($"{Name} согласен?");
                    Console.Write("> ");
                    string input = Console.ReadLine().ToLower();
                    if (Activity.Yes.Pattern().IsMatch(input))
                        return true;
                    else if (Activity.No.Pattern().IsMatch(input))
                        return false;
                    else
                        throw new ArgumentOutOfRangeException("Неправильно введена команда");
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    Error(ex);
                }
            }
        }
        private IEnumerable<RentCell> GetNoMonopolyRentCells()
        {
            IEnumerable<StreetRentCell> streetRentCells = OwnRentCells
                .Where(c => c.StreetType != StreetType.Railway)
                .Cast<StreetRentCell>();
            IEnumerable<StreetRentCell> freeStreetRentCells = streetRentCells
                .Where(s => !s.CanBuildHouses);
            IEnumerable<RentCell> rentCells = OwnRentCells
                .Where(c => c.StreetType == StreetType.Railway)
                .Concat(freeStreetRentCells);
            return rentCells;
        }
        private void PrintRentCells(IEnumerable<RentCell> rentCells)
        {
            Console.WriteLine($"Список всех улиц и ж/д игрока {Name}:");
            foreach (RentCell rentCell in rentCells)
                Console.WriteLine($"{rentCell.Name}");
        }

        //передвижение
        private void Move()
        {
            Points = Roll();
            if (Points == null)
            {
                CurrentCell = GameField.Cells[(int)CellName.GoToPrison];
                CurrentCell.Play(this);
            }
            else if (DoubleCount != 0) 
            {
                NewCurrentCell();
                CurrentCell.Play(this);
                Move();
            }
            else
            {
                NewCurrentCell();
                CurrentCell.Play(this);
            }
        }
        public int? Roll()
        {
            int dice1 = Random.Next(MinPoint, MaxPoint);
            int dice2 = Random.Next(MinPoint, MaxPoint);
            RollText(Name, dice1, dice2);
            if (dice1 == dice2)
            {
                DoubleCount++;
                if (DoubleCount < 3)
                    return dice1 + dice2;
                DoubleCount = 0;
                return null;
            }
            DoubleCount = 0;
            return dice1 + dice2;
        }
        private static void RollText(string name, int dice1, int dice2)
        {
            Console.WriteLine("\n{0}, бросайте кубики (нажмите ввод)", name);
            Console.ReadKey();
            Console.Write("Бросок");
            Thread.Sleep(HalfSecond);
            Console.Write(".");
            Thread.Sleep(HalfSecond);
            Console.Write(".");
            Thread.Sleep(HalfSecond);
            Console.Write(". ");
            Thread.Sleep(HalfSecond);

            Console.Write($" {dice1}");
            Thread.Sleep(HalfSecond);
            Console.WriteLine($" {dice2}");
            Thread.Sleep(HalfSecond);
            Console.WriteLine("У вас {0} очков", dice1 + dice2);
        }
        public void NewCurrentCell()
        {
            for (int i = 0; i < GameField.Cells.Length; i++)
                if (GameField.Cells[i] == CurrentCell)
                {
                    i = (i + (int)Points) % GameField.Cells.Length;
                    CurrentCell = GameField.Cells[i];
                    break;
                }
        }
        
        //покупка свободной улицы или ж/д (вызов из класса RentCell)
        public void BuyFreeRentCell(RentCell rentCell)
        {
            PrintBillAndMakeTransaction(rentCell);
            OwnRentCells.Add(rentCell);
            CheckOwnRentCells(rentCell);
        }
        private void PrintBillAndMakeTransaction (RentCell rentCell)
        {
            Money -= rentCell.SaleCost;
            Console.WriteLine($"Покупка: {rentCell.Name}, баланс игрока {Name} составляет {Money} $");
            rentCell.SaleCost = rentCell.Cost;
        }
        private void CheckOwnRentCells(RentCell rentCell)//после покупки 
        {
            if (rentCell.StreetType == StreetType.Railway)
            {
                IEnumerable<RentCell> railways = OwnRentCells.Where(c => c.StreetType == StreetType.Railway);
                foreach (RentCell cell in railways)
                {
                    cell.Rent *= (int)Math.Pow(2, railways.Count() - 1);
                    if (cell.Rent > 25)
                        Console.WriteLine($"У клетки {cell.Name} увеличилась рента и теперь она составляет {cell.Rent} $");
                }
            }
            IEnumerable<StreetRentCell> blackStreets = OwnRentCells
                .Where(c => c.StreetType == StreetType.Black).Cast<StreetRentCell>();
            if (blackStreets.Count() == 2)
                RentUpX2ForStreets(blackStreets);

            IEnumerable<IGrouping<StreetType, StreetRentCell>> streetsGroupByType = OwnRentCells
                .Where(c => c.StreetType != StreetType.Railway)
                .Cast<StreetRentCell>().GroupBy(c => c.StreetType);
            foreach (IGrouping<StreetType, StreetRentCell> streetGroupByType in streetsGroupByType)
                if(streetGroupByType.Count() == 3)
                    RentUpX2ForStreets(streetGroupByType);
        }
        private static void RentUpX2ForStreets(IEnumerable<StreetRentCell> streets)
        {
            Console.WriteLine("Вы достигли монополии, выкупив все улицы одного цвета!");
            Console.WriteLine("Теперь в свой ход можно строить на них дома!");
            Console.WriteLine("Список улиц монополии:");
            foreach (StreetRentCell street in streets)
            {
                street.Rent *= 2;
                street.CanBuildHouses = true;
                Console.WriteLine($"{street.Name} - рента {street.Rent} $");
            }
        }
    }
}
