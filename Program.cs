using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Configuration.Install;
using System.Reflection;
using System.Windows.Forms;

namespace AtlasTCPSvc
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (System.Environment.UserInteractive)
            {
                if (args.Length > 0)
                {
                    switch (args[0])
                    {
                        case "-install":
                            {
                                ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                                MessageBox.Show("Service installed", "AtlasTCPSvc");
                                break;
                            }
                        case "-uninstall":
                            {
                                ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                                MessageBox.Show("Service uninstalled", "AtlasTCPSvc");
                                break;
                            }
                    }
                }
                else
                {
                    MessageBox.Show("Usage: AtlasTCPSvc.exe -install -- for install system service\n" +
                        "AtlasTCPSvc.exe -uninstall -- for uninstall service", "AtlasTCPSvc");
                }
            } else {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] {	new MainService() };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }

}
