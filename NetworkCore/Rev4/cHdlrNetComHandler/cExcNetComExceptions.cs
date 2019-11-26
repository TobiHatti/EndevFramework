using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndevFramework.NetworkCore
{
    public class NetComExceptions
    {
        // ╔════╤════════════════════════════════════════════════════════╗
        // ║ 5a │ C L A S S E S   ( I N T E R N A L )                    ║
        // ╚════╧════════════════════════════════════════════════════════╝ 

        #region ═╣ C L A S S E S   ( I N T E R N A L ) ╠═

        /// <summary>
        /// =====================================   <para />
        /// FRAMEWORK: EndevFrameworkNetworkCore    <para />
        /// SUB-PACKAGE: Exceptions                 <para />
        /// =====================================   <para />
        /// DESCRIPTION:                            <para />
        /// Provides a basic Runtime-Exception.
        /// </summary>
        internal class NetComException : Exception
        {
            public NetComException() { }
            public NetComException(string message) : base(message) { }
            public NetComException(string message, Exception innerException) : base(message, innerException) { }
        }

        /// <summary>
        /// =====================================   <para />
        /// FRAMEWORK: EndevFrameworkNetworkCore    <para />
        /// SUB-PACKAGE: Exceptions                 <para />
        /// =====================================   <para />
        /// DESCRIPTION:                            <para />
        /// Provides a basic
        /// NotImplemented-Exception.
        /// </summary>
        internal class NetComNotImplementedException : NotImplementedException
        {
            public NetComNotImplementedException() { }
            public NetComNotImplementedException(string message) : base(message) { }
            public NetComNotImplementedException(string message, Exception innerException) : base(message, innerException) { }
        }

        /// <summary>
        /// =====================================   <para />
        /// FRAMEWORK: EndevFrameworkNetworkCore    <para />
        /// SUB-PACKAGE: Exceptions                 <para />
        /// =====================================   <para />
        /// DESCRIPTION:                            <para />
        /// Provides a exception that can be thrown
        /// when a packets signature is invalid
        /// </summary>
        internal class NetComSignatureException : NetComException
        {
            public NetComSignatureException() { }
            public NetComSignatureException(string message) : base(message) { }
            public NetComSignatureException(string message, Exception innerException) : base(message, innerException) { }
        }

        /// <summary>
        /// =====================================   <para />
        /// FRAMEWORK: EndevFrameworkNetworkCore    <para />
        /// SUB-PACKAGE: Exceptions                 <para />
        /// =====================================   <para />
        /// DESCRIPTION:                            <para />
        /// Provides a exception that can be thrown
        /// when a client-authentication fails
        /// </summary>
        internal class NetComAuthenticationException : NetComException
        {
            public NetComAuthenticationException() { }
            public NetComAuthenticationException(string message) : base(message) { }
            public NetComAuthenticationException(string message, Exception innerException) : base(message, innerException) { }
        }

        /// <summary>
        /// =====================================   <para />
        /// FRAMEWORK: EndevFrameworkNetworkCore    <para />
        /// SUB-PACKAGE: Exceptions                 <para />
        /// =====================================   <para />
        /// DESCRIPTION:                            <para />
        /// Provides a exception that can be thrown
        /// when a parsing error occures
        /// </summary>
        internal class NetComParsingException : NetComException
        {
            public NetComParsingException() { }
            public NetComParsingException(string message) : base(message) { }
            public NetComParsingException(string message, Exception innerException) : base(message, innerException) { }
        }

        /// <summary>
        /// =====================================   <para />
        /// FRAMEWORK: EndevFrameworkNetworkCore    <para />
        /// SUB-PACKAGE: Exceptions                 <para />
        /// =====================================   <para />
        /// DESCRIPTION:                            <para />
        /// Provides a exception that can be thrown
        /// when version error or violation occures
        /// </summary>
        internal class NetComVersionException : NetComException
        {
            public NetComVersionException() { }
            public NetComVersionException(string message) : base(message) { }
            public NetComVersionException(string message, Exception innerException) : base(message, innerException) { }
        }

        #endregion
    }
}
