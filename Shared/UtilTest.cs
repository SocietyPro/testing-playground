using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Arbitrage
{
    [TestClass]
    public class UtilTest
    {
        [TestMethod]
        public void TestCeilingAfterPoint()
        {
            Assert.AreEqual(new Decimal(123.5), Util.CeilingAfterPoint(new Decimal(123.456), 1));
            Assert.AreEqual(new Decimal(123.5), Util.CeilingAfterPoint(new Decimal(123.421), 1));
            Assert.AreEqual(new Decimal(123.1), Util.CeilingAfterPoint(new Decimal(123.01), 1));
            Assert.AreEqual(new Decimal(123.13), Util.CeilingAfterPoint(new Decimal(123.1288), 2));
        }

        [TestMethod]
        public void TestCleanStringSafeForLog()
        {
            Assert.AreEqual("ab", Util.CleanStringSafeForLog("ab"));
            Assert.AreEqual(" ab ", Util.CleanStringSafeForLog("{ab}"));
            Assert.AreEqual(" ab", Util.CleanStringSafeForLog("{ab"));
            Assert.AreEqual("ab ", Util.CleanStringSafeForLog("ab}"));
            
        }

        [TestMethod]
        public void TestComputeMd5()
        {
            Assert.AreEqual("E80B5017098950FC58AAD83C8C14978E", Util.ComputeMd5("abcdef"));
        }

        [TestMethod]
        public void TestFindBetween()
        {
            string data = "abcdefghijklmnopqrstuvwxyz";
            string result = Util.FindBetween("b", "h", data);
            Assert.AreEqual("cdefg", result);

            result = Util.FindBetween("b", "h", data, -1);
            Assert.AreEqual("cdefg", result);

            result = Util.FindBetween("b", "h", data, 3);
            Assert.IsNull(result);

            result = Util.FindBetween("b", "h", data,41);
            Assert.IsNull(result);

            
        }

        
        
    }
}
