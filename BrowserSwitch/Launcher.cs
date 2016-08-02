using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BrowserSwitch
{
    public class Launcher
    {
        public static DateTime logTime;
        public static string executionPath;

        public static void LaunchBrowser(JObject browserRules, string targetUrl)
        {
            try
            {
                // initialise
                logTime = DateTime.Now;
                executionPath = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
                var browser = String.Empty;
                var domain = new Uri(targetUrl).Host;

                // cycle through the urlTypes to try and identify which rule matches our targetUrl ... first one wins
                foreach (var urlRule in browserRules["urlRules"])
                {
                    var urlValues = urlRule["values"];
                    var urlBrowser = urlRule["browser"].Value<string>();
                    var urlType = urlRule["type"].Value<string>();

                    switch (urlType)
                    {
                        case "fullDomain":
                            if (urlValues.Where(c => c.ToString() == domain).Count() > 0) { browser = urlBrowser; }
                            break;
                        case "domainStart":
                            if (urlValues.Where(c => domain.StartsWith(c.ToString())).Count() > 0) { browser = urlBrowser; }
                            break;
                        case "domainEnd":
                            if (urlValues.Where(c => domain.EndsWith(c.ToString())).Count() > 0) { browser = urlBrowser; }
                            break;
                        case "Intranet":
                            if (!domain.Contains(".")) { browser = urlBrowser; }
                            break;
                        default:
                            break;
                    }

                    // if the browser has been set, exit the foreach
                    if (String.IsNullOrEmpty(browser) == false)
                    {
                        WriteToLogFile($"URL Type: {urlType}");
                        break;
                    }
                }

                // if no match, use the default browser
                if (String.IsNullOrEmpty(browser))
                {
                    WriteToLogFile($"URL Type: default");
                    browser = browserRules.SelectToken("defaultBrowser").ToString();
                }

                // launch the chosen browser
                var chosenBrowser = browserRules["browsers"].Children()[browser].ToArray()[0];
                var cmdFile = chosenBrowser.SelectToken("command").ToString();
                cmdFile = cmdFile.Replace("{exePath}", executionPath);
                var cmdArgs = chosenBrowser.SelectToken("args").ToString();
                cmdArgs = cmdArgs.Replace("{url}", targetUrl);
                WriteToLogFile($"Browser: {browser}");
                WriteToLogFile($"File: {cmdFile}");
                WriteToLogFile($"Args: {cmdArgs}");
                ProcessStartInfo startInfo = new ProcessStartInfo(cmdFile, cmdArgs);
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                WriteToLogFile(ex.ToString());
            }
        }

        private static void WriteToLogFile(string logText)
        {
            logText = DateTime.Now.ToString("yyyyMMdd HHmmss.fff - ") + logText + Environment.NewLine;
            Trace.TraceInformation(logText);
            if (ConfigurationManager.AppSettings["enableLogging"].ToLower() == "true")
            {
                File.AppendAllText(executionPath + $@"\log_{logTime.ToFileTime()}.txt", logText);
            }
        }
    }
}
