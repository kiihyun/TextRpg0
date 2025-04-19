using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Linq;

namespace TextRpg
{
    [Serializable]
    public class Player
    {
        public int level = 1;
        public string name = "Chad";
        public Job job;
        public int attack = 10;
        public int defence = 5;
        public int hp = 100;
        public int gold = 1500;

        public Player(string name)
        {
            this.name = name;
        }
    }

    [Serializable]
    public class Item
    {

        public string name;
        public int attack;
        public int defence;
        public string description;
        public int gold;
        public bool isBuy;
        public bool isEquip;
        public ItemType type;

        public Item(string name, int attack, int defence, string description, int gold, bool isBuy, bool isEquip, ItemType type)
        {
            this.name = name;
            this.attack = attack;
            this.defence = defence;
            this.description = description;
            this.gold = gold;
            this.isBuy = isBuy;
            this.isEquip = isEquip;
            this.type = type;
        }
        public void PrintItem()
        {
            Console.WriteLine($"{name.PadRight(15)}|{(defence > 0 ? $"방어력{defence}" : $"공격력{attack}")}|{description.PadRight(20)}|{(isBuy == true ? "구매완료" : $"{gold} G")}");
        }
    }

    public enum Job
    {
        전사 = 1,
        법사,
        궁수
    }

    public enum ItemType
    {
        Weapon,
        Armor,
    }
    enum GameState
    {
        GameStart,
        Info,
        Inventory,
        Shop
    }
    public class Inventory
    {
        public List<Item> inventoryItemList = new List<Item>() { };
        public void PrintSellItems()
        {
            int index = 1;
            foreach (Item item in inventoryItemList)
            {
                Console.WriteLine($"{index} {item.name.PadRight(5)}|{(item.attack > 0 ? $"공격력 +{item.attack} " : $"방어력 + {item.defence} ")}|{item.description}");
                index++;
            }
        }
        public void PrintInventoryItem()
        {
            foreach (Item item in inventoryItemList)
            {
                string isEquiped;
                if (item.isEquip == true)
                    isEquiped = "[E]";
                else
                    isEquiped = "";
                Console.WriteLine($"-{isEquiped}{item.name.PadRight(5)}|{(item.defence > 0 ? $"방어력 +{item.defence}" : $"공격력 +{item.attack}")}|{item.description}");
            }
        }
        public void PrintEquipItem()
        {
            foreach (Item item in inventoryItemList)
            {
                string isEquiped;
                if (item.isEquip == true)
                    isEquiped = "[E]";
                else
                    isEquiped = "";
                int index = inventoryItemList.IndexOf(item) + 1;
                Console.WriteLine($"-{index} {isEquiped} {item.name.PadRight(10)}|{(item.defence > 0 ? $"방어력 +{item.defence}" : $"공격력 +{item.attack}")}|{item.description}");
            }
        }
        public void EquipItem(int index)
        {
            if (inventoryItemList[index].isEquip == false)
            {
                foreach (Item item in inventoryItemList)
                {
                    if (item.type == inventoryItemList[index].type)
                    {
                        item.isEquip = false;
                    }
                }
                inventoryItemList[index].isEquip = true;
            }
            else
                inventoryItemList[index].isEquip = false;
        }

        public bool SellItem(int index, Player _Player, Inventory inventory)
        {
            Item seletedItem = inventoryItemList[index];

            seletedItem.isBuy = false;
            _Player.gold += (int)(seletedItem.gold * 0.85f);
            inventoryItemList.Remove(seletedItem);
            return true;
        }
    }
    class Shop
    {

        public void PrintShopItems(Item[] items)
        {
            int index = 1;
            foreach (Item item in items)
            {
                Console.Write($"{index}.");
                item.PrintItem();
                index++;
            }
        }

        public bool BuyItem(int index, Item[] items, Player _player, Inventory inventory)
        {
            Item seletedItem = items[index];

            if (seletedItem.isBuy == true)
            {
                Console.WriteLine("이미 구매한 아이템입니다");
                return false;
            }
            if (seletedItem.gold > _player.gold)
            {
                Console.WriteLine("골드가 부족합니다");
                return false;
            }

            seletedItem.isBuy = true;
            _player.gold -= seletedItem.gold;
            inventory.inventoryItemList.Add(seletedItem);
            return true;
        }


    }

