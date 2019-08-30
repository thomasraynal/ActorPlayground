using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public class ActorId
    {
        public ActorId(string value, string adress)
        {
            Value = value;
            Adress = adress;
        }

        public string Value { get; }
        public string Adress { get; }

        public override bool Equals(object obj)
        {
            return obj is ActorId id &&
                   Value == id.Value &&
                   Adress == id.Adress;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value, Adress);
        }
    }
}
