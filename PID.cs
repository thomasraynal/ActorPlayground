using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground
{
    public class PID
    {
        private Process _process;

        public PID(string address, string id)
        {
            Address = address;
            Id = id;
        }

        internal PID(string address, string id, Process process) : this(address, id)
        {
            _process = process;
        }

        internal Process Ref
        {
            get
            {
                var p = _process;

                if (p != null)
                {
                    if (p is ActorProcess lp && lp.IsDead)
                    {
                        _process = null;
                    }
                    return _process;
                }
                return _process;
            }
        }

        public string Address { get; }
        public string Id { get; }

        internal void SendUserMessage(object message)
        {
            var reff = Ref ?? ProcessRegistry.Instance.Get(this);
            reff.SendUserMessage(this, message);
        }

        public void SendSystemMessage(object sys)
        {
            var reff = Ref ?? ProcessRegistry.Instance.Get(this);
            reff.SendSystemMessage(this, sys);
        }

        public string ToShortString()
        {
            return Address + "/" + Id;
        }
    }
}
