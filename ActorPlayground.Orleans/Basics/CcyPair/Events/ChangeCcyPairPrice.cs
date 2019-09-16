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

        public double Ask { get;  set; }
        public double Bid { get;  set; }
        public string Market { get;  set; }
    }
}
