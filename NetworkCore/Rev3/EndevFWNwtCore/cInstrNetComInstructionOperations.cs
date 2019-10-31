using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EndevFWNwtCore
{
    public class InstructionOperations
    {
        public static IEnumerable<InstructionBase> Parse(NetComUser pLocalUser, Socket pReceptionSocket, string pInstructionString, ClientList pServerClientList = null)
        {
            //  RSA:<Base64>;RSA:<Base64>;

            lock (pLocalUser)
            {
                foreach (string encodedInstruction in pInstructionString.Split(';'))
                {
                    string username = null;
                    string password = null;
                    string instruction = null;
                    string value = null;
                    List<object> parameters = new List<object>();
                    string signaturePlain = null;
                    string signatureRSA = null;
                    string publicKey = null;
                    NetComUser user = null;

                    if (string.IsNullOrEmpty(encodedInstruction)) continue;

                    // [RSA:]<Base64>

                    bool rsaEncoded = false;

                    string encInstr = encodedInstruction;

                    if(encInstr.StartsWith("RSA:"))
                    {
                        rsaEncoded = true;
                        encInstr = encInstr.Remove(0, 4);
                    }

                    // <Base64>

                    string decodedInstruction = Base64Handler.Decode(encInstr);

                    // INS:B64,VAL:B64,PAR:B64#B64|B64#B64|,

                    foreach (string encodedInstrSegment in decodedInstruction.Split(','))
                    {
                        if (string.IsNullOrEmpty(encodedInstrSegment)) continue;

                        // INS:B64 / PAR:B64#B64|B64#B64| 

                        string[] encodedSegmentParts = encodedInstrSegment.Split(':');

                        switch(encodedSegmentParts[0])
                        {
                            case "PUK": publicKey = Base64Handler.Decode(encodedSegmentParts[1]); break;
                            case "USR": username = Base64Handler.Decode(encodedSegmentParts[1]); break;
                            case "PSW": 
                                if(rsaEncoded)
                                    password = RSAHandler.Decrypt(pLocalUser.RSAKeys.PrivateKey, encodedSegmentParts[1]); 
                                break;
                            case "SGP": signaturePlain = Base64Handler.Decode(encodedSegmentParts[1]); break;
                            case "SGC":
                                if (rsaEncoded)
                                    signatureRSA = encodedSegmentParts[1]; 
                                break;
                            case "INS": instruction = Base64Handler.Decode(encodedSegmentParts[1]); break;
                            case "VAL": value = Base64Handler.Decode(encodedSegmentParts[1]); break;
                            case "PAR": 
                            
                                break;
                        }
                    }

                    // Check signature
                    if (rsaEncoded && !RSAHandler.Verify(publicKey, signaturePlain, signatureRSA)) 
                        throw new NetComSignatureException("*** The received packets signature is invalid! ***");

                    if(pServerClientList != null)
                    {
                        // Server -> Assign received data to clientlist

                        pServerClientList[pReceptionSocket].SetUserData(username, password, publicKey);

                        if(!pServerClientList[pReceptionSocket].Authenticate(password))
                            throw new NetComAuthenticationException("*** Authentication failed! Wrong username / password ***");

                        user = pServerClientList[pReceptionSocket];
                    }
                    else
                    {
                        // Client -> Ignore authentication-data

                        user = new NetComUser();
                        user.SetUserData(username, password, publicKey);
                    }

                    if(value == null && parameters.Count == 0) 
                        yield return (InstructionBase)Activator.CreateInstance(Type.GetType(instruction), user);
                    else if(parameters.Count == 0)
                        yield return (InstructionBase)Activator.CreateInstance(Type.GetType(instruction), user, value);
                    else
                        yield return (InstructionBase)Activator.CreateInstance(Type.GetType(instruction), user, value, parameters.ToArray());
                }
            }
        }
    }
}
