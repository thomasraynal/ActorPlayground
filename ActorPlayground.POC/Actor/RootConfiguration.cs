using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public class RootConfiguration : IRootConfiguration
    {
        public RootConfiguration(string adress)
        {
            Adress = adress;
        }

        public string Adress { get; }
    }
}
