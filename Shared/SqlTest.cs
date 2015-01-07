using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Arbitrage
{
    [TestClass]
    public class SqlTest
    {
        [TestMethod]
        public void TestDbNullToInt()
        {
            Assert.AreEqual(32, Sql.DbNullToInt(32));
        }
    }
}
