using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC.Message
{
    public class Pulse : IEvent
    {
        public Pulse(string adress)
        {
            Adress = adress;
        }

        string Adress { get; }
    }
}
