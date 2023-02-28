using System.Diagnostics;
using System.Numerics;
using System.Text;
using static GADE6112.Model;
using static GADE6112.Model.Character;
using static GADE6112.Model.Tile;
using System.Runtime.Serialization.Formatters.Binary;


namespace GADE6112
{

    class Model
    {
        Controller _controller = new Controller();
        readonly View view = new View();
        public abstract class Tile
        {

            protected int x;
            protected int y;
            public int Y { get; set; }
            public int X { get; set; }
            protected char Symbol { get; set; }
            public enum TileType { Hero, SwampCreature, Gold, Weapon, Obstacle, Empty, Mage, DroppedWeapon };
            public TileType Type { get; set; }

            public bool IsValidMove(Movement direction)
            {
                int nextX = X;
                int nextY = Y;
                switch (direction)
                {
                    case Movement.Up:
                        nextY--;
                        break;
                    case Movement.Down:
                        nextY++;
                        break;
                    case Movement.Left:
                        nextX--;
                        break;
                    case Movement.Right:
                        nextX++;
                        break;
                    default:
                        break;
                }
                //GameEngine gm = new GameEngine();
                Controller _controller = new Controller();
                Map map = _controller.Map;
                if (nextX < 0 || nextX >= map.Width || nextY < 0 || nextY >= map.Height)
                {
                    return false;
                }

                Tile nextTile = map.Tiles[nextX, nextY];
                if (nextTile is Obstacle)
                {
                    return false;
                }


                return true;
            }

