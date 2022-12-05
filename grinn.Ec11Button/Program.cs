// See https://aka.ms/new-console-template for more information

using grinn.Ec11Button;
using grinn.Ec11Button.MpdCommands;

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
var mpdConnection = new MpdSocketConnection("/var/run/mpd/socket");

var playlistInfo = await mpdConnection.SendCommandToSocket(new GetQueueInfo());

foreach (var playListItem in playlistInfo)
{
    Console.WriteLine(playListItem.ToString());
}

void HandleClick(object? sender, bool args)
{
    Console.WriteLine($"{nameof(HandleClick)} - click event arrived in main program.");
}

void HandleEncoderChange(object? sender, RotaryEncoderDirectionArgs args)
{
    Console.WriteLine($"{nameof(HandleEncoderChange)} - {args.Value}");
}