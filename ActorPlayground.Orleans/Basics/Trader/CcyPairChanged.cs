using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Orleans.Basics
{
    public class CcyPairChanged : ICcyPair, IHasStreamId
    {
        public CcyPairChanged(string id, bool isActive, double ask, double bid)
        {
            IsActive = isActive;
            StreamId = id;
            Ask = ask;
            Bid = bid;
            Date = DateTime.Now;
        }

        public string StreamId { get;  }

        public bool IsActive { get; }

        public double Ask { get; }

        public double Bid { get; }

        public DateTime Date { get; }

    }
}
