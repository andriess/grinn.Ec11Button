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
var bytesReceived = await socket.ReceiveAsync(receivedBytes, SocketFlags.None);

if (bytesReceived != 0)
{
    // Convert byteCount bytes to ASCII characters using the 'responseChars' buffer as destination
    int charCount = Encoding.ASCII.GetChars(receivedBytes, 0, bytesReceived, receivedChars, 0);

    // Print the contents of the 'responseChars' buffer to Console.Out
    await Console.Out.WriteAsync(receivedChars.AsMemory(0, charCount));
}

/*var mpdEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6600);
var tcpClient = new TcpClient();
await tcpClient.ConnectAsync(mpdEndpoint);

var connectionStream = tcpClient.GetStream();
var reader = new StreamReader(connectionStream, Encoding.ASCII);
var writer = new StreamWriter(connectionStream, Encoding.ASCII) { NewLine = "\n"};

var isConnected = reader.ReadLine();
var version = isConnected?[7..];*/

await Task.Delay(Timeout.Infinite);

void HandleClick(object? sender, bool args)
{
    Console.WriteLine($"{nameof(HandleClick)} - click event arrived in main program.");
}

void HandleEncoderChange(object? sender, RotaryEncoderDirectionArgs args)
{
    Console.WriteLine($"{nameof(HandleEncoderChange)} - {args.Value}");
}