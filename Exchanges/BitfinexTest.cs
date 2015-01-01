using System.IO;
using System.Net;
using BackendTest.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace Arbitrage.Exchanges
{
    [TestClass]
    public class BitfinexTest
    {
        
        private WebRequestHelper webHelper = new WebRequestHelper();
        private TestObjectFactory factory = new TestObjectFactory();
        
        [TestMethod]
        public void TestTradeHistory()
        {
            ClientBitfinex client = new ClientBitfinex();
            client.ApiPassword = "pwd";
            using (Stream stream = factory.ToStream("mockedresponse"))
            {
                webHelper.MockRequest(stream);
                string result = client.GetTradeHistory();
                Assert.AreEqual("mockedresponse", result);
            }
            
            
        }
    }
}