    class RestSystem
    {
        public void ProcessRest(Player _player)
        {
            if (_player.gold >= 500)
            {
                Console.WriteLine("휴식을 완료했습니다");
                _player.hp = 100;
                _player.gold -= 500;
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Gold가 부족합니다");
                Console.ReadKey();
            }
        }
    }

    public class SaveData
    {
        public string playerName;
        public int hp;
        public int attack;
        public int defence;
        public int gold;
        public List<Item> inventoryItems;
    }
    public static class SaveSystem
    {
        private static string savePath = "D:\\UnityProject\\git\\TextRpg0\\textRpg01\\textRpg01\\saveData.json";

        public static void SaveGame(Player player, Inventory inventory)
        {
            SaveData data = new SaveData()
            {
                playerName = player.name,
                hp = player.hp,
                attack = player.attack,
                defence = player.defence,
                gold = player.gold,
                inventoryItems = inventory.inventoryItemList
            };

            string json = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(savePath, json);
            Console.WriteLine("게임이 저장되었습니다.");
        }

        public static bool LoadGame(Player player, Inventory inventory)
        {
            if (!File.Exists(savePath))
            {
                Console.WriteLine("저장된 파일이 없습니다.");
                return false;
            }

            string json = File.ReadAllText(savePath);
            SaveData data = JsonConvert.DeserializeObject<SaveData>(json);

            player.name = data.playerName;
            player.hp = data.hp;
            player.attack = data.attack;
            player.defence = data.defence;
            player.gold = data.gold;

            inventory.inventoryItemList = data.inventoryItems;

            Console.WriteLine("게임을 불러왔습니다.");
            return true;
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            var gameLogic = new GameLogic();
            gameLogic.StartGame();

            //equipItem.AddEquipItem(items[1]);
            //equipItem.AddEquipItem(items[5]);

        }

        class GameLogic
        {

            public Player player;
            private bool _isGameOver = false;
            public int behavior = 0;
            Item[] itemArr = new Item[]
                {
            new Item("수련자갑옷",0,5,"수련에 도움을 주는 갑옷입니다",1000,false,false,ItemType.Armor),
            new Item("무쇠갑옷",0,5,"무쇠로 만들어져 튼튼한 갑옷입니다.",100,true,true,ItemType.Armor),
            new Item("스파르타의 갑옷",0,5,"스파르타의 전사들이 사용했다는 전설의 갑옷입니다.",3500,false,false,ItemType.Armor),
            new Item("낡은 검 ",2,0,"쉽게 볼수 있는 낡은 검입니다",600,true,false,ItemType.Weapon),
            new Item("청동 도끼",5,0,"어디선가 사용됐던거 같은 도끼입니다.",1500,false,false,ItemType.Weapon),
            new Item("스파르타의 창",7,0,"스파르타의 전사들이 사용했다는 전설의 창입니다.",2000,true,true,ItemType.Weapon),
            new Item("엑스칼리버",7,0,"매우 크고 강력한 검",10000,false,false,ItemType.Weapon),
                };
            //static Player player = new Player("");
            Inventory inventory = new Inventory();
            Shop shop = new Shop();
            RestSystem restSystem = new RestSystem();
            //static EquipItem equipItem = new EquipItem();


            public void StartGame()
            {

                inventory.inventoryItemList.Add(itemArr[1]);
                inventory.inventoryItemList.Add(itemArr[3]);
                inventory.inventoryItemList.Add(itemArr[5]);

                Init();
                ShowStart();

                while (!_isGameOver)
                {
                    InputHandler();
                }

                Console.WriteLine(" 게임이 종료되었습니다");
            }

            private void InputHandler()
            {
                var input = Console.ReadKey();
                if (input.Key == ConsoleKey.Escape)
                {
                    _isGameOver = true;
                }
            }

