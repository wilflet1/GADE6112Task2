using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GADE6112.Model;

namespace GADE6112
{
    public partial class Form1 : Form
    {
        private Button[] shopButtons;
        private Weapon[] shopWeapons;
        public Label mapLabel, statsLabel;
        private GameEngine _gameEngine = new GameEngine();

        public Form1()
        {
            InitializeComponent();
            shopButtons = new Button[] { button2, button3, button4 };

            shopWeapons = _gameEngine.Shop.Weapons;
        }
        private void UpdateShopButtons()
        {
            for (int i = 0; i < shopButtons.Length; i++)
            {
                Button button = shopButtons[i];
                Weapon weapon = shopWeapons[i];

                button.Text = weapon.Type.ToString();

                button.Enabled = _gameEngine.Map.Hero.Gold >= weapon.Cost;

                button.Tag = i;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            _gameEngine.Save();
        }

        private void button2_Click(object sender, EventArgs e) //first shop slot
        {
            Weapon weapon = shopWeapons[0];
            Shop shop = new Shop(_gameEngine.Map.Hero);

            if (_gameEngine.Map.Hero.Gold > weapon.Cost)
            {
                _gameEngine.Shop.Buy(weapon.Cost);
                UpdateShopButtons();
                statsLabel.Text = _gameEngine.Map.Hero.ToString();
                _gameEngine.Map.Hero.CurrentWeapon = weapon;
                shopWeapons[0] = shop.RandomWeapon();
            }
            button2.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e) //second shop slot
        {
            Weapon weapon = shopWeapons[1];
            Shop shop = new Shop(_gameEngine.Map.Hero);

            if (_gameEngine.Map.Hero.Gold > weapon.Cost)
            {
                _gameEngine.Shop.Buy(weapon.Cost);
                UpdateShopButtons();
                statsLabel.Text = _gameEngine.Map.Hero.ToString();
                _gameEngine.Map.Hero.CurrentWeapon = weapon;
                shopWeapons[1] = shop.RandomWeapon();
            }
            button3.Enabled = false;
        }

        private void button4_Click(object sender, EventArgs e) //third shop slot
        {
            Weapon weapon = shopWeapons[2];
            Shop shop = new Shop(_gameEngine.Map.Hero);

            if (_gameEngine.Map.Hero.Gold > weapon.Cost)
            {
                //shopWeapons[2];
                
                _gameEngine.Shop.Buy(weapon.Cost);
                UpdateShopButtons();
                statsLabel.Text = _gameEngine.Map.Hero.ToString();
                _gameEngine.Map.Hero.CurrentWeapon = weapon;
                shopWeapons[2] = shop.RandomWeapon();
            }
            button4.Enabled = false;
        }

        void Start()
        {
            //statsLabel = this.Controls["1"]
            GameEngine _gameEngine = new GameEngine();
            statsLabel.Text = _gameEngine.Map.Hero.ToString();
        }
    }
}
