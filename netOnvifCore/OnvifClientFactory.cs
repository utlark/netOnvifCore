using System.ServiceModel;
using System.ServiceModel.Channels;
using netOnvifCore.DeviceManagement;
using netOnvifCore.Imaging;
using netOnvifCore.Media;
using netOnvifCore.Media2;
using netOnvifCore.Ptz;
using netOnvifCore.Security;
using DateTime = System.DateTime;

namespace netOnvifCore;

public static class OnvifClientFactory
{
    public static async Task<DeviceClient> CreateDeviceClientAsync(string deviceIp, string username, string password)
    {
        var binding   = CreateBinding();
        var endpoint  = new EndpointAddress(new Uri($"http://{deviceIp}/onvif/device_service"));
        var device    = new DeviceClient(binding, endpoint);
        var timeShift = await GetDeviceTimeShift(device);

        device = new(binding, endpoint);
        device.ChannelFactory.Endpoint.EndpointBehaviors.Clear();
        device.ChannelFactory.Endpoint.EndpointBehaviors.Add(new SoapSecurityHeaderBehavior(username, password, timeShift));

        // Connectivity Test
        await device.OpenAsync();

        return device;
    }

    public static async Task<MediaClient> CreateMediaClientAsync(DeviceClient deviceClient)
    {
        var capabilities = await deviceClient.GetCapabilitiesAsync(new[] { CapabilityCategory.All });
        var media        = new MediaClient(CreateBinding(), new(new Uri(capabilities.Capabilities.Media.XAddr)));

        media.ChannelFactory.Endpoint.EndpointBehaviors.Clear();
        foreach (var endpointEndpointBehavior in deviceClient.ChannelFactory.Endpoint.EndpointBehaviors)
            media.ChannelFactory.Endpoint.EndpointBehaviors.Add(endpointEndpointBehavior);

        // Connectivity Test
        await media.OpenAsync();

        return media;
    }

    public static async Task<Media2Client> CreateMedia2ClientAsync(DeviceClient deviceClient)
    {
        var capabilities    = await deviceClient.GetCapabilitiesAsync(new[] { CapabilityCategory.All });
        var newDeviceClient = new Media2Client(CreateBinding(), new(new Uri(capabilities.Capabilities.Media.XAddr)));

        newDeviceClient.ChannelFactory.Endpoint.EndpointBehaviors.Clear();
        foreach (var endpointEndpointBehavior in deviceClient.ChannelFactory.Endpoint.EndpointBehaviors)
            newDeviceClient.ChannelFactory.Endpoint.EndpointBehaviors.Add(endpointEndpointBehavior);

        // Connectivity Test
        await newDeviceClient.OpenAsync();

        return newDeviceClient;
    }

    public static async Task<ImagingPortClient> CreateImagingClientAsync(DeviceClient deviceClient)
    {
        var capabilities    = await deviceClient.GetCapabilitiesAsync(new[] { CapabilityCategory.All });
        var newDeviceClient = new ImagingPortClient(CreateBinding(), new(new Uri(capabilities.Capabilities.Imaging.XAddr)));

        newDeviceClient.ChannelFactory.Endpoint.EndpointBehaviors.Clear();
        foreach (var endpointEndpointBehavior in deviceClient.ChannelFactory.Endpoint.EndpointBehaviors)
            newDeviceClient.ChannelFactory.Endpoint.EndpointBehaviors.Add(endpointEndpointBehavior);

        // Connectivity Test
        await newDeviceClient.OpenAsync();

        return newDeviceClient;
    }

    public static async Task<PTZClient> CreatePtzClientAsync(DeviceClient deviceClient)
    {
        var capabilities    = await deviceClient.GetCapabilitiesAsync(new[] { CapabilityCategory.All });
        var newDeviceClient = new PTZClient(CreateBinding(), new(new Uri(capabilities.Capabilities.PTZ.XAddr)));

        newDeviceClient.ChannelFactory.Endpoint.EndpointBehaviors.Clear();
        foreach (var endpointEndpointBehavior in deviceClient.ChannelFactory.Endpoint.EndpointBehaviors)
            newDeviceClient.ChannelFactory.Endpoint.EndpointBehaviors.Add(endpointEndpointBehavior);

        // Connectivity Test
        await newDeviceClient.OpenAsync();

        return newDeviceClient;
    }

    public static async Task<MediaClient> CreateMediaClientAsync(string deviceIp, string username, string password) =>
        await CreateMediaClientAsync(await CreateDeviceClientAsync(deviceIp, username, password));

    public static async Task<Media2Client> CreateMedia2ClientAsync(string deviceIp, string username, string password) =>
        await CreateMedia2ClientAsync(await CreateDeviceClientAsync(deviceIp, username, password));

    public static async Task<ImagingPortClient> CreateImagingClientAsync(string deviceIp, string username, string password) =>
        await CreateImagingClientAsync(await CreateDeviceClientAsync(deviceIp, username, password));

    public static async Task<PTZClient> CreatePtzClientAsync(string deviceIp, string username, string password) =>
        await CreatePtzClientAsync(await CreateDeviceClientAsync(deviceIp, username, password));

    private static CustomBinding CreateBinding()
    {
        var binding = new CustomBinding();

        binding.Elements.Add(new TextMessageEncodingBindingElement
        {
            MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None)
        });

        binding.Elements.Add(new HttpTransportBindingElement
        {
            AllowCookies           = true,
            MaxBufferSize          = int.MaxValue,
            MaxReceivedMessageSize = int.MaxValue
        });

        return binding;
    }

    private static async Task<TimeSpan> GetDeviceTimeShift(Device device)
    {
        var utc = (await device.GetSystemDateAndTimeAsync()).UTCDateTime;
        return new DateTime(utc.Date.Year, utc.Date.Month, utc.Date.Day, utc.Time.Hour, utc.Time.Minute, utc.Time.Second) - DateTime.UtcNow;
    }
}