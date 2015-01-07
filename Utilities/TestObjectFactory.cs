using System;
using System.IO;
using System.Linq;
using Telerik.JustMock;

namespace Arbitrage.Utilities
{
    public class TestObjectFactory
    {
        public Stream ToStream(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public TradeOrder CreateTradeOrderWithoutOperations(string id)
        {
            TradeOrder order = Mock.Create<TradeOrder>(Behavior.CallOriginal);
            Mock.Arrange(()=> order.Save()).DoNothing();
            return order;
        }

        public TradeOrder CreateTradeOrder(string id)
        {
            TradeOrder order = new TradeOrder();
            order.Id = id;
            order.VolumeExecuted = 12;
            order.VolumeReceived = 15;
            order.DateExecutedFirst = new DateTime(2015,01,03);
            order.Pair = new CurrencyPair(Exchange.OKCoin, Currency.CNY, Currency.BTC);
            order.Volume = 150;
            order.Price = new Decimal(345.10);
            order.Active = true;
            order.VolumeExecutedAtMarket = 12;
            order.VolumeReceivedAtMarket = 15;
            order.VolumeBalanceReportedReceived = 13;
            return order;
        }
       
    }
}
