using System;
using System.Windows.Forms;
using System.Threading;
using System.Numerics;
using Akka;
using Akka.Actor;
using Akka.Routing;
using System.Diagnostics;
using Akka.Configuration;

namespace Fraktal
{
    public static class Run
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
