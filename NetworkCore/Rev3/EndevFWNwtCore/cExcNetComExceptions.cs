using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Segment Complete [Last Modified 29.10.2019]

namespace EndevFWNwtCore
{
    /// <summary>
    /// =====================================   <para />
    /// FRAMEWORK: EndevFrameworkNetworkCore    <para />
    /// SUB-PACKAGE: Exceptions                 <para />
    /// =====================================   <para />
    /// DESCRIPTION:                            <para />
    /// Provides a basic Runtime-Exception.
    /// </summary>
    public class NetComException : Exception
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
    public class NetComNotImplementedException : NotImplementedException
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
    public class NetComSignatureException : NetComException
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
    public class NetComAuthenticationException : NetComException
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
    public class NetComParsingException : NetComException
    {
        public NetComParsingException() { }
        public NetComParsingException(string message) : base(message) { }
        public NetComParsingException(string message, Exception innerException) : base(message, innerException) { }
    }
}
