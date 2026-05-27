using System;
using System.Net;
using System.Net.Sockets;

namespace IpCalculatorLinux.Models;

/// <summary>
/// Klasa odpowiedzialna za obliczenia sieciowe (Kalkulator IP).
/// </summary>
public static class IpCalculatorLogic
{
    /// <summary>
    /// Oblicza wszystkie dane podsieci na podstawie adresu IP i maski (CIDR).
    /// </summary>
    /// <param name="ipAddress">Adres IP w formacie string (np. "192.168.1.10")</param>
    /// <param name="cidr">Maska w formacie CIDR (np. 24)</param>
    /// <returns>Obiekt z wynikami obliczeń lub null jeśli dane są błędne</returns>
    public static NetworkInfo? Calculate(string ipAddress, int cidr)
    {
        if (cidr < 0 || cidr > 32) return null;
        if (!IPAddress.TryParse(ipAddress, out var parsedIp)) return null;
        if (parsedIp.AddressFamily != AddressFamily.InterNetwork) return null; // Tylko IPv4 na razie

        byte[] ipBytes = parsedIp.GetAddressBytes();
        
        // Oblicz maskę podsieci (Subnet Mask)
        uint mask = ~(uint.MaxValue >> cidr);
        byte[] maskBytes = BitConverter.GetBytes(mask);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(maskBytes);
        }
        var subnetMask = new IPAddress(maskBytes);

        // Oblicz adres sieci (Network Address)
        byte[] networkBytes = new byte[4];
        for (int i = 0; i < 4; i++)
        {
            networkBytes[i] = (byte)(ipBytes[i] & maskBytes[i]);
        }
        var networkAddress = new IPAddress(networkBytes);

        // Oblicz adres rozgłoszeniowy (Broadcast Address)
        byte[] broadcastBytes = new byte[4];
        for (int i = 0; i < 4; i++)
        {
            broadcastBytes[i] = (byte)(networkBytes[i] | ~maskBytes[i]);
        }
        var broadcastAddress = new IPAddress(broadcastBytes);

        // Liczba hostów
        uint totalHosts = 0;
        uint usableHosts = 0;
        
        if (cidr < 31)
        {
            totalHosts = (uint)Math.Pow(2, 32 - cidr);
            usableHosts = totalHosts - 2;
        }
        else if (cidr == 31)
        {
            totalHosts = 2;
            usableHosts = 2; // w RFC 3021 /31 ma 2 użyteczne hosty
        }
        else if (cidr == 32)
        {
            totalHosts = 1;
            usableHosts = 1;
        }

        // Pierwszy i ostatni host
        string firstHost = "N/A";
        string lastHost = "N/A";
        
        if (cidr < 31)
        {
            byte[] firstHostBytes = new byte[4];
            Array.Copy(networkBytes, firstHostBytes, 4);
            firstHostBytes[3]++;
            firstHost = new IPAddress(firstHostBytes).ToString();

            byte[] lastHostBytes = new byte[4];
            Array.Copy(broadcastBytes, lastHostBytes, 4);
            lastHostBytes[3]--;
            lastHost = new IPAddress(lastHostBytes).ToString();
        }
        else if (cidr == 31)
        {
            firstHost = networkAddress.ToString();
            lastHost = broadcastAddress.ToString();
        }
        else if (cidr == 32)
        {
            firstHost = networkAddress.ToString();
            lastHost = networkAddress.ToString();
        }

        return new NetworkInfo
        {
            IpAddress = parsedIp.ToString(),
            SubnetMask = subnetMask.ToString(),
            NetworkAddress = networkAddress.ToString(),
            BroadcastAddress = broadcastAddress.ToString(),
            FirstHost = firstHost,
            LastHost = lastHost,
            TotalHosts = totalHosts,
            UsableHosts = usableHosts,
            Cidr = cidr,
            IpClass = GetIpClass(ipBytes[0])
        };
    }

    private static string GetIpClass(byte firstOctet)
    {
        if (firstOctet >= 1 && firstOctet <= 126) return "A";
        if (firstOctet == 127) return "Loopback";
        if (firstOctet >= 128 && firstOctet <= 191) return "B";
        if (firstOctet >= 192 && firstOctet <= 223) return "C";
        if (firstOctet >= 224 && firstOctet <= 239) return "D (Multicast)";
        if (firstOctet >= 240 && firstOctet <= 255) return "E (Eksperymentalna)";
        return "Nieznana";
    }
}

/// <summary>
/// Model przechowujący wyniki obliczeń.
/// </summary>
public class NetworkInfo
{
    public string IpAddress { get; set; } = "";
    public string SubnetMask { get; set; } = "";
    public string NetworkAddress { get; set; } = "";
    public string BroadcastAddress { get; set; } = "";
    public string FirstHost { get; set; } = "";
    public string LastHost { get; set; } = "";
    public uint TotalHosts { get; set; }
    public uint UsableHosts { get; set; }
    public int Cidr { get; set; }
    public string IpClass { get; set; } = "";
}
