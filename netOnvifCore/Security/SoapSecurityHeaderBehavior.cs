using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace netOnvifCore.Security;

public class SoapSecurityHeaderBehavior : IEndpointBehavior
{
    private readonly string   _password;
    private readonly TimeSpan _timeShift;
    private readonly string   _username;

    public SoapSecurityHeaderBehavior(string username, string password, TimeSpan timeShift)
    {
        _username  = username;
        _password  = password;
        _timeShift = timeShift;
    }

    public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
    {
    }

    public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
    {
        clientRuntime.ClientMessageInspectors.Add(new SoapSecurityHeaderInspector(_username, _password, _timeShift));
    }

    public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
    {
    }

    public void Validate(ServiceEndpoint endpoint)
    {
    }
}