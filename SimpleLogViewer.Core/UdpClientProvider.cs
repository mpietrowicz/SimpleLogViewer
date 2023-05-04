using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SimpleLogViewer.Core;

public class UdpClientProvider : IDisposable
{
    private readonly string _ip;
    private readonly int _port;
    private UdpClient _udpClient;
    private bool _running = false;

    public UdpClientProvider(string ip, int port)
    {
        _ip = ip;
        _port = port;
    }

    public void Listen()
    {
        _udpClient = new UdpClient(_ip, _port);
        _running = true;
        MainTask = Task.Run(StartListening);
    }

    private async Task StartListening()
    {
        _udpClient.BeginReceive(ReceiveCallback, new UdpState() {Client = _udpClient});
        do
        {
            await Task.Delay(10);
        } while (_running);

        await Task.CompletedTask;
    }

    private async void ReceiveCallback(IAsyncResult ar)
    {
        if (ar.AsyncState is not UdpState state)
        {
            return;
        }
        var receivedResult = await state.Client.ReceiveAsync();
        var data = Encoding.ASCII.GetString(receivedResult.Buffer);
        
        _udpClient.BeginReceive(ReceiveCallback, new UdpState() {Client = _udpClient});
    }

    public Task MainTask { get; set; }

    public void Stop()
    {
        _running = false;
        _udpClient.Dispose();
    }

    public void Dispose()
    {
        Stop();
    }
}