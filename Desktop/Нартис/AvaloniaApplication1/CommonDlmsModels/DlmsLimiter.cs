using System.Collections.Generic;

namespace DLMSClient.Net
{
    public class DlmsLimiter
    {
        public string Title { get; set; }
        public string Obis { get; set; }
        public bool IsActive { get; set; }
        public bool CanUserChangeValue { get; set; } = true;
        public long TheresholdValue { get; set; }
        public uint MinimumDurationOfExcess { get; set; }

        public bool MinimumDurationOfReturnAvailable { get; set; } = true;

        public uint MinimumDurationOfReturn { get; set; }
        public double Scaler { get; set; }
        public double CustomScaler { get; set; } = 1;

        public double Value { get => Scaler * CustomScaler * TheresholdValue; }

        public static List<DlmsLimiter> GetAllStemLimiters()
        {
            return new List<DlmsLimiter>
                {
                    new DlmsLimiter
                    {
                        Obis = "0.0.17.0.0.255",
                        Title = "Ограничитель макс. мощности, кВт",
                        CustomScaler = 0.001
                    },
                    new DlmsLimiter {Obis = "0.0.17.0.1.255", Title = "Ограничитель макс. тока, А"},
                    new DlmsLimiter {Obis = "0.0.17.0.2.255", Title = "Ограничитель макс. напряжения, В"},
                    new DlmsLimiter
                    {
                        Obis = "0.0.17.0.3.255",
                        Title = "Ограничитель магнитного воздействия, мТл",
                        CanUserChangeValue = false
                    },
                    new DlmsLimiter
                    {
                        Obis = "0.0.17.0.4.255",
                        Title = "Ограничитель по разбалансу токов",
                        CanUserChangeValue = false
                    },
                    new DlmsLimiter
                    {
                        Obis = "0.0.17.0.6.255",
                        Title = "Ограничитель по возникновению события наличия тока в  отсутствии напряжения",
                        CanUserChangeValue = false
                    },
                };
        }

        public static List<DlmsLimiter> GetAllNartisLimiters()
        {
            return new List<DlmsLimiter>
                {
                    new DlmsLimiter
                    {
                        Obis = "0.0.17.0.0.255",
                        Title = "Ограничитель макс. мощности, кВт",
                        CustomScaler = 0.001
                    },
                    new DlmsLimiter {Obis = "0.0.17.0.1.255", Title = "Ограничитель макс. тока, А"},
                    new DlmsLimiter {Obis = "0.0.17.0.2.255", Title = "Ограничитель макс. напряжения, В"},
                    new DlmsLimiter
                    {
                        Obis = "0.0.17.0.3.255",
                        Title = "Ограничитель магнитного воздействия, мТл",
                        CanUserChangeValue = false
                    },
                    new DlmsLimiter
                    {
                        Obis = "0.0.17.0.4.255",
                        Title = "Ограничитель по разбалансу токов",
                        CanUserChangeValue = false
                    },
                    new DlmsLimiter
                    {
                        Obis = "0.0.17.0.6.255",
                        Title = "Ограничитель по возникновению события наличия тока в  отсутствии напряжения",
                        CanUserChangeValue = false
                    },
                    new DlmsLimiter
                    {
                        Obis = "0.0.17.0.7.255",
                        Title = "Ограничитель по активной энергии за расчетный период, кВт*ч",
                        CustomScaler = 0.001
                    },
                     new DlmsLimiter
                    {
                        Obis = "0.0.17.0.8.255",
                        Title = "Ограничитель по открытию корпуса прибора",
                        CanUserChangeValue = false
                    },
                    new DlmsLimiter
                    {
                        Obis = "0.0.17.0.9.255",
                        Title = "Ограничитель по открытию крышки клеммников",
                        CanUserChangeValue = false
                    },
                };
        }
    }

}
