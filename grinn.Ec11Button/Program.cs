// See https://aka.ms/new-console-template for more information

using System.Device.Gpio;
using System.Device.Spi;
using grinn.Ec11Button;
using grinn.Ec11Button.Hardware;
using Iot.Device.Spi;
using SkiaSharp;

Console.WriteLine("Hello, World!");

const int encoderPinA = 17;
const int encoderPinB = 18;
const int encoderPinC = 27;

const int displayPinCS = 7;
const int displayPinSCK = 11;
const int displayPinMOSI = 10;
const int displayPinDC = 9;
const int displayPinBL = 19;

var gpioController = new GpioController();

// My single button code. This should move somewhere smarter once we start orchestrating our gui+controls
var rotaryButton = new CustomRotaryEncoder(encoderPinA, encoderPinB, encoderPinC, 20, gpioController);

// Init MPD connection, this is going to fail fast if it can't connect. Which is fine with me. 
var mpdConnection = new MpdSocketConnection("/var/run/mpd/socket");
await mpdConnection.Connect();

// Display tests
var spiConfig = new SpiConnectionSettings(0, 0);
spiConfig.ClockFrequency = 10_000_000; // 10mhz

var d = SpiDevice.Create(spiConfig);

// write reset command
gpioController.Write(displayPinDC, PinValue.Low);;
d.WriteByte(0x01);

gpioController.Write(displayPinDC, PinValue.Low);;
d.WriteByte(0x36); 
gpioController.Write(displayPinDC, PinValue.High);;
d.WriteByte(0x70);

gpioController.Write(displayPinDC, PinValue.Low);;
d.WriteByte(0x36); // Frame rate ctrl - idle mode

gpioController.Write(displayPinDC, PinValue.High);;
d.WriteByte(0x0C);
d.WriteByte(0x0C);
d.WriteByte(0x00);
d.WriteByte(0x33);
d.WriteByte(0x33);

gpioController.Write(displayPinDC, PinValue.Low);;
d.WriteByte(0x3A);

gpioController.Write(displayPinDC, PinValue.High);;
d.WriteByte(0x05);

gpioController.Write(displayPinDC, PinValue.Low);;
d.WriteByte(0xB7);
gpioController.Write(displayPinDC, PinValue.High);;
d.WriteByte(0x14);

gpioController.Write(displayPinDC, PinValue.Low);;
d.WriteByte(0xBB);
gpioController.Write(displayPinDC, PinValue.High);;
d.WriteByte(0x37);
     
gpioController.Write(displayPinDC, PinValue.Low);;
d.WriteByte(0xC0); // Power control
gpioController.Write(displayPinDC, PinValue.High);;
d.WriteByte(0x2C);

gpioController.Write(displayPinDC, PinValue.Low);;
d.WriteByte(0xC2); // Power control
gpioController.Write(displayPinDC, PinValue.High);;
d.WriteByte(0x01);

gpioController.Write(displayPinDC, PinValue.Low);;
d.WriteByte(0xC3); // Power control
gpioController.Write(displayPinDC, PinValue.High);;
d.WriteByte(0x12);

gpioController.Write(displayPinDC, PinValue.Low);;
d.WriteByte(0xC4); // Power control
gpioController.Write(displayPinDC, PinValue.High);;
d.WriteByte(0x20);

gpioController.Write(displayPinDC, PinValue.Low);;
d.WriteByte(0xD0);
gpioController.Write(displayPinDC, PinValue.High);;
d.WriteByte(0xA4);
d.WriteByte(0xA1);


/*
 *         SendCommand(SWRESET); // Software reset
        Task.Delay(TimeSpan.FromMilliseconds(150));
        
        SendCommand(MADCTL); 
        SendData((byte)0x70);
 */

/*var spiDevice = new SoftwareSpi(displayPinSCK, -1, displayPinMOSI, displayPinCS, spiConfig, gpioController);
var display = new St7789(240, 240, displayPinDC, displayPinBL, gpioController, d);

using var bitmap = SKBitmap.Decode(@"cat.jpg");
var image = SKImage.FromBitmap(bitmap);

display.SetWindows();*/
//display.Display(image);

await Task.Delay(int.MaxValue);