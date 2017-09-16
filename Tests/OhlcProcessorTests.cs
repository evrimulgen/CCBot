using System;
using System.Collections.Generic;
using Core.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.OhlcAnalysis;

namespace Tests
{
    [TestClass]
    public class OhlcProcessorTests
    {
        private IOhlcProcessor _processor;
        public OhlcProcessorTests()
        {
            Setup();
            
        }

        private void Setup()
        { 
        }

        [TestMethod]
        public void TestMethod1()
        {

        }

        [TestMethod]
        public void TestSimpleMovingAverage()
        {
            var candleData = GetCandleData();
            var result = _processor.GetMovingAverage(candleData.ResultSet[60], 5);

            Assert.AreEqual(result, 2);
        }

        public CandleData GetCandleData()
        {
            var data = new CandleData()
            {
                ResultSet = new Dictionary<int, IList<CandleStick>>()
            };

            var candleSet = new List<CandleStick>();

            var candle1 = new CandleStick()
            {
                CloseTime = 1505599920,
                OpenPrice = 0.014018m,
                HighPrice = 0.014018m,
                LowPrice = 0.014018m,
                ClosePrice = 0.014018m,
                Volume = 41.618446m
            };

            var candle2 = new CandleStick()
            {
                CloseTime = 1505599860m,
                OpenPrice =  0.014158m,
                HighPrice = 0.014158m,
                LowPrice = 0.014011m,
                ClosePrice =0.014143m,
                Volume = 300.7724m
            };

            var candle3 = new CandleStick()
            {
                CloseTime = 1505599800,
                OpenPrice =  0.014158m,
                HighPrice = 0.014158m,
                LowPrice = 0.014158m,
                ClosePrice =0.014158m,
                Volume = 11.847191m
            };

            var candle4 = new CandleStick()
            {
                CloseTime = 1505599740,
                OpenPrice =  0.014158m,
                HighPrice = 0.014158m,
                LowPrice = 0.014158m,
                ClosePrice =0.014158m,
                Volume = 1.9391322m
            };

            candleSet.Add(candle1);
            candleSet.Add(candle2);
            candleSet.Add(candle3);
            candleSet.Add(candle4);

            data.ResultSet.Add(60, candleSet);
            return data;
        }
    }
}
