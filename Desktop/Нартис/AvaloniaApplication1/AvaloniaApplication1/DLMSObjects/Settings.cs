using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gurux.Common;
using Gurux.Serial;
using Gurux.Net;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Secure;
using System.Diagnostics;
using System.IO.Ports;
using Gurux.DLMS.Objects.Enums;
using Gurux.MQTT;
using Gurux.DLMS;
using ThingLing.Controls;

namespace AvaloniaApplication1.DLMSObjects
{
    public class Settings
    {
        public IGXMedia media = null;
        public TraceLevel trace = TraceLevel.Info;
        public GXDLMSSecureClient client = new GXDLMSSecureClient(true);
        // Invocation counter (frame counter).
        public string invocationCounter = null;
        //Objects to read.
        public List<KeyValuePair<string, int>> readObjects = new List<KeyValuePair<string, int>>();
        //Cache file.
        public string outputFile = null;
        //Client and server certificates are exported from the meter.
        public string ExportSecuritySetupLN = null;

        //Generate new client and server certificates and import them to the server.
        public string GenerateSecuritySetupLN = null;

        public static int GetParameters(string[] args, Settings settings)
        {
            GXSerial serial;
            //Has user give the custom serial port settings or are the default values used in mode E.
            bool modeEDefaultValues = true;
            string[] tmp;
            List<GXCmdParameter> parameters = GXCommon.GetParameters(args, "h:p:c:s:r:i:It:a:P:g:S:C:n:v:o:T:A:B:D:d:l:F:m:E:V:G:M:K:N:W:w:f:L:q:");
            foreach (GXCmdParameter it in parameters)
            {
                switch (it.Tag)
                {
                    case 'r':
                        if (string.Compare(it.Value, "sn", true) == 0)
                        {
                            settings.client.UseLogicalNameReferencing = false;
                        }
                        else if (string.Compare(it.Value, "ln", true) == 0)
                        {
                            settings.client.UseLogicalNameReferencing = true;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid reference option.");
                        }
                        break;
                    case 'h':
                        {
                            //Host address.
                            if (settings.media == null)
                            {
                                settings.media = new GXNet();
                            }
                            if (settings.media is GXNet net)
                            {
                                net.HostName = it.Value;
                            }
                            else if (settings.media is GXMqtt mqtt)
                            {
                                mqtt.ServerAddress = it.Value;
                            }
                        }
                        break;
                    case 't':
                        //Trace.
                        try
                        {
                            settings.trace = (TraceLevel)Enum.Parse(typeof(TraceLevel), it.Value);
                        }
                        catch (Exception)
                        {
                            throw new ArgumentException("Invalid trace level option. (Error, Warning, Info, Verbose, Off)");
                        }
                        break;
                    case 'p':
                        {
                            //Port.
                            if (settings.media == null)
                            {
                                settings.media = new GXNet();
                            }
                            if (settings.media is GXNet net)
                            {
                                net.Port = int.Parse(it.Value);
                            }
                            else if (settings.media is GXMqtt mqtt)
                            {
                                mqtt.Port = int.Parse(it.Value);
                            }
                        }
                        break;
                    case 'P'://Password
                        if (it.Value.StartsWith("0x"))
                        {
                            settings.client.Password = GXCommon.HexToBytes(it.Value.Substring(2));
                        }
                        else
                        {
                            settings.client.Password = ASCIIEncoding.ASCII.GetBytes(it.Value);
                        }
                        break;
                    case 'i':
                        try
                        {
                            settings.client.InterfaceType = (InterfaceType)Enum.Parse(typeof(InterfaceType), it.Value);
                            settings.client.Plc.Reset();
                            if (modeEDefaultValues && settings.client.InterfaceType == InterfaceType.HdlcWithModeE &&
                                settings.media is GXSerial)
                            {
                                serial = settings.media as GXSerial;
                                serial.BaudRate = 300;
                                serial.DataBits = 7;
                                serial.Parity = Parity.Even;
                                serial.StopBits = StopBits.One;
                            }
                        }
                        catch (Exception)
                        {
                            throw new ArgumentException("Invalid interface type option. (HDLC, WRAPPER, HdlcWithModeE, Plc, PlcHdlc)");
                        }
                        break;
                    case 'I':
                        //AutoIncreaseInvokeID.
                        settings.client.AutoIncreaseInvokeID = true;
                        break;
                    case 'v':
                        settings.invocationCounter = it.Value.Trim();
                        Gurux.DLMS.Objects.GXDLMSObject.ValidateLogicalName(settings.invocationCounter);
                        break;
                    case 'g':
                        //Get (read) selected objects.
                        foreach (string o in it.Value.Split(new char[] { ';', ',' }))
                        {
                            tmp = o.Split(new char[] { ':' });
                            if (tmp.Length != 2)
                            {
                                throw new ArgumentOutOfRangeException("Invalid Logical name or attribute index.");
                            }
                            settings.readObjects.Add(new KeyValuePair<string, int>(tmp[0].Trim(), int.Parse(tmp[1].Trim())));
                        }
                        break;
                    case 'S'://Serial Port
                        settings.media = new GXSerial();
                        serial = settings.media as GXSerial;
                        tmp = it.Value.Split(':');
                        serial.PortName = tmp[0];
                        if (tmp.Length > 1)
                        {
                            modeEDefaultValues = false;
                            serial.BaudRate = int.Parse(tmp[1]);
                            serial.DataBits = int.Parse(tmp[2].Substring(0, 1));
                            serial.Parity = (Parity)Enum.Parse(typeof(Parity), tmp[2].Substring(1, tmp[2].Length - 2));
                            serial.StopBits = (StopBits)int.Parse(tmp[2].Substring(tmp[2].Length - 1, 1));
                        }
                        else
                        {
                            if (settings.client.InterfaceType == InterfaceType.HdlcWithModeE)
                            {
                                serial.BaudRate = 300;
                                serial.DataBits = 7;
                                serial.Parity = Parity.Even;
                                serial.StopBits = StopBits.One;
                            }
                            else
                            {
                                serial.BaudRate = 9600;
                                serial.DataBits = 8;
                                serial.Parity = Parity.None;
                                serial.StopBits = StopBits.One;
                            }
                        }
                        break;
                    case 'q':
                        {
                            GXMqtt mqtt = new GXMqtt()
                            {
                                Topic = it.Value,
                            };
                            if (settings.media is GXNet net)
                            {
                                mqtt.ServerAddress = net.HostName;
                                if (net.Port != 0)
                                {
                                    mqtt.Port = net.Port;
                                }
                            }
                            settings.media = mqtt;
                        }
                        break;
                    case 'a':
                        try
                        {
                            if (string.Compare("None", it.Value, true) == 0)
                            {
                                settings.client.Authentication = Authentication.None;
                            }
                            else if (string.Compare("Low", it.Value, true) == 0)
                            {
                                settings.client.Authentication = Authentication.Low;
                            }
                            else if (string.Compare("High", it.Value, true) == 0)
                            {
                                settings.client.Authentication = Authentication.High;
                            }
                            else if (string.Compare("HighMd5", it.Value, true) == 0)
                            {
                                settings.client.Authentication = Authentication.HighMD5;
                            }
                            else if (string.Compare("HighSha1", it.Value, true) == 0)
                            {
                                settings.client.Authentication = Authentication.HighSHA1;
                            }
                            else if (string.Compare("HighSha256", it.Value, true) == 0)
                            {
                                settings.client.Authentication = Authentication.HighSHA256;
                            }
                            else if (string.Compare("HighGMac", it.Value, true) == 0)
                            {
                                settings.client.Authentication = Authentication.HighGMAC;
                            }
                            else if (string.Compare("HighECDSA", it.Value, true) == 0)
                            {
                                settings.client.Authentication = Authentication.HighECDSA;
                            }
                            else
                            {
                                throw new ArgumentException("Invalid Authentication option: '" + it.Value + "'. (None, Low, High, HighMd5, HighSha1, HighGMac, HighSha256)");
                            }
                        }
                        catch (Exception)
                        {
                            throw new ArgumentException("Invalid Authentication option: '" + it.Value + "'. (None, Low, High, HighMd5, HighSha1, HighGMac, HighSha256)");
                        }
                        break;
                    case 'C':
                        try
                        {
                            if (string.Compare("None", it.Value, true) == 0)
                            {
                                settings.client.Ciphering.Security = Security.None;
                            }
                            else if (string.Compare("Authentication", it.Value, true) == 0)
                            {
                                settings.client.Ciphering.Security = Security.Authentication;
                            }
                            else if (string.Compare("Encryption", it.Value, true) == 0)
                            {
                                settings.client.Ciphering.Security = Security.Encryption;
                            }
                            else if (string.Compare("AuthenticationEncryption", it.Value, true) == 0)
                            {
                                settings.client.Ciphering.Security = Security.AuthenticationEncryption;
                            }
                            else
                            {
                                throw new ArgumentException("Invalid Ciphering option '" + it.Value + "'. (None, Authentication, Encryption, AuthenticationEncryption)");
                            }
                        }
                        catch (Exception)
                        {
                            throw new ArgumentException("Invalid Ciphering option '" + it.Value + "'. (None, Authentication, Encryption, AuthenticationEncryption)");
                        }
                        break;
                    case 'V':
                        try
                        {
                            settings.client.Ciphering.SecuritySuite = (SecuritySuite)Enum.Parse(typeof(SecuritySuite), it.Value);
                        }
                        catch (Exception)
                        {
                            throw new ArgumentException("Invalid security suite option '" + it.Value + "'. (Suite0, Suite1, Suite2)");
                        }
                        break;
                    case 'K':
                        try
                        {
                            settings.client.Ciphering.Signing = (Signing)Enum.Parse(typeof(Signing), it.Value);
                            settings.client.ProposedConformance |= Conformance.GeneralProtection;
                        }
                        catch (Exception)
                        {
                            throw new ArgumentException("Invalid security suite option '" + it.Value + "'. (None, OnePassDiffieHellman, StaticUnifiedModel, GeneralSigning)");
                        }
                        break;
                    case 'T':
                        settings.client.Ciphering.SystemTitle = GXCommon.HexToBytes(it.Value);
                        break;
                    case 'M':
                        settings.client.ServerSystemTitle = GXCommon.HexToBytes(it.Value);
                        break;
                    case 'A':
                        settings.client.Ciphering.AuthenticationKey = GXCommon.HexToBytes(it.Value);
                        break;
                    case 'B':
                        settings.client.Ciphering.BlockCipherKey = GXCommon.HexToBytes(it.Value);
                        break;
                    case 'D':
                        settings.client.Ciphering.DedicatedKey = GXCommon.HexToBytes(it.Value);
                        break;
                    case 'F':
                        settings.client.Ciphering.InvocationCounter = UInt32.Parse(it.Value.Trim());
                        break;
                    case 'N':
                        settings.GenerateSecuritySetupLN = it.Value.Trim();
                        break;
                    case 'E':
                        settings.ExportSecuritySetupLN = it.Value.Trim();
                        break;
                    case 'o':
                        settings.outputFile = it.Value;
                        break;
                    case 'd':
                        try
                        {
                            settings.client.Standard = (Standard)Enum.Parse(typeof(Standard), it.Value);
                            if (settings.client.Standard == Standard.Italy || settings.client.Standard == Standard.India || settings.client.Standard == Standard.SaudiArabia)
                            {
                                settings.client.UseUtc2NormalTime = true;
                            }
                        }
                        catch (Exception)
                        {
                            throw new ArgumentException("Invalid DLMS standard option '" + it.Value + "'. (DLMS, India, Italy, SaudiArabia, IDIS)");
                        }
                        break;
                    case 'c':
                        settings.client.ClientAddress = int.Parse(it.Value);
                        break;
                    case 's':
                        if (settings.client.ServerAddress != 1)
                        {
                            settings.client.ServerAddress = GXDLMSClient.GetServerAddress(settings.client.ServerAddress, int.Parse(it.Value));
                        }
                        else
                        {
                            settings.client.ServerAddress = int.Parse(it.Value);
                        }
                        break;
                    case 'l':
                        settings.client.ServerAddress = GXDLMSClient.GetServerAddress(int.Parse(it.Value), settings.client.ServerAddress);
                        break;
                    case 'n':
                        settings.client.ServerAddress = GXDLMSClient.GetServerAddressFromSerialNumber(int.Parse(it.Value), 1);
                        break;
                    case 'm':
                        settings.client.Plc.MacDestinationAddress = UInt16.Parse(it.Value);
                        break;
                    case 'W':
                        settings.client.GbtWindowSize = byte.Parse(it.Value);
                        break;
                    case 'w':
                        settings.client.HdlcSettings.WindowSizeRX = settings.client.HdlcSettings.WindowSizeTX = byte.Parse(it.Value);
                        break;
                    case 'f':
                        settings.client.HdlcSettings.MaxInfoRX = settings.client.HdlcSettings.MaxInfoTX = UInt16.Parse(it.Value);
                        break;
                    case 'L':
                        settings.client.ManufacturerId = it.Value;
                        break;
                    case 'G':
                        tmp = it.Value.Split(':');
                        settings.client.Gateway = new GXDLMSGateway();
                        settings.client.Gateway.NetworkId = byte.Parse(tmp[0]);
                        if (tmp[1].StartsWith("0x"))
                        {
                            settings.client.Gateway.PhysicalDeviceAddress = GXDLMSTranslator.HexToBytes(tmp[1]);
                        }
                        else
                        {
                            settings.client.Gateway.PhysicalDeviceAddress = ASCIIEncoding.ASCII.GetBytes(tmp[1]);
                        }
                        break;
                    case '?':
                        switch (it.Tag)
                        {
                            case 'c':
                                throw new ArgumentException("Missing mandatory client option.");
                            case 's':
                                throw new ArgumentException("Missing mandatory server option.");
                            case 'h':
                                throw new ArgumentException("Missing mandatory host name option.");
                            case 'p':
                                throw new ArgumentException("Missing mandatory port option.");
                            case 'r':
                                throw new ArgumentException("Missing mandatory reference option.");
                            case 'a':
                                throw new ArgumentException("Missing mandatory authentication option.");
                            case 'S':
                                throw new ArgumentException("Missing mandatory Serial port option.");
                            case 't':
                                throw new ArgumentException("Missing mandatory trace option.");
                            case 'g':
                                throw new ArgumentException("Missing mandatory OBIS code option.");
                            case 'C':
                                throw new ArgumentException("Missing mandatory Ciphering option.");
                            case 'v':
                                throw new ArgumentException("Missing mandatory invocation counter logical name option.");
                            case 'T':
                                throw new ArgumentException("Missing mandatory system title option.");
                            case 'A':
                                throw new ArgumentException("Missing mandatory authentication key option.");
                            case 'B':
                                throw new ArgumentException("Missing mandatory block cipher key option.");
                            case 'D':
                                throw new ArgumentException("Missing mandatory dedicated key option.");
                            case 'F':
                                throw new ArgumentException("Missing mandatory frame counter option.");
                            case 'd':
                                throw new ArgumentException("Missing mandatory DLMS standard option.");
                            case 'K':
                                throw new ArgumentException("Missing mandatory key agreement scheme option.");
                            case 'l':
                                throw new ArgumentException("Missing mandatory logical server address option.");
                            case 'm':
                                throw new ArgumentException("Missing mandatory MAC destination address option.");
                            case 'i':
                                throw new ArgumentException("Missing mandatory interface type option.");
                            case 'R':
                                throw new ArgumentException("Missing mandatory logical name of security setup object.");
                            default:
                                ShowHelp();
                                return 1;
                        }
                    default:
                        ShowHelp();
                        return 1;
                }
            }
            if (settings.media == null)
            {
                ShowHelp();
                return 1;
            }
            return 0;
        }

