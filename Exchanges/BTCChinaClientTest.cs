using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Telerik.JustMock;

namespace Arbitrage.Exchanges
{
    [TestClass]
    public class BTCChinaClientTest
    {
        private ClientBTCChina client;
        private PrivateAccessor accessor;

        [TestInitialize]
        public void Setup()
        {
            client = Mock.Create<ClientBTCChina>(Behavior.CallOriginal);
            accessor = new PrivateAccessor(client);
        }

        [TestMethod]
        [Ignore]
        public void Test_ExecuteTradeOrderSupportedPair()
        {
            TradeOrder order = Mock.Create<TradeOrder>(Behavior.CallOriginal);
            Mock.NonPublic.Arrange(client, "ApplyJsonDataToTraderOrder", order, ArgExpr.IsAny<Dictionary<string, object>>(), ArgExpr.Ref(ArgExpr.IsAny<string>())).DoNothing();
            //ApplyJsonDataToTraderOrder(TradeOrder tradeOrder, JToken orderJson)
            accessor.CallMethod("_ExecuteTradeOrder", order);
        }

        private void MockDateTimeNow(DateTime tradeDate)
        {
            Mock.SetupStatic(typeof(DateTime));
            Mock.Arrange(() => DateTime.UtcNow).Returns(tradeDate);
        }

       private void MockReadStatus()
        {
            Mock.SetupStatic(typeof(JObject));
            Mock.Arrange(() => JObject.Parse(Arg.AnyString)).DoNothing();
            Mock.NonPublic.Arrange(client, "ReadStatus", ArgExpr.IsAny<TradeOrder>(), ArgExpr.IsAny<JObject>()).DoNothing().OccursOnce();
        }

        private void MockSendRequest()
        {
            Mock.NonPublic.Arrange<string>(client, "SendRequest", ArgExpr.IsAny<string>(), ArgExpr.IsAny<Dictionary<string, object>>(), ArgExpr.Ref(ArgExpr.IsAny<string>())).Returns("response");
        }
    }
}
