using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication1.DLMSObjects
{
    public class MeterObject
    {
        public string ObisCode { get; set; }
        public ObjectType Type { get; set; }
        public int AttributeIndex { get; set; }

        public MeterObject(GXDLMSObject obj, int index)
        {
            ObisCode = obj.LogicalName;
            Type = obj.ObjectType;
            AttributeIndex = index;
        }

        public MeterObject(string obisCode, ObjectType type, int index)
        {
            ObisCode = obisCode;
            Type = type;
            AttributeIndex = index;
        }

        public static MeterObject NetworkFrequency()
        {
            return new MeterObject("1.0.14.7.0.255", ObjectType.Register, 2);
        }

        public static MeterObject SerialNumber()
        {
            return new MeterObject("0.0.96.1.0.255", ObjectType.Data, 2);
        }

        public static MeterObject MeterType()
        {
            return new MeterObject("0.0.96.1.1.255", ObjectType.Data, 2);
        }

        public static MeterObject MetrologicalSWVersion()
        {
            return new MeterObject("0.0.96.1.2.255", ObjectType.Data, 2);
        }

        public static MeterObject CommunicationSWVersion()
        {
            return new MeterObject("0.0.96.1.3.255", ObjectType.Data, 2);
        }

        public static MeterObject ProductionDate()
        {
            return new MeterObject("0.0.96.1.4.255", ObjectType.Data, 2);
        }

        public static MeterObject SpecialDays()
        {
            return new MeterObject("0.0.11.0.0.255", ObjectType.SpecialDaysTable, 2);
        }

        public static MeterObject Calendar()
        {
            return new MeterObject("0.0.13.0.0.255", ObjectType.ActivityCalendar, 3);
        }

        public static MeterObject Test()
        {
            return new MeterObject("0.0.10.0.100.255", ObjectType.ScriptTable, 2);
        }
    }
}