        static void ShowHelp()
        {
            MessageBox.ShowAsync("GuruxDlmsSample reads data from the DLMS/COSEM device.");
            MessageBox.ShowAsync("GuruxDlmsSample -h [Meter IP Address] -p [Meter Port No] -c 16 -s 1 -r SN");
            MessageBox.ShowAsync(" -h \t host name or IP address.");
            MessageBox.ShowAsync(" -p \t port number or name (Example: 1000).");
            MessageBox.ShowAsync(" -q \t MQTT topic.");
            MessageBox.ShowAsync(" -S [COM1:9600:8None1]\t serial port.");
            MessageBox.ShowAsync(" -a \t Authentication (None, Low, High).");
            MessageBox.ShowAsync(" -P \t Password for authentication.");
            MessageBox.ShowAsync(" -c \t Client address. (Default: 16)");
            MessageBox.ShowAsync(" -s \t Server address. (Default: 1)");
            MessageBox.ShowAsync(" -n \t Server address as serial number.");
            MessageBox.ShowAsync(" -l \t Logical Server address.");
            MessageBox.ShowAsync(" -r [sn, ln]\t Short name or Logical Name (default) referencing is used.");
            MessageBox.ShowAsync(" -t [Error, Warning, Info, Verbose] Trace messages.");
            MessageBox.ShowAsync(" -g \"0.0.1.0.0.255:1; 0.0.1.0.0.255:2\" Get selected object(s) with given attribute index.");
            MessageBox.ShowAsync(" -C \t Security Level. (None, Authentication, Encrypted, AuthenticationEncryption)");
            MessageBox.ShowAsync(" -V \t Security Suite version. (Default: Suite0). (Suite0, Suite1 or Suite2)");
            MessageBox.ShowAsync(" -K \t Signing (None, EphemeralUnifiedModel, OnePassDiffieHellman or StaticUnifiedModel, GeneralSigning).");
            MessageBox.ShowAsync(" -v \t Invocation counter data object Logical Name. Ex. 0.0.43.1.1.255");
            MessageBox.ShowAsync(" -I \t Auto increase invoke ID");
            MessageBox.ShowAsync(" -o \t Cache association view to make reading faster. Ex. -o C:\\device.xml");
            MessageBox.ShowAsync(" -T \t System title that is used with chiphering. Ex -T 4775727578313233");
            MessageBox.ShowAsync(" -M \t Meter system title that is used with chiphering. Ex -T 4775727578313233");
            MessageBox.ShowAsync(" -A \t Authentication key that is used with chiphering. Ex -A D0D1D2D3D4D5D6D7D8D9DADBDCDDDEDF");
            MessageBox.ShowAsync(" -B \t Block cipher key that is used with chiphering. Ex -B 000102030405060708090A0B0C0D0E0F");
            MessageBox.ShowAsync(" -D \t Dedicated key that is used with chiphering. Ex -D 00112233445566778899AABBCCDDEEFF");
            MessageBox.ShowAsync(" -F \t Initial Frame Counter (Invocation counter) value.");
            MessageBox.ShowAsync(" -d \t Used DLMS standard. Ex -d India (DLMS, India, Italy, SaudiArabia, IDIS)");
            MessageBox.ShowAsync(" -E \t Export client and server certificates from the meter. Ex. -E 0.0.43.0.0.255.");
            MessageBox.ShowAsync(" -N \t Generate new client and server certificates and import them to the server. Ex. -R 0.0.43.0.0.255.");
            MessageBox.ShowAsync(" -G \t Use Gateway with given NetworkId and PhysicalDeviceAddress. Ex -G 0:1.");
            MessageBox.ShowAsync(" -i \t Used communication interface. Ex. -i WRAPPER.");
            MessageBox.ShowAsync(" -m \t Used PLC MAC address. Ex. -m 1.");
            MessageBox.ShowAsync(" -G \t Gateway settings NetworkId:PhysicalDeviceAddress. Ex -G 1:12345678");
            MessageBox.ShowAsync(" -W \t General Block Transfer window size.");
            MessageBox.ShowAsync(" -w \t HDLC Window size. Default is 1");
            MessageBox.ShowAsync(" -f \t HDLC Frame size. Default is 128");
            MessageBox.ShowAsync(" -L \t Manufacturer ID (Flag ID) is used to use manufacturer depending functionality. -L LGZ");
            MessageBox.ShowAsync("Example:");
            MessageBox.ShowAsync("Read LG device using TCP/IP connection.");
            MessageBox.ShowAsync("GuruxDlmsSample -r SN -c 16 -s 1 -h [Meter IP Address] -p [Meter Port No]");
            MessageBox.ShowAsync("Read LG device using serial port connection.");
            MessageBox.ShowAsync("GuruxDlmsSample -r SN -c 16 -s 1 -sp COM1");
            MessageBox.ShowAsync("Read Indian device using serial port connection.");
            MessageBox.ShowAsync("GuruxDlmsSample -S COM1 -c 16 -s 1 -a Low -P [password]");
            MessageBox.ShowAsync("Read MQTT device -h [Broker address] -q [Topic/meterId]");
        }
    }
}
