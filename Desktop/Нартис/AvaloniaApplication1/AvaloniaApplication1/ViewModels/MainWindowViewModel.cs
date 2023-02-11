using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reactive;
using CommunityToolkit.Mvvm.Input;
using AvaloniaApplication1.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Gurux.Serial;
using Gurux.DLMS;
using Gurux.Common;
using System.Diagnostics;
using ThingLing.Controls;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Gurux.DLMS.Objects;
using Gurux.DLMS.Enums;
using AvaloniaApplication1.DLMSObjects;
using LiveCharts.Helpers;
using CommonDlmsModels;
using Gurux.Net;
using Gurux.DLMS.Reader;
using System.Reflection.PortableExecutable;

//для графики-вектора
//using LiveCharts;
//using LiveChartsCore.SkiaSharpView;
//using LiveChartsCore;
//using JetBrains.Annotations;
//using LiveCharts.Definitions.Series;

namespace AvaloniaApplication1.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public string connectStatus { get; set; } = "конфигуратор запущен, ожидается старт подключения";
        public static string connectInfoLog { get; set; } = string.Empty;
        public AsyncRelayCommand tryOptoConnect { get; set; }
        public AsyncRelayCommand showInfoAboutProg { get; set; }
        public bool isPressed { get; set; } = false;

        public event PropertyChangedEventHandler? PropertyChanged;
        public async System.Threading.Tasks.Task OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


        public MainWindowViewModel()
        {

            tryOptoConnect = new AsyncRelayCommand(tryOptoConnectFunc);//new RelayCommand(tryOptoConnectFunc);
            showInfoAboutProg = new AsyncRelayCommand(showInfoAboutProgFunc);

        }
        private async System.Threading.Tasks.Task tryOptoConnectFunc()
        {
            Gurux.DLMS.Reader.GXDLMSReader reader = null;
            try
            { 
            //var gxSerial1 = new Gurux.Serial.GXSerial("COM3"); //определяем последовательный порт 3 (оптопорт)
            //string[] ports = Gurux.Serial.GXSerial.GetPortNames();
            //MessageBox.ShowAsync(string.Join(",",ports));
            string receivedText = "НИЧЕГО НЕ ПРИНЯЛ";


            isPressed = true;
            connectStatus = "подключение...";
            await OnPropertyChanged(nameof(connectStatus));
            await OnPropertyChanged(nameof(isPressed));

            ////new Task(() =>
            ////{
            ////    int i = 0;
            ////    while (true)
            ////    {
            ////        MessageBox.ShowAsync("{i}");
            ////        i++;
            ////    }
            ////}).Start();

            //ReceiveParameters<byte[]> receiveParameters = new ReceiveParameters<byte[]>()//нужно указать параметры приема
            //{
            //    WaitTime = 5000,
            //    Count = 15 //сколько байт собираемся принять
            //};

            ////открываем порт
            //try
            //{
            //    gxSerial1.Open();
            //    gxSerial1.DtrEnable = gxSerial1.RtsEnable = true;
            //}
            //catch (Exception ex) { await MessageBox.ShowAsync(ex.Message); }

            ////пробуем что-то передать
            //byte[] bytesToSend = { 0x7e, 0xa0, 0x15, 0x02, 0x23, 0x61, 0x93, 0xef, 0xea, 0x81, 0x80, 0x08, 0x05, 0x02, 0x01, 0x00, 0x06, 0x02, 0x01, 0x00, 0x56, 0x96, 0x7e };//7e a0 15 02 23 61 93 ef ea 81 80 08 05 02 01 00 06 02 01 00 56 96 7e    - подключение к НАРТИС 100 кайфа


            ////Task.Factory.StartNew(() => { gxSerial1.Send(bytesToSend); });//асинх
            //gxSerial1.Send(bytesToSend);//не асинхрон 

            //пытаемся в рамках гурукс клиента манипулировать соединением
            // попробуем с реализованным высокоуровневым клиентом все сделать GXDLMSClient client = new GXDLMSClient();
            //var client = new ClientDLMS(null);//TODO - настроить клиента перед подключением
            //var responce = client.ReadData<byte>("0.0.96.5.3.255", new CancellationToken());
            Settings settings = new Settings();
            var client = new GXDLMSClient();
            var args = new string[] { "-S", "COM3", "-c", "16", "-s", "1", "-a", "High", "-P", "12345" };// сами жестко задаем параметры при старте
            int ret = Settings.GetParameters(args, settings);
            reader = new Gurux.DLMS.Reader.GXDLMSReader(settings.client, settings.media, settings.trace, settings.invocationCounter);
            reader.OnNotification += (data) =>
            {
                MessageBox.ShowAsync(data?.ToString());
            };
            try
            {
                settings.media.Open();
            }
            catch (System.IO.IOException ex)
            {
                MessageBox.ShowAsync("----------------------------------------------------------");
                MessageBox.ShowAsync(ex.Message);
                MessageBox.ShowAsync("Available ports:");
                MessageBox.ShowAsync(string.Join(" ", GXSerial.GetPortNames()));
            }
            //Some meters need a break here.
            Thread.Sleep(1000);
            //Export client and server certificates from the meter.
            if (!string.IsNullOrEmpty(settings.ExportSecuritySetupLN))
            {
                //реализован в другом потоке
                new System.Threading.Tasks.Task(() => { reader.ExportMeterCertificates(settings.ExportSecuritySetupLN); }).Start();
                    
            }
            //Generate new client and server certificates and import them to the server.
            else if (!string.IsNullOrEmpty(settings.GenerateSecuritySetupLN))
            {
                    new System.Threading.Tasks.Task(() =>
                    {
                        reader.GenerateCertificates(settings.GenerateSecuritySetupLN);
                    }).Start();
            }
            else if (settings.readObjects.Count != 0)
            {
                bool read = false;
                if (settings.outputFile != null)
                {
                    try
                    {
                        settings.client.Objects.Clear();
                        settings.client.Objects.AddRange(GXDLMSObjectCollection.Load(settings.outputFile));
                        read = true;
                    }
                    catch (Exception)
                    {
                        //It's OK if this fails.
                    }
                }
                new System.Threading.Tasks.Task(() =>
                {
                  reader.InitializeConnection();
                }).Start();
                    
                if (!read)
                {
                    reader.GetAssociationView(settings.outputFile);
                }
                foreach (KeyValuePair<string, int> it in settings.readObjects)
                {
                    object val = reader.Read(settings.client.Objects.FindByLN(ObjectType.None, it.Key), it.Value);
                    reader.ShowValue(val, it.Value);
                }
                if (settings.outputFile != null)
                {
                    try
                    {
                        settings.client.Objects.Save(settings.outputFile, new GXXmlWriterSettings() { UseMeterTime = true, IgnoreDefaultValues = false });
                    }
                    catch (Exception)
                    {
                        //It's OK if this fails.
                    }
                }
            }
            else
            {
                    //new System.Threading.Tasks.Task(() =>
                    //{
                    //    reader.ReadAll(settings.outputFile);
                    //}).Start();
                    reader.ReadAll(settings.outputFile); //эта функция тратит больше всего времени
            }
        }
            catch(Exception ex) { MessageBox.ShowAsync(ex.Message); }
            finally 
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    MessageBox.ShowAsync("Ended. Press any key to continue.");
                }
            }



            //client.UseLogicalNameReferencing = true;
            //client.InterfaceType = InterfaceType.HDLC;
            //client.ClientAddress = 0x30;
            //client.ServerAddress = 0x11;
            //GXReplyData reply = new GXReplyData();
            //byte[] data;
            //data = client.SNRMRequest();
            //if (data != null)
            //{
            //    ReadDLMSPacket(data, reply,client);
            //    //Has server accepted client.
            //    client.ParseUAResponse(reply.Data);
            //}
            ////Some meters need a break here.
            //Thread.Sleep(1000);
            ////Export client and server certificates from the meter.
            //if (!string.IsNullOrEmpty(settings.ExportSecuritySetupLN))
            //{
            //    reader.ExportMeterCertificates(settings.ExportSecuritySetupLN);
            //}
            ////Generate new client and server certificates and import them to the server.
            //else if (!string.IsNullOrEmpty(settings.GenerateSecuritySetupLN))
            //{
            //    reader.GenerateCertificates(settings.GenerateSecuritySetupLN);
            //}
            //else if (settings.readObjects.Count != 0)
            //{
            //    bool read = false;
            //    if (settings.outputFile != null)
            //    {
            //        try
            //        {
            //            settings.client.Objects.Clear();
            //            settings.client.Objects.AddRange(GXDLMSObjectCollection.Load(settings.outputFile));
            //            read = true;
            //        }
            //        catch (Exception)
            //        {
            //            //It's OK if this fails.
            //        }
            //    }
            //    reader.InitializeConnection();
            //    if (!read)
            //    {
            //        reader.GetAssociationView(settings.outputFile);
            //    }
            //    foreach (KeyValuePair<string, int> it in settings.readObjects)
            //    {
            //        object val = reader.Read(settings.client.Objects.FindByLN(ObjectType.None, it.Key), it.Value);
            //        reader.ShowValue(val, it.Value);
            //    }
            //    if (settings.outputFile != null)
            //    {
            //        try
            //        {
            //            settings.client.Objects.Save(settings.outputFile, new GXXmlWriterSettings() { UseMeterTime = true, IgnoreDefaultValues = false });
            //        }
            //        catch (Exception)
            //        {
            //            //It's OK if this fails.
            //        }
            //    }
            //}
            //else
            //{
            //    reader.ReadAll(settings.outputFile);
            //}




            ////попробуем что-то получить
            //if (gxSerial1.Receive(receiveParameters))
            //{
            //    receivedText = "";
            //    foreach (var a in receiveParameters.Reply)
            //    {
            //        receivedText += string.Format("{0:X}", a);
            //        receivedText += ",";
            //    }
            //    //receivedText = String.Join(",", receiveParameters.Reply);
            //    //receivedText = GXCommon.ToHex(receiveParameters.Reply, true);
            //}

            //MessageBox.ShowAsync();



            //if (gxSerial1.IsOpen)
            //{
            //    gxSerial1_OnMediaStateChange(this, new MediaStateEventArgs(MediaState.Open));
            //}
            //else
            //{
            //    gxSerial1_OnMediaStateChange(this, new MediaStateEventArgs(MediaState.Closed));
            //}


            //connectInfoLog = receivedText;

            //connectInfoLog = "Fdsfsdfs";

            OnPropertyChanged(nameof(connectInfoLog));

        }

        public async System.Threading.Tasks.Task showInfoAboutProgFunc()
        {
             MessageBox.ShowAsync("info about prog");
        }


        /*
        public GXDLMSSettings Settings
        {
            get;
            private set;
        }
        */

        /*
        public GXDLMSObjectCollection Objects
        {
            get
            {
                return Settings.Objects;
            }
        }
        */


        //GXDLMSClient
        //public object GetAtrData(GXDLMSObject obj, int index, CancellationToken token)
        //{
        //    var requset = Read(obj, index);
        //    GXReplyData reply = BaseRead(requset, token);
        //    return UpdateValue(obj, index, reply.Value);
        //}

        //public object GetUpdatedValue(MeterObject obj, CancellationToken token)
        //{
        //    var dlmsObject = Objects.FindByLN(obj.Type, obj.ObisCode);
        //    if (dlmsObject == null)
        //    {
        //        throw new Exception("can't find object with this obis"); //CanNotFindObjectWithOBIS(obj.ObisCode);
        //    }
        //    return GetAtrData(dlmsObject, obj.AttributeIndex, token);
        //}


    }


}
