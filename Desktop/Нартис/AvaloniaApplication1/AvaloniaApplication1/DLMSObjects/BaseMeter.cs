
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using Gurux.DLMS;
//using Gurux.DLMS.Enums;
//using Gurux.DLMS.Internal;
//using Gurux.DLMS.Objects;
//using Gurux.DLMS.Objects.Enums;
//using Gurux.DLMS.Secure;
//using Newtonsoft.Json;
//using Quartz.Util;
//using System.IO;

//using System.Threading;
//using static System.Environment;

//namespace AvaloniaApplication1.DLMSObjects
//{
//    public class BaseMeter
//    {
//        public ClientDLMS Client { get; set; }
//        public string CurrentSerial { get; set; }
//        public bool IsGatewaySupported { get; protected set; }
//        public bool IsTelemetrySupported { get; protected set; }
//        public bool IsTamperSupported { get; protected set; }
//        public bool IsThersholdsSupported { get; protected set; }
//        public bool IsSecondRsSupported { get; protected set; }
//        public bool IsSinglePhase { get; protected set; } = false;
//        public bool IsCurrentReadingsResetSupported { get; protected set; } = false;
//        public bool IsMaxPowerSupported { get; protected set; } = true;
//        public bool IsDayZonesSupported { get; protected set; } = true;


//        public BaseMeter(ClientDLMS dlmsClient, uint meterType)
//        {
//            IsGatewaySupported = false;
//            IsTelemetrySupported = false;
//            Client = dlmsClient;
//            MeterType = meterType;
//            IsCurrentReadingsResetSupported = false;
//            IsTamperSupported = true;
//            IsSecondRsSupported = false;
//        }

//        public uint MeterType { get; set; }

//        protected string _accountingPointInfoObis = "0.0.96.1.10.255";

//        protected string _amperageTransformObis = "1.0.0.4.2.255";
//        protected string _voltageTransformObis = "1.0.0.4.3.255";

//        protected string _activePowerForPeriodObis = "1.0.1.4.0.255";


//        protected string _serialObis = "0.0.96.1.0.255";
//        protected string _meterTypeObis = "0.0.96.1.1.255";
//        protected string _productionDateObis = "0.0.96.1.4.255";
//        protected string _clockDateObis = "0.0.1.0.0.255";
//        protected string _metrologicalSoftwareObis = "0.0.96.1.2.255";
//        protected string _softwareObis = "0.0.96.1.3.255";
//        protected string _currentTariff = "0.0.96.14.0.255";
//        protected string _transformationCoefficentVoltage = "1.0.0.4.3.255";
//        protected string _transformationCoefficentAmperage = "1.0.0.4.2.255";
//        protected string _crc = "0.0.128.3.1.255";


//        protected string[] _currentValuesProfileAmperage = new[] { "1.0.31.7.0.255", "1.0.51.7.0.255", "1.0.71.7.0.255" };
//        protected string[] _currentValuesProfileVoltage = new[] { "1.0.32.7.0.255", "1.0.52.7.0.255", "1.0.72.7.0.255" };
//        protected string[] _currentValuesProfileActivePower = new[] { "1.0.21.7.0.255", "1.0.41.7.0.255", "1.0.61.7.0.255", "1.0.1.7.0.255" };
//        protected string[] _currentValuesProfileReactivePower = new[] { "1.0.23.7.0.255", "1.0.43.7.0.255", "1.0.63.7.0.255", "1.0.3.7.0.255" };
//        protected string[] _currentValuesProfileFullPower = new[] { "1.0.29.7.0.255", "1.0.49.7.0.255", "1.0.69.7.0.255", "1.0.9.7.0.255" };
//        protected string[] _currentValuesProfilePowerCoefficient = new[] { "1.0.33.7.0.255", "1.0.53.7.0.255", "1.0.73.7.0.255", "1.0.13.7.0.255" };
//        protected string[] _currentValuesProfileInphaseVoltage = new[] { "1.0.12.7.1.255", "1.0.12.7.2.255", "1.0.12.7.3.255" };
//        protected string _currentValuesProfileFrequency = "1.0.14.7.0.255";
//        protected string _temperatureObis = "0.0.96.9.0.255";
//        protected string _zeroUnsimmetryObis = "1.0.136.7.0.255";
//        protected string _reverseUnsimmetryObis = "1.0.135.7.0.255";
//        protected string[] _flicker = new string[] { "1.0.11.7.127.255", "1.0.12.7.127.255", "1.0.15.7.127.255" };
//        protected string[] _inphaseAngels = new string[] { "1.0.81.7.40.255", "1.0.81.7.51.255", "1.0.81.7.62.255" };
//        protected string[] _tanObis = new string[] { "1.0.128.7.0.255", "1.0.129.7.0.255", "1.0.130.7.0.255", "1.0.131.7.0.255" };
//        protected string[] _maxPowerObis = new string[] { "1.0.35.6.0.255", "1.0.55.6.0.255", "1.0.75.6.0.255", "1.0.15.6.0.255" };

//        protected string _currentValuesProfile = "1.0.94.7.0.255";
//        protected string _currentValuesProfileScaler = "1.0.94.7.3.255";

//        protected string _automaticSwitchOnCount = "0.128.4.0.0.255";
//        protected Version _puVersion;
//        protected Version _miVersion;
//        protected string _lineLossesObis = "1.0.88.8.0.255";
//        protected string _transformLossesObis = "1.0.89.8.0.255";

//        public bool IsCountOfAutomaticSwitchOnSupported { get; set; } = true;

//        public static BaseMeter GetInstance(ClientDLMS client, uint meterType)
//        {
//            BaseMeter meter = null;

//            //if (StemMeter.CheckType(meterType))
//            //    meter = new StemMeter(client, meterType);
//            //else if (NartisSinglePhase.CheckType(meterType))
//            //    meter = new NartisSinglePhase(client, meterType);
//            //else
//                meter = new BaseMeter(client, meterType);

//            return meter;
//        }

//        public static BaseMeter TryToConnectAndGetMeter(ClientDLMS client, CancellationToken token)
//        {

//            client.InitConnection();
//            client.SendAARQRequest(token);//эта функция выбрасывает ArgumentException

//            BaseMeter meter = null;

//            if (client.CurrentAccessType == AccessType.PublicAccess)
//            {
//                client.GetObjects(token);
//            }
//            else
//            {
//                client.ParseObjects(new GXByteBuffer(Resources.ObjectsData), false);
//                var meterType = client.ReadData<uint>("0.0.96.1.2.255", token); //я изменил на сподэсовский

//                meter = GetInstance(client, meterType.Value);

//                var versionResponse = meter.GetSoftwareVersion(token);


//                if (versionResponse.IsOk)
//                {
//                    meter.SetSoftwareVersion(versionResponse.Value);
//                    //+MessageBox.Show(versionResponse.Value.PuVersionString);
//                }

//                if (versionResponse.IsOk && meterType.IsOk && IsAvailableLocalAssociationObject(versionResponse.Value.RawValue, meterType.Value.ToString(), client.CurrentAccessType))
//                {
//                    var data = File.ReadAllBytes(CreateAssociationFilePath(versionResponse.Value.RawValue, meterType.Value.ToString(), client.CurrentAccessType));
//                    client.ParseObjects(new GXByteBuffer(data), false);
//                    client.ParsedObjects = client.Objects;
//                }
//                else //по этому пути идет приложение
//                {
//                    client.GetObjects(token);

//                    //+client.ObjectsReply = new byte[] { 0x01, 0x6, 0x3 };
//                    //+versionResponse.Value.RawValue = "0.1.23.4";
//                    File.WriteAllBytes(CreateAssociationFilePath("0.1.23.4", meterType.Value.ToString(), client.CurrentAccessType),
//                                     client.ObjectsReply); //возникает исключение из-за этой функции
//                }

//            }


//            return meter ?? new BaseMeter(client, 0);
//        }

//        public SoftwareVersion MeterVersion { get; private set; }

//        public void SetSoftwareVersion(SoftwareVersion version)
//        {
//            MeterVersion = version;
//        }

//        public Response RestMaxPower(CancellationToken token)
//        {
//            var result = new Response();
//            foreach (var maxpower in _maxPowerObis)
//            {
//                var regitr = Client.Objects.FindByLN(ObjectType.Register, maxpower);
//                result.AddErrorOfComponent(Client.Execute(regitr, 1, token));
//            }
//            return result;
//        }

//        public virtual Response<CurrentQuality> GetNetworkQuality(CancellationToken token)
//        {
//            var result = new Response<CurrentQuality>() { Value = new CurrentQuality() };

//            var currentReactive = Client.ReadRegister<double>("1.0.131.7.0.255", token);
//            result.AddErrorOfComponent(currentReactive);
//            result.Value.CurrentReactive = currentReactive.Value;

//            var currentDuration = Client.ReadRegister<uint>("1.0.131.43.0.255", token);//uint
//            result.AddErrorOfComponent(currentDuration);
//            result.Value.CurrentDuration = currentDuration.Value;

//            var currentZone = Client.ReadData<byte>("0.0.96.14.1.255", token);//uint
//            result.AddErrorOfComponent(currentZone);
//            result.Value.CurrentDayZone = currentZone.Value;

//            var positiveDeviationPhaseA = Client.ReadRegister<double>("1.128.1.24.0.255", token);
//            result.AddErrorOfComponent(positiveDeviationPhaseA);
//            result.Value.PositiveDeviationPhaseA = positiveDeviationPhaseA.Value;

//            var negativeDeviationPhaseA = Client.ReadRegister<double>("1.128.2.24.0.255", token);
//            result.AddErrorOfComponent(negativeDeviationPhaseA);
//            result.Value.NegativeDeviationPhaseA = negativeDeviationPhaseA.Value;

//            return result;

//        }

//        private double? GetValueForProfile(
//    List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> profileCaptureObjects, object[] profileValues,
//    object[] scaleArray, string ObisCode, List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> scaleobj,
//    long additionalMult = 3)
//        {
//            double? result = null;
//            try
//            {
//                var index = profileCaptureObjects.FindIndex(m => m.Key.LogicalName.Equals(ObisCode));
//                var value = profileValues[index];
//                var scaleindex = scaleobj.FindIndex(m => m.Key.LogicalName.Equals(ObisCode));
//                var scaleFactor = (sbyte)(scaleArray[scaleindex] as object[])[0];
//                result = ConvertTo(Convert.ToInt64(value), Convert.ToInt64(scaleFactor), additionalMult);
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e);
//            }

//            return result;
//        }

//        public double ConvertTo(long num, long startPow, long additional)
//        {
//            return num * Math.Pow(10, startPow - additional);
//        }


//        public Response WriteImpulseMode(byte mode, CancellationToken token)
//        {
//            var telemetryTable = Client.Objects.FindByLN(ObjectType.ScriptTable, "0.0.10.0.103.255");
//            return Client.Execute(telemetryTable, 1, token, mode, DataType.UInt16);
//        }

//        public virtual ProfileReadResponse ReadEventsProfiles(EventProfileType[] profiles, CancellationToken token)
//        {
//            var result = new ProfileReadResponse(false);
//            result.MeterInfo = new ProfileMeterInfo() { MeterTypeInt = MeterType, Version = MeterVersion, MeterType = ConvertMeterType(MeterType) };
//            var allRawProfiles = new List<RawEventProfile>();
//            if (profiles == null || profiles.Count() == 0)
//                return result;

//            var requirements = EventProfileRequirements.GetRequirementsForNartis(profiles);

//            if (requirements == null || requirements.Count() == 0)
//                return result;

