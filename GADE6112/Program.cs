using System;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;

namespace GADE6112
{
    static class Program
    {
        
        [STAThread]
        static void Main()
        {
           
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            
            View.MyForm frm = new View.MyForm();
            frm.MapLabel.Text = "";//engine.ToString();

            






        }
    }
}