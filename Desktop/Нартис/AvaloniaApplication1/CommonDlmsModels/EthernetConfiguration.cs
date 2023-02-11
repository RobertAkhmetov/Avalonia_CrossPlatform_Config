namespace DLMSClient.Net
{

    public class EthernetConfiguration
    {
        public string Mac { get; set; }
        public ushort PortOfIncomingConnections { get; set; }
        public string IpAdress { get; set; }
        public string IpGatewayAdress { get; set; }
        public string SubnetMask { get; set; }
        public int InterCharachterTimeout { get; set; }
        public int InterFrameTimeout { get; set; }

        public ushort PortOfRemoteServer { get; set; }
        public string IpAdressOfRemoteServer { get; set; }
        public int TcpInactivityTimeoutOfRemote { get; set; }

        public int TcpInactivityTimeoutServerMode { get; set; }


        public EthernetMode Mode { get; set; }
    }

}