            public Tile(int x, int y, char type)
            {
                X = x;
                Y = y;
                Symbol = type;
            }
            public bool IsPassable(TileType tile)
            {
                if (tile == TileType.Obstacle)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public class Obstacle : Tile
        {
            public Obstacle(int x, int y, char type) : base(x, y, type) { }
        }

        public class EmptyTile : Tile
        {
            public EmptyTile(int x, int y, char type) : base(x, y, type) { }
        }

        public abstract class Character : Tile
        {
            protected int _hp;
            protected int _maxHp;
            protected int _damage;
            protected Tile[] _vision;
            protected int _range;
            protected int _gold;
            protected Weapon _weapon;

            public int HP { get { return _hp; } set { _hp = value; } }
            public int MaxHP { get { return _maxHp; } set { _maxHp = value; } }
            public int Damage { get { return _damage; } set { _damage = value; } }
            public Tile[] Vision { get { return _vision; } }
            public int Range { get { return _range; } set { _range = value; } }
            public int Gold { get { return _gold; } set { _gold = value; } }
            public Weapon CurrentWeapon { get { return _weapon; } set { _weapon = value; } }

            public enum Movement
            {
                NoMovement,
                Up,
                Down,
                Left,
                Right
            }

            public Character(int x, int y, char symbol) : base(x, y, symbol)
            {
                
                x = X;
                y = Y;
                symbol = Symbol;
                _vision = Vision;
                
            }

            public virtual void Attack(Character target)
            {
                if (_weapon != null)
                {
                    target.HP -= CurrentWeapon.Damage;
                    CurrentWeapon.Durability--;
                    if (CurrentWeapon.Durability == 0)
                    {
                        _weapon = null;
                        CurrentWeapon = target._weapon;
                        
                    }
                }
                else
                {
                    target.HP -= 1;
                    if (target.IsDead())
                    {
                        //
                    }
                }
            }
            public virtual void Loot(Character victim)
            {
                Gold += victim.Gold;
                victim.Gold = 0;
                if (_weapon != null && victim._weapon != null && !(this is Mage))
                {
                    CurrentWeapon = victim.CurrentWeapon;
                    victim.CurrentWeapon = null;
                    if(this is Hero)
                    {
                        
                    }
                    Console.WriteLine($"{GetType().Name} looted {CurrentWeapon.Type} from {victim.GetType().Name}!");
                }
            }

            public bool IsDead()
            {
                return _hp <= 0;
            }

            public virtual bool CheckRange(Tile target)
            {
                int distance = DistanceTo(target);
                return distance <= _weapon.Range;
            }

            private int DistanceTo(Tile target)
            {
                int dx = Math.Abs(X - target.X);
                int dy = Math.Abs(Y - target.Y);
                return dx + dy;
            }

            public void Move(Movement move)
            {
                switch (move)
                {
                    case Movement.Up:
                        if (IsPassable(_vision[0].Type))
                            Y--;
                        break;
                    case Movement.Down:
                        if (IsPassable(_vision[1].Type))
                            Y++;
                        break;
                    case Movement.Left:
                        if (IsPassable(_vision[2].Type))
                            X--;
                        break;
                    case Movement.Right:
                        if (IsPassable(_vision[3].Type))
                            X++;
                        break;
                }
            }

            public abstract Movement ReturnMove(Movement move = 0);

            public virtual void Pickup(Item item)
            {
                if (item is Gold)
                {
                    _gold += ((Gold)item).Amount;
                }
                else if (item is Weapon)
                {
                    Equip((Weapon)item);
                }
            }

            private void Equip(Weapon w)
            {
                _weapon = w;
            }

            public override string ToString()
            {
                string weaponString;
                if (_weapon.Type.ToString() == "Bare Hands")
                {
                    weaponString = "Bare Hands";
                }
                else
                {
                    weaponString = $"{_weapon.Type.ToString()} ({_weapon.Damage} DMG, {_weapon.Range} Range, {_weapon.Durability} Durability)";
                }
                return $"HP: {_hp}/{_maxHp}\nCurrent Weapon: {weaponString}\nGold: {_gold}";
            }
        }
        public abstract class Enemy : Character
        {
            protected Random random;
            protected Weapon _weapon;
            public Weapon Weapon => _weapon;
            public Enemy(int x, int y, int damage, int hp, char symbol) : base(x, y, symbol)
            {
                Damage = damage;
                HP = hp;
                MaxHP = hp;
                random = new Random();
                if (GetType() == typeof(SwampCreature))
                {
                   _weapon = new MeleeWeapon(MeleeWeapon.Types.Dagger);
                }
                else if (GetType() == typeof(Leader))
                {
                    _weapon = new MeleeWeapon(MeleeWeapon.Types.Longsword);
                }
            }
            private void Equip(Weapon w)
            {
                _weapon = w;
            }
            public bool TakeDamage(int damage)
            {
                HP -= damage;
                return HP < 0;
            }
            public override string ToString()
            {
                if (Weapon == null)
                {
                    return $"Barehanded: {GetType().Name} ({HP}/{MaxHP}HP) at [{X}, {Y}] ({Damage} DMG)";
                }
                else
                {
                    return $"Equipped: {GetType().Name} ({HP}/{MaxHP}HP) at [{X}, {Y}] with {Weapon.Type.ToString()} " +
                           $"({Weapon.Durability}x{Weapon.Damage} DMG)";
                }
            }
            public void PickUp(Item item)
            {
                if (item.GetType() == typeof(Gold))
                {
                    Gold += ((Gold)item).Amount;
                }
                else if (item.GetType().IsSubclassOf(typeof(Weapon)))
                {
                    Equip((Weapon)item);
                }
            }
        }
        public class SwampCreature : Enemy
        {
            public SwampCreature(int x, int y) : base(x, y, damage: 1, hp: 10, symbol: 'S')
            {
                x = X;
                y = Y;
                Weapon weapon = new MeleeWeapon(MeleeWeapon.Types.Dagger);
                _weapon = weapon;
                this.Gold = 1;

            }

            public override Movement ReturnMove(Movement move = 0)
            {
                int direction = random.Next(1, 5);
                Movement chosenMove = (Movement)direction;


                while (!IsValidMove(chosenMove))
                {
                    direction = random.Next(1, 5);
                    chosenMove = (Movement)direction;
                }

                return chosenMove;
            }
            protected Tile GetTileInDirection(Movement move)
            {
                Tile tile = Vision[0];
                int x = X;
                int y = Y;
                //var blank = EmptyTile;
                switch (move)
                {
                    case Movement.Up:
                        y--;
                        tile = Vision[0];
                        break;
                    case Movement.Down:
                        y++;
                        tile = Vision[1];
                        break;
                    case Movement.Left:
                        x--;
                        tile = Vision[2];
                        break;
                    case Movement.Right:
                        x++;
                        tile = Vision[3];
                        break;
                }

                return tile;
                ////{
                ////    // the tile is passable
                ////}
                ////else
                ////{
                ////    // Return the tile at the given coordinates
                ////    return Vision[x, y];
                ////}
            }
            private bool IsValidMove(Movement move)
            {
                Tile tile = GetTileInDirection(move);
                return tile != null && (tile is EmptyTile || tile is Obstacle);
            }
        }
        public class Hero : Character
        {
            private const int HERO_DAMAGE = 2;
            private int goldCount;
            private bool _playerMoved;
            public int GoldCount
            {
                get { return goldCount; }
                set { goldCount = value; }
            }
            public bool PlayerMoved
            {
                get { return _playerMoved; }
                set { _playerMoved = value; }
            }
            public Hero(int x, int y, char symbol, int hp) : base(x, y, 'H')
            {
                Damage = HERO_DAMAGE;
                this.Gold = 0;
            }

            public override Movement ReturnMove(Movement movement)
            {
                int newX = X;
                int newY = Y;
                Tile tile = Vision[0];
                switch (movement)
                {
                    case Movement.Up:
                        newY--;
                        tile = Vision[0];
                        break;
                    case Movement.Down:
                        newY++;
                        tile = Vision[1];
                        break;
                    case Movement.Left:
                        newX--;
                        tile = Vision[2];
                        break;
                    case Movement.Right:
                        newX++;
                        tile = Vision[3];
                        break;
                    default:
                        return Movement.NoMovement;
                }

                TileType tileType = tile?.Type ?? TileType.Empty;
                if (IsPassable(tileType))
                {
                    X = newX;
                    Y = newY;
                    return movement;
                }
                else
                {
                    return Movement.NoMovement;
                }
            }

            public bool Attack(Enemy enemy)
            {
                if (IsAdjacent(enemy))
                {
                    int damageDealt = CalculateDamage();
                    bool isEnemyDead = enemy.TakeDamage(damageDealt);
                    if (isEnemyDead)
                    {
                        Controller _controller = new Controller();
                        _controller.Map.RemoveEnemy(enemy.X, enemy.Y);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }

            private bool IsAdjacent(Enemy enemy)
            {
                return (Math.Abs(enemy.X - X) + Math.Abs(enemy.Y - Y)) == 1;
            }

            private int CalculateDamage()
            {
                return Damage;
            }

            public override string ToString()
            {
                return $"Player Stats:\nHP: {HP}/MaxHP: {MaxHP} \nDamage: {Damage} \n[{X},{Y}]\nGold: {goldCount}";
            }
        }

        public class Map
        {
            private Tile[,] tiles;
            private Hero hero;
            private Enemy[] enemies;
            private Item[] items;
            private int width;
            private int height;
            private Random random;

            public Enemy[] EnemiesLst
            {
                get { return enemies; }
                set { enemies = value; }
            }
            public void RemoveEnemy(int x, int y)
            {
                Tiles[x, y].Type = TileType.Empty;
            }

            public Tile[,] Tiles
            {
                get { return tiles; }
                set { tiles = value; }
            }

            public Hero Hero
            {
                get { return hero; }
                set { hero = value; }
            }

            public Enemy[] Enemies
            {
                get { return enemies; }
                set { enemies = value; }
            }

            public Item[] Items
            {
                get { return items; }
                set { items = value; }
            }

            public int Width
            {
                get { return width; }
                set { width = value; }
            }

            public int Height
            {
                get { return height; }
                set { height = value; }
            }

            public Random Random
            {
                get { return random; }
                set { random = value; }
            }

            public Map(int minWidth, int maxWidth, int minHeight, int maxHeight, int numEnemies, int numItems, int numWeapons)
            {
                random = new Random();

                width = random.Next(minWidth, maxWidth + 1);
                height = random.Next(minHeight, maxHeight + 1);


                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (i == 0 || i == height - 1 || j == 0 || j == width - 1)
                        {
                            Tiles[i, j] = new Obstacle(j, i, '#');
                        }
                        else
                        {
                            Tiles[i, j] = new EmptyTile(j, i, '.');
                        }
                    }
                }
                enemies = new Enemy[numEnemies];
          //      numWeapons = new [numEnemies];

                for (int i = 0; i < numEnemies; i++)
                {
                    int rand = random.Next(1, 3);
                    if (rand == 1)
                    {
                        enemies[i] = (SwampCreature)Create(TileType.SwampCreature);
                    }
                    if (rand == 2)
                    {
                        enemies[i] = (Mage)Create(TileType.Mage);
                    }

                    tiles[enemies[i].Y, enemies[i].X] = enemies[i];
                }
                for (int i = 0; i < numWeapons; i++)
                {
                    int x = random.Next(0, width);
                    int y = random.Next(0, height);
                    while (tiles[x, y].Type != Tile.TileType.Empty)
                    {
                        x = random.Next(0, width);
                        y = random.Next(0, height);
                    }
           //         tiles[x, y] = Tile.TileType.DroppedWeapon; place weapon task 2 3.1
                }

                UpdateVision();
            }

            public void UpdateVision()
            {
                Hero.Vision[0] = tiles[Hero.Y + 1, Hero.X];
                Hero.Vision[1] = tiles[Hero.Y - 1, Hero.X];
                Hero.Vision[2] = tiles[Hero.Y, Hero.X - 1];
                Hero.Vision[3] = tiles[Hero.Y, Hero.X + 1];

                for (int i = Hero.Y - 1; i <= Hero.Y + 1; i++)
                {
                    for (int j = Hero.X - 1; j <= Hero.X + 1; j++)
                    {
                        if (i < 0 || i >= height || j < 0 || j >= width || (i == Hero.Y && j == Hero.X))
                        {
                            continue;
                        }

                        if (tiles[i, j] is Item)
                        {
                            Item itemTile = (Item)tiles[i, j];
                            Hero.Vision[4] = itemTile;
                        }
                    }
                }
            }

            private Tile Create(TileType type)
            {
                int x;
                int y;

                do
                {
                    x = random.Next(width);
                    y = random.Next(height);
                } while (!(tiles[y, x] is EmptyTile));

                switch (type)
                {
                    case TileType.Hero:
                        return new Hero(x, y, 'H', 1);
                    case TileType.SwampCreature:
                        return new SwampCreature(x, y);
                    case TileType.Mage:
                        return new Mage(x, y);
                    case TileType.Gold:
                        return new Gold(x, y, random.Next(1, 6));
                    default:
                        return null;
                }
            }
            private List<Character> GetCharacters()
            {
                List<Character> characters = new List<Character>();
                characters.Add(hero);
                characters.AddRange(enemies);
                return characters;
            }
            public Item GetItemAtPosition(int x, int y)
            {
                Item item = null;
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].X == x && items[i].Y == y)
                    {
                        item = items[i];
                        RemoveEnemy(items[i].X, items[i].Y);
                        break;
                    }
                }
                if (item != null)
                    return item;
                else return null;

            }
        }


