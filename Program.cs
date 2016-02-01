using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Configuration.Install;
using System.Reflection;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace AtlasTCPSvc
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void showMessage(string msg)
        {
            Console.WriteLine(msg);
        }

        static void Main(string[] args)
        {
            if (System.Environment.UserInteractive)
            {
                showMessage("\r\nATLAS TCP serivce, v" + Assembly.GetExecutingAssembly().GetName().Version.ToString() + "\r\n");
                RegistryKey pkey = null;
                try
                {
                    pkey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\AtlasTCPSvc");
                }
                catch (Exception e)
                {
                    showMessage("This program must be launched from administrative or system account.\r\n\r\n" + e.Message);
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
                                showMessage("Service installed");
                                break;
                            }
                        case "-uninstall":
                            {
                                ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                                showMessage("Service uninstalled");
                                break;
                            }
                        case "-add":
                            {
                                string token = args[1];
                                if (token.Length < 10)
                                    showMessage("Token must be at least 10 characters long.");
                                else if (tokenList.Contains(token))
                                    showMessage("This token is already registered.");
                                else
                                {
                                    tokenList.Add(token);
                                    showMessage("Token added.");
                                }
                                break;
                            }
                        case "-list":
                            {
                                string msg = "Registered tokens (total " + tokenList.Count.ToString() + " entries):\n"
                                    + string.Join("\n", tokenList);
                                showMessage(msg);
                                break;
                            }
                        case "-del":
                            {
                                string token = args[1];
                                if (tokenList.Contains(token))
                                {
                                    tokenList.Remove(token);
                                    showMessage("Token removed.");
                                }
                                else
                                    showMessage("Unable to find specified token.");
                                break;
                            }
                    }

                    pkey.SetValue("TokenList",tokenList.ToArray());
                    pkey.Close();
                }
                else
                {
                    showMessage("Usage: AtlasTCPSvc.exe -install -- for install system service\r\n" +
                        "AtlasTCPSvc.exe -uninstall -- for uninstall service\r\n" +
                        "AtlasTCPSvc.exe -add tokenID -- add auth token to database\r\n" +
                        "AtlasTCPSvc.exe -del tokenID -- delete auth token from database\r\n" +
                        "AtlasTCPSvc.exe -list -- list registered tokens");
                }
            } else {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] {	new MainService() };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }

}
