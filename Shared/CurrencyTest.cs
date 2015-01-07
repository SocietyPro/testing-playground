using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Arbitrage
{
    [TestClass]
    public class CurrencyTest
    {
        [TestMethod]
        public void TestCurrencyCodeUnknown4Chars()
        {
           Currency value = CurrencyInfo.CurrencyCode("ABCD");
           Assert.AreEqual(Currency.UNK, value);
        }

        [TestMethod]
        public void TestCurrencyCodeFull()
        {
            Currency value = CurrencyInfo.CurrencyCode("Dollar");
            Assert.AreEqual(Currency.UNK, value);
        }

        [TestMethod]
        public void TestCurrencyCodeXbt()
        {
            Currency value = CurrencyInfo.CurrencyCode("XBT");
            Assert.AreEqual(Currency.BTC, value);
        }

        [TestMethod]
        public void TestCurrencyCodeUsd()
        {
            Currency value = CurrencyInfo.CurrencyCode("usd");
            Assert.AreEqual(Currency.USD, value);
        }

        [TestMethod]
        public void TestCurrencyCodeEur()
        {
            Currency value = CurrencyInfo.CurrencyCode("eur");
            Assert.AreEqual(Currency.EUR, value);
        }

        [TestMethod]
        public void TestCurrencyCodeCad()
        {
            Currency value = CurrencyInfo.CurrencyCode("Cad");
            Assert.AreEqual(Currency.CAD, value);
        }

        [TestMethod]
        public void TestCurrencyCodeCny()
        {
            Currency value = CurrencyInfo.CurrencyCode("CNY");
            Assert.AreEqual(Currency.CNY, value);
        }

        [TestMethod]
        public void TestCurrencyCodeLtc()
        {
            Currency value = CurrencyInfo.CurrencyCode("LTC");
            Assert.AreEqual(Currency.LTC, value);
        }

        [TestMethod]
        public void TestCurrencyCodeTooLongCad()
        {
            Currency value = CurrencyInfo.CurrencyCode("acad");
            Assert.AreEqual(Currency.CAD, value);
        }
    }
}


    
  

