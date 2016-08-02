using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.IO;
using BrowserSwitch;

namespace BrowserSwitchTests
{
    [TestClass]
    public class BrowserTests
    {
        JObject browserRules;

        [TestInitialize]
        public void InitialiseRuleData()
        {
            browserRules = JObject.Parse(File.ReadAllText(Environment.CurrentDirectory + "\\testRules.json"));
        }

        [TestMethod]
        public void TestFullDomain()
        {
            Launcher.LaunchBrowser(browserRules, "http://fulldomain1.httpbin.org/response-headers?Content-Type=text%2Fplain%3B+charset%3DUTF-8&Test=FullDomain&Browser=IE");
        }

        [TestMethod]
        public void TestDomainStart()
        {
            Launcher.LaunchBrowser(browserRules, "http://firstpart.httpbin.org/response-headers?Content-Type=text%2Fplain%3B+charset%3DUTF-8&Test=DomainStart&Browser=Chrome");
        }

        [TestMethod]
        public void TestDomainEnd()
        {
            Launcher.LaunchBrowser(browserRules, "http://skipfirst.httpbin.org/response-headers?Content-Type=text%2Fplain%3B+charset%3DUTF-8&Test=DomainEnd&Browser=Firefox");
        }

        [TestMethod]
        public void TestIntranet()
        {
            Launcher.LaunchBrowser(browserRules, "http://sso/default.aspx?path=/onlinetravelbookingtool");
        }

        [TestMethod]
        public void TestDefault()
        {
            Launcher.LaunchBrowser(browserRules, "http://www.cnn.com");
        }
    }
}