//            foreach (var requirement in requirements)
//            {
//                var response = ReadEventProfile(requirement, token);
//                result.AddErrorOfComponent(response);
//                if (response.IsOk)
//                    allRawProfiles.Add(response.Value);
//            }

//            if (result.IsOk)
//                result.Profiles = BaseJournalProfile.ConvertProfilesForNartis(allRawProfiles.ToArray());

//            return result;
//        }

//        public Response<CurrrentQuilityParameters> GetQualityParameters(CancellationToken token)
//        {
//            var result = new Response<CurrrentQuilityParameters>() { Value = new CurrrentQuilityParameters() };

//            var reverseUnsimmetryResponse = GetReverseUnsimmetry(token);
//            result.AddErrorOfComponent(reverseUnsimmetryResponse);
//            result.Value.ReverseUnsimmetry = reverseUnsimmetryResponse.Value;

//            var zeroUnsimmetryResponse = GetZeroUnsimmetry(token);
//            result.AddErrorOfComponent(zeroUnsimmetryResponse);
//            result.Value.ZeroUnsimmetry = zeroUnsimmetryResponse.Value;
//            return result;
//        }


//        protected Response<RawEventProfile> ReadEventProfile(EventProfileRequirements requirement, CancellationToken token)
//        {
//            var result = new Response<RawEventProfile>();

//            var profile = Client.ParsedObjects.FindByLN(ObjectType.ProfileGeneric, requirement.Obis) as GXDLMSProfileGeneric;
//            if (profile == null)
//            {
//                result.AddError($"Профиль {requirement.Obis} не найден");
//                return result;
//            }

//            result.Value = new RawEventProfile(requirement);

//            var collumns = Client.ReadCollumns(profile, token) as List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>;
//            var profileFromDevice = Client.GetUpdatedValue(new MeterObject(profile, 2), token) as object[][];

//            foreach (var profileUnit in profileFromDevice)
//            {
//                try
//                {
//                    var newModel = new RawEventProfileUnit();

//                    newModel.DateTime = GetValueForProfile<GXDateTime>(collumns, profileUnit, "0.0.1.0.0.255");
//                    newModel.WorkTime = GetValueForProfile<uint>(collumns, profileUnit, "0.0.96.8.0.255");
//                    newModel.Value = GetValueForProfile<byte>(collumns, profileUnit, requirement.EventCodeObis);
//                    newModel.AdditionalAtributes = new Dictionary<string, object>();

//                    if (requirement.AdditionalAtributes != null && requirement.AdditionalAtributes.Count() > 0)
//                    {
//                        foreach (var additionalAttribute in requirement.AdditionalAtributes)
//                            newModel.AdditionalAtributes.Add(additionalAttribute, GetValueForProfile(collumns, profileUnit, additionalAttribute));
//                    }

//                    result.Value.Units.Add(newModel);
//                }
//                catch (Exception e)
//                {
//                }
//            }

//            if (result?.Value?.Units != null)
//                result.Value.Units = result.Value.Units.OrderBy(val => val.DateTime).ToList();

//            return result;
//        }


//        public virtual BaseJournalProfile[] ParseProfileCsv(List<CsvProfile> data)
//        {
//            return BaseJournalProfile.ConvertProfilesFromNartis(data);
//        }

//        private object GetValueForProfile(List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> profileCaptureObjects,
//            object[] profileValues, string ObisCode)
//        {
//            object result = null;
//            try
//            {
//                var index = profileCaptureObjects.FindIndex(m => m.Key.LogicalName.Equals(ObisCode));
//                result = profileValues[index];
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e);
//            }

//            return result;
//        }

//        protected T GetValueForProfile<T>(List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> profileCaptureObjects,
//            object[] profileValues, string ObisCode)
//        {
//            var result = default(T);
//            try
//            {
//                var index = profileCaptureObjects.FindIndex(m => m.Key.LogicalName.Equals(ObisCode));
//                result = (T)profileValues[index];
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e);
//            }

//            return result;
//        }

//        private static bool IsAvailableLocalAssociationObject(string version, string type, AccessType accessType)
//        {
//            return File.Exists(CreateAssociationFilePath(version, type, accessType));
//        }

//        private static string CreateAssociationFilePath(string MiVersion, string type, AccessType accessType)
//        {
//            return Path.Combine(baseFilePath, $"{type} {MiVersion} {accessType}");
//        }

//        private static readonly string baseFilePath = Path.Combine(GetFolderPath(SpecialFolder.ApplicationData), "Meter_Config");

//        public virtual IImpulseOutputs ReadImpulseOutputState(CancellationToken token)
//        {
//            var response = Client.ReadData<byte>("0.0.96.4.2.255", token);

//            return new NartisImpulseOutput(response.Value);
//        }

//        public virtual Response<Theresholds> ReadTheresholds(CancellationToken token)
//        {
//            var result = new Response<Theresholds>() { Value = new Theresholds() };
//            var responseActiveEnergy = Client.ReadData<bool>("0.128.5.0.1.255", token);
//            result.AddErrorOfComponent(responseActiveEnergy);
//            result.Value.CommitPermissionActiveExport = responseActiveEnergy.Value;

//            var responseTg = Client.ReadRegister<double>("1.0.131.35.0.255", token);
//            result.AddErrorOfComponent(responseTg);
//            result.Value.TgThreshold = responseTg.Value;


//            var responseAmperage = Client.ReadRegister<double>("1.0.11.35.0.255", token);
//            result.AddErrorOfComponent(responseAmperage);
//            result.Value.MaxAmperageThreshold = responseAmperage.Value;

//            return result;
//        }

//        public virtual Response WriteTheresholds(Theresholds theresholds, CancellationToken token)
//        {
//            var result = new Response();
//            var responseActiveEnergy = Client.WriteData<bool>("0.128.5.0.1.255", theresholds.CommitPermissionActiveExport, token);
//            result.AddErrorOfComponent(responseActiveEnergy);

//            var responseTg = Client.WriteRegisterInt("1.0.131.35.0.255", theresholds.TgThreshold, token);
//            result.AddErrorOfComponent(responseTg);

//            var responseAmperage = Client.WriteRegisterInt("1.0.11.35.0.255", theresholds.MaxAmperageThreshold, token);
//            result.AddErrorOfComponent(responseAmperage);

//            return result;
//        }

//        public Response<PushSetupSettings> GetPushSetupSettings(CancellationToken token)
//        {
//            var pushSetup = (GXDLMSPushSetup)Client.Objects.FindByLN(ObjectType.PushSetup, "0.0.25.9.0.255");
//            var response = Client.ReadDlmsObject(pushSetup, new[] { 3, 6, 7 }, token);
//            if (!response.IsOk)
//                return new Response<PushSetupSettings>(response);

//            return new Response<PushSetupSettings>()
//            {
//                Value = new PushSetupSettings()
//                {
//                    DestionationAddress = pushSetup.Destination,
//                    MessageType = pushSetup.Message,
//                    NumberOfRetries = pushSetup.NumberOfRetries,
//                    RepetitionDelay = pushSetup.RepetitionDelay,
//                    ServiceType = pushSetup.Service
//                }
//            };
//        }

//        public Response<PushState> GetPushSetupState(CancellationToken token)
//        {
//            var result = Client.ReadData<byte>("0.0.96.5.3.255", token);
//            if (!result.IsOk)
//                return new Response<PushState>(result);

//            return new Response<PushState>()
//            {
//                Value = (PushState)result.Value
//            };
//        }

//        public Response WritePushSetupSettings(PushSetupSettings settings, CancellationToken token)
//        {
//            var pushSetup = (GXDLMSPushSetup)Client.Objects.FindByLN(ObjectType.PushSetup, "0.0.25.9.0.255");
//            pushSetup.Destination = settings.DestionationAddress;
//            pushSetup.Message = settings.MessageType;
//            pushSetup.NumberOfRetries = settings.NumberOfRetries;
//            pushSetup.RepetitionDelay = settings.RepetitionDelay;
//            pushSetup.Service = settings.ServiceType;

//            return Client.WriteDlmsObject(pushSetup, new[] { 3, 6, 7 }, token);
//        }

//        public virtual Response<TamperMode> GetTamperMode(CancellationToken token)
//        {
//            var response = Client.ReadData<uint>("0.128.5.0.0.255", token);
//            var data = (TamperMode)response.Value;
//            return new Response<TamperMode>(response) { Value = data };
//        }

//        public Response<DayZonesConfiguration> GetDayZonesConfiguration(CancellationToken token)
//        {
//            var result = new Response<DayZonesConfiguration>() { Value = new DayZonesConfiguration() };

//            var tariffResponse = GetTariffSchedule("0.0.13.0.1.255", null, null, token);
//            result.AddErrorOfComponent(tariffResponse);
//            result.Value.Schedule = tariffResponse.Value;


//            var highResponse = ReadRegisterDouble("1.0.131.35.1.255", token);
//            result.AddErrorOfComponent(highResponse);
//            result.Value.HighPowerZoneThereshold = highResponse.Value ?? 0;

//            var lowResponse = ReadRegisterDouble("1.0.131.35.2.255", token);
//            result.AddErrorOfComponent(lowResponse);
//            result.Value.LowPowerZoneThereshold = lowResponse.Value ?? 0;

//            return result;
//        }

//        public Response WriteDayZonesConfiguration(DayZonesConfiguration config, CancellationToken token)
//        {
//            var result = new Response();
//            result.AddErrorOfComponent(WriteTariffSchedule(config.Schedule, token, "0.0.13.0.1.255", null));
//            result.AddErrorOfComponent(Client.WriteRegisterUInt("1.0.131.35.1.255", config.HighPowerZoneThereshold, token));
//            result.AddErrorOfComponent(Client.WriteRegisterUInt("1.0.131.35.2.255", config.LowPowerZoneThereshold, token));

//            return result;
//        }

//        private Response<DLMSSchedule> GetTariffSchedule(string calendarObis, string specialDayObis, string activationDateObis, CancellationToken token)
//        {
//            var result = new Response<DLMSSchedule>() { Value = new DLMSSchedule() };

//            var profile =
//                (GXDLMSActivityCalendar)Client.Objects.FindByLN(ObjectType.ActivityCalendar,
//                    calendarObis);

//            var specialDayTable = (GXDLMSSpecialDaysTable)Client.ParsedObjects.FindByLN(ObjectType.SpecialDaysTable,
//                specialDayObis);

//            if (activationDateObis != null)
//            {
//                result.Value.ActivationDate = (GXDateTime)Client.GetUpdatedValue(new MeterObject(activationDateObis, ObjectType.Data, 2), token);
//            }
//            result.Value.SeasonProfiles =
//                ((GXDLMSSeasonProfile[])Client.GetUpdatedValue(new MeterObject(profile, 3), token))?.ToList();
//            result.Value.WeekProfiles =
//                ((GXDLMSWeekProfile[])Client.GetUpdatedValue(new MeterObject(profile, 4), token))?.ToList();
//            result.Value.DayProfiles =
//                ((GXDLMSDayProfile[])Client.GetUpdatedValue(new MeterObject(profile, 5), token))?.ToList();

//            if (specialDayObis != null)
//            {
//                result.Value.SpecialDays = ((GXDLMSSpecialDay[])Client.GetUpdatedValue(new MeterObject(specialDayTable, 2), token))?.ToList();
//            }


//            return result;
//        }

//        public virtual Response<PhaseAngles> GetPhaseAngle(CancellationToken token)
//        {
//            var result = new Response<PhaseAngles>();
//            result.Value = new PhaseAngles();

