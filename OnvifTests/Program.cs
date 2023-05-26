using System;
using System.Threading.Tasks;
using netOnvifCore;
using netOnvifCore.DeviceManagement;
using OnvifDiscovery;

namespace OnvifTests;

public static class Program
{
    public static async Task Main()
    {
        var cameraIp = "";
        await new Discovery().Discover(1, device => cameraIp = device.Address);
        Console.WriteLine($"Address: {cameraIp}");

        if (!string.IsNullOrEmpty(cameraIp))
        {
            var device            = await OnvifClientFactory.CreateDeviceClientAsync(cameraIp, "root", "root");
            var deviceInformation = await device.GetDeviceInformationAsync(new GetDeviceInformationRequest());

            Console.WriteLine($"Manufacturer: {deviceInformation.Manufacturer}");
            Console.WriteLine($"Model: {deviceInformation.Model}");
            Console.WriteLine($"FirmwareVersion: {deviceInformation.FirmwareVersion}");
            Console.WriteLine($"HardwareId: {deviceInformation.HardwareId}");
            Console.WriteLine($"SerialNumber: {deviceInformation.SerialNumber}");

            var media = await OnvifClientFactory.CreateMediaClientAsync(device);

            var configurations = await media.GetVideoEncoderConfigurationsAsync();

            foreach (var videoEncoderConfiguration in configurations.Configurations)
            {
                var options = await media.GetVideoEncoderConfigurationOptionsAsync(videoEncoderConfiguration.token, "");
                Console.WriteLine(options.GuaranteedFrameRateSupported);
            }
        }
    }
}