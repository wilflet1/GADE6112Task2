using System.Diagnostics;
using System.Text;
using static GADE6112.Model;
using static GADE6112.Model.Character;
using static GADE6112.Model.Tile;

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
            public enum TileType { Hero, SwampCreature, Gold, Weapon, Obstacle, Empty };
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
                GameEngine gm = new GameEngine();
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

            public int HP { get { return _hp; } set { _hp = value; } }
            public int MaxHP { get { return _maxHp; } set { _maxHp = value; } }
            public int Damage { get { return _damage; } set { _damage = value; } }
            public Tile[] Vision { get { return _vision; } }

            public int Range { get { return _range; } set { _range = value; } }

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
                //_hp = hp;
                // _maxHp = hp;
                // _damage = damage;
                _vision = Vision;
            }

            public virtual void Attack(Character target)
            {
                target.HP -= _damage;
            }

            public bool IsDead()
            {
                return _hp <= 0;
            }

            public virtual bool CheckRange(Character target)
            {
                int distance = DistanceTo(target);
                return distance <= 1;
            }

            private int DistanceTo(Character target)
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
            public abstract override string ToString();
        }
        public abstract class Enemy : Character
        {
            protected Random random;

            public Enemy(int x, int y, int damage, int hp, char symbol) : base(x, y, symbol)
            {
                Damage = damage;
                HP = hp;
                MaxHP = hp;
                random = new Random();
            }
            public bool TakeDamage(int damage)
            {
                HP -= damage;
                return HP < 0;
            }
            public override string ToString()
            {
                return $"{GetType().Name} at [{X}, {Y}] (with {Damage} DMG)";
            }
        }
        public class SwampCreature : Enemy
        {
            public SwampCreature(int x, int y) : base(x, y, damage: 1, hp: 10, symbol: 'S')
            {
                x = X;
                y = Y;

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

            public Hero(int x, int y, char symbol, int hp) : base(x, y, 'H')
            {
                Damage = HERO_DAMAGE;
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
                return $"Player Stats:\nHP: {HP}/MaxHP: {MaxHP} \nDamage: {Damage} \n[{X},{Y}]";
            }
        }

        public class Map
        {
            private Tile[,] tiles;
            private Hero hero;
            private SwampCreature[] swampCreatures;
            private int width;
            private int height;
            private Random random;

            public void RemoveEnemy(int x, int y)
            {
                tiles[x, y].Type = TileType.Empty;
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

            public SwampCreature[] SwampCreatures
            {
                get { return swampCreatures; }
                set { swampCreatures = value; }
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
            public Map(int minWidth, int maxWidth, int minHeight, int maxHeight, int numEnemies)
            {
                random = new Random();

                width = random.Next(minWidth, maxWidth + 1);
                height = random.Next(minHeight, maxHeight + 1);

                tiles = new Tile[height, width];

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (i == 0 || i == height - 1 || j == 0 || j == width - 1)
                        {
                            tiles[i, j] = new Obstacle(j, i, '#');
                        }
                        else
                        {
                            tiles[i, j] = new EmptyTile(j, i, '.');
                        }
                    }
                }

                hero = (Hero)Create(TileType.Hero);
                tiles[hero.Y, hero.X] = hero;

                swampCreatures = new SwampCreature[numEnemies];
                for (int i = 0; i < numEnemies; i++)
                {
                    swampCreatures[i] = (SwampCreature)Create(TileType.SwampCreature);
                    tiles[swampCreatures[i].Y, swampCreatures[i].X] = swampCreatures[i];
                }

                UpdateVision();
            }

            public void UpdateVision()
            {
                //foreach (Character character in GetCharacters())
                //{
                //    int x = character.X;
                //    int y = character.Y;

                //    character.Vision[0] = tiles[y + 1, x];
                //    character.Vision[1] = tiles[y - 1, x];
                //    character.Vision[2] = tiles[y, x - 1];
                //    character.Vision[3] = tiles[y, x + 1];
                //}
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
                    default:
                        return null;
                }
            }

            private List<Character> GetCharacters()
            {
                List<Character> characters = new List<Character>();
                characters.Add(hero);
                characters.AddRange(swampCreatures);
                return characters;
            }
        }

        public class GameEngine
        {
            private Map _map;

            public GameEngine()
            {
                _map = new Map(10, 20, 10, 20, 5);
                // Set initial player position to the center of the map
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
                    return true;
                }
                else
                {
                    return false;
                }
            }

            // Override ToString to return a string representation of the map
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
                            tileChars[x, y] = "E";
                        }
                        else if (tile.Type == TileType.Obstacle)
                        {
                            tileChars[x, y] = "#";
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
        }

        public List<Enemy> Enemies { get; set; }

        public Model()
        {
            Hero hero = new Hero(1, 1, 'H', 1);
            Enemies = new List<Enemy>();
        }

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
        public class Gold : Item
        {
            private int amount;
            private Random random;

            public int Amount
            {
                get { return amount; }
            }

            public Gold(int x, int y) : base(x, y)
            {
                random = new Random();
                amount = random.Next(1, 6);
            }

            public override string ToString()
            {
                return "Gold (" + amount + ")";
            }
        }
        public class Mage : Enemy
        {
            private const int hp = 5;
            private const int damage = 5;

            public Mage(int x, int y) : base(x, y, hp, damage, 'M')
            {
            }

            public override Movement ReturnMove(Movement move)
            {
                return Movement.NoMovement;
            }

            public override bool CheckRange(Character character)
            {
                int xDiff = Math.Abs(character.X - X);
                int yDiff = Math.Abs(character.Y - Y);
                return (xDiff <= 1 && yDiff <= 1);
            }
        }
    }

}




