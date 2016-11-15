using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameNS;
using Designer;
using Filer;

namespace Sokoban
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            FormMain formMain = new FormMain();
            formMain.AddController(new SokobanController(new Game(), formMain, new Design(), new TheFiler(new Converter())));
            Application.Run(formMain);
        }
    }
}