//            var phaseA = Client.ReadRegister<double>("1.0.81.7.40.255", token);
//            result.AddErrorOfComponent(phaseA);
//            result.Value.IaUa = phaseA.Value;

//            var phaseB = Client.ReadRegister<double>("1.0.81.7.51.255", token);
//            result.AddErrorOfComponent(phaseB);
//            result.Value.IbUb = phaseB.Value;

//            var phaseC = Client.ReadRegister<double>("1.0.81.7.62.255", token);
//            result.AddErrorOfComponent(phaseC);
//            result.Value.IcUc = phaseC.Value;

//            return result;
//        }

//        public virtual Response WriteTariffSchedule(DLMSSchedule dlmsModel, CancellationToken token, string calendarObis = "0.0.13.0.0.255", string specialDayTableObis = "0.0.11.0.0.255")
//        {
//            var response = new Response();

//            var profile = (GXDLMSActivityCalendar)Client.Objects.FindByLN(ObjectType.ActivityCalendar, calendarObis);

//            profile.DayProfileTablePassive = dlmsModel.DayProfiles.ToArray();
//            profile.WeekProfileTablePassive = dlmsModel.WeekProfiles.ToArray();
//            profile.SeasonProfilePassive = dlmsModel.SeasonProfiles.ToArray();
//            profile.CalendarNamePassive = "1";

//            response.AddErrorOfComponent(Client.WriteDlmsObject(profile, new[] { 9, 8, 7, 6 }, token));

//            if (response.IsOk)
//            {
//                if (dlmsModel.ActivateNow)
//                {
//                    response.AddErrorOfComponent(Client.Execute(profile, 1, token));
//                }
//                else
//                {
//                    profile.Time = dlmsModel.ActivationDate;
//                    response.AddErrorOfComponent(Client.WriteDlmsObject(profile, 10, token));
//                }

//            }

//            if (specialDayTableObis != null)
//            {
//                var specialDayTable = (GXDLMSSpecialDaysTable)Client.ParsedObjects.FindByLN(ObjectType.SpecialDaysTable, specialDayTableObis);
//                specialDayTable.Entries = dlmsModel.SpecialDays.ToArray();
//                response.AddErrorOfComponent(Client.WriteDlmsObject(specialDayTable, 2, token));
//            }


//            return response;
//        }

//        public virtual Response WriteTamperMode(TamperMode mode, CancellationToken token)
//        {
//            var uintMode = (uint)mode;
//            return Client.WriteData<uint>("0.128.5.0.0.255", uintMode, token);
//        }

//        public virtual void WriteLimiters(List<DlmsLimiter> limiters, CancellationToken token)
//        {
//            //TODO
//            foreach (var limiter in limiters)
//            {
//                var lim =
//                    (GXDLMSLimiter)Client.ParsedObjects.FindByLN(ObjectType.Limiter, limiter.Obis);

//                if (limiter.IsActive)
//                {
//                    if (limiter.CanUserChangeValue)
//                    {
//                        var activeLimiterType = lim.ThresholdActive.GetType();
//                        var dt = GXCommon.GetValueType(lim.ThresholdActive);
//                        lim.ThresholdNormal =
//                            Convert.ChangeType(limiter.TheresholdValue, activeLimiterType);
//                        lim.SetDataType(4, dt);

//                        Client.WriteObject(lim, 4, token);
//                    }

//                    lim.MinOverThresholdDuration = limiter.MinimumDurationOfExcess;
//                    Client.WriteObject(lim, 6, token);

//                    lim.MinUnderThresholdDuration = limiter.MinimumDurationOfReturn;
//                    Client.WriteObject(lim, 7, token);

//                }
//                else
//                {
//                    var dt = GXCommon.GetValueType(lim.MinOverThresholdDuration);
//                    lim.MinOverThresholdDuration = 0xFFFFFFFF;
//                    Client.WriteObject(lim, 6, token);

//                    dt = GXCommon.GetValueType(lim.MinUnderThresholdDuration);
//                    lim.MinUnderThresholdDuration = 0xFFFFFFFF;
//                    Client.WriteObject(lim, 7, token);
//                }
//            }
//        }

//        public virtual Response WriteEthernetMode(byte mode, CancellationToken token)
//        {
//            return Client.WriteData<byte>("0.128.6.0.0.255", mode, token);
//        }

//        public virtual Response WriteEthernetClientIp(string ip, CancellationToken token)
//        {
//            var ipSetup = (GXDLMSIp4Setup)Client.Objects.FindByLN(ObjectType.Ip4Setup, "0.4.25.1.0.255");
//            ipSetup.IPAddress = ip;
//            return Client.WriteDlmsObject(ipSetup, 3, token);
//        }

//        public virtual Response WriteEthernetClientPort(int timeout, ushort port, CancellationToken token)
//        {
//            var tcpSetup = (GXDLMSTcpUdpSetup)Client.Objects.FindByLN(ObjectType.TcpUdpSetup, "0.2.25.0.0.255");
//            tcpSetup.Port = port;
//            tcpSetup.InactivityTimeout = timeout;
//            return Client.WriteDlmsObject(tcpSetup, new[] { 2, 6 }, token);
//        }

//        public virtual Response WriteMac(string obis, string mac, CancellationToken token)
//        {
//            var macSetup = (GXDLMSMacAddressSetup)Client.Objects.FindByLN(ObjectType.MacAddressSetup, obis);
//            macSetup.MacAddress = mac;
//            return Client.WriteDlmsObject(macSetup, 2, token);
//        }

//        public virtual Response WritePort(string obis, ushort port, CancellationToken token)
//        {
//            var tcpSetup =
//                (GXDLMSTcpUdpSetup)Client.Objects.FindByLN(ObjectType.TcpUdpSetup, obis);
//            tcpSetup.Port = port;
//            return Client.WriteDlmsObject(tcpSetup, 2, token);
//        }

//        public virtual Response ResetGSM(CancellationToken token)
//        {
//            var gsmParams = (GXDLMSRegister)Client.Objects.FindByLN(ObjectType.Register, "0.0.96.12.5.255");
//            return Client.Execute(gsmParams, 1, token);
//        }

//        public virtual Response WriteIpSetup(string obis, string ip, ulong subnetMask, ulong gatewayIPAddress, CancellationToken token)
//        {
//            var ipSetup = (GXDLMSIp4Setup)Client.Objects.FindByLN(ObjectType.Ip4Setup, obis);

//            ipSetup.IPAddress = ip;
//            ipSetup.SubnetMask = subnetMask;
//            ipSetup.GatewayIPAddress = gatewayIPAddress;

//            return Client.WriteDlmsObject(ipSetup, new[] { 3, 6, 7 }, token);
//        }

//        public virtual Response WriteIpSetup(string obis, string ip, CancellationToken token)
//        {
//            var ipSetup = (GXDLMSIp4Setup)Client.Objects.FindByLN(ObjectType.Ip4Setup, obis);

//            ipSetup.IPAddress = ip;

//            return Client.WriteDlmsObject(ipSetup, new[] { 3 }, token);
//        }

//        protected static ulong stringToIntIP(string ip)
//        {
//            var splited = ip.Split('.');
//            var listOfInt = new List<ulong>();

//            foreach (var item in splited)
//                listOfInt.Add(uint.Parse(item));


//            var value = listOfInt[0] * 16777216 + listOfInt[1] * 65536 + listOfInt[2] * 256 + listOfInt[3];
//            return value;
//        }

//        public virtual Response ResetEventProfiles(EventProfileType[] profiles, CancellationToken token)
//        {
//            var result = new Response();
//            var requirements = EventProfileRequirements.GetRequirementsForNartis(profiles);

//            foreach (var profile in requirements)
//            {
//                result.AddErrorOfComponent(ResetProfile(profile.Obis, token));
//                if (!result.IsOk)
//                    break;
//            }
//            return result;
//        }

//        public virtual Response WriteHdlcSetup(string obis, int interCharachterTimeout, int interFrameTimeout, CancellationToken token)
//        {
//            var interfaceSettings = (GXDLMSHdlcSetup)Client.Objects.FindByLN(ObjectType.IecHdlcSetup, obis);

//            interfaceSettings.InterCharachterTimeout = interCharachterTimeout;
//            interfaceSettings.InactivityTimeout = interFrameTimeout;

//            return Client.WriteDlmsObject(interfaceSettings, new[] { 7, 8 }, token);
//        }



//        public virtual Response WriteAPN(string obis, string apn, CancellationToken token)
//        {
//            var interfaceSettings = (GXDLMSGprsSetup)Client.Objects.FindByLN(ObjectType.GprsSetup, obis);

//            interfaceSettings.APN = apn;

//            return Client.WriteDlmsObject(interfaceSettings, new[] { 2 }, token);
//        }


//        public virtual Response WriteHdlcSetup(HdlcConfiguration[] configurations, CancellationToken token)
//        {
//            var result = new Response();
//            foreach (var config in configurations)
//            {
//                result.AddErrorOfComponent(WriteHdlcSetup(config, token));
//                if (!result.IsOk)
//                    break;
//            }
//            return result;
//        }


//        public virtual Response WriteHdlcSetup(HdlcConfiguration configuration, CancellationToken token)
//        {
//            var hdlc = (GXDLMSHdlcSetup)Client.ParsedObjects.FindByLN(ObjectType.IecHdlcSetup, configuration.Obis);
//            hdlc.CommunicationSpeed = configuration.CommunicationSpeed;
//            hdlc.DeviceAddress = configuration.DeviceAddress;
//            hdlc.InterCharachterTimeout = configuration.InterCharachterTimeout;
//            hdlc.InactivityTimeout = configuration.InactivityTimeout;

//            return Client.WriteDlmsObject(hdlc, new[] { 2, 7, 8, 9 }, token);
//        }

//        public virtual string GetFullAPN(GsmConfiguration configuration)
//        {
//            return $"{configuration.APN}/{configuration.ApnUser}/{configuration.ApnPassword}";
//        }

//        public virtual string GetFullCommunicationParameters(GsmConfiguration configuration)
//        {
//            return $"{(int)configuration.Mode}/{configuration.SmsCenter}";
//        }



//        public virtual Response WriteGSMConfiguration(GsmConfiguration configuration, CancellationToken token)
//        {
//            var response = new Response();

//            response.AddErrorOfComponent(WriteAPN("0.0.25.4.0.255", GetFullAPN(configuration), token));

//            if (configuration.Mode == ModemMode.Client)
//            {
//                response.AddErrorOfComponent(WritePort("0.4.25.0.0.255", configuration.Port, token));
//                response.AddErrorOfComponent(WriteIpSetup("0.2.25.1.0.255", configuration.ClientIP, token));
//            }
//            else
//            {
//                response.AddErrorOfComponent(WritePort("0.3.25.0.0.255", configuration.Port, token));
//            }

//            response.AddErrorOfComponent(Client.WriteRegister<string>("0.0.96.12.5.255", GetFullCommunicationParameters(configuration), token));
//            response.AddErrorOfComponent(WriteHdlcSetup("0.6.22.0.0.255", configuration.InterCharachterTimeout, configuration.InterFrameTimeout, token));
//            return response;
//        }

//        public virtual Response SetClockHard(DateTime time, CancellationToken token)
//        {
//            var clock = Client.Objects.FindByLN(ObjectType.Clock, _clockDateObis) as GXDLMSClock;
//            var guruxTime = new GXDateTime(time);
//            guruxTime.DaylightSavingsBegin = false;
//            guruxTime.DaylightSavingsEnd = false;
//            guruxTime.Status = ClockStatus.Ok;
//            clock.Time = guruxTime;
//            clock.Status = ClockStatus.Ok;
//            return Client.WriteDlmsObject(clock, 2, token);
//        }

