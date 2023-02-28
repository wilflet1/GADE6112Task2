using System;
using System.Windows.Forms;
using static GADE6112.Model;
using static System.Windows.Forms.DataFormats;

namespace GADE6112
{
    static class Program
    {
        
        [STAThread]
        static void Main()
        {
            View.MyForm frm = new View.MyForm();
            frm.MapLabel.Text = "";//engine.ToString();

            GameEngine gameEngine = new GameEngine();
            gameEngine.Load("savegame.bin");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            
            







        }
    }
}