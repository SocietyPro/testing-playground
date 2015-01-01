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
        
        private TestObjectFactory factory = new TestObjectFactory();
        private HttpWebResponse response;
        private HttpWebRequest request;
        private ClientBitfinex client; 

        [TestInitialize]
        public void Initialize()
        {
            client = new ClientBitfinex();
            request = Mock.Create<HttpWebRequest>();
            response = Mock.Create<HttpWebResponse>();
            Mock.SetupStatic<WebRequest>();
            Mock.Arrange(() => WebRequest.Create(Arg.AnyString)).Returns(request);
            Mock.Arrange(() => request.GetResponse()).Returns(response);
            client.ApiPassword = "pwd";
        }

        [TestCleanup]
        public void Cleanup()
        {
            
        }

        [TestMethod]
        public void TestTradeHistory()
        {
            using (Stream stream = factory.ToStream("mockedresponse"))
            {
                Mock.Arrange(() => response.GetResponseStream()).Returns(stream);
                string result = client.GetTradeHistory();
                Assert.AreEqual("mockedresponse", result);
            }
        }
    }
}
