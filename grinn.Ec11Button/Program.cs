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
var mpdEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6600);
var tcpClient = new TcpClient();
await tcpClient.ConnectAsync(mpdEndpoint);

var connectionStream = tcpClient.GetStream();
var reader = new StreamReader(connectionStream, Encoding.ASCII);
var writer = new StreamWriter(connectionStream, Encoding.ASCII) { NewLine = "\n"};

var isConnected = reader.ReadLine();
var version = isConnected.Substring(7);

Console.Write(version);

await Task.Delay(Timeout.Infinite);

void HandleClick(object? sender, bool args)
{
    Console.WriteLine($"{nameof(HandleClick)} - click event arrived in main program.");
}

void HandleEncoderChange(object? sender, RotaryEncoderDirectionArgs args)
{
    Console.WriteLine($"{nameof(HandleEncoderChange)} - {args.Value}");
}