using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NCILib = EndevFWNetCore.NetComInstructionLib;

namespace EndevFWNetCore
{
    public enum MessageType
    {
        INSTRUCTION,
        PREAUTH,
        NONE
    }

    public abstract class NetComInstruction
    {
        public MessageType MsgType { get; set; } = MessageType.INSTRUCTION;
        public string Username { get; set; } = null;
        public string Password { get; set; } = null;
        public string Instruction { get; set; } = null;
        public string Value { get; set; } = null;
        public object[] Parameters { get; set; } = null;
        public string ReplyRequest { get; set; } = null;

        public NetComInstruction(NetComUser pUser) : this(pUser, null, null, null) { }
        public NetComInstruction(NetComUser pUser, string pValue) : this(pUser, pValue, null, null) { }
        public NetComInstruction(NetComUser pUser, string pValue, object[] pParameters) : this(pUser, pValue, pParameters, null) { }
        public NetComInstruction(NetComUser pUser, string pValue, string pReplyRequest) : this(pUser, pValue, null, pReplyRequest) { }
        public NetComInstruction(NetComUser pUser, string pValue, object[] pParameters, string pReplyRequest)
        {
            MsgType = MessageType.INSTRUCTION;
            if (pUser.GetType() == typeof(NetComClient))
            {
                Username = (pUser as NetComClient).Username;
                Password = (pUser as NetComClient).Password;
            }

            if (pUser.GetType() == typeof(NetComUserDummy))
            {
                Username = (pUser as NetComUserDummy).Username;
                Password = (pUser as NetComUserDummy).Password;
            }

            Value = pValue;
            Parameters = pParameters;
            ReplyRequest = pReplyRequest;
        }

        public virtual string Encode(bool pRSAEncrypt = false, string pPartnerPublicKey = null)
        {
            StringBuilder sb = new StringBuilder();

            if(pRSAEncrypt) sb.Append("RSA");
            sb.Append("{");

            if(MsgType != MessageType.NONE)  sb.Append($"[MESSAGETYPE:{B64E(MsgType.ToString())}],");
            if(Username != null)     sb.Append($"[USERNAME:{B64E(Username)}],");

            if (Password != null)
            {
                if (pRSAEncrypt) sb.Append($"[PASSWORD:{NetComUser.LocalUser.RSA.Encrypt(B64E(Password), pPartnerPublicKey)}],");
                else sb.Append($"[PASSWORD:{B64E(Password)}],");
            }
               
            if(Instruction != null)  sb.Append($"[INSTRUCTION:{B64E(Instruction.ToString())}],");
            if(Value != null)        sb.Append($"[VALUE:{B64E(Value)}],");
            if (Parameters != null)
            {
                sb.Append($"[PARAMETERS:");
                foreach (object param in Parameters)
                    sb.Append($"<{B64E(param.GetType().ToString())}#{B64E(param.ToString())}>|");
                sb.Append($"],");
            }
            if(ReplyRequest != null) sb.Append($"[REPREQ:{B64E(ReplyRequest)}],");

            sb.Append("};");

            return sb.ToString();
        }

        // Base64 Encode
        private string B64E(string pPlainText) => Convert.ToBase64String(Encoding.UTF8.GetBytes(pPlainText));

        // Base64 Decode
        private static string B64D(string pBase64String) => Encoding.UTF8.GetString(Convert.FromBase64String(pBase64String));

        public abstract void Execute(NetComClientData pClient = null);

        // ToString is identical to Encode
        public sealed override string ToString() => Encode(false);

        public static IEnumerable<NetComInstruction> Parse(NetComUser pLocalUser, string pNetComInstructionString)
        {
            NetComRSAHandler RSA = null;
            
            string ncInstr = pNetComInstructionString;

            bool rsaMessage = false;

            if (pLocalUser.GetType() == typeof(NetComServer)) RSA = (pLocalUser as NetComServer).RSA;
            if (pLocalUser.GetType() == typeof(NetComClient))  RSA = (pLocalUser as NetComClient).RSA;

            // Split up multiple commands (e.g. blocked buffer) 
            foreach(string instr in ncInstr.Split(';'))
            {
                if (string.IsNullOrEmpty(instr)) continue;

                // Check if Instruction is RSA-Encrypted
                if (pNetComInstructionString.StartsWith("RSA")) rsaMessage = true;

                string tmpInstr = instr.TrimStart('{').TrimEnd('}');

                MessageType messageType = MessageType.NONE;
                string username = null;
                string password = null;
                string instruction = null;
                string value = null;
                List<string[]> parameters = new List<string[]>();
                string replyrequest = null;

                foreach (string instrObj in tmpInstr.Split(','))
                {
                    if (string.IsNullOrEmpty(instrObj)) continue;

                    string[] instrParts = instrObj.TrimStart('[').TrimEnd(']').Split(':');

                    switch(instrParts[0])
                    {
                        case "MESSAGETYPE":
                            Enum.TryParse(B64D(instrParts[1]), out messageType);
                            break;
                        case "USERNAME":
                            username = B64D(instrParts[1]);
                            break;
                        case "PASSWORD":
                            if (rsaMessage) password = B64D(RSA.Decrypt(instrParts[1]));
                            else password = B64D(instrParts[1]);
                            break;
                        case "INSTRUCTION":
                            instruction = B64D(instrParts[1]);
                            break;
                        case "VALUE":
                            value = B64D(instrParts[1]);
                            break;
                        case "PARAMETERS":
                            foreach(string paramGroup in instrParts[1].Split('|'))
                            {
                                if (string.IsNullOrEmpty(paramGroup)) continue;
                                parameters.Add(paramGroup.TrimStart('<').TrimEnd('>').Split('='));
                            }
                            break;
                        case "REPREQ":
                            replyrequest = instrParts[1];
                            break;
                    }
                }

                yield return (NetComInstruction) Activator.CreateInstance(Type.GetType(instruction), new NetComUserDummy(username, password, value) as NetComUser, value, parameters.ToArray(), replyrequest);
            }
        }
    }
}
