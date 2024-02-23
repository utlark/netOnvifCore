using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace netOnvifCore.Security;

public class SoapSecurityHeaderInspector(string username, string password, TimeSpan timeShift) : IClientMessageInspector
{
    public void AfterReceiveReply(ref Message reply, object correlationState)
    {
        var headerIndex = reply.Headers.FindHeader("Security", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
        if (headerIndex >= 0)
            reply.Headers.RemoveAt(headerIndex);
    }

    public object? BeforeSendRequest(ref Message request, IClientChannel channel)
    {
        request.Headers.Add(new SoapSecurityHeader(username, password, timeShift));
        return null;
    }
}