using Gurux.DLMS.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonDlmsModels
{
    public class PushSetupSettings
    {
        public string DestionationAddress { get; set; }
        public MessageType MessageType { get; set; }
        public byte NumberOfRetries { get; set; }
        public ushort RepetitionDelay { get; set; }
        public ServiceType ServiceType { get; set; }

    }

}
