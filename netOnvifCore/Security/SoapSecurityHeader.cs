using System.Security.Cryptography;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;

namespace netOnvifCore.Security;

public class SoapSecurityHeader : MessageHeader, IDisposable
{
    private readonly string _created;

    private readonly RandomNumberGenerator _numberGenerator = RandomNumberGenerator.Create();
    private readonly string                _password;
    private readonly string                _username;

    private byte[]? _nonce;

    public SoapSecurityHeader(string username, string password, TimeSpan timeShift)
    {
        _username = username;
        _password = password;
        _created  = DateTime.UtcNow.Add(timeShift).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    }

    public override string Name      => "Security";
    public override string Namespace => "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";

    private byte[] Nonce
    {
        get
        {
            if (_nonce != null) return _nonce;
            _nonce = new byte[0x10];
            _numberGenerator.GetBytes(_nonce);
            return _nonce;
        }
    }

    public void Dispose() => _numberGenerator.Dispose();

    protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
    {
        writer.WriteStartElement("UsernameToken");

        writer.WriteStartElement("Username");
        writer.WriteValue(_username);
        writer.WriteEndElement();

        writer.WriteStartElement("Password");
        writer.WriteAttributeString("Type", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordDigest");
        writer.WriteValue(PasswordDigest(Nonce, _created, _password));
        writer.WriteEndElement();

        writer.WriteStartElement("Nonce");
        writer.WriteAttributeString("EncodingType", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary");
        writer.WriteValue(Convert.ToBase64String(Nonce));
        writer.WriteEndElement();

        writer.WriteStartElement("Created");
        writer.WriteXmlnsAttribute("", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
        writer.WriteValue(_created);
        writer.WriteEndElement();

        writer.WriteEndElement();
    }

    protected override void OnWriteStartHeader(XmlDictionaryWriter writer, MessageVersion messageVersion)
    {
        writer.WriteStartElement("", Name, Namespace);
        writer.WriteAttributeString("s", "mustUnderstand", "http://www.w3.org/2003/05/soap-envelope", "1");
        writer.WriteXmlnsAttribute("", Namespace);
    }

    private static string PasswordDigest(IEnumerable<byte> nonce, string created, string secret) =>
        Convert.ToBase64String(SHA1.HashData(nonce.Concat(Encoding.UTF8.GetBytes(created + secret)).ToArray()));
}