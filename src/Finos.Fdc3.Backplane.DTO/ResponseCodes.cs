namespace Finos.Fdc3.Backplane.DTO
{
    /// <summary>
    /// Response codes 
    /// </summary>
    public enum ResponseCodes
    {
        /// <summary>
        /// Multihost scenario, no member nodes connected
        /// </summary>
        NoMemberNodesConnected = 70000,

        /// <summary>
        /// Broadcast payload invalid
        /// </summary>
        BroadcastPayloadInvalid = 80000,
    }
}
