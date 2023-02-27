using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GADE6112.Model;

namespace GADE6112
{
    public partial class Form1 : Form
    {
        public Label mapLabel, statsLabel;
        public Form1()
        {
            InitializeComponent();
        }
        void Start()
        {
            //statsLabel = this.Controls["1"]
            GameEngine _gameEngine = new GameEngine();
            statsLabel.Text = _gameEngine.Map.Hero.ToString();
        }
    }
}
