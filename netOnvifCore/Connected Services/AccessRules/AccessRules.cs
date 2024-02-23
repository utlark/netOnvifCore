﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace netOnvifCore.AccessRules
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://www.onvif.org/ver10/accessrules/wsdl", ConfigurationName="netOnvifCore.AccessRules.AccessRulesPort")]
    public interface AccessRulesPort
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.onvif.org/ver10/accessrules/wsdl/GetServiceCapabilities", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DataEntity))]
        [return: System.ServiceModel.MessageParameterAttribute(Name="Capabilities")]
        System.Threading.Tasks.Task<netOnvifCore.AccessRules.ServiceCapabilities> GetServiceCapabilitiesAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.onvif.org/ver10/accessrules/wsdl/GetAccessProfileInfo", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DataEntity))]
        System.Threading.Tasks.Task<netOnvifCore.AccessRules.GetAccessProfileInfoResponse> GetAccessProfileInfoAsync(netOnvifCore.AccessRules.GetAccessProfileInfoRequest request);
        
        // CODEGEN: Generating message contract since the operation has multiple return values.
        [System.ServiceModel.OperationContractAttribute(Action="http://www.onvif.org/ver10/accessrules/wsdl/GetAccessProfileInfoList", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DataEntity))]
        System.Threading.Tasks.Task<netOnvifCore.AccessRules.GetAccessProfileInfoListResponse> GetAccessProfileInfoListAsync(netOnvifCore.AccessRules.GetAccessProfileInfoListRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.onvif.org/ver10/accessrules/wsdl/GetAccessProfiles", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DataEntity))]
        System.Threading.Tasks.Task<netOnvifCore.AccessRules.GetAccessProfilesResponse> GetAccessProfilesAsync(netOnvifCore.AccessRules.GetAccessProfilesRequest request);
        
        // CODEGEN: Generating message contract since the operation has multiple return values.
        [System.ServiceModel.OperationContractAttribute(Action="http://www.onvif.org/ver10/accessrules/wsdl/GetAccessProfileList", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DataEntity))]
        System.Threading.Tasks.Task<netOnvifCore.AccessRules.GetAccessProfileListResponse> GetAccessProfileListAsync(netOnvifCore.AccessRules.GetAccessProfileListRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.onvif.org/ver10/accessrules/wsdl/CreateAccessProfile", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DataEntity))]
        [return: System.ServiceModel.MessageParameterAttribute(Name="Token")]
        System.Threading.Tasks.Task<string> CreateAccessProfileAsync(netOnvifCore.AccessRules.AccessProfile AccessProfile);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.onvif.org/ver10/accessrules/wsdl/ModifyAccessProfile", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DataEntity))]
        System.Threading.Tasks.Task ModifyAccessProfileAsync(netOnvifCore.AccessRules.AccessProfile AccessProfile);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.onvif.org/ver10/accessrules/wsdl/SetAccessProfile", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DataEntity))]
        System.Threading.Tasks.Task SetAccessProfileAsync(netOnvifCore.AccessRules.AccessProfile AccessProfile);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.onvif.org/ver10/accessrules/wsdl/DeleteAccessProfile", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DataEntity))]
        System.Threading.Tasks.Task DeleteAccessProfileAsync(string Token);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.onvif.org/ver10/accessrules/wsdl")]
    public partial class ServiceCapabilities
    {
        
        private System.Xml.XmlElement[] anyField;
        
        private uint maxLimitField;
        
        private uint maxAccessProfilesField;
        
        private uint maxAccessPoliciesPerAccessProfileField;
        
        private bool multipleSchedulesPerAccessPointSupportedField;
        
        private bool clientSuppliedTokenSupportedField;
        
        public ServiceCapabilities()
        {
            this.clientSuppliedTokenSupportedField = false;
        }
        
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
        public uint MaxLimit
        {
            get
            {
                return this.maxLimitField;
            }
            set
            {
                this.maxLimitField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint MaxAccessProfiles
        {
            get
            {
                return this.maxAccessProfilesField;
            }
            set
            {
                this.maxAccessProfilesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint MaxAccessPoliciesPerAccessProfile
        {
            get
            {
                return this.maxAccessPoliciesPerAccessProfileField;
            }
            set
            {
                this.maxAccessPoliciesPerAccessProfileField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool MultipleSchedulesPerAccessPointSupported
        {
            get
            {
                return this.multipleSchedulesPerAccessPointSupportedField;
            }
            set
            {
                this.multipleSchedulesPerAccessPointSupportedField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool ClientSuppliedTokenSupported
        {
            get
            {
                return this.clientSuppliedTokenSupportedField;
            }
            set
            {
                this.clientSuppliedTokenSupportedField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.onvif.org/ver10/accessrules/wsdl")]
    public partial class AccessProfileExtension
    {
        
        private System.Xml.XmlElement[] anyField;
        
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
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.onvif.org/ver10/accessrules/wsdl")]
    public partial class AccessPolicyExtension
    {
        
        private System.Xml.XmlElement[] anyField;
        
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
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.onvif.org/ver10/accessrules/wsdl")]
    public partial class AccessPolicy
    {
        
        private string scheduleTokenField;
        
        private string entityField;
        
        private System.Xml.XmlQualifiedName entityTypeField;
        
        private AccessPolicyExtension extensionField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string ScheduleToken
        {
            get
            {
                return this.scheduleTokenField;
            }
            set
            {
                this.scheduleTokenField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string Entity
        {
            get
            {
                return this.entityField;
            }
            set
            {
                this.entityField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public System.Xml.XmlQualifiedName EntityType
        {
            get
            {
                return this.entityTypeField;
            }
            set
            {
                this.entityTypeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public AccessPolicyExtension Extension
        {
            get
            {
                return this.extensionField;
            }
            set
            {
                this.extensionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AccessProfileInfo))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AccessProfile))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.onvif.org/ver10/pacs")]
    public partial class DataEntity
    {
        
        private string tokenField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string token
        {
            get
            {
                return this.tokenField;
            }
            set
            {
                this.tokenField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AccessProfile))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.onvif.org/ver10/accessrules/wsdl")]
    public partial class AccessProfileInfo : DataEntity
    {
        
        private string nameField;
        
        private string descriptionField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.onvif.org/ver10/accessrules/wsdl")]
    public partial class AccessProfile : AccessProfileInfo
    {
        
        private AccessPolicy[] accessPolicyField;
        
        private AccessProfileExtension extensionField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("AccessPolicy", Order=0)]
        public AccessPolicy[] AccessPolicy
        {
            get
            {
                return this.accessPolicyField;
            }
            set
            {
                this.accessPolicyField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public AccessProfileExtension Extension
        {
            get
            {
                return this.extensionField;
            }
            set
            {
                this.extensionField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetAccessProfileInfo", WrapperNamespace="http://www.onvif.org/ver10/accessrules/wsdl", IsWrapped=true)]
    public partial class GetAccessProfileInfoRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.onvif.org/ver10/accessrules/wsdl", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute("Token")]
        public string[] Token;
        
        public GetAccessProfileInfoRequest()
        {
        }
        
        public GetAccessProfileInfoRequest(string[] Token)
        {
            this.Token = Token;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetAccessProfileInfoResponse", WrapperNamespace="http://www.onvif.org/ver10/accessrules/wsdl", IsWrapped=true)]
    public partial class GetAccessProfileInfoResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.onvif.org/ver10/accessrules/wsdl", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute("AccessProfileInfo")]
        public netOnvifCore.AccessRules.AccessProfileInfo[] AccessProfileInfo;
        
        public GetAccessProfileInfoResponse()
        {
        }
        
        public GetAccessProfileInfoResponse(netOnvifCore.AccessRules.AccessProfileInfo[] AccessProfileInfo)
        {
            this.AccessProfileInfo = AccessProfileInfo;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetAccessProfileInfoList", WrapperNamespace="http://www.onvif.org/ver10/accessrules/wsdl", IsWrapped=true)]
    public partial class GetAccessProfileInfoListRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.onvif.org/ver10/accessrules/wsdl", Order=0)]
        public int Limit;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.onvif.org/ver10/accessrules/wsdl", Order=1)]
        public string StartReference;
        
        public GetAccessProfileInfoListRequest()
        {
        }
        
        public GetAccessProfileInfoListRequest(int Limit, string StartReference)
        {
            this.Limit = Limit;
            this.StartReference = StartReference;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetAccessProfileInfoListResponse", WrapperNamespace="http://www.onvif.org/ver10/accessrules/wsdl", IsWrapped=true)]
    public partial class GetAccessProfileInfoListResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.onvif.org/ver10/accessrules/wsdl", Order=0)]
        public string NextStartReference;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.onvif.org/ver10/accessrules/wsdl", Order=1)]
        [System.Xml.Serialization.XmlElementAttribute("AccessProfileInfo")]
        public netOnvifCore.AccessRules.AccessProfileInfo[] AccessProfileInfo;
        
        public GetAccessProfileInfoListResponse()
        {
        }
        
        public GetAccessProfileInfoListResponse(string NextStartReference, netOnvifCore.AccessRules.AccessProfileInfo[] AccessProfileInfo)
        {
            this.NextStartReference = NextStartReference;
            this.AccessProfileInfo = AccessProfileInfo;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetAccessProfiles", WrapperNamespace="http://www.onvif.org/ver10/accessrules/wsdl", IsWrapped=true)]
    public partial class GetAccessProfilesRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.onvif.org/ver10/accessrules/wsdl", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute("Token")]
        public string[] Token;
        
        public GetAccessProfilesRequest()
        {
        }
        
        public GetAccessProfilesRequest(string[] Token)
        {
            this.Token = Token;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetAccessProfilesResponse", WrapperNamespace="http://www.onvif.org/ver10/accessrules/wsdl", IsWrapped=true)]
    public partial class GetAccessProfilesResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.onvif.org/ver10/accessrules/wsdl", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute("AccessProfile")]
        public netOnvifCore.AccessRules.AccessProfile[] AccessProfile;
        
        public GetAccessProfilesResponse()
        {
        }
        
        public GetAccessProfilesResponse(netOnvifCore.AccessRules.AccessProfile[] AccessProfile)
        {
            this.AccessProfile = AccessProfile;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetAccessProfileList", WrapperNamespace="http://www.onvif.org/ver10/accessrules/wsdl", IsWrapped=true)]
    public partial class GetAccessProfileListRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.onvif.org/ver10/accessrules/wsdl", Order=0)]
        public int Limit;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.onvif.org/ver10/accessrules/wsdl", Order=1)]
        public string StartReference;
        
        public GetAccessProfileListRequest()
        {
        }
        
        public GetAccessProfileListRequest(int Limit, string StartReference)
        {
            this.Limit = Limit;
            this.StartReference = StartReference;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetAccessProfileListResponse", WrapperNamespace="http://www.onvif.org/ver10/accessrules/wsdl", IsWrapped=true)]
    public partial class GetAccessProfileListResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.onvif.org/ver10/accessrules/wsdl", Order=0)]
        public string NextStartReference;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.onvif.org/ver10/accessrules/wsdl", Order=1)]
        [System.Xml.Serialization.XmlElementAttribute("AccessProfile")]
        public netOnvifCore.AccessRules.AccessProfile[] AccessProfile;
        
        public GetAccessProfileListResponse()
        {
        }
        
        public GetAccessProfileListResponse(string NextStartReference, netOnvifCore.AccessRules.AccessProfile[] AccessProfile)
        {
            this.NextStartReference = NextStartReference;
            this.AccessProfile = AccessProfile;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    public interface AccessRulesPortChannel : netOnvifCore.AccessRules.AccessRulesPort, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    public partial class AccessRulesPortClient : System.ServiceModel.ClientBase<netOnvifCore.AccessRules.AccessRulesPort>, netOnvifCore.AccessRules.AccessRulesPort
    {
        
        public AccessRulesPortClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public System.Threading.Tasks.Task<netOnvifCore.AccessRules.ServiceCapabilities> GetServiceCapabilitiesAsync()
        {
            return base.Channel.GetServiceCapabilitiesAsync();
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<netOnvifCore.AccessRules.GetAccessProfileInfoResponse> netOnvifCore.AccessRules.AccessRulesPort.GetAccessProfileInfoAsync(netOnvifCore.AccessRules.GetAccessProfileInfoRequest request)
        {
            return base.Channel.GetAccessProfileInfoAsync(request);
        }
        
        public System.Threading.Tasks.Task<netOnvifCore.AccessRules.GetAccessProfileInfoResponse> GetAccessProfileInfoAsync(string[] Token)
        {
            netOnvifCore.AccessRules.GetAccessProfileInfoRequest inValue = new netOnvifCore.AccessRules.GetAccessProfileInfoRequest();
            inValue.Token = Token;
            return ((netOnvifCore.AccessRules.AccessRulesPort)(this)).GetAccessProfileInfoAsync(inValue);
        }
        
        public System.Threading.Tasks.Task<netOnvifCore.AccessRules.GetAccessProfileInfoListResponse> GetAccessProfileInfoListAsync(netOnvifCore.AccessRules.GetAccessProfileInfoListRequest request)
        {
            return base.Channel.GetAccessProfileInfoListAsync(request);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<netOnvifCore.AccessRules.GetAccessProfilesResponse> netOnvifCore.AccessRules.AccessRulesPort.GetAccessProfilesAsync(netOnvifCore.AccessRules.GetAccessProfilesRequest request)
        {
            return base.Channel.GetAccessProfilesAsync(request);
        }
        
        public System.Threading.Tasks.Task<netOnvifCore.AccessRules.GetAccessProfilesResponse> GetAccessProfilesAsync(string[] Token)
        {
            netOnvifCore.AccessRules.GetAccessProfilesRequest inValue = new netOnvifCore.AccessRules.GetAccessProfilesRequest();
            inValue.Token = Token;
            return ((netOnvifCore.AccessRules.AccessRulesPort)(this)).GetAccessProfilesAsync(inValue);
        }
        
        public System.Threading.Tasks.Task<netOnvifCore.AccessRules.GetAccessProfileListResponse> GetAccessProfileListAsync(netOnvifCore.AccessRules.GetAccessProfileListRequest request)
        {
            return base.Channel.GetAccessProfileListAsync(request);
        }
        
        public System.Threading.Tasks.Task<string> CreateAccessProfileAsync(netOnvifCore.AccessRules.AccessProfile AccessProfile)
        {
            return base.Channel.CreateAccessProfileAsync(AccessProfile);
        }
        
        public System.Threading.Tasks.Task ModifyAccessProfileAsync(netOnvifCore.AccessRules.AccessProfile AccessProfile)
        {
            return base.Channel.ModifyAccessProfileAsync(AccessProfile);
        }
        
        public System.Threading.Tasks.Task SetAccessProfileAsync(netOnvifCore.AccessRules.AccessProfile AccessProfile)
        {
            return base.Channel.SetAccessProfileAsync(AccessProfile);
        }
        
        public System.Threading.Tasks.Task DeleteAccessProfileAsync(string Token)
        {
            return base.Channel.DeleteAccessProfileAsync(Token);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
    }
}