//        public virtual Response SetClockSoft(int time, CancellationToken token)
//        {
//            var clock = Client.Objects.FindByLN(ObjectType.Clock, _clockDateObis) as GXDLMSClock;
//            return Client.Execute(clock, 6, token, (Int16)time);
//        }



//        public virtual Response<GsmInfo> GetGsmInfo(CancellationToken token)
//        {
//            var result = new Response<GsmInfo>() { Value = new GsmInfo() };
//            var gprsSetup = (GXDLMSGprsSetup)Client.Objects.FindByLN(ObjectType.GprsSetup, "0.0.25.4.0.255");
//            result.AddErrorOfComponent(Client.ReadDlmsObject(gprsSetup, 2, token));
//            result.Value.ParseApn(gprsSetup.APN);

//            var communicationParamsResponse = Client.ReadRegister<string>("0.0.96.12.5.255", token);
//            result.AddErrorOfComponent(communicationParamsResponse);
//            result.Value.ParseCommunicationParameters(communicationParamsResponse.Value);

//            var gsmDiag = (GXDLMSGSMDiagnostic)Client.Objects.FindByLN(ObjectType.GSMDiagnostic, "0.0.25.6.0.255");
//            result.AddErrorOfComponent(Client.ReadDlmsObject(gsmDiag, new[] { 2, 3, 4, 6 }, token));
//            result.Value.SignalLevel = gsmDiag.CellInfo.SignalDescription();
//            result.Value.Operator = gsmDiag.Operator;
//            result.Value.CircuitSwitchStatus = gsmDiag.CircuitSwitchStatus;
//            result.Value.Status = gsmDiag.Status;


//            var ipClient = (GXDLMSIp4Setup)Client.Objects.FindByLN(ObjectType.Ip4Setup, "0.2.25.1.0.255");
//            result.AddErrorOfComponent(Client.ReadDlmsObject(ipClient, new[] { 3 }, token));
//            result.Value.ClientIP = ipClient.IPAddress;

//            var ipServer = (GXDLMSIp4Setup)Client.Objects.FindByLN(ObjectType.Ip4Setup, "0.3.25.1.0.255");
//            result.AddErrorOfComponent(Client.ReadDlmsObject(ipServer, new[] { 3 }, token));
//            result.Value.ServerIP = ipServer.IPAddress;

//            if (result.Value.Mode == ModemMode.Client)
//            {
//                var portServer = (GXDLMSTcpUdpSetup)Client.Objects.FindByLN(ObjectType.TcpUdpSetup, "0.3.25.0.0.255");
//                result.AddErrorOfComponent(Client.ReadDlmsObject(portServer, new[] { 2 }, token));
//                result.Value.Port = portServer.Port;
//            }
//            else
//            {
//                var portClient = (GXDLMSTcpUdpSetup)Client.Objects.FindByLN(ObjectType.TcpUdpSetup, "0.4.25.0.0.255");
//                result.AddErrorOfComponent(Client.ReadDlmsObject(portClient, new[] { 2 }, token));
//                result.Value.Port = portClient.Port;
//            }

//            var interfaceSettings = (GXDLMSHdlcSetup)Client.Objects.FindByLN(ObjectType.IecHdlcSetup, "0.6.22.0.0.255");
//            result.AddErrorOfComponent(Client.ReadDlmsObject(interfaceSettings, new[] { 7, 8 }, token));
//            result.Value.InterCharachterTimeout = interfaceSettings.InterCharachterTimeout;
//            result.Value.InterFrameTimeout = interfaceSettings.InactivityTimeout;

//            return result;
//        }

//        public virtual Response<EthernetConfiguration> ReadEthernetConfiguration(CancellationToken token)
//        {
//            var response = new Response<EthernetConfiguration>() { Value = new EthernetConfiguration() };

//            var macSetup = (GXDLMSMacAddressSetup)Client.Objects.FindByLN(ObjectType.MacAddressSetup, "0.1.25.2.0.255");
//            response.AddErrorOfComponent(Client.ReadDlmsObject(macSetup, 2, token));
//            response.Value.Mac = macSetup.MacAddress;

//            var incoming =
//                (GXDLMSTcpUdpSetup)Client.Objects.FindByLN(ObjectType.TcpUdpSetup, "0.1.25.0.0.255");
//            response.AddErrorOfComponent(Client.ReadDlmsObject(incoming, new[] { 2, 6 }, token));

//            response.Value.PortOfIncomingConnections = incoming.Port;

//            var ipSetup = (GXDLMSIp4Setup)Client.Objects.FindByLN(ObjectType.Ip4Setup, "0.1.25.1.0.255");
//            response.AddErrorOfComponent(Client.ReadDlmsObject(ipSetup, new[] { 3, 6, 7 }, token));

//            response.Value.IpAdress = ipSetup.IPAddress;
//            response.Value.IpGatewayAdress = intToIP(ipSetup.GatewayIPAddress);
//            response.Value.SubnetMask = intToIP(ipSetup.SubnetMask);

//            var interfaceSettings = (GXDLMSHdlcSetup)Client.Objects.FindByLN(ObjectType.IecHdlcSetup, "0.4.22.0.0.255");
//            response.AddErrorOfComponent(Client.ReadDlmsObject(interfaceSettings, new[] { 7, 8 }, token));
//            response.Value.InterCharachterTimeout = interfaceSettings.InterCharachterTimeout;
//            response.Value.InterFrameTimeout = interfaceSettings.InactivityTimeout;

//            var modeResponse = Client.ReadData<byte>("0.128.6.0.0.255", token);
//            response.AddErrorOfComponent(modeResponse);
//            response.Value.Mode = (EthernetMode)modeResponse.Value;
//            response.Value.TcpInactivityTimeoutServerMode = incoming.InactivityTimeout;

//            var clIpSetup = (GXDLMSIp4Setup)Client.Objects.FindByLN(ObjectType.Ip4Setup, "0.4.25.1.0.255");
//            response.AddErrorOfComponent(Client.ReadDlmsObject(clIpSetup, new[] { 3 }, token));
//            response.Value.IpAdressOfRemoteServer = clIpSetup.IPAddress;

//            var cltcpSetup = (GXDLMSTcpUdpSetup)Client.Objects.FindByLN(ObjectType.TcpUdpSetup, "0.2.25.0.0.255");
//            response.AddErrorOfComponent(Client.ReadDlmsObject(cltcpSetup, new[] { 2, 6 }, token));
//            response.Value.PortOfRemoteServer = cltcpSetup.Port;
//            response.Value.TcpInactivityTimeoutOfRemote = cltcpSetup.InactivityTimeout;

//            return response;
//        }

//        protected static string intToIP(ulong ip)
//        {
//            var local = new List<ulong>();
//            local.Add(ip & 0xFF);
//            local.Add((ip >> 8) & 0xFF);
//            local.Add((ip >> 16) & 0xFF);
//            local.Add((ip >> 24) & 0xFF);
//            local.Reverse();
//            return string.Join(".", local);
//        }

//        public virtual Response WriteEthernetConfiguration(EthernetConfiguration configuration, CancellationToken token)
//        {
//            var response = new Response();

//            response.AddErrorOfComponent(WriteMac("0.1.25.2.0.255", configuration.Mac, token));
//            response.AddErrorOfComponent(WritePort("0.1.25.0.0.255", configuration.PortOfIncomingConnections, token));
//            response.AddErrorOfComponent(WriteIpSetup("0.1.25.1.0.255", configuration.IpAdress, stringToIntIP(configuration.SubnetMask), stringToIntIP(configuration.IpGatewayAdress), token));
//            response.AddErrorOfComponent(WriteHdlcSetup("0.4.22.0.0.255", configuration.InterCharachterTimeout, configuration.InterFrameTimeout, token));

//            if (configuration.Mode == EthernetMode.Server)
//            {

//            }
//            else if (configuration.Mode == EthernetMode.Client)
//            {
//                response.AddErrorOfComponent(WriteEthernetClientIp(configuration.IpAdressOfRemoteServer, token));
//                response.AddErrorOfComponent(WriteEthernetClientPort(configuration.TcpInactivityTimeoutOfRemote, configuration.PortOfRemoteServer, token));
//            }

//            response.AddErrorOfComponent(WriteEthernetMode((byte)configuration.Mode, token));
//            return response;
//        }
//        public virtual Response WriteControlMode(ControlMode mode, CancellationToken token)
//        {
//            var result = new Response();
//            var dissconnectControl = Client.Objects.FindByLN(ObjectType.DisconnectControl, "0.0.96.3.10.255") as GXDLMSDisconnectControl;
//            if (dissconnectControl == null)
//            {
//                result.AddError("Object not found");
//                return result;
//            }

//            dissconnectControl.ControlMode = mode;

//            try
//            {
//                Client.WriteObject(dissconnectControl, 4, token);
//            }
//            catch (Exception e)
//            {
//                result.AddError(e.Message);
//            }

//            return result;
//        }
//        public virtual Response<List<DlmsLimiter>> GetAllLimiters(CancellationToken token)
//        {
//            var result = new Response<List<DlmsLimiter>>() { Value = new List<DlmsLimiter>() };

//            foreach (var limiter in DlmsLimiter.GetAllNartisLimiters())
//            {
//                var response = GetLimitter(limiter, token);
//                if (response == null)
//                    continue;

//                result.AddErrorOfComponent(response);
//                result.Value.Add(response.Value);
//            }

//            return result;
//        }
//        public virtual Response<LoadControlModel> GetControlModel(CancellationToken token)
//        {
//            var result = new Response<LoadControlModel>();
//            try
//            {
//                var diss = Client.ParsedObjects.FindByLN(ObjectType.DisconnectControl, "0.0.96.3.10.255");
//                result.Value = new LoadControlModel();
//                result.Value.Mode = (ControlMode)Client.GetUpdatedValue(new MeterObject(diss, 4), token);
//                result.Value.OutputState = (bool)Client.GetUpdatedValue(new MeterObject(diss, 2), token);
//                result.Value.State = (ControlState)Client.GetUpdatedValue(new MeterObject(diss, 3), token);
//            }
//            catch (Exception e)
//            {
//                result.AddError(e.Message);
//            }

//            return result;
//        }
//        public virtual Response<DlmsLimiter> GetLimitter(DlmsLimiter dlmslimiter, CancellationToken token)
//        {
//            if (!Client.IsObjectWithObisExsists(dlmslimiter.Obis))
//                return null;

//            var result = new Response<DlmsLimiter>();

//            try
//            {
//                var lim = (GXDLMSLimiter)Client.Objects.FindByLN(ObjectType.Limiter, dlmslimiter.Obis);

//                var register = Client.GetUpdatedValue(new MeterObject(lim, 2), token) as GXDLMSRegister;
//                double scaler = 1;
//                if (register != null)
//                {
//                    Client.GetUpdatedValue(new MeterObject(register, 3), token);
//                    scaler = register.Scaler;
//                }
//                var theresholdValue = Convert.ToInt64(Client.GetUpdatedValue(new MeterObject(lim, 3), token));

//                var theresholdTime = (uint)Client.GetUpdatedValue(new MeterObject(lim, 6), token);
//                var theresholdReturnTime = (uint)Client.GetUpdatedValue(new MeterObject(lim, 7), token);

