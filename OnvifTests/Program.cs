using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using netOnvifCore;
using netOnvifCore.DeviceManagement;
using netOnvifCore.Media;
using Newtonsoft.Json;
using OnvifDiscovery;
using Formatting = Newtonsoft.Json.Formatting;

namespace OnvifTests;

public static class Program
{
    private const string BasePath = "CameraSettings/";
    private const string DevicePath = $"{BasePath}/Device/Get";
    private const string MethodsPath = $"{BasePath}/Methods";
    private const string MediaPath = $"{BasePath}/Media/Get";

    public static async Task Main()
    {
        var cameraIp = "10.59.219.12";
        await new Discovery().Discover(1, device => cameraIp = device.Address);
        Console.WriteLine($"Address: {cameraIp}");

        if (!string.IsNullOrEmpty(cameraIp))
        {
            if (Directory.Exists(BasePath))
                Directory.Delete(BasePath, true);

            var device = OnvifClientFactory.CreateDeviceClientAsync(cameraIp, "root", "root").Result;
            var media = await OnvifClientFactory.CreateMediaClientAsync(device);

            await AllGetMethods();
            await AllSetMethods();
            await AllOtherMethods();
            await AllDeviceGetMethods(device);
            await AllMediaGetMethods(media);
        }
    }

    private static async Task Serialize(object obj, string directory, string fileName)
    {
        Directory.CreateDirectory($"./{directory}");
        await File.WriteAllTextAsync($"./{directory}/{fileName}.json", JsonConvert.SerializeObject(obj, Formatting.Indented));
    }

    private static async Task AllGetMethods()
    {
        var deviceGetMethods = typeof(DeviceClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x);
        await Serialize(deviceGetMethods, $"{MethodsPath}/Device", "getMethods");

        var mediaGetMethods = typeof(MediaClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x);
        await Serialize(mediaGetMethods, $"{MethodsPath}/Media", "getMethods");
    }

    private static async Task AllSetMethods()
    {
        var deviceGetMethods = typeof(DeviceClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x);
        await Serialize(deviceGetMethods, $"{MethodsPath}/Device", "setMethods");

        var mediaGetMethods = typeof(MediaClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x);
        await Serialize(mediaGetMethods, $"{MethodsPath}/Media", "setMethods");
    }

    private static async Task AllOtherMethods()
    {
        var deviceGetMethods = typeof(DeviceClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x);
        await Serialize(deviceGetMethods, $"{MethodsPath}/Device", "otherMethods");

        var mediaGetMethods = typeof(MediaClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x);
        await Serialize(mediaGetMethods, $"{MethodsPath}/Media", "otherMethods");
    }