        public class GameEngine
        {

            private Map _map;
            View view = new View();
            public GameEngine()
            {
                _map = new Map(10, 20, 10, 20, 5, 5, 5);
                _map.Hero.X = 5;
                _map.Hero.Y = 5;
            }

            public Map Map => _map;

            public bool MovePlayer(Movement direction)
            {
                int targetX = _map.Hero.X;
                int targetY = _map.Hero.Y;
                int previosPosX = _map.Hero.X;
                int previosPosY = _map.Hero.Y;

                switch (direction)
                {
                    case Movement.Up:
                        targetY--;
                        break;
                    case Movement.Down:
                        targetY++;
                        break;
                    case Movement.Left:
                        targetX--;
                        break;
                    case Movement.Right:
                        targetX++;
                        break;
                    default:
                        Debug.WriteLine("invalid movement in GameEngine");
                        break;
                }
                Tile tile = _map.Tiles[targetX, targetY];
                if (tile.IsValidMove(direction))
                {
                    tile.Type = TileType.Hero;
                    _map.Tiles[previosPosX, previosPosY].Type = TileType.Empty;

                    if (tile is Gold)
                    {
                        Gold gold = (Gold)tile;
                        _map.Hero.GoldCount += gold.Amount;
                        _map.RemoveEnemy(gold.X, gold.Y);
                    }
                    if (tile! is Gold && tile is Item)
                    {
                        Map.GetItemAtPosition(tile.X, tile.Y);
                    }
                    return true;
                    Map.UpdateVision();
                    Map.ToString();
                    view.UpdateEnemies();
                    view.UpdateHero();
                }
                else
                {
                    return false;
                    Map.UpdateVision();
                    Map.ToString();
                    view.UpdateEnemies();
                    view.UpdateHero();
                }

            }
            public void Save()
            {
                // Create a binary formatter to serialize the Map object
                BinaryFormatter formatter = new BinaryFormatter();

                // Open a file stream to save the serialized object to disk
                using (FileStream stream = new FileStream("savegame.bin", FileMode.Create))
                {
                    formatter.Serialize(stream, _map);
                }
            }

