using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public class RootRemoteConfiguration : IRootRemoteConfiguration
    {
        public RootRemoteConfiguration(string adress)
        {
            Adress = adress;
        }

        public string Adress { get; }
    }
}