    private static async Task AllDeviceGetMethods(DeviceClient device)
    {
        var accessPolicy = device.GetAccessPolicyAsync().Result;
        await Serialize(accessPolicy, $"{DevicePath}", "accessPolicy");

        // ошибка var authFailureWarningConfiguration = device.GetAuthFailureWarningConfigurationAsync(new GetAuthFailureWarningConfigurationRequest()).Result;
        // ошибка var authFailureWarningOptions = device.GetAuthFailureWarningOptionsAsync(new GetAuthFailureWarningOptionsRequest()).Result;

        var caCertificates = device.GetCACertificatesAsync().Result;
        await Serialize(caCertificates, $"{DevicePath}", "caCertificates");
        foreach (var conf in caCertificates.CACertificate)
        {
            var pkcs10Request = device.GetPkcs10RequestAsync(conf.CertificateID, "", new BinaryData()).Result;
            await Serialize(pkcs10Request, $"{DevicePath}/caCertificates", "pkcs10Request");
        }

        var capabilities = device.GetCapabilitiesAsync(new[] { CapabilityCategory.All }).Result;
        await Serialize(capabilities, $"{DevicePath}", "capabilities");

        var certificates = device.GetCertificatesAsync().Result;
        await Serialize(certificates, $"{DevicePath}", "certificates");
        foreach (var conf in certificates.NvtCertificate)
        {
            var certificateInformation = device.GetCertificateInformationAsync(conf.CertificateID).Result;
            await Serialize(certificateInformation, $"{DevicePath}/certificates", $"{certificateInformation.CertificateInformation.CertificateID}");
        }

        var certificatesStatus = device.GetCertificatesStatusAsync().Result;
        await Serialize(certificatesStatus, $"{DevicePath}", "certificatesStatus");
        foreach (var conf in certificatesStatus.CertificateStatus)
        {
            await Serialize(conf, $"{DevicePath}/certificatesStatus", $"{conf.CertificateID}");  
        }

        var clientCertificateMode = device.GetClientCertificateModeAsync().Result;
        await Serialize(clientCertificateMode, $"{DevicePath}", "clientCertificateMode");

        var deviceInformation = device.GetDeviceInformationAsync(new GetDeviceInformationRequest()).Result;
        await Serialize(deviceInformation, $"{DevicePath}", "deviceInformation");

        var discoveryMode = device.GetDiscoveryModeAsync().Result;
        await Serialize(discoveryMode, $"{DevicePath}", "discoveryMode");

        var dns = device.GetDNSAsync().Result;
        await Serialize(dns, $"{DevicePath}", "dns");

        var dot11Capabilities = device.GetDot11CapabilitiesAsync(new XmlElement[] { }).Result;
        await Serialize(dot11Capabilities, $"{DevicePath}", "dot11Capabilities");

        var zeroConfiguration = device.GetZeroConfigurationAsync().Result;
        await Serialize(zeroConfiguration, $"{DevicePath}", "zeroConfiguration");
        var dot11Status = device.GetDot11StatusAsync(zeroConfiguration.InterfaceToken).Result;
        await Serialize(dot11Status, $"{DevicePath}", "dot11Status");

        var dot1XConfigurations = device.GetDot1XConfigurationsAsync().Result;
        await Serialize(dot1XConfigurations, $"{DevicePath}", "dot1XConfigurations");
        foreach (var conf in dot1XConfigurations.Dot1XConfiguration)
        {
            var dot1XConfiguration = device.GetDot1XConfigurationAsync(conf.Dot1XConfigurationToken).Result;
            await Serialize(dot1XConfiguration, $"{DevicePath}/dot1XConfigurations", $"{dot1XConfiguration.Dot1XConfigurationToken}");
        }

        var dpAddresses = device.GetDPAddressesAsync().Result;
        await Serialize(dpAddresses, $"{DevicePath}", "dpAddresses");
        foreach (var conf in dpAddresses.DPAddress) 
            await Serialize(conf, $"{DevicePath}/dpAddresses", $"{conf.DNSname}");

        var dynamicDns = device.GetDynamicDNSAsync().Result;
        await Serialize(dynamicDns, $"{DevicePath}", "dynamicDns");

        var endpointReference = device.GetEndpointReferenceAsync(new GetEndpointReferenceRequest()).Result;
        await Serialize(endpointReference, $"{DevicePath}", "endpointReference");

        // ошибка var geoLocation = device.GetGeoLocationAsync().Result;

        var hostname = device.GetHostnameAsync().Result;
        await Serialize(hostname, $"{DevicePath}", "hostname");

        var ipAddressFilter = device.GetIPAddressFilterAsync().Result;
        await Serialize(ipAddressFilter, $"{DevicePath}", "ipAddressFilter");

        var networkDefaultGateway = device.GetNetworkDefaultGatewayAsync().Result;
        await Serialize(networkDefaultGateway, $"{DevicePath}", "networkDefaultGateway");

        var networkInterfaces = device.GetNetworkInterfacesAsync().Result;
        await Serialize(networkInterfaces, $"{DevicePath}", "networkInterfaces");
        foreach (var conf in networkInterfaces.NetworkInterfaces) 
            await Serialize(conf, $"{DevicePath}/networkInterfaces", $"{conf.token}");

        var networkProtocols = device.GetNetworkProtocolsAsync().Result;
        await Serialize(networkProtocols, $"{DevicePath}", "networkProtocols");
        foreach (var conf in networkProtocols.NetworkProtocols) 
            await Serialize(conf, $"{DevicePath}/networkProtocols", $"{conf.Name}");
        
        var ntp = device.GetNTPAsync().Result;
        await Serialize(ntp, $"{DevicePath}", "ntp");

        // ошибка var passwordComplexityConfiguration = device.GetPasswordComplexityConfigurationAsync(new GetPasswordComplexityConfigurationRequest()).Result;
        // ошибка var passwordComplexityOptions = device.GetPasswordComplexityOptionsAsync(new GetPasswordComplexityOptionsRequest()).Result;
        // ошибка var passwordHistoryConfiguration = device.GetPasswordHistoryConfigurationAsync(new GetPasswordHistoryConfigurationRequest()).Result;

        var relayOutputs = device.GetRelayOutputsAsync().Result;
        await Serialize(relayOutputs, $"{DevicePath}", "relayOutputs");
        foreach (var conf in relayOutputs.RelayOutputs) 
            await Serialize(conf, $"{DevicePath}/relayOutputs", $"{conf.token}");

        var remoteDiscoveryMode = device.GetRemoteDiscoveryModeAsync().Result;
        await Serialize(remoteDiscoveryMode, $"{DevicePath}", "remoteDiscoveryMode");

        var remoteUser = device.GetRemoteUserAsync().Result;
        await Serialize(remoteUser, $"{DevicePath}", "remoteUser");

        var scopes = device.GetScopesAsync().Result;
        await Serialize(scopes, $"{DevicePath}", "scopes");
        foreach (var conf in scopes.Scopes) 
            await Serialize(conf, $"{DevicePath}/scopes", $"{conf.ScopeItem.Split('/').Last()}");

        var serviceCapabilities = device.GetServiceCapabilitiesAsync().Result;
        await Serialize(serviceCapabilities, $"{DevicePath}", "serviceCapabilities");

        var services = device.GetServicesAsync(true).Result;
        await Serialize(services, $"{DevicePath}", "services");
        foreach (var conf in services.Service) 
            await Serialize(conf, $"{DevicePath}/services", $"{conf.Namespace.Split('/')[^1]}");

        // ошибка var storageConfigurations = device.GetStorageConfigurationsAsync().Result;
        // foreach (var conf in storageConfigurations.StorageConfigurations)
        // {
        //     var storageConfiguration = device.GetStorageConfigurationAsync(conf.token).Result;
        // }

        var systemBackup = device.GetSystemBackupAsync().Result;
        await Serialize(systemBackup, $"{DevicePath}", "systemBackup");
        foreach (var conf in systemBackup.BackupFiles)
        {
            await Serialize(systemBackup, $"{DevicePath}/systemBackup", $"{conf.Name}");
        }

        var systemDateAndTime = device.GetSystemDateAndTimeAsync().Result;
        await Serialize(systemDateAndTime, $"{DevicePath}", "systemDateAndTime");

        var systemLogAccess = device.GetSystemLogAsync(SystemLogType.Access).Result;
        await Serialize(systemLogAccess, $"{DevicePath}", "systemLogAccess");

        var systemLogSystem = device.GetSystemLogAsync(SystemLogType.System).Result;
        await Serialize(systemLogSystem, $"{DevicePath}", "systemLogSystem");

        var systemSupportInformation = device.GetSystemSupportInformationAsync().Result;
        await Serialize(systemSupportInformation, $"{DevicePath}", "systemSupportInformation");

        var systemUris = device.GetSystemUrisAsync(new GetSystemUrisRequest()).Result;
        await Serialize(systemUris, $"{DevicePath}", "systemUris");
        foreach (var conf in systemUris.SystemLogUris) 
            await Serialize(conf, $"{DevicePath}/systemUris", $"{conf.Uri}");

        var users = device.GetUsersAsync().Result;
        await Serialize(users, $"{DevicePath}", "users");
        foreach (var conf in users.User) 
            await Serialize(conf, $"{DevicePath}/users", $"{conf.Username}");

        var wsdlUrl = device.GetWsdlUrlAsync().Result;
        await Serialize(wsdlUrl, $"{DevicePath}", "wsdlUrl");
    }

