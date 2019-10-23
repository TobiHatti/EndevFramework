using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndevFWNetCore
{
    public abstract class NetComInstructionBase
    {
        public string MessageType { get; set; } = null;
        public string Username { get; set; } = null;
        public string Password { get; set; } = null;
        public string Instruction { get; set; } = null;
        public string Value { get; set; } = null;
        public string Parameters { get; set; } = null;
        public string ReplyRequest { get; set; } = null;

        public virtual string Encode()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("{");

            if(MessageType != null)  sb.Append($"[MESSAGETYPE:{MessageType}],");
            if(Username != null)     sb.Append($"[USERNAME:{Username}],");
            if(Password != null)     sb.Append($"[PASSWORD:{Password}],");
            if(Instruction != null)  sb.Append($"[INSTRUCTION:{Instruction}],");
            if(Value != null)        sb.Append($"[VALUE:{Value}],");
            if(Parameters != null)   sb.Append($"[PARAMETERS:{Parameters}],");
            if(ReplyRequest != null) sb.Append($"[REPREQ:{ReplyRequest}],");

            sb.Append("};");

            return sb.ToString();
        }

        public abstract void Execute();

        // ToString is identical to Encode
        public sealed override string ToString() => Encode();

        // Pure Cosmetic, so it doesn't show up when trying to override e.g. Encode()
        public sealed override bool Equals(object obj) => base.Equals(obj);
        public sealed override int GetHashCode() => base.GetHashCode();
    }
}
