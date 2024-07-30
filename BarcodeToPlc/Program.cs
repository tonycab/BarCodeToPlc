using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BarcodeToPlc
{
    internal static class Program
    { 
    
        const string AppId = "Local\\1DDFB948-19F1-417C-903D-BE05335DB8A4";

        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            using (Mutex mutex = new Mutex(false, AppId))
            {
                if (!mutex.WaitOne(0))
                {
                    Console.WriteLine("2nd instance");
                    return;
                }

            }


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
