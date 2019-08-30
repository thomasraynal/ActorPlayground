using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public class ActorId
    {
        public ActorId(string value, string adress, ActorType type)
        {
            Value = value;
            Adress = adress;
            Type = type;
        }

        public string Value { get; }
        public string Adress { get; }
        public ActorType Type { get; }

        public override bool Equals(object obj)
        {
            return obj is ActorId id &&
                   Value == id.Value &&
                   Adress == id.Adress &&
                   Type == id.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value, Adress, Type);
        }
    }
}
