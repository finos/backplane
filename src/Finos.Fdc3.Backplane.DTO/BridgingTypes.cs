using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Finos.Fdc3.Backplane.DTO
{
    /// <summary>
    /// Metadata relating to the FDC3 Desktop Agent implementation and its provider.
    /// </summary>
    public partial class BaseImplementationMetadata
    {
        /// <summary>
        /// The version number of the FDC3 specification that the implementation provides.
        /// The string must be a numeric semver version, e.g. 1.2 or 1.2.1.
        /// </summary>
        [JsonProperty("fdc3Version")]
        public string Fdc3Version { get; set; }

        /// <summary>
        /// Metadata indicating whether the Desktop Agent implements optional features of
        /// the Desktop Agent API.
        /// </summary>
        [JsonProperty("optionalFeatures")]
        public BaseImplementationMetadataOptionalFeatures OptionalFeatures { get; set; }

        /// <summary>
        /// The name of the provider of the Desktop Agent implementation (e.g. Finsemble, Glue42,
        /// OpenFin etc.).
        /// </summary>
        [JsonProperty("provider")]
        public string Provider { get; set; }

        /// <summary>
        /// The version of the provider of the Desktop Agent implementation (e.g. 5.3.0).
        /// </summary>
        [JsonProperty("providerVersion", NullValueHandling = NullValueHandling.Ignore)]
        public string ProviderVersion { get; set; }
    }

    /// <summary>
    /// Metadata indicating whether the Desktop Agent implements optional features of
    /// the Desktop Agent API.
    /// </summary>
    public partial class BaseImplementationMetadataOptionalFeatures
    {
        /// <summary>
        /// Used to indicate whether the experimental Desktop Agent Bridging
        /// feature is implemented by the Desktop Agent.
        /// </summary>
        [JsonProperty("DesktopAgentBridging")]
        public bool DesktopAgentBridging { get; set; }

        /// <summary>
        /// Used to indicate whether the exposure of 'originating app metadata' for
        /// context and intent messages is supported by the Desktop Agent.
        /// </summary>
        [JsonProperty("OriginatingAppMetadata")]
        public bool OriginatingAppMetadata { get; set; }

        /// <summary>
        /// Used to indicate whether the optional `fdc3.joinUserChannel`,
        /// `fdc3.getCurrentChannel` and `fdc3.leaveCurrentChannel` are implemented by
        /// the Desktop Agent.
        /// </summary>
        [JsonProperty("UserChannelMembershipAPIs")]
        public bool UserChannelMembershipApIs { get; set; }
    }

    /// <summary>
    /// A response message from a Desktop Agent to the Bridge containing an error, to be used in
    /// preference to the standard response when an error needs to be returned.
    /// </summary>
    public partial class AgentErrorResponseMessage
    {
        [JsonProperty("meta")]
        public AgentResponseMetadata Meta { get; set; }

        /// <summary>
        /// Error message payload containing an standardized error string.
        /// </summary>
        [JsonProperty("payload")]
        public ErrorResponseMessagePayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Response' appended.
        /// </summary>
        [JsonProperty("type")]
        public ResponseMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for a response messages sent by a Desktop Agent to the Bridge
    /// </summary>
    public partial class AgentResponseMetadata
    {
        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// Error message payload containing an standardized error string.
    /// </summary>
    public partial class ErrorResponseMessagePayload
    {
        [JsonProperty("error")]
        public ResponseErrorDetail Error { get; set; }
    }

    /// <summary>
    /// A request message from a Desktop Agent to the Bridge.
    /// </summary>
    public partial class AgentRequestMessage
    {
        [JsonProperty("meta")]
        public AgentRequestMetadata Meta { get; set; }

        /// <summary>
        /// The message payload typically contains the arguments to FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public Dictionary<string, object> Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Request' appended.
        /// </summary>
        [JsonProperty("type")]
        public RequestMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for a request message sent by Desktop Agents to the Bridge.
    /// </summary>
    public partial class AgentRequestMetadata
    {
        /// <summary>
        /// Optional field that represents the destination that the request should be routed to. Must
        /// be set by the Desktop Agent for API calls that include a target app parameter and must
        /// include the name of the Desktop Agent hosting the target application.
        /// </summary>
        [JsonProperty("destination", NullValueHandling = NullValueHandling.Ignore)]
        public BridgeParticipantIdentifier Destination { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        /// <summary>
        /// Field that represents the source application that the request was received from, or the
        /// source Desktop Agent if it issued the request itself.
        /// </summary>
        [JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
        public SourceIdentifier Source { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// Optional field that represents the destination that the request should be routed to. Must
    /// be set by the Desktop Agent for API calls that include a target app parameter and must
    /// include the name of the Desktop Agent hosting the target application.
    ///
    /// Represents identifiers that MUST include the Desktop Agent name and MAY identify a
    /// specific app or instance.
    ///
    /// Field that represents the source application that the request was received from, or the
    /// source Desktop Agent if it issued the request itself. The Desktop Agent identifier MUST
    /// be set by the bridge.
    ///
    /// Identifies a particular Desktop Agent in Desktop Agent Bridging scenarios
    /// where a request needs to be directed to a Desktop Agent rather than a specific app, or a
    /// response message is returned by the Desktop Agent (or more specifically its resolver)
    /// rather than a specific app. Used as a substitute for `AppIdentifier` in cases where no
    /// app details are available or are appropriate.
    ///
    /// Array of DesktopAgentIdentifiers for responses that were not returned to the bridge
    /// before the timeout or because an error occurred. May be omitted if all sources responded
    /// without errors. MUST include the `desktopAgent` field when returned by the bridge.
    ///
    /// Array of DesktopAgentIdentifiers for the sources that generated responses to the request.
    /// Will contain a single value for individual responses and multiple values for responses
    /// that were collated by the bridge. May be omitted if all sources errored. MUST include the
    /// `desktopAgent` field when returned by the bridge.
    ///
    /// Field that represents a destination Desktop Agent that a request is to be sent to.
    ///
    /// Field that represents a destination App on a remote Desktop Agent that a request is to be
    /// sent to.
    ///
    /// Identifies an application, or instance of an application, and is used to target FDC3 API
    /// calls, such as `fdc3.open` or `fdc3.raiseIntent` at specific applications or application
    /// instances.
    ///
    /// Will always include at least an `appId` field, which uniquely identifies a specific app.
    ///
    /// If the `instanceId` field is set then the `AppMetadata` object represents a specific
    /// instance of the application that may be addressed using that Id.
    ///
    /// Field that represents the source application that a request or response was received
    /// from.
    ///
    /// Identifier for the app instance that was selected (or started) to resolve the intent.
    /// `source.instanceId` MUST be set, indicating the specific app instance that
    /// received the intent.
    /// </summary>
    public partial class BridgeParticipantIdentifier
    {
        /// <summary>
        /// Used in Desktop Agent Bridging to attribute or target a message to a
        /// particular Desktop Agent.
        ///
        /// The Desktop Agent that the app is available on. Used in Desktop Agent Bridging to
        /// identify the Desktop Agent to target.
        /// </summary>
        [JsonProperty("desktopAgent")]
        public string DesktopAgent { get; set; }

        /// <summary>
        /// The unique application identifier located within a specific application directory
        /// instance. An example of an appId might be 'app@sub.root'
        /// </summary>
        [JsonProperty("appId", NullValueHandling = NullValueHandling.Ignore)]
        public string AppId { get; set; }

        /// <summary>
        /// An optional instance identifier, indicating that this object represents a specific
        /// instance of the application described.
        /// </summary>
        [JsonProperty("instanceId", NullValueHandling = NullValueHandling.Ignore)]
        public string InstanceId { get; set; }
    }

    /// <summary>
    /// Field that represents the source application that the request was received from, or the
    /// source Desktop Agent if it issued the request itself.
    ///
    /// Field that represents the source application that a request or response was received
    /// from, or the source Desktop Agent if it issued the request or response itself.
    ///
    /// Identifies an application, or instance of an application, and is used to target FDC3 API
    /// calls, such as `fdc3.open` or `fdc3.raiseIntent` at specific applications or application
    /// instances.
    ///
    /// Will always include at least an `appId` field, which uniquely identifies a specific app.
    ///
    /// If the `instanceId` field is set then the `AppMetadata` object represents a specific
    /// instance of the application that may be addressed using that Id.
    ///
    /// Field that represents the source application that a request or response was received
    /// from.
    ///
    /// Identifier for the app instance that was selected (or started) to resolve the intent.
    /// `source.instanceId` MUST be set, indicating the specific app instance that
    /// received the intent.
    ///
    /// Identifies a particular Desktop Agent in Desktop Agent Bridging scenarios
    /// where a request needs to be directed to a Desktop Agent rather than a specific app, or a
    /// response message is returned by the Desktop Agent (or more specifically its resolver)
    /// rather than a specific app. Used as a substitute for `AppIdentifier` in cases where no
    /// app details are available or are appropriate.
    ///
    /// Array of DesktopAgentIdentifiers for responses that were not returned to the bridge
    /// before the timeout or because an error occurred. May be omitted if all sources responded
    /// without errors. MUST include the `desktopAgent` field when returned by the bridge.
    ///
    /// Array of DesktopAgentIdentifiers for the sources that generated responses to the request.
    /// Will contain a single value for individual responses and multiple values for responses
    /// that were collated by the bridge. May be omitted if all sources errored. MUST include the
    /// `desktopAgent` field when returned by the bridge.
    ///
    /// Field that represents a destination Desktop Agent that a request is to be sent to.
    /// </summary>
    public partial class SourceIdentifier
    {
        /// <summary>
        /// The unique application identifier located within a specific application directory
        /// instance. An example of an appId might be 'app@sub.root'
        /// </summary>
        [JsonProperty("appId", NullValueHandling = NullValueHandling.Ignore)]
        public string AppId { get; set; }

        /// <summary>
        /// The Desktop Agent that the app is available on. Used in Desktop Agent Bridging to
        /// identify the Desktop Agent to target.
        ///
        /// Used in Desktop Agent Bridging to attribute or target a message to a
        /// particular Desktop Agent.
        /// </summary>
        [JsonProperty("desktopAgent", NullValueHandling = NullValueHandling.Ignore)]
        public string DesktopAgent { get; set; }

        /// <summary>
        /// An optional instance identifier, indicating that this object represents a specific
        /// instance of the application described.
        /// </summary>
        [JsonProperty("instanceId", NullValueHandling = NullValueHandling.Ignore)]
        public string InstanceId { get; set; }
    }

    /// <summary>
    /// A response message from a Desktop Agent to the Bridge.
    /// </summary>
    public partial class AgentResponseMessage
    {
        [JsonProperty("meta")]
        public AgentResponseMetadata Meta { get; set; }

        /// <summary>
        /// The message payload typically contains return values for FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public Dictionary<string, object> Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Response' appended.
        /// </summary>
        [JsonProperty("type")]
        public ResponseMessageType Type { get; set; }
    }

    /// <summary>
    /// A response message from the Bridge back to the original Desktop Agent that raised the
    /// request, used where all connected agents returned errors.
    /// </summary>
    public partial class BridgeErrorResponseMessage
    {
        [JsonProperty("meta")]
        public BridgeErrorResponseMessageMeta Meta { get; set; }

        /// <summary>
        /// The error message payload contains details of an error return to the app or agent that
        /// raised the original request.
        /// </summary>
        [JsonProperty("payload")]
        public ResponseErrorMessagePayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Response' appended.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Metadata required in a response message collated and/or forwarded on by the Bridge
    /// </summary>
    public partial class BridgeErrorResponseMessageMeta
    {
        [JsonProperty("errorDetails")]
        public ResponseErrorDetail[] ErrorDetails { get; set; }

        [JsonProperty("errorSources")]
        public DesktopAgentIdentifier[] ErrorSources { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// Identifies a particular Desktop Agent in Desktop Agent Bridging scenarios
    /// where a request needs to be directed to a Desktop Agent rather than a specific app, or a
    /// response message is returned by the Desktop Agent (or more specifically its resolver)
    /// rather than a specific app. Used as a substitute for `AppIdentifier` in cases where no
    /// app details are available or are appropriate.
    ///
    /// Array of DesktopAgentIdentifiers for responses that were not returned to the bridge
    /// before the timeout or because an error occurred. May be omitted if all sources responded
    /// without errors. MUST include the `desktopAgent` field when returned by the bridge.
    ///
    /// Array of DesktopAgentIdentifiers for the sources that generated responses to the request.
    /// Will contain a single value for individual responses and multiple values for responses
    /// that were collated by the bridge. May be omitted if all sources errored. MUST include the
    /// `desktopAgent` field when returned by the bridge.
    ///
    /// Field that represents a destination Desktop Agent that a request is to be sent to.
    /// </summary>
    public partial class DesktopAgentIdentifier
    {
        /// <summary>
        /// Used in Desktop Agent Bridging to attribute or target a message to a
        /// particular Desktop Agent.
        /// </summary>
        [JsonProperty("desktopAgent")]
        public string DesktopAgent { get; set; }
    }

    /// <summary>
    /// The error message payload contains details of an error return to the app or agent that
    /// raised the original request.
    /// </summary>
    public partial class ResponseErrorMessagePayload
    {
        [JsonProperty("error", NullValueHandling = NullValueHandling.Ignore)]
        public ResponseErrorDetail? Error { get; set; }
    }

    /// <summary>
    /// A request message forwarded from the Bridge onto a Desktop Agent connected to it.
    /// </summary>
    public partial class BridgeRequestMessage
    {
        [JsonProperty("meta")]
        public BridgeRequestMetadata Meta { get; set; }

        /// <summary>
        /// The message payload typically contains the arguments to FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public Dictionary<string, object> Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Request' appended.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Metadata required in a request message forwarded on by the Bridge
    /// </summary>
    public partial class BridgeRequestMetadata
    {
        /// <summary>
        /// Optional field that represents the destination that the request should be routed to. Must
        /// be set by the Desktop Agent for API calls that include a target app parameter and must
        /// include the name of the Desktop Agent hosting the target application.
        /// </summary>
        [JsonProperty("destination", NullValueHandling = NullValueHandling.Ignore)]
        public BridgeParticipantIdentifier Destination { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        /// <summary>
        /// Field that represents the source application that the request was received from, or the
        /// source Desktop Agent if it issued the request itself. The Desktop Agent identifier MUST
        /// be set by the bridge.
        /// </summary>
        [JsonProperty("source")]
        public BridgeParticipantIdentifier Source { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// A response message from the Bridge back to the original Desktop Agent that raised the
    /// request.
    /// </summary>
    public partial class BridgeResponseMessage
    {
        [JsonProperty("meta")]
        public BridgeResponseMessageMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains return values for FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public Dictionary<string, object> Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Response' appended.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Metadata required in a response message collated and/or forwarded on by the Bridge
    /// </summary>
    public partial class BridgeResponseMessageMeta
    {
        [JsonProperty("errorDetails", NullValueHandling = NullValueHandling.Ignore)]
        public ResponseErrorDetail[] ErrorDetails { get; set; }

        [JsonProperty("errorSources", NullValueHandling = NullValueHandling.Ignore)]
        public DesktopAgentIdentifier[] ErrorSources { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("sources", NullValueHandling = NullValueHandling.Ignore)]
        public DesktopAgentIdentifier[] Sources { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// A request to broadcast context on a channel.
    ///
    /// A request message from a Desktop Agent to the Bridge.
    /// </summary>
    public partial class BroadcastAgentRequest
    {
        [JsonProperty("meta")]
        public BroadcastAgentRequestMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains the arguments to FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public BroadcastAgentRequestPayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Request' appended.
        /// </summary>
        [JsonProperty("type")]
        public RequestMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for a request message sent by Desktop Agents to the Bridge.
    /// </summary>
    public partial class BroadcastAgentRequestMeta
    {
        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        /// <summary>
        /// Field that represents the source application that the request was received from, or the
        /// source Desktop Agent if it issued the request itself.
        /// </summary>
        [JsonProperty("source")]
        public SourceClass Source { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// Identifies an application, or instance of an application, and is used to target FDC3 API
    /// calls, such as `fdc3.open` or `fdc3.raiseIntent` at specific applications or application
    /// instances.
    ///
    /// Will always include at least an `appId` field, which uniquely identifies a specific app.
    ///
    /// If the `instanceId` field is set then the `AppMetadata` object represents a specific
    /// instance of the application that may be addressed using that Id.
    ///
    /// Field that represents the source application that a request or response was received
    /// from.
    ///
    /// Identifier for the app instance that was selected (or started) to resolve the intent.
    /// `source.instanceId` MUST be set, indicating the specific app instance that
    /// received the intent.
    ///
    /// Field that represents the source application that the request was received from, or the
    /// source Desktop Agent if it issued the request itself.
    ///
    /// Field that represents the source application that a request or response was received
    /// from, or the source Desktop Agent if it issued the request or response itself.
    ///
    /// Identifies a particular Desktop Agent in Desktop Agent Bridging scenarios
    /// where a request needs to be directed to a Desktop Agent rather than a specific app, or a
    /// response message is returned by the Desktop Agent (or more specifically its resolver)
    /// rather than a specific app. Used as a substitute for `AppIdentifier` in cases where no
    /// app details are available or are appropriate.
    ///
    /// Array of DesktopAgentIdentifiers for responses that were not returned to the bridge
    /// before the timeout or because an error occurred. May be omitted if all sources responded
    /// without errors. MUST include the `desktopAgent` field when returned by the bridge.
    ///
    /// Array of DesktopAgentIdentifiers for the sources that generated responses to the request.
    /// Will contain a single value for individual responses and multiple values for responses
    /// that were collated by the bridge. May be omitted if all sources errored. MUST include the
    /// `desktopAgent` field when returned by the bridge.
    ///
    /// Field that represents a destination Desktop Agent that a request is to be sent to.
    /// </summary>
    public partial class SourceClass
    {
        /// <summary>
        /// The unique application identifier located within a specific application directory
        /// instance. An example of an appId might be 'app@sub.root'
        /// </summary>
        [JsonProperty("appId")]
        public string AppId { get; set; }

        /// <summary>
        /// The Desktop Agent that the app is available on. Used in Desktop Agent Bridging to
        /// identify the Desktop Agent to target.
        ///
        /// Used in Desktop Agent Bridging to attribute or target a message to a
        /// particular Desktop Agent.
        /// </summary>
        [JsonProperty("desktopAgent", NullValueHandling = NullValueHandling.Ignore)]
        public string DesktopAgent { get; set; }

        /// <summary>
        /// An optional instance identifier, indicating that this object represents a specific
        /// instance of the application described.
        /// </summary>
        [JsonProperty("instanceId", NullValueHandling = NullValueHandling.Ignore)]
        public string InstanceId { get; set; }
    }

    /// <summary>
    /// The message payload typically contains the arguments to FDC3 API functions.
    /// </summary>
    public partial class BroadcastAgentRequestPayload
    {
        /// <summary>
        /// The Id of the Channel that the broadcast was sent on
        /// </summary>
        [JsonProperty("channelId")]
        public string ChannelId { get; set; }

        /// <summary>
        /// The context object that was the payload of a broadcast message.
        /// </summary>
        [JsonProperty("context")]
        public ContextElement Context { get; set; }
    }

    /// <summary>
    /// The context object that was the payload of a broadcast message.
    ///
    /// The `fdc3.context` type defines the basic contract or "shape" for all data exchanged by
    /// FDC3 operations. As such, it is not really meant to be used on its own, but is imported
    /// by more specific type definitions (standardized or custom) to provide the structure and
    /// properties shared by all FDC3 context data types.
    ///
    /// The key element of FDC3 context types is their mandatory `type` property, which is used
    /// to identify what type of data the object represents, and what shape it has.
    ///
    /// The FDC3 context type, and all derived types, define the minimum set of fields a context
    /// data object of a particular type can be expected to have, but this can always be extended
    /// with custom fields as appropriate.
    /// </summary>
    public partial class ContextElement
    {
        /// <summary>
        /// Context data objects may include a set of equivalent key-value pairs that can be used to
        /// help applications identify and look up the context type they receive in their own domain.
        /// The idea behind this design is that applications can provide as many equivalent
        /// identifiers to a target application as possible, e.g. an instrument may be represented by
        /// an ISIN, CUSIP or Bloomberg identifier.
        ///
        /// Identifiers do not make sense for all types of data, so the `id` property is therefore
        /// optional, but some derived types may choose to require at least one identifier.
        /// </summary>
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Id { get; set; }

        /// <summary>
        /// Context data objects may include a name property that can be used for more information,
        /// or display purposes. Some derived types may require the name object as mandatory,
        /// depending on use case.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// The type property is the only _required_ part of the FDC3 context data schema. The FDC3
        /// [API](https://fdc3.finos.org/docs/api/spec) relies on the `type` property being present
        /// to route shared context data appropriately.
        ///
        /// FDC3 [Intents](https://fdc3.finos.org/docs/intents/spec) also register the context data
        /// types they support in an FDC3 [App
        /// Directory](https://fdc3.finos.org/docs/app-directory/overview), used for intent discovery
        /// and routing.
        ///
        /// Standardized FDC3 context types have well-known `type` properties prefixed with the
        /// `fdc3` namespace, e.g. `fdc3.instrument`. For non-standard types, e.g. those defined and
        /// used by a particular organization, the convention is to prefix them with an
        /// organization-specific namespace, e.g. `blackrock.fund`.
        ///
        /// See the [Context Data Specification](https://fdc3.finos.org/docs/context/spec) for more
        /// information about context data types.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// A request to broadcast context on a channel.
    ///
    /// A request message forwarded from the Bridge onto a Desktop Agent connected to it.
    /// </summary>
    public partial class BroadcastBridgeRequest
    {
        [JsonProperty("meta")]
        public BroadcastBridgeRequestMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains the arguments to FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public BroadcastBridgeRequestPayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Request' appended.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Metadata required in a request message forwarded on by the Bridge
    /// </summary>
    public partial class BroadcastBridgeRequestMeta
    {
        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        /// <summary>
        /// Field that represents the source application that the request was received from, or the
        /// source Desktop Agent if it issued the request itself. The Desktop Agent identifier MUST
        /// be set by the bridge.
        /// </summary>
        [JsonProperty("source")]
        public MetaSource Source { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// Identifies an application, or instance of an application, and is used to target FDC3 API
    /// calls, such as `fdc3.open` or `fdc3.raiseIntent` at specific applications or application
    /// instances.
    ///
    /// Will always include at least an `appId` field, which uniquely identifies a specific app.
    ///
    /// If the `instanceId` field is set then the `AppMetadata` object represents a specific
    /// instance of the application that may be addressed using that Id.
    ///
    /// Field that represents the source application that a request or response was received
    /// from.
    ///
    /// Identifier for the app instance that was selected (or started) to resolve the intent.
    /// `source.instanceId` MUST be set, indicating the specific app instance that
    /// received the intent.
    ///
    /// Optional field that represents the destination that the request should be routed to. Must
    /// be set by the Desktop Agent for API calls that include a target app parameter and must
    /// include the name of the Desktop Agent hosting the target application.
    ///
    /// Represents identifiers that MUST include the Desktop Agent name and MAY identify a
    /// specific app or instance.
    ///
    /// Field that represents the source application that the request was received from, or the
    /// source Desktop Agent if it issued the request itself. The Desktop Agent identifier MUST
    /// be set by the bridge.
    ///
    /// Identifies a particular Desktop Agent in Desktop Agent Bridging scenarios
    /// where a request needs to be directed to a Desktop Agent rather than a specific app, or a
    /// response message is returned by the Desktop Agent (or more specifically its resolver)
    /// rather than a specific app. Used as a substitute for `AppIdentifier` in cases where no
    /// app details are available or are appropriate.
    ///
    /// Array of DesktopAgentIdentifiers for responses that were not returned to the bridge
    /// before the timeout or because an error occurred. May be omitted if all sources responded
    /// without errors. MUST include the `desktopAgent` field when returned by the bridge.
    ///
    /// Array of DesktopAgentIdentifiers for the sources that generated responses to the request.
    /// Will contain a single value for individual responses and multiple values for responses
    /// that were collated by the bridge. May be omitted if all sources errored. MUST include the
    /// `desktopAgent` field when returned by the bridge.
    ///
    /// Field that represents a destination Desktop Agent that a request is to be sent to.
    ///
    /// Field that represents a destination App on a remote Desktop Agent that a request is to be
    /// sent to.
    /// </summary>
    public partial class MetaSource
    {
        /// <summary>
        /// The unique application identifier located within a specific application directory
        /// instance. An example of an appId might be 'app@sub.root'
        /// </summary>
        [JsonProperty("appId")]
        public string AppId { get; set; }

        /// <summary>
        /// The Desktop Agent that the app is available on. Used in Desktop Agent Bridging to
        /// identify the Desktop Agent to target.
        ///
        /// Used in Desktop Agent Bridging to attribute or target a message to a
        /// particular Desktop Agent.
        /// </summary>
        [JsonProperty("desktopAgent")]
        public string DesktopAgent { get; set; }

        /// <summary>
        /// An optional instance identifier, indicating that this object represents a specific
        /// instance of the application described.
        /// </summary>
        [JsonProperty("instanceId", NullValueHandling = NullValueHandling.Ignore)]
        public string InstanceId { get; set; }
    }

    /// <summary>
    /// The message payload typically contains the arguments to FDC3 API functions.
    /// </summary>
    public partial class BroadcastBridgeRequestPayload
    {
        /// <summary>
        /// The Id of the Channel that the broadcast was sent on
        /// </summary>
        [JsonProperty("channelId")]
        public string ChannelId { get; set; }

        /// <summary>
        /// The context object that was the payload of a broadcast message.
        /// </summary>
        [JsonProperty("context")]
        public ContextElement Context { get; set; }
    }

    /// <summary>
    /// A message used during the connection flow for a Desktop Agent to the Bridge. Used for
    /// messages sent in either direction.
    /// </summary>
    public partial class ConnectionStepMessage
    {
        [JsonProperty("meta")]
        public ConnectionStepMetadata Meta { get; set; }

        /// <summary>
        /// The message payload, containing data pertaining to this connection step.
        /// </summary>
        [JsonProperty("payload")]
        public Dictionary<string, object> Payload { get; set; }

        /// <summary>
        /// Identifies the type of the connection step message.
        /// </summary>
        [JsonProperty("type")]
        public ConnectionStepMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for this connection step message.
    /// </summary>
    public partial class ConnectionStepMetadata
    {
        [JsonProperty("requestUuid", NullValueHandling = NullValueHandling.Ignore)]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid", NullValueHandling = NullValueHandling.Ignore)]
        public string ResponseUuid { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// Hello message sent by the Bridge to anyone connecting to the Bridge (enables
    /// identification as a bridge and confirmation of whether authentication is required)
    ///
    /// A message used during the connection flow for a Desktop Agent to the Bridge. Used for
    /// messages sent in either direction.
    /// </summary>
    public partial class ConnectionStep2Hello
    {
        [JsonProperty("meta")]
        public ConnectionStep2HelloMeta Meta { get; set; }

        /// <summary>
        /// The message payload, containing data pertaining to this connection step.
        /// </summary>
        [JsonProperty("payload")]
        public ConnectionStep2HelloPayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the connection step message.
        /// </summary>
        [JsonProperty("type")]
        public ConnectionStepMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for this connection step message.
    /// </summary>
    public partial class ConnectionStep2HelloMeta
    {
        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload, containing data pertaining to this connection step.
    /// </summary>
    public partial class ConnectionStep2HelloPayload
    {
        /// <summary>
        /// A flag indicating whether the Desktop Agent Bridge requires authentication or not.
        /// </summary>
        [JsonProperty("authRequired")]
        public bool AuthRequired { get; set; }

        /// <summary>
        /// An optional Desktop Agent Bridge JWT authentication token if the Desktop Agent want to
        /// authenticate a bridge.
        /// </summary>
        [JsonProperty("authToken", NullValueHandling = NullValueHandling.Ignore)]
        public string AuthToken { get; set; }

        /// <summary>
        /// The version of the Bridge
        /// </summary>
        [JsonProperty("desktopAgentBridgeVersion")]
        public string DesktopAgentBridgeVersion { get; set; }

        /// <summary>
        /// The FDC3 versions supported by the Bridge
        /// </summary>
        [JsonProperty("supportedFDC3Versions")]
        public string[] SupportedFdc3Versions { get; set; }
    }

    /// <summary>
    /// Handshake message sent by the Desktop Agent to the Bridge (including requested name,
    /// channel state and authentication data)
    ///
    /// A message used during the connection flow for a Desktop Agent to the Bridge. Used for
    /// messages sent in either direction.
    /// </summary>
    public partial class ConnectionStep3Handshake
    {
        [JsonProperty("meta")]
        public ConnectionStep3HandshakeMeta Meta { get; set; }

        /// <summary>
        /// The message payload, containing data pertaining to this connection step.
        /// </summary>
        [JsonProperty("payload")]
        public ConnectionStep3HandshakePayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the connection step message.
        /// </summary>
        [JsonProperty("type")]
        public ConnectionStepMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for this connection step message.
    /// </summary>
    public partial class ConnectionStep3HandshakeMeta
    {
        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload, containing data pertaining to this connection step.
    /// </summary>
    public partial class ConnectionStep3HandshakePayload
    {
        [JsonProperty("authToken", NullValueHandling = NullValueHandling.Ignore)]
        public string AuthToken { get; set; }

        /// <summary>
        /// The current state of the Desktop Agent's channels, excluding any private channels, as a
        /// mapping of channel id to an array of Context objects, most recent first.
        /// </summary>
        [JsonProperty("channelsState")]
        public Dictionary<string, ContextElement[]> ChannelsState { get; set; }

        /// <summary>
        /// Desktop Agent ImplementationMetadata trying to connect to the bridge.
        /// </summary>
        [JsonProperty("implementationMetadata")]
        public ImplementationMetadataElement ImplementationMetadata { get; set; }

        /// <summary>
        /// The requested Desktop Agent name
        /// </summary>
        [JsonProperty("requestedName")]
        public string RequestedName { get; set; }
    }

    /// <summary>
    /// Desktop Agent ImplementationMetadata trying to connect to the bridge.
    ///
    /// Metadata relating to the FDC3 Desktop Agent implementation and its provider.
    /// </summary>
    public partial class ImplementationMetadataElement
    {
        /// <summary>
        /// The version number of the FDC3 specification that the implementation provides.
        /// The string must be a numeric semver version, e.g. 1.2 or 1.2.1.
        /// </summary>
        [JsonProperty("fdc3Version")]
        public string Fdc3Version { get; set; }

        /// <summary>
        /// Metadata indicating whether the Desktop Agent implements optional features of
        /// the Desktop Agent API.
        /// </summary>
        [JsonProperty("optionalFeatures")]
        public ImplementationMetadataOptionalFeatures OptionalFeatures { get; set; }

        /// <summary>
        /// The name of the provider of the Desktop Agent implementation (e.g. Finsemble, Glue42,
        /// OpenFin etc.).
        /// </summary>
        [JsonProperty("provider")]
        public string Provider { get; set; }

        /// <summary>
        /// The version of the provider of the Desktop Agent implementation (e.g. 5.3.0).
        /// </summary>
        [JsonProperty("providerVersion", NullValueHandling = NullValueHandling.Ignore)]
        public string ProviderVersion { get; set; }
    }

    /// <summary>
    /// Metadata indicating whether the Desktop Agent implements optional features of
    /// the Desktop Agent API.
    /// </summary>
    public partial class ImplementationMetadataOptionalFeatures
    {
        /// <summary>
        /// Used to indicate whether the experimental Desktop Agent Bridging
        /// feature is implemented by the Desktop Agent.
        /// </summary>
        [JsonProperty("DesktopAgentBridging")]
        public bool DesktopAgentBridging { get; set; }

        /// <summary>
        /// Used to indicate whether the exposure of 'originating app metadata' for
        /// context and intent messages is supported by the Desktop Agent.
        /// </summary>
        [JsonProperty("OriginatingAppMetadata")]
        public bool OriginatingAppMetadata { get; set; }

        /// <summary>
        /// Used to indicate whether the optional `fdc3.joinUserChannel`,
        /// `fdc3.getCurrentChannel` and `fdc3.leaveCurrentChannel` are implemented by
        /// the Desktop Agent.
        /// </summary>
        [JsonProperty("UserChannelMembershipAPIs")]
        public bool UserChannelMembershipApIs { get; set; }
    }

    /// <summary>
    /// Message sent by Bridge to Desktop Agent if their authentication fails.
    ///
    /// A message used during the connection flow for a Desktop Agent to the Bridge. Used for
    /// messages sent in either direction.
    /// </summary>
    public partial class ConnectionStep4AuthenticationFailed
    {
        [JsonProperty("meta")]
        public ConnectionStep4AuthenticationFailedMeta Meta { get; set; }

        /// <summary>
        /// The message payload, containing data pertaining to this connection step.
        /// </summary>
        [JsonProperty("payload")]
        public ConnectionStep4AuthenticationFailedPayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the connection step message.
        /// </summary>
        [JsonProperty("type")]
        public ConnectionStepMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for this connection step message.
    /// </summary>
    public partial class ConnectionStep4AuthenticationFailedMeta
    {
        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload, containing data pertaining to this connection step.
    /// </summary>
    public partial class ConnectionStep4AuthenticationFailedPayload
    {
        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }
    }

    /// <summary>
    /// Message sent by Bridge to all Desktop Agent when an agent joins or leaves the bridge,
    /// includes the details of all agents, the change made and the expected channel state for
    /// all agents.
    ///
    /// A message used during the connection flow for a Desktop Agent to the Bridge. Used for
    /// messages sent in either direction.
    /// </summary>
    public partial class ConnectionStep6ConnectedAgentsUpdate
    {
        [JsonProperty("meta")]
        public ConnectionStep6ConnectedAgentsUpdateMeta Meta { get; set; }

        /// <summary>
        /// The message payload, containing data pertaining to this connection step.
        /// </summary>
        [JsonProperty("payload")]
        public ConnectionStep6ConnectedAgentsUpdatePayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the connection step message.
        /// </summary>
        [JsonProperty("type")]
        public ConnectionStepMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for this connection step message.
    /// </summary>
    public partial class ConnectionStep6ConnectedAgentsUpdateMeta
    {
        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload, containing data pertaining to this connection step.
    /// </summary>
    public partial class ConnectionStep6ConnectedAgentsUpdatePayload
    {
        /// <summary>
        /// Should be set when an agent first connects to the bridge and provide its assigned name.
        /// </summary>
        [JsonProperty("addAgent", NullValueHandling = NullValueHandling.Ignore)]
        public string AddAgent { get; set; }

        /// <summary>
        /// Desktop Agent Bridge implementation metadata of all connected agents.
        /// </summary>
        [JsonProperty("allAgents")]
        public ImplementationMetadataElement[] AllAgents { get; set; }

        /// <summary>
        /// The updated state of channels that should be adopted by the agents. Should only be set
        /// when an agent is connecting to the bridge.
        /// </summary>
        [JsonProperty("channelsState", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, ContextElement[]> ChannelsState { get; set; }

        /// <summary>
        /// Should be set when an agent disconnects from the bridge and provide the name that no
        /// longer is assigned.
        /// </summary>
        [JsonProperty("removeAgent", NullValueHandling = NullValueHandling.Ignore)]
        public string RemoveAgent { get; set; }
    }

    /// <summary>
    /// A response to a findInstances request that contains an error.
    ///
    /// A response message from a Desktop Agent to the Bridge containing an error, to be used in
    /// preference to the standard response when an error needs to be returned.
    /// </summary>
    public partial class FindInstancesAgentErrorResponse
    {
        [JsonProperty("meta")]
        public FindInstancesAgentErrorResponseMeta Meta { get; set; }

        /// <summary>
        /// Error message payload containing an standardized error string.
        /// </summary>
        [JsonProperty("payload")]
        public FindInstancesAgentErrorResponsePayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Response' appended.
        /// </summary>
        [JsonProperty("type")]
        public ResponseMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for a response messages sent by a Desktop Agent to the Bridge
    /// </summary>
    public partial class FindInstancesAgentErrorResponseMeta
    {
        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// Error message payload containing an standardized error string.
    /// </summary>
    public partial class FindInstancesAgentErrorResponsePayload
    {
        [JsonProperty("error")]
        public ErrorMessage Error { get; set; }
    }

    /// <summary>
    /// A request for details of instances of a particular app
    ///
    /// A request message from a Desktop Agent to the Bridge.
    /// </summary>
    public partial class FindInstancesAgentRequest
    {
        [JsonProperty("meta")]
        public FindInstancesAgentRequestMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains the arguments to FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public FindInstancesAgentRequestPayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Request' appended.
        /// </summary>
        [JsonProperty("type")]
        public RequestMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for a request message sent by Desktop Agents to the Bridge.
    /// </summary>
    public partial class FindInstancesAgentRequestMeta
    {
        /// <summary>
        /// Optional field that represents the destination that the request should be routed to. Must
        /// be set by the Desktop Agent for API calls that include a target app parameter and must
        /// include the name of the Desktop Agent hosting the target application.
        /// </summary>
        [JsonProperty("destination", NullValueHandling = NullValueHandling.Ignore)]
        public DestinationClass Destination { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        /// <summary>
        /// Field that represents the source application that the request was received from, or the
        /// source Desktop Agent if it issued the request itself.
        /// </summary>
        [JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
        public SourceIdentifier Source { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// Identifies a particular Desktop Agent in Desktop Agent Bridging scenarios
    /// where a request needs to be directed to a Desktop Agent rather than a specific app, or a
    /// response message is returned by the Desktop Agent (or more specifically its resolver)
    /// rather than a specific app. Used as a substitute for `AppIdentifier` in cases where no
    /// app details are available or are appropriate.
    ///
    /// Array of DesktopAgentIdentifiers for responses that were not returned to the bridge
    /// before the timeout or because an error occurred. May be omitted if all sources responded
    /// without errors. MUST include the `desktopAgent` field when returned by the bridge.
    ///
    /// Array of DesktopAgentIdentifiers for the sources that generated responses to the request.
    /// Will contain a single value for individual responses and multiple values for responses
    /// that were collated by the bridge. May be omitted if all sources errored. MUST include the
    /// `desktopAgent` field when returned by the bridge.
    ///
    /// Field that represents a destination Desktop Agent that a request is to be sent to.
    ///
    /// Optional field that represents the destination that the request should be routed to. Must
    /// be set by the Desktop Agent for API calls that include a target app parameter and must
    /// include the name of the Desktop Agent hosting the target application.
    ///
    /// Represents identifiers that MUST include the Desktop Agent name and MAY identify a
    /// specific app or instance.
    ///
    /// Field that represents the source application that the request was received from, or the
    /// source Desktop Agent if it issued the request itself. The Desktop Agent identifier MUST
    /// be set by the bridge.
    ///
    /// Field that represents a destination App on a remote Desktop Agent that a request is to be
    /// sent to.
    ///
    /// Identifies an application, or instance of an application, and is used to target FDC3 API
    /// calls, such as `fdc3.open` or `fdc3.raiseIntent` at specific applications or application
    /// instances.
    ///
    /// Will always include at least an `appId` field, which uniquely identifies a specific app.
    ///
    /// If the `instanceId` field is set then the `AppMetadata` object represents a specific
    /// instance of the application that may be addressed using that Id.
    ///
    /// Field that represents the source application that a request or response was received
    /// from.
    ///
    /// Identifier for the app instance that was selected (or started) to resolve the intent.
    /// `source.instanceId` MUST be set, indicating the specific app instance that
    /// received the intent.
    /// </summary>
    public partial class DestinationClass
    {
        /// <summary>
        /// Used in Desktop Agent Bridging to attribute or target a message to a
        /// particular Desktop Agent.
        ///
        /// The Desktop Agent that the app is available on. Used in Desktop Agent Bridging to
        /// identify the Desktop Agent to target.
        /// </summary>
        [JsonProperty("desktopAgent")]
        public string DesktopAgent { get; set; }

        /// <summary>
        /// The unique application identifier located within a specific application directory
        /// instance. An example of an appId might be 'app@sub.root'
        /// </summary>
        [JsonProperty("appId", NullValueHandling = NullValueHandling.Ignore)]
        public string AppId { get; set; }

        /// <summary>
        /// An optional instance identifier, indicating that this object represents a specific
        /// instance of the application described.
        /// </summary>
        [JsonProperty("instanceId", NullValueHandling = NullValueHandling.Ignore)]
        public string InstanceId { get; set; }
    }

    /// <summary>
    /// The message payload typically contains the arguments to FDC3 API functions.
    /// </summary>
    public partial class FindInstancesAgentRequestPayload
    {
        [JsonProperty("app")]
        public AppIdentifier App { get; set; }
    }

    /// <summary>
    /// Identifies an application, or instance of an application, and is used to target FDC3 API
    /// calls, such as `fdc3.open` or `fdc3.raiseIntent` at specific applications or application
    /// instances.
    ///
    /// Will always include at least an `appId` field, which uniquely identifies a specific app.
    ///
    /// If the `instanceId` field is set then the `AppMetadata` object represents a specific
    /// instance of the application that may be addressed using that Id.
    ///
    /// Field that represents the source application that a request or response was received
    /// from.
    ///
    /// Identifier for the app instance that was selected (or started) to resolve the intent.
    /// `source.instanceId` MUST be set, indicating the specific app instance that
    /// received the intent.
    /// </summary>
    public partial class AppIdentifier
    {
        /// <summary>
        /// The unique application identifier located within a specific application directory
        /// instance. An example of an appId might be 'app@sub.root'
        /// </summary>
        [JsonProperty("appId")]
        public string AppId { get; set; }

        /// <summary>
        /// The Desktop Agent that the app is available on. Used in Desktop Agent Bridging to
        /// identify the Desktop Agent to target.
        /// </summary>
        [JsonProperty("desktopAgent", NullValueHandling = NullValueHandling.Ignore)]
        public string DesktopAgent { get; set; }

        /// <summary>
        /// An optional instance identifier, indicating that this object represents a specific
        /// instance of the application described.
        /// </summary>
        [JsonProperty("instanceId", NullValueHandling = NullValueHandling.Ignore)]
        public string InstanceId { get; set; }
    }

    /// <summary>
    /// A response to a findInstances request.
    ///
    /// A response message from a Desktop Agent to the Bridge.
    /// </summary>
    public partial class FindInstancesAgentResponse
    {
        [JsonProperty("meta")]
        public FindInstancesAgentResponseMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains return values for FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public FindInstancesAgentResponsePayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Response' appended.
        /// </summary>
        [JsonProperty("type")]
        public ResponseMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for a response messages sent by a Desktop Agent to the Bridge
    /// </summary>
    public partial class FindInstancesAgentResponseMeta
    {
        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload typically contains return values for FDC3 API functions.
    /// </summary>
    public partial class FindInstancesAgentResponsePayload
    {
        [JsonProperty("appIdentifiers")]
        public AppMetadata[] AppIdentifiers { get; set; }
    }

    /// <summary>
    /// Extends an `AppIdentifier`, describing an application or instance of an application, with
    /// additional descriptive metadata that is usually provided by an FDC3 App Directory that
    /// the desktop agent connects to.
    ///
    /// The additional information from an app directory can aid in rendering UI elements, such
    /// as a launcher menu or resolver UI. This includes a title, description, tooltip and icon
    /// and screenshot URLs.
    ///
    /// Note that as `AppMetadata` instances are also `AppIdentifiers` they may be passed to the
    /// `app` argument of `fdc3.open`, `fdc3.raiseIntent` etc.
    /// </summary>
    public partial class AppMetadata
    {
        /// <summary>
        /// The unique application identifier located within a specific application directory
        /// instance. An example of an appId might be 'app@sub.root'
        /// </summary>
        [JsonProperty("appId")]
        public string AppId { get; set; }

        /// <summary>
        /// A longer, multi-paragraph description for the application that could include markup
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        /// <summary>
        /// The Desktop Agent that the app is available on. Used in Desktop Agent Bridging to
        /// identify the Desktop Agent to target.
        /// </summary>
        [JsonProperty("desktopAgent", NullValueHandling = NullValueHandling.Ignore)]
        public string DesktopAgent { get; set; }

        /// <summary>
        /// A list of icon URLs for the application that can be used to render UI elements
        /// </summary>
        [JsonProperty("icons", NullValueHandling = NullValueHandling.Ignore)]
        public Icon[] Icons { get; set; }

        /// <summary>
        /// An optional instance identifier, indicating that this object represents a specific
        /// instance of the application described.
        /// </summary>
        [JsonProperty("instanceId", NullValueHandling = NullValueHandling.Ignore)]
        public string InstanceId { get; set; }

        /// <summary>
        /// An optional set of, implementation specific, metadata fields that can be used to
        /// disambiguate instances, such as a window title or screen position. Must only be set if
        /// `instanceId` is set.
        /// </summary>
        [JsonProperty("instanceMetadata", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> InstanceMetadata { get; set; }

        /// <summary>
        /// The 'friendly' app name.
        /// This field was used with the `open` and `raiseIntent` calls in FDC3 <2.0, which now
        /// require an `AppIdentifier` wth `appId` set.
        /// Note that for display purposes the `title` field should be used, if set, in preference to
        /// this field.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// The type of output returned for any intent specified during resolution. May express a
        /// particular context type (e.g. "fdc3.instrument"), channel (e.g. "channel") or a channel
        /// that will receive a specified type (e.g. "channel<fdc3.instrument>").
        /// </summary>
        [JsonProperty("resultType")]
        public string ResultType { get; set; }

        /// <summary>
        /// Images representing the app in common usage scenarios that can be used to render UI
        /// elements
        /// </summary>
        [JsonProperty("screenshots", NullValueHandling = NullValueHandling.Ignore)]
        public Image[] Screenshots { get; set; }

        /// <summary>
        /// A more user-friendly application title that can be used to render UI elements
        /// </summary>
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        /// <summary>
        /// A tooltip for the application that can be used to render UI elements
        /// </summary>
        [JsonProperty("tooltip", NullValueHandling = NullValueHandling.Ignore)]
        public string Tooltip { get; set; }

        /// <summary>
        /// The Version of the application.
        /// </summary>
        [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
        public string Version { get; set; }
    }

    /// <summary>
    /// SPDX-License-Identifier: Apache-2.0
    /// Copyright FINOS FDC3 contributors - see NOTICE file
    /// </summary>
    public partial class Icon
    {
        /// <summary>
        /// The icon dimension, formatted as `<height>x<width>`.
        /// </summary>
        [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
        public string Size { get; set; }

        /// <summary>
        /// The icon url
        /// </summary>
        [JsonProperty("src")]
        public string Src { get; set; }

        /// <summary>
        /// Icon media type. If not present the Desktop Agent may use the src file extension.
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }
    }

    /// <summary>
    /// SPDX-License-Identifier: Apache-2.0
    /// Copyright FINOS FDC3 contributors - see NOTICE file
    /// </summary>
    public partial class Image
    {
        /// <summary>
        /// Caption for the image.
        /// </summary>
        [JsonProperty("label", NullValueHandling = NullValueHandling.Ignore)]
        public string Label { get; set; }

        /// <summary>
        /// The image dimension, formatted as `<height>x<width>`.
        /// </summary>
        [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
        public string Size { get; set; }

        /// <summary>
        /// The image url.
        /// </summary>
        [JsonProperty("src")]
        public string Src { get; set; }

        /// <summary>
        /// Image media type. If not present the Desktop Agent may use the src file extension.
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }
    }

    /// <summary>
    /// A response to a findInstances request that contains an error.
    ///
    /// A response message from the Bridge back to the original Desktop Agent that raised the
    /// request, used where all connected agents returned errors.
    /// </summary>
    public partial class FindInstancesBridgeErrorResponse
    {
        [JsonProperty("meta")]
        public FindInstancesBridgeErrorResponseMeta Meta { get; set; }

        /// <summary>
        /// The error message payload contains details of an error return to the app or agent that
        /// raised the original request.
        /// </summary>
        [JsonProperty("payload")]
        public FindInstancesBridgeErrorResponsePayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Response' appended.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Metadata required in a response message collated and/or forwarded on by the Bridge
    /// </summary>
    public partial class FindInstancesBridgeErrorResponseMeta
    {
        [JsonProperty("errorDetails")]
        public ResponseErrorDetail[] ErrorDetails { get; set; }

        [JsonProperty("errorSources")]
        public DesktopAgentIdentifier[] ErrorSources { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The error message payload contains details of an error return to the app or agent that
    /// raised the original request.
    /// </summary>
    public partial class FindInstancesBridgeErrorResponsePayload
    {
        [JsonProperty("error")]
        public ErrorMessage Error { get; set; }
    }

    /// <summary>
    /// A request for details of instances of a particular app
    ///
    /// A request message forwarded from the Bridge onto a Desktop Agent connected to it.
    /// </summary>
    public partial class FindInstancesBridgeRequest
    {
        [JsonProperty("meta")]
        public FindInstancesBridgeRequestMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains the arguments to FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public FindInstancesBridgeRequestPayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Request' appended.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Metadata required in a request message forwarded on by the Bridge
    /// </summary>
    public partial class FindInstancesBridgeRequestMeta
    {
        /// <summary>
        /// Optional field that represents the destination that the request should be routed to. Must
        /// be set by the Desktop Agent for API calls that include a target app parameter and must
        /// include the name of the Desktop Agent hosting the target application.
        /// </summary>
        [JsonProperty("destination", NullValueHandling = NullValueHandling.Ignore)]
        public DestinationClass Destination { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        /// <summary>
        /// Field that represents the source application that the request was received from, or the
        /// source Desktop Agent if it issued the request itself. The Desktop Agent identifier MUST
        /// be set by the bridge.
        /// </summary>
        [JsonProperty("source")]
        public MetaSourceClass Source { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// Field that represents the source application that the request was received from, or the
    /// source Desktop Agent if it issued the request itself.
    ///
    /// Field that represents the source application that a request or response was received
    /// from, or the source Desktop Agent if it issued the request or response itself.
    ///
    /// Identifies an application, or instance of an application, and is used to target FDC3 API
    /// calls, such as `fdc3.open` or `fdc3.raiseIntent` at specific applications or application
    /// instances.
    ///
    /// Will always include at least an `appId` field, which uniquely identifies a specific app.
    ///
    /// If the `instanceId` field is set then the `AppMetadata` object represents a specific
    /// instance of the application that may be addressed using that Id.
    ///
    /// Field that represents the source application that a request or response was received
    /// from.
    ///
    /// Identifier for the app instance that was selected (or started) to resolve the intent.
    /// `source.instanceId` MUST be set, indicating the specific app instance that
    /// received the intent.
    ///
    /// Identifies a particular Desktop Agent in Desktop Agent Bridging scenarios
    /// where a request needs to be directed to a Desktop Agent rather than a specific app, or a
    /// response message is returned by the Desktop Agent (or more specifically its resolver)
    /// rather than a specific app. Used as a substitute for `AppIdentifier` in cases where no
    /// app details are available or are appropriate.
    ///
    /// Array of DesktopAgentIdentifiers for responses that were not returned to the bridge
    /// before the timeout or because an error occurred. May be omitted if all sources responded
    /// without errors. MUST include the `desktopAgent` field when returned by the bridge.
    ///
    /// Array of DesktopAgentIdentifiers for the sources that generated responses to the request.
    /// Will contain a single value for individual responses and multiple values for responses
    /// that were collated by the bridge. May be omitted if all sources errored. MUST include the
    /// `desktopAgent` field when returned by the bridge.
    ///
    /// Field that represents a destination Desktop Agent that a request is to be sent to.
    ///
    /// Optional field that represents the destination that the request should be routed to. Must
    /// be set by the Desktop Agent for API calls that include a target app parameter and must
    /// include the name of the Desktop Agent hosting the target application.
    ///
    /// Represents identifiers that MUST include the Desktop Agent name and MAY identify a
    /// specific app or instance.
    ///
    /// Field that represents the source application that the request was received from, or the
    /// source Desktop Agent if it issued the request itself. The Desktop Agent identifier MUST
    /// be set by the bridge.
    ///
    /// Field that represents a destination App on a remote Desktop Agent that a request is to be
    /// sent to.
    /// </summary>
    public partial class MetaSourceClass
    {
        /// <summary>
        /// The unique application identifier located within a specific application directory
        /// instance. An example of an appId might be 'app@sub.root'
        /// </summary>
        [JsonProperty("appId", NullValueHandling = NullValueHandling.Ignore)]
        public string AppId { get; set; }

        /// <summary>
        /// The Desktop Agent that the app is available on. Used in Desktop Agent Bridging to
        /// identify the Desktop Agent to target.
        ///
        /// Used in Desktop Agent Bridging to attribute or target a message to a
        /// particular Desktop Agent.
        /// </summary>
        [JsonProperty("desktopAgent")]
        public string DesktopAgent { get; set; }

        /// <summary>
        /// An optional instance identifier, indicating that this object represents a specific
        /// instance of the application described.
        /// </summary>
        [JsonProperty("instanceId", NullValueHandling = NullValueHandling.Ignore)]
        public string InstanceId { get; set; }
    }

    /// <summary>
    /// The message payload typically contains the arguments to FDC3 API functions.
    /// </summary>
    public partial class FindInstancesBridgeRequestPayload
    {
        [JsonProperty("app")]
        public AppIdentifier App { get; set; }
    }

    /// <summary>
    /// A response to a findInstances request.
    ///
    /// A response message from the Bridge back to the original Desktop Agent that raised the
    /// request.
    /// </summary>
    public partial class FindInstancesBridgeResponse
    {
        [JsonProperty("meta")]
        public FindInstancesBridgeResponseMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains return values for FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public FindInstancesBridgeResponsePayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Response' appended.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Metadata required in a response message collated and/or forwarded on by the Bridge
    /// </summary>
    public partial class FindInstancesBridgeResponseMeta
    {
        [JsonProperty("errorDetails", NullValueHandling = NullValueHandling.Ignore)]
        public ResponseErrorDetail[] ErrorDetails { get; set; }

        [JsonProperty("errorSources", NullValueHandling = NullValueHandling.Ignore)]
        public DesktopAgentIdentifier[] ErrorSources { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("sources", NullValueHandling = NullValueHandling.Ignore)]
        public DesktopAgentIdentifier[] Sources { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload typically contains return values for FDC3 API functions.
    /// </summary>
    public partial class FindInstancesBridgeResponsePayload
    {
        [JsonProperty("appIdentifiers")]
        public AppMetadata[] AppIdentifiers { get; set; }
    }

    /// <summary>
    /// A response to a findIntent request that contains an error.
    ///
    /// A response message from a Desktop Agent to the Bridge containing an error, to be used in
    /// preference to the standard response when an error needs to be returned.
    /// </summary>
    public partial class FindIntentAgentErrorResponse
    {
        [JsonProperty("meta")]
        public FindIntentAgentErrorResponseMeta Meta { get; set; }

        /// <summary>
        /// Error message payload containing an standardized error string.
        /// </summary>
        [JsonProperty("payload")]
        public FindIntentAgentErrorResponsePayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Response' appended.
        /// </summary>
        [JsonProperty("type")]
        public ResponseMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for a response messages sent by a Desktop Agent to the Bridge
    /// </summary>
    public partial class FindIntentAgentErrorResponseMeta
    {
        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// Error message payload containing an standardized error string.
    /// </summary>
    public partial class FindIntentAgentErrorResponsePayload
    {
        [JsonProperty("error")]
        public ErrorMessage Error { get; set; }
    }

    /// <summary>
    /// A request for details of apps available to resolve a particular intent and context pair.
    ///
    /// A request message from a Desktop Agent to the Bridge.
    /// </summary>
    public partial class FindIntentAgentRequest
    {
        [JsonProperty("meta")]
        public FindIntentAgentRequestMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains the arguments to FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public FindIntentAgentRequestPayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Request' appended.
        /// </summary>
        [JsonProperty("type")]
        public RequestMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for a request message sent by Desktop Agents to the Bridge.
    /// </summary>
    public partial class FindIntentAgentRequestMeta
    {
        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        /// <summary>
        /// Field that represents the source application that the request was received from, or the
        /// source Desktop Agent if it issued the request itself.
        /// </summary>
        [JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
        public SourceIdentifier Source { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Optional field that represents the destination that the request should be routed to. Must
        /// be set by the Desktop Agent for API calls that include a target app parameter and must
        /// include the name of the Desktop Agent hosting the target application.
        /// </summary>
        [JsonProperty("destination", NullValueHandling = NullValueHandling.Ignore)]
        public BridgeParticipantIdentifier Destination { get; set; }
    }

    /// <summary>
    /// The message payload typically contains the arguments to FDC3 API functions.
    /// </summary>
    public partial class FindIntentAgentRequestPayload
    {
        [JsonProperty("context", NullValueHandling = NullValueHandling.Ignore)]
        public ContextElement Context { get; set; }

        [JsonProperty("intent")]
        public string Intent { get; set; }
    }

    /// <summary>
    /// A response to a findIntent request.
    ///
    /// A response message from a Desktop Agent to the Bridge.
    /// </summary>
    public partial class FindIntentAgentResponse
    {
        [JsonProperty("meta")]
        public FindIntentAgentResponseMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains return values for FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public FindIntentAgentResponsePayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Response' appended.
        /// </summary>
        [JsonProperty("type")]
        public ResponseMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for a response messages sent by a Desktop Agent to the Bridge
    /// </summary>
    public partial class FindIntentAgentResponseMeta
    {
        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload typically contains return values for FDC3 API functions.
    /// </summary>
    public partial class FindIntentAgentResponsePayload
    {
        [JsonProperty("appIntent")]
        public AppIntent AppIntent { get; set; }
    }

    /// <summary>
    /// An interface that relates an intent to apps
    /// </summary>
    public partial class AppIntent
    {
        /// <summary>
        /// Details of applications that can resolve the intent.
        /// </summary>
        [JsonProperty("apps")]
        public AppMetadata[] Apps { get; set; }

        /// <summary>
        /// Details of the intent whose relationship to resolving applications is being described.
        /// </summary>
        [JsonProperty("intent")]
        public IntentMetadata Intent { get; set; }
    }

    /// <summary>
    /// Details of the intent whose relationship to resolving applications is being described.
    ///
    /// Intent descriptor
    /// </summary>
    public partial class IntentMetadata
    {
        /// <summary>
        /// Display name for the intent.
        /// </summary>
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// The unique name of the intent that can be invoked by the raiseIntent call
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    /// <summary>
    /// A response to a findIntent request that contains an error.
    ///
    /// A response message from the Bridge back to the original Desktop Agent that raised the
    /// request, used where all connected agents returned errors.
    /// </summary>
    public partial class FindIntentBridgeErrorResponse
    {
        [JsonProperty("meta")]
        public FindIntentBridgeErrorResponseMeta Meta { get; set; }

        /// <summary>
        /// The error message payload contains details of an error return to the app or agent that
        /// raised the original request.
        /// </summary>
        [JsonProperty("payload")]
        public FindIntentBridgeErrorResponsePayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Response' appended.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Metadata required in a response message collated and/or forwarded on by the Bridge
    /// </summary>
    public partial class FindIntentBridgeErrorResponseMeta
    {
        [JsonProperty("errorDetails")]
        public ResponseErrorDetail[] ErrorDetails { get; set; }

        [JsonProperty("errorSources")]
        public DesktopAgentIdentifier[] ErrorSources { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The error message payload contains details of an error return to the app or agent that
    /// raised the original request.
    /// </summary>
    public partial class FindIntentBridgeErrorResponsePayload
    {
        [JsonProperty("error")]
        public ErrorMessage Error { get; set; }
    }

    /// <summary>
    /// A request for details of apps available to resolve a particular intent and context pair.
    ///
    /// A request message forwarded from the Bridge onto a Desktop Agent connected to it.
    /// </summary>
    public partial class FindIntentBridgeRequest
    {
        [JsonProperty("meta")]
        public FindIntentBridgeRequestMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains the arguments to FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public FindIntentBridgeRequestPayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Request' appended.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Metadata required in a request message forwarded on by the Bridge
    /// </summary>
    public partial class FindIntentBridgeRequestMeta
    {
        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        /// <summary>
        /// Field that represents the source application that the request was received from, or the
        /// source Desktop Agent if it issued the request itself. The Desktop Agent identifier MUST
        /// be set by the bridge.
        /// </summary>
        [JsonProperty("source")]
        public BridgeParticipantIdentifier Source { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Optional field that represents the destination that the request should be routed to. Must
        /// be set by the Desktop Agent for API calls that include a target app parameter and must
        /// include the name of the Desktop Agent hosting the target application.
        /// </summary>
        [JsonProperty("destination", NullValueHandling = NullValueHandling.Ignore)]
        public BridgeParticipantIdentifier Destination { get; set; }
    }

    /// <summary>
    /// The message payload typically contains the arguments to FDC3 API functions.
    /// </summary>
    public partial class FindIntentBridgeRequestPayload
    {
        [JsonProperty("context", NullValueHandling = NullValueHandling.Ignore)]
        public ContextElement Context { get; set; }

        [JsonProperty("intent")]
        public string Intent { get; set; }
    }

    /// <summary>
    /// A response to a findIntent request.
    ///
    /// A response message from the Bridge back to the original Desktop Agent that raised the
    /// request.
    /// </summary>
    public partial class FindIntentBridgeResponse
    {
        [JsonProperty("meta")]
        public FindIntentBridgeResponseMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains return values for FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public FindIntentBridgeResponsePayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Response' appended.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Metadata required in a response message collated and/or forwarded on by the Bridge
    /// </summary>
    public partial class FindIntentBridgeResponseMeta
    {
        [JsonProperty("errorDetails", NullValueHandling = NullValueHandling.Ignore)]
        public ResponseErrorDetail[] ErrorDetails { get; set; }

        [JsonProperty("errorSources", NullValueHandling = NullValueHandling.Ignore)]
        public DesktopAgentIdentifier[] ErrorSources { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("sources", NullValueHandling = NullValueHandling.Ignore)]
        public DesktopAgentIdentifier[] Sources { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload typically contains return values for FDC3 API functions.
    /// </summary>
    public partial class FindIntentBridgeResponsePayload
    {
        [JsonProperty("appIntent")]
        public AppIntent AppIntent { get; set; }
    }

    /// <summary>
    /// A response to a findIntentsByContext request that contains an error.
    ///
    /// A response message from a Desktop Agent to the Bridge containing an error, to be used in
    /// preference to the standard response when an error needs to be returned.
    /// </summary>
    public partial class FindIntentsByContextAgentErrorResponse
    {
        [JsonProperty("meta")]
        public FindIntentsByContextAgentErrorResponseMeta Meta { get; set; }

        /// <summary>
        /// Error message payload containing an standardized error string.
        /// </summary>
        [JsonProperty("payload")]
        public FindIntentsByContextAgentErrorResponsePayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Response' appended.
        /// </summary>
        [JsonProperty("type")]
        public ResponseMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for a response messages sent by a Desktop Agent to the Bridge
    /// </summary>
    public partial class FindIntentsByContextAgentErrorResponseMeta
    {
        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// Error message payload containing an standardized error string.
    /// </summary>
    public partial class FindIntentsByContextAgentErrorResponsePayload
    {
        [JsonProperty("error")]
        public ErrorMessage Error { get; set; }
    }

    /// <summary>
    /// A request for details of intents and apps available to resolve them for a particular
    /// context.
    ///
    /// A request message from a Desktop Agent to the Bridge.
    /// </summary>
    public partial class FindIntentsByContextAgentRequest
    {
        [JsonProperty("meta")]
        public FindIntentsByContextAgentRequestMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains the arguments to FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public FindIntentsByContextAgentRequestPayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Request' appended.
        /// </summary>
        [JsonProperty("type")]
        public RequestMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for a request message sent by Desktop Agents to the Bridge.
    /// </summary>
    public partial class FindIntentsByContextAgentRequestMeta
    {
        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        /// <summary>
        /// Field that represents the source application that the request was received from, or the
        /// source Desktop Agent if it issued the request itself.
        /// </summary>
        [JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
        public SourceClass Source { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Optional field that represents the destination that the request should be routed to. Must
        /// be set by the Desktop Agent for API calls that include a target app parameter and must
        /// include the name of the Desktop Agent hosting the target application.
        /// </summary>
        [JsonProperty("destination", NullValueHandling = NullValueHandling.Ignore)]
        public BridgeParticipantIdentifier Destination { get; set; }
    }

    /// <summary>
    /// The message payload typically contains the arguments to FDC3 API functions.
    /// </summary>
    public partial class FindIntentsByContextAgentRequestPayload
    {
        [JsonProperty("context")]
        public ContextElement Context { get; set; }
    }

    /// <summary>
    /// A response to a findIntentsByContext request.
    ///
    /// A response message from a Desktop Agent to the Bridge.
    /// </summary>
    public partial class FindIntentsByContextAgentResponse
    {
        [JsonProperty("meta")]
        public FindIntentsByContextAgentResponseMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains return values for FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public FindIntentsByContextAgentResponsePayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Response' appended.
        /// </summary>
        [JsonProperty("type")]
        public ResponseMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for a response messages sent by a Desktop Agent to the Bridge
    /// </summary>
    public partial class FindIntentsByContextAgentResponseMeta
    {
        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload typically contains return values for FDC3 API functions.
    /// </summary>
    public partial class FindIntentsByContextAgentResponsePayload
    {
        [JsonProperty("appIntents")]
        public AppIntent[] AppIntents { get; set; }
    }

    /// <summary>
    /// A response to a findIntentsByContext request that contains an error.
    ///
    /// A response message from the Bridge back to the original Desktop Agent that raised the
    /// request, used where all connected agents returned errors.
    /// </summary>
    public partial class FindIntentsByContextBridgeErrorResponse
    {
        [JsonProperty("meta")]
        public FindIntentsByContextBridgeErrorResponseMeta Meta { get; set; }

        /// <summary>
        /// The error message payload contains details of an error return to the app or agent that
        /// raised the original request.
        /// </summary>
        [JsonProperty("payload")]
        public FindIntentsByContextBridgeErrorResponsePayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Response' appended.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Metadata required in a response message collated and/or forwarded on by the Bridge
    /// </summary>
    public partial class FindIntentsByContextBridgeErrorResponseMeta
    {
        [JsonProperty("errorDetails")]
        public ResponseErrorDetail[] ErrorDetails { get; set; }

        [JsonProperty("errorSources")]
        public DesktopAgentIdentifier[] ErrorSources { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The error message payload contains details of an error return to the app or agent that
    /// raised the original request.
    /// </summary>
    public partial class FindIntentsByContextBridgeErrorResponsePayload
    {
        [JsonProperty("error")]
        public ErrorMessage Error { get; set; }
    }

    /// <summary>
    /// A request for details of intents and apps available to resolve them for a particular
    /// context.
    ///
    /// A request message forwarded from the Bridge onto a Desktop Agent connected to it.
    /// </summary>
    public partial class FindIntentsByContextBridgeRequest
    {
        [JsonProperty("meta")]
        public FindIntentsByContextBridgeRequestMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains the arguments to FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public FindIntentsByContextBridgeRequestPayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Request' appended.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Metadata required in a request message forwarded on by the Bridge
    /// </summary>
    public partial class FindIntentsByContextBridgeRequestMeta
    {
        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        /// <summary>
        /// Field that represents the source application that the request was received from, or the
        /// source Desktop Agent if it issued the request itself. The Desktop Agent identifier MUST
        /// be set by the bridge.
        /// </summary>
        [JsonProperty("source")]
        public MetaSource Source { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Optional field that represents the destination that the request should be routed to. Must
        /// be set by the Desktop Agent for API calls that include a target app parameter and must
        /// include the name of the Desktop Agent hosting the target application.
        /// </summary>
        [JsonProperty("destination", NullValueHandling = NullValueHandling.Ignore)]
        public BridgeParticipantIdentifier Destination { get; set; }
    }

    /// <summary>
    /// The message payload typically contains the arguments to FDC3 API functions.
    /// </summary>
    public partial class FindIntentsByContextBridgeRequestPayload
    {
        [JsonProperty("context")]
        public ContextElement Context { get; set; }
    }

    /// <summary>
    /// A response to a findIntentsByContext request.
    ///
    /// A response message from the Bridge back to the original Desktop Agent that raised the
    /// request.
    /// </summary>
    public partial class FindIntentsByContextBridgeResponse
    {
        [JsonProperty("meta")]
        public FindIntentsByContextBridgeResponseMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains return values for FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public FindIntentsByContextBridgeResponsePayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Response' appended.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Metadata required in a response message collated and/or forwarded on by the Bridge
    /// </summary>
    public partial class FindIntentsByContextBridgeResponseMeta
    {
        [JsonProperty("errorDetails", NullValueHandling = NullValueHandling.Ignore)]
        public ResponseErrorDetail[] ErrorDetails { get; set; }

        [JsonProperty("errorSources", NullValueHandling = NullValueHandling.Ignore)]
        public DesktopAgentIdentifier[] ErrorSources { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("sources", NullValueHandling = NullValueHandling.Ignore)]
        public DesktopAgentIdentifier[] Sources { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload typically contains return values for FDC3 API functions.
    /// </summary>
    public partial class FindIntentsByContextBridgeResponsePayload
    {
        [JsonProperty("appIntents")]
        public AppIntent[] AppIntents { get; set; }
    }

    /// <summary>
    /// A response to a getAppMetadata request that contains an error.
    ///
    /// A response message from a Desktop Agent to the Bridge containing an error, to be used in
    /// preference to the standard response when an error needs to be returned.
    /// </summary>
    public partial class GetAppMetadataAgentErrorResponse
    {
        [JsonProperty("meta")]
        public GetAppMetadataAgentErrorResponseMeta Meta { get; set; }

        /// <summary>
        /// Error message payload containing an standardized error string.
        /// </summary>
        [JsonProperty("payload")]
        public GetAppMetadataAgentErrorResponsePayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Response' appended.
        /// </summary>
        [JsonProperty("type")]
        public ResponseMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for a response messages sent by a Desktop Agent to the Bridge
    /// </summary>
    public partial class GetAppMetadataAgentErrorResponseMeta
    {
        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// Error message payload containing an standardized error string.
    /// </summary>
    public partial class GetAppMetadataAgentErrorResponsePayload
    {
        [JsonProperty("error")]
        public ErrorMessage Error { get; set; }
    }

    /// <summary>
    /// A request for metadata about an app
    ///
    /// A request message from a Desktop Agent to the Bridge.
    /// </summary>
    public partial class GetAppMetadataAgentRequest
    {
        [JsonProperty("meta")]
        public GetAppMetadataAgentRequestMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains the arguments to FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public GetAppMetadataAgentRequestPayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Request' appended.
        /// </summary>
        [JsonProperty("type")]
        public RequestMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for a request message sent by Desktop Agents to the Bridge.
    /// </summary>
    public partial class GetAppMetadataAgentRequestMeta
    {
        /// <summary>
        /// Optional field that represents the destination that the request should be routed to. Must
        /// be set by the Desktop Agent for API calls that include a target app parameter and must
        /// include the name of the Desktop Agent hosting the target application.
        /// </summary>
        [JsonProperty("destination", NullValueHandling = NullValueHandling.Ignore)]
        public DestinationClass Destination { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        /// <summary>
        /// Field that represents the source application that the request was received from, or the
        /// source Desktop Agent if it issued the request itself.
        /// </summary>
        [JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
        public SourceIdentifier Source { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload typically contains the arguments to FDC3 API functions.
    /// </summary>
    public partial class GetAppMetadataAgentRequestPayload
    {
        [JsonProperty("app")]
        public AppDestinationIdentifier App { get; set; }
    }

    /// <summary>
    /// Field that represents a destination App on a remote Desktop Agent that a request is to be
    /// sent to.
    ///
    /// Identifies a particular Desktop Agent in Desktop Agent Bridging scenarios
    /// where a request needs to be directed to a Desktop Agent rather than a specific app, or a
    /// response message is returned by the Desktop Agent (or more specifically its resolver)
    /// rather than a specific app. Used as a substitute for `AppIdentifier` in cases where no
    /// app details are available or are appropriate.
    ///
    /// Array of DesktopAgentIdentifiers for responses that were not returned to the bridge
    /// before the timeout or because an error occurred. May be omitted if all sources responded
    /// without errors. MUST include the `desktopAgent` field when returned by the bridge.
    ///
    /// Array of DesktopAgentIdentifiers for the sources that generated responses to the request.
    /// Will contain a single value for individual responses and multiple values for responses
    /// that were collated by the bridge. May be omitted if all sources errored. MUST include the
    /// `desktopAgent` field when returned by the bridge.
    ///
    /// Field that represents a destination Desktop Agent that a request is to be sent to.
    ///
    /// Identifies an application, or instance of an application, and is used to target FDC3 API
    /// calls, such as `fdc3.open` or `fdc3.raiseIntent` at specific applications or application
    /// instances.
    ///
    /// Will always include at least an `appId` field, which uniquely identifies a specific app.
    ///
    /// If the `instanceId` field is set then the `AppMetadata` object represents a specific
    /// instance of the application that may be addressed using that Id.
    ///
    /// Field that represents the source application that a request or response was received
    /// from.
    ///
    /// Identifier for the app instance that was selected (or started) to resolve the intent.
    /// `source.instanceId` MUST be set, indicating the specific app instance that
    /// received the intent.
    /// </summary>
    public partial class AppDestinationIdentifier
    {
        /// <summary>
        /// Used in Desktop Agent Bridging to attribute or target a message to a
        /// particular Desktop Agent.
        ///
        /// The Desktop Agent that the app is available on. Used in Desktop Agent Bridging to
        /// identify the Desktop Agent to target.
        /// </summary>
        [JsonProperty("desktopAgent")]
        public string DesktopAgent { get; set; }

        /// <summary>
        /// The unique application identifier located within a specific application directory
        /// instance. An example of an appId might be 'app@sub.root'
        /// </summary>
        [JsonProperty("appId")]
        public string AppId { get; set; }

        /// <summary>
        /// An optional instance identifier, indicating that this object represents a specific
        /// instance of the application described.
        /// </summary>
        [JsonProperty("instanceId", NullValueHandling = NullValueHandling.Ignore)]
        public string InstanceId { get; set; }
    }

    /// <summary>
    /// A response to a getAppMetadata request.
    ///
    /// A response message from a Desktop Agent to the Bridge.
    /// </summary>
    public partial class GetAppMetadataAgentResponse
    {
        [JsonProperty("meta")]
        public GetAppMetadataAgentResponseMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains return values for FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public GetAppMetadataAgentResponsePayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Response' appended.
        /// </summary>
        [JsonProperty("type")]
        public ResponseMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for a response messages sent by a Desktop Agent to the Bridge
    /// </summary>
    public partial class GetAppMetadataAgentResponseMeta
    {
        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload typically contains return values for FDC3 API functions.
    /// </summary>
    public partial class GetAppMetadataAgentResponsePayload
    {
        [JsonProperty("appMetadata")]
        public AppMetadata AppMetadata { get; set; }
    }

    /// <summary>
    /// A response to a getAppMetadata request that contains an error.
    ///
    /// A response message from the Bridge back to the original Desktop Agent that raised the
    /// request, used where all connected agents returned errors.
    /// </summary>
    public partial class GetAppMetadataBridgeErrorResponse
    {
        [JsonProperty("meta")]
        public GetAppMetadataBridgeErrorResponseMeta Meta { get; set; }

        /// <summary>
        /// The error message payload contains details of an error return to the app or agent that
        /// raised the original request.
        /// </summary>
        [JsonProperty("payload")]
        public GetAppMetadataBridgeErrorResponsePayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Response' appended.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Metadata required in a response message collated and/or forwarded on by the Bridge
    /// </summary>
    public partial class GetAppMetadataBridgeErrorResponseMeta
    {
        [JsonProperty("errorDetails")]
        public ResponseErrorDetail[] ErrorDetails { get; set; }

        [JsonProperty("errorSources")]
        public DesktopAgentIdentifier[] ErrorSources { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The error message payload contains details of an error return to the app or agent that
    /// raised the original request.
    /// </summary>
    public partial class GetAppMetadataBridgeErrorResponsePayload
    {
        [JsonProperty("error")]
        public ErrorMessage Error { get; set; }
    }

    /// <summary>
    /// A request for metadata about an app
    ///
    /// A request message forwarded from the Bridge onto a Desktop Agent connected to it.
    /// </summary>
    public partial class GetAppMetadataBridgeRequest
    {
        [JsonProperty("meta")]
        public GetAppMetadataBridgeRequestMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains the arguments to FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public GetAppMetadataBridgeRequestPayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Request' appended.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Metadata required in a request message forwarded on by the Bridge
    /// </summary>
    public partial class GetAppMetadataBridgeRequestMeta
    {
        /// <summary>
        /// Optional field that represents the destination that the request should be routed to. Must
        /// be set by the Desktop Agent for API calls that include a target app parameter and must
        /// include the name of the Desktop Agent hosting the target application.
        /// </summary>
        [JsonProperty("destination", NullValueHandling = NullValueHandling.Ignore)]
        public DestinationClass Destination { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        /// <summary>
        /// Field that represents the source application that the request was received from, or the
        /// source Desktop Agent if it issued the request itself. The Desktop Agent identifier MUST
        /// be set by the bridge.
        /// </summary>
        [JsonProperty("source")]
        public MetaSourceClass Source { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload typically contains the arguments to FDC3 API functions.
    /// </summary>
    public partial class GetAppMetadataBridgeRequestPayload
    {
        [JsonProperty("app")]
        public AppDestinationIdentifier App { get; set; }
    }

    /// <summary>
    /// A response to a getAppMetadata request.
    ///
    /// A response message from the Bridge back to the original Desktop Agent that raised the
    /// request.
    /// </summary>
    public partial class GetAppMetadataBridgeResponse
    {
        [JsonProperty("meta")]
        public GetAppMetadataBridgeResponseMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains return values for FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public GetAppMetadataBridgeResponsePayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Response' appended.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Metadata required in a response message collated and/or forwarded on by the Bridge
    /// </summary>
    public partial class GetAppMetadataBridgeResponseMeta
    {
        [JsonProperty("errorDetails", NullValueHandling = NullValueHandling.Ignore)]
        public ResponseErrorDetail[] ErrorDetails { get; set; }

        [JsonProperty("errorSources", NullValueHandling = NullValueHandling.Ignore)]
        public DesktopAgentIdentifier[] ErrorSources { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("sources", NullValueHandling = NullValueHandling.Ignore)]
        public DesktopAgentIdentifier[] Sources { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload typically contains return values for FDC3 API functions.
    /// </summary>
    public partial class GetAppMetadataBridgeResponsePayload
    {
        [JsonProperty("appMetadata")]
        public AppMetadata AppMetadata { get; set; }
    }

    /// <summary>
    /// A response to an open request that contains an error
    ///
    /// A response message from a Desktop Agent to the Bridge containing an error, to be used in
    /// preference to the standard response when an error needs to be returned.
    /// </summary>
    public partial class OpenAgentErrorResponse
    {
        [JsonProperty("meta")]
        public OpenAgentErrorResponseMeta Meta { get; set; }

        /// <summary>
        /// Error message payload containing an standardized error string.
        /// </summary>
        [JsonProperty("payload")]
        public OpenAgentErrorResponsePayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Response' appended.
        /// </summary>
        [JsonProperty("type")]
        public ResponseMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for a response messages sent by a Desktop Agent to the Bridge
    /// </summary>
    public partial class OpenAgentErrorResponseMeta
    {
        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// Error message payload containing an standardized error string.
    /// </summary>
    public partial class OpenAgentErrorResponsePayload
    {
        [JsonProperty("error")]
        public OpenErrorMessage Error { get; set; }
    }

    /// <summary>
    /// A request to open an application
    ///
    /// A request message from a Desktop Agent to the Bridge.
    /// </summary>
    public partial class OpenAgentRequest
    {
        [JsonProperty("meta")]
        public OpenAgentRequestMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains the arguments to FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public OpenAgentRequestPayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Request' appended.
        /// </summary>
        [JsonProperty("type")]
        public RequestMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for a request message sent by Desktop Agents to the Bridge.
    /// </summary>
    public partial class OpenAgentRequestMeta
    {
        /// <summary>
        /// Optional field that represents the destination that the request should be routed to. Must
        /// be set by the Desktop Agent for API calls that include a target app parameter and must
        /// include the name of the Desktop Agent hosting the target application.
        /// </summary>
        [JsonProperty("destination", NullValueHandling = NullValueHandling.Ignore)]
        public DestinationClass Destination { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        /// <summary>
        /// Field that represents the source application that the request was received from, or the
        /// source Desktop Agent if it issued the request itself.
        /// </summary>
        [JsonProperty("source")]
        public SourceClass Source { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload typically contains the arguments to FDC3 API functions.
    /// </summary>
    public partial class OpenAgentRequestPayload
    {
        /// <summary>
        /// The application to open on the specified Desktop Agent
        /// </summary>
        [JsonProperty("app")]
        public AppToOpen App { get; set; }

        [JsonProperty("context", NullValueHandling = NullValueHandling.Ignore)]
        public ContextElement Context { get; set; }
    }

    /// <summary>
    /// The application to open on the specified Desktop Agent
    ///
    /// Identifies a particular Desktop Agent in Desktop Agent Bridging scenarios
    /// where a request needs to be directed to a Desktop Agent rather than a specific app, or a
    /// response message is returned by the Desktop Agent (or more specifically its resolver)
    /// rather than a specific app. Used as a substitute for `AppIdentifier` in cases where no
    /// app details are available or are appropriate.
    ///
    /// Array of DesktopAgentIdentifiers for responses that were not returned to the bridge
    /// before the timeout or because an error occurred. May be omitted if all sources responded
    /// without errors. MUST include the `desktopAgent` field when returned by the bridge.
    ///
    /// Array of DesktopAgentIdentifiers for the sources that generated responses to the request.
    /// Will contain a single value for individual responses and multiple values for responses
    /// that were collated by the bridge. May be omitted if all sources errored. MUST include the
    /// `desktopAgent` field when returned by the bridge.
    ///
    /// Field that represents a destination Desktop Agent that a request is to be sent to.
    ///
    /// Identifies an application, or instance of an application, and is used to target FDC3 API
    /// calls, such as `fdc3.open` or `fdc3.raiseIntent` at specific applications or application
    /// instances.
    ///
    /// Will always include at least an `appId` field, which uniquely identifies a specific app.
    ///
    /// If the `instanceId` field is set then the `AppMetadata` object represents a specific
    /// instance of the application that may be addressed using that Id.
    ///
    /// Field that represents the source application that a request or response was received
    /// from.
    ///
    /// Identifier for the app instance that was selected (or started) to resolve the intent.
    /// `source.instanceId` MUST be set, indicating the specific app instance that
    /// received the intent.
    /// </summary>
    public partial class AppToOpen
    {
        /// <summary>
        /// Used in Desktop Agent Bridging to attribute or target a message to a
        /// particular Desktop Agent.
        ///
        /// The Desktop Agent that the app is available on. Used in Desktop Agent Bridging to
        /// identify the Desktop Agent to target.
        /// </summary>
        [JsonProperty("desktopAgent")]
        public string DesktopAgent { get; set; }

        /// <summary>
        /// The unique application identifier located within a specific application directory
        /// instance. An example of an appId might be 'app@sub.root'
        /// </summary>
        [JsonProperty("appId")]
        public string AppId { get; set; }

        /// <summary>
        /// An optional instance identifier, indicating that this object represents a specific
        /// instance of the application described.
        /// </summary>
        [JsonProperty("instanceId", NullValueHandling = NullValueHandling.Ignore)]
        public string InstanceId { get; set; }
    }

    /// <summary>
    /// A response to an open request
    ///
    /// A response message from a Desktop Agent to the Bridge.
    /// </summary>
    public partial class OpenAgentResponse
    {
        [JsonProperty("meta")]
        public OpenAgentResponseMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains return values for FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public OpenAgentResponsePayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Response' appended.
        /// </summary>
        [JsonProperty("type")]
        public ResponseMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for a response messages sent by a Desktop Agent to the Bridge
    /// </summary>
    public partial class OpenAgentResponseMeta
    {
        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload typically contains return values for FDC3 API functions.
    /// </summary>
    public partial class OpenAgentResponsePayload
    {
        [JsonProperty("appIdentifier")]
        public AppIdentifier AppIdentifier { get; set; }
    }

    /// <summary>
    /// A response to an open request that contains an error
    ///
    /// A response message from the Bridge back to the original Desktop Agent that raised the
    /// request, used where all connected agents returned errors.
    /// </summary>
    public partial class OpenBridgeErrorResponse
    {
        [JsonProperty("meta")]
        public OpenBridgeErrorResponseMeta Meta { get; set; }

        /// <summary>
        /// The error message payload contains details of an error return to the app or agent that
        /// raised the original request.
        /// </summary>
        [JsonProperty("payload")]
        public OpenBridgeErrorResponsePayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Response' appended.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Metadata required in a response message collated and/or forwarded on by the Bridge
    /// </summary>
    public partial class OpenBridgeErrorResponseMeta
    {
        [JsonProperty("errorDetails")]
        public ResponseErrorDetail[] ErrorDetails { get; set; }

        [JsonProperty("errorSources")]
        public DesktopAgentIdentifier[] ErrorSources { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The error message payload contains details of an error return to the app or agent that
    /// raised the original request.
    /// </summary>
    public partial class OpenBridgeErrorResponsePayload
    {
        [JsonProperty("error")]
        public OpenErrorMessage Error { get; set; }
    }

    /// <summary>
    /// A request to open an application
    ///
    /// A request message forwarded from the Bridge onto a Desktop Agent connected to it.
    /// </summary>
    public partial class OpenBridgeRequest
    {
        [JsonProperty("meta")]
        public OpenBridgeRequestMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains the arguments to FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public OpenBridgeRequestPayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Request' appended.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Metadata required in a request message forwarded on by the Bridge
    /// </summary>
    public partial class OpenBridgeRequestMeta
    {
        /// <summary>
        /// Optional field that represents the destination that the request should be routed to. Must
        /// be set by the Desktop Agent for API calls that include a target app parameter and must
        /// include the name of the Desktop Agent hosting the target application.
        /// </summary>
        [JsonProperty("destination", NullValueHandling = NullValueHandling.Ignore)]
        public DestinationClass Destination { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        /// <summary>
        /// Field that represents the source application that the request was received from, or the
        /// source Desktop Agent if it issued the request itself. The Desktop Agent identifier MUST
        /// be set by the bridge.
        /// </summary>
        [JsonProperty("source")]
        public MetaSource Source { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload typically contains the arguments to FDC3 API functions.
    /// </summary>
    public partial class OpenBridgeRequestPayload
    {
        /// <summary>
        /// The application to open on the specified Desktop Agent
        /// </summary>
        [JsonProperty("app")]
        public AppToOpen App { get; set; }

        [JsonProperty("context", NullValueHandling = NullValueHandling.Ignore)]
        public ContextElement Context { get; set; }
    }

    /// <summary>
    /// A response to an open request
    ///
    /// A response message from the Bridge back to the original Desktop Agent that raised the
    /// request.
    /// </summary>
    public partial class OpenBridgeResponse
    {
        [JsonProperty("meta")]
        public OpenBridgeResponseMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains return values for FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public OpenBridgeResponsePayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Response' appended.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Metadata required in a response message collated and/or forwarded on by the Bridge
    /// </summary>
    public partial class OpenBridgeResponseMeta
    {
        [JsonProperty("errorDetails", NullValueHandling = NullValueHandling.Ignore)]
        public ResponseErrorDetail[] ErrorDetails { get; set; }

        [JsonProperty("errorSources", NullValueHandling = NullValueHandling.Ignore)]
        public DesktopAgentIdentifier[] ErrorSources { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("sources", NullValueHandling = NullValueHandling.Ignore)]
        public DesktopAgentIdentifier[] Sources { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload typically contains return values for FDC3 API functions.
    /// </summary>
    public partial class OpenBridgeResponsePayload
    {
        [JsonProperty("appIdentifier")]
        public AppIdentifier AppIdentifier { get; set; }
    }

    /// <summary>
    /// A request to broadcast on a PrivateChannel.
    ///
    /// A request message from a Desktop Agent to the Bridge.
    /// </summary>
    public partial class PrivateChannelBroadcastAgentRequest
    {
        [JsonProperty("meta")]
        public PrivateChannelBroadcastAgentRequestMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains the arguments to FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public PrivateChannelBroadcastAgentRequestPayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Request' appended.
        /// </summary>
        [JsonProperty("type")]
        public RequestMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for a request message sent by Desktop Agents to the Bridge.
    /// </summary>
    public partial class PrivateChannelBroadcastAgentRequestMeta
    {
        /// <summary>
        /// Optional field that represents the destination that the request should be routed to. Must
        /// be set by the Desktop Agent for API calls that include a target app parameter and must
        /// include the name of the Desktop Agent hosting the target application.
        /// </summary>
        [JsonProperty("destination", NullValueHandling = NullValueHandling.Ignore)]
        public MetaDestination Destination { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        /// <summary>
        /// Field that represents the source application that the request was received from, or the
        /// source Desktop Agent if it issued the request itself.
        /// </summary>
        [JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
        public SourceClass Source { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// Field that represents a destination App on a remote Desktop Agent that a request is to be
    /// sent to.
    ///
    /// Identifies a particular Desktop Agent in Desktop Agent Bridging scenarios
    /// where a request needs to be directed to a Desktop Agent rather than a specific app, or a
    /// response message is returned by the Desktop Agent (or more specifically its resolver)
    /// rather than a specific app. Used as a substitute for `AppIdentifier` in cases where no
    /// app details are available or are appropriate.
    ///
    /// Array of DesktopAgentIdentifiers for responses that were not returned to the bridge
    /// before the timeout or because an error occurred. May be omitted if all sources responded
    /// without errors. MUST include the `desktopAgent` field when returned by the bridge.
    ///
    /// Array of DesktopAgentIdentifiers for the sources that generated responses to the request.
    /// Will contain a single value for individual responses and multiple values for responses
    /// that were collated by the bridge. May be omitted if all sources errored. MUST include the
    /// `desktopAgent` field when returned by the bridge.
    ///
    /// Field that represents a destination Desktop Agent that a request is to be sent to.
    ///
    /// Identifies an application, or instance of an application, and is used to target FDC3 API
    /// calls, such as `fdc3.open` or `fdc3.raiseIntent` at specific applications or application
    /// instances.
    ///
    /// Will always include at least an `appId` field, which uniquely identifies a specific app.
    ///
    /// If the `instanceId` field is set then the `AppMetadata` object represents a specific
    /// instance of the application that may be addressed using that Id.
    ///
    /// Field that represents the source application that a request or response was received
    /// from.
    ///
    /// Identifier for the app instance that was selected (or started) to resolve the intent.
    /// `source.instanceId` MUST be set, indicating the specific app instance that
    /// received the intent.
    ///
    /// Optional field that represents the destination that the request should be routed to. Must
    /// be set by the Desktop Agent for API calls that include a target app parameter and must
    /// include the name of the Desktop Agent hosting the target application.
    ///
    /// Represents identifiers that MUST include the Desktop Agent name and MAY identify a
    /// specific app or instance.
    ///
    /// Field that represents the source application that the request was received from, or the
    /// source Desktop Agent if it issued the request itself. The Desktop Agent identifier MUST
    /// be set by the bridge.
    /// </summary>
    public partial class MetaDestination
    {
        /// <summary>
        /// Used in Desktop Agent Bridging to attribute or target a message to a
        /// particular Desktop Agent.
        ///
        /// The Desktop Agent that the app is available on. Used in Desktop Agent Bridging to
        /// identify the Desktop Agent to target.
        /// </summary>
        [JsonProperty("desktopAgent")]
        public string DesktopAgent { get; set; }

        /// <summary>
        /// The unique application identifier located within a specific application directory
        /// instance. An example of an appId might be 'app@sub.root'
        /// </summary>
        [JsonProperty("appId")]
        public string AppId { get; set; }

        /// <summary>
        /// An optional instance identifier, indicating that this object represents a specific
        /// instance of the application described.
        /// </summary>
        [JsonProperty("instanceId", NullValueHandling = NullValueHandling.Ignore)]
        public string InstanceId { get; set; }
    }

    /// <summary>
    /// The message payload typically contains the arguments to FDC3 API functions.
    /// </summary>
    public partial class PrivateChannelBroadcastAgentRequestPayload
    {
        /// <summary>
        /// The Id of the PrivateChannel that the broadcast was sent on
        /// </summary>
        [JsonProperty("channelId")]
        public string ChannelId { get; set; }

        /// <summary>
        /// The context object that was the payload of a broadcast message.
        /// </summary>
        [JsonProperty("context")]
        public ContextElement Context { get; set; }
    }

    /// <summary>
    /// A request to broadcast on a PrivateChannel.
    ///
    /// A request message forwarded from the Bridge onto a Desktop Agent connected to it.
    /// </summary>
    public partial class PrivateChannelBroadcastBridgeRequest
    {
        [JsonProperty("meta")]
        public PrivateChannelBroadcastBridgeRequestMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains the arguments to FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public PrivateChannelBroadcastBridgeRequestPayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Request' appended.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Metadata required in a request message forwarded on by the Bridge
    /// </summary>
    public partial class PrivateChannelBroadcastBridgeRequestMeta
    {
        /// <summary>
        /// Optional field that represents the destination that the request should be routed to. Must
        /// be set by the Desktop Agent for API calls that include a target app parameter and must
        /// include the name of the Desktop Agent hosting the target application.
        /// </summary>
        [JsonProperty("destination", NullValueHandling = NullValueHandling.Ignore)]
        public MetaDestination Destination { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        /// <summary>
        /// Field that represents the source application that the request was received from, or the
        /// source Desktop Agent if it issued the request itself. The Desktop Agent identifier MUST
        /// be set by the bridge.
        /// </summary>
        [JsonProperty("source")]
        public MetaSource Source { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload typically contains the arguments to FDC3 API functions.
    /// </summary>
    public partial class PrivateChannelBroadcastBridgeRequestPayload
    {
        /// <summary>
        /// The Id of the PrivateChannel that the broadcast was sent on
        /// </summary>
        [JsonProperty("channelId")]
        public string ChannelId { get; set; }

        /// <summary>
        /// The context object that was the payload of a broadcast message.
        /// </summary>
        [JsonProperty("context")]
        public ContextElement Context { get; set; }
    }

    /// <summary>
    /// A request to forward on an EventListenerAdded event, relating to a PrivateChannel
    ///
    /// A request message from a Desktop Agent to the Bridge.
    /// </summary>
    public partial class PrivateChannelEventListenerAddedAgentRequest
    {
        [JsonProperty("meta")]
        public PrivateChannelEventListenerAddedAgentRequestMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains the arguments to FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public PrivateChannelEventListenerAddedAgentRequestPayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Request' appended.
        /// </summary>
        [JsonProperty("type")]
        public RequestMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for a request message sent by Desktop Agents to the Bridge.
    /// </summary>
    public partial class PrivateChannelEventListenerAddedAgentRequestMeta
    {
        /// <summary>
        /// Optional field that represents the destination that the request should be routed to. Must
        /// be set by the Desktop Agent for API calls that include a target app parameter and must
        /// include the name of the Desktop Agent hosting the target application.
        /// </summary>
        [JsonProperty("destination", NullValueHandling = NullValueHandling.Ignore)]
        public MetaDestination Destination { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        /// <summary>
        /// Field that represents the source application that the request was received from, or the
        /// source Desktop Agent if it issued the request itself.
        /// </summary>
        [JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
        public SourceClass Source { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload typically contains the arguments to FDC3 API functions.
    /// </summary>
    public partial class PrivateChannelEventListenerAddedAgentRequestPayload
    {
        [JsonProperty("channelId")]
        public string ChannelId { get; set; }

        [JsonProperty("listenerType")]
        public Empty ListenerType { get; set; }
    }

    /// <summary>
    /// A request to forward on an EventListenerAdded event, relating to a PrivateChannel
    ///
    /// A request message forwarded from the Bridge onto a Desktop Agent connected to it.
    /// </summary>
    public partial class PrivateChannelEventListenerAddedBridgeRequest
    {
        [JsonProperty("meta")]
        public PrivateChannelEventListenerAddedBridgeRequestMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains the arguments to FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public PrivateChannelEventListenerAddedBridgeRequestPayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Request' appended.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Metadata required in a request message forwarded on by the Bridge
    /// </summary>
    public partial class PrivateChannelEventListenerAddedBridgeRequestMeta
    {
        /// <summary>
        /// Optional field that represents the destination that the request should be routed to. Must
        /// be set by the Desktop Agent for API calls that include a target app parameter and must
        /// include the name of the Desktop Agent hosting the target application.
        /// </summary>
        [JsonProperty("destination", NullValueHandling = NullValueHandling.Ignore)]
        public MetaDestination Destination { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        /// <summary>
        /// Field that represents the source application that the request was received from, or the
        /// source Desktop Agent if it issued the request itself. The Desktop Agent identifier MUST
        /// be set by the bridge.
        /// </summary>
        [JsonProperty("source")]
        public MetaSource Source { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload typically contains the arguments to FDC3 API functions.
    /// </summary>
    public partial class PrivateChannelEventListenerAddedBridgeRequestPayload
    {
        [JsonProperty("channelId")]
        public string ChannelId { get; set; }

        [JsonProperty("listenerType")]
        public Empty ListenerType { get; set; }
    }

    /// <summary>
    /// A request to forward on an EventListenerRemoved event, relating to a PrivateChannel
    ///
    /// A request message from a Desktop Agent to the Bridge.
    /// </summary>
    public partial class PrivateChannelEventListenerRemovedAgentRequest
    {
        [JsonProperty("meta")]
        public PrivateChannelEventListenerRemovedAgentRequestMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains the arguments to FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public PrivateChannelEventListenerRemovedAgentRequestPayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Request' appended.
        /// </summary>
        [JsonProperty("type")]
        public RequestMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for a request message sent by Desktop Agents to the Bridge.
    /// </summary>
    public partial class PrivateChannelEventListenerRemovedAgentRequestMeta
    {
        /// <summary>
        /// Optional field that represents the destination that the request should be routed to. Must
        /// be set by the Desktop Agent for API calls that include a target app parameter and must
        /// include the name of the Desktop Agent hosting the target application.
        /// </summary>
        [JsonProperty("destination", NullValueHandling = NullValueHandling.Ignore)]
        public MetaDestination Destination { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        /// <summary>
        /// Field that represents the source application that the request was received from, or the
        /// source Desktop Agent if it issued the request itself.
        /// </summary>
        [JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
        public SourceClass Source { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload typically contains the arguments to FDC3 API functions.
    /// </summary>
    public partial class PrivateChannelEventListenerRemovedAgentRequestPayload
    {
        [JsonProperty("channelId")]
        public string ChannelId { get; set; }

        [JsonProperty("listenerType")]
        public Empty ListenerType { get; set; }
    }

    /// <summary>
    /// A request to forward on an EventListenerRemoved event, relating to a PrivateChannel
    ///
    /// A request message forwarded from the Bridge onto a Desktop Agent connected to it.
    /// </summary>
    public partial class PrivateChannelEventListenerRemovedBridgeRequest
    {
        [JsonProperty("meta")]
        public PrivateChannelEventListenerRemovedBridgeRequestMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains the arguments to FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public PrivateChannelEventListenerRemovedBridgeRequestPayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Request' appended.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Metadata required in a request message forwarded on by the Bridge
    /// </summary>
    public partial class PrivateChannelEventListenerRemovedBridgeRequestMeta
    {
        /// <summary>
        /// Optional field that represents the destination that the request should be routed to. Must
        /// be set by the Desktop Agent for API calls that include a target app parameter and must
        /// include the name of the Desktop Agent hosting the target application.
        /// </summary>
        [JsonProperty("destination", NullValueHandling = NullValueHandling.Ignore)]
        public MetaDestination Destination { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        /// <summary>
        /// Field that represents the source application that the request was received from, or the
        /// source Desktop Agent if it issued the request itself. The Desktop Agent identifier MUST
        /// be set by the bridge.
        /// </summary>
        [JsonProperty("source")]
        public MetaSource Source { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload typically contains the arguments to FDC3 API functions.
    /// </summary>
    public partial class PrivateChannelEventListenerRemovedBridgeRequestPayload
    {
        [JsonProperty("channelId")]
        public string ChannelId { get; set; }

        [JsonProperty("listenerType")]
        public Empty ListenerType { get; set; }
    }

    /// <summary>
    /// A request to forward on an AddContextListener event, relating to a PrivateChannel
    ///
    /// A request message from a Desktop Agent to the Bridge.
    /// </summary>
    public partial class PrivateChannelOnAddContextListenerAgentRequest
    {
        [JsonProperty("meta")]
        public PrivateChannelOnAddContextListenerAgentRequestMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains the arguments to FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public PrivateChannelOnAddContextListenerAgentRequestPayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Request' appended.
        /// </summary>
        [JsonProperty("type")]
        public RequestMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for a request message sent by Desktop Agents to the Bridge.
    /// </summary>
    public partial class PrivateChannelOnAddContextListenerAgentRequestMeta
    {
        /// <summary>
        /// Optional field that represents the destination that the request should be routed to. Must
        /// be set by the Desktop Agent for API calls that include a target app parameter and must
        /// include the name of the Desktop Agent hosting the target application.
        /// </summary>
        [JsonProperty("destination", NullValueHandling = NullValueHandling.Ignore)]
        public MetaDestination Destination { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        /// <summary>
        /// Field that represents the source application that the request was received from, or the
        /// source Desktop Agent if it issued the request itself.
        /// </summary>
        [JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
        public SourceClass Source { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload typically contains the arguments to FDC3 API functions.
    /// </summary>
    public partial class PrivateChannelOnAddContextListenerAgentRequestPayload
    {
        [JsonProperty("channelId")]
        public string ChannelId { get; set; }

        [JsonProperty("contextType")]
        public string ContextType { get; set; }
    }

    /// <summary>
    /// A request to forward on an AddContextListener event, relating to a PrivateChannel
    ///
    /// A request message forwarded from the Bridge onto a Desktop Agent connected to it.
    /// </summary>
    public partial class PrivateChannelOnAddContextListenerBridgeRequest
    {
        [JsonProperty("meta")]
        public PrivateChannelOnAddContextListenerBridgeRequestMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains the arguments to FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public PrivateChannelOnAddContextListenerBridgeRequestPayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Request' appended.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Metadata required in a request message forwarded on by the Bridge
    /// </summary>
    public partial class PrivateChannelOnAddContextListenerBridgeRequestMeta
    {
        /// <summary>
        /// Optional field that represents the destination that the request should be routed to. Must
        /// be set by the Desktop Agent for API calls that include a target app parameter and must
        /// include the name of the Desktop Agent hosting the target application.
        /// </summary>
        [JsonProperty("destination", NullValueHandling = NullValueHandling.Ignore)]
        public MetaDestination Destination { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        /// <summary>
        /// Field that represents the source application that the request was received from, or the
        /// source Desktop Agent if it issued the request itself. The Desktop Agent identifier MUST
        /// be set by the bridge.
        /// </summary>
        [JsonProperty("source")]
        public MetaSource Source { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload typically contains the arguments to FDC3 API functions.
    /// </summary>
    public partial class PrivateChannelOnAddContextListenerBridgeRequestPayload
    {
        [JsonProperty("channelId")]
        public string ChannelId { get; set; }

        [JsonProperty("contextType")]
        public string ContextType { get; set; }
    }

    /// <summary>
    /// A request to forward on a Disconnect event, relating to a PrivateChannel
    ///
    /// A request message from a Desktop Agent to the Bridge.
    /// </summary>
    public partial class PrivateChannelOnDisconnectAgentRequest
    {
        [JsonProperty("meta")]
        public PrivateChannelOnDisconnectAgentRequestMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains the arguments to FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public PrivateChannelOnDisconnectAgentRequestPayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Request' appended.
        /// </summary>
        [JsonProperty("type")]
        public RequestMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for a request message sent by Desktop Agents to the Bridge.
    /// </summary>
    public partial class PrivateChannelOnDisconnectAgentRequestMeta
    {
        /// <summary>
        /// Optional field that represents the destination that the request should be routed to. Must
        /// be set by the Desktop Agent for API calls that include a target app parameter and must
        /// include the name of the Desktop Agent hosting the target application.
        /// </summary>
        [JsonProperty("destination", NullValueHandling = NullValueHandling.Ignore)]
        public MetaDestination Destination { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        /// <summary>
        /// Field that represents the source application that the request was received from, or the
        /// source Desktop Agent if it issued the request itself.
        /// </summary>
        [JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
        public SourceClass Source { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload typically contains the arguments to FDC3 API functions.
    /// </summary>
    public partial class PrivateChannelOnDisconnectAgentRequestPayload
    {
        [JsonProperty("channelId")]
        public string ChannelId { get; set; }
    }

    /// <summary>
    /// A request to forward on a Disconnect event, relating to a PrivateChannel
    ///
    /// A request message forwarded from the Bridge onto a Desktop Agent connected to it.
    /// </summary>
    public partial class PrivateChannelOnDisconnectBridgeRequest
    {
        [JsonProperty("meta")]
        public PrivateChannelOnDisconnectBridgeRequestMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains the arguments to FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public PrivateChannelOnDisconnectBridgeRequestPayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Request' appended.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Metadata required in a request message forwarded on by the Bridge
    /// </summary>
    public partial class PrivateChannelOnDisconnectBridgeRequestMeta
    {
        /// <summary>
        /// Optional field that represents the destination that the request should be routed to. Must
        /// be set by the Desktop Agent for API calls that include a target app parameter and must
        /// include the name of the Desktop Agent hosting the target application.
        /// </summary>
        [JsonProperty("destination", NullValueHandling = NullValueHandling.Ignore)]
        public MetaDestination Destination { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        /// <summary>
        /// Field that represents the source application that the request was received from, or the
        /// source Desktop Agent if it issued the request itself. The Desktop Agent identifier MUST
        /// be set by the bridge.
        /// </summary>
        [JsonProperty("source")]
        public MetaSource Source { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload typically contains the arguments to FDC3 API functions.
    /// </summary>
    public partial class PrivateChannelOnDisconnectBridgeRequestPayload
    {
        [JsonProperty("channelId")]
        public string ChannelId { get; set; }
    }

    /// <summary>
    /// A request to forward on an Unsubscribe event, relating to a PrivateChannel
    ///
    /// A request message from a Desktop Agent to the Bridge.
    /// </summary>
    public partial class PrivateChannelOnUnsubscribeAgentRequest
    {
        [JsonProperty("meta")]
        public PrivateChannelOnUnsubscribeAgentRequestMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains the arguments to FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public PrivateChannelOnUnsubscribeAgentRequestPayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Request' appended.
        /// </summary>
        [JsonProperty("type")]
        public RequestMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for a request message sent by Desktop Agents to the Bridge.
    /// </summary>
    public partial class PrivateChannelOnUnsubscribeAgentRequestMeta
    {
        /// <summary>
        /// Optional field that represents the destination that the request should be routed to. Must
        /// be set by the Desktop Agent for API calls that include a target app parameter and must
        /// include the name of the Desktop Agent hosting the target application.
        /// </summary>
        [JsonProperty("destination", NullValueHandling = NullValueHandling.Ignore)]
        public MetaDestination Destination { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        /// <summary>
        /// Field that represents the source application that the request was received from, or the
        /// source Desktop Agent if it issued the request itself.
        /// </summary>
        [JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
        public SourceClass Source { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload typically contains the arguments to FDC3 API functions.
    /// </summary>
    public partial class PrivateChannelOnUnsubscribeAgentRequestPayload
    {
        [JsonProperty("channelId")]
        public string ChannelId { get; set; }

        [JsonProperty("contextType")]
        public string ContextType { get; set; }
    }

    /// <summary>
    /// A request to forward on an Unsubscribe event, relating to a PrivateChannel
    ///
    /// A request message forwarded from the Bridge onto a Desktop Agent connected to it.
    /// </summary>
    public partial class PrivateChannelOnUnsubscribeBridgeRequest
    {
        [JsonProperty("meta")]
        public ERequestMetadata Meta { get; set; }

        /// <summary>
        /// The message payload typically contains the arguments to FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public PrivateChannelOnUnsubscribeBridgeRequestPayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Request' appended.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Metadata required in a request message forwarded on by the Bridge
    /// </summary>
    public partial class ERequestMetadata
    {
        /// <summary>
        /// Optional field that represents the destination that the request should be routed to. Must
        /// be set by the Desktop Agent for API calls that include a target app parameter and must
        /// include the name of the Desktop Agent hosting the target application.
        /// </summary>
        [JsonProperty("destination", NullValueHandling = NullValueHandling.Ignore)]
        public MetaDestination Destination { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        /// <summary>
        /// Field that represents the source application that the request was received from, or the
        /// source Desktop Agent if it issued the request itself. The Desktop Agent identifier MUST
        /// be set by the bridge.
        /// </summary>
        [JsonProperty("source")]
        public MetaSource Source { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload typically contains the arguments to FDC3 API functions.
    /// </summary>
    public partial class PrivateChannelOnUnsubscribeBridgeRequestPayload
    {
        [JsonProperty("channelId")]
        public string ChannelId { get; set; }

        [JsonProperty("contextType")]
        public string ContextType { get; set; }
    }

    /// <summary>
    /// A response to a request to raise an intent that contains an error.
    ///
    /// A response message from a Desktop Agent to the Bridge containing an error, to be used in
    /// preference to the standard response when an error needs to be returned.
    /// </summary>
    public partial class RaiseIntentAgentErrorResponse
    {
        [JsonProperty("meta")]
        public RaiseIntentAgentErrorResponseMeta Meta { get; set; }

        /// <summary>
        /// Error message payload containing an standardized error string.
        /// </summary>
        [JsonProperty("payload")]
        public RaiseIntentAgentErrorResponsePayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Response' appended.
        /// </summary>
        [JsonProperty("type")]
        public ResponseMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for a response messages sent by a Desktop Agent to the Bridge
    /// </summary>
    public partial class RaiseIntentAgentErrorResponseMeta
    {
        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// Error message payload containing an standardized error string.
    /// </summary>
    public partial class RaiseIntentAgentErrorResponsePayload
    {
        [JsonProperty("error")]
        public ErrorMessage Error { get; set; }
    }

    /// <summary>
    /// A request to raise an intent.
    ///
    /// A request message from a Desktop Agent to the Bridge.
    /// </summary>
    public partial class RaiseIntentAgentRequest
    {
        [JsonProperty("meta")]
        public RaiseIntentAgentRequestMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains the arguments to FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public RaiseIntentAgentRequestPayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Request' appended.
        /// </summary>
        [JsonProperty("type")]
        public RequestMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for a request message sent by Desktop Agents to the Bridge.
    /// </summary>
    public partial class RaiseIntentAgentRequestMeta
    {
        /// <summary>
        /// Optional field that represents the destination that the request should be routed to. Must
        /// be set by the Desktop Agent for API calls that include a target app parameter and must
        /// include the name of the Desktop Agent hosting the target application.
        /// </summary>
        [JsonProperty("destination")]
        public MetaDestination Destination { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        /// <summary>
        /// Field that represents the source application that the request was received from, or the
        /// source Desktop Agent if it issued the request itself.
        /// </summary>
        [JsonProperty("source")]
        public SourceClass Source { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload typically contains the arguments to FDC3 API functions.
    /// </summary>
    public partial class RaiseIntentAgentRequestPayload
    {
        [JsonProperty("app")]
        public AppDestinationIdentifier App { get; set; }

        [JsonProperty("context")]
        public ContextElement Context { get; set; }

        [JsonProperty("intent")]
        public string Intent { get; set; }
    }

    /// <summary>
    /// A response to a request to raise an intent.
    ///
    /// A response message from a Desktop Agent to the Bridge.
    /// </summary>
    public partial class RaiseIntentAgentResponse
    {
        [JsonProperty("meta")]
        public RaiseIntentAgentResponseMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains return values for FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public RaiseIntentAgentResponsePayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Response' appended.
        /// </summary>
        [JsonProperty("type")]
        public ResponseMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for a response messages sent by a Desktop Agent to the Bridge
    /// </summary>
    public partial class RaiseIntentAgentResponseMeta
    {
        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload typically contains return values for FDC3 API functions.
    /// </summary>
    public partial class RaiseIntentAgentResponsePayload
    {
        [JsonProperty("intentResolution")]
        public IntentResolution IntentResolution { get; set; }
    }

    /// <summary>
    /// IntentResolution provides a standard format for data returned upon resolving an intent.
    ///
    /// ```javascript
    /// //resolve a "Chain" type intent
    /// let resolution = await agent.raiseIntent("intentName", context);
    ///
    /// //resolve a "Client-Service" type intent with a data response or a Channel
    /// let resolution = await agent.raiseIntent("intentName", context);
    /// try {
    /// const result = await resolution.getResult();
    /// if (result && result.broadcast) {
    /// console.log(`${resolution.source} returned a channel with id ${result.id}`);
    /// } else if (result){
    /// console.log(`${resolution.source} returned data: ${JSON.stringify(result)}`);
    /// } else {
    /// console.error(`${resolution.source} didn't return data`
    /// }
    /// } catch(error) {
    /// console.error(`${resolution.source} returned an error: ${error}`);
    /// }
    ///
    /// // Use metadata about the resolving app instance to target a further intent
    /// await agent.raiseIntent("intentName", context, resolution.source);
    /// ```
    /// </summary>
    public partial class IntentResolution
    {
        /// <summary>
        /// The intent that was raised. May be used to determine which intent the user
        /// chose in response to `fdc3.raiseIntentForContext()`.
        /// </summary>
        [JsonProperty("intent")]
        public string Intent { get; set; }

        /// <summary>
        /// Identifier for the app instance that was selected (or started) to resolve the intent.
        /// `source.instanceId` MUST be set, indicating the specific app instance that
        /// received the intent.
        /// </summary>
        [JsonProperty("source")]
        public AppIdentifier Source { get; set; }

        /// <summary>
        /// The version number of the Intents schema being used.
        /// </summary>
        [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
        public string Version { get; set; }
    }

    /// <summary>
    /// A response to a request to raise an intent that contains an error.
    ///
    /// A response message from the Bridge back to the original Desktop Agent that raised the
    /// request, used where all connected agents returned errors.
    /// </summary>
    public partial class RaiseIntentBridgeErrorResponse
    {
        [JsonProperty("meta")]
        public RaiseIntentBridgeErrorResponseMeta Meta { get; set; }

        /// <summary>
        /// The error message payload contains details of an error return to the app or agent that
        /// raised the original request.
        /// </summary>
        [JsonProperty("payload")]
        public RaiseIntentBridgeErrorResponsePayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Response' appended.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Metadata required in a response message collated and/or forwarded on by the Bridge
    /// </summary>
    public partial class RaiseIntentBridgeErrorResponseMeta
    {
        [JsonProperty("errorDetails")]
        public ResponseErrorDetail[] ErrorDetails { get; set; }

        [JsonProperty("errorSources")]
        public DesktopAgentIdentifier[] ErrorSources { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The error message payload contains details of an error return to the app or agent that
    /// raised the original request.
    /// </summary>
    public partial class RaiseIntentBridgeErrorResponsePayload
    {
        [JsonProperty("error")]
        public ErrorMessage Error { get; set; }
    }

    /// <summary>
    /// A request to raise an intent.
    ///
    /// A request message forwarded from the Bridge onto a Desktop Agent connected to it.
    /// </summary>
    public partial class RaiseIntentBridgeRequest
    {
        [JsonProperty("meta")]
        public RaiseIntentBridgeRequestMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains the arguments to FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public RaiseIntentBridgeRequestPayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Request' appended.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Metadata required in a request message forwarded on by the Bridge
    /// </summary>
    public partial class RaiseIntentBridgeRequestMeta
    {
        /// <summary>
        /// Optional field that represents the destination that the request should be routed to. Must
        /// be set by the Desktop Agent for API calls that include a target app parameter and must
        /// include the name of the Desktop Agent hosting the target application.
        /// </summary>
        [JsonProperty("destination")]
        public MetaDestination Destination { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        /// <summary>
        /// Field that represents the source application that the request was received from, or the
        /// source Desktop Agent if it issued the request itself. The Desktop Agent identifier MUST
        /// be set by the bridge.
        /// </summary>
        [JsonProperty("source")]
        public MetaSource Source { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload typically contains the arguments to FDC3 API functions.
    /// </summary>
    public partial class RaiseIntentBridgeRequestPayload
    {
        [JsonProperty("app")]
        public AppDestinationIdentifier App { get; set; }

        [JsonProperty("context")]
        public ContextElement Context { get; set; }

        [JsonProperty("intent")]
        public string Intent { get; set; }
    }

    /// <summary>
    /// A response to a request to raise an intent.
    ///
    /// A response message from the Bridge back to the original Desktop Agent that raised the
    /// request.
    /// </summary>
    public partial class RaiseIntentBridgeResponse
    {
        [JsonProperty("meta")]
        public RaiseIntentBridgeResponseMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains return values for FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public RaiseIntentBridgeResponsePayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Response' appended.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Metadata required in a response message collated and/or forwarded on by the Bridge
    /// </summary>
    public partial class RaiseIntentBridgeResponseMeta
    {
        [JsonProperty("errorDetails", NullValueHandling = NullValueHandling.Ignore)]
        public ResponseErrorDetail[] ErrorDetails { get; set; }

        [JsonProperty("errorSources", NullValueHandling = NullValueHandling.Ignore)]
        public DesktopAgentIdentifier[] ErrorSources { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("sources", NullValueHandling = NullValueHandling.Ignore)]
        public DesktopAgentIdentifier[] Sources { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload typically contains return values for FDC3 API functions.
    /// </summary>
    public partial class RaiseIntentBridgeResponsePayload
    {
        [JsonProperty("intentResolution")]
        public IntentResolution IntentResolution { get; set; }
    }

    /// <summary>
    /// A secondary response to a request to raise an intent used to deliver the intent result,
    /// which contains an error
    ///
    /// A response message from a Desktop Agent to the Bridge containing an error, to be used in
    /// preference to the standard response when an error needs to be returned.
    /// </summary>
    public partial class RaiseIntentResultAgentErrorResponse
    {
        [JsonProperty("meta")]
        public RaiseIntentResultAgentErrorResponseMeta Meta { get; set; }

        /// <summary>
        /// Error message payload containing an standardized error string.
        /// </summary>
        [JsonProperty("payload")]
        public RaiseIntentResultAgentErrorResponsePayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Response' appended.
        /// </summary>
        [JsonProperty("type")]
        public ResponseMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for a response messages sent by a Desktop Agent to the Bridge
    /// </summary>
    public partial class RaiseIntentResultAgentErrorResponseMeta
    {
        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// Error message payload containing an standardized error string.
    /// </summary>
    public partial class RaiseIntentResultAgentErrorResponsePayload
    {
        [JsonProperty("error")]
        public RaiseIntentResultErrorMessage Error { get; set; }
    }

    /// <summary>
    /// A secondary response to a request to raise an intent used to deliver the intent result
    ///
    /// A response message from a Desktop Agent to the Bridge.
    /// </summary>
    public partial class RaiseIntentResultAgentResponse
    {
        [JsonProperty("meta")]
        public RaiseIntentResultAgentResponseMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains return values for FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public RaiseIntentResultAgentResponsePayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Response' appended.
        /// </summary>
        [JsonProperty("type")]
        public ResponseMessageType Type { get; set; }
    }

    /// <summary>
    /// Metadata for a response messages sent by a Desktop Agent to the Bridge
    /// </summary>
    public partial class RaiseIntentResultAgentResponseMeta
    {
        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload typically contains return values for FDC3 API functions.
    /// </summary>
    public partial class RaiseIntentResultAgentResponsePayload
    {
        [JsonProperty("intentResult")]
        public IntentResult IntentResult { get; set; }
    }

    public partial class IntentResult
    {
        [JsonProperty("context", NullValueHandling = NullValueHandling.Ignore)]
        public ContextElement Context { get; set; }

        [JsonProperty("channel", NullValueHandling = NullValueHandling.Ignore)]
        public Channel Channel { get; set; }
    }

    /// <summary>
    /// Represents a context channel that applications can use to send and receive
    /// context data.
    ///
    /// Please note that There are differences in behavior when you interact with a
    /// User channel via the `DesktopAgent` interface and the `Channel` interface.
    /// Specifically, when 'joining' a User channel or adding a context listener
    /// when already joined to a channel via the `DesktopAgent` interface, existing
    /// context (matching the type of the context listener) on the channel is
    /// received by the context listener immediately. Whereas, when a context
    /// listener is added via the Channel interface, context is not received
    /// automatically, but may be retrieved manually via the `getCurrentContext()`
    /// function.
    /// </summary>
    public partial class Channel
    {
        /// <summary>
        /// Channels may be visualized and selectable by users. DisplayMetadata may be used to
        /// provide hints on how to see them.
        /// For App channels, displayMetadata would typically not be present.
        /// </summary>
        [JsonProperty("displayMetadata", NullValueHandling = NullValueHandling.Ignore)]
        public DisplayMetadata DisplayMetadata { get; set; }

        /// <summary>
        /// Constant that uniquely identifies this channel.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Uniquely defines each channel type.
        /// Can be "user", "app" or "private".
        /// </summary>
        [JsonProperty("type")]
        public TypeEnum Type { get; set; }
    }

    /// <summary>
    /// Channels may be visualized and selectable by users. DisplayMetadata may be used to
    /// provide hints on how to see them.
    /// For App channels, displayMetadata would typically not be present.
    ///
    /// A system channel will be global enough to have a presence across many apps. This gives us
    /// some hints
    /// to render them in a standard way. It is assumed it may have other properties too, but if
    /// it has these,
    /// this is their meaning.
    /// </summary>
    public partial class DisplayMetadata
    {
        /// <summary>
        /// The color that should be associated within this channel when displaying this channel in a
        /// UI, e.g: `0xFF0000`.
        /// </summary>
        [JsonProperty("color", NullValueHandling = NullValueHandling.Ignore)]
        public string Color { get; set; }

        /// <summary>
        /// A URL of an image that can be used to display this channel
        /// </summary>
        [JsonProperty("glyph", NullValueHandling = NullValueHandling.Ignore)]
        public string Glyph { get; set; }

        /// <summary>
        /// A user-readable name for this channel, e.g: `"Red"`
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
    }

    /// <summary>
    /// A secondary response to a request to raise an intent used to deliver the intent result,
    /// which contains an error
    ///
    /// A response message from the Bridge back to the original Desktop Agent that raised the
    /// request, used where all connected agents returned errors.
    /// </summary>
    public partial class RaiseIntentResultBridgeErrorResponse
    {
        [JsonProperty("meta")]
        public RaiseIntentResultBridgeErrorResponseMeta Meta { get; set; }

        /// <summary>
        /// The error message payload contains details of an error return to the app or agent that
        /// raised the original request.
        /// </summary>
        [JsonProperty("payload")]
        public RaiseIntentResultBridgeErrorResponsePayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Response' appended.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Metadata required in a response message collated and/or forwarded on by the Bridge
    /// </summary>
    public partial class RaiseIntentResultBridgeErrorResponseMeta
    {
        [JsonProperty("errorDetails")]
        public ResponseErrorDetail[] ErrorDetails { get; set; }

        [JsonProperty("errorSources")]
        public DesktopAgentIdentifier[] ErrorSources { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The error message payload contains details of an error return to the app or agent that
    /// raised the original request.
    /// </summary>
    public partial class RaiseIntentResultBridgeErrorResponsePayload
    {
        [JsonProperty("error")]
        public RaiseIntentResultErrorMessage Error { get; set; }
    }

    /// <summary>
    /// A secondary response to a request to raise an intent used to deliver the intent result
    ///
    /// A response message from the Bridge back to the original Desktop Agent that raised the
    /// request.
    /// </summary>
    public partial class RaiseIntentResultBridgeResponse
    {
        [JsonProperty("meta")]
        public RaiseIntentResultBridgeResponseMeta Meta { get; set; }

        /// <summary>
        /// The message payload typically contains return values for FDC3 API functions.
        /// </summary>
        [JsonProperty("payload")]
        public RaiseIntentResultBridgeResponsePayload Payload { get; set; }

        /// <summary>
        /// Identifies the type of the message and it is typically set to the FDC3 function name that
        /// the message relates to, e.g. 'findIntent', with 'Response' appended.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Metadata required in a response message collated and/or forwarded on by the Bridge
    /// </summary>
    public partial class RaiseIntentResultBridgeResponseMeta
    {
        [JsonProperty("errorDetails", NullValueHandling = NullValueHandling.Ignore)]
        public ResponseErrorDetail[] ErrorDetails { get; set; }

        [JsonProperty("errorSources", NullValueHandling = NullValueHandling.Ignore)]
        public DesktopAgentIdentifier[] ErrorSources { get; set; }

        [JsonProperty("requestUuid")]
        public string RequestUuid { get; set; }

        [JsonProperty("responseUuid")]
        public string ResponseUuid { get; set; }

        [JsonProperty("sources", NullValueHandling = NullValueHandling.Ignore)]
        public DesktopAgentIdentifier[] Sources { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// The message payload typically contains return values for FDC3 API functions.
    /// </summary>
    public partial class RaiseIntentResultBridgeResponsePayload
    {
        [JsonProperty("intentResult")]
        public IntentResult IntentResult { get; set; }
    }

    /// <summary>
    /// The `fdc3.context` type defines the basic contract or "shape" for all data exchanged by
    /// FDC3 operations. As such, it is not really meant to be used on its own, but is imported
    /// by more specific type definitions (standardized or custom) to provide the structure and
    /// properties shared by all FDC3 context data types.
    ///
    /// The key element of FDC3 context types is their mandatory `type` property, which is used
    /// to identify what type of data the object represents, and what shape it has.
    ///
    /// The FDC3 context type, and all derived types, define the minimum set of fields a context
    /// data object of a particular type can be expected to have, but this can always be extended
    /// with custom fields as appropriate.
    /// </summary>
    public partial class Context
    {
        /// <summary>
        /// Context data objects may include a set of equivalent key-value pairs that can be used to
        /// help applications identify and look up the context type they receive in their own domain.
        /// The idea behind this design is that applications can provide as many equivalent
        /// identifiers to a target application as possible, e.g. an instrument may be represented by
        /// an ISIN, CUSIP or Bloomberg identifier.
        ///
        /// Identifiers do not make sense for all types of data, so the `id` property is therefore
        /// optional, but some derived types may choose to require at least one identifier.
        /// </summary>
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Id { get; set; }

        /// <summary>
        /// Context data objects may include a name property that can be used for more information,
        /// or display purposes. Some derived types may require the name object as mandatory,
        /// depending on use case.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// The type property is the only _required_ part of the FDC3 context data schema. The FDC3
        /// [API](https://fdc3.finos.org/docs/api/spec) relies on the `type` property being present
        /// to route shared context data appropriately.
        ///
        /// FDC3 [Intents](https://fdc3.finos.org/docs/intents/spec) also register the context data
        /// types they support in an FDC3 [App
        /// Directory](https://fdc3.finos.org/docs/app-directory/overview), used for intent discovery
        /// and routing.
        ///
        /// Standardized FDC3 context types have well-known `type` properties prefixed with the
        /// `fdc3` namespace, e.g. `fdc3.instrument`. For non-standard types, e.g. those defined and
        /// used by a particular organization, the convention is to prefix them with an
        /// organization-specific namespace, e.g. `blackrock.fund`.
        ///
        /// See the [Context Data Specification](https://fdc3.finos.org/docs/context/spec) for more
        /// information about context data types.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Array of error message strings for responses that were not returned to the bridge before
    /// the timeout or because an error occurred. Should be the same length as the `errorSources`
    /// array and ordered the same. May be omitted if all sources responded without errors.
    ///
    /// Constants representing the errors that can be encountered when calling the `open` method
    /// on the DesktopAgent object (`fdc3`).
    ///
    /// Constants representing the errors that can be encountered when calling the `findIntent`,
    /// `findIntentsByContext`, `raiseIntent` or `raiseIntentForContext` methods on the
    /// DesktopAgent (`fdc3`).
    /// </summary>
    public enum ResponseErrorDetail { AccessDenied, AgentDisconnected, AppNotFound, AppTimeout, CreationFailed, DesktopAgentNotFound, ErrorOnLaunch, IntentDeliveryFailed, IntentHandlerRejected, MalformedContext, MalformedMessage, NoAppsFound, NoChannelFound, NoResultReturned, NotConnectedToBridge, ResolverTimeout, ResolverUnavailable, ResponseToBridgeTimedOut, TargetAppUnavailable, TargetInstanceUnavailable, UserCancelledResolution };

    /// <summary>
    /// Identifies the type of the message and it is typically set to the FDC3 function name that
    /// the message relates to, e.g. 'findIntent', with 'Response' appended.
    /// </summary>
    public enum ResponseMessageType { FindInstancesResponse, FindIntentResponse, FindIntentsByContextResponse, GetAppMetadataResponse, OpenResponse, RaiseIntentResponse, RaiseIntentResultResponse };

    /// <summary>
    /// Identifies the type of the message and it is typically set to the FDC3 function name that
    /// the message relates to, e.g. 'findIntent', with 'Request' appended.
    /// </summary>
    public enum RequestMessageType { BroadcastRequest, FindInstancesRequest, FindIntentRequest, FindIntentsByContextRequest, GetAppMetadataRequest, OpenRequest, PrivateChannelBroadcast, PrivateChannelEventListenerAdded, PrivateChannelOnAddContextListener, PrivateChannelOnDisconnect, PrivateChannelOnUnsubscribe, RaiseIntentRequest };

    /// <summary>
    /// Identifies the type of the connection step message.
    /// </summary>
    public enum ConnectionStepMessageType { AuthenticationFailed, ConnectedAgentsUpdate, Handshake, Hello };

    /// <summary>
    /// Constants representing the errors that can be encountered when calling the `findIntent`,
    /// `findIntentsByContext`, `raiseIntent` or `raiseIntentForContext` methods on the
    /// DesktopAgent (`fdc3`).
    ///
    /// Array of error message strings for responses that were not returned to the bridge before
    /// the timeout or because an error occurred. Should be the same length as the `errorSources`
    /// array and ordered the same. May be omitted if all sources responded without errors.
    ///
    /// Constants representing the errors that can be encountered when calling the `open` method
    /// on the DesktopAgent object (`fdc3`).
    /// </summary>
    public enum ErrorMessage { AgentDisconnected, DesktopAgentNotFound, IntentDeliveryFailed, MalformedContext, MalformedMessage, NoAppsFound, NotConnectedToBridge, ResolverTimeout, ResolverUnavailable, ResponseToBridgeTimedOut, TargetAppUnavailable, TargetInstanceUnavailable, UserCancelledResolution };

    /// <summary>
    /// Constants representing the errors that can be encountered when calling the `open` method
    /// on the DesktopAgent object (`fdc3`).
    ///
    /// Array of error message strings for responses that were not returned to the bridge before
    /// the timeout or because an error occurred. Should be the same length as the `errorSources`
    /// array and ordered the same. May be omitted if all sources responded without errors.
    ///
    /// Constants representing the errors that can be encountered when calling the `findIntent`,
    /// `findIntentsByContext`, `raiseIntent` or `raiseIntentForContext` methods on the
    /// DesktopAgent (`fdc3`).
    /// </summary>
    public enum OpenErrorMessage { AgentDisconnected, AppNotFound, AppTimeout, DesktopAgentNotFound, ErrorOnLaunch, MalformedContext, MalformedMessage, NotConnectedToBridge, ResolverUnavailable, ResponseToBridgeTimedOut };

    public enum Empty { OnAddContextListener, OnDisconnect, OnUnsubscribe };

    /// <summary>
    /// Array of error message strings for responses that were not returned to the bridge before
    /// the timeout or because an error occurred. Should be the same length as the `errorSources`
    /// array and ordered the same. May be omitted if all sources responded without errors.
    ///
    /// Constants representing the errors that can be encountered when calling the `open` method
    /// on the DesktopAgent object (`fdc3`).
    ///
    /// Constants representing the errors that can be encountered when calling the `findIntent`,
    /// `findIntentsByContext`, `raiseIntent` or `raiseIntentForContext` methods on the
    /// DesktopAgent (`fdc3`).
    /// </summary>
    public enum RaiseIntentResultErrorMessage { AgentDisconnected, IntentHandlerRejected, MalformedMessage, NoResultReturned, NotConnectedToBridge, ResponseToBridgeTimedOut };

    /// <summary>
    /// Uniquely defines each channel type.
    /// Can be "user", "app" or "private".
    /// </summary>
    public enum TypeEnum { App, Private, User };

    public class SchemasApiApiSchema
    {
        public static object FromJson(string json) => JsonConvert.DeserializeObject<object>(json);
    }

    public partial class BaseImplementationMetadata
    {
        public static BaseImplementationMetadata FromJson(string json) => JsonConvert.DeserializeObject<BaseImplementationMetadata>(json);
    }

    public partial class AgentErrorResponseMessage
    {
        public static AgentErrorResponseMessage FromJson(string json) => JsonConvert.DeserializeObject<AgentErrorResponseMessage>(json);
    }

    public partial class AgentRequestMessage
    {
        public static AgentRequestMessage FromJson(string json) => JsonConvert.DeserializeObject<AgentRequestMessage>(json);
    }

    public partial class AgentResponseMessage
    {
        public static AgentResponseMessage FromJson(string json) => JsonConvert.DeserializeObject<AgentResponseMessage>(json);
    }

    public partial class BridgeErrorResponseMessage
    {
        public static BridgeErrorResponseMessage FromJson(string json) => JsonConvert.DeserializeObject<BridgeErrorResponseMessage>(json);
    }

    public partial class BridgeRequestMessage
    {
        public static BridgeRequestMessage FromJson(string json) => JsonConvert.DeserializeObject<BridgeRequestMessage>(json);
    }

    public partial class BridgeResponseMessage
    {
        public static BridgeResponseMessage FromJson(string json) => JsonConvert.DeserializeObject<BridgeResponseMessage>(json);
    }

    public partial class BroadcastAgentRequest
    {
        public static BroadcastAgentRequest FromJson(string json) => JsonConvert.DeserializeObject<BroadcastAgentRequest>(json);
    }

    public partial class BroadcastBridgeRequest
    {
        public static BroadcastBridgeRequest FromJson(string json) => JsonConvert.DeserializeObject<BroadcastBridgeRequest>(json);
    }

    public class BridgingCommons
    {
        public static Dictionary<string, object> FromJson(string json) => JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
    }

    public partial class ConnectionStepMessage
    {
        public static ConnectionStepMessage FromJson(string json) => JsonConvert.DeserializeObject<ConnectionStepMessage>(json);
    }

    public partial class ConnectionStep2Hello
    {
        public static ConnectionStep2Hello FromJson(string json) => JsonConvert.DeserializeObject<ConnectionStep2Hello>(json);
    }

    public partial class ConnectionStep3Handshake
    {
        public static ConnectionStep3Handshake FromJson(string json) => JsonConvert.DeserializeObject<ConnectionStep3Handshake>(json);
    }

    public partial class ConnectionStep4AuthenticationFailed
    {
        public static ConnectionStep4AuthenticationFailed FromJson(string json) => JsonConvert.DeserializeObject<ConnectionStep4AuthenticationFailed>(json);
    }

    public partial class ConnectionStep6ConnectedAgentsUpdate
    {
        public static ConnectionStep6ConnectedAgentsUpdate FromJson(string json) => JsonConvert.DeserializeObject<ConnectionStep6ConnectedAgentsUpdate>(json);
    }

    public partial class FindInstancesAgentErrorResponse
    {
        public static FindInstancesAgentErrorResponse FromJson(string json) => JsonConvert.DeserializeObject<FindInstancesAgentErrorResponse>(json);
    }

    public partial class FindInstancesAgentRequest
    {
        public static FindInstancesAgentRequest FromJson(string json) => JsonConvert.DeserializeObject<FindInstancesAgentRequest>(json);
    }

    public partial class FindInstancesAgentResponse
    {
        public static FindInstancesAgentResponse FromJson(string json) => JsonConvert.DeserializeObject<FindInstancesAgentResponse>(json);
    }

    public partial class FindInstancesBridgeErrorResponse
    {
        public static FindInstancesBridgeErrorResponse FromJson(string json) => JsonConvert.DeserializeObject<FindInstancesBridgeErrorResponse>(json);
    }

    public partial class FindInstancesBridgeRequest
    {
        public static FindInstancesBridgeRequest FromJson(string json) => JsonConvert.DeserializeObject<FindInstancesBridgeRequest>(json);
    }

    public partial class FindInstancesBridgeResponse
    {
        public static FindInstancesBridgeResponse FromJson(string json) => JsonConvert.DeserializeObject<FindInstancesBridgeResponse>(json);
    }

    public partial class FindIntentAgentErrorResponse
    {
        public static FindIntentAgentErrorResponse FromJson(string json) => JsonConvert.DeserializeObject<FindIntentAgentErrorResponse>(json);
    }

    public partial class FindIntentAgentRequest
    {
        public static FindIntentAgentRequest FromJson(string json) => JsonConvert.DeserializeObject<FindIntentAgentRequest>(json);
    }

    public partial class FindIntentAgentResponse
    {
        public static FindIntentAgentResponse FromJson(string json) => JsonConvert.DeserializeObject<FindIntentAgentResponse>(json);
    }

    public partial class FindIntentBridgeErrorResponse
    {
        public static FindIntentBridgeErrorResponse FromJson(string json) => JsonConvert.DeserializeObject<FindIntentBridgeErrorResponse>(json);
    }

    public partial class FindIntentBridgeRequest
    {
        public static FindIntentBridgeRequest FromJson(string json) => JsonConvert.DeserializeObject<FindIntentBridgeRequest>(json);
    }

    public partial class FindIntentBridgeResponse
    {
        public static FindIntentBridgeResponse FromJson(string json) => JsonConvert.DeserializeObject<FindIntentBridgeResponse>(json);
    }

    public partial class FindIntentsByContextAgentErrorResponse
    {
        public static FindIntentsByContextAgentErrorResponse FromJson(string json) => JsonConvert.DeserializeObject<FindIntentsByContextAgentErrorResponse>(json);
    }

    public partial class FindIntentsByContextAgentRequest
    {
        public static FindIntentsByContextAgentRequest FromJson(string json) => JsonConvert.DeserializeObject<FindIntentsByContextAgentRequest>(json);
    }

    public partial class FindIntentsByContextAgentResponse
    {
        public static FindIntentsByContextAgentResponse FromJson(string json) => JsonConvert.DeserializeObject<FindIntentsByContextAgentResponse>(json);
    }

    public partial class FindIntentsByContextBridgeErrorResponse
    {
        public static FindIntentsByContextBridgeErrorResponse FromJson(string json) => JsonConvert.DeserializeObject<FindIntentsByContextBridgeErrorResponse>(json);
    }

    public partial class FindIntentsByContextBridgeRequest
    {
        public static FindIntentsByContextBridgeRequest FromJson(string json) => JsonConvert.DeserializeObject<FindIntentsByContextBridgeRequest>(json);
    }

    public partial class FindIntentsByContextBridgeResponse
    {
        public static FindIntentsByContextBridgeResponse FromJson(string json) => JsonConvert.DeserializeObject<FindIntentsByContextBridgeResponse>(json);
    }

    public partial class GetAppMetadataAgentErrorResponse
    {
        public static GetAppMetadataAgentErrorResponse FromJson(string json) => JsonConvert.DeserializeObject<GetAppMetadataAgentErrorResponse>(json);
    }

    public partial class GetAppMetadataAgentRequest
    {
        public static GetAppMetadataAgentRequest FromJson(string json) => JsonConvert.DeserializeObject<GetAppMetadataAgentRequest>(json);
    }

    public partial class GetAppMetadataAgentResponse
    {
        public static GetAppMetadataAgentResponse FromJson(string json) => JsonConvert.DeserializeObject<GetAppMetadataAgentResponse>(json);
    }

    public partial class GetAppMetadataBridgeErrorResponse
    {
        public static GetAppMetadataBridgeErrorResponse FromJson(string json) => JsonConvert.DeserializeObject<GetAppMetadataBridgeErrorResponse>(json);
    }

    public partial class GetAppMetadataBridgeRequest
    {
        public static GetAppMetadataBridgeRequest FromJson(string json) => JsonConvert.DeserializeObject<GetAppMetadataBridgeRequest>(json);
    }

    public partial class GetAppMetadataBridgeResponse
    {
        public static GetAppMetadataBridgeResponse FromJson(string json) => JsonConvert.DeserializeObject<GetAppMetadataBridgeResponse>(json);
    }

    public partial class OpenAgentErrorResponse
    {
        public static OpenAgentErrorResponse FromJson(string json) => JsonConvert.DeserializeObject<OpenAgentErrorResponse>(json);
    }

    public partial class OpenAgentRequest
    {
        public static OpenAgentRequest FromJson(string json) => JsonConvert.DeserializeObject<OpenAgentRequest>(json);
    }

    public partial class OpenAgentResponse
    {
        public static OpenAgentResponse FromJson(string json) => JsonConvert.DeserializeObject<OpenAgentResponse>(json);
    }

    public partial class OpenBridgeErrorResponse
    {
        public static OpenBridgeErrorResponse FromJson(string json) => JsonConvert.DeserializeObject<OpenBridgeErrorResponse>(json);
    }

    public partial class OpenBridgeRequest
    {
        public static OpenBridgeRequest FromJson(string json) => JsonConvert.DeserializeObject<OpenBridgeRequest>(json);
    }

    public partial class OpenBridgeResponse
    {
        public static OpenBridgeResponse FromJson(string json) => JsonConvert.DeserializeObject<OpenBridgeResponse>(json);
    }

    public partial class PrivateChannelBroadcastAgentRequest
    {
        public static PrivateChannelBroadcastAgentRequest FromJson(string json) => JsonConvert.DeserializeObject<PrivateChannelBroadcastAgentRequest>(json);
    }

    public partial class PrivateChannelBroadcastBridgeRequest
    {
        public static PrivateChannelBroadcastBridgeRequest FromJson(string json) => JsonConvert.DeserializeObject<PrivateChannelBroadcastBridgeRequest>(json);
    }

    public partial class PrivateChannelEventListenerAddedAgentRequest
    {
        public static PrivateChannelEventListenerAddedAgentRequest FromJson(string json) => JsonConvert.DeserializeObject<PrivateChannelEventListenerAddedAgentRequest>(json);
    }

    public partial class PrivateChannelEventListenerAddedBridgeRequest
    {
        public static PrivateChannelEventListenerAddedBridgeRequest FromJson(string json) => JsonConvert.DeserializeObject<PrivateChannelEventListenerAddedBridgeRequest>(json);
    }

    public partial class PrivateChannelEventListenerRemovedAgentRequest
    {
        public static PrivateChannelEventListenerRemovedAgentRequest FromJson(string json) => JsonConvert.DeserializeObject<PrivateChannelEventListenerRemovedAgentRequest>(json);
    }

    public partial class PrivateChannelEventListenerRemovedBridgeRequest
    {
        public static PrivateChannelEventListenerRemovedBridgeRequest FromJson(string json) => JsonConvert.DeserializeObject<PrivateChannelEventListenerRemovedBridgeRequest>(json);
    }

    public partial class PrivateChannelOnAddContextListenerAgentRequest
    {
        public static PrivateChannelOnAddContextListenerAgentRequest FromJson(string json) => JsonConvert.DeserializeObject<PrivateChannelOnAddContextListenerAgentRequest>(json);
    }

    public partial class PrivateChannelOnAddContextListenerBridgeRequest
    {
        public static PrivateChannelOnAddContextListenerBridgeRequest FromJson(string json) => JsonConvert.DeserializeObject<PrivateChannelOnAddContextListenerBridgeRequest>(json);
    }

    public partial class PrivateChannelOnDisconnectAgentRequest
    {
        public static PrivateChannelOnDisconnectAgentRequest FromJson(string json) => JsonConvert.DeserializeObject<PrivateChannelOnDisconnectAgentRequest>(json);
    }

    public partial class PrivateChannelOnDisconnectBridgeRequest
    {
        public static PrivateChannelOnDisconnectBridgeRequest FromJson(string json) => JsonConvert.DeserializeObject<PrivateChannelOnDisconnectBridgeRequest>(json);
    }

    public partial class PrivateChannelOnUnsubscribeAgentRequest
    {
        public static PrivateChannelOnUnsubscribeAgentRequest FromJson(string json) => JsonConvert.DeserializeObject<PrivateChannelOnUnsubscribeAgentRequest>(json);
    }

    public partial class PrivateChannelOnUnsubscribeBridgeRequest
    {
        public static PrivateChannelOnUnsubscribeBridgeRequest FromJson(string json) => JsonConvert.DeserializeObject<PrivateChannelOnUnsubscribeBridgeRequest>(json);
    }

    public partial class RaiseIntentAgentErrorResponse
    {
        public static RaiseIntentAgentErrorResponse FromJson(string json) => JsonConvert.DeserializeObject<RaiseIntentAgentErrorResponse>(json);
    }

    public partial class RaiseIntentAgentRequest
    {
        public static RaiseIntentAgentRequest FromJson(string json) => JsonConvert.DeserializeObject<RaiseIntentAgentRequest>(json);
    }

    public partial class RaiseIntentAgentResponse
    {
        public static RaiseIntentAgentResponse FromJson(string json) => JsonConvert.DeserializeObject<RaiseIntentAgentResponse>(json);
    }

    public partial class RaiseIntentBridgeErrorResponse
    {
        public static RaiseIntentBridgeErrorResponse FromJson(string json) => JsonConvert.DeserializeObject<RaiseIntentBridgeErrorResponse>(json);
    }

    public partial class RaiseIntentBridgeRequest
    {
        public static RaiseIntentBridgeRequest FromJson(string json) => JsonConvert.DeserializeObject<RaiseIntentBridgeRequest>(json);
    }

    public partial class RaiseIntentBridgeResponse
    {
        public static RaiseIntentBridgeResponse FromJson(string json) => JsonConvert.DeserializeObject<RaiseIntentBridgeResponse>(json);
    }

    public partial class RaiseIntentResultAgentErrorResponse
    {
        public static RaiseIntentResultAgentErrorResponse FromJson(string json) => JsonConvert.DeserializeObject<RaiseIntentResultAgentErrorResponse>(json);
    }

    public partial class RaiseIntentResultAgentResponse
    {
        public static RaiseIntentResultAgentResponse FromJson(string json) => JsonConvert.DeserializeObject<RaiseIntentResultAgentResponse>(json);
    }

    public partial class RaiseIntentResultBridgeErrorResponse
    {
        public static RaiseIntentResultBridgeErrorResponse FromJson(string json) => JsonConvert.DeserializeObject<RaiseIntentResultBridgeErrorResponse>(json);
    }

    public partial class RaiseIntentResultBridgeResponse
    {
        public static RaiseIntentResultBridgeResponse FromJson(string json) => JsonConvert.DeserializeObject<RaiseIntentResultBridgeResponse>(json);
    }

    public partial class Context
    {
        public static Context FromJson(string json) => JsonConvert.DeserializeObject<Context>(json);
    }

    public static class Serialize
    {
        public static string ToJson(this object self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this BaseImplementationMetadata self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this AgentErrorResponseMessage self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this AgentRequestMessage self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this AgentResponseMessage self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this BridgeErrorResponseMessage self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this BridgeRequestMessage self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this BridgeResponseMessage self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this BroadcastAgentRequest self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this BroadcastBridgeRequest self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this Dictionary<string, object> self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this ConnectionStepMessage self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this ConnectionStep2Hello self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this ConnectionStep3Handshake self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this ConnectionStep4AuthenticationFailed self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this ConnectionStep6ConnectedAgentsUpdate self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this FindInstancesAgentErrorResponse self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this FindInstancesAgentRequest self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this FindInstancesAgentResponse self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this FindInstancesBridgeErrorResponse self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this FindInstancesBridgeRequest self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this FindInstancesBridgeResponse self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this FindIntentAgentErrorResponse self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this FindIntentAgentRequest self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this FindIntentAgentResponse self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this FindIntentBridgeErrorResponse self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this FindIntentBridgeRequest self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this FindIntentBridgeResponse self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this FindIntentsByContextAgentErrorResponse self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this FindIntentsByContextAgentRequest self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this FindIntentsByContextAgentResponse self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this FindIntentsByContextBridgeErrorResponse self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this FindIntentsByContextBridgeRequest self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this FindIntentsByContextBridgeResponse self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this GetAppMetadataAgentErrorResponse self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this GetAppMetadataAgentRequest self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this GetAppMetadataAgentResponse self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this GetAppMetadataBridgeErrorResponse self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this GetAppMetadataBridgeRequest self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this GetAppMetadataBridgeResponse self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this OpenAgentErrorResponse self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this OpenAgentRequest self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this OpenAgentResponse self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this OpenBridgeErrorResponse self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this OpenBridgeRequest self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this OpenBridgeResponse self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this PrivateChannelBroadcastAgentRequest self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this PrivateChannelBroadcastBridgeRequest self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this PrivateChannelEventListenerAddedAgentRequest self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this PrivateChannelEventListenerAddedBridgeRequest self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this PrivateChannelEventListenerRemovedAgentRequest self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this PrivateChannelEventListenerRemovedBridgeRequest self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this PrivateChannelOnAddContextListenerAgentRequest self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this PrivateChannelOnAddContextListenerBridgeRequest self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this PrivateChannelOnDisconnectAgentRequest self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this PrivateChannelOnDisconnectBridgeRequest self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this PrivateChannelOnUnsubscribeAgentRequest self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this PrivateChannelOnUnsubscribeBridgeRequest self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this RaiseIntentAgentErrorResponse self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this RaiseIntentAgentRequest self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this RaiseIntentAgentResponse self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this RaiseIntentBridgeErrorResponse self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this RaiseIntentBridgeRequest self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this RaiseIntentBridgeResponse self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this RaiseIntentResultAgentErrorResponse self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this RaiseIntentResultAgentResponse self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this RaiseIntentResultBridgeErrorResponse self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this RaiseIntentResultBridgeResponse self) => JsonConvert.SerializeObject(self);
        public static string ToJson(this Context self) => JsonConvert.SerializeObject(self);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                ResponseErrorDetailConverter.Singleton,
                ResponseMessageTypeConverter.Singleton,
                RequestMessageTypeConverter.Singleton,
                ConnectionStepMessageTypeConverter.Singleton,
                ErrorMessageConverter.Singleton,
                OpenErrorMessageConverter.Singleton,
                EmptyConverter.Singleton,
                RaiseIntentResultErrorMessageConverter.Singleton,
                TypeEnumConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ResponseErrorDetailConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ResponseErrorDetail) || t == typeof(ResponseErrorDetail?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "AccessDenied":
                    return ResponseErrorDetail.AccessDenied;
                case "AgentDisconnected":
                    return ResponseErrorDetail.AgentDisconnected;
                case "AppNotFound":
                    return ResponseErrorDetail.AppNotFound;
                case "AppTimeout":
                    return ResponseErrorDetail.AppTimeout;
                case "CreationFailed":
                    return ResponseErrorDetail.CreationFailed;
                case "DesktopAgentNotFound":
                    return ResponseErrorDetail.DesktopAgentNotFound;
                case "ErrorOnLaunch":
                    return ResponseErrorDetail.ErrorOnLaunch;
                case "IntentDeliveryFailed":
                    return ResponseErrorDetail.IntentDeliveryFailed;
                case "IntentHandlerRejected":
                    return ResponseErrorDetail.IntentHandlerRejected;
                case "MalformedContext":
                    return ResponseErrorDetail.MalformedContext;
                case "MalformedMessage":
                    return ResponseErrorDetail.MalformedMessage;
                case "NoAppsFound":
                    return ResponseErrorDetail.NoAppsFound;
                case "NoChannelFound":
                    return ResponseErrorDetail.NoChannelFound;
                case "NoResultReturned":
                    return ResponseErrorDetail.NoResultReturned;
                case "NotConnectedToBridge":
                    return ResponseErrorDetail.NotConnectedToBridge;
                case "ResolverTimeout":
                    return ResponseErrorDetail.ResolverTimeout;
                case "ResolverUnavailable":
                    return ResponseErrorDetail.ResolverUnavailable;
                case "ResponseToBridgeTimedOut":
                    return ResponseErrorDetail.ResponseToBridgeTimedOut;
                case "TargetAppUnavailable":
                    return ResponseErrorDetail.TargetAppUnavailable;
                case "TargetInstanceUnavailable":
                    return ResponseErrorDetail.TargetInstanceUnavailable;
                case "UserCancelledResolution":
                    return ResponseErrorDetail.UserCancelledResolution;
            }
            throw new Exception("Cannot unmarshal type ResponseErrorDetail");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (ResponseErrorDetail)untypedValue;
            switch (value)
            {
                case ResponseErrorDetail.AccessDenied:
                    serializer.Serialize(writer, "AccessDenied");
                    return;
                case ResponseErrorDetail.AgentDisconnected:
                    serializer.Serialize(writer, "AgentDisconnected");
                    return;
                case ResponseErrorDetail.AppNotFound:
                    serializer.Serialize(writer, "AppNotFound");
                    return;
                case ResponseErrorDetail.AppTimeout:
                    serializer.Serialize(writer, "AppTimeout");
                    return;
                case ResponseErrorDetail.CreationFailed:
                    serializer.Serialize(writer, "CreationFailed");
                    return;
                case ResponseErrorDetail.DesktopAgentNotFound:
                    serializer.Serialize(writer, "DesktopAgentNotFound");
                    return;
                case ResponseErrorDetail.ErrorOnLaunch:
                    serializer.Serialize(writer, "ErrorOnLaunch");
                    return;
                case ResponseErrorDetail.IntentDeliveryFailed:
                    serializer.Serialize(writer, "IntentDeliveryFailed");
                    return;
                case ResponseErrorDetail.IntentHandlerRejected:
                    serializer.Serialize(writer, "IntentHandlerRejected");
                    return;
                case ResponseErrorDetail.MalformedContext:
                    serializer.Serialize(writer, "MalformedContext");
                    return;
                case ResponseErrorDetail.MalformedMessage:
                    serializer.Serialize(writer, "MalformedMessage");
                    return;
                case ResponseErrorDetail.NoAppsFound:
                    serializer.Serialize(writer, "NoAppsFound");
                    return;
                case ResponseErrorDetail.NoChannelFound:
                    serializer.Serialize(writer, "NoChannelFound");
                    return;
                case ResponseErrorDetail.NoResultReturned:
                    serializer.Serialize(writer, "NoResultReturned");
                    return;
                case ResponseErrorDetail.NotConnectedToBridge:
                    serializer.Serialize(writer, "NotConnectedToBridge");
                    return;
                case ResponseErrorDetail.ResolverTimeout:
                    serializer.Serialize(writer, "ResolverTimeout");
                    return;
                case ResponseErrorDetail.ResolverUnavailable:
                    serializer.Serialize(writer, "ResolverUnavailable");
                    return;
                case ResponseErrorDetail.ResponseToBridgeTimedOut:
                    serializer.Serialize(writer, "ResponseToBridgeTimedOut");
                    return;
                case ResponseErrorDetail.TargetAppUnavailable:
                    serializer.Serialize(writer, "TargetAppUnavailable");
                    return;
                case ResponseErrorDetail.TargetInstanceUnavailable:
                    serializer.Serialize(writer, "TargetInstanceUnavailable");
                    return;
                case ResponseErrorDetail.UserCancelledResolution:
                    serializer.Serialize(writer, "UserCancelledResolution");
                    return;
            }
            throw new Exception("Cannot marshal type ResponseErrorDetail");
        }

        public static readonly ResponseErrorDetailConverter Singleton = new ResponseErrorDetailConverter();
    }

    internal class ResponseMessageTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ResponseMessageType) || t == typeof(ResponseMessageType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "findInstancesResponse":
                    return ResponseMessageType.FindInstancesResponse;
                case "findIntentResponse":
                    return ResponseMessageType.FindIntentResponse;
                case "findIntentsByContextResponse":
                    return ResponseMessageType.FindIntentsByContextResponse;
                case "getAppMetadataResponse":
                    return ResponseMessageType.GetAppMetadataResponse;
                case "openResponse":
                    return ResponseMessageType.OpenResponse;
                case "raiseIntentResponse":
                    return ResponseMessageType.RaiseIntentResponse;
                case "raiseIntentResultResponse":
                    return ResponseMessageType.RaiseIntentResultResponse;
            }
            throw new Exception("Cannot unmarshal type ResponseMessageType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (ResponseMessageType)untypedValue;
            switch (value)
            {
                case ResponseMessageType.FindInstancesResponse:
                    serializer.Serialize(writer, "findInstancesResponse");
                    return;
                case ResponseMessageType.FindIntentResponse:
                    serializer.Serialize(writer, "findIntentResponse");
                    return;
                case ResponseMessageType.FindIntentsByContextResponse:
                    serializer.Serialize(writer, "findIntentsByContextResponse");
                    return;
                case ResponseMessageType.GetAppMetadataResponse:
                    serializer.Serialize(writer, "getAppMetadataResponse");
                    return;
                case ResponseMessageType.OpenResponse:
                    serializer.Serialize(writer, "openResponse");
                    return;
                case ResponseMessageType.RaiseIntentResponse:
                    serializer.Serialize(writer, "raiseIntentResponse");
                    return;
                case ResponseMessageType.RaiseIntentResultResponse:
                    serializer.Serialize(writer, "raiseIntentResultResponse");
                    return;
            }
            throw new Exception("Cannot marshal type ResponseMessageType");
        }

        public static readonly ResponseMessageTypeConverter Singleton = new ResponseMessageTypeConverter();
    }

    internal class RequestMessageTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(RequestMessageType) || t == typeof(RequestMessageType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "PrivateChannel.broadcast":
                    return RequestMessageType.PrivateChannelBroadcast;
                case "PrivateChannel.eventListenerAdded":
                    return RequestMessageType.PrivateChannelEventListenerAdded;
                case "PrivateChannel.onAddContextListener":
                    return RequestMessageType.PrivateChannelOnAddContextListener;
                case "PrivateChannel.onDisconnect":
                    return RequestMessageType.PrivateChannelOnDisconnect;
                case "PrivateChannel.onUnsubscribe":
                    return RequestMessageType.PrivateChannelOnUnsubscribe;
                case "broadcastRequest":
                    return RequestMessageType.BroadcastRequest;
                case "findInstancesRequest":
                    return RequestMessageType.FindInstancesRequest;
                case "findIntentRequest":
                    return RequestMessageType.FindIntentRequest;
                case "findIntentsByContextRequest":
                    return RequestMessageType.FindIntentsByContextRequest;
                case "getAppMetadataRequest":
                    return RequestMessageType.GetAppMetadataRequest;
                case "openRequest":
                    return RequestMessageType.OpenRequest;
                case "raiseIntentRequest":
                    return RequestMessageType.RaiseIntentRequest;
            }
            throw new Exception("Cannot unmarshal type RequestMessageType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (RequestMessageType)untypedValue;
            switch (value)
            {
                case RequestMessageType.PrivateChannelBroadcast:
                    serializer.Serialize(writer, "PrivateChannel.broadcast");
                    return;
                case RequestMessageType.PrivateChannelEventListenerAdded:
                    serializer.Serialize(writer, "PrivateChannel.eventListenerAdded");
                    return;
                case RequestMessageType.PrivateChannelOnAddContextListener:
                    serializer.Serialize(writer, "PrivateChannel.onAddContextListener");
                    return;
                case RequestMessageType.PrivateChannelOnDisconnect:
                    serializer.Serialize(writer, "PrivateChannel.onDisconnect");
                    return;
                case RequestMessageType.PrivateChannelOnUnsubscribe:
                    serializer.Serialize(writer, "PrivateChannel.onUnsubscribe");
                    return;
                case RequestMessageType.BroadcastRequest:
                    serializer.Serialize(writer, "broadcastRequest");
                    return;
                case RequestMessageType.FindInstancesRequest:
                    serializer.Serialize(writer, "findInstancesRequest");
                    return;
                case RequestMessageType.FindIntentRequest:
                    serializer.Serialize(writer, "findIntentRequest");
                    return;
                case RequestMessageType.FindIntentsByContextRequest:
                    serializer.Serialize(writer, "findIntentsByContextRequest");
                    return;
                case RequestMessageType.GetAppMetadataRequest:
                    serializer.Serialize(writer, "getAppMetadataRequest");
                    return;
                case RequestMessageType.OpenRequest:
                    serializer.Serialize(writer, "openRequest");
                    return;
                case RequestMessageType.RaiseIntentRequest:
                    serializer.Serialize(writer, "raiseIntentRequest");
                    return;
            }
            throw new Exception("Cannot marshal type RequestMessageType");
        }

        public static readonly RequestMessageTypeConverter Singleton = new RequestMessageTypeConverter();
    }

    internal class ConnectionStepMessageTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ConnectionStepMessageType) || t == typeof(ConnectionStepMessageType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "authenticationFailed":
                    return ConnectionStepMessageType.AuthenticationFailed;
                case "connectedAgentsUpdate":
                    return ConnectionStepMessageType.ConnectedAgentsUpdate;
                case "handshake":
                    return ConnectionStepMessageType.Handshake;
                case "hello":
                    return ConnectionStepMessageType.Hello;
            }
            throw new Exception("Cannot unmarshal type ConnectionStepMessageType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (ConnectionStepMessageType)untypedValue;
            switch (value)
            {
                case ConnectionStepMessageType.AuthenticationFailed:
                    serializer.Serialize(writer, "authenticationFailed");
                    return;
                case ConnectionStepMessageType.ConnectedAgentsUpdate:
                    serializer.Serialize(writer, "connectedAgentsUpdate");
                    return;
                case ConnectionStepMessageType.Handshake:
                    serializer.Serialize(writer, "handshake");
                    return;
                case ConnectionStepMessageType.Hello:
                    serializer.Serialize(writer, "hello");
                    return;
            }
            throw new Exception("Cannot marshal type ConnectionStepMessageType");
        }

        public static readonly ConnectionStepMessageTypeConverter Singleton = new ConnectionStepMessageTypeConverter();
    }

    internal class ErrorMessageConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ErrorMessage) || t == typeof(ErrorMessage?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "AgentDisconnected":
                    return ErrorMessage.AgentDisconnected;
                case "DesktopAgentNotFound":
                    return ErrorMessage.DesktopAgentNotFound;
                case "IntentDeliveryFailed":
                    return ErrorMessage.IntentDeliveryFailed;
                case "MalformedContext":
                    return ErrorMessage.MalformedContext;
                case "MalformedMessage":
                    return ErrorMessage.MalformedMessage;
                case "NoAppsFound":
                    return ErrorMessage.NoAppsFound;
                case "NotConnectedToBridge":
                    return ErrorMessage.NotConnectedToBridge;
                case "ResolverTimeout":
                    return ErrorMessage.ResolverTimeout;
                case "ResolverUnavailable":
                    return ErrorMessage.ResolverUnavailable;
                case "ResponseToBridgeTimedOut":
                    return ErrorMessage.ResponseToBridgeTimedOut;
                case "TargetAppUnavailable":
                    return ErrorMessage.TargetAppUnavailable;
                case "TargetInstanceUnavailable":
                    return ErrorMessage.TargetInstanceUnavailable;
                case "UserCancelledResolution":
                    return ErrorMessage.UserCancelledResolution;
            }
            throw new Exception("Cannot unmarshal type ErrorMessage");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (ErrorMessage)untypedValue;
            switch (value)
            {
                case ErrorMessage.AgentDisconnected:
                    serializer.Serialize(writer, "AgentDisconnected");
                    return;
                case ErrorMessage.DesktopAgentNotFound:
                    serializer.Serialize(writer, "DesktopAgentNotFound");
                    return;
                case ErrorMessage.IntentDeliveryFailed:
                    serializer.Serialize(writer, "IntentDeliveryFailed");
                    return;
                case ErrorMessage.MalformedContext:
                    serializer.Serialize(writer, "MalformedContext");
                    return;
                case ErrorMessage.MalformedMessage:
                    serializer.Serialize(writer, "MalformedMessage");
                    return;
                case ErrorMessage.NoAppsFound:
                    serializer.Serialize(writer, "NoAppsFound");
                    return;
                case ErrorMessage.NotConnectedToBridge:
                    serializer.Serialize(writer, "NotConnectedToBridge");
                    return;
                case ErrorMessage.ResolverTimeout:
                    serializer.Serialize(writer, "ResolverTimeout");
                    return;
                case ErrorMessage.ResolverUnavailable:
                    serializer.Serialize(writer, "ResolverUnavailable");
                    return;
                case ErrorMessage.ResponseToBridgeTimedOut:
                    serializer.Serialize(writer, "ResponseToBridgeTimedOut");
                    return;
                case ErrorMessage.TargetAppUnavailable:
                    serializer.Serialize(writer, "TargetAppUnavailable");
                    return;
                case ErrorMessage.TargetInstanceUnavailable:
                    serializer.Serialize(writer, "TargetInstanceUnavailable");
                    return;
                case ErrorMessage.UserCancelledResolution:
                    serializer.Serialize(writer, "UserCancelledResolution");
                    return;
            }
            throw new Exception("Cannot marshal type ErrorMessage");
        }

        public static readonly ErrorMessageConverter Singleton = new ErrorMessageConverter();
    }

    internal class OpenErrorMessageConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(OpenErrorMessage) || t == typeof(OpenErrorMessage?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "AgentDisconnected":
                    return OpenErrorMessage.AgentDisconnected;
                case "AppNotFound":
                    return OpenErrorMessage.AppNotFound;
                case "AppTimeout":
                    return OpenErrorMessage.AppTimeout;
                case "DesktopAgentNotFound":
                    return OpenErrorMessage.DesktopAgentNotFound;
                case "ErrorOnLaunch":
                    return OpenErrorMessage.ErrorOnLaunch;
                case "MalformedContext":
                    return OpenErrorMessage.MalformedContext;
                case "MalformedMessage":
                    return OpenErrorMessage.MalformedMessage;
                case "NotConnectedToBridge":
                    return OpenErrorMessage.NotConnectedToBridge;
                case "ResolverUnavailable":
                    return OpenErrorMessage.ResolverUnavailable;
                case "ResponseToBridgeTimedOut":
                    return OpenErrorMessage.ResponseToBridgeTimedOut;
            }
            throw new Exception("Cannot unmarshal type OpenErrorMessage");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (OpenErrorMessage)untypedValue;
            switch (value)
            {
                case OpenErrorMessage.AgentDisconnected:
                    serializer.Serialize(writer, "AgentDisconnected");
                    return;
                case OpenErrorMessage.AppNotFound:
                    serializer.Serialize(writer, "AppNotFound");
                    return;
                case OpenErrorMessage.AppTimeout:
                    serializer.Serialize(writer, "AppTimeout");
                    return;
                case OpenErrorMessage.DesktopAgentNotFound:
                    serializer.Serialize(writer, "DesktopAgentNotFound");
                    return;
                case OpenErrorMessage.ErrorOnLaunch:
                    serializer.Serialize(writer, "ErrorOnLaunch");
                    return;
                case OpenErrorMessage.MalformedContext:
                    serializer.Serialize(writer, "MalformedContext");
                    return;
                case OpenErrorMessage.MalformedMessage:
                    serializer.Serialize(writer, "MalformedMessage");
                    return;
                case OpenErrorMessage.NotConnectedToBridge:
                    serializer.Serialize(writer, "NotConnectedToBridge");
                    return;
                case OpenErrorMessage.ResolverUnavailable:
                    serializer.Serialize(writer, "ResolverUnavailable");
                    return;
                case OpenErrorMessage.ResponseToBridgeTimedOut:
                    serializer.Serialize(writer, "ResponseToBridgeTimedOut");
                    return;
            }
            throw new Exception("Cannot marshal type OpenErrorMessage");
        }

        public static readonly OpenErrorMessageConverter Singleton = new OpenErrorMessageConverter();
    }

    internal class EmptyConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Empty) || t == typeof(Empty?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "onAddContextListener":
                    return Empty.OnAddContextListener;
                case "onDisconnect":
                    return Empty.OnDisconnect;
                case "onUnsubscribe":
                    return Empty.OnUnsubscribe;
            }
            throw new Exception("Cannot unmarshal type Empty");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Empty)untypedValue;
            switch (value)
            {
                case Empty.OnAddContextListener:
                    serializer.Serialize(writer, "onAddContextListener");
                    return;
                case Empty.OnDisconnect:
                    serializer.Serialize(writer, "onDisconnect");
                    return;
                case Empty.OnUnsubscribe:
                    serializer.Serialize(writer, "onUnsubscribe");
                    return;
            }
            throw new Exception("Cannot marshal type Empty");
        }

        public static readonly EmptyConverter Singleton = new EmptyConverter();
    }

    internal class RaiseIntentResultErrorMessageConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(RaiseIntentResultErrorMessage) || t == typeof(RaiseIntentResultErrorMessage?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "AgentDisconnected":
                    return RaiseIntentResultErrorMessage.AgentDisconnected;
                case "IntentHandlerRejected":
                    return RaiseIntentResultErrorMessage.IntentHandlerRejected;
                case "MalformedMessage":
                    return RaiseIntentResultErrorMessage.MalformedMessage;
                case "NoResultReturned":
                    return RaiseIntentResultErrorMessage.NoResultReturned;
                case "NotConnectedToBridge":
                    return RaiseIntentResultErrorMessage.NotConnectedToBridge;
                case "ResponseToBridgeTimedOut":
                    return RaiseIntentResultErrorMessage.ResponseToBridgeTimedOut;
            }
            throw new Exception("Cannot unmarshal type RaiseIntentResultErrorMessage");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (RaiseIntentResultErrorMessage)untypedValue;
            switch (value)
            {
                case RaiseIntentResultErrorMessage.AgentDisconnected:
                    serializer.Serialize(writer, "AgentDisconnected");
                    return;
                case RaiseIntentResultErrorMessage.IntentHandlerRejected:
                    serializer.Serialize(writer, "IntentHandlerRejected");
                    return;
                case RaiseIntentResultErrorMessage.MalformedMessage:
                    serializer.Serialize(writer, "MalformedMessage");
                    return;
                case RaiseIntentResultErrorMessage.NoResultReturned:
                    serializer.Serialize(writer, "NoResultReturned");
                    return;
                case RaiseIntentResultErrorMessage.NotConnectedToBridge:
                    serializer.Serialize(writer, "NotConnectedToBridge");
                    return;
                case RaiseIntentResultErrorMessage.ResponseToBridgeTimedOut:
                    serializer.Serialize(writer, "ResponseToBridgeTimedOut");
                    return;
            }
            throw new Exception("Cannot marshal type RaiseIntentResultErrorMessage");
        }

        public static readonly RaiseIntentResultErrorMessageConverter Singleton = new RaiseIntentResultErrorMessageConverter();
    }

    internal class TypeEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(TypeEnum) || t == typeof(TypeEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "app":
                    return TypeEnum.App;
                case "private":
                    return TypeEnum.Private;
                case "user":
                    return TypeEnum.User;
            }
            throw new Exception("Cannot unmarshal type TypeEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (TypeEnum)untypedValue;
            switch (value)
            {
                case TypeEnum.App:
                    serializer.Serialize(writer, "app");
                    return;
                case TypeEnum.Private:
                    serializer.Serialize(writer, "private");
                    return;
                case TypeEnum.User:
                    serializer.Serialize(writer, "user");
                    return;
            }
            throw new Exception("Cannot marshal type TypeEnum");
        }

        public static readonly TypeEnumConverter Singleton = new TypeEnumConverter();
    }
}
