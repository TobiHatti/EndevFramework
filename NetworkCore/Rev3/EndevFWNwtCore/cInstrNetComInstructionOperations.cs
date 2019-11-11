using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

// Segment Complete [Last Modified 31.10.2019]

namespace EndevFWNwtCore
{
    /// <summary>
    /// =====================================   <para />
    /// FRAMEWORK: EndevFrameworkNetworkCore    <para />
    /// SUB-PACKAGE: Instruction-Objects        <para />
    /// =====================================   <para />
    /// DESCRIPTION:                            <para />
    /// Contains operations for manpulating 
    /// and changing instructions.
    /// </summary>
    public class InstructionOperations
    {
        public static bool ForceInstructionsetVersion { get; set; } = false;
        public static bool ForceFrameworkVersion { get; set; } = false;

        /// <summary>
        /// Parses an encoded instruction-string and returns 
        /// all instruction-objects included in the string
        /// </summary>
        /// <param name="pLocalUser">Local user</param>
        /// <param name="pReceptionSocket">Remote user, from which the instruction was received</param>
        /// <param name="pInstructionString">Encoded instruction-string. Can contain multiple instructions.</param>
        /// <param name="pServerClientList">Client-List for authentication (NetComServer only!)</param>
        /// <returns></returns>
        public static IEnumerable<InstructionBase> Parse(NetComUser pLocalUser, Socket pReceptionSocket, string pInstructionString, ClientList pServerClientList = null)
        {
            //  RSA:<Base64>;RSA:<Base64>;

            lock (pLocalUser)
            {
                foreach (string encodedInstruction in pInstructionString.Split(';'))
                {
                    if (string.IsNullOrEmpty(encodedInstruction)) continue;

                    string frameworkVersion = null;
                    string instructionsetVersion = null;
                    string username = null;
                    string password = null;
                    string instruction = null;
                    string value = null;
                    List<object> parameters = new List<object>();
                    string signaturePlain = null;
                    string signatureRSA = null;
                    string publicKey = null;
                    NetComUser user = null;

                    // RSA:<Base64>

                    bool rsaEncoded = false;

                    string encInstr = encodedInstruction;

                    if(encInstr.StartsWith("RSA:"))
                    {
                        rsaEncoded = true;
                        encInstr = encInstr.Remove(0, 4);
                    }

                    // <Base64>
                    string decodedInstruction;
                    try
                    {
                        decodedInstruction = Base64Handler.Decode(encInstr);
                    }
                    catch
                    {
                        Console.WriteLine("Could not parse instruction!");
                        continue;
                    }
                      

                    // INS:B64,VAL:B64,PAR:B64#B64|B64#B64|,

                    foreach (string encodedInstrSegment in decodedInstruction.Split(','))
                    {
                        if (string.IsNullOrEmpty(encodedInstrSegment)) continue;

                        // INS:B64 / PAR:B64#B64|B64#B64| 

                        string[] encodedSegmentParts = encodedInstrSegment.Split(':');

                        switch(encodedSegmentParts[0])
                        {
                            case "FWV": frameworkVersion = Base64Handler.Decode(encodedSegmentParts[1], "ERROR"); break;
                            case "ISV": instructionsetVersion = Base64Handler.Decode(encodedSegmentParts[1], "ERROR"); break;
                            case "PUK": publicKey = Base64Handler.Decode(encodedSegmentParts[1], "ERROR"); break;
                            case "USR": username = Base64Handler.Decode(encodedSegmentParts[1], "ERROR"); break;
                            case "PSW": 
                                if(rsaEncoded)
                                    password = RSAHandler.Decrypt(pLocalUser.RSAKeys.PrivateKey, encodedSegmentParts[1]); 
                                break;
                            case "SGP": signaturePlain = Base64Handler.Decode(encodedSegmentParts[1], "ERROR"); break;
                            case "SGC":
                                if (rsaEncoded)
                                    signatureRSA = encodedSegmentParts[1]; 
                                break;
                            case "INS": instruction = Base64Handler.Decode(encodedSegmentParts[1], "ERROR"); break;
                            case "VAL": value = Base64Handler.Decode(encodedSegmentParts[1], "ERROR"); break;
                            case "PAR": 
                                    
                                foreach(string paramGroup in encodedSegmentParts[1].Split('|'))
                                {
                                    if (string.IsNullOrEmpty(paramGroup)) continue;

                                    string[] paramParts = paramGroup.Split('#');

                                    string paramTypeStr = Base64Handler.Decode(paramParts[0], "ERROR");
                                    string paramValueStr = Base64Handler.Decode(paramParts[1], "ERROR");

                                    Type paramType = Type.GetType(paramTypeStr);

                                    try
                                    {
                                        object convParam = null;
                                        TypeConverter converter = TypeDescriptor.GetConverter(paramType);
                                        if (converter != null)
                                        {
                                            convParam = converter.ConvertFromString(paramValueStr);
                                        }
                                        else convParam = null;

                                        parameters.Add(convParam);

                                    }
                                    catch (NotSupportedException)
                                    {
                                        throw new NetComParsingException($"*** Could not parse the following parameter: Type: {paramTypeStr}, Value: {paramValueStr} ***");
                                    }
                                }

                                break;
                        }
                    }
                    
                    // Check instructionset-version
                    if (ForceInstructionsetVersion && InstructionBase.InstructionSetVersion != instructionsetVersion)
                        throw new NetComVersionException($"*** The received package uses a different verion of the Instruction-Set! Local version: {InstructionBase.InstructionSetVersion} - Senders version: {instructionsetVersion} ***");

                    // Check framework-version
                    if (ForceFrameworkVersion && InstructionBase.FrameworkVersion != frameworkVersion)
                        throw new NetComVersionException($"*** The received package was built using a different verion of the Framework! Local version: {InstructionBase.FrameworkVersion} - Senders version: {frameworkVersion} ***");

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

                    if (value == null && parameters.Count == 0)
                        yield return (InstructionBase)Activator.CreateInstance(Type.GetType(instruction), user, pLocalUser);
                    else if (parameters.Count == 0)
                        yield return (InstructionBase)Activator.CreateInstance(Type.GetType(instruction), user, pLocalUser, value);
                    else
                        yield return (InstructionBase)Activator.CreateInstance(Type.GetType(instruction), user, pLocalUser, value, parameters.ToArray());
                }
            }
        }
    }
}
