using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Configuration.Install;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Text.RegularExpressions;

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
                RegistryKey pkey = null;
                try
                {
                    pkey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\AtlasTCPSvc");
                }
                catch (Exception e)
                {
                    MessageBox.Show("This program must be launched from administrative or system account.\r\n\r\n" + e.Message, "AtlasTCPSvc");
                    return;
                }
                List<string> tokenList = new List<string>((string[])pkey.GetValue("TokenList", new string[] {}));

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
                        case "-add":
                            {
                                string token = args[1];
                                if (token.Length < 10)
                                    MessageBox.Show("Token must be at least 10 characters long.", "AtlasTCPSvc");
                                else if (tokenList.Contains(token))
                                    MessageBox.Show("This token is already registered.", "AtlasTCPSvc");
                                else
                                {
                                    tokenList.Add(token);
                                    MessageBox.Show("Token added.", "AtlasTCPSvc");
                                }
                                break;
                            }
                        case "-list":
                            {
                                string msg = "Registered tokens (total " + tokenList.Count.ToString() + " entries):\n"
                                    + string.Join("\n", tokenList);
                                Console.Write(msg);
                                MessageBox.Show(msg, "AtlasTCPSvc");
                                break;
                            }
                        case "-del":
                            {
                                string token = args[1];
                                if (tokenList.Contains(token))
                                {
                                    tokenList.Remove(token);
                                    MessageBox.Show("Token removed.", "AtlasTCPSvc");
                                }
                                else
                                    MessageBox.Show("Unable to find specified token.", "AtlasTCPSvc");
                                break;
                            }
                    }

                    pkey.SetValue("TokenList",tokenList.ToArray());
                    pkey.Close();
                }
                else
                {
                    MessageBox.Show("Usage: AtlasTCPSvc.exe -install -- for install system service\n" +
                        "AtlasTCPSvc.exe -uninstall -- for uninstall service\n" +
                        "AtlasTCPSvc.exe -add tokenID -- add auth token to database\n" +
                        "AtlasTCPSvc.exe -del tokenID -- delete auth token from database\n" +
                        "AtlasTCPSvc.exe -list -- list registered tokens", "AtlasTCPSvc");
                }
            } else {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] {	new MainService() };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }

}
