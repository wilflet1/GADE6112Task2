using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GADE6112.Model;
using static GADE6112.Model.Character;
using static System.Windows.Forms.DataFormats;

namespace GADE6112
{
    class View
    {
        private Map _map = new Map(20, 30, 20, 30, 5, 5);
        private Controller _controller;
        public class MyForm : Form1
        {
           // private Model _model;
            
            private ComboBox _enemyComboBox;
            private Button _attackButton;
            private Controller _controller;
            public Form form = ActiveForm;
            public Model _model { get; } = new Model() ;
            public Map Map { get; }
            public MyForm()
            {
                

                _enemyComboBox = new ComboBox();
                // _enemyComboBox.DataSource = _model.Enemies[]; 
                _enemyComboBox.DisplayMember = "Name";


                _attackButton = new Button();
                _attackButton.Text = "Attack";
                _attackButton.Click += AttackButton_Click;


                Controls.Add(_enemyComboBox);
                Controls.Add(_attackButton);
            }

            private void AttackButton_Click(object sender, EventArgs e)
            {
                // Get the selected enemy from the ComboBox
                Enemy selectedEnemy = (Enemy)_enemyComboBox.SelectedItem;

                // Call the attack method on the selected enemy
                _controller.Hero.Attack(selectedEnemy);
            }
            
            

            //public View(Model model, Controller controller)
            //{
            //    _model = model;
            //    _controller = controller;
            //}

            
            public Label StatsLabel
            {
                get { return statsLabel; }
                set { statsLabel = value; }
            }
            public Label MapLabel
            {
                get { return mapLabel; }
                set { mapLabel = value; }
            }

            private void OnButtonClick(object sender, EventArgs e)
            {
                //_controller.UpdateModel();
                UpdateView();
            }
            public void UpdateView()
            {

                // Update the form UI based on the model data
                mapLabel = form.Controls["label1"] as Label;
                //statsLabel = .ToString();
                // mapLabel.Text = Model.GameEngine.
            }

            

        }
        Form1 form = new Form1();
        public void UpdateHero()
            {
                Label heroLabel = form.Controls["heroLabel"] as Label;
                heroLabel.Text = _controller.Hero.ToString();
            }
            public void UpdateEnemies()
            {

                Label enemyLabel = form.Controls["enemyLabel"] as Label;
                StringBuilder sb = new StringBuilder();
                Model model = new Model();
                foreach (Enemy enemy in _map.Enemies)
                {
                    sb.AppendLine(enemy.ToString());
                }
                enemyLabel.Text = sb.ToString();
            }
        
    }
}