            public void Load(string fileName)
            {
                BinaryFormatter formatter = new BinaryFormatter();

                using (FileStream stream = new FileStream(fileName, FileMode.Open))
                {
                    _map = (Map)formatter.Deserialize(stream);
                }
            }
            public override string ToString()
            {
                string[,] tileChars = new string[_map.Width, _map.Height];

                for (int y = 0; y < _map.Height; y++)
                {
                    for (int x = 0; x < _map.Width; x++)
                    {
                        Tile tile = _map.Tiles[x, y];
                        if (tile.Type == TileType.Hero)
                        {
                            tileChars[x, y] = "H";
                        }
                        else if (tile.Type == TileType.Empty)
                        {
                            tileChars[x, y] = ".";
                        }
                        else if (tile.Type == TileType.SwampCreature)
                        {
                            tileChars[x, y] = "S";
                        }
                        else if (tile.Type == TileType.Obstacle)
                        {
                            tileChars[x, y] = "#";
                        }
                        else if (tile.Type == TileType.Mage)
                        {
                            tileChars[x, y] = "M";
                        }
                        else if (tile.Type == TileType.Gold)
                        {
                            tileChars[x, y] = "G";
                        }
                    }
                }

                string output = "";
                for (int y = 0; y < _map.Height; y++)
                {
                    for (int x = 0; x < _map.Width; x++)
                    {
                        output += tileChars[x, y];
                    }
                    output += "\n";
                }

                return output;
            }
            public void MoveEnemies()
            {
                foreach (Enemy enemy in _map.Enemies)
                {
                    if (enemy.Type == TileType.Mage)
                    {
                        continue;
                    }

                    int currRow = enemy.Y;
                    int currCol = enemy.X;

                    int playerRow = _map.Hero.X;
                    int playerCol = _map.Hero.Y;

                    int distance = Math.Abs(currRow - playerRow) + Math.Abs(currCol - playerCol);

                    if (_map.Hero.PlayerMoved)
                    {
                        _map.Hero.PlayerMoved = false;

                        if (distance == 1)
                        {
                            enemy.Attack(_map.Hero);
                            continue;
                        }

                        int nextRow = currRow;
                        int nextCol = currCol;

                        if (playerRow < currRow && enemy.IsValidMove(Movement.Up))
                        {
                            nextRow--;
                        }
                        else if (playerRow > currRow && enemy.IsValidMove(Movement.Down))
                        {
                            nextRow++;
                        }
                        else if (playerCol < currCol && enemy.IsValidMove(Movement.Left))
                        {
                            nextCol--;
                        }
                        else if (playerCol > currCol && enemy.IsValidMove(Movement.Right))
                        {
                            nextCol++;
                        }
                        else
                        {
                            Movement randomMove = (Movement)_map.Random.Next(0, 4);
                            enemy.IsValidMove(randomMove);

                            switch (randomMove)
                            {
                                case Movement.Up:
                                    nextRow--;
                                    enemy.Move(Movement.Up);
                                    break;
                                case Movement.Down:
                                    nextRow++;
                                    enemy.Move(Movement.Down);
                                    break;
                                case Movement.Left:
                                    nextCol--;
                                    enemy.Move(Movement.Left);
                                    break;
                                case Movement.Right:
                                    nextCol++;
                                    enemy.Move(Movement.Right);
                                    break;
                            }
                        }

                        
                        _map.UpdateVision();
                    }
                    else
                    {
                        _map.UpdateVision();
                    }
                }
            }
        }



