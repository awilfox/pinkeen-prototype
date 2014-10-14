using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;

namespace DCPProject.ProtoLib
{
    /// <summary>
    /// Establishes and maintains a connection to the DCP network.
    /// </summary>
    public abstract class Connection
    {
        /// <summary>
        /// Internal storage for the user's handle.
        /// </summary>
        protected string handle;

        /// <summary>
        /// Internal storage for the current client name.
        /// </summary>
        protected string clientName;

        /// <summary>
        /// Set this to the name and version of your client in your subclass'
        /// constructor to ensure proper handling of connection requests.
        /// </summary>
        protected string clientVersion;

        /// <summary>
        /// Delegate for message-related events.
        /// </summary>
        /// <param name="message">The message causing the event.</param>
        public delegate void MessageHandler(Message message);

        /// <summary>
        /// Signals an incoming message.
        /// </summary>
        public event MessageHandler IncomingMessage;

        /// <summary>
        /// Signals an outgoing message.
        /// </summary>
        public event MessageHandler OutgoingMessage;

        /// <summary>
        /// Connect to the specified <paramref name="server"/> on
        /// <paramref name="port"/>.
        /// </summary>
        /// <param name="server">The DNS name or IP address of the server to
        /// connect to.</param>
        /// <param name="port">The TCP port number to connect to.</param>
        /// <param name="register">Attempt to register the handle as a new
        /// account.  This method will disconnect automatically if registration
        /// fails.</param>
        /// <returns>true if a connection was established successfully; false
        /// otherwise.</returns>
        public abstract bool ConnectTo(string server, short port = 7266,
            bool register = false);

        /// <summary>
        /// Send a message over an active connection.
        /// </summary>
        /// <remarks>You cannot send a message over a closed connection.
        /// Attempts to do this wiil fail.</remarks>
        /// <param name="message">The message to send.</param>
        /// <returns>true if the message was sent to the server successfully;
        /// false otherwise.</returns>
        public abstract bool Send(Message message);

        /// <summary>
        /// Disconnect from the DCP server.
        /// </summary>
        public abstract void Disconnect();

        /// <summary>
        /// The user's handle/username.
        /// </summary>
        public string Handle
        {
            get { return handle; }
        }

        /// <summary>
        /// The name of this client.
        /// </summary>
        public string ClientName
        {
            get { return ClientName; }
        }
    }
}