//                if (dlmslimiter.CanUserChangeValue)
//                {
//                    if (theresholdTime == uint.MaxValue)
//                    {
//                        dlmslimiter.IsActive = false;
//                        dlmslimiter.Scaler = scaler;
//                    }
//                    else
//                    {
//                        dlmslimiter.IsActive = true;
//                        dlmslimiter.MinimumDurationOfExcess = theresholdTime;
//                        dlmslimiter.MinimumDurationOfReturn = theresholdReturnTime;
//                        dlmslimiter.TheresholdValue = theresholdValue;
//                        dlmslimiter.Scaler = scaler;
//                    }
//                }
//                else
//                {
//                    if ((theresholdTime == 0xFFFFFFFF && theresholdReturnTime == 0xFFFFFFFF) || theresholdValue == 0xFFFFFFFF)
//                    {
//                        dlmslimiter.IsActive = false;
//                        dlmslimiter.Scaler = scaler;
//                    }
//                    else
//                    {
//                        dlmslimiter.IsActive = true;
//                        dlmslimiter.MinimumDurationOfExcess = theresholdTime;
//                        dlmslimiter.MinimumDurationOfReturn = theresholdReturnTime;
//                        dlmslimiter.TheresholdValue = theresholdValue;
//                        dlmslimiter.Scaler = scaler;
//                    }
//                }

//                result.Value = dlmslimiter;
//            }
//            catch (Exception e)
//            {
//                result.AddError(e.Message);
//            }

//            return result;
//        }
//        public virtual IImpulseOutputs GetAllImpulseOutputs()
//        {
//            return new NartisImpulseOutput(1);
//        }
//        public virtual Response<double?[]> GetFlicker(CancellationToken token)
//        {
//            return ReadPhaseRegisters(_flicker, token);
//        }

//        public Response ResetCurrentReadings(CancellationToken token)
//        {
//            var script = (GXDLMSScriptTable)Client.Objects.FindByLN(ObjectType.ScriptTable, "0.0.10.1.0.255");
//            return Client.Execute(script, 1, token, (UInt16)1);
//        }

//        public virtual Response<IndicationDataModel> ReadIndication(CancellationToken token)
//        {
//            var result = new Response<IndicationDataModel>() { Value = new IndicationDataModel() };

//            var switchTimeResponse = Client.ReadData<byte>("0.128.1.0.0.255", token);
//            result.AddErrorOfComponent(switchTimeResponse);
//            if (switchTimeResponse.IsOk)
//                result.Value.SwitchTime = switchTimeResponse.Value;

//            result.Value.Models = new List<IndicationModeDataModel>();
//            for (int i = 0; i < IndicationDataModel.ObisModes.Count(); i++)
//            {
//                var response = Client.ReadData<byte[]>(IndicationDataModel.ObisModes[i], token);
//                result.AddErrorOfComponent(response);
//                if (!response.IsOk)
//                    break;
//                if (response.Value != null && response.Value.Length > 0)
//                {
//                    foreach (var value in response.Value)
//                    {
//                        result.Value.Models.Add(new IndicationModeDataModel() { Cycle = i + 1, Mode = value });
//                    }
//                }
//            }

//            return result;
//        }

//        public virtual Response SetIndication(IndicationDataModel model, CancellationToken token)
//        {
//            var result = new Response();
//            result.AddErrorOfComponent(Client.WriteDataAsUByte("0.128.1.0.0.255", model.SwitchTime, token));

//            for (int i = 0; i < IndicationDataModel.ObisModes.Count(); i++)
//            {
//                var modeModel = model.Models.Where(m => m.Cycle.Equals(i + 1)).Select(m => m.Mode).ToArray();
//                result.AddErrorOfComponent(Client.WriteData<byte[]>(IndicationDataModel.ObisModes[i], modeModel, token));
//                if (modeModel.Length == 0)
//                    break;
//            }

//            return result;
//        }
//        public virtual NetworkStateModel GetNetworkState(CancellationToken token)
//        {
//            var result = new NetworkStateModel();

//            var profile = GetCurrentParametersProfile(token);
//            result.ParametersProfile = profile.Value;
//            result.AddErrorOfComponent(result);

//            var temperature = GetTemperature(token);
//            result.Temperature = temperature.Value;
//            result.AddErrorOfComponent(temperature);

//            var inphaseAngles = ReadPhaseRegisters(_inphaseAngels, token);
//            result.InphaseAngles = inphaseAngles.Value;
//            result.AddErrorOfComponent(inphaseAngles);

//            var tan = ReadPhaseRegisters(_tanObis, token);
//            result.Tan = tan.Value;
//            result.AddErrorOfComponent(tan);

//            //макс мощность
//            var maxPower = ReadPhaseRegisters(_maxPowerObis, token, 0.001);
//            result.MaxPower = maxPower.Value;
//            result.AddErrorOfComponent(maxPower);


//            /*result.MiddlePower.Summ =
//                ConvertTo(
//                    Convert.ToInt64(
//                        client.GetUpdatedValue(new MeterObject("1.0.1.4.0.255", ObjectType.DemandRegister, 2),
//                            token)), Convert.ToInt64(-3), 3);*/

//            return result;
//        }



//        public virtual Response<Tan> ReadTan(CancellationToken token)
//        {
//            var result = new Response<Tan>();
//            result.Value = new Tan();

//            var tan = ReadPhaseRegisters(_tanObis, token);
//            result.AddErrorOfComponent(tan);

//            if (result.IsOk)
//            {
//                result.Value.A = tan.Value[0];
//                result.Value.B = tan.Value[1];
//                result.Value.C = tan.Value[2];
//                result.Value.Summ = tan.Value[3];
//            }

//            return result;

//        }
//        public Response<double?> GetZeroUnsimmetry(CancellationToken token)
//        {
//            return ReadRegisterDouble(_zeroUnsimmetryObis, token);
//        }
//        public Response<double?> GetReverseUnsimmetry(CancellationToken token)
//        {
//            return ReadRegisterDouble(_reverseUnsimmetryObis, token);
//        }
//        public virtual Response<double?> GetTemperature(CancellationToken token)
//        {
//            return ReadIntRegisterAncConverToDouble(_temperatureObis, token);
//        }
//        public Response<double?[]> ReadPhaseRegisters(string[] obisCodes, CancellationToken token, double additinalMult = 1)
//        {
//            var resultResponce = new Response<double?[]>();
//            var result = new List<double?>();

//            foreach (var obis in obisCodes)
//            {
//                if (!Client.IsObjectWithObisExsists(obis))
//                    result.Add(null);
//                else
//                {
//                    var registerResponse = ReadRegisterDouble(obis, token, additinalMult);
//                    if (registerResponse.Value.HasValue)
//                        registerResponse.Value = registerResponse.Value.Value;

//                    result.Add(registerResponse.Value);
//                    resultResponce.AddErrorOfComponent(registerResponse);
//                }
//            }
//            resultResponce.Value = result.ToArray();
//            return resultResponce;
//        }
//        public Response<double?> ReadRegisterDouble(string obis, CancellationToken token, double additinalMult = 1)
//        {
//            var regValue = Client.ReadRegister<double>(obis, token);
//            if (!regValue.IsOk)
//                return new Response<double?>(regValue) { Value = null };

//            return new Response<double?>(regValue) { Value = regValue.Value * additinalMult };
//        }
//        public Response<double?> ReadIntRegisterAncConverToDouble(string obis, CancellationToken token)
//        {
//            var regValue = Client.ReadRegister<int>(obis, token);
//            if (!regValue.IsOk)
//                return new Response<double?>(regValue) { Value = null };

//            return new Response<double?>(regValue) { Value = regValue.Value };
//        }
//        public Response<CurrentParametersProfile> GetCurrentParametersProfile(CancellationToken token)
//        {
//            var customProfileResponse = Client.ReadProfile(_currentValuesProfile, _currentValuesProfileScaler, token);
//            if (!customProfileResponse.IsOk)
//                return new Response<CurrentParametersProfile>(customProfileResponse);

//            var customProfile = customProfileResponse.Value;

//            var result = new CurrentParametersProfile();

//            result.Amperage = customProfile.GetValues(_currentValuesProfileAmperage);
//            result.Voltage = customProfile.GetValues(_currentValuesProfileVoltage);
//            result.ActivePower = customProfile.GetValues(_currentValuesProfileActivePower, -3);
//            result.ReactivePower = customProfile.GetValues(_currentValuesProfileReactivePower, -3);
//            result.FullPower = customProfile.GetValues(_currentValuesProfileFullPower, -3);
//            result.PowerCoefficient = customProfile.GetValues(_currentValuesProfilePowerCoefficient);
//            result.InphaseVoltage = customProfile.GetValues(_currentValuesProfileInphaseVoltage);
//            result.Frequency = customProfile.GetDoubleValue(_currentValuesProfileFrequency);
//            result.UaUbAngle = customProfile.GetDoubleValue("1.0.81.7.62.255", -3);
//            result.UaUcAngle = customProfile.GetDoubleValue("1.0.81.7.62.255", -3);
//            result.ZeroWireAmperage = customProfile.GetDoubleValue("1.0.91.7.0.255");
//            result.DifferentialCurrent = customProfile.GetDoubleValue("1.0.91.7.131.255");

//            return new Response<CurrentParametersProfile>() { Value = result };
//        }
//        public virtual PassportDataModel GetPassportData(CancellationToken token)
//        {
//            var response = new PassportDataModel();

//            try
//            {

//                if (Client.IsObjectWithObisExsists(_clockDateObis))
//                    response.AddObject(PassportDataModel.PassportDataField.Clock, GetClock(token));

//                if (Client.IsObjectWithObisExsists(_serialObis))
//                {
//                    var serial = GetSerial(token);
//                    response.AddObject(PassportDataModel.PassportDataField.Serial, serial);
//                    CurrentSerial = serial.Value;
//                }

//                if (Client.IsObjectWithObisExsists(_meterTypeObis))
//                    response.AddObject(PassportDataModel.PassportDataField.MeterType, GetMeterType(token));

//                if (Client.IsObjectWithObisExsists(_productionDateObis))
//                    response.AddObject(PassportDataModel.PassportDataField.ProductionDate, GetProductionDate(token));

//                if (Client.IsObjectWithObisExsists(_metrologicalSoftwareObis))
//                    response.AddObject(PassportDataModel.PassportDataField.MetrologicalSoftwareVersion, GetMetrologicalSoftwareVersion(token));

//                if (Client.IsObjectWithObisExsists(_softwareObis))
//                {
//                    var softwareVersion = GetSoftwareVersion(token);
//                    var stringResponse = new Response<string>() { IsOk = true, Errors = softwareVersion.Errors };

//                    response.AddObject(PassportDataModel.PassportDataField.VersionPu, new Response<string>() { IsOk = true, Errors = softwareVersion.Errors, Value = softwareVersion.Value.PuVersionString });
//                    _puVersion = softwareVersion.Value.PuVersion;
//                    _miVersion = softwareVersion.Value.MiVersion;

//                    //if (softwareVersion.Value.NeedMiVersion)
//                    response.AddObject(PassportDataModel.PassportDataField.VersionMi, new Response<string>() { IsOk = true, Errors = softwareVersion.Errors, Value = "0.0.0.0" });
//                    //response.AddObject(PassportDataModel.PassportDataField.VersionMi, new Response<string>() { IsOk = true, Errors = softwareVersion.Errors, Value = softwareVersion.Value.MiVersion.ToString() });
//                }

//                if (Client.IsObjectWithObisExsists(_currentTariff))
//                    response.AddObject(PassportDataModel.PassportDataField.CurrentTariff, GetCurrentTariff(token));

//                if (Client.IsObjectWithObisExsists(_transformationCoefficentAmperage))
//                    response.AddObject(PassportDataModel.PassportDataField.TransformatonCoeficentAmperage, GetTransformationCoefficentAmperage(token));

