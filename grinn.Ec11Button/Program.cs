// See https://aka.ms/new-console-template for more information

using System.Device.Spi;

Console.WriteLine("Hello, World!");

const int encoderPinA = 17;
const int encoderPinB = 18;
const int encoderPinC = 27;

const int displayPinCS = 7;
const int displayPinSCK = 11;
const int displayPinMOSI = 10;
const int displayPinDC = 9;
const int displayPinBL = 19;

/*var gpioController = new GpioController();

// My single button code. This should move somewhere smarter once we start orchestrating our gui+controls
var rotaryButton = new CustomRotaryEncoder(encoderPinA, encoderPinB, encoderPinC, 20, gpioController);

// Init MPD connection, this is going to fail fast if it can't connect. Which is fine with me. 
var mpdConnection = new MpdSocketConnection("/var/run/mpd/socket");
await mpdConnection.Connect();*/

// Display tests
var spiConfig = new SpiConnectionSettings(0, 0)
{
    ClockFrequency = 100_000,
    Mode = SpiMode.Mode3
};

var spidevice = SpiDevice.Create(spiConfig);
var bus0 = SpiDevice.GetBusInfo(0);
var bus1 = SpiDevice.GetBusInfo(1);

Console.WriteLine($"{nameof(bus0.MaxClockFrequency)}: {bus0.MaxClockFrequency}");
Console.WriteLine($"{nameof(bus0.MinClockFrequency)}: {bus0.MinClockFrequency}");

Console.WriteLine($"{nameof(bus1.MaxClockFrequency)}: {bus1.MaxClockFrequency}");
Console.WriteLine($"{nameof(bus1.MinClockFrequency)}: {bus1.MinClockFrequency}");


/*
var display = new St7789(240, 240, displayPinDC, displayPinBL, gpioController, d);

using var bitmap = SKBitmap.Decode(@"cat.jpg");
var image = SKImage.FromBitmap(bitmap);
*/
//display.Display(image);

await Task.Delay(int.MaxValue);