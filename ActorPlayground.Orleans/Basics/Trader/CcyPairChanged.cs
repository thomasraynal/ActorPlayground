using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Orleans.Basics
{
    public class CcyPairChanged : ICcyPair, IHasStreamId
    {
        public CcyPairChanged(string market, string ccyPair, bool isActive, double ask, double bid)
        {
            IsActive = isActive;
            StreamId = ccyPair;
            Ask = ask;
            Bid = bid;
            Date = DateTime.Now;
            Market = market;
        }

        public string Market { get; }

        public string StreamId { get;  }

        public bool IsActive { get; }

        public double Ask { get; }

        public double Bid { get; }

        public DateTime Date { get; }

    }
}