//                if (Client.IsObjectWithObisExsists(_transformationCoefficentVoltage))
//                    response.AddObject(PassportDataModel.PassportDataField.TransformatonCoeficentVoltage, GetTransformationCoefficentVoltage(token));

//                if (Client.IsObjectWithObisExsists(_crc))
//                    response.AddObject(PassportDataModel.PassportDataField.CRC, GetCRC(token));
//            }
//            catch
//            {
//                response.IsOk = false;
//            }

//            return response;
//        }
//        public virtual Response<DateTime> GetProductionDate(CancellationToken token)
//        {
//            var response = Client.ReadData<string>(_productionDateObis, token);

//            if (!response.IsOk)
//                response.IsOk = true;
//            //return new Response<DateTime>() { IsOk = response.IsOk, Errors = response.Errors };
//            //var date = DataHelper.ConverteProductionDate(response.Value);
//            var date = (System.Nullable<DateTime>)Convert.ToDateTime(response.Value);

//            //if(!date.HasValue)
//            //{
//            //    response.AddError($"Неверный формат даты {response.Value}");
//            //    return new Response<DateTime>() { IsOk = false, Errors = response.Errors };
//            //}

//            return new Response<DateTime>() { IsOk = true, Errors = response.Errors, Value = date.Value };
//        }
//        public virtual Response<string> GetCurrentTariff(CancellationToken token)
//        {
//            var response = Client.ReadData<byte>(_currentTariff, token);
//            //if (!response.IsOk)
//            //    return new Response<string>() { IsOk = response.IsOk, Errors = response.Errors };


//            return new Response<string>() { IsOk = true, Errors = response.Errors, Value = $"T{response.Value}" };
//        }
//        public virtual Response<uint> GetAutomaticSwitchOnCount(CancellationToken token)
//        {
//            return Client.ReadData<uint>(_automaticSwitchOnCount, token);
//        }
//        public virtual Response WriteAutomaticSwitchOnCount(uint value, CancellationToken token)
//        {
//            return Client.WriteData<uint>(_automaticSwitchOnCount, value, token);
//        }
//        public virtual Response<SoftwareVersion> GetSoftwareVersion(CancellationToken token)
//        {
//            var response = Client.ReadData<uint>(_softwareObis, token);
//            // if (!response.IsOk)
//            // return new Response<SoftwareVersion>() { IsOk = response.IsOk, Errors = response.Errors };


//            return new Response<SoftwareVersion>() { IsOk = true, Errors = response.Errors, Value = new SoftwareVersion(response.Value) };
//        }
//        public virtual Response<string> GetMetrologicalSoftwareVersion(CancellationToken token)
//        {
//            var response = Client.ReadData<uint>(_metrologicalSoftwareObis, token);
//            //if(!response.IsOk)
//            //    return new Response<string>() { IsOk = response.IsOk, Errors = response.Errors };


//            return new Response<string>() { IsOk = true, Errors = response.Errors, Value = DataHelper.IntToHex((int)response.Value) };
//        }
//        public virtual Response<string> GetSerial(CancellationToken token) => Client.ReadData<string>(_serialObis, token);
//        public virtual Response<GXDateTime> GetClock(CancellationToken token) => Client.ReadClock(_clockDateObis, token);
//        public virtual Response<uint> GetTransformationCoefficentAmperage(CancellationToken token) => Client.ReadData<uint>(_transformationCoefficentAmperage, token);
//        public virtual Response<uint> GetTransformationCoefficentVoltage(CancellationToken token) => Client.ReadData<uint>(_transformationCoefficentVoltage, token);
//        public virtual Response<string> GetCRC(CancellationToken token)
//        {
//            var crcResponse = Client.ReadData<uint>(_crc, token);
//            //if (!crcResponse.IsOk)
//            //    return new Response<string>() { IsOk = crcResponse.IsOk, Errors = crcResponse.Errors};

//            return new Response<string>() { IsOk = true, Errors = crcResponse.Errors, Value = DataHelper.IntToHex((int)crcResponse.Value) };
//        }
//        public virtual Response<string> GetMeterType(CancellationToken token)
//        {
//            var meterType = Client.ReadData<uint>(_meterTypeObis, token);
//            return new Response<string>() { IsOk = true, Errors = meterType.Errors, Value = ConvertMeterType(meterType.Value) };
//        }
//        public virtual string ConvertMeterType(uint valueFromDevice)
//        {
//            string value = null;
//            try
//            {
//                var types = Encoding.UTF8.GetString(Resources.NartisThreePhaseMeterType);
//                var meterTypes = JsonConvert.DeserializeObject<Dictionary<string, string>>(types);
//                var key = valueFromDevice.ToString();
//                if (meterTypes.ContainsKey(key))
//                    value = meterTypes[key];
//                else
//                    value = "НЕ ОПРЕДЕЛЁН";
//                //value = DataHelper.IntToHex((int)valueFromDevice);
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e);
//            }

//            return value;
//        }
//        public virtual Response<EnergyReport> GetEnergyReport(CancellationToken token)
//        {
//            var result = new EnergyReport();

//            var profileResponse = Client.ReadProfile("1.0.94.7.0.255", "1.0.94.7.3.255", token);
//            var resultResponse = new Response<EnergyReport>();
//            resultResponse.AddErrorOfComponent(profileResponse);
//            if (!resultResponse.IsOk)
//                return resultResponse;

//            var profile = profileResponse.Value;

//            var activeImport = profile.GetFractionalValueForProfile("1.0.1.8.0.255");
//            var activeExport = profile.GetFractionalValueForProfile("1.0.2.8.0.255");

//            result.SummaryEnergy.ActiveImport = activeImport;
//            result.SummaryEnergy.ActiveExport = activeExport;
//            result.SummaryEnergy.ReactiveExport = profile.GetFractionalValueForProfile("1.0.4.8.0.255");
//            result.SummaryEnergy.ReactiveImport = profile.GetFractionalValueForProfile("1.0.3.8.0.255");

//            if (activeImport != null && activeExport != null)
//            {
//                result.SummaryEnergy.ActiveEnergy = activeImport + activeExport;
//            }

//            result.TimeStamp = profile.GetTimestamp("0.0.1.0.0.255");

//            for (var i = 0; i < 8; i++)
//            {
//                var newValue = new TariffEnergy(i + 1);
//                var currentActiveImport = profile.GetFractionalValueForProfile("1.0.1.8." + (i + 1) + ".255");
//                var currentActiveExport = profile.GetFractionalValueForProfile("1.0.2.8." + (i + 1) + ".255");

//                newValue.ActiveImport = currentActiveImport;
//                newValue.ActiveExport = currentActiveExport;
//                newValue.ReactiveImport = profile.GetFractionalValueForProfile("1.0.3.8." + (i + 1) + ".255");
//                newValue.ReactiveExport = profile.GetFractionalValueForProfile("1.0.4.8." + (i + 1) + ".255");
//                if (currentActiveImport != null && currentActiveExport != null)
//                {
//                    newValue.ActiveEnergy = currentActiveExport + currentActiveImport;
//                }
//                result.EnergyByTariffs.Add(newValue);
//            }


//            if (profile.IsObisExsits(_lineLossesObis) && profile.IsObisExsits(_transformLossesObis))
//            {
//                var line = profile.GetDoubleValue(_lineLossesObis);
//                var transform = profile.GetDoubleValue(_transformLossesObis);

//                var transformResist = GetTranformationResistance(token);
//                var lineResist = GetLineResistance(token);

//                if (transformResist.IsOk)
//                {
//                    result.TransformLosses = transform.Value / Convert.ToDouble(transformResist.Value) * Math.Pow(10, -3);
//                }

//                if (lineResist.IsOk)
//                {
//                    result.LineLosses = line.Value * Convert.ToDouble(lineResist.Value) * Math.Pow(10, -3);
//                }
//            }

//            resultResponse.Value = result;

//            return resultResponse;
//        }

//        public Response WriteLineResistance(double value, CancellationToken token)
//        {
//            return WriteResistance(value, "1.0.0.10.2.255", token);
//        }

//        public virtual List<Harmonic> GetHarmonics(CancellationToken token)
//        {
//            var result = new List<Harmonic>();
//            return result;
//        }


//        public Response WriteTrasformationResistance(double value, CancellationToken token)
//        {
//            return WriteResistance(value, "1.0.0.10.1.255", token);
//        }

//        public Response ChangeReaderPassword(string newPassword, CancellationToken token)
//        {
//            var encoding = new ASCIIEncoding();
//            var bytes = encoding.GetBytes(newPassword);
//            var association = (GXDLMSAssociationLogicalName)Client.Objects.FindByLN(ObjectType.AssociationLogicalName, "0.0.40.0.2.255");

//            association.Secret = bytes;
//            return Client.WriteDlmsObject(association, 7, token);
//        }


//        public virtual Response ChangeConfiguratorPassword(string newPassword, string oldPassword, CancellationToken token)
//        {
//            var encoding = new ASCIIEncoding();
//            var bytes = encoding.GetBytes(newPassword);
//            var association = (GXDLMSAssociationLogicalName)Client.Objects.FindByLN(ObjectType.AssociationLogicalName, "0.0.40.0.3.255");
//            association.Secret = bytes;
//            bytes = GXSecure.Secure(Client.Settings, Client.Settings.Cipher, 0, bytes, encoding.GetBytes(oldPassword));
//            return Client.Execute(association, 2, token, bytes, DataType.OctetString);
//        }

//        public virtual Response<uint> GetWritePeriod(CancellationToken token)
//        {
//            return Client.ReadData<uint>("1.0.0.8.4.255", token);
//        }

//        public virtual Response SetWritePeriod(uint value, CancellationToken token)
//        {
//            return Client.WriteData("1.0.0.8.4.255", value, token);
//        }

//        public virtual Response ResetCurrentProfile(CancellationToken token)
//        {
//            return ResetProfile("1.0.99.1.0.255", token);
//        }

//        public virtual Response ResetDayProfile(CancellationToken token)
//        {
//            return ResetProfile("1.0.98.2.0.255", token);
//        }

//        public virtual Response ResetYearProfile(CancellationToken token)
//        {
//            return ResetProfile("0.0.99.98.14.255", token);
//        }

//        public virtual Response WriteGatewayConfiguration(GatewayType gateway, uint timeout, CancellationToken token)
//        {
//            var result = Client.WriteData<uint>("0.0.166.0.3.255", timeout, token);

//            var scriptTable = Client.Objects.FindByLN(ObjectType.ScriptTable, "0.0.10.0.130.255");
//            result.AddErrorOfComponent(Client.Execute(scriptTable, 1, token, (UInt16)gateway, DataType.UInt16));

//            return result;
//        }

//        public virtual Response ResetHourProfile(CancellationToken token)
//        {
//            return ResetProfile("1.0.99.2.0.255", token);
//        }

//        public virtual Response ResetMonthProfile(CancellationToken token)
//        {
//            return ResetProfile("1.0.98.1.0.255", token);
//        }

//        public virtual Response ResetProfile(string obis, CancellationToken token)
//        {
//            var profile = (GXDLMSProfileGeneric)Client.Objects.FindByLN(ObjectType.ProfileGeneric, obis);
//            return Client.Execute(profile, 1, token);
//        }

//        protected Response WriteResistance(double value, string obis, CancellationToken token)
//        {
//            var resistance = (GXDLMSRegister)Client.ParsedObjects.FindByLN(ObjectType.Register, obis);

//            var scalerReadResponse = Client.ReadDlmsObject(resistance, new[] { 3 }, token);

//            if (!scalerReadResponse.IsOk)
//                return scalerReadResponse;

