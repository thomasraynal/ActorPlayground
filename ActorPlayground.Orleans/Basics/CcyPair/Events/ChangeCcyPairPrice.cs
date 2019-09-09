using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Orleans.Basics
{
    public class ChangeCcyPairPrice : CcyEventBase
    {
        public ChangeCcyPairPrice(string ccyPair, string market, double ask, double bid) : base(ccyPair)
        {
            Ask = ask;
            Bid = bid;
            Market = market;
        }

        public double Ask { get; }
        public double Bid { get; }
        public string Market { get; }
    }
}
