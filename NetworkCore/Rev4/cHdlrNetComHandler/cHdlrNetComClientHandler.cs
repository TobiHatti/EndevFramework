﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndevFramework.NetworkCore
{
    public class ClientHandler : NetComHandler
    {
        protected override void NetComOperatorExecutor()
        {
            ncOperator = new NetComClient(this, handlerData);
            ncOperator.Start();
        }

        /*
        public NetComClient(string pServerIP, int pPort)
            : this(IPAddress.Parse(pServerIP), pPort) { }

        /// <summary>
        /// Creates a new NetCom-client instance.
        /// </summary>
        /// <param name="pServerIP">IP of the server</param>
        /// <param name="pPort">TCP port of the server</param>
        public NetComClient(IPAddress pServerIP, int pPort)
        {
            RSAKeys = RSAHandler.GenerateKeyPair();
            port = pPort;
            serverIP = pServerIP;
        }

        */


        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 1a │ F I E L D S   ( P R I V A T E )                        ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝    

        #region ═╣ F I E L D S   ( P R I V A T E ) ╠═ 
        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 1b │ F I E L D S   ( P R O T E C T E D )                    ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝    

        #region ═╣ F I E L D S   ( P R O T E C T E D ) ╠═ 
        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 1c │ D E L E G A T E S                                      ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝    

        #region ═╣ D E L E G A T E S ╠═ 
        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 2a │ P R O P E R T I E S   ( I N T E R N A L )              ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝  

        #region ═╣ P R O P E R T I E S   ( I N T E R N A L ) ╠═ 
        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 2b │ P R O P E R T I E S   ( P U B L I C )                  ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝  

        #region ═╣ P R O P E R T I E S   ( P U B L I C ) ╠═ 
        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 3  │ C O N S T R U C T O R S                                ║
        // ╚════╧════════════════════════════════════════════════════════╝  

        #region ═╣ C O N S T R U C T O R S ╠═ 
        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 4a │ M E T H O D S   ( P R I V A T E )                      ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝  

        #region ═╣ M E T H O D S   ( P R I V A T E ) ╠═ 
        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 4b │ M E T H O D S   ( I N T E R N A L )                    ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝ 

        #region ═╣ M E T H O D S   ( I N T E R N A L ) ╠═ 
        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 4c │ M E T H O D S   ( P R O T E C T E D )                  ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝ 

        #region ═╣ M E T H O D S   ( P R O T E C T E D ) ╠═ 
        #endregion

        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 4d │ M E T H O D S   ( P U B L I C )                        ║
        // ╟────┴────────────────────────────────────────────────────────╢ 
        // ║ N O N - S T A T I C   &   S T A T I C                       ║ 
        // ╚═════════════════════════════════════════════════════════════╝ 

        #region ═╣ M E T H O D S   ( P U B L I C ) ╠═ 
        #endregion

    }
}
