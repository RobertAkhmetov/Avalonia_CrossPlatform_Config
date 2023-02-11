using System.Collections.Generic;

namespace DLMSClient.Net
{
    public class IndicationDataModel
    {
        public uint SwitchTime { get; set; }
        public List<IndicationModeDataModel> Models { get; set; }

        public static string[] ObisModes { get; set; } = new string[]
        {
            "0.128.2.1.0.255",
            "0.128.2.2.0.255",
            "0.128.2.3.0.255",
            "0.128.2.4.0.255",
            "0.128.2.5.0.255"
        };
    }
}
