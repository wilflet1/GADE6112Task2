using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GADE6112.Model.Character;
using static GADE6112.Model;
using System.Diagnostics;
using System.Collections.Specialized;
using System.ComponentModel;

namespace GADE6112
{
    class Controller
    {
        GameEngine _gameEngine = new GameEngine();
        
        public string? mapString;

        //Model.Hero hero = new Model.Hero(1, 1, 'f', 1);

        //private Model? _model;

        private View? _view;

        public Hero? Hero { get; set; }

        public Map Map { get; set; }
        public Controller()
        {
            //hero.PropertyChanged += Hero_PropertyChanged;
            //_model.Enemies.CollectionChanged += Enemies_CollectionChanged;
        }
        private void Hero_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _view.UpdateHero();
        }

        private void Enemies_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _view.UpdateEnemies();
        }
        //public void Initialize()
        //{
        //    model.Controller = this;
        //    view.Controller = this;

        //    view.Map = model.Map;
        //    view.Hero = model.Hero;

        //    model.MapChanged += view.OnMapChanged;
        //    model.HeroChanged += view.OnHeroChanged;       
        //}

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            mapString = _gameEngine.ToString();
            Debug.WriteLine(mapString);
            switch (e.KeyCode)
            {
                case Keys.Up:
                    _gameEngine.MovePlayer(Movement.Up);
                    Console.WriteLine("Up");
                    break;
                case Keys.Down:
                    _gameEngine.MovePlayer(Movement.Down);
                    break;
                case Keys.Left:
                    _gameEngine.MovePlayer(Movement.Left);
                    break;
                case Keys.Right:
                    _gameEngine.MovePlayer(Movement.Right);
                    break;
                default:
                    break;
            }

            // Refresh the display to update the game view
            // UpdateDisplay();
        }
    }
}
