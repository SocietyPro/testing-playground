using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace Arbitrage.Exchanges
{
    [TestClass]
    public class VaultOfSatoshiTest
    {

        private ClientVaultOfSatoshi client;
        private PrivateAccessor accessor;

        [TestInitialize]
        public void Setup()
        {
            client = Mock.Create<ClientVaultOfSatoshi>(Behavior.CallOriginal);
            client.ApiKey = "key";
            client.ApiPassword = "pwd";
            accessor = new PrivateAccessor(client);
        }

        [TestMethod]
        [Ignore]
        public void TestSendPrivateRequest()
        {
            //JToken SendPrivateRequest(string urlPath, out string error, Dictionary<string, string> parameters = null) 
            string url = "some_url";
            string error = "";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            var result = accessor.CallMethod("SendPrivateRequest", url, error, parameters);
        }
    }
}
