using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Orleans.Basics
{
    [Serializable]
    public class ChangeCcyPairPrice : CcyEventBase
    {
        public ChangeCcyPairPrice()
        {
        }

        public ChangeCcyPairPrice(string ccyPair, string market, double ask, double bid) : base(ccyPair)
        {
            Ask = ask;
            Bid = bid;
            Market = market;
        }

        public double Ask { get; internal set; }
        public double Bid { get; internal set; }
        public string Market { get; internal set; }
    }
}
