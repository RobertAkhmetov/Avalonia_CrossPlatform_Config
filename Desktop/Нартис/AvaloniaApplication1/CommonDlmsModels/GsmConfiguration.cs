namespace DLMSClient.Net
{
    public class GsmConfiguration
    {
        public ModemMode Mode { get; set; }
        public string APN { get; set; }
        public string ApnUser { get; set; }
        public string ApnPassword { get; set; }
        public string SmsCenter { get; set; }
        public string ClientIP { get; set; }
        public ushort Port { get; set; }
        public int InterCharachterTimeout { get; set; }
        public int InterFrameTimeout { get; set; }
    }

}
