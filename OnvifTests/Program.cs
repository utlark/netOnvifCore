using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using netOnvifCore;
using netOnvifCore.AccessControl;
using netOnvifCore.AccessRules;
using netOnvifCore.ActionEngine;
using netOnvifCore.Analytics;
using netOnvifCore.ApplicationManagement;
using netOnvifCore.AuthenticationBehavior;
using netOnvifCore.Credential;
using netOnvifCore.DeviceIO;
using netOnvifCore.Display;
using netOnvifCore.DoorControl;
using netOnvifCore.Event;
using netOnvifCore.Imaging;
using netOnvifCore.Media;
using netOnvifCore.Media2;
using netOnvifCore.Provisioning;
using netOnvifCore.Ptz;
using netOnvifCore.Receiver;
using netOnvifCore.Recording;
using netOnvifCore.Replay;
using netOnvifCore.Schedule;
using netOnvifCore.Search;
using netOnvifCore.Security;
using netOnvifCore.Thermal;
using netOnvifCore.Uplink;
using Newtonsoft.Json;
using CapabilityCategory = netOnvifCore.DeviceManagement.CapabilityCategory;
using DeviceClient = netOnvifCore.DeviceManagement.DeviceClient;
using Formatting = Newtonsoft.Json.Formatting;
using GetAuthFailureWarningConfigurationRequest = netOnvifCore.DeviceManagement.GetAuthFailureWarningConfigurationRequest;
using GetAuthFailureWarningOptionsRequest = netOnvifCore.DeviceManagement.GetAuthFailureWarningOptionsRequest;
using GetDeviceInformationRequest = netOnvifCore.DeviceManagement.GetDeviceInformationRequest;
using GetEndpointReferenceRequest = netOnvifCore.DeviceManagement.GetEndpointReferenceRequest;
using GetPasswordComplexityConfigurationRequest = netOnvifCore.DeviceManagement.GetPasswordComplexityConfigurationRequest;
using GetPasswordComplexityOptionsRequest = netOnvifCore.DeviceManagement.GetPasswordComplexityOptionsRequest;
using GetPasswordHistoryConfigurationRequest = netOnvifCore.DeviceManagement.GetPasswordHistoryConfigurationRequest;
using GetSystemUrisRequest = netOnvifCore.DeviceManagement.GetSystemUrisRequest;
using StreamSetup = netOnvifCore.Media.StreamSetup;
using StreamType = netOnvifCore.Media.StreamType;
using SystemLogType = netOnvifCore.DeviceManagement.SystemLogType;
using Transport = netOnvifCore.Media.Transport;
using TransportProtocol = netOnvifCore.Media.TransportProtocol;

