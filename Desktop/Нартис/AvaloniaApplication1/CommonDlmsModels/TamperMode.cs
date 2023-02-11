using System;

namespace DLMSClient.Net.MeterVariants
{
    [Flags]
    public enum TamperMode : uint
    {
        MagneticField = 0x1,
        ElectricCover = 0x2,
        DeviceCover = 0x4,
        ActivePower = 0x8,
        WrongPhaseOrder = 0x100,
        Programming = 0x10,
        VoltageJournal = 0x20,
        VoltageDeviationLow = 0x40,
        VoltageDeviationHigh = 0x80,
    }


}
