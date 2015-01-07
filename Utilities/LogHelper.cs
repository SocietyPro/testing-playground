using System;
using System.Linq;
using Telerik.JustMock;

namespace Arbitrage.Utilities
{
    public class LogHelper
    {
        public static void MockTradeLog()
        {
            Mock.SetupStatic(typeof(TradeLog));
            Mock.Arrange(() => TradeLog.Log(Arg.AnyString)).DoNothing();
        }
    }
}
