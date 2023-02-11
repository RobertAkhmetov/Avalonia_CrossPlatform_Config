using Gurux.DLMS.Objects.Enums;

namespace DLMSClient.Net
{
    public class HdlcConfiguration
    {
        public string Obis { get; set; }
        public BaudRate CommunicationSpeed { get; set; }
        public int DeviceAddress { get; set; }
        public int InterCharachterTimeout { get; set; }
        public int InactivityTimeout { get; set; }

    }

}
