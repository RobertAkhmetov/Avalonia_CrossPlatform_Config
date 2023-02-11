/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Gurux.Common;
using Gurux.DLMS;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;
using System.Xml.Serialization;
using System.IO;


namespace AvaloniaApplication1.DLMSObjects
{
    [XmlType("data-value")]
    public class DataValue
    {
        [XmlArray("array")]
        [XmlArrayItem("double-long-unsigned")]
        public uint[] Array { get; set; }
    }

    [XmlType("notification-body")]
    public class NotificationBody
    {
        [XmlElement("data-value")]
        public DataValue Value { get; set; }
    }

    [XmlType("data-notification")]
    public class Notification
    {
        [XmlElement("long-invoke-id-and-priority")]
        public uint IdAndPriority { get; set; }

        [XmlElement("date-time")]
        public DateTime Date { get; set; }

        [XmlElement("notification-body")]
        public NotificationBody Body { get; set; }
    }

    [XmlType("xDLMS-APDU")]
    public class ApduXml
    {
        [XmlElement("data-notification")]
        public Notification Notification { get; set; }

        public static ApduXml ParseApduXml(string value)
        {
            var serializator = new XmlSerializer(typeof(ApduXml));
            ApduXml result = null;
            try
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(value)))
                {
                    result = (ApduXml)serializator.Deserialize(stream);
                }
            }
            catch
            {

            }

            return result;
        }
    }
    public class DlmsNotification
    {
        public uint Serial { get; set; }
        public DateTime Time { get; set; }
        public uint Value { get; set; }

        public DlmsNotification(ApduXml apdu)
        {
            Time = apdu.Notification.Date;
            Serial = apdu.Notification.Body.Value.Array[0];
            Value = apdu.Notification.Body.Value.Array[1];
        }

        public DlmsNotification(GXReplyData data)
        {
            Time = data.Time;
            Serial = (uint)((object[])data.Value)[0];
            Value = (uint)((object[])data.Value)[1];
        }
    }
    public interface ILogger
    {
        void LogRecieve(byte[] parametersReply);
        void WriteToLog(string s);
        void LogSend(byte[] data);
    }
    public interface ILinkInterface
    {
        void SetLogger(ILogger logger);
        bool IsOpen { get; }
        void Send(byte[] data);
        bool Open();
        void Close();
        bool Recieve(ReceiveParameters<byte[]> p);
        string LinkDescription { get; }

        void InitInterface();
        //event TCPInterface.LinkStateHandler Link;
    }
    public enum AccessType
    {
        PublicAccess,
        ReaderAccess,
        ConfiguratorAccess
    };
    public class ClientDLMS : GXDLMSClient
    {
        public ILinkInterface linkInterface { get; set; }
        public int Timeout { get; set; } = 2000;
        public AccessType CurrentAccessType { get; set; }

        public ReceiveParameters<byte[]> DefaultRecieveParameters()
        {
            return new ReceiveParameters<byte[]>()
            {
                AllData = true,
                Eop = (byte)0x7E,
                Count = 1,
                WaitTime = Timeout,
            };
        }

        private ILogger _logger;
        private DlmsNotification _lastNotification;

        public ClientDLMS(ILinkInterface inteface, AccessType access = AccessType.ReaderAccess, string password = "111",
            int serverAddr = 144, int timeout = 10000, ILogger loger = null) : base()
        {

            _logger = loger ?? new Logger();
            CurrentAccessType = access;
            inteface?.SetLogger(_logger);
            if (inteface is ComInterface)
            {
                ((ComInterface)inteface).Timeout = timeout;
            }
            Settings.Limits.MaxInfoRX = 256;
            Settings.Limits.MaxInfoTX = 256;

            Timeout = timeout;
            UseLogicalNameReferencing = true;

            ServerAddress = serverAddr;
            linkInterface = inteface;

            switch (access)
            {
                case AccessType.ReaderAccess:
                    Authentication = Authentication.Low;
                    Password = Encoding.ASCII.GetBytes(password);
                    ClientAddress = 0x20;
                    break;

                case AccessType.PublicAccess:
                    Authentication = Authentication.None;
                    ClientAddress = 0x10;
                    break;

                case AccessType.ConfiguratorAccess:
                    Authentication = Authentication.High;
                    Password = Encoding.ASCII.GetBytes(password);
                    ClientAddress = 0x30;
                    break;
            }
        }

        public void ReadDLMSPacket(byte[] data, GXReplyData reply, System.Threading.CancellationToken token)
        {
            var notify = new GXReplyData();
            var reciveParams = DefaultRecieveParameters();
            if (data == null)
            {
                throw Exceptions.SendNullError();
            }
            bool succeeded = false;
            Send(data);

            Debug.WriteLine("RP: " + reciveParams.Count);

            succeeded = linkInterface.Recieve(reciveParams);
            var replyBuffer = new GXByteBuffer(reciveParams.Reply);
            var counter = 0;
            var allNotify = new List<byte>();
            string notifyString = null;
            do
            {
                counter++;
                Debug.WriteLine($"Counter {counter}");
                var result = GetData(replyBuffer, reply, notify);

                if (!result)
                {
                    if (!reply.IsComplete || (notify.FrameId == 0x13 && !notify.IsComplete))
                    {
                        Debug.WriteLine("Frame not complete");
                        if (!linkInterface.Recieve(reciveParams))
                        {
                            string err = "Failed to receive reply from the device in given time.";
                            throw new Exception(err);
                        }

                        replyBuffer.Capacity = reciveParams.Reply.Count();
                        replyBuffer.Size = reciveParams.Reply.Count();
                        replyBuffer.Data = reciveParams.Reply;


                    }
                    else
                    {
                        throw new Exception();
                    }

                }

                Debug.WriteLine($"Notify Frame Id = {notify.FrameId}");
                Debug.WriteLine($"Frame Id = {reply.FrameId}");

                if (notify.FrameId == 0x13 && notify.IsComplete)
                {
                    notify.FrameId = 0x0;
                    notifyString = notify.Data.GetStringUtf8(0, notify.Data.Size);
                    allNotify = notify.Data.Data.ToList();
                    Debug.WriteLine("LastNotify " + ByteArrayToString(notify.Data.Data));

                    if (replyBuffer.Position == replyBuffer.Size)
                    {
                        if (!linkInterface.Recieve(reciveParams))
                        {
                            string err = "Failed to receive reply from the device in given time.";
                            throw new Exception(err);
                        }

                        replyBuffer.Capacity = reciveParams.Reply.Count();
                        replyBuffer.Size = reciveParams.Reply.Count();
                        replyBuffer.Data = reciveParams.Reply;
                    }
                }

            } while (replyBuffer.Position != replyBuffer.Size);

            if (allNotify.Count() > 0)
            {
                Debug.WriteLine("Notify Recieve And Handle Complete");
                var xmlNotification = ParseXmlNotification(notifyString);
                if (xmlNotification != null)
                {
                    LastNotification = xmlNotification;
                }
                else
                {
                    var pduNotification = ParsePduNotification(notify);
                    if (pduNotification != null)
                        LastNotification = pduNotification;
                }

            }
        }

        public event Action<DlmsNotification> NotificationChanged;
        public DlmsNotification LastNotification
        {
            get => _lastNotification; set
            {
                _lastNotification = value;
                NotificationChanged?.Invoke(value);
            }
        }

        private DlmsNotification ParseXmlNotification(string xml)
        {
            var parsedXml = ApduXml.ParseApduXml(xml);
            if (parsedXml == null)
                return null;

            return new DlmsNotification(parsedXml);
        }

        private DlmsNotification ParsePduNotification(GXReplyData data)
        {
            DlmsNotification result = null;
            try
            {
                result = new DlmsNotification(data);
            }
            catch
            {

            }
            return result;
        }

        public static string ByteArrayToString(byte[] ba)
        {
            if (ba == null)
            {
                return "";
            }
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2} ", b);
            return hex.ToString();
        }


        public GXDLMSObjectCollection GetObjects(CancellationToken token)
        {
            var requset = Read("0.0.40.0.0.255", ObjectType.AssociationLogicalName, 2);
            GXReplyData reply = BaseRead(requset, token);
            ParsedObjects = ParseObjects(reply.Data, false);
            ObjectsReply = reply.Data.Data;
            return ParsedObjects;
        }

        public byte[] ObjectsReply { get; set; }

        public GXDLMSObjectCollection ParsedObjects { get; set; }

        public GXReplyData KeepAlive(CancellationToken token)
        {
            GXReplyData reply = new GXReplyData();
            ReadDLMSPacket(GetKeepAlive(), reply, token);
            return reply;
        }
        public Response WriteData<T>(string obis, T value, CancellationToken token)
        {
            var response = new Response();
            try
            {
                var meterObject = Objects.FindByLN(ObjectType.Data, obis) as GXDLMSData;
                meterObject.Value = value;
                WriteObject(meterObject, 2, token);
            }
            catch
            {
                response.IsOk = false;
            }

            return response;
        }

        public Response WriteDataAsUByte(string obis, uint value, CancellationToken token)
        {
            var response = new Response();
            try
            {
                var meterObject = Objects.FindByLN(ObjectType.Data, obis) as GXDLMSData;
                meterObject.SetDataType(2, DataType.UInt8);
                meterObject.Value = value;
                WriteObject(meterObject, 2, token);
            }
            catch
            {
                response.IsOk = false;
            }

            return response;
        }

        public Response<ulong> ReadDemandRegistrAsULong(string obis, CancellationToken token, int atr = 8)
        {
            var response = new Response<ulong>();
            try
            {
                var newValue = Convert.ToUInt64(GetUpdatedValue(new MeterObject(obis, ObjectType.DemandRegister, atr), token));
                response.Value = newValue;
            }
            catch
            {
                response.IsOk = false;
            }

            return response;
        }


        public Response<T> ReadData<T>(string obis, System.Threading.CancellationToken token)
        {
            var response = new Response<T>();

            //var newValue = GetUpdatedValue(new MeterObject(obis, ObjectType.Data, 2), token);


            //if (newValue is T)
            //    response.Value = (T)newValue;
            //else
            //{
            //    var strT = typeof(System.String);
            //    var realValue = newValue as Response<T>;
            //    var respT = realValue.Value.GetType();
            //    if (newValue is byte[] & strT.Equals(respT))//response.GetType is StringBuilder)
            //    {
            //        Response<string> response1 = new Response<string>() { Value = Encoding.UTF8.GetString((byte[])newValue) };
            //        var newresponse = response1;
            //        return newresponse as Response<T>;
            //    }
            //}            
            //    response.AddError($"Неверный тип данных объекта {obis}: Ожидаемый {typeof(T)} - Полученный {newValue.GetType()}");


            var newValue = GetUpdatedValue(new MeterObject(obis, ObjectType.Data, 2), token);
            try
            {
                if (obis == "0.0.96.1.0.255") //серийный номер
                {
                    var responseByteRes = new byte[12];
                    int i = 0;

                    foreach (byte b in newValue as byte[])
                    {
                        responseByteRes[i] = (byte)(b - 0x30);
                        i++;
                    }

                    var responseString = String.Join("", responseByteRes);
                    var response1 = new Response<string>() { Value = responseString };//{ Value = (uint)BitConverter.ToInt32(responseByteRes, 0) };
                    return response1 as Response<T>;
                }

                if (obis == "0.0.96.1.1.255") //тип счетчика
                {
                    var response1 = new Response<uint>() { Value = ((byte[])newValue)[0] };//(uint)BitConverter.ToInt32((byte[])newValue, 0) };
                    return response1 as Response<T>;
                }

                if (obis == "0.0.96.1.4.255") //дата выпуска счетчика
                {
                    string respVal = $"{(uint)BitConverter.ToInt32((byte[])newValue, 0)}";
                    switch ((uint)BitConverter.ToInt32((byte[])newValue, 0))
                    {
                        case (0xffffffff): respVal = "31.12.3999 23:59:59"; break;
                        default: respVal = "01.01.0001 00:00:01"; break;
                    }
                    var response1 = new Response<string>() { Value = respVal };
                    return response1 as Response<T>;
                }

                if (obis == "0.0.96.1.3.255")
                {

                }



                //else if(typeof(T) == typeof(uint) && newValue is byte[])
                //{

                //}  

                if (newValue is T)
                    response.Value = (T)newValue;
                else
                {
                    response.AddError($"Неверный тип данных объекта {obis}: Ожидаемый {typeof(T)} - Полученный {newValue.GetType()}");
                }

            }
            catch { }

            return response;


        }

        public Response<string> ReadDataOctetString(string obis, CancellationToken token)
        {
            var response = new Response<string>();
            try
            {
                var newValue = GetUpdatedValue(new MeterObject(obis, ObjectType.Data, 2), token);

                var crcObj = Objects.FindByLN(ObjectType.Data, obis);
                crcObj.SetUIDataType(2, DataType.OctetString);
                var valueFromDevice = GetAtrData(crcObj, 2, token) as string;
                response.Value = valueFromDevice;
            }
            catch
            {
                response.IsOk = false;
            }

            return response;
        }

        public Response<string> ReadDataAsOctetString(string obis, CancellationToken token)
        {
            var response = new Response<string>();
            try
            {

                var points = Objects.FindByLN(ObjectType.Data, obis) as GXDLMSData;
                points.SetDataType(2, DataType.OctetString);
                var value = GetUpdatedValue(new MeterObject(points, 2), token) as string;
                response.Value = value;

            }
            catch
            {
                response.IsOk = false;
            }

            return response;
        }

        public Response<GXDateTime> ReadClock(string obis, CancellationToken token)
        {
            var response = new Response<GXDateTime>();
            try
            {
                var newValue = GetUpdatedValue(new MeterObject(obis, ObjectType.Clock, 2), token);

                if (newValue is GXDateTime)
                    response.Value = (GXDateTime)newValue;
                else
                    response.AddError($"Неверный тип данных объекта {obis}: Ожидаемый {typeof(GXDateTime)} - Полученный {newValue.GetType()}");

            }
            catch
            {
                response.IsOk = false;
            }

            return response;
        }

        public object GetUpdatedValue(MeterObject obj, CancellationToken token)
        {
            var dlmsObject = Objects.FindByLN(obj.Type, obj.ObisCode);
            if (dlmsObject == null)
            {
                throw new CanNotFindObjectWithOBIS(obj.ObisCode);
            }
            return GetAtrData(dlmsObject, obj.AttributeIndex, token);
        }

        public Response<T> GetUpdatedValue<T>(MeterObject obj, CancellationToken token)
        {
            var response = new Response<T>();
            try
            {
                response.Value = (T)GetUpdatedValue(obj, token);
            }
            catch (Exception e)
            {
                response.AddError(e.Message);
            }

            return response;
        }

        public Response WriteDlmsObject(GXDLMSObject item, int[] attributeIndex, CancellationToken token)
        {
            var response = new Response();
            foreach (var atribute in attributeIndex)
            {
                response.AddErrorOfComponent(WriteDlmsObject(item, atribute, token));
                if (!response.IsOk)
                    break;
            }

            return response;
        }

        public Response ReadDlmsObject(GXDLMSObject item, int[] attributeIndex, CancellationToken token)
        {
            var response = new Response();
            foreach (var atribute in attributeIndex)
            {
                response.AddErrorOfComponent(ReadDlmsObject(item, atribute, token));
                if (!response.IsOk)
                    break;
            }

            return response;
        }

        public Response ReadDlmsObject(GXDLMSObject item, int attributeIndex, CancellationToken token)
        {
            var response = new Response();
            try
            {
                GetUpdatedValue(new MeterObject(item, attributeIndex), token);
            }
            catch (Exception e)
            {
                response.AddError(e.Message);
            }
            return response;
        }

        public Response WriteDlmsObject(GXDLMSObject item, int attributeIndex, CancellationToken token)
        {
            var data = Write(item, attributeIndex);
            var response = new Response();
            try
            {
                BaseRead(data, token);
            }
            catch (Exception e)
            {
                response.AddError(e.Message);
            }
            return response;
        }

        public GXReplyData WriteObject(GXDLMSObject item, int attributeIndex, CancellationToken token)
        {
            var data = Write(item, attributeIndex);
            GXReplyData reply = BaseRead(data, token);
            return reply;
        }

        public GXReplyData WriteObject(GXDLMSObject item, int attributeIndex, DataType type, object value,
            ObjectType objType, CancellationToken token)
        {
            var data = Write(item, value, type, objType, attributeIndex);
            GXReplyData reply = BaseRead(data, token);

            return reply;
        }


        public object ReadProfileByCount(GXDLMSProfileGeneric profile, int index, int count, CancellationToken token)
        {
            var response = ReadRowsByEntry(profile, index, count);
            GXReplyData reply = BaseRead(response, token);

            return UpdateValue(profile, 2, reply.Value);
        }

        public object ReadProfileByRange(GXDLMSProfileGeneric profile, GXDateTime start, GXDateTime end,
            CancellationToken token)
        {
            var response = ReadRowsByRange(profile, start, end);

            GXReplyData reply = BaseRead(response, token);

            return UpdateValue(profile, 2, reply.Value);
        }

        public object ReadCollumns(GXDLMSProfileGeneric it, CancellationToken token)
        {
            var requsets = Read(it, 3);
            GXReplyData replys = BaseRead(requsets, token);
            return UpdateValue(it, 3, replys.Value);
        }

        private GXReplyData BaseRead(byte[][] dataArray, CancellationToken token)
        {
            byte[] datas;
            if (token.IsCancellationRequested)
            {
                return new GXReplyData();
            }

            bool canceledHandled = false;

            GXReplyData repl = new GXReplyData();

            foreach (byte[] req in dataArray)
            {
                ReadDLMSPacket(req, repl, token);
                while (repl.IsMoreData)
                {
                    if (token.IsCancellationRequested && !canceledHandled)
                    {
                        _logger.WriteToLog("Cancel");
                        if (repl.MoreData == RequestTypes.DataBlock)
                        {
                            canceledHandled = true;
                            Settings.IncreaseBlockIndex();
                            throw new Exception();
                        }

                    }
                    datas = ReceiverReady(repl.MoreData);
                    ReadDLMSPacket(datas, repl, token);
                }
            }

            HandleReply(repl);

            return repl;
        }

        private void HandleReply(GXReplyData data)
        {
            if (data.Error != 0)
            {
                throw new ReplyException() { ErrorCode = (ErrorCode)data.Error, ErrorDescription = GXDLMS.GetDescription((ErrorCode)data.Error) };
            }
        }

        public object ReadProfile(GXDLMSProfileGeneric it, CancellationToken token)
        {
            var request = Read(it, 2);
            var reply = BaseRead(request, token);
            return UpdateValue(it, 2, reply.Value);
        }

        public Response WriteDemandRegistrPeriod(string obis, ulong value, CancellationToken token)
        {
            var response = new Response();
            try
            {
                var reg = (GXDLMSDemandRegister)Objects.FindByLN(ObjectType.DemandRegister, obis);
                reg.Period = value;
                WriteObject(reg, 8, token);
            }
            catch (Exception e)
            {
                response.AddError(e.Message);
            }

            return response;
        }

        public Response WriteRegister<T>(string obis, T value, CancellationToken token)
        {
            var response = new Response();
            try
            {
                var reg = (GXDLMSRegister)Objects.FindByLN(ObjectType.Register, obis);
                reg.Value = value;
                WriteObject(reg, 2, token);
            }
            catch (Exception e)
            {
                response.AddError(e.Message);
            }

            return response;
        }


        public Response WriteRegisterUInt(string obis, double value, CancellationToken token)
        {
            var response = new Response();
            try
            {
                var reg = (GXDLMSRegister)Objects.FindByLN(ObjectType.Register, obis);
                GetUpdatedValue(new MeterObject(reg, 3), token);
                reg.Value = (uint)(value / reg.Scaler);
                WriteObject(reg, 2, token);
            }
            catch (Exception e)
            {
                response.AddError(e.Message);
            }

            return response;
        }

        public Response WriteRegisterInt(string obis, double value, CancellationToken token)
        {
            var response = new Response();
            try
            {
                var reg = (GXDLMSRegister)Objects.FindByLN(ObjectType.Register, obis);
                GetUpdatedValue(new MeterObject(reg, 3), token);
                reg.Value = (int)(value / reg.Scaler);
                WriteObject(reg, 2, token);
            }
            catch (Exception e)
            {
                response.AddError(e.Message);
            }

            return response;
        }


        public Response<T> ReadRegister<T>(string obis, CancellationToken token)
        {
            var response = new Response<T>();
            try
            {
                var reg = (GXDLMSRegister)Objects.FindByLN(ObjectType.Register, obis);
                GetUpdatedValue(new MeterObject(reg, 3), token);
                GetUpdatedValue(new MeterObject(reg, 2), token);

                if (reg.Value is T)
                    response.Value = (T)reg.Value;
                else
                    response.AddError($"Неверный тип данных объекта {obis}: Ожидаемый {typeof(T)} - Полученный {reg.Value.GetType()}");

            }
            catch (Exception e)
            {
                response.AddError(e.Message);
            }

            return response;
        }

        public Response<DlmsArchive> ReadArchive(string profileObis, string scalerObis, CancellationToken token)
        {
            var response = new Response<DlmsArchive>();
            try
            {
                var profile = (GXDLMSProfileGeneric)Objects.FindByLN(ObjectType.ProfileGeneric, profileObis);

                var profileCaptureObjects = ReadCollumns(profile, token) as List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>;
                var profileValues = (ReadProfile(profile, token) as object[][]);

                var scale = (GXDLMSProfileGeneric)Objects.FindByLN(ObjectType.ProfileGeneric, scalerObis);
                ReadCollumns(scale, token);
                var scaleBuffer = (ReadProfile(scale, token) as object[][]).Single();
                var scalerCaptureObjects =
                    ReadCollumns(scale, token) as List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>;

                var archive = new DlmsArchive();

                if (profileCaptureObjects != null)
                {
                    foreach (var profileItem in profileValues)
                    {
                        var archiveItem = new ArchiveItem();

                        foreach (var profileCaptured in profileCaptureObjects)
                        {

                            var currentObjectObis = profileCaptured.Key.LogicalName;
                            var index = profileCaptureObjects.FindIndex(m => m.Key.LogicalName.Equals(currentObjectObis));
                            object scalerValue = null;
                            if (scalerCaptureObjects?.Count(m => m.Key.LogicalName.Equals(currentObjectObis)) != 0)
                            {
                                var sclerIndex =
                                    scalerCaptureObjects.FindIndex(m => m.Key.LogicalName.Equals(currentObjectObis));
                                scalerValue = (scaleBuffer[sclerIndex] as object[])?.First();
                            }
                            var currentProfileItem = new ProfileItem(currentObjectObis, profileItem[index], scalerValue);

                            if (currentObjectObis == "0.0.1.0.0.255")
                            {
                                archiveItem.Timestamp = currentProfileItem.Value as GXDateTime;
                            }
                            else if (currentObjectObis == "0.0.96.8.0.255")
                            {
                                archiveItem.WorkTime = (uint)currentProfileItem.Value;
                            }
                            else
                            {
                                archiveItem.Items.Add(currentProfileItem);
                            }

                        }

                        archive.ArchiveItems.Add(archiveItem);
                    }

                }


                response.Value = archive;
            }
            catch (Exception ex)
            {
                response.AddError(ex.ToString());
            }


            return response;
        }

        public Response<List<DlmsProfile>> ReadProfileWithMultipleValues(string profileObis, string scalerObis, CancellationToken token)
        {
            var response = new Response<List<DlmsProfile>>() { Value = new List<DlmsProfile>() };
            try
            {
                var profile = (GXDLMSProfileGeneric)Objects.FindByLN(ObjectType.ProfileGeneric, profileObis);

                var profileCaptureObjects = ReadCollumns(profile, token) as List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>;
                var profileValuesArray = (ReadProfile(profile, token) as object[][]);

                var scale = (GXDLMSProfileGeneric)Objects.FindByLN(ObjectType.ProfileGeneric, scalerObis);
                ReadCollumns(scale, token);
                var scaleBuffer = (ReadProfile(scale, token) as object[][]).Single();
                var scalerCaptureObjects =
                    ReadCollumns(scale, token) as List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>;



                if (profileCaptureObjects != null)
                {
                    foreach (var profileValues in profileValuesArray)
                    {
                        var result = new DlmsProfile(profileObis, scalerObis);

                        foreach (var profileCaptured in profileCaptureObjects)
                        {
                            var currentObjectObis = profileCaptured.Key.LogicalName;
                            var index = profileCaptureObjects.FindIndex(m => m.Key.LogicalName.Equals(currentObjectObis));
                            object scalerValue = null;
                            if (scalerCaptureObjects?.Count(m => m.Key.LogicalName.Equals(currentObjectObis)) != 0)
                            {
                                var sclerIndex =
                                    scalerCaptureObjects.FindIndex(m => m.Key.LogicalName.Equals(currentObjectObis));
                                scalerValue = (scaleBuffer[sclerIndex] as object[])?.First();
                            }

                            result.Items.Add(new ProfileItem(currentObjectObis, profileValues[index], scalerValue));
                        }

                        response.Value.Add(result);

                    }

                }


            }
            catch (Exception ex)
            {
                response.AddError(ex.ToString());
            }


            return response;
        }


        public Response<DlmsProfile> ReadProfile(string profileObis, string scalerObis, CancellationToken token)
        {
            var response = new Response<DlmsProfile>();
            try
            {
                var profile = (GXDLMSProfileGeneric)Objects.FindByLN(ObjectType.ProfileGeneric, profileObis);

                var profileCaptureObjects = ReadCollumns(profile, token) as List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>;
                Debug.WriteLine("-----------------");
                var profileValues = (ReadProfile(profile, token) as object[][]).Single();
                Debug.WriteLine("-----------------");

                var scale = (GXDLMSProfileGeneric)Objects.FindByLN(ObjectType.ProfileGeneric, scalerObis);
                ReadCollumns(scale, token);
                var scaleBuffer = (ReadProfile(scale, token) as object[][]).Single();
                var scalerCaptureObjects =
                    ReadCollumns(scale, token) as List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>;

                var result = new DlmsProfile(profileObis, scalerObis);


                if (profileCaptureObjects != null)
                    foreach (var profileCaptured in profileCaptureObjects)
                    {
                        var currentObjectObis = profileCaptured.Key.LogicalName;
                        var index = profileCaptureObjects.FindIndex(m => m.Key.LogicalName.Equals(currentObjectObis));
                        object scalerValue = null;
                        if (scalerCaptureObjects?.Count(m => m.Key.LogicalName.Equals(currentObjectObis)) != 0)
                        {
                            var sclerIndex =
                                scalerCaptureObjects.FindIndex(m => m.Key.LogicalName.Equals(currentObjectObis));
                            scalerValue = (scaleBuffer[sclerIndex] as object[])?.First();
                        }

                        result.Items.Add(new ProfileItem(currentObjectObis, profileValues[index], scalerValue));
                    }

                response.Value = result;
            }
            catch (Exception ex)
            {
                response.AddError(ex.ToString());
            }


            return response;
        }


        public Response<DlmsProfile> ReadProfile(string profileObis, string scalerObis, DateTime startTime, DateTime endTime, CancellationToken token)
        {
            var response = new Response<DlmsProfile>();
            try
            {
                var profile = (GXDLMSProfileGeneric)Objects.FindByLN(ObjectType.ProfileGeneric, profileObis);

                var profileCaptureObjects = ReadCollumns(profile, token) as List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>;
                var profileValues = (ReadProfileByRange(profile, new GXDateTime(startTime), new GXDateTime(endTime), token) as object[][]).Single();

                var scale = (GXDLMSProfileGeneric)Objects.FindByLN(ObjectType.ProfileGeneric, scalerObis);
                ReadCollumns(scale, token);
                var scaleBuffer = (ReadProfile(scale, token) as object[][]).Single();
                var scalerCaptureObjects =
                    ReadCollumns(scale, token) as List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>;

                var result = new DlmsProfile(profileObis, scalerObis);


                if (profileCaptureObjects != null)
                    foreach (var profileCaptured in profileCaptureObjects)
                    {
                        var currentObjectObis = profileCaptured.Key.LogicalName;
                        var index = profileCaptureObjects.FindIndex(m => m.Key.LogicalName.Equals(currentObjectObis));
                        object scalerValue = null;
                        if (scalerCaptureObjects?.Count(m => m.Key.LogicalName.Equals(currentObjectObis)) != 0)
                        {
                            var sclerIndex =
                                scalerCaptureObjects.FindIndex(m => m.Key.LogicalName.Equals(currentObjectObis));
                            scalerValue = (scaleBuffer[sclerIndex] as object[])?.First();
                        }

                        result.Items.Add(new ProfileItem(currentObjectObis, profileValues[index], scalerValue));
                    }

                response.Value = result;
            }
            catch (Exception ex)
            {
                response.AddError(ex.ToString());
            }


            return response;
        }

        public object GetAtrData(GXDLMSObject obj, int index, CancellationToken token)
        {
            var requset = Read(obj, index);
            GXReplyData reply = BaseRead(requset, token);
            return UpdateValue(obj, index, reply.Value);
        }

        public GXReplyData ExecuteMethod(GXDLMSObject profile, int index, CancellationToken token,
            object dataToMethod = null, DataType type = DataType.None)
        {
            var requset = Method(profile, index, dataToMethod, type);
            GXReplyData reply = BaseRead(requset, token);

            return reply;
        }

        public Response Execute(GXDLMSObject profile, int index, CancellationToken token,
    object dataToMethod = null, DataType type = DataType.None)
        {
            var response = new Response();
            try
            {
                ExecuteMethod(profile, index, token, dataToMethod, type);
            }
            catch (Exception e)
            {
                response.AddError(e.Message);
            }
            return response;
        }

        public void ApplicationAssociationRequest(CancellationToken token)
        {
            var reply = new GXReplyData();
            if (Authentication > Authentication.Low)
            {
                foreach (byte[] request in GetApplicationAssociationRequest())
                {
                    ReadDLMSPacket(request, reply, token);
                }
                ParseApplicationAssociationResponse(reply.Data);
            }
        }

        public void SendAARQRequest(System.Threading.CancellationToken token)
        {
            var requests = AARQRequest();
            GXReplyData reply = BaseRead(requests, token);
            ParseAAREResponse(reply.Data);

            if (IsAuthenticationRequired)
            {
                var rr = GetApplicationAssociationRequest();

                GXReplyData newreply = new GXReplyData();
                foreach (var r in rr)
                {
                    ReadDLMSPacket(r, newreply, token);
                }

                ParseApplicationAssociationResponse(newreply.Data);
            }
        }

        public bool InitConnection()
        {
            bool success = false;
            if (!linkInterface.IsOpen)
            {
                Debug.WriteLine("Link init");
                if (linkInterface.Open())
                {
                    try
                    {
                        linkInterface.InitInterface();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                }
            }

            SendSNRMRequest();
            success = UaResponseRead();

            return success;
        }

        public void CloseConnection()
        {
            if (linkInterface == null)
            {
                Debug.WriteLine("Link is empty");
            }
            else
            {
                Debug.WriteLine("Close connection");
                try
                {
                    linkInterface?.Close();

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    throw;
                }

            }
        }

        public void SendSNRMRequest()
        {
            Send(SNRMRequest());
        }

        public bool UaResponseRead()
        {
            var recieveParams = DefaultRecieveParameters();
            var result = linkInterface.Recieve(recieveParams);
            var notify = new GXReplyData();
            GXReplyData replyData = new GXReplyData();
            GetData(recieveParams.Reply, replyData, notify);
            ParseUAResponse(replyData.Data);
            return replyData.Data.Size > 0;
        }

        public void SoftDisconnect()
        {
            //List<string> res = new List<string>();
            //byte[] im = DisconnectRequest();
            //foreach (byte a in im)
            //{
            //    res.Add(a.ToString());
            //}
            //MessageBox.Show(String.Format("{0:X2}",String.Join(",",res.ToArray())));
            ReadDLMSPacket(DisconnectRequest(), new GXReplyData(), new CancellationToken());

        }

        private void Send(byte[] request) // тут конкретно побайтово передаются данные на счетчик
        {
            if (request == null)
            {
                throw Exceptions.SendNullError();
            }

            linkInterface.Send(request);
        }

        public void HardDisconnect()
        {
            ReadDLMSPacket(GXDLMS.GetHdlcFrame(Settings, (byte)Command.DisconnectRequest, null), new GXReplyData(), new CancellationToken());
        }

        public bool IsObjectWithObisExsists(string obis)
        {
            return Objects.Count(ob => ob.LogicalName.Equals(obis)) > 0;
        }
    }
}
*/