using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
using OnvifDiscovery;
using CapabilityCategory = netOnvifCore.DeviceManagement.CapabilityCategory;
using DeviceClient = netOnvifCore.DeviceManagement.DeviceClient;
using GetDeviceInformationRequest = netOnvifCore.DeviceManagement.GetDeviceInformationRequest;
using GetEndpointReferenceRequest = netOnvifCore.DeviceManagement.GetEndpointReferenceRequest;
using StreamSetup = netOnvifCore.Media.StreamSetup;
using StreamType = netOnvifCore.Media.StreamType;
using Transport = netOnvifCore.Media.Transport;
using TransportProtocol = netOnvifCore.Media.TransportProtocol;

namespace OnvifTests;

public static class Program
{
    private const string BasePath = "CameraSettings/";
    private const string DevicePath = $"{BasePath}/Device/Get";
    private const string MethodsPath = $"{BasePath}/Methods";
    private const string MediaPath = $"{BasePath}/Media/Get";

    private static readonly List<(string Ip, (string Login, string Password) User)> Cameras = new()
    {
        new ValueTuple<string, (string Login, string Password)>("20.0.1.10", new ValueTuple<string, string>("root", "root")),
        new ValueTuple<string, (string Login, string Password)>("20.0.1.11", new ValueTuple<string, string>("admin", "admin")),
        new ValueTuple<string, (string Login, string Password)>("20.0.1.12", new ValueTuple<string, string>("root", "root")),
        new ValueTuple<string, (string Login, string Password)>("20.0.1.13", new ValueTuple<string, string>("admin", "123456"))
    };

