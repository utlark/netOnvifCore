using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace netOnvifCore.Security;

public class SoapSecurityHeaderInspector : IClientMessageInspector
{
    private readonly string   _password;
    private readonly TimeSpan _timeShift;
    private readonly string   _username;

    public SoapSecurityHeaderInspector(string username, string password, TimeSpan timeShift)
    {
        _username  = username;
        _password  = password;
        _timeShift = timeShift;
    }

    public void AfterReceiveReply(ref Message reply, object correlationState)
    {
        var headerIndex = reply.Headers.FindHeader("Security", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
        if (headerIndex >= 0)
            reply.Headers.RemoveAt(headerIndex);
    }

    public object? BeforeSendRequest(ref Message request, IClientChannel channel)
    {
        request.Headers.Add(new SoapSecurityHeader(_username, _password, _timeShift));
        return null;
    }
}