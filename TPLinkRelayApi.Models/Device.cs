using System.Net;
using System.Net.NetworkInformation;

namespace TPLinkRelayApi.Models;

public readonly record struct Device(string Alias, IPAddress IPAddress, PhysicalAddress PhysicalAddress);
