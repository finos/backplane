namespace Finos.Fdc3.Backplane.Client.Transport
{
    /// <summary>
    /// Connection State of Connection
    /// </summary>
    public enum ConnectionState
    {
        /// <summary>
        /// Disconnect
        /// </summary>
        Disconnected,
        /// <summary>
        /// Connecting
        /// </summary>
        Connecting,
        /// <summary>
        /// Reconnecting
        /// </summary>
        Reconnecting,
        /// <summary>
        /// Connected
        /// </summary>
        Connected
    }
}
