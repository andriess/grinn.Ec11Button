// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;
using System.Text;
using grinn.Ec11Button;

Console.WriteLine("Hello, World!");

const int encoderPinA = 17;
const int encoderPinB = 18;
const int encoderPinC = 27;
// My single button code. This should move somewhere smarter once we start orchestrating our gui+controls
var rotaryButton = new CustomRotaryEncoder(encoderPinA, encoderPinB, encoderPinC, 20);
rotaryButton.Debounce = TimeSpan.FromMilliseconds(175);
rotaryButton.OnEncoderChange += HandleEncoderChange;
rotaryButton.OnClick += HandleClick;


// Trying some MPD stuff here: 
const string unixSocketPath = "/var/run/mpd/socket";
using var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
var endpoint = new UnixDomainSocketEndPoint(unixSocketPath);
await socket.ConnectAsync(endpoint);

var receivedBytes = new byte[256];
var receivedChars = new char[256];

var onConnectedResponse = await ReceiveResponseFromSocket(socket, receivedBytes, receivedChars);
Console.WriteLine(onConnectedResponse);

SendCommandToSocket(socket, "ping");

var onPingResponse = await ReceiveResponseFromSocket(socket, receivedBytes, receivedChars);
Console.WriteLine(onPingResponse);

static async Task<string> ReceiveResponseFromSocket(Socket socket, byte[] receivedBytes, char[] receivedChars)
{
    #region Validate Arguments
        if (socket == null)
            throw new ArgumentNullException(nameof(socket), "Socket cannot be null.");
    #endregion
    
    var bytesReceived = await socket.ReceiveAsync(receivedBytes, SocketFlags.None);

    // Convert byteCount bytes to ASCII characters using the 'responseChars' buffer as destination
    Encoding.ASCII.GetChars(receivedBytes, 0, bytesReceived, receivedChars, 0);

    return new string(receivedChars);
}

static void SendCommandToSocket(Socket socket, string command)
{
    #region Validate Arguments
        if (string.IsNullOrEmpty(command))
            throw new ArgumentNullException(nameof(command), "Command cannot be null or empty.");
        if (socket == null)
            throw new ArgumentNullException(nameof(socket), "Socket cannot be null.");
    #endregion
    
    var requestBytes = Encoding.ASCII.GetBytes(command);
    var bytesSend = 0;
    while (bytesSend < requestBytes.Length)
    {
        bytesSend += socket.Send(requestBytes, bytesSend, requestBytes.Length - bytesSend, SocketFlags.None);
    }
}

void HandleClick(object? sender, bool args)
{
    Console.WriteLine($"{nameof(HandleClick)} - click event arrived in main program.");
}

void HandleEncoderChange(object? sender, RotaryEncoderDirectionArgs args)
{
    Console.WriteLine($"{nameof(HandleEncoderChange)} - {args.Value}");
}