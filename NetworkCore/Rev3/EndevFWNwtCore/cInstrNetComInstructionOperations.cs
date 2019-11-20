using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EndevFrameworkNetworkCore
{
    /// <summary>
    /// =====================================   <para />
    /// FRAMEWORK: EndevFrameworkNetworkCore    <para />
    /// SUB-PACKAGE: Instruction-Objects        <para />
    /// =====================================   <para />
    /// DESCRIPTION:                            <para />
    /// Contains operations for manipulating 
    /// and changing instructions.
    /// </summary>
    public class InstructionOperations
    {
        public static bool ForceInstructionsetVersion { get; set; } = false;
        public static bool ForceFrameworkVersion { get; set; } = false;

        /// <summary>
        /// Parses an encoded instruction-string and returns 
        /// all instruction-objects included in the string.
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
                foreach (string encodedInstruction in pInstructionString.Split('%'))
                {
                    if (string.IsNullOrEmpty(encodedInstruction)) continue;

                    string instructionID = null;
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

                    if (encInstr.StartsWith("R:"))
                    {
                        rsaEncoded = true;
                        encInstr = encInstr.Remove(0, 2);
                    }

                    // <Base64>
                    string decodedInstruction;
                    try
                    {
                        //decodedInstruction = Base64Handler.Decode(encInstr);
                        decodedInstruction = encInstr;
                    }
                    catch (Exception ex)
                    {
                        (pLocalUser as NetComOperator).Debug("Instruction-Parsing: Could not decode from Base64.", DebugType.Error);
                        if ((pLocalUser as NetComOperator).ShowExceptions) (pLocalUser as NetComOperator).Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);

                        continue;
                    }


                    // INS:B64,VAL:B64,PAR:B64#B64|B64#B64|,

                    foreach (string encodedInstrSegment in decodedInstruction.Split('$'))
                    {
                        if (string.IsNullOrEmpty(encodedInstrSegment)) continue;

                        // INS:B64 / PAR:B64#B64|B64#B64| 

                        string[] encodedSegmentParts = encodedInstrSegment.Split(':');

                        switch (encodedSegmentParts[0])
                        {
                            case "IID": instructionID = encodedSegmentParts[1]; break;
                            case "FWV": frameworkVersion = encodedSegmentParts[1]; break;
                            case "ISV": instructionsetVersion = encodedSegmentParts[1]; break;
                            case "PUK": publicKey = encodedSegmentParts[1]; break;
                            case "USR": username = Base64Handler.Decode(encodedSegmentParts[1], "ERROR"); break;
                            case "PSW":
                                if (rsaEncoded)
                                    password = RSAHandler.Decrypt(pLocalUser.RSAKeys.PrivateKey, encodedSegmentParts[1]);
                                break;
                            case "SGP": signaturePlain = encodedSegmentParts[1]; break;
                            case "SGC":
                                if (rsaEncoded)
                                    signatureRSA = encodedSegmentParts[1];
                                break;
                            case "INS": instruction = encodedSegmentParts[1]; break;
                            case "VAL": value = Base64Handler.Decode(encodedSegmentParts[1], "ERROR"); break;
                            case "PAR":

                                foreach (string paramGroup in encodedSegmentParts[1].Split('|'))
                                {
                                    if (string.IsNullOrEmpty(paramGroup)) continue;

                                    string[] paramParts = paramGroup.Split('#');

                                    string paramTypeStr = paramParts[0];
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
                                    catch (NotSupportedException ex)
                                    {
                                        (pLocalUser as NetComOperator).Debug("Broadcast-Error.", DebugType.Error);
                                        if ((pLocalUser as NetComOperator).ShowExceptions) (pLocalUser as NetComOperator).Debug($"({ex.GetType().Name}) {ex.Message}", DebugType.Exception);


                                        throw new NetComParsingException($"*** Could not parse the following parameter: Type: {paramTypeStr}, Value: {paramValueStr} ***");
                                    }
                                }

                                break;
                        }
                    }

                    // Check if the instruction has been processed before
                    if (instruction != typeof(InstructionLibraryEssentials.ReceptionConfirmation).AssemblyQualifiedName)
                    {
                        if ((pLocalUser as NetComOperator).InstructionLogIncomming.Contains(instructionID))
                        {
                            if (pLocalUser.GetType() == typeof(NetComClient))
                                (pLocalUser as NetComClient).Send(new InstructionLibraryEssentials.ReceptionConfirmation(pLocalUser, null, instructionID));

                            if (pLocalUser.GetType() == typeof(NetComServer))
                                (pLocalUser as NetComServer).Send(new InstructionLibraryEssentials.ReceptionConfirmation(pLocalUser, (pLocalUser as NetComServer).ConnectedClients[pReceptionSocket], instructionID));

                            continue;
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

                    if (pServerClientList != null)
                    {
                        // Server -> Assign received data to clientlist

                        pServerClientList[pReceptionSocket].SetUserData(username, password, publicKey);

                        if (instruction != typeof(InstructionLibraryEssentials.KeyExchangeClient2Server).AssemblyQualifiedName
                            && !pServerClientList[pReceptionSocket].Authenticate(password))
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

                    // Send confirmation (if the instruction is not a confirmation itself)
                    if (instruction != typeof(InstructionLibraryEssentials.ReceptionConfirmation).AssemblyQualifiedName)
                    {
                        (pLocalUser as NetComOperator).InstructionLogIncomming.Add(instructionID);

                        if (pLocalUser.GetType() == typeof(NetComClient))
                            (pLocalUser as NetComClient).Send(new InstructionLibraryEssentials.ReceptionConfirmation(pLocalUser, null, instructionID));

                        if (pLocalUser.GetType() == typeof(NetComServer))
                            (pLocalUser as NetComServer).Send(new InstructionLibraryEssentials.ReceptionConfirmation(pLocalUser, (pLocalUser as NetComServer).ConnectedClients[pReceptionSocket], instructionID));
                    }
                }
            }
        }
    }
}
