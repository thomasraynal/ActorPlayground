using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Orleans.Basics
{
    public class CcyPairChanged : ICcyPair
    {
        public CcyPairChanged(string id, bool isActive, double ask, double bid)
        {
            IsActive = isActive;
            Id = id;
            Ask = ask;
            Bid = bid;
            Date = DateTime.Now;
        }

        public string Id { get; }

        public bool IsActive { get; }

        public double Ask { get; }

        public double Bid { get; }

        public DateTime Date { get; }
    }
}