        //public Model()
        //{
        //    Hero hero = new Hero(1, 1, 'H', 1);
        //    enemies = new Enemy[2];
        //}

        public void AttackEnemy(Enemy enemy)
        {
            _controller.Hero.Attack(enemy);
        }

        public abstract class Item : Tile
        {
            public Item(int x, int y) : base(x, y, ' ')
            {
            }

            public abstract override string ToString();
        }
        public abstract class Weapon : Item
        {
            protected int damage;
            protected int durability;
            protected int cost;
            protected string weaponType;
            public int Damage
            {
                get { return damage; }
                set { damage = value; }
            }

            public virtual int Range
            {
                get { return 1; }
            }

            public int Durability
            {
                get { return durability; }
                set { durability = value; }
            }

            public int Cost
            {
                get { return cost; }
                set { cost = value; }
            }

            public string WeaponType
            {
                get { return weaponType; }
                set { weaponType = value; }
            }

            public Weapon(int x, int y, char symbol) : base(x, y)
            {
            }

            public abstract override string ToString();
        }
        public class MeleeWeapon : Weapon
        {
            public enum Types { Dagger, Longsword }

            //public string WeaponType { get; }

            public override int Range { get { return 1; } }

            public MeleeWeapon(Types weaponType, int x = 0, int y = 0) : base(x, y, 'D')
            {
                switch (weaponType)
                {
                    case Types.Dagger:
                        this.WeaponType = "Dagger";
                        this.Durability = 10;
                        this.Damage = 3;
                        this.Cost = 3;
                        this.Symbol = 'D';
                        break;

                    case Types.Longsword:
                        this.WeaponType = "Longsword";
                        this.Durability = 6;
                        this.Damage = 4;
                        this.Cost = 5;
                        this.Symbol = 'L';
                        break;
                }
            }