    public static async Task Main()
    {
        var camera = Cameras[0];
        await new Discovery().Discover(1, device => camera = Cameras.First(x => x.Ip == device.Address));
        Console.WriteLine($"Address: {camera.Ip}");

        if (!string.IsNullOrEmpty(camera.Ip))
        {
            if (Directory.Exists(BasePath))
                Directory.Delete(BasePath, true);

            var device = OnvifClientFactory.CreateDeviceClientAsync(camera.Ip, camera.User.Login, camera.User.Password).Result;
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
        await Serialize(typeof(PACSPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(PACSPortClient)}", "getMethods");

        await Serialize(typeof(AccessRulesPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(AccessRulesPortClient)}", "getMethods");

        await Serialize(typeof(ActionEnginePortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(ActionEnginePortClient)}", "getMethods");

        await Serialize(typeof(AnalyticsEnginePortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(AnalyticsEnginePortClient)}", "getMethods");

        await Serialize(typeof(RuleEnginePortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(RuleEnginePortClient)}", "getMethods");

        await Serialize(typeof(AppManagementClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(AppManagementClient)}", "getMethods");

        await Serialize(typeof(AuthenticationBehaviorPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(AuthenticationBehaviorPortClient)}", "getMethods");

        await Serialize(typeof(CredentialPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(CredentialPortClient)}", "getMethods");

        await Serialize(typeof(netOnvifCore.DeviceIO.DeviceClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(netOnvifCore.DeviceIO.DeviceClient)}IO", "getMethods");

        await Serialize(typeof(DeviceIOPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(DeviceIOPortClient)}", "getMethods");

        await Serialize(typeof(DeviceClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(DeviceClient)}", "getMethods");

        await Serialize(typeof(DisplayPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(DisplayPortClient)}", "getMethods");

        await Serialize(typeof(DoorControlPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(DoorControlPortClient)}", "getMethods");

        await Serialize(typeof(CreatePullPointClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(CreatePullPointClient)}", "getMethods");

        await Serialize(typeof(EventPortTypeClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(EventPortTypeClient)}", "getMethods");

        await Serialize(typeof(NotificationConsumerClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(NotificationConsumerClient)}", "getMethods");

        await Serialize(typeof(NotificationProducerClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(NotificationProducerClient)}", "getMethods");

        await Serialize(typeof(PausableSubscriptionManagerClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(PausableSubscriptionManagerClient)}", "getMethods");

        await Serialize(typeof(PullPointClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(PullPointClient)}", "getMethods");

        await Serialize(typeof(PullPointSubscriptionClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(PullPointSubscriptionClient)}", "getMethods");

        await Serialize(typeof(SubscriptionManagerClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(SubscriptionManagerClient)}", "getMethods");

        await Serialize(typeof(ImagingPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(ImagingPortClient)}", "getMethods");

        await Serialize(typeof(MediaClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(MediaClient)}", "getMethods");

        await Serialize(typeof(Media2Client).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(Media2Client)}", "getMethods");

        await Serialize(typeof(ProvisioningServiceClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(ProvisioningServiceClient)}", "getMethods");

        await Serialize(typeof(PTZClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(PTZClient)}", "getMethods");

        await Serialize(typeof(ReceiverPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(ReceiverPortClient)}", "getMethods");

        await Serialize(typeof(RecordingPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(RecordingPortClient)}", "getMethods");

        await Serialize(typeof(ReplayPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(ReplayPortClient)}", "getMethods");

        await Serialize(typeof(SchedulePortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(SchedulePortClient)}", "getMethods");

        await Serialize(typeof(SearchPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(SearchPortClient)}", "getMethods");

        await Serialize(typeof(AdvancedSecurityServiceClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(AdvancedSecurityServiceClient)}", "getMethods");

        await Serialize(typeof(Dot1XClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(Dot1XClient)}", "getMethods");

        await Serialize(typeof(KeystoreClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(KeystoreClient)}", "getMethods");

        await Serialize(typeof(TLSServerClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(TLSServerClient)}", "getMethods");

        await Serialize(typeof(ThermalPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(ThermalPortClient)}", "getMethods");

        await Serialize(typeof(UplinkPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Get"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(UplinkPortClient)}", "getMethods");
    }

    private static async Task AllSetMethods()
    {
        await Serialize(typeof(PACSPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(PACSPortClient)}", "setMethods");

        await Serialize(typeof(AccessRulesPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(AccessRulesPortClient)}", "setMethods");

        await Serialize(typeof(ActionEnginePortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(ActionEnginePortClient)}", "setMethods");

        await Serialize(typeof(AnalyticsEnginePortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(AnalyticsEnginePortClient)}", "setMethods");

        await Serialize(typeof(RuleEnginePortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(RuleEnginePortClient)}", "setMethods");

        await Serialize(typeof(AppManagementClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(AppManagementClient)}", "setMethods");

        await Serialize(typeof(AuthenticationBehaviorPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(AuthenticationBehaviorPortClient)}", "setMethods");

        await Serialize(typeof(CredentialPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(CredentialPortClient)}", "setMethods");

        await Serialize(typeof(netOnvifCore.DeviceIO.DeviceClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(netOnvifCore.DeviceIO.DeviceClient)}IO", "setMethods");

        await Serialize(typeof(DeviceIOPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(DeviceIOPortClient)}", "setMethods");

        await Serialize(typeof(DeviceClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(DeviceClient)}", "setMethods");

        await Serialize(typeof(DisplayPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(DisplayPortClient)}", "setMethods");

        await Serialize(typeof(DoorControlPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(DoorControlPortClient)}", "setMethods");

        await Serialize(typeof(CreatePullPointClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(CreatePullPointClient)}", "setMethods");

        await Serialize(typeof(EventPortTypeClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(EventPortTypeClient)}", "setMethods");

        await Serialize(typeof(NotificationConsumerClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(NotificationConsumerClient)}", "setMethods");

        await Serialize(typeof(NotificationProducerClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(NotificationProducerClient)}", "setMethods");

        await Serialize(typeof(PausableSubscriptionManagerClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(PausableSubscriptionManagerClient)}", "setMethods");

        await Serialize(typeof(PullPointClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(PullPointClient)}", "setMethods");

        await Serialize(typeof(PullPointSubscriptionClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(PullPointSubscriptionClient)}", "setMethods");

        await Serialize(typeof(SubscriptionManagerClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(SubscriptionManagerClient)}", "setMethods");

        await Serialize(typeof(ImagingPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(ImagingPortClient)}", "setMethods");

        await Serialize(typeof(MediaClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(MediaClient)}", "setMethods");

        await Serialize(typeof(Media2Client).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(Media2Client)}", "setMethods");

        await Serialize(typeof(ProvisioningServiceClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(ProvisioningServiceClient)}", "setMethods");

        await Serialize(typeof(PTZClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(PTZClient)}", "setMethods");

        await Serialize(typeof(ReceiverPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(ReceiverPortClient)}", "setMethods");

        await Serialize(typeof(RecordingPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(RecordingPortClient)}", "setMethods");

        await Serialize(typeof(ReplayPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(ReplayPortClient)}", "setMethods");

        await Serialize(typeof(SchedulePortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(SchedulePortClient)}", "setMethods");

        await Serialize(typeof(SearchPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(SearchPortClient)}", "setMethods");

        await Serialize(typeof(AdvancedSecurityServiceClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(AdvancedSecurityServiceClient)}", "setMethods");

        await Serialize(typeof(Dot1XClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(Dot1XClient)}", "setMethods");

        await Serialize(typeof(KeystoreClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(KeystoreClient)}", "setMethods");

        await Serialize(typeof(TLSServerClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(TLSServerClient)}", "setMethods");

        await Serialize(typeof(ThermalPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(ThermalPortClient)}", "setMethods");

        await Serialize(typeof(UplinkPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(UplinkPortClient)}", "setMethods");
    }

    private static async Task AllOtherMethods()
    {
        await Serialize(typeof(PACSPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(PACSPortClient)}", "otherMethods");

        await Serialize(typeof(AccessRulesPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(AccessRulesPortClient)}", "otherMethods");

        await Serialize(typeof(ActionEnginePortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(ActionEnginePortClient)}", "otherMethods");

        await Serialize(typeof(AnalyticsEnginePortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(AnalyticsEnginePortClient)}", "otherMethods");

        await Serialize(typeof(RuleEnginePortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(RuleEnginePortClient)}", "otherMethods");

        await Serialize(typeof(AppManagementClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(AppManagementClient)}", "otherMethods");

        await Serialize(typeof(AuthenticationBehaviorPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(AuthenticationBehaviorPortClient)}", "otherMethods");

        await Serialize(typeof(CredentialPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(CredentialPortClient)}", "otherMethods");

        await Serialize(typeof(netOnvifCore.DeviceIO.DeviceClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(netOnvifCore.DeviceIO.DeviceClient)}IO", "otherMethods");

        await Serialize(typeof(DeviceIOPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(DeviceIOPortClient)}", "otherMethods");

        await Serialize(typeof(DeviceClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(DeviceClient)}", "otherMethods");

        await Serialize(typeof(DisplayPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(DisplayPortClient)}", "otherMethods");

        await Serialize(typeof(DoorControlPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(DoorControlPortClient)}", "otherMethods");

        await Serialize(typeof(CreatePullPointClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(CreatePullPointClient)}", "otherMethods");

        await Serialize(typeof(EventPortTypeClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(EventPortTypeClient)}", "otherMethods");

        await Serialize(typeof(NotificationConsumerClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(NotificationConsumerClient)}", "otherMethods");

        await Serialize(typeof(NotificationProducerClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(NotificationProducerClient)}", "otherMethods");

        await Serialize(typeof(PausableSubscriptionManagerClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(PausableSubscriptionManagerClient)}", "otherMethods");

        await Serialize(typeof(PullPointClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(PullPointClient)}", "otherMethods");

        await Serialize(typeof(PullPointSubscriptionClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(PullPointSubscriptionClient)}", "otherMethods");

        await Serialize(typeof(SubscriptionManagerClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(SubscriptionManagerClient)}", "otherMethods");

        await Serialize(typeof(ImagingPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(ImagingPortClient)}", "otherMethods");

        await Serialize(typeof(MediaClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(MediaClient)}", "otherMethods");

        await Serialize(typeof(Media2Client).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(Media2Client)}", "otherMethods");

        await Serialize(typeof(ProvisioningServiceClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(ProvisioningServiceClient)}", "otherMethods");

        await Serialize(typeof(PTZClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(PTZClient)}", "otherMethods");

        await Serialize(typeof(ReceiverPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(ReceiverPortClient)}", "otherMethods");

        await Serialize(typeof(RecordingPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(RecordingPortClient)}", "otherMethods");

        await Serialize(typeof(ReplayPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(ReplayPortClient)}", "otherMethods");

        await Serialize(typeof(SchedulePortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(SchedulePortClient)}", "otherMethods");

        await Serialize(typeof(SearchPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(SearchPortClient)}", "otherMethods");

        await Serialize(typeof(AdvancedSecurityServiceClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(AdvancedSecurityServiceClient)}", "otherMethods");

        await Serialize(typeof(Dot1XClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(Dot1XClient)}", "otherMethods");

        await Serialize(typeof(KeystoreClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(KeystoreClient)}", "otherMethods");

        await Serialize(typeof(TLSServerClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(TLSServerClient)}", "otherMethods");

        await Serialize(typeof(ThermalPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(ThermalPortClient)}", "otherMethods");

        await Serialize(typeof(UplinkPortClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.Name.StartsWith("Get") && !m.Name.StartsWith("Set"))
            .Select(x => x.Name)
            .OrderBy(x => x), $"{MethodsPath}/{nameof(UplinkPortClient)}", "otherMethods");
    }

    private static async Task AllDeviceGetMethods(DeviceClient device)
    {
        var accessPolicy = device.GetAccessPolicyAsync().Result;
        await Serialize(accessPolicy, $"{DevicePath}", "accessPolicy");

        // MicroDigital не поддерживает var authFailureWarningConfiguration = device.GetAuthFailureWarningConfigurationAsync(new GetAuthFailureWarningConfigurationRequest()).Result;
        // MicroDigital не поддерживает var authFailureWarningOptions = device.GetAuthFailureWarningOptionsAsync(new GetAuthFailureWarningOptionsRequest()).Result;

        // Infinity не поддерживает var caCertificates = device.GetCACertificatesAsync().Result;
        // await Serialize(caCertificates, $"{DevicePath}", "caCertificates");
        // foreach (var conf in caCertificates.CACertificate)
        // {
        //     var pkcs10Request = device.GetPkcs10RequestAsync(conf.CertificateID, "", new BinaryData()).Result;
        //     await Serialize(pkcs10Request, $"{DevicePath}/caCertificates", "pkcs10Request");
        // }

        var capabilities = device.GetCapabilitiesAsync(new[] { CapabilityCategory.All }).Result;
        await Serialize(capabilities, $"{DevicePath}", "capabilities");

        // Infinity не поддерживает var certificates = device.GetCertificatesAsync().Result;
        // await Serialize(certificates, $"{DevicePath}", "certificates");
        // foreach (var conf in certificates.NvtCertificate)
        // {
        //     var certificateInformation = device.GetCertificateInformationAsync(conf.CertificateID).Result;
        //     await Serialize(certificateInformation, $"{DevicePath}/certificates", $"{certificateInformation.CertificateInformation.CertificateID}");
        // }

        // Infinity не поддерживает var certificatesStatus = device.GetCertificatesStatusAsync().Result;
        // await Serialize(certificatesStatus, $"{DevicePath}", "certificatesStatus");
        // foreach (var conf in certificatesStatus.CertificateStatus)
        //     await Serialize(conf, $"{DevicePath}/certificatesStatus", $"{conf.CertificateID}");

        // Infinity не поддерживает var clientCertificateMode = device.GetClientCertificateModeAsync().Result;
        // await Serialize(clientCertificateMode, $"{DevicePath}", "clientCertificateMode");

        var deviceInformation = device.GetDeviceInformationAsync(new GetDeviceInformationRequest()).Result;
        await Serialize(deviceInformation, $"{DevicePath}", "deviceInformation");

        var discoveryMode = device.GetDiscoveryModeAsync().Result;
        await Serialize(discoveryMode, $"{DevicePath}", "discoveryMode");

        var dns = device.GetDNSAsync().Result;
        await Serialize(dns, $"{DevicePath}", "dns");

        // Infinity не поддерживает var dot11Capabilities = device.GetDot11CapabilitiesAsync(new XmlElement[] { }).Result;
        // await Serialize(dot11Capabilities, $"{DevicePath}", "dot11Capabilities");

        var zeroConfiguration = device.GetZeroConfigurationAsync().Result;
        await Serialize(zeroConfiguration, $"{DevicePath}", "zeroConfiguration");
        // Infinity не поддерживает var dot11Status = device.GetDot11StatusAsync(zeroConfiguration.InterfaceToken).Result;
        // await Serialize(dot11Status, $"{DevicePath}", "dot11Status");

        // Infinity не поддерживает var dot1XConfigurations = device.GetDot1XConfigurationsAsync().Result;
        // await Serialize(dot1XConfigurations, $"{DevicePath}", "dot1XConfigurations");
        // foreach (var conf in dot1XConfigurations.Dot1XConfiguration)
        // {
        //     var dot1XConfiguration = device.GetDot1XConfigurationAsync(conf.Dot1XConfigurationToken).Result;
        //     await Serialize(dot1XConfiguration, $"{DevicePath}/dot1XConfigurations", $"{dot1XConfiguration.Dot1XConfigurationToken}");
        // }

        // Infinity не поддерживает var dpAddresses = device.GetDPAddressesAsync().Result;
        // await Serialize(dpAddresses, $"{DevicePath}", "dpAddresses");
        // foreach (var conf in dpAddresses.DPAddress)
        //     await Serialize(conf, $"{DevicePath}/dpAddresses", $"{conf.DNSname}");

        // Infinity не поддерживает var dynamicDns = device.GetDynamicDNSAsync().Result;
        // await Serialize(dynamicDns, $"{DevicePath}", "dynamicDns");

        var endpointReference = device.GetEndpointReferenceAsync(new GetEndpointReferenceRequest()).Result;
        await Serialize(endpointReference, $"{DevicePath}", "endpointReference");

        // MicroDigital не поддерживает var geoLocation = device.GetGeoLocationAsync().Result;

        var hostname = device.GetHostnameAsync().Result;
        await Serialize(hostname, $"{DevicePath}", "hostname");

        // Infinity не поддерживает var ipAddressFilter = device.GetIPAddressFilterAsync().Result;
        // await Serialize(ipAddressFilter, $"{DevicePath}", "ipAddressFilter");

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

        // MicroDigital не поддерживает var passwordComplexityConfiguration = device.GetPasswordComplexityConfigurationAsync(new GetPasswordComplexityConfigurationRequest()).Result;
        // MicroDigital не поддерживает var passwordComplexityOptions = device.GetPasswordComplexityOptionsAsync(new GetPasswordComplexityOptionsRequest()).Result;
        // MicroDigital не поддерживает var passwordHistoryConfiguration = device.GetPasswordHistoryConfigurationAsync(new GetPasswordHistoryConfigurationRequest()).Result;

        var relayOutputs = device.GetRelayOutputsAsync().Result;
        await Serialize(relayOutputs, $"{DevicePath}", "relayOutputs");
        foreach (var conf in relayOutputs.RelayOutputs)
            await Serialize(conf, $"{DevicePath}/relayOutputs", $"{conf.token}");

        // Infinity не поддерживает var remoteDiscoveryMode = device.GetRemoteDiscoveryModeAsync().Result;
        // await Serialize(remoteDiscoveryMode, $"{DevicePath}", "remoteDiscoveryMode");

        // Infinity не поддерживает var remoteUser = device.GetRemoteUserAsync().Result;
        // await Serialize(remoteUser, $"{DevicePath}", "remoteUser");

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

        // MicroDigital не поддерживает var storageConfigurations = device.GetStorageConfigurationsAsync().Result;
        // foreach (var conf in storageConfigurations.StorageConfigurations)
        // {
        //     var storageConfiguration = device.GetStorageConfigurationAsync(conf.token).Result;
        // }

        // Infinity не поддерживает var systemBackup = device.GetSystemBackupAsync().Result;
        // await Serialize(systemBackup, $"{DevicePath}", "systemBackup");
        // foreach (var conf in systemBackup.BackupFiles)
        //     await Serialize(systemBackup, $"{DevicePath}/systemBackup", $"{conf.Name}");

        var systemDateAndTime = device.GetSystemDateAndTimeAsync().Result;
        await Serialize(systemDateAndTime, $"{DevicePath}", "systemDateAndTime");

        // Infinity не поддерживает var systemLogAccess = device.GetSystemLogAsync(SystemLogType.Access).Result;
        // await Serialize(systemLogAccess, $"{DevicePath}", "systemLogAccess");

        // Infinity не поддерживает var systemLogSystem = device.GetSystemLogAsync(SystemLogType.System).Result;
        // await Serialize(systemLogSystem, $"{DevicePath}", "systemLogSystem");

        // Infinity не поддерживает var systemSupportInformation = device.GetSystemSupportInformationAsync().Result;
        // await Serialize(systemSupportInformation, $"{DevicePath}", "systemSupportInformation");

        // Infinity не поддерживает var systemUris = device.GetSystemUrisAsync(new GetSystemUrisRequest()).Result;
        // await Serialize(systemUris, $"{DevicePath}", "systemUris");
        // foreach (var conf in systemUris.SystemLogUris)
        //     await Serialize(conf, $"{DevicePath}/systemUris", $"{conf.Uri}");

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
                    await Serialize(opt, $"{MediaPath}/audioEncoderConfigurations/audioEncoderConfigurationOptions/Options", $"{opt.Encoding}");
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

            // MicroDigital не поддерживает var compatibleVideoAnalyticsConfigurations = media.GetCompatibleVideoAnalyticsConfigurationsAsync(prof.token).Result;

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

        // MicroDigital не поддерживает var metadataConfigurations = media.GetMetadataConfigurationsAsync().Result;
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

        var streamSetup = new StreamSetup { Stream = StreamType.RTPUnicast, Transport = new Transport { Protocol = TransportProtocol.UDP, Tunnel = null } };
        foreach (var prof in profiles.Profiles)
        {
            var streamUri = media.GetStreamUriAsync(streamSetup, prof.token).Result;
            await Serialize(streamUri, $"{MediaPath}/streamUri", $"{prof.Name}");
        }

        // MicroDigital не поддерживает var videoAnalyticsConfigurations = media.GetVideoAnalyticsConfigurationsAsync().Result;
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

            // MicroDigital не поддерживает var oSDs = media.GetOSDsAsync(conf.token).Result;
            // MicroDigital не поддерживает var osd = media.GetOSDAsync(new GetOSDRequest()).Result;
            // MicroDigital не поддерживает var osdOptions = media.GetOSDOptionsAsync(new GetOSDOptionsRequest()).Result;

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