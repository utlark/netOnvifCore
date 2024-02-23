﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace netOnvifCore.Replay
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://www.onvif.org/ver10/replay/wsdl", ConfigurationName="netOnvifCore.Replay.ReplayPort")]
    public interface ReplayPort
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.onvif.org/ver10/replay/wsdl/GetServiceCapabilities", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [return: System.ServiceModel.MessageParameterAttribute(Name="Capabilities")]
        System.Threading.Tasks.Task<netOnvifCore.Replay.Capabilities> GetServiceCapabilitiesAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.onvif.org/ver10/replay/wsdl/GetReplayUri", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<netOnvifCore.Replay.GetReplayUriResponse> GetReplayUriAsync(netOnvifCore.Replay.GetReplayUriRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.onvif.org/ver10/replay/wsdl/GetReplayConfiguration", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [return: System.ServiceModel.MessageParameterAttribute(Name="Configuration")]
        System.Threading.Tasks.Task<netOnvifCore.Replay.ReplayConfiguration> GetReplayConfigurationAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.onvif.org/ver10/replay/wsdl/SetReplayConfiguration", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task SetReplayConfigurationAsync(netOnvifCore.Replay.ReplayConfiguration Configuration);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.onvif.org/ver10/replay/wsdl")]
    public partial class Capabilities
    {
        
        private System.Xml.XmlElement[] anyField;
        
        private bool reversePlaybackField;
        
        private bool reversePlaybackFieldSpecified;
        
        private float[] sessionTimeoutRangeField;
        
        private bool rTP_RTSP_TCPField;
        
        private bool rTP_RTSP_TCPFieldSpecified;
        
        private string rTSPWebSocketUriField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute(Order=0)]
        public System.Xml.XmlElement[] Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool ReversePlayback
        {
            get
            {
                return this.reversePlaybackField;
            }
            set
            {
                this.reversePlaybackField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ReversePlaybackSpecified
        {
            get
            {
                return this.reversePlaybackFieldSpecified;
            }
            set
            {
                this.reversePlaybackFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public float[] SessionTimeoutRange
        {
            get
            {
                return this.sessionTimeoutRangeField;
            }
            set
            {
                this.sessionTimeoutRangeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool RTP_RTSP_TCP
        {
            get
            {
                return this.rTP_RTSP_TCPField;
            }
            set
            {
                this.rTP_RTSP_TCPField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool RTP_RTSP_TCPSpecified
        {
            get
            {
                return this.rTP_RTSP_TCPFieldSpecified;
            }
            set
            {
                this.rTP_RTSP_TCPFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="anyURI")]
        public string RTSPWebSocketUri
        {
            get
            {
                return this.rTSPWebSocketUriField;
            }
            set
            {
                this.rTSPWebSocketUriField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.onvif.org/ver10/schema")]
    public partial class ReplayConfiguration
    {
        
        private string sessionTimeoutField;
        
        private System.Xml.XmlElement[] anyField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="duration", Order=0)]
        public string SessionTimeout
        {
            get
            {
                return this.sessionTimeoutField;
            }
            set
            {
                this.sessionTimeoutField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute(Order=1)]
        public System.Xml.XmlElement[] Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.onvif.org/ver10/schema")]
    public partial class Transport
    {
        
        private TransportProtocol protocolField;
        
        private Transport tunnelField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public TransportProtocol Protocol
        {
            get
            {
                return this.protocolField;
            }
            set
            {
                this.protocolField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public Transport Tunnel
        {
            get
            {
                return this.tunnelField;
            }
            set
            {
                this.tunnelField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.onvif.org/ver10/schema")]
    public enum TransportProtocol
    {
        
        /// <remarks/>
        UDP,
        
        /// <remarks/>
        TCP,
        
        /// <remarks/>
        RTSP,
        
        /// <remarks/>
        HTTP,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.onvif.org/ver10/schema")]
    public partial class StreamSetup
    {
        
        private StreamType streamField;
        
        private Transport transportField;
        
        private System.Xml.XmlElement[] anyField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public StreamType Stream
        {
            get
            {
                return this.streamField;
            }
            set
            {
                this.streamField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public Transport Transport
        {
            get
            {
                return this.transportField;
            }
            set
            {
                this.transportField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute(Order=2)]
        public System.Xml.XmlElement[] Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.onvif.org/ver10/schema")]
    public enum StreamType
    {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("RTP-Unicast")]
        RTPUnicast,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("RTP-Multicast")]
        RTPMulticast,
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetReplayUri", WrapperNamespace="http://www.onvif.org/ver10/replay/wsdl", IsWrapped=true)]
    public partial class GetReplayUriRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.onvif.org/ver10/replay/wsdl", Order=0)]
        public netOnvifCore.Replay.StreamSetup StreamSetup;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.onvif.org/ver10/replay/wsdl", Order=1)]
        public string RecordingToken;
        
        public GetReplayUriRequest()
        {
        }
        
        public GetReplayUriRequest(netOnvifCore.Replay.StreamSetup StreamSetup, string RecordingToken)
        {
            this.StreamSetup = StreamSetup;
            this.RecordingToken = RecordingToken;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetReplayUriResponse", WrapperNamespace="http://www.onvif.org/ver10/replay/wsdl", IsWrapped=true)]
    public partial class GetReplayUriResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.onvif.org/ver10/replay/wsdl", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(DataType="anyURI")]
        public string Uri;
        
        public GetReplayUriResponse()
        {
        }
        
        public GetReplayUriResponse(string Uri)
        {
            this.Uri = Uri;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    public interface ReplayPortChannel : netOnvifCore.Replay.ReplayPort, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    public partial class ReplayPortClient : System.ServiceModel.ClientBase<netOnvifCore.Replay.ReplayPort>, netOnvifCore.Replay.ReplayPort
    {
        
        public ReplayPortClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public System.Threading.Tasks.Task<netOnvifCore.Replay.Capabilities> GetServiceCapabilitiesAsync()
        {
            return base.Channel.GetServiceCapabilitiesAsync();
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<netOnvifCore.Replay.GetReplayUriResponse> netOnvifCore.Replay.ReplayPort.GetReplayUriAsync(netOnvifCore.Replay.GetReplayUriRequest request)
        {
            return base.Channel.GetReplayUriAsync(request);
        }
        
        public System.Threading.Tasks.Task<netOnvifCore.Replay.GetReplayUriResponse> GetReplayUriAsync(netOnvifCore.Replay.StreamSetup StreamSetup, string RecordingToken)
        {
            netOnvifCore.Replay.GetReplayUriRequest inValue = new netOnvifCore.Replay.GetReplayUriRequest();
            inValue.StreamSetup = StreamSetup;
            inValue.RecordingToken = RecordingToken;
            return ((netOnvifCore.Replay.ReplayPort)(this)).GetReplayUriAsync(inValue);
        }
        
        public System.Threading.Tasks.Task<netOnvifCore.Replay.ReplayConfiguration> GetReplayConfigurationAsync()
        {
            return base.Channel.GetReplayConfigurationAsync();
        }
        
        public System.Threading.Tasks.Task SetReplayConfigurationAsync(netOnvifCore.Replay.ReplayConfiguration Configuration)
        {
            return base.Channel.SetReplayConfigurationAsync(Configuration);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
    }
}