    private static async Task AllMediaGetMethods(MediaClient media)
    {
        var profiles = media.GetProfilesAsync().Result;
        await Serialize(profiles, $"{MediaPath}", "profiles");
        foreach (var conf in profiles.Profiles) 
            await Serialize(conf, $"{MediaPath}/profiles", $"{conf.token}");

        var audioDecoderConfigurations = media.GetAudioDecoderConfigurationsAsync().Result;
        await Serialize(audioDecoderConfigurations, $"{MediaPath}", "audioDecoderConfigurations");
        foreach (var conf in audioDecoderConfigurations.Configurations)
        {
            var audioDecoderConfiguration = media.GetAudioDecoderConfigurationAsync(conf.token).Result;
            await Serialize(audioDecoderConfiguration, $"{MediaPath}/audioDecoderConfigurations", $"{audioDecoderConfiguration.Name}");
            foreach (var prof in profiles.Profiles)
            {
                var audioDecoderConfigurationOptions = media.GetAudioDecoderConfigurationOptionsAsync(conf.token, prof.token).Result;
                await Serialize(audioDecoderConfigurationOptions, $"{MediaPath}/audioDecoderConfigurations/AudioDecoderConfigurationOptions", $"{prof.Name}");
            }
        }

        var audioEncoderConfigurations = media.GetAudioEncoderConfigurationsAsync().Result;
        await Serialize(audioEncoderConfigurations, $"{MediaPath}", "audioEncoderConfigurations");
        foreach (var conf in audioEncoderConfigurations.Configurations)
        {
            var audioEncoderConfiguration = media.GetAudioEncoderConfigurationAsync(conf.token).Result;
            await Serialize(audioEncoderConfiguration, $"{MediaPath}/audioEncoderConfigurations", $"{audioEncoderConfiguration.Name}");
            foreach (var prof in profiles.Profiles)
            {
                var audioEncoderConfigurationOptions = media.GetAudioEncoderConfigurationOptionsAsync(conf.token, prof.token).Result;
                await Serialize(audioEncoderConfigurationOptions, $"{MediaPath}/audioEncoderConfigurations/audioEncoderConfigurationOptions", $"{prof.Name}");
                foreach (var opt in audioEncoderConfigurationOptions.Options)
                    await Serialize(opt, $"{MediaPath}/audioEncoderConfigurations/audioEncoderConfigurationOptions/Options                                                                  ", $"{opt.Encoding}");
            }
        }

        var audioOutputs = media.GetAudioOutputsAsync().Result;
        await Serialize(audioOutputs, $"{MediaPath}", "audioOutputs");
        foreach (var conf in audioOutputs.AudioOutputs) 
            await Serialize(conf, $"{MediaPath}/audioOutputs", $"{conf.token}");

        var audioOutputConfigurations = media.GetAudioOutputConfigurationsAsync().Result;
        await Serialize(audioOutputConfigurations, $"{MediaPath}", "audioOutputConfigurations");
        foreach (var conf in audioOutputConfigurations.Configurations)
        {
            var audioOutputConfiguration = media.GetAudioOutputConfigurationAsync(conf.token).Result;
            await Serialize(audioOutputConfiguration, $"{MediaPath}/audioOutputConfiguration", $"{audioOutputConfiguration.Name}");
            foreach (var prof in profiles.Profiles)
            {
                var audioOutputConfigurationOptions = media.GetAudioOutputConfigurationOptionsAsync(conf.token, prof.token).Result;
                await Serialize(audioOutputConfigurationOptions, $"{MediaPath}/audioOutputConfiguration/audioOutputConfigurationOptions", $"{prof.Name}");
            }
        }

        var audioSources = media.GetAudioSourcesAsync().Result;
        await Serialize(audioSources, $"{MediaPath}", "audioSources");
        foreach (var conf in audioSources.AudioSources) 
            await Serialize(conf, $"{MediaPath}/audioSources", $"{conf.token}");

        var audioSourceConfigurations = media.GetAudioSourceConfigurationsAsync().Result;
        await Serialize(audioSourceConfigurations, $"{MediaPath}", "audioSourceConfigurations");
        foreach (var conf in audioSourceConfigurations.Configurations)
        {
            var audioSourceConfiguration = media.GetAudioSourceConfigurationAsync(conf.token).Result;
            await Serialize(audioSourceConfiguration, $"{MediaPath}/audioSourceConfigurations", $"{audioSourceConfiguration.Name}");
            foreach (var prof in profiles.Profiles)
            {
                var audioSourceConfigurationOptions = media.GetAudioSourceConfigurationOptionsAsync(conf.token, prof.token).Result;
                await Serialize(audioSourceConfigurationOptions, $"{MediaPath}/audioSourceConfigurations/audioSourceConfigurationOptions", $"{prof.Name}");
            }
        }

        foreach (var prof in profiles.Profiles)
        {
            var compatibleAudioDecoderConfigurations = media.GetCompatibleAudioDecoderConfigurationsAsync(prof.token).Result;
            await Serialize(compatibleAudioDecoderConfigurations, $"{MediaPath}/compatibleAudioDecoderConfigurations", $"{prof.Name}");
            foreach (var conf in compatibleAudioDecoderConfigurations.Configurations)
                await Serialize(conf, $"{MediaPath}/compatibleAudioDecoderConfigurations/{prof.Name}", $"{conf.Name}");

            var compatibleAudioEncoderConfigurations = media.GetCompatibleAudioEncoderConfigurationsAsync(prof.token).Result;
            await Serialize(compatibleAudioEncoderConfigurations, $"{MediaPath}/compatibleAudioEncoderConfigurations", $"{prof.Name}");
            foreach (var conf in compatibleAudioEncoderConfigurations.Configurations)
                await Serialize(conf, $"{MediaPath}/compatibleAudioEncoderConfigurations/{prof.Name}", $"{conf.Name}");

            var compatibleAudioOutputConfigurations = media.GetCompatibleAudioOutputConfigurationsAsync(prof.token).Result;
            await Serialize(compatibleAudioOutputConfigurations, $"{MediaPath}/compatibleAudioOutputConfigurations", $"{prof.Name}");
            foreach (var conf in compatibleAudioOutputConfigurations.Configurations)
                await Serialize(conf, $"{MediaPath}/compatibleAudioOutputConfigurations/{prof.Name}", $"{conf.Name}");

            var compatibleAudioSourceConfigurations = media.GetCompatibleAudioSourceConfigurationsAsync(prof.token).Result;
            await Serialize(compatibleAudioSourceConfigurations, $"{MediaPath}/compatibleAudioSourceConfigurations", $"{prof.Name}");
            foreach (var conf in compatibleAudioSourceConfigurations.Configurations)
                await Serialize(conf, $"{MediaPath}/compatibleAudioSourceConfigurations/{prof.Name}", $"{conf.Name}");

            var compatibleMetadataConfigurations = media.GetCompatibleMetadataConfigurationsAsync(prof.token).Result;
            await Serialize(compatibleMetadataConfigurations, $"{MediaPath}/compatibleMetadataConfigurations", $"{prof.Name}");
            foreach (var conf in compatibleMetadataConfigurations.Configurations)
                await Serialize(conf, $"{MediaPath}/compatibleMetadataConfigurations/{prof.Name}", $"{conf.Name}");

            // не поддерживается var compatibleVideoAnalyticsConfigurations = media.GetCompatibleVideoAnalyticsConfigurationsAsync(prof.token).Result;

            var compatibleVideoEncoderConfigurations = media.GetCompatibleVideoEncoderConfigurationsAsync(prof.token).Result;
            await Serialize(compatibleVideoEncoderConfigurations, $"{MediaPath}/compatibleVideoEncoderConfigurations", $"{prof.Name}");
            foreach (var conf in compatibleVideoEncoderConfigurations.Configurations)
                await Serialize(conf, $"{MediaPath}/compatibleVideoEncoderConfigurations/{prof.Name}", $"{conf.Name}");

            var compatibleVideoSourceConfigurations = media.GetCompatibleVideoSourceConfigurationsAsync(prof.token).Result;
            await Serialize(compatibleVideoSourceConfigurations, $"{MediaPath}/compatibleVideoSourceConfigurations", $"{prof.Name}");
            foreach (var conf in compatibleVideoSourceConfigurations.Configurations)
                await Serialize(conf, $"{MediaPath}/compatibleVideoSourceConfigurations/{prof.Name}", $"{conf.Name}");

            var profile = media.GetProfileAsync(prof.token).Result;
            await Serialize(profile, $"{MediaPath}/profile", $"{prof.Name}");

            var snapshotUri = media.GetSnapshotUriAsync(prof.token).Result;
            await Serialize(snapshotUri, $"{MediaPath}/snapshotUri", $"{prof.Name}");
        }

        // ошибка var metadataConfigurations = media.GetMetadataConfigurationsAsync().Result;
        // foreach (var conf in metadataConfigurations.Configurations)
        // {
        //     var metadataConfiguration = media.GetMetadataConfigurationAsync(conf.token).Result;
        //     foreach (var prof in profiles.Profiles)
        //     {
        //         var metadataConfigurationOptions = media.GetMetadataConfigurationOptionsAsync(conf.token, prof.token).Result;
        //     }
        // }

        var serviceCapabilities = media.GetServiceCapabilitiesAsync().Result;
        await Serialize(serviceCapabilities, $"{MediaPath}", "serviceCapabilities");

        // не понял как вызвать var streamUri = media.GetStreamUriAsync().Result;

        // не поддерживается var videoAnalyticsConfigurations = media.GetVideoAnalyticsConfigurationsAsync().Result;
        // foreach (var conf in videoAnalyticsConfigurations.Configurations)
        // {
        //     var videoAnalyticsConfiguration = media.GetVideoAnalyticsConfigurationAsync(conf.token).Result;
        // }

        var videoEncoderConfigurations = media.GetVideoEncoderConfigurationsAsync().Result;
        await Serialize(videoEncoderConfigurations, $"{MediaPath}", "videoEncoderConfigurations");
        foreach (var conf in videoEncoderConfigurations.Configurations)
        {
            var videoEncoderConfiguration = media.GetVideoEncoderConfigurationAsync(conf.token).Result;
            await Serialize(videoEncoderConfiguration, $"{MediaPath}/videoEncoderConfigurations", $"{videoEncoderConfiguration.Name}");
            foreach (var prof in profiles.Profiles)
            {
                var videoEncoderConfigurationOptions = media.GetVideoEncoderConfigurationOptionsAsync(conf.token, prof.token).Result;
                await Serialize(videoEncoderConfigurationOptions, $"{MediaPath}/videoEncoderConfigurations/videoEncoderConfigurationOptions", $"{prof.Name}");
            }
        }

        var videoSourceConfigurations = media.GetVideoSourceConfigurationsAsync().Result;
        await Serialize(videoSourceConfigurations, $"{MediaPath}", "videoSourceConfigurations");
        foreach (var conf in videoSourceConfigurations.Configurations)
        {
            var videoSourceConfiguration = media.GetVideoSourceConfigurationAsync(conf.token).Result;
            await Serialize(videoSourceConfiguration, $"{MediaPath}/videoSourceConfigurations", $"{videoSourceConfiguration.Name}");
            foreach (var prof in profiles.Profiles)
            {
                var videoSourceConfigurationOptions = media.GetVideoSourceConfigurationOptionsAsync(conf.token, prof.token).Result;
                await Serialize(videoSourceConfigurationOptions, $"{MediaPath}/videoSourceConfigurations/videoSourceConfigurationOptions", $"{prof.Name}");
            }

            // ошибка var oSDs = media.GetOSDsAsync(conf.token).Result;
            // ошибка var osd = media.GetOSDAsync(new GetOSDRequest()).Result;
            // ошибка var osdOptions = media.GetOSDOptionsAsync(new GetOSDOptionsRequest()).Result;

            var guaranteedNumberOfVideoEncoderInstances = media.GetGuaranteedNumberOfVideoEncoderInstancesAsync(new GetGuaranteedNumberOfVideoEncoderInstancesRequest(conf.token)).Result;
            await Serialize(guaranteedNumberOfVideoEncoderInstances, $"{MediaPath}/videoSourceConfigurations", "guaranteedNumberOfVideoEncoderInstances");
        }

        var videoSources = media.GetVideoSourcesAsync().Result;
        await Serialize(videoSources, $"{MediaPath}", "videoSources");
        foreach (var conf in videoSources.VideoSources)
        {
            var videoSourceModes = media.GetVideoSourceModesAsync(conf.token).Result;
            await Serialize(videoSourceModes, $"{MediaPath}/videoSources", $"{videoSourceModes}");
            foreach (var mode in videoSourceModes.VideoSourceModes) 
                await Serialize(mode, $"{MediaPath}/videoSources/videoSourceModes", $"{mode.token}");
        }
    }
}