            private void Init()
            {
                Console.Clear();
                Console.WriteLine("스파르타던전에 오신것을 환영합니다 \n이름을 입력하세요");
                string? playerName = Console.ReadLine();

                if (string.IsNullOrEmpty(playerName))
                {
                    Console.WriteLine("잘못된 이름입니다.");
                }
                else
                {
                    player = new Player(playerName);
                    Console.WriteLine($"{player.name}님, 입장하셨습니다");
                }

                //직업선택
                Console.WriteLine("직업을 선택하세요 1.전사 2.법사 3.궁수");
                int job;
                if (!int.TryParse(Console.ReadLine(), out job))
                {
                    Console.WriteLine("숫자를 입력해주세요.");
                    Console.ReadKey();
                }
                if (job >= 1 && job <= 3)
                {
                    player.job = (Job)job;
                    switch (player.job)
                    {
                        case Job.전사:
                            Console.WriteLine("전사를 선택했습니다");
                            break;
                        case Job.법사:
                            Console.WriteLine("법사를 선택했습니다");
                            break;
                        case Job.궁수:
                            Console.WriteLine("궁수를 선택했습니다");
                            break;

                    }
                }

            }


            private void ShowStart()
            {
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
                    Console.Write("이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.\r\n\r\n1. 상태 보기\r\n2. 인벤토리\r\n3. 상점\r\n");
                    Console.WriteLine("4. 던전입장");
                    Console.WriteLine("5. 휴식하기");
                    Console.WriteLine("6. 저장하기");
                    Console.WriteLine("7. 불러오기");
                    Console.WriteLine("\r\n원하시는 행동을 입력해주세요\r\n>>");
                    if (!int.TryParse(Console.ReadLine(), out behavior))
                    {
                        Console.WriteLine("숫자를 입력해주세요.");
                        Console.ReadKey();
                        continue;
                    }

                    if (behavior == 1)
                    {
                        ShowInfo();
                    }
                    else if (behavior == 2)
                    {
                        ShowInventory();
                    }
                    else if (behavior == 3)
                    {
                        ShowShop();
                    }
                    else if (behavior == 4)
                    {
                        ShowDungeon();
                    }
                    else if (behavior == 5)
                    {
                        ShowRest();
                    }
                    else if (behavior == 6)
                    {
                        SaveSystem.SaveGame(player, inventory);
                    }
                    else if (behavior == 7)
                    {
                        SaveSystem.LoadGame(player, inventory);
                    }
                    else
                    {
                        Console.WriteLine("잘못된 입력입니다");
                        Console.Read();
                    }
                }
            }

            private void ShowDungeon()
            {
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("던전입장\r\n이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.\r\n\r\n1. 쉬운 던전     | 방어력 5 이상 권장\r\n2. 일반 던전     | 방어력 11 이상 권장\r\n3. 어려운 던전    | 방어력 17 이상 권장\r\n0. 나가기\r\n\r\n원하시는 행동을 입력해주세요.\r\n>>");
                    if (!int.TryParse(Console.ReadLine(), out behavior))
                    {
                        Console.WriteLine("숫자를 입력해주세요.");
                        Console.ReadKey();
                    }
                    switch (behavior)
                    {
                        case 0:
                            ShowStart();
                            break;
                        case 1:
                            showDungeonClear(5, 1000);
                            break;
                        case 2:
                            showDungeonClear(11, 1700);
                            break;
                        case 3:
                            showDungeonClear(17, 2500);
                            break;
                    }
                }
            }


            private void showDungeonClear(int dungeonDefence, int dungeonGold)
            {
                Random random = new Random();
                Console.Clear();


                int weaponAttack = 0;
                int armorDefence = 0;
                foreach (Item item in inventory.inventoryItemList)
                {
                    if (item.isEquip == true)
                    {
                        armorDefence += item.defence;
                        weaponAttack += item.attack;
                    }
                }
                int totalAttack = player.attack + weaponAttack;
                int totalDefence = player.defence + armorDefence;
                bool isSuccess = totalDefence > dungeonDefence || random.NextDouble() > 0.4;

                if (isSuccess)
                {
                    int plusGold = (int)(dungeonGold * random.Next(totalAttack, totalAttack * 2) * 0.01f);
                    int reduceHp = random.Next(20 + (totalDefence - dungeonDefence), 36 - (totalDefence - dungeonDefence));
                    Console.WriteLine("던전 클리어");
                    Console.WriteLine("축하합니다!!\r\n 던전을 클리어 하였습니다.\r\n");
                    Console.WriteLine($"[탐험 결과]\r\n체력 {player.hp} -> {player.hp - reduceHp}");
                    Console.WriteLine($"Gold {player.gold} G -> {player.gold + plusGold + dungeonGold} G ");
                    Console.WriteLine("0. 나가기\r\n\r\n원하시는 행동을 입력해주세요.\r\n>>");
                    player.gold += dungeonGold;
                    player.gold += plusGold;
                    player.hp -= reduceHp;
                }
                else
                {
                    int reducedHp = (int)(player.hp * 0.5f);

                    Console.WriteLine("던전 클리어 실패\r\n");
                    Console.WriteLine($"[탐험 결과]\r\n체력 {player.hp} -> {reducedHp}");
                    Console.WriteLine("0. 나가기\r\n\r\n원하시는 행동을 입력해주세요.\r\n>>");
                    player.hp = reducedHp;
                }
                if (!int.TryParse(Console.ReadLine(), out behavior))
                {
                    Console.WriteLine("숫자를 입력해주세요.");
                    Console.ReadKey();
                }
                if (behavior == 0)
                {
                    ShowStart();
                }
            }

