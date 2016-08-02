using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Reflection;

namespace BrowserSwitch
{
    class Program
    {
        static void Main(string[] args)
        {
            string usageMessage = String.Format("Usage Instructions: {0}-r to register the http and http handler in the registry{0}-h to show this Message", Environment.NewLine);

            if (args.Length == 0)
            {
                Console.WriteLine(usageMessage);
            }
            else
            {
                switch (args[0].ToLower())
                {
                    case "-h":
                    case "-help":
                        Console.WriteLine(usageMessage);
                        break;
                    case "-r":
                        RegisterHandler();
                        break;
                    default:
                        // load the browser rules from the rules.json file and call the LaunchBrowser function
                        var browserRules = JObject.Parse(File.ReadAllText(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName + "\\rules.json"));
                        Launcher.LaunchBrowser(browserRules, args[0]);
                        break;
                }
            }
        }

        private static void RegisterHandler()
        {
            // prepare the HKEY_LOCAL_MACHINE\SOFTWARE\Classes\BrowserSwitch registry settings
            if (Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\BrowserSwitch") == null)
            {
                Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Classes\BrowserSwitch\DefaultIcon");
                Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Classes\BrowserSwitch\shell\open\command");
            }
            Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\BrowserSwitch\DefaultIcon", true).SetValue(String.Empty, Assembly.GetExecutingAssembly().Location + ",0");
            Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\BrowserSwitch\shell\open\command", true).SetValue(String.Empty, Assembly.GetExecutingAssembly().Location + " \"%1\"");

            // prepare the HKEY_LOCAL_MACHINE\SOFTWARE\Clients\StartMenuInternet\BrowserSwitch registry settings
            if (Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet\BrowserSwitch") == null)
            {
                Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Clients\StartMenuInternet\BrowserSwitch\Capabilities\StartMenu");
                Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Clients\StartMenuInternet\BrowserSwitch\Capabilities\URLAssociations");
                Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Clients\StartMenuInternet\BrowserSwitch\DefaultIcon");
                Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Clients\StartMenuInternet\BrowserSwitch\shell\open\command");
            }
            Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet\BrowserSwitch", true).SetValue(String.Empty, "BrowserSwitch");
            Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet\BrowserSwitch\Capabilities", true).SetValue("ApplicationIcon", Assembly.GetExecutingAssembly().Location + ",0");
            Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet\BrowserSwitch\Capabilities\StartMenu", true).SetValue("StartMenuInternet", "BrowserSwitch");
            Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet\BrowserSwitch\Capabilities\URLAssociations", true).SetValue("http", "BrowserSwitch");
            Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet\BrowserSwitch\Capabilities\URLAssociations", true).SetValue("https", "BrowserSwitch");
            Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet\BrowserSwitch\DefaultIcon", true).SetValue(String.Empty, Assembly.GetExecutingAssembly().Location + ",0");
            Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet\BrowserSwitch\shell\open\command", true).SetValue(String.Empty, Assembly.GetExecutingAssembly().Location + " \"%1\"");

            // add to the registered applications
            Registry.LocalMachine.OpenSubKey(@"SOFTWARE\RegisteredApplications", true).SetValue("BrowserSwitch", @"Software\Clients\StartMenuInternet\BrowserSwitch\Capabilities");
            
            // set the registry keys for the http and https protocols, to point to this console app for older platforms
            foreach (string protocol in new string[] { "http", "https" })
            {
                // get the current protocol key
                Registry.ClassesRoot.OpenSubKey($@"{protocol}\shell\open\command", true).SetValue(string.Empty, Assembly.GetExecutingAssembly().Location + " \"%1\"");
                Registry.LocalMachine.OpenSubKey($@"SOFTWARE\Classes\{protocol}\shell\open\command", true).SetValue(String.Empty, Assembly.GetExecutingAssembly().Location + " \"%1\"");
            }

            // TODO: This is still not registering completely ... have to go to Control Panel\Programs\Default Programs\Set Associations and set the http association for this to work
        }
    }
}