//            resistance.Value = (uint)(value / resistance.Scaler);
//            resistance.SetDataType(2, DataType.UInt32);

//            return Client.WriteDlmsObject(resistance, 2, token);
//        }

//        public virtual Response<double> GetLineResistance(CancellationToken token)
//        {
//            return Client.ReadRegister<double>("1.0.0.10.2.255", token);
//        }

//        public virtual Response<double> GetTranformationResistance(CancellationToken token)
//        {
//            return Client.ReadRegister<double>("1.0.0.10.1.255", token);
//        }

//        public virtual Response<DlmsArchive> GetLoadProfile(DateTime? startTime, DateTime? endTime, CancellationToken token)
//        {
//            //var currentTime = GetClock();
//            var profile =
//                    Client.Objects.FindByLN(ObjectType.ProfileGeneric, "1.0.99.1.0.255") as
//                        GXDLMSProfileGeneric;
//            var collumns =
//                Client.ReadCollumns(profile, token) as List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>;
//            Response<DlmsArchive> currentProfile = null;
//            if (startTime.HasValue && endTime.HasValue)
//            {
//                //currentProfile = Client.ReadProfile("1.0.99.1.0.255", "1.0.94.7.4.255", startTime.Value, endTime.Value, token);
//            }
//            else
//            {
//                currentProfile = Client.ReadArchive("1.0.99.1.0.255", "1.0.94.7.4.255", token);
//            }

//            return currentProfile;
//        }

//        public virtual Response<string> GetAccountingPointInfo(CancellationToken token, string encoding = "windows-1251")
//        {
//            var response = Client.ReadDataAsOctetString(_accountingPointInfoObis, token);
//            if (response.IsOk)
//            {
//                var hexInterpretation = StringToArrayOfByte(response.Value);
//                if (hexInterpretation == null || hexInterpretation.Count() == 0)
//                {
//                }
//                else
//                {
//                    var dstEncodingFormat = Encoding.GetEncoding(encoding);
//                    response.Value = dstEncodingFormat.GetString(hexInterpretation);
//                }
//            }

//            return response;
//        }
//        public virtual void WriteAccountingPointInfo(string info, CancellationToken token, string encoding = "windows-1251")
//        {
//            var points = Client.ParsedObjects.FindByLN(ObjectType.Data, _accountingPointInfoObis) as GXDLMSData;
//            var dstEncodingFormat = Encoding.GetEncoding(encoding);
//            var sourceValue = dstEncodingFormat.GetBytes(info);
//            points.SetDataType(2, DataType.OctetString);
//            points.Value = sourceValue;
//            var reply = Client.WriteObject(points, 2, token);
//        }

//        public virtual Response<ulong> GetActivePowerForPeriod(CancellationToken token)
//        {
//            return Client.ReadDemandRegistrAsULong(_activePowerForPeriodObis, token);
//        }

//        public virtual Response WriteActivePowerForPeriod(ulong period, CancellationToken token)
//        {
//            return Client.WriteDemandRegistrPeriod(_activePowerForPeriodObis, period, token);
//        }


//        public virtual Response WriteTransformCoeff(uint amperage, uint voltage, CancellationToken token)
//        {
//            var result = new Response();
//            result.AddErrorOfComponent(Client.WriteData(_amperageTransformObis, amperage, token));
//            result.AddErrorOfComponent(Client.WriteData(_voltageTransformObis, voltage, token));

//            return result;
//        }

//        protected static byte[] StringToArrayOfByte(string hex)
//        {
//            var hexArray = hex.Split(' ');
//            var result = new List<byte>();
//            foreach (var hexStr in hexArray)
//            {
//                if (hexStr.Count() != 2)
//                    return null;
//                try
//                {
//                    result.Add(Convert.ToByte(hexStr, 16));
//                }
//                catch
//                {
//                    return null;
//                }
//            }

//            return result.ToArray();
//        }

//        public PqpResponse ReadPqpProfile(CancellationToken token)
//        {
//            var result = new PqpResponse();
//            try
//            {
//                var rawResult = new List<PqpProfileUnit>();
//                var profile = Client.Objects.FindByLN(ObjectType.ProfileGeneric, "0.0.99.98.126.255") as GXDLMSProfileGeneric;

//                var collumns = Client.ReadCollumns(profile, token) as List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>;
//                var count = (int)Client.GetUpdatedValue(new MeterObject(profile, 7), token);
//                object[][] currentProfile = (Client.ReadProfileByCount(profile, 1, count, token) as object[][])?.Reverse().ToArray();

//                var scale = (GXDLMSProfileGeneric)Client.ParsedObjects.FindByLN(ObjectType.ProfileGeneric, "1.0.94.7.10.255");
//                var collumnsScale = Client.ReadCollumns(scale, token) as List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>;

//                var scaleBuffer = (Client.ReadProfile(scale, token) as object[][]).First();

//                foreach (var ccr in currentProfile)
//                {
//                    var newModel = new PqpProfileUnit();

//                    newModel.EndDate = GetValueForProfile<GXDateTime>(collumns, ccr, "0.0.1.0.0.255");
//                    newModel.StartDate = newModel.EndDate.AddMinutes(-10);
//                    newModel.VoltageA = GetValueForProfile(collumns, ccr, scaleBuffer, "1.0.32.24.0.255", collumnsScale, 0).Value;
//                    newModel.VoltageB = GetValueForProfile(collumns, ccr, scaleBuffer, "1.0.52.24.0.255", collumnsScale, 0).Value;
//                    newModel.VoltageC = GetValueForProfile(collumns, ccr, scaleBuffer, "1.0.72.24.0.255", collumnsScale, 0).Value;


//                    newModel.InphaseVoltageAB = GetValueForProfile(collumns, ccr, scaleBuffer, "1.0.124.24.0.255", collumnsScale, 0).Value;
//                    newModel.InphaseVoltageBC = GetValueForProfile(collumns, ccr, scaleBuffer, "1.0.125.24.0.255", collumnsScale, 0).Value;
//                    newModel.InphaseVoltageCA = GetValueForProfile(collumns, ccr, scaleBuffer, "1.0.126.24.0.255", collumnsScale, 0).Value;

//                    newModel.PosDeviationVoltageA = GetValueForProfile(collumns, ccr, scaleBuffer, "1.128.1.24.0.255", collumnsScale, 0).Value;
//                    newModel.PosDeviationVoltageB = GetValueForProfile(collumns, ccr, scaleBuffer, "1.129.1.24.0.255", collumnsScale, 0).Value;
//                    newModel.PosDeviationVoltageC = GetValueForProfile(collumns, ccr, scaleBuffer, "1.130.1.24.0.255", collumnsScale, 0).Value;

//                    newModel.NegDeviationVoltageA = GetValueForProfile(collumns, ccr, scaleBuffer, "1.128.2.24.0.255", collumnsScale, 0).Value;
//                    newModel.NegDeviationVoltageB = GetValueForProfile(collumns, ccr, scaleBuffer, "1.129.2.24.0.255", collumnsScale, 0).Value;
//                    newModel.NegDeviationVoltageC = GetValueForProfile(collumns, ccr, scaleBuffer, "1.130.2.24.0.255", collumnsScale, 0).Value;

//                    newModel.AmperageA = GetValueForProfile(collumns, ccr, scaleBuffer, "1.0.31.24.0.255", collumnsScale, 0).Value;
//                    newModel.AmperageB = GetValueForProfile(collumns, ccr, scaleBuffer, "1.0.51.24.0.255", collumnsScale, 0).Value;
//                    newModel.AmperageC = GetValueForProfile(collumns, ccr, scaleBuffer, "1.0.71.24.0.255", collumnsScale, 0).Value;

//                    newModel.ActivePowerA = GetValueForProfile(collumns, ccr, scaleBuffer, "1.0.21.24.0.255", collumnsScale).Value;
//                    newModel.ActivePowerB = GetValueForProfile(collumns, ccr, scaleBuffer, "1.0.41.24.0.255", collumnsScale).Value;
//                    newModel.ActivePowerC = GetValueForProfile(collumns, ccr, scaleBuffer, "1.0.61.24.0.255", collumnsScale).Value;


//                    newModel.ReactivePowerA = GetValueForProfile(collumns, ccr, scaleBuffer, "1.0.23.24.0.255", collumnsScale).Value;
//                    newModel.ReactivePowerB = GetValueForProfile(collumns, ccr, scaleBuffer, "1.0.43.24.0.255", collumnsScale).Value;
//                    newModel.ReactivePowerC = GetValueForProfile(collumns, ccr, scaleBuffer, "1.0.63.24.0.255", collumnsScale).Value;

//                    newModel.FullPowerA = GetValueForProfile(collumns, ccr, scaleBuffer, "1.0.29.24.0.255", collumnsScale).Value;
//                    newModel.FullPowerB = GetValueForProfile(collumns, ccr, scaleBuffer, "1.0.49.24.0.255", collumnsScale).Value;
//                    newModel.FullPowerC = GetValueForProfile(collumns, ccr, scaleBuffer, "1.0.69.24.0.255", collumnsScale).Value;

//                    newModel.ZeroUnsim = GetValueForProfile(collumns, ccr, scaleBuffer, "1.0.136.24.0.255", collumnsScale).Value;
//                    newModel.ReverseUnsim = GetValueForProfile(collumns, ccr, scaleBuffer, "1.0.135.24.0.255", collumnsScale).Value;

//                    rawResult.Add(newModel);
//                }
//                result.Value = rawResult.ToArray();
//            }
//            catch (Exception e)
//            {
//                result.AddError(e.Message);
//            }

//            return result;
//        }

//        public MaceProfileResponse ReadMace(CancellationToken token)
//        {
//            var result = new MaceProfileResponse();
//            try
//            {
//                var rawResult = new List<MaceProfile>();
//                var profile = Client.ParsedObjects.FindByLN(ObjectType.ProfileGeneric, "0.0.99.98.127.255") as GXDLMSProfileGeneric;

//                var collumns = Client.ReadCollumns(profile, token) as List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>;

//                var count = (int)Client.GetUpdatedValue(new MeterObject(profile, 7), token);
//                var currentProfile = (Client.ReadProfileByCount(profile, 1, count, token) as object[][])?.Reverse().ToArray();
//                var scale = (GXDLMSProfileGeneric)Client.ParsedObjects.FindByLN(ObjectType.ProfileGeneric, "1.0.94.7.8.255");
//                var collumnsScale = Client.ReadCollumns(scale, token) as List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>;

//                var scaleBuffer = (Client.ReadProfile(scale, token) as object[][]).First();

//                foreach (var ccr in currentProfile)
//                {
//                    var newModel = new MaceProfile();

//                    /*newModel.Serial = DlmsManager.Instance.CurrentSerial;
//                    newModel.MeterType = DlmsManager.Instance.CurrentType;*/
//                    newModel.EndDate = GetValueForProfile<GXDateTime>(collumns, ccr, "0.0.1.0.0.255");
//                    newModel.StartDate = newModel.EndDate.AddDays(-1);
//                    newModel.GreatestPonA = GetValueForProfile(collumns, ccr, scaleBuffer, "1.128.1.1.0.255", collumnsScale, 0).Value * 100;
//                    newModel.GreatestPonB = GetValueForProfile(collumns, ccr, scaleBuffer, "1.129.1.1.0.255", collumnsScale, 0).Value * 100;
//                    newModel.GreatestPonC = GetValueForProfile(collumns, ccr, scaleBuffer, "1.130.1.1.0.255", collumnsScale, 0).Value * 100;