            public override string ToString()
            {
                return $"{this.WeaponType} ({this.Durability})";
            }
        }
        public class RangedWeapon : Weapon
        {
            public enum Types { Rifle, Longbow }
            public string WeaponType { get; }
            public override int Range { get; }

            public RangedWeapon(Types type, int x = 0, int y = 0) : base(x, y, ' ')
            {
                switch (type)
                {
                    case Types.Rifle:
                        this.WeaponType = "Rifle";
                        Durability = 3;
                        Range = 3;
                        Damage = 5;
                        Cost = 7;
                        break;
                    case Types.Longbow:
                        this.WeaponType = "Longbow";
                        Durability = 4;
                        Range = 2;
                        Damage = 4;
                        Cost = 6;
                        break;
                    default:
                        this.WeaponType = "Ranged";
                        break;
                }
            }

            public RangedWeapon(Types type, int durability, int x = 0, int y = 0) : base(x, y, ' ')
            {
                switch (type)
                {
                    case Types.Rifle:
                        this.WeaponType = "Rifle";
                        Durability = durability;
                        Range = 3;
                        Damage = 5;
                        Cost = 7;
                        break;
                    case Types.Longbow:
                        this.WeaponType = "Longbow";
                        Durability = durability;
                        Range = 2;
                        Damage = 4;
                        Cost = 6;
                        break;
                    default:
                        this.WeaponType = "Ranged";
                        break;
                }
            }
            public override string ToString()
            {
                return "";
            }
        }

