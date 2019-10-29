using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndevFWNwtCore
{
    /// <summary>
    /// =====================================   <para />
    /// FRAMEWORK: EndevFrameworkNetworkCore    <para />
    /// SUB-PACKAGE: User-Objects               <para />
    /// =====================================   <para />
    /// DESCRIPTION:                            <para />
    /// Stores multiple connected clients 
    /// connected to the server
    /// </summary>
    public class ClientList : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            throw new NetComNotImplementedException();
        }
    }
}