namespace OnvifTests;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public static class Program
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    public enum CameraManufacturers
    {
        NovaCam,
        Ltv,
        Infinity,
        MicroDigital,
        UniView
    }

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    public enum InfinityRotateModes
    {
        Normal = 0,
        Flip = 180,
        Mirror = 89,
        Both = 269
    }

    private const string BasePath = "OnvifData/";
    private const string MethodsPath = $"{BasePath}/Methods";

    private static string _devicePath;
    private static string _mediaPath;
    private static string _media2Path;
    private static string _imagingPath;
    private static string _ptzPath;

    private static CameraManufacturers _cameraManufacturer;

    private static readonly List<(string Ip, (string Login, string Password) User)> AvailableCameras = new()
    {
        new ValueTuple<string, (string Login, string Password)>("20.0.1.10", new ValueTuple<string, string>("root", "root")),
        new ValueTuple<string, (string Login, string Password)>("20.0.1.11", new ValueTuple<string, string>("admin", "admin")),
        new ValueTuple<string, (string Login, string Password)>("20.0.1.13", new ValueTuple<string, string>("admin", "123456")),
        new ValueTuple<string, (string Login, string Password)>("10.15.51.120", new ValueTuple<string, string>("admin", "admin")),
        new ValueTuple<string, (string Login, string Password)>("10.15.51.126", new ValueTuple<string, string>("admin", "123456-Saut"))
    };

    public static async Task Main()
    {
        if (!Directory.Exists(MethodsPath))
        {
            await SaveAllOnvifFilteredMethods(m => m.Name.StartsWith("Get"), "getMethods");
            await SaveAllOnvifFilteredMethods(m => m.Name.StartsWith("Set"), "setMethods");
            await SaveAllOnvifFilteredMethods(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"), "otherMethods");
        }

        //foreach (var camera in AvailableCameras)
        {
            var camera = AvailableCameras[0];
            Console.WriteLine($"Address: {camera.Ip}");

            var device = OnvifClientFactory.CreateDeviceClientAsync(camera.Ip, camera.User.Login, camera.User.Password)
                .Result;

            _cameraManufacturer =
                device.GetDeviceInformationAsync(new GetDeviceInformationRequest()).Result.Manufacturer switch
                {
                    "BASIC_45S" => CameraManufacturers.NovaCam,
                    "LTV" => CameraManufacturers.Ltv,
                    "Infinity" => CameraManufacturers.Infinity,
                    "Microdigital Inc.," => CameraManufacturers.MicroDigital,
                    "UNIVIEW" => CameraManufacturers.UniView,
                    _ => _cameraManufacturer
                };

            if (Directory.Exists($"{BasePath}/{_cameraManufacturer}"))
                Directory.Delete($"{BasePath}/{_cameraManufacturer}", true);

            _devicePath = $"{BasePath}/CamerasSettings/{_cameraManufacturer}/Device/Get";
            _mediaPath = $"{BasePath}/CamerasSettings/{_cameraManufacturer}/Media/Get";
            _media2Path = $"{BasePath}/CamerasSettings/{_cameraManufacturer}/Media2/Get";
            _imagingPath = $"{BasePath}/CamerasSettings/{_cameraManufacturer}/Imaging/Get";
            _ptzPath = $"{BasePath}/CamerasSettings/{_cameraManufacturer}/Ptz/Get";

            var media = OnvifClientFactory.CreateMediaClientAsync(device).Result;
            var imaging = OnvifClientFactory.CreateImagingClientAsync(device).Result;

            //await SaveAllDeviceClientGetMethods(device);
            await SaveAllMediaClientGetMethods(media);
            //await SaveAllImagingClientGetMethods(media, imaging);

            //await ExecuteAndIgnoreExceptions(async () => await SaveAllMedia2ClientGetMethods(media, OnvifClientFactory.CreateMedia2ClientAsync(device).Result));
            //await ExecuteAndIgnoreExceptions(async () => await SaveAllPtzClientGetMethods(media, OnvifClientFactory.CreatePtzClientAsync(device).Result));
        }
    }

    private static async Task ExecuteAndIgnoreExceptions(Func<Task> taskAction)
    {
        try
        {
            await taskAction();
        }
        catch
        {
            // Проигнорировать исключение и продолжить выполнение кода
        }
    }

    private static async Task Serialize(object obj, string directory, string fileName)
    {
        Directory.CreateDirectory($"./{directory}");
        await File.WriteAllTextAsync($"./{directory}/{fileName}.json", JsonConvert.SerializeObject(obj, Formatting.Indented));
    }

    private static async Task SetSourceRotate(MediaClient media, InfinityRotateModes modes)
    {
        var configuration = media.GetVideoSourceConfigurationsAsync().Result.Configurations[0];

        var doc = new XmlDocument();

        var modeElement = doc.CreateElement("tt:Mode", "http://www.onvif.org/ver10/schema");
        modeElement.InnerText = "ON";

        var degreeElement = doc.CreateElement("tt:Degree", "http://www.onvif.org/ver10/schema");
        degreeElement.InnerText = ((int)modes).ToString();

        var rotateElement = doc.CreateElement("tt:Rotate", "http://www.onvif.org/ver10/schema");
        rotateElement.AppendChild(modeElement);
        rotateElement.AppendChild(degreeElement);

        var extensionElement = doc.CreateElement("tt:Extension", "http://www.onvif.org/ver10/schema");
        extensionElement.AppendChild(rotateElement);

        configuration.Any = new[] { extensionElement };

        await media.SetVideoSourceConfigurationAsync(configuration, true);
    }

    private static async Task SaveAllOnvifFilteredMethods(Func<MethodInfo, bool> filterCondition, string fileName)
    {
        await Serialize(typeof(PACSPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(PACSPortClient)}", fileName);

        await Serialize(typeof(AccessRulesPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(AccessRulesPortClient)}", fileName);

        await Serialize(typeof(ActionEnginePortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(ActionEnginePortClient)}", fileName);

        await Serialize(typeof(AnalyticsEnginePortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(AnalyticsEnginePortClient)}", fileName);

        await Serialize(typeof(RuleEnginePortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(RuleEnginePortClient)}", fileName);

        await Serialize(typeof(AppManagementClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(AppManagementClient)}", fileName);

        await Serialize(typeof(AuthenticationBehaviorPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(AuthenticationBehaviorPortClient)}", fileName);

        await Serialize(typeof(CredentialPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(CredentialPortClient)}", fileName);

        await Serialize(typeof(netOnvifCore.DeviceIO.DeviceClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(netOnvifCore.DeviceIO.DeviceClient)}IO", fileName);

        await Serialize(typeof(DeviceIOPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(DeviceIOPortClient)}", fileName);

        await Serialize(typeof(DeviceClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(DeviceClient)}", fileName);

        await Serialize(typeof(DisplayPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(DisplayPortClient)}", fileName);

        await Serialize(typeof(DoorControlPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(DoorControlPortClient)}", fileName);

        await Serialize(typeof(CreatePullPointClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(CreatePullPointClient)}", fileName);

        await Serialize(typeof(EventPortTypeClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(EventPortTypeClient)}", fileName);

        await Serialize(typeof(NotificationConsumerClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(NotificationConsumerClient)}", fileName);

        await Serialize(typeof(NotificationProducerClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(NotificationProducerClient)}", fileName);

        await Serialize(typeof(PausableSubscriptionManagerClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(PausableSubscriptionManagerClient)}", fileName);

        await Serialize(typeof(PullPointClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(PullPointClient)}", fileName);

        await Serialize(typeof(PullPointSubscriptionClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(PullPointSubscriptionClient)}", fileName);

        await Serialize(typeof(SubscriptionManagerClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(SubscriptionManagerClient)}", fileName);

        await Serialize(typeof(ImagingPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(ImagingPortClient)}", fileName);

        await Serialize(typeof(MediaClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(MediaClient)}", fileName);

        await Serialize(typeof(Media2Client).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(Media2Client)}", fileName);

        await Serialize(typeof(ProvisioningServiceClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(ProvisioningServiceClient)}", fileName);

        await Serialize(typeof(PTZClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(PTZClient)}", fileName);

        await Serialize(typeof(ReceiverPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(ReceiverPortClient)}", fileName);

        await Serialize(typeof(RecordingPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(RecordingPortClient)}", fileName);

        await Serialize(typeof(ReplayPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(ReplayPortClient)}", fileName);

        await Serialize(typeof(SchedulePortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(SchedulePortClient)}", fileName);

        await Serialize(typeof(SearchPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(SearchPortClient)}", fileName);

        await Serialize(typeof(AdvancedSecurityServiceClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(AdvancedSecurityServiceClient)}", fileName);

        await Serialize(typeof(Dot1XClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(Dot1XClient)}", fileName);

        await Serialize(typeof(KeystoreClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(KeystoreClient)}", fileName);

        await Serialize(typeof(TLSServerClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(TLSServerClient)}", fileName);

        await Serialize(typeof(ThermalPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(ThermalPortClient)}", fileName);

        await Serialize(typeof(UplinkPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(filterCondition)
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(UplinkPortClient)}", fileName);
    }

    private static async Task SaveAllDeviceClientGetMethods(DeviceClient device)
    {
        await ExecuteAndIgnoreExceptions(async () =>
        {
            var accessPolicy = device.GetAccessPolicyAsync().Result;
            await Serialize(accessPolicy, $"{_devicePath}", "accessPolicy");
        });

        await ExecuteAndIgnoreExceptions(async () =>
        {
            var authFailureWarningConfiguration = device.GetAuthFailureWarningConfigurationAsync(new GetAuthFailureWarningConfigurationRequest()).Result;
            await Serialize(authFailureWarningConfiguration, $"{_devicePath}", "authFailureWarningConfiguration");
        });

        await ExecuteAndIgnoreExceptions(async () =>
        {
            var authFailureWarningOptions = device.GetAuthFailureWarningOptionsAsync(new GetAuthFailureWarningOptionsRequest()).Result;
            await Serialize(authFailureWarningOptions, $"{_devicePath}", "authFailureWarningOptions");
        });

        await ExecuteAndIgnoreExceptions(async () =>
        {
            var caCertificates = device.GetCACertificatesAsync().Result;
            await Serialize(caCertificates, $"{_devicePath}", "caCertificates");
            foreach (var conf in caCertificates.CACertificate)
            {
                await Serialize(conf, $"{_devicePath}/caCertificates", $"{conf.CertificateID}");

                var pkcs10Request = device.GetPkcs10RequestAsync(conf.CertificateID, null, null).Result;
                await Serialize(pkcs10Request, $"{_devicePath}/caCertificates/{conf.CertificateID}", "pkcs10Request");
            }
        });

        var capabilities = device.GetCapabilitiesAsync(new[] { CapabilityCategory.All }).Result;
        await Serialize(capabilities, $"{_devicePath}", "capabilities");

        await ExecuteAndIgnoreExceptions(async () =>
        {
            var certificates = device.GetCertificatesAsync().Result;
            await Serialize(certificates, $"{_devicePath}", "certificates");
            foreach (var conf in certificates.NvtCertificate)
            {
                await Serialize(conf, $"{_devicePath}/certificates", $"{conf.CertificateID}");

                var certificateInformation = device.GetCertificateInformationAsync(conf.CertificateID).Result;
                await Serialize(certificateInformation, $"{_devicePath}/certificates/{conf.CertificateID}", "certificateInformation");
            }
        });

        await ExecuteAndIgnoreExceptions(async () =>
        {
            var certificatesStatus = device.GetCertificatesStatusAsync().Result;
            await Serialize(certificatesStatus, $"{_devicePath}", "certificatesStatus");
            foreach (var conf in certificatesStatus.CertificateStatus)
                await Serialize(conf, $"{_devicePath}/certificatesStatus", $"{conf.CertificateID}");
        });

        await ExecuteAndIgnoreExceptions(async () =>
        {
            var clientCertificateMode = device.GetClientCertificateModeAsync().Result;
            await Serialize(clientCertificateMode, $"{_devicePath}", "clientCertificateMode");
        });

        var deviceInformation = device.GetDeviceInformationAsync(new GetDeviceInformationRequest()).Result;
        await Serialize(deviceInformation, $"{_devicePath}", "deviceInformation");

        var discoveryMode = device.GetDiscoveryModeAsync().Result;
        await Serialize(discoveryMode, $"{_devicePath}", "discoveryMode");

        var dns = device.GetDNSAsync().Result;
        await Serialize(dns, $"{_devicePath}", "dns");

        await ExecuteAndIgnoreExceptions(async () =>
        {
            var dot11Capabilities = device.GetDot11CapabilitiesAsync(new XmlElement[] { }).Result;
            await Serialize(dot11Capabilities, $"{_devicePath}", "dot11Capabilities");
        });

        var zeroConfiguration = device.GetZeroConfigurationAsync().Result;
        await Serialize(zeroConfiguration, $"{_devicePath}", "zeroConfiguration");

        await ExecuteAndIgnoreExceptions(async () =>
        {
            var dot11Status = device.GetDot11StatusAsync(zeroConfiguration.InterfaceToken).Result;
            await Serialize(dot11Status, $"{_devicePath}", "dot11Status");
        });

        await ExecuteAndIgnoreExceptions(async () =>
        {
            var dot1XConfigurations = device.GetDot1XConfigurationsAsync().Result;
            await Serialize(dot1XConfigurations, $"{_devicePath}", "dot1XConfigurations");
            foreach (var conf in dot1XConfigurations.Dot1XConfiguration)
            {
                await Serialize(conf, $"{_devicePath}/dot1XConfigurations", $"{conf.Identity}");

                var dot1XConfiguration = device.GetDot1XConfigurationAsync(conf.Dot1XConfigurationToken).Result;
                await Serialize(dot1XConfiguration, $"{_devicePath}/dot1XConfigurations/{conf.Identity}", "dot1XConfiguration");
            }
        });

        await ExecuteAndIgnoreExceptions(async () =>
        {
            var dpAddresses = device.GetDPAddressesAsync().Result;
            await Serialize(dpAddresses, $"{_devicePath}", "dpAddresses");
            foreach (var conf in dpAddresses.DPAddress)
                await Serialize(conf, $"{_devicePath}/dpAddresses", $"{conf.DNSname}");
        });

        await ExecuteAndIgnoreExceptions(async () =>
        {
            var dynamicDns = device.GetDynamicDNSAsync().Result;
            await Serialize(dynamicDns, $"{_devicePath}", "dynamicDns");
        });

        await ExecuteAndIgnoreExceptions(async () =>
        {
            var endpointReference = device.GetEndpointReferenceAsync(new GetEndpointReferenceRequest()).Result;
            await Serialize(endpointReference, $"{_devicePath}", "endpointReference");
        });

        await ExecuteAndIgnoreExceptions(async () =>
        {
            var geoLocation = device.GetGeoLocationAsync().Result;
            await Serialize(geoLocation, $"{_devicePath}", "geoLocation");
        });

        var hostname = device.GetHostnameAsync().Result;
        await Serialize(hostname, $"{_devicePath}", "hostname");

        await ExecuteAndIgnoreExceptions(async () =>
        {
            var ipAddressFilter = device.GetIPAddressFilterAsync().Result;
            await Serialize(ipAddressFilter, $"{_devicePath}", "ipAddressFilter");
        });

        var networkDefaultGateway = device.GetNetworkDefaultGatewayAsync().Result;
        await Serialize(networkDefaultGateway, $"{_devicePath}", "networkDefaultGateway");

        var networkInterfaces = device.GetNetworkInterfacesAsync().Result;
        await Serialize(networkInterfaces, $"{_devicePath}", "networkInterfaces");
        foreach (var conf in networkInterfaces.NetworkInterfaces)
            await Serialize(conf, $"{_devicePath}/networkInterfaces", $"{conf.token}");

        var networkProtocols = device.GetNetworkProtocolsAsync().Result;
        await Serialize(networkProtocols, $"{_devicePath}", "networkProtocols");
        foreach (var conf in networkProtocols.NetworkProtocols)
            await Serialize(conf, $"{_devicePath}/networkProtocols", $"{conf.Name}");

        var ntp = device.GetNTPAsync().Result;
        await Serialize(ntp, $"{_devicePath}", "ntp");

        await ExecuteAndIgnoreExceptions(async () =>
        {
            var passwordComplexityConfiguration = device.GetPasswordComplexityConfigurationAsync(new GetPasswordComplexityConfigurationRequest()).Result;
            await Serialize(passwordComplexityConfiguration, $"{_devicePath}", "passwordComplexityConfiguration");
        });

        await ExecuteAndIgnoreExceptions(async () =>
        {
            var passwordComplexityOptions = device.GetPasswordComplexityOptionsAsync(new GetPasswordComplexityOptionsRequest()).Result;
            await Serialize(passwordComplexityOptions, $"{_devicePath}", "passwordComplexityOptions");
        });

        await ExecuteAndIgnoreExceptions(async () =>
        {
            var passwordHistoryConfiguration = device.GetPasswordHistoryConfigurationAsync(new GetPasswordHistoryConfigurationRequest()).Result;
            await Serialize(passwordHistoryConfiguration, $"{_devicePath}", "passwordHistoryConfiguration");
        });

        var relayOutputs = device.GetRelayOutputsAsync().Result;
        await Serialize(relayOutputs, $"{_devicePath}", "relayOutputs");
        foreach (var conf in relayOutputs.RelayOutputs)
            await Serialize(conf, $"{_devicePath}/relayOutputs", $"{conf.token}");

        await ExecuteAndIgnoreExceptions(async () =>
        {
            var remoteDiscoveryMode = device.GetRemoteDiscoveryModeAsync().Result;
            await Serialize(remoteDiscoveryMode, $"{_devicePath}", "remoteDiscoveryMode");
        });

        await ExecuteAndIgnoreExceptions(async () =>
        {
            var remoteUser = device.GetRemoteUserAsync().Result;
            await Serialize(remoteUser, $"{_devicePath}", "remoteUser");
        });

        var scopes = device.GetScopesAsync().Result;
        await Serialize(scopes, $"{_devicePath}", "scopes");
        foreach (var conf in scopes.Scopes)
            await Serialize(conf, $"{_devicePath}/scopes", $"{conf.ScopeItem.Split('/').Last()}");

        var serviceCapabilities = device.GetServiceCapabilitiesAsync().Result;
        await Serialize(serviceCapabilities, $"{_devicePath}", "serviceCapabilities");

        var services = device.GetServicesAsync(true).Result;
        await Serialize(services, $"{_devicePath}", "services");
        foreach (var conf in services.Service)
            await Serialize(conf, $"{_devicePath}/services", $"{conf.Namespace.Split('/')[^1]}");

        await ExecuteAndIgnoreExceptions(async () =>
        {
            var storageConfigurations = device.GetStorageConfigurationsAsync().Result;
            await Serialize(storageConfigurations, $"{_devicePath}", "storageConfigurations");
            foreach (var conf in storageConfigurations.StorageConfigurations)
            {
                await Serialize(conf, $"{_devicePath}/storageConfigurations", $"{conf.token}");

                var storageConfiguration = device.GetStorageConfigurationAsync(conf.token).Result;
                await Serialize(storageConfiguration, $"{_devicePath}/storageConfigurations/{conf.token}", "storageConfiguration");
            }
        });

        await ExecuteAndIgnoreExceptions(async () =>
        {
            var systemBackup = device.GetSystemBackupAsync().Result;
            await Serialize(systemBackup, $"{_devicePath}", "systemBackup");
            foreach (var conf in systemBackup.BackupFiles)
                await Serialize(systemBackup, $"{_devicePath}/systemBackup", $"{conf.Name}");
        });

        var systemDateAndTime = device.GetSystemDateAndTimeAsync().Result;
        await Serialize(systemDateAndTime, $"{_devicePath}", "systemDateAndTime");

        await ExecuteAndIgnoreExceptions(async () =>
        {
            var systemLogAccess = device.GetSystemLogAsync(SystemLogType.Access).Result;
            await Serialize(systemLogAccess, $"{_devicePath}", "systemLogAccess");
        });

        await ExecuteAndIgnoreExceptions(async () =>
        {
            var systemLogSystem = device.GetSystemLogAsync(SystemLogType.System).Result;
            await Serialize(systemLogSystem, $"{_devicePath}", "systemLogSystem");
        });

        await ExecuteAndIgnoreExceptions(async () =>
        {
            var systemSupportInformation = device.GetSystemSupportInformationAsync().Result;
            await Serialize(systemSupportInformation, $"{_devicePath}", "systemSupportInformation");
        });

        await ExecuteAndIgnoreExceptions(async () =>
        {
            var systemUris = device.GetSystemUrisAsync(new GetSystemUrisRequest()).Result;
            await Serialize(systemUris, $"{_devicePath}", "systemUris");
            foreach (var conf in systemUris.SystemLogUris)
                await Serialize(conf, $"{_devicePath}/systemUris", $"{conf.Uri}");
        });

        var users = device.GetUsersAsync().Result;
        await Serialize(users, $"{_devicePath}", "users");
        foreach (var conf in users.User)
            await Serialize(conf, $"{_devicePath}/users", $"{conf.Username}");

        var wsdlUrl = device.GetWsdlUrlAsync().Result;
        await Serialize(wsdlUrl, $"{_devicePath}", "wsdlUrl");
    }

    private static async Task SaveAllMediaClientGetMethods(MediaClient media)
    {
        var profiles = media.GetProfilesAsync().Result;
        await Serialize(profiles, $"{_mediaPath}", "profiles");
        foreach (var conf in profiles.Profiles)
            await Serialize(conf, $"{_mediaPath}/profiles", $"{conf.token}");

        var audioDecoderConfigurations = media.GetAudioDecoderConfigurationsAsync().Result;
        await Serialize(audioDecoderConfigurations, $"{_mediaPath}", "audioDecoderConfigurations");
        foreach (var conf in audioDecoderConfigurations.Configurations)
        {
            await Serialize(conf, $"{_mediaPath}/audioDecoderConfigurations", $"{conf.Name}");

            var audioDecoderConfiguration = media.GetAudioDecoderConfigurationAsync(conf.token).Result;
            await Serialize(audioDecoderConfiguration, $"{_mediaPath}/audioDecoderConfigurations/{conf.Name}", "audioDecoderConfiguration");

            foreach (var prof in profiles.Profiles)
            {
                var audioDecoderConfigurationOptions = media.GetAudioDecoderConfigurationOptionsAsync(conf.token, prof.token).Result;
                await Serialize(audioDecoderConfigurationOptions, $"{_mediaPath}/audioDecoderConfigurations/{conf.Name}/audioDecoderConfigurationOptions/{prof.Name}", "audioDecoderConfigurationOptions");
            }
        }

        var audioEncoderConfigurations = media.GetAudioEncoderConfigurationsAsync().Result;
        await Serialize(audioEncoderConfigurations, $"{_mediaPath}", "audioEncoderConfigurations");
        foreach (var conf in audioEncoderConfigurations.Configurations)
        {
            await Serialize(conf, $"{_mediaPath}/audioEncoderConfigurations", $"{conf.Name}");

            var audioEncoderConfiguration = media.GetAudioEncoderConfigurationAsync(conf.token).Result;
            await Serialize(audioEncoderConfiguration, $"{_mediaPath}/audioEncoderConfigurations/{conf.Name}", "audioEncoderConfiguration");

            foreach (var prof in profiles.Profiles)
                await ExecuteAndIgnoreExceptions(async () =>
                {
                    var audioEncoderConfigurationOptions = media.GetAudioEncoderConfigurationOptionsAsync(conf.token, prof.token).Result;
                    await Serialize(audioEncoderConfigurationOptions, $"{_mediaPath}/audioEncoderConfigurations/{conf.Name}/audioEncoderConfigurationOptions/{prof.Name}", "audioEncoderConfigurationOptions");
                    foreach (var opt in audioEncoderConfigurationOptions.Options)
                        await Serialize(opt, $"{_mediaPath}/audioEncoderConfigurations/{conf.Name}/audioEncoderConfigurationOptions/{prof.Name}/audioEncoderConfigurationOptions", $"{opt.Encoding}");
                });
        }

        var audioOutputs = media.GetAudioOutputsAsync().Result;
        await Serialize(audioOutputs, $"{_mediaPath}", "audioOutputs");
        foreach (var conf in audioOutputs.AudioOutputs)
            await Serialize(conf, $"{_mediaPath}/audioOutputs", $"{conf.token}");

        var audioOutputConfigurations = media.GetAudioOutputConfigurationsAsync().Result;
        await Serialize(audioOutputConfigurations, $"{_mediaPath}", "audioOutputConfigurations");
        foreach (var conf in audioOutputConfigurations.Configurations)
        {
            await Serialize(conf, $"{_mediaPath}/audioOutputConfigurations", $"{conf.Name}");

            var audioOutputConfiguration = media.GetAudioOutputConfigurationAsync(conf.token).Result;
            await Serialize(audioOutputConfiguration, $"{_mediaPath}/audioOutputConfiguration/{conf.Name}", "audioOutputConfiguration");

            foreach (var prof in profiles.Profiles)
            {
                var audioOutputConfigurationOptions = media.GetAudioOutputConfigurationOptionsAsync(conf.token, prof.token).Result;
                await Serialize(audioOutputConfigurationOptions, $"{_mediaPath}/audioOutputConfiguration/{conf.Name}/audioOutputConfigurationOptions/{prof.Name}", "audioOutputConfigurationOptions");
            }
        }

        var audioSources = media.GetAudioSourcesAsync().Result;
        await Serialize(audioSources, $"{_mediaPath}", "audioSources");
        foreach (var conf in audioSources.AudioSources)
            await Serialize(conf, $"{_mediaPath}/audioSources", $"{conf.token}");

        var audioSourceConfigurations = media.GetAudioSourceConfigurationsAsync().Result;
        await Serialize(audioSourceConfigurations, $"{_mediaPath}", "audioSourceConfigurations");
        foreach (var conf in audioSourceConfigurations.Configurations)
        {
            await Serialize(conf, $"{_mediaPath}/audioSourceConfigurations", $"{conf.Name}");

            var audioSourceConfiguration = media.GetAudioSourceConfigurationAsync(conf.token).Result;
            await Serialize(audioSourceConfiguration, $"{_mediaPath}/audioSourceConfigurations/{conf.Name}", "audioSourceConfiguration");

            foreach (var prof in profiles.Profiles)
            {
                var audioSourceConfigurationOptions = media.GetAudioSourceConfigurationOptionsAsync(conf.token, prof.token).Result;
                await Serialize(audioSourceConfigurationOptions, $"{_mediaPath}/audioSourceConfigurations/{conf.Name}/audioSourceConfigurationOptions/{prof.Name}", "audioSourceConfigurationOptions");
            }
        }

        var streamSetup = new StreamSetup { Stream = StreamType.RTPUnicast, Transport = new Transport { Protocol = TransportProtocol.UDP, Tunnel = null } };
        foreach (var prof in profiles.Profiles)
        {
            if (_cameraManufacturer != CameraManufacturers.NovaCam)
            {
                var compatibleAudioDecoderConfigurations = media.GetCompatibleAudioDecoderConfigurationsAsync(prof.token).Result;
                await Serialize(compatibleAudioDecoderConfigurations, $"{_mediaPath}/profiles/{prof.Name}", "compatibleAudioDecoderConfigurations");
                foreach (var conf in compatibleAudioDecoderConfigurations.Configurations)
                    await Serialize(conf, $"{_mediaPath}/profiles/{prof.Name}/compatibleAudioDecoderConfigurations", $"{conf.Name}");
            }

            await ExecuteAndIgnoreExceptions(async () =>
            {
                var compatibleAudioEncoderConfigurations = media.GetCompatibleAudioEncoderConfigurationsAsync(prof.token).Result;
                await Serialize(compatibleAudioEncoderConfigurations, $"{_mediaPath}/profiles/{prof.Name}", "compatibleAudioEncoderConfigurations");
                foreach (var conf in compatibleAudioEncoderConfigurations.Configurations)
                    await Serialize(conf, $"{_mediaPath}/profiles/{prof.Name}/compatibleAudioEncoderConfigurations", $"{conf.Name}");
            });

            var compatibleAudioOutputConfigurations = media.GetCompatibleAudioOutputConfigurationsAsync(prof.token).Result;
            await Serialize(compatibleAudioOutputConfigurations, $"{_mediaPath}/profiles/{prof.Name}", "compatibleAudioOutputConfigurations");
            foreach (var conf in compatibleAudioOutputConfigurations.Configurations)
                await Serialize(conf, $"{_mediaPath}/profiles/{prof.Name}/compatibleAudioOutputConfigurations", $"{conf.Name}");

            var compatibleAudioSourceConfigurations = media.GetCompatibleAudioSourceConfigurationsAsync(prof.token).Result;
            await Serialize(compatibleAudioSourceConfigurations, $"{_mediaPath}/profiles/{prof.Name}", "compatibleAudioSourceConfigurations");
            foreach (var conf in compatibleAudioSourceConfigurations.Configurations)
                await Serialize(conf, $"{_mediaPath}/profiles/{prof.Name}/compatibleAudioSourceConfigurations", $"{conf.Name}");

            var compatibleMetadataConfigurations = media.GetCompatibleMetadataConfigurationsAsync(prof.token).Result;
            await Serialize(compatibleMetadataConfigurations, $"{_mediaPath}/profiles/{prof.Name}", "compatibleMetadataConfigurations");
            foreach (var conf in compatibleMetadataConfigurations.Configurations)
                await Serialize(conf, $"{_mediaPath}/profiles/{prof.Name}/compatibleMetadataConfigurations", $"{conf.Name}");

            if (_cameraManufacturer != CameraManufacturers.NovaCam)
                await ExecuteAndIgnoreExceptions(async () =>
                {
                    var compatibleVideoAnalyticsConfigurations = media.GetCompatibleVideoAnalyticsConfigurationsAsync(prof.token).Result;
                    await Serialize(compatibleVideoAnalyticsConfigurations, $"{_mediaPath}/profiles/{prof.Name}", "compatibleVideoAnalyticsConfigurations");
                    foreach (var conf in compatibleVideoAnalyticsConfigurations.Configurations)
                        await Serialize(conf, $"{_mediaPath}/profiles/{prof.Name}/compatibleVideoAnalyticsConfigurations", $"{conf.Name}");
                });

            var compatibleVideoEncoderConfigurations = media.GetCompatibleVideoEncoderConfigurationsAsync(prof.token).Result;
            await Serialize(compatibleVideoEncoderConfigurations, $"{_mediaPath}/profiles/{prof.Name}", "compatibleVideoEncoderConfigurations");
            foreach (var conf in compatibleVideoEncoderConfigurations.Configurations)
                await Serialize(conf, $"{_mediaPath}/profiles/{prof.Name}/compatibleVideoEncoderConfigurations", $"{conf.Name}");

            var compatibleVideoSourceConfigurations = media.GetCompatibleVideoSourceConfigurationsAsync(prof.token).Result;
            await Serialize(compatibleVideoSourceConfigurations, $"{_mediaPath}/profiles/{prof.Name}", "compatibleVideoSourceConfigurations");
            foreach (var conf in compatibleVideoSourceConfigurations.Configurations)
                await Serialize(conf, $"{_mediaPath}/profiles/{prof.Name}/compatibleVideoSourceConfigurations", $"{conf.Name}");

            var profile = media.GetProfileAsync(prof.token).Result;
            await Serialize(profile, $"{_mediaPath}/profiles/{prof.Name}", "profile");

            var snapshotUri = media.GetSnapshotUriAsync(prof.token).Result;
            await Serialize(snapshotUri, $"{_mediaPath}/profiles/{prof.Name}", "snapshotUri");

            var streamUri = media.GetStreamUriAsync(streamSetup, prof.token).Result;
            await Serialize(streamUri, $"{_mediaPath}/profiles/{prof.Name}", "streamUri");
        }

        await ExecuteAndIgnoreExceptions(async () =>
        {
            var metadataConfigurations = media.GetMetadataConfigurationsAsync().Result;
            await Serialize(metadataConfigurations, $"{_mediaPath}", "metadataConfigurations");
            foreach (var conf in metadataConfigurations.Configurations)
            {
                await Serialize(conf, $"{_mediaPath}/metadataConfigurations", $"{conf.Name}");

                var metadataConfiguration = media.GetMetadataConfigurationAsync(conf.token).Result;
                await Serialize(metadataConfiguration, $"{_mediaPath}/metadataConfigurations/{conf.Name}", "metadataConfiguration");

                foreach (var prof in profiles.Profiles)
                {
                    var metadataConfigurationOptions = media.GetMetadataConfigurationOptionsAsync(conf.token, prof.token).Result;
                    await Serialize(metadataConfigurationOptions, $"{_mediaPath}/metadataConfigurations/{conf.Name}/metadataConfigurationOptions/{prof.Name}", "metadataConfigurationOptions");
                }
            }
        });

        var serviceCapabilities = media.GetServiceCapabilitiesAsync().Result;
        await Serialize(serviceCapabilities, $"{_mediaPath}", "serviceCapabilities");

        await ExecuteAndIgnoreExceptions(async () =>
        {
            var videoAnalyticsConfigurations = media.GetVideoAnalyticsConfigurationsAsync().Result;
            await Serialize(videoAnalyticsConfigurations, $"{_mediaPath}", "videoAnalyticsConfigurations");
            foreach (var conf in videoAnalyticsConfigurations.Configurations)
            {
                await Serialize(conf, $"{_mediaPath}/videoAnalyticsConfigurations", $"{conf.Name}");

                var videoAnalyticsConfiguration = media.GetVideoAnalyticsConfigurationAsync(conf.token).Result;
                await Serialize(videoAnalyticsConfiguration, $"{_mediaPath}/videoAnalyticsConfigurations/{conf.Name}", "videoAnalyticsConfiguration");
            }
        });

        var videoEncoderConfigurations = media.GetVideoEncoderConfigurationsAsync().Result;
        await Serialize(videoEncoderConfigurations, $"{_mediaPath}", "videoEncoderConfigurations");
        foreach (var conf in videoEncoderConfigurations.Configurations)
        {
            await Serialize(conf, $"{_mediaPath}/videoEncoderConfigurations", $"{conf.Name}");

            var videoEncoderConfiguration = media.GetVideoEncoderConfigurationAsync(conf.token).Result;
            await Serialize(videoEncoderConfiguration, $"{_mediaPath}/videoEncoderConfigurations/{conf.Name}", "videoEncoderConfiguration");

            foreach (var prof in profiles.Profiles)
            {
                var videoEncoderConfigurationOptions = media.GetVideoEncoderConfigurationOptionsAsync(conf.token, prof.token).Result;
                await Serialize(videoEncoderConfigurationOptions, $"{_mediaPath}/videoEncoderConfigurations/{conf.Name}/videoEncoderConfigurationOptions/{prof.Name}", "videoEncoderConfigurationOptions");
            }
        }

        var videoSourceConfigurations = media.GetVideoSourceConfigurationsAsync().Result;
        await Serialize(videoSourceConfigurations, $"{_mediaPath}", "videoSourceConfigurations");
        foreach (var conf in videoSourceConfigurations.Configurations)
        {
            await Serialize(conf, $"{_mediaPath}/videoSourceConfigurations", $"{conf.Name}");

            var videoSourceConfiguration = media.GetVideoSourceConfigurationAsync(conf.token).Result;
            await Serialize(videoSourceConfiguration, $"{_mediaPath}/videoSourceConfigurations/{conf.Name}", "videoSourceConfiguration");

            foreach (var prof in profiles.Profiles)
            {
                var videoSourceConfigurationOptions = media.GetVideoSourceConfigurationOptionsAsync(conf.token, prof.token).Result;
                await Serialize(videoSourceConfigurationOptions, $"{_mediaPath}/videoSourceConfigurations/{conf.Name}/videoSourceConfigurationOptions/{prof.Name}", "videoSourceConfigurationOptions");
            }

            await ExecuteAndIgnoreExceptions(async () =>
            {
                var oSDs = media.GetOSDsAsync(conf.token).Result;
                await Serialize(oSDs, $"{_mediaPath}/videoSourceConfigurations/{conf.Name}", "oSDs");
                foreach (var osdConf in oSDs.OSDs)
                {
                    var osd = media.GetOSDAsync(new GetOSDRequest(osdConf.token, null)).Result;
                    await Serialize(osd, $"{_mediaPath}/videoSourceConfigurations/{conf.Name}/osd/{osdConf.token}", "osd");

                    var osdOptions = media.GetOSDOptionsAsync(new GetOSDOptionsRequest(osdConf.token, null)).Result;
                    await Serialize(osdOptions, $"{_mediaPath}/videoSourceConfigurations/{conf.Name}/osdOptions/{osdConf.token}", "osdOptions");
                }
            });

            var guaranteedNumberOfVideoEncoderInstances = media.GetGuaranteedNumberOfVideoEncoderInstancesAsync(new GetGuaranteedNumberOfVideoEncoderInstancesRequest(conf.token)).Result;
            await Serialize(guaranteedNumberOfVideoEncoderInstances, $"{_mediaPath}/videoSourceConfigurations/{conf.Name}", "guaranteedNumberOfVideoEncoderInstances");
        }

        var videoSources = media.GetVideoSourcesAsync().Result;
        await Serialize(videoSources, $"{_mediaPath}", "videoSources");
        foreach (var conf in videoSources.VideoSources)
        {
            await Serialize(conf, $"{_mediaPath}/videoSources", $"{conf.token}");

            await ExecuteAndIgnoreExceptions(async () =>
            {
                var videoSourceModes = media.GetVideoSourceModesAsync(conf.token).Result;
                await Serialize(videoSourceModes, $"{_mediaPath}/videoSources/{conf.token}", "videoSources");
                foreach (var mode in videoSourceModes.VideoSourceModes)
                    await Serialize(mode, $"{_mediaPath}/videoSources/{conf.token}/videoSourceModes/{mode.token}", "mode");
            });
        }
    }

    private static async Task SaveAllMedia2ClientGetMethods(MediaClient media, Media2Client media2)
    {
        await Task.Delay(0);
        await ExecuteAndIgnoreExceptions(async () =>
        {
            var profiles = media2.GetProfilesAsync(null, null).Result;
            await Serialize(profiles, $"{_media2Path}", "profiles");
            foreach (var conf in profiles.Profiles)
            {
                await Serialize(profiles, $"{_media2Path}/profiles", $"{conf.Name}");

                var videoSourceConfigurations = media2.GetVideoSourceConfigurationsAsync(null, conf.token).Result;
                await Serialize(videoSourceConfigurations, $"{_media2Path}/profiles/{conf.Name}", "videoSourceConfigurations");
                foreach (var prof in videoSourceConfigurations.Configurations)
                {
                    await Serialize(prof, $"{_media2Path}/profiles/{conf.Name}/videoSourceConfigurations", $"{prof.Name}");

                    await ExecuteAndIgnoreExceptions(async () =>
                    {
                        var videoSourceConfigurationOptions = media2.GetVideoSourceConfigurationOptionsAsync(null, prof.token).Result;
                        await Serialize(videoSourceConfigurationOptions, $"{_media2Path}/profiles/{conf.Name}/videoSourceConfigurations", "videoSourceConfigurationOptions");
                    });
                }

                var videoEncoderConfigurations = media2.GetVideoEncoderConfigurationsAsync(null, conf.token).Result;
                await Serialize(videoEncoderConfigurations, $"{_media2Path}/profiles/{conf.Name}", "videoEncoderConfigurations");
                foreach (var prof in videoEncoderConfigurations.Configurations)
                {
                    await Serialize(prof, $"{_media2Path}/profiles/{conf.Name}/videoEncoderConfigurations", $"{prof.Name}");

                    await ExecuteAndIgnoreExceptions(async () =>
                    {
                        var videoEncoderConfigurationOptions = media2.GetVideoEncoderConfigurationOptionsAsync(null, prof.token).Result;
                        await Serialize(videoEncoderConfigurationOptions, $"{_media2Path}/profiles/{conf.Name}/videoEncoderConfigurations", "videoEncoderConfigurationOptions");
                    });
                }
            }
        });

        await ExecuteAndIgnoreExceptions(async () =>
        {
            var videoSources = media.GetVideoSourcesAsync().Result;
            foreach (var conf in videoSources.VideoSources)
            {
                var videoSourceModes = media2.GetVideoSourceModesAsync(conf.token).Result;
                await Serialize(videoSourceModes, $"{_media2Path}/videoSources/{conf.token}", "videoSourceModes");
                foreach (var mode in videoSourceModes.VideoSourceModes)
                    await Serialize(mode, $"{_media2Path}/videoSources/{conf.token}/videoSourceModes", $"{mode.token}");
            }
        });

        //var analyticsConfigurations = media2.GetAnalyticsConfigurationsAsync().Result;
        //var audioDecoderConfigurationOptions = media2.GetAudioDecoderConfigurationOptionsAsync().Result;
        //var audioDecoderConfigurations = media2.GetAudioDecoderConfigurationsAsync().Result;
        //var audioEncoderConfigurationOptions = media2.GetAudioEncoderConfigurationOptionsAsync().Result;
        //var audioEncoderConfigurations = media2.GetAudioEncoderConfigurationsAsync().Result;
        //var audioOutputConfigurationOptions = media2.GetAudioOutputConfigurationOptionsAsync().Result;
        //var audioOutputConfigurations = media2.GetAudioOutputConfigurationsAsync().Result;
        //var audioSourceConfigurationOptions = media2.GetAudioSourceConfigurationOptionsAsync().Result;
        //var audioSourceConfigurations = media2.GetAudioSourceConfigurationsAsync().Result;
        //var maskOptions = media2.GetMaskOptionsAsync().Result;
        //var masks = media2.GetMasksAsync().Result;
        //var metadataConfigurationOptions = media2.GetMetadataConfigurationOptionsAsync().Result;
        //var metadataConfigurations = media2.GetMetadataConfigurationsAsync().Result;
        //var osdOptions = media2.GetOSDOptionsAsync().Result;
        //var oSDs = media2.GetOSDsAsync().Result;
        //var serviceCapabilities = media2.GetServiceCapabilitiesAsync().Result;
        //var snapshotUri = media2.GetSnapshotUriAsync().Result;
        //var streamUri = media2.GetStreamUriAsync().Result;
        //var videoEncoderInstances = media2.GetVideoEncoderInstancesAsync().Result;
    }

    private static async Task SaveAllImagingClientGetMethods(MediaClient media, ImagingPortClient imaging)
    {
        var serviceCapabilitiesImaging = imaging.GetServiceCapabilitiesAsync().Result;
        await Serialize(serviceCapabilitiesImaging, $"{_imagingPath}", "serviceCapabilities");

        var videoSources = media.GetVideoSourcesAsync().Result;
        await Serialize(videoSources, $"{_imagingPath}", "videoSources");
        foreach (var conf in videoSources.VideoSources)
        {
            await Serialize(conf, $"{_imagingPath}/videoSources/", $"{conf.token}");

            await ExecuteAndIgnoreExceptions(async () =>
            {
                var status = imaging.GetStatusAsync(conf.token).Result;
                await Serialize(status, $"{_imagingPath}/videoSources/{conf.token}", "status");
            });

            var options = imaging.GetOptionsAsync(conf.token).Result;
            await Serialize(options, $"{_imagingPath}/videoSources/{conf.token}", "options");

            var moveOptions = imaging.GetMoveOptionsAsync(conf.token).Result;
            await Serialize(moveOptions, $"{_imagingPath}/videoSources/{conf.token}", "moveOptions");

            var imagingSettings = imaging.GetImagingSettingsAsync(conf.token).Result;
            await Serialize(imagingSettings, $"{_imagingPath}/videoSources/{conf.token}", "imagingSettings");

            await ExecuteAndIgnoreExceptions(async () =>
            {
                var presets = imaging.GetPresetsAsync(conf.token).Result;
                await Serialize(presets, $"{_imagingPath}/videoSources/{conf.token}", "presets");
                foreach (var pres in presets.Preset)
                    await Serialize(pres, $"{_imagingPath}/videoSources/{conf.token}/presets", $"{pres.Name}");
            });

            await ExecuteAndIgnoreExceptions(async () =>
            {
                var currentPreset = imaging.GetCurrentPresetAsync(conf.token).Result;
                await Serialize(currentPreset, $"{_imagingPath}/videoSources/{conf.token}", "currentPreset");
            });
        }
    }

    private static async Task SaveAllPtzClientGetMethods(MediaClient media, PTZClient ptz)
    {
        var configurations = ptz.GetConfigurationsAsync().Result;
        await Serialize(configurations, $"{_ptzPath}", "configurations");
        foreach (var conf in configurations.PTZConfiguration)
        {
            await Serialize(conf, $"{_ptzPath}/configurations", $"{conf.Name}");

            var configuration = ptz.GetConfigurationAsync(conf.token).Result;
            await Serialize(configuration, $"{_ptzPath}/configurations/{conf.Name}", "configuration");

            var configurationOptions = ptz.GetConfigurationOptionsAsync(conf.token).Result;
            await Serialize(configurationOptions, $"{_ptzPath}/configurations/{conf.Name}", "configurationOptions");
        }

        var profiles = media.GetProfilesAsync().Result;
        await Serialize(profiles, $"{_ptzPath}", "profiles");
        foreach (var conf in profiles.Profiles)
        {
            await Serialize(conf, $"{_ptzPath}/profiles", $"{conf.Name}");

            var status = ptz.GetStatusAsync(conf.token).Result;
            await Serialize(status, $"{_ptzPath}/profiles/{conf.Name}", "status");

            var compatibleConfigurations = ptz.GetCompatibleConfigurationsAsync(conf.token).Result;
            await Serialize(compatibleConfigurations, $"{_ptzPath}/profiles/{conf.Name}", "compatibleConfigurations");
            foreach (var compConf in compatibleConfigurations.PTZConfiguration)
                await Serialize(compConf, $"{_ptzPath}/profiles/{conf.Name}/compatibleConfigurations", $"{compConf.Name}");

            var presets = ptz.GetPresetsAsync(conf.token).Result;
            await Serialize(presets, $"{_ptzPath}/profiles/{conf.Name}", "presets");
            foreach (var pres in presets.Preset)
            {
                await Serialize(pres, $"{_ptzPath}/profiles/{conf.Name}/presets", $"{pres.Name}");

                var presetTour = ptz.GetPresetTourAsync(pres.token, conf.token).Result;
                await Serialize(presetTour, $"{_ptzPath}/profiles/{conf.Name}/presets", "presetTour");
            }

            var presetTours = ptz.GetPresetToursAsync(conf.token).Result;
            await Serialize(presetTours, $"{_ptzPath}/profiles/{conf.Name}", "presetTours");
            foreach (var pres in presetTours.PresetTour)
            {
                await Serialize(pres, $"{_ptzPath}/profiles/{conf.Name}/presetTours", $"{pres.Name}");

                await ExecuteAndIgnoreExceptions(async () =>
                {
                    var presetTourOptions = ptz.GetPresetTourOptionsAsync(conf.token, pres.token).Result;
                    await Serialize(presetTourOptions, $"{_ptzPath}/profiles/{conf.Name}/presetTours/{pres.Name}", "presetTourOptions");
                });
            }
        }

        var nodes = ptz.GetNodesAsync().Result;
        await Serialize(nodes, $"{_ptzPath}", "nodes");
        foreach (var conf in nodes.PTZNode)
        {
            await Serialize(conf, $"{_ptzPath}/nodes", $"{conf.Name}");

            var node = ptz.GetNodeAsync(conf.token).Result;
            await Serialize(node, $"{_ptzPath}/nodes/{conf.Name}", "node");
        }

        var serviceCapabilities = ptz.GetServiceCapabilitiesAsync().Result;
        await Serialize(serviceCapabilities, $"{_ptzPath}", "serviceCapabilities");
    }
}