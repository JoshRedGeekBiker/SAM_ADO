using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

static class Program
    {
        [STAThread]
        static void Main()
        {
            var cuenta = 0;
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Process[] procesos = Process.GetProcesses();

            foreach(Process p in procesos)
            {
                if (p.ProcessName == "SAM")
                {
                    cuenta++;
                }
            }

            if (cuenta < 2)
             {
                ParSAM.LeerArchivo();

                new SAM();
            }

            Environment.Exit(0);
        }
    }

