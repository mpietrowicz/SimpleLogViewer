using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using static Avalonia.Threading.Dispatcher;

namespace SimpleLogViewer.ViewModels
{
   

    [DataContract]
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ObservableCollection<Event> _allMessages;

        [DataMember]
        [ObservableProperty]
        public int _listenPort = 7072;




        public MainWindowViewModel()
        {
            this._allMessages = new ObservableCollection<Event>();
            //Startup();
       
            var udpClient = new UdpClient("127.0.0.1",_listenPort);


            var s = new UdpState()
            {
                Client = udpClient,
            };
            Task.Run(async () =>
            {
                udpClient.BeginReceive(ReceiveCallback, s);
                do
                {
                    await  Task.Delay(100);

                } while (true);

            });

        }

        public bool CompleteReceive = false;
        private async void ReceiveCallback(IAsyncResult ar)
        {
            CompleteReceive = false;
            if (ar.AsyncState is not UdpState state)
            {
                CompleteReceive = true;
                return;
            }

            //UdpClient u = ((UdpState)(ar.AsyncState)).Client;
            //IPEndPoint e = ((UdpState)(ar.AsyncState)).IP;

            var receivedResult = await state.Client.ReceiveAsync();
            var data = Encoding.ASCII.GetString(receivedResult.Buffer);

            XmlSerializer serializer = new XmlSerializer(typeof(EventLog4J));
            using StringReader reader = new StringReader(data);
            var test = serializer.Deserialize(reader) as EventLog4J;
            if (test != null)
            {
                await UIThread.InvokeAsync(() =>
                {
                    _allMessages.Add(new Event(test));
                    if (_allMessages.Count() > 1000)
                    {
                        _allMessages =
                            new ObservableCollection<Event>(_allMessages.OrderBy(x => x.Timestamp));

                        foreach (var @event in _allMessages.Take(100))
                        {
                            _allMessages.Remove(@event);
                        }
                    }
                });

                //  OnPropertyChanged(nameof(_allMessages));
            }
            state.Client.BeginReceive(ReceiveCallback, ar);
        }

        public void Startup()
        {
            Task.Run(async () =>
            {

                using var udpClient = new UdpClient(_listenPort);

                while (true)
                {
                    try
                    {
                        var receivedResult = await udpClient.ReceiveAsync();
                        var data = Encoding.ASCII.GetString(receivedResult.Buffer);

                        XmlSerializer serializer = new XmlSerializer(typeof(EventLog4J));
                        using StringReader reader = new StringReader(data);
                        var test = serializer.Deserialize(reader) as EventLog4J;
                        if (test != null)
                        {
                            await UIThread.InvokeAsync(() =>
                             {
                                 _allMessages.Add(new Event(test));
                                 if (_allMessages.Count() > 1000)
                                 {
                                     _allMessages =
                                         new ObservableCollection<Event>(_allMessages.OrderBy(x => x.Timestamp));

                                     foreach (var @event in _allMessages.Take(100))
                                     {
                                         _allMessages.Remove(@event);
                                     }
                                 }
                             });

                            //  OnPropertyChanged(nameof(_allMessages));
                        }
                    }
                    catch (Exception e)
                    {
                        throw;
                    }


                }
            });
        }
    }
}