using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Orleans.Basics
{
    public class CcyPairChanged : ICcyPair, IEvent
    {
        public CcyPairChanged(string market, string ccyPair, bool isActive, double ask, double bid)
        {
            IsActive = isActive;
            StreamId = ccyPair;
            Ask = ask;
            Bid = bid;
            Date = DateTime.Now;
            Group = market;
        }

        public string Group { get; }

        public string StreamId { get;  }

        public bool IsActive { get; }

        public double Ask { get; }

        public double Bid { get; }

        public DateTime Date { get; }

    }
}