//                    newModel.TtwoPonA = GetValueForProfile(collumns, ccr, scaleBuffer, "1.128.1.4.0.255", collumnsScale, 0).Value * 100;
//                    newModel.TtwoPonB = GetValueForProfile(collumns, ccr, scaleBuffer, "1.129.1.4.0.255", collumnsScale, 0).Value * 100;
//                    newModel.TtwoPonC = GetValueForProfile(collumns, ccr, scaleBuffer, "1.130.1.4.0.255", collumnsScale, 0).Value * 100;

//                    newModel.GreatestOonA = GetValueForProfile(collumns, ccr, scaleBuffer, "1.128.2.1.0.255", collumnsScale, 0).Value * 100;
//                    newModel.GreatestOonB = GetValueForProfile(collumns, ccr, scaleBuffer, "1.129.2.1.0.255", collumnsScale, 0).Value * 100;
//                    newModel.GreatestOonC = GetValueForProfile(collumns, ccr, scaleBuffer, "1.130.2.1.0.255", collumnsScale, 0).Value * 100;

//                    newModel.TtwoOonA = GetValueForProfile(collumns, ccr, scaleBuffer, "1.128.2.4.0.255", collumnsScale, 0).Value * 100;
//                    newModel.TtwoOonB = GetValueForProfile(collumns, ccr, scaleBuffer, "1.129.2.4.0.255", collumnsScale, 0).Value * 100;
//                    newModel.TtwoOonC = GetValueForProfile(collumns, ccr, scaleBuffer, "1.130.2.4.0.255", collumnsScale, 0).Value * 100;

//                    newModel.TopK2u = GetValueForProfile(collumns, ccr, scaleBuffer, "1.128.4.2.0.255", collumnsScale, 0).Value * 100;
//                    newModel.GreatestK2u = GetValueForProfile(collumns, ccr, scaleBuffer, "1.128.4.1.0.255", collumnsScale, 0).Value * 100;
//                    newModel.TOneK2u = GetValueForProfile(collumns, ccr, scaleBuffer, "1.128.4.3.0.255", collumnsScale, 0).Value * 100;
//                    newModel.TtwoK2u = GetValueForProfile(collumns, ccr, scaleBuffer, "1.128.4.4.0.255", collumnsScale, 0).Value * 100;


//                    newModel.TopK0u = GetValueForProfile(collumns, ccr, scaleBuffer, "1.128.3.2.0.255", collumnsScale, 0).Value * 100;
//                    newModel.GreatestK0u = GetValueForProfile(collumns, ccr, scaleBuffer, "1.128.3.1.0.255", collumnsScale, 0).Value * 100;
//                    newModel.TOneK0u = GetValueForProfile(collumns, ccr, scaleBuffer, "1.128.3.3.0.255", collumnsScale, 0).Value * 100;
//                    newModel.TtwoK0u = GetValueForProfile(collumns, ccr, scaleBuffer, "1.128.3.4.0.255", collumnsScale, 0).Value * 100;

//                    newModel.TopDf = GetValueForProfile(collumns, ccr, scaleBuffer, "1.128.5.2.0.255", collumnsScale, 0).Value;
//                    newModel.GreatestDf = GetValueForProfile(collumns, ccr, scaleBuffer, "1.128.5.1.0.255", collumnsScale, 0).Value;
//                    newModel.DownDf = GetValueForProfile(collumns, ccr, scaleBuffer, "1.128.5.6.0.255", collumnsScale, 0).Value;
//                    newModel.LowestDf = GetValueForProfile(collumns, ccr, scaleBuffer, "1.128.5.5.0.255", collumnsScale, 0).Value;

//                    newModel.TOneDf = GetValueForProfile(collumns, ccr, scaleBuffer, "1.128.5.3.0.255", collumnsScale, 0).Value * 100;
//                    newModel.TtwoDf = GetValueForProfile(collumns, ccr, scaleBuffer, "1.128.5.4.0.255", collumnsScale, 0).Value * 100;

//                    newModel.GreatestKdfA = GetValueForProfile(collumns, ccr, scaleBuffer, "1.128.6.1.0.255", collumnsScale, 0).Value;
//                    newModel.GreatestKdfB = GetValueForProfile(collumns, ccr, scaleBuffer, "1.129.6.1.0.255", collumnsScale, 0).Value;
//                    newModel.GreatestKdfC = GetValueForProfile(collumns, ccr, scaleBuffer, "1.130.6.1.0.255", collumnsScale, 0).Value;

//                    //
//                    newModel.SummHarmonic = new HarmonicParameters();
//                    newModel.SummHarmonic.PhaseA.K95 = GetValueForProfile(collumns, ccr, scaleBuffer, "1.128.7.2.0.255", collumnsScale, 0).Value * 100;
//                    newModel.SummHarmonic.PhaseA.K100 = GetValueForProfile(collumns, ccr, scaleBuffer, "1.128.7.1.0.255", collumnsScale, 0).Value * 100;
//                    newModel.SummHarmonic.PhaseA.TOne = GetValueForProfile(collumns, ccr, scaleBuffer, "1.128.7.3.0.255", collumnsScale, 0).Value * 100;
//                    newModel.SummHarmonic.PhaseA.TTwo = GetValueForProfile(collumns, ccr, scaleBuffer, "1.128.7.4.0.255", collumnsScale, 0).Value * 100;


//                    newModel.SummHarmonic.PhaseB.K95 = GetValueForProfile(collumns, ccr, scaleBuffer, "1.129.7.2.0.255", collumnsScale, 0).Value * 100;
//                    newModel.SummHarmonic.PhaseB.K100 = GetValueForProfile(collumns, ccr, scaleBuffer, "1.129.7.1.0.255", collumnsScale, 0).Value * 100;
//                    newModel.SummHarmonic.PhaseB.TOne = GetValueForProfile(collumns, ccr, scaleBuffer, "1.129.7.3.0.255", collumnsScale, 0).Value * 100;
//                    newModel.SummHarmonic.PhaseB.TTwo = GetValueForProfile(collumns, ccr, scaleBuffer, "1.129.7.4.0.255", collumnsScale, 0).Value * 100;

//                    newModel.SummHarmonic.PhaseC.K95 = GetValueForProfile(collumns, ccr, scaleBuffer, "1.130.7.2.0.255", collumnsScale, 0).Value * 100;
//                    newModel.SummHarmonic.PhaseC.K100 = GetValueForProfile(collumns, ccr, scaleBuffer, "1.130.7.1.0.255", collumnsScale, 0).Value * 100;
//                    newModel.SummHarmonic.PhaseC.TOne = GetValueForProfile(collumns, ccr, scaleBuffer, "1.130.7.3.0.255", collumnsScale, 0).Value * 100;
//                    newModel.SummHarmonic.PhaseC.TTwo = GetValueForProfile(collumns, ccr, scaleBuffer, "1.130.7.4.0.255", collumnsScale, 0).Value * 100;


//                    //

//                    for (int harmIndex = 2; harmIndex <= 40; harmIndex++)
//                    {
//                        var harmonic = new HarmonicParameters() { Number = harmIndex.ToString() };

//                        harmonic.PhaseA.K95 = GetValueForProfile(collumns, ccr, scaleBuffer, "1.128." + (harmIndex + 9) + ".2.0.255", collumnsScale, 0).Value * 100;
//                        harmonic.PhaseA.K100 = GetValueForProfile(collumns, ccr, scaleBuffer, "1.128." + (harmIndex + 9) + ".1.0.255", collumnsScale, 0).Value * 100;
//                        harmonic.PhaseA.TOne = GetValueForProfile(collumns, ccr, scaleBuffer, "1.128." + (harmIndex + 9) + ".3.0.255", collumnsScale, 0).Value * 100;
//                        harmonic.PhaseA.TTwo = GetValueForProfile(collumns, ccr, scaleBuffer, "1.128." + (harmIndex + 9) + ".4.0.255", collumnsScale, 0).Value * 100;


//                        harmonic.PhaseB.K95 = GetValueForProfile(collumns, ccr, scaleBuffer, "1.129." + (harmIndex + 9) + ".2.0.255", collumnsScale, 0).Value * 100;
//                        harmonic.PhaseB.K100 = GetValueForProfile(collumns, ccr, scaleBuffer, "1.129." + (harmIndex + 9) + ".1.0.255", collumnsScale, 0).Value * 100;
//                        harmonic.PhaseB.TOne = GetValueForProfile(collumns, ccr, scaleBuffer, "1.129." + (harmIndex + 9) + ".3.0.255", collumnsScale, 0).Value * 100;
//                        harmonic.PhaseB.TTwo = GetValueForProfile(collumns, ccr, scaleBuffer, "1.129." + (harmIndex + 9) + ".4.0.255", collumnsScale, 0).Value * 100;

//                        harmonic.PhaseC.K95 = GetValueForProfile(collumns, ccr, scaleBuffer, "1.130." + (harmIndex + 9) + ".2.0.255", collumnsScale, 0).Value * 100;
//                        harmonic.PhaseC.K100 = GetValueForProfile(collumns, ccr, scaleBuffer, "1.130." + (harmIndex + 9) + ".1.0.255", collumnsScale, 0).Value * 100;
//                        harmonic.PhaseC.TOne = GetValueForProfile(collumns, ccr, scaleBuffer, "1.130." + (harmIndex + 9) + ".3.0.255", collumnsScale, 0).Value * 100;
//                        harmonic.PhaseC.TTwo = GetValueForProfile(collumns, ccr, scaleBuffer, "1.130." + (harmIndex + 9) + ".4.0.255", collumnsScale, 0).Value * 100;


//                        newModel.Harmonics.Add(harmonic);
//                    }

//                    rawResult.Add(newModel);
//                }

//                result.Value = rawResult.ToArray();
//            }
//            catch (Exception e)
//            {
//                result.AddError(e.Message);
//            }

//            return result;
//        }

//        public void ReadMaceProfile(CancellationToken token)
//        {

//        }

//        public Response<QualityReportSourceData> GetQualityReportSourceData(CancellationToken token)
//        {
//            var result = new Response<QualityReportSourceData>() { Value = new QualityReportSourceData() { Serial = CurrentSerial, MeterType = MeterType } };


//            var powerResponse = ReadEventsProfiles(new[] { EventProfileType.PowerEventsProfile }, token);
//            if (!powerResponse.IsOk)
//            {
//                result.AddErrorOfComponent(powerResponse);
//                return result;
//            }
//            result.Value.Power = powerResponse.Profiles.Single();

//            var qualityResponse = ReadEventsProfiles(new[] { EventProfileType.NetworkQualityEventsProfile }, token);
//            if (!qualityResponse.IsOk)
//            {
//                result.AddErrorOfComponent(qualityResponse);
//                return result;
//            }
//            result.Value.Quality = qualityResponse.Profiles.Single();

//            var voltageResponse = ReadEventsProfiles(new[] { EventProfileType.VoltageEventsProfile }, token);
//            if (!voltageResponse.IsOk)
//            {
//                result.AddErrorOfComponent(voltageResponse);
//                return result;
//            }
//            result.Value.Voltage = voltageResponse.Profiles.Single();

//            var pqpProfile = ReadPqpProfile(token);
//            if (!pqpProfile.IsOk)
//            {
//                result.AddErrorOfComponent(pqpProfile);
//                return result;
//            }
//            result.Value.Pqp = pqpProfile.Value;

//            var maceResponse = ReadMace(token);
//            if (!maceResponse.IsOk)
//            {
//                result.AddErrorOfComponent(maceResponse);
//                return result;
//            }
//            result.Value.Profiles = maceResponse.Value;


//            return result;

//        }

//    }
//}
