using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndevFWNetCore
{
    public enum MessageType
    {
        INSTRUCTION,
        PREAUTH,
        NONE
    }

    public enum Instruction
    {
        NONE,
        PREAUTH,
        PLAINTEXT,
        MESSAGEBOX,
        NOTIFYBUTTON
    }

    public abstract class NetComInstruction
    {
        public MessageType MsgType { get; set; } = MessageType.INSTRUCTION;
        public string Username { get; set; } = null;
        public string Password { get; set; } = null;
        public Instruction Instruction { get; set; } = Instruction.NONE;
        public string Value { get; set; } = null;
        public object[] Parameters { get; set; } = null;
        public string ReplyRequest { get; set; } = null;

        public NetComInstruction(INetComUser pUser)
        {
            MsgType = MessageType.INSTRUCTION;
            if (pUser.GetType() == typeof(NetComClient))
            {
                Username = (pUser as NetComClient).Username;
                Password = (pUser as NetComClient).Password;
            }
        }
        
        public virtual string Encode()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("{");

            if(MsgType != MessageType.NONE)  sb.Append($"[MESSAGETYPE:{MsgType.ToString()}],");
            if(Username != null)     sb.Append($"[USERNAME:{Username}],");
            if(Password != null)     sb.Append($"[PASSWORD:{Password}],");
            if(Instruction != Instruction.NONE)  sb.Append($"[INSTRUCTION:{Instruction}],");
            if(Value != null)        sb.Append($"[VALUE:{Value}],");
            if (Parameters != null)
            {
                sb.Append($"[PARAMETERS:");
                foreach (object param in Parameters)
                    sb.Append($"<{param.GetType().ToString()}={param.ToString()}>|");
                sb.Append($"],");
            }
            if(ReplyRequest != null) sb.Append($"[REPREQ:{ReplyRequest}],");

            sb.Append("};");

            return sb.ToString();


            // Parameters: 
            // [PARAMETERS:<System.Type.MyType:MyValie>,<...:...>,<...:...>]
        }

        public abstract void Execute();

        // ToString is identical to Encode
        public sealed override string ToString() => Encode();

        // Pure Cosmetic, so it doesn't show up when trying to override e.g. Encode()
        public sealed override bool Equals(object obj) => base.Equals(obj);
        public sealed override int GetHashCode() => base.GetHashCode();

        public static NetComInstruction Parse(string pNetComInstructionString)
        {
            return null;
        }
    }
}
