using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using winformapp;


namespace WEBDESKTOP
{
    static class Program
    {


        [STAThread]
        static void Main(string[] args)
        {


                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());

        }


    }
}