            private void ShowInfo()
            {
                while (true)
                {

                    int weaponAttack = 0;
                    int armorDefence = 0;
                    //foreach (Item item in inventory.inventoryItemList)
                    //{
                    //    if (item.isEquip == true)
                    //    {
                    //        armorDefence += item.defence;
                    //        weaponAttack += item.attack;
                    //    }
                    //}
                    var isEquippedItem = from item in inventory.inventoryItemList
                                         where item.isEquip == true
                                         select item;
                    foreach (var item in isEquippedItem)
                    {
                        armorDefence += item.defence;
                        weaponAttack += item.attack;
                    }
                    int totalAttack = player.attack + weaponAttack;
                    int totalDefence = player.defence + armorDefence;
                    Console.Clear();
                    Console.WriteLine("상태 보기\r\n캐릭터의 정보가 표시됩니다.\r\n\r\n");
                    Console.WriteLine($"Lv. {player.level}");
                    Console.WriteLine($"{player.name} ({player.job})");
                    Console.WriteLine($"공격력 : {totalAttack} (+{weaponAttack})");
                    Console.WriteLine($"방어력 : {totalDefence} (+{armorDefence})");
                    Console.WriteLine($"체력 : {player.hp}");
                    Console.WriteLine($"Gold : {player.gold} G");
                    Console.WriteLine("0.나가기");
                    Console.Write("원하시는 행동을 입력해주세요\r\n>>");


                    if (!int.TryParse(Console.ReadLine(), out behavior))
                    {
                        Console.WriteLine("숫자를 입력해주세요.");
                        Console.ReadKey();
                        continue;
                    }
                    switch (behavior)
                    {
                        case 0:
                            ShowStart();
                            break;
                        default:
                            Console.WriteLine("잘못된 입력입니다.\n");
                            break;
                    }
                }
            }
            private void ShowInventory()
            {
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("인벤토리\r\n보유 중인 아이템을 관리할 수 있습니다.\r\n\r\n[아이템 목록]");
                    inventory.PrintInventoryItem();
                    Console.WriteLine("\r\n\r\n1.장착 관리\r\n0.나가기\r\n\r\n원하시는 행동을 입력해주세요.\r\n >>");
                    Console.WriteLine();
                    if (!int.TryParse(Console.ReadLine(), out behavior))
                    {
                        Console.WriteLine("숫자를 입력해주세요.");
                        Console.ReadKey();
                        continue;
                    }

                    switch (behavior)
                    {
                        case 0:
                            ShowStart();
                            break;
                        case 1:
                            ShowEquipItem();
                            break;
                    }
                }
            }
            private void ShowBuyItem(Item[] items)
            {
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine($"상점 - 아이템 구매\r\n필요한 아이템을 얻을 수 있는 상점입니다.\r\n\r\n[보유 골드]\r\n{player.gold} G\r\n\r\n[아이템 목록]");
                    Console.WriteLine();
                    shop.PrintShopItems(items);

                    Console.WriteLine("0. 나가기\r\n\r\n원하시는 행동을 입력해주세요.");
                    if (!int.TryParse(Console.ReadLine(), out behavior))
                    {
                        Console.WriteLine("숫자를 입력해주세요.");
                        Console.ReadKey();
                        continue;
                    }
                    if (behavior > items.Length)
                    {
                        Console.WriteLine("잘못 입력하셨습니다");
                    }
                    else
                    {
                        switch (behavior)
                        {
                            case 0:
                                ShowStart();
                                break;
                            default:
                                bool success = shop.BuyItem(behavior - 1, items, player, inventory);
                                if (success)
                                {
                                    Console.WriteLine("구매가 완료되었습니다.");
                                }
                                Console.ReadKey();
                                break;
                        }
                    }
                }
            }
            private void ShowEquipItem()
            {
                while (true)
                {

                    Console.Clear();
                    Console.WriteLine("인벤토리 - 장착 관리\r\n보유 중인 아이템을 관리할 수 있습니다.\r\n\r\n[아이템 목록]\r\n");
                    inventory.PrintEquipItem();
                    Console.WriteLine("\r\n\r\n0. 나가기\r\n\r\n원하시는 행동을 입력해주세요.\r\n>>");
                    if (!int.TryParse(Console.ReadLine(), out behavior))
                    {
                        Console.WriteLine("숫자를 입력해주세요.");
                        Console.ReadKey();
                        continue;
                    }
                    if (behavior > inventory.inventoryItemList.Count)
                    {
                        Console.WriteLine("잘못 입력하셨습니다");
                        Console.ReadKey();
                    }
                    else
                    {
                        switch (behavior)
                        {
                            case 0:
                                ShowStart();
                                break;
                            default:
                                inventory.EquipItem(behavior - 1);
                                break;
                        }
                    }
                }
            }

            public void ShowShop()
            {
                while (true)
                {

                    Console.Clear();
                    Console.WriteLine($"상점\r\n필요한 아이템을 얻을 수 있는 상점입니다.\r\n\r\n");
                    Console.WriteLine($"[보유 골드]\r\n{player.gold} G\r\n\r\n[아이템 목록]");
                    shop.PrintShopItems(itemArr);
                    Console.WriteLine("\n1. 아이템 구매\r\n2. 아이템 판매\r\n0. 나가기\r\n");
                    Console.WriteLine("\r\n원하시는 행동을 입력해주세요.\r\n>>");
                    if (!int.TryParse(Console.ReadLine(), out behavior))
                    {
                        Console.WriteLine("숫자를 입력해주세요.");
                        Console.ReadKey();
                        continue;
                    }
                    switch (behavior)
                    {
                        case 0:
                            ShowStart();
                            break;
                        case 1:
                            ShowBuyItem(itemArr);
                            break;
                        case 2:
                            ShowSellItem(itemArr);
                            break;
                    }
                }
            }

            private void ShowSellItem(Item[] items)
            {
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine($"상점 - 아이템 판매\r\n필요한 아이템을 얻을 수 있는 상점입니다.\r\n\r\n[보유 골드]\r\n{player.gold} G\r\n\r\n[아이템 목록]\r\n");
                    inventory.PrintSellItems();
                    Console.WriteLine("\r\n\r\n0. 나가기\r\n\r\n원하시는 행동을 입력해주세요.\r\n>>");
                    if (!int.TryParse(Console.ReadLine(), out behavior))
                    {
                        Console.WriteLine("숫자를 입력해주세요");
                        Console.ReadKey();
                        continue;
                    }
                    if (behavior == 0)
                    {
                        ShowStart();
                    }
                    else if (behavior > inventory.inventoryItemList.Count)
                    {
                        Console.WriteLine("잘못 입력했습니다");
                        Console.ReadKey();
                    }
                    else
                    {
                        bool success = inventory.SellItem(behavior - 1, player, inventory);
                        if (success)
                        {
                            Console.WriteLine("판매가 완료되었습니다.");
                            Console.ReadKey();
                        }
                        Console.ReadKey();
                    }
                }
            }

            public void ShowRest()
            {
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine($"휴식하기\r\n500 G 를 내면 체력을 회복할 수 있습니다. (보유 골드 : {player.gold} G)\r\n\r\n1. 휴식하기\r\n0. 나가기\r\n\r\n원하시는 행동을 입력해주세요.\r\n>>");

                    if (!int.TryParse(Console.ReadLine(), out behavior))
                    {
                        Console.WriteLine("숫자를 입력해주세요.");
                        Console.ReadKey();
                        continue;
                    }
                    if (behavior == 1)
                    {
                        restSystem.ProcessRest(player);
                    }
                    else if (behavior == 0)
                    {
                        ShowStart();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("잘못 입력했습니다");
                        Console.ReadKey();
                    }
                }
            }
        }
    }
}