        public class Leader : Enemy
        {
            private Tile? target;
            Controller _controller = new Controller();
            public Tile Target
            {
                get { return target; }
                set { target = value; }
            }

            public Leader(int x, int y) : base(x, y, 2, 20, 'B')
            {
                Weapon weapon = new MeleeWeapon(MeleeWeapon.Types.Longsword);
                _weapon = weapon;
                this.Gold = 2;
            }

            public override Movement ReturnMove(Movement move = 0)
            {
                if (CheckRange(target))
                {
                    return Movement.NoMovement;
                }
                else
                {
                    int dx = target.X - X;
                    int dy = target.Y - Y;

                    while (!IsPassable(_vision[(int)GetDirection(dx, dy)].Type))
                    {
                        dx = random.Next(-1, 2);
                        dy = random.Next(-1, 2);
                    }

                    return GetDirection(dx, dy);
                }
            }

            private Movement GetDirection(int dx, int dy)
            {
                if (dx > 0)
                {
                    return Movement.Right;
                }
                else if (dx < 0)
                {
                    return Movement.Left;
                }
                else if (dy > 0)
                {
                    return Movement.Down;
                }
                else if (dy < 0)
                {
                    return Movement.Up;
                }
                else
                {
                    return Movement.NoMovement;
                }
            }
        }
        public class Shop
        {
            private Weapon[] weapons;
            private Random random;
            private Character buyer;

            public Shop(Character buyer)
            {
                this.buyer = buyer;
                weapons = new Weapon[3];
                random = new Random();
                for (int i = 0; i < weapons.Length; i++)
                {
                    weapons[i] = RandomWeapon();
                }
            }

            private Weapon RandomWeapon()
            {
                int randomIndex = random.Next(4);
                switch (randomIndex)
                {
                    case 0:
                        return new MeleeWeapon(MeleeWeapon.Types.Dagger);
                    case 1:
                        return new MeleeWeapon(MeleeWeapon.Types.Longsword);
                    case 2:
                        return new RangedWeapon(RangedWeapon.Types.Longbow);
                    default:
                        return new RangedWeapon(RangedWeapon.Types.Rifle);
                }
            }

            public bool CanBuy(int num)
            {
                return buyer.Gold >= weapons[num].Cost;
            }

            public void Buy(int num)
            {
                if (CanBuy(num))
                {
                    buyer.Gold -= weapons[num].Cost;
                    buyer.Pickup(weapons[num]);
                    weapons[num] = RandomWeapon();
                }
            }

            public string DisplayWeapon(int num)
            {
                return $"Buy {weapons[num].GetType().Name} ({weapons[num].Cost} Gold)";
            }
        }
        public class Gold : Item
        {
            private int _amount;
            // private Random random;

            public int Amount
            {
                get { return _amount; }
            }

            public Gold(int x, int y, int amount) : base(x, y)
            {
                _amount = amount;
            }

            public override string ToString()
            {
                return "Gold (" + _amount + ")";
            }
        }
        public class Mage : Enemy
        {
            private const int hp = 5;
            private const int damage = 5;

            public Mage(int x, int y) : base(x, y, hp, damage, 'M')
            {
                this.Gold = 3;
            }

            public override Movement ReturnMove(Movement move)
            {
                return Movement.NoMovement;
            }

            public override bool CheckRange(Tile character)
            {
                int xDiff = Math.Abs(character.X - X);
                int yDiff = Math.Abs(character.Y - Y);
                return (xDiff <= 1 && yDiff <= 1);
            }
            public void Attack(Enemy[] enemies)
            {
                foreach (Enemy enemy in enemies)
                {
                    int xDiff = Math.Abs(this.X - enemy.X);
                    int yDiff = Math.Abs(this.Y - enemy.Y);
                    int distance = xDiff + yDiff;

                    if (distance <= 2 && enemy.Type == TileType.Mage || enemy.Type == TileType.SwampCreature || enemy.Type == TileType.Hero)
                    {
                        //var target = enemy.X, enemy.Y;
                        enemy.TakeDamage(this.Damage);
                    }
                }
            }
        }
    }

}




