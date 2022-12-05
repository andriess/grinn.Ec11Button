using System.Net.Sockets;
using System.Text;
using grinn.Ec11Button.MpdCommands;

namespace grinn.Ec11Button;

public class MpdSocketConnection
{
    private static readonly byte[] ReceivedBytes = new byte[2048];
    private static readonly char[] ReceivedChars = new char[2048];

    private readonly Socket _socket;
    private readonly UnixDomainSocketEndPoint _endpoint;

    public MpdSocketConnection(string socketPath)
    {
        _socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
        _endpoint = new UnixDomainSocketEndPoint(socketPath);
    }

    public async Task Connect()
    {
        await _socket.ConnectAsync(_endpoint);
        var connectionResponse = await ReceiveResponseFromSocket();

        if (!connectionResponse.StartsWith("OK MPD "))
        {
            throw new Exception($"Could not connect to MPD. Response: {connectionResponse}");
        }
    }
    
    public async Task<IList<T>> SendCommandToSocket<T>(IMpdCommand<T> command)
    {
        if (!_socket.Connected)
            await Connect();
        
        var requestBytes = Encoding.ASCII.GetBytes($"{command.CommandName}\n");
        var bytesSend = 0;
        while (bytesSend < requestBytes.Length)
        {
            bytesSend += await _socket.SendAsync(requestBytes.AsMemory(bytesSend), SocketFlags.None);
        }
        var responseString = await ReceiveResponseFromSocket();

        return command.ParseCommandResponse(responseString);
    }
    
    private async Task<string> ReceiveResponseFromSocket()
    {
        Array.Clear(ReceivedBytes);
        Array.Clear(ReceivedChars);

        var bytesReceived = await _socket.ReceiveAsync(ReceivedBytes, SocketFlags.None);
    
        Console.WriteLine($"{nameof(ReceiveResponseFromSocket)} - Bytes received: {bytesReceived}");
        // Convert byteCount bytes to ASCII characters using the 'responseChars' buffer as destination
        Encoding.ASCII.GetChars(ReceivedBytes, 0, bytesReceived, ReceivedChars, 0);

        var responseString = new string(ReceivedChars);
        Console.WriteLine($"{nameof(ReceiveResponseFromSocket)} - Response string: {responseString}");

        return responseString;
    }
